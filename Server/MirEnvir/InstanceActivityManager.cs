using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.MirDatabase;
using Server.MirObjects;
using S = ServerPackets;

namespace Server.MirEnvir
{
    public class InstanceActivityInstance
    {
        public InstanceActivityInfo Template;
        public Map Map;
        public List<PlayerObject> Players = new List<PlayerObject>();
        public List<MonsterObject> Monsters = new List<MonsterObject>(); // 副本内普通怪
        public MonsterObject Boss;                                        // 副本 BOSS
        public int CurrentWave;                                           // 当前已开始的波次（从 0 开始计数）
        public DateTime StartTime;
        public bool Completed;
        public int Score;
        public DateTime? EndTime;
        public int InstanceId;
        public string TimerKey;
        public DateTime? ExitTime;
    }

    public class InstanceActivityManager
    {
        private readonly Envir _envir;
        private readonly List<InstanceActivityInstance> _instances = new List<InstanceActivityInstance>();
        private readonly Dictionary<InstanceActivityInfo, List<NPCObject>> _entranceNpcs = new Dictionary<InstanceActivityInfo, List<NPCObject>>();
        private readonly Dictionary<uint, InstanceActivityInfo> _entranceNpcLookup = new Dictionary<uint, InstanceActivityInfo>();
        private readonly HashSet<InstanceActivityInfo> _loggedConfigIssues = new HashSet<InstanceActivityInfo>();
        private bool _loggedInit;
        private long _nextScheduleTime;
        private int _nextInstanceId = 1;
        // key: 活动Index, value: (key: 角色Index, value: (日期, 当日进入次数))
        private readonly Dictionary<int, Dictionary<int, (DateTime Date, int Count)>> _enterCounts
            = new Dictionary<int, Dictionary<int, (DateTime Date, int Count)>>();

        // 通关后在副本内停留的时间（秒），然后自动传送出本（服务端 Settings 控制）
        private int ExitDelaySeconds => Math.Max(0, Settings.InstanceActivityExitDelaySeconds);

        private int GetExitDelaySeconds(InstanceActivityInfo info)
        {
            if (info != null && info.ExitDelaySeconds > 0)
                return info.ExitDelaySeconds;

            return ExitDelaySeconds;
        }

        private int GetMonsterWaveCount(InstanceActivityInfo info)
        {
            if (info == null) return 1;
            return Math.Max(1, info.MonsterWaveCount);
        }

        private static string BuildInstanceTimerKey(int instanceId) => $"IA_{instanceId}";

        private void SendOrUpdateInstanceTimer(PlayerObject player, InstanceActivityInstance instance)
        {
            if (player == null || instance?.Template == null) return;
            if (instance.Template.InstanceTimeLimitSeconds <= 0) return;

            var elapsed = (int)Math.Max(0, (_envir.Now - instance.StartTime).TotalSeconds);
            var remaining = instance.Template.InstanceTimeLimitSeconds - elapsed;
            if (remaining < 0) remaining = 0;

            player.Enqueue(new S.SetTimer
            {
                Key = instance.TimerKey,
                Type = 1,
                Seconds = remaining
            });
        }

        private void ExpireInstanceTimer(PlayerObject player, InstanceActivityInstance instance)
        {
            if (player == null || instance == null) return;
            if (string.IsNullOrEmpty(instance.TimerKey)) return;

            player.Enqueue(new S.ExpireTimer { Key = instance.TimerKey });
        }

        private string BuildExitTimerKey(InstanceActivityInstance instance)
        {
            return instance == null ? null : instance.TimerKey + "_exit";
        }

        private void SendExitTimer(PlayerObject player, InstanceActivityInstance instance, int seconds)
        {
            if (player == null || instance == null) return;
            var key = BuildExitTimerKey(instance);
            if (string.IsNullOrEmpty(key)) return;

            player.Enqueue(new S.SetTimer
            {
                Key = key,
                Type = 2,
                Seconds = seconds
            });
        }

        private void ExpireExitTimer(PlayerObject player, InstanceActivityInstance instance)
        {
            if (player == null || instance == null) return;
            var key = BuildExitTimerKey(instance);
            if (string.IsNullOrEmpty(key)) return;

            player.Enqueue(new S.ExpireTimer { Key = key });
        }

        public InstanceActivityManager(Envir envir)
        {
            _envir = envir;
            MessageQueue.Instance.Enqueue("[InstanceActivity] 管理器已创建");
        }

        public void Process()
        {
            try
            {
                if (_envir.Time < _nextScheduleTime) return;

                _nextScheduleTime = _envir.Time + 1000;

                var now = _envir.Now;

                if (!_loggedInit)
                {
                    _loggedInit = true;
                    MessageQueue.Instance.Enqueue($"[InstanceActivity] 当前活动数量: {_envir.InstanceActivityList.Count}");
                }

                // 防止枚举期间列表被修改导致异常
                var activitySnapshot = _envir.InstanceActivityList.ToList();
                foreach (var info in activitySnapshot)
                {
                    if (info == null || !info.Enabled)
                        continue;

                    if ((info.TimeSlots == null || info.TimeSlots.Count == 0) && !_loggedConfigIssues.Contains(info))
                    {
                        MessageQueue.Instance.Enqueue($"[InstanceActivity] 活动未启动: {info.Name} 没有配置时间段");
                        _loggedConfigIssues.Add(info);
                        continue;
                    }

                    if ((info.EntranceMapIndices == null || info.EntranceMapIndices.Count == 0) && !_loggedConfigIssues.Contains(info))
                    {
                        MessageQueue.Instance.Enqueue($"[InstanceActivity] 活动未启动: {info.Name} 没有勾选入口地图");
                        _loggedConfigIssues.Add(info);
                        continue;
                    }

                    var running = IsActivityRunning(info, now);
                    var hasNpcs = _entranceNpcs.TryGetValue(info, out var list) && list.Count > 0;

                    if (running && !hasNpcs)
                        SpawnEntranceNpc(info);

                    if (!running && hasNpcs)
                        DespawnEntranceNpcs(info);
                }

                // 兜底：活动被删除/禁用/列表变更时，回收残留的入口 NPC
                var activitySet = new HashSet<InstanceActivityInfo>(activitySnapshot.Where(a => a != null));
                foreach (var kvp in _entranceNpcs.ToList())
                {
                    var info = kvp.Key;
                    if (info == null || !activitySet.Contains(info) || !info.Enabled)
                    {
                        DespawnEntranceNpcs(info);
                        _entranceNpcs.Remove(info);
                        _loggedConfigIssues.Remove(info);
                    }
                }

                // 副本超时处理：到时未通关则判定失败并送出
                foreach (var inst in _instances.Where(i => i != null && !i.Completed && i.Template != null && i.Template.InstanceTimeLimitSeconds > 0).ToList())
                {
                    var elapsed = (_envir.Now - inst.StartTime).TotalSeconds;
                    if (elapsed < inst.Template.InstanceTimeLimitSeconds) continue;

                    inst.Completed = true;
                    inst.EndTime = _envir.Now;
                    inst.Score = 0;

                    foreach (var p in inst.Players.Where(p => p != null && p.CurrentMap == inst.Map).ToList())
                    {
                        ExpireInstanceTimer(p, inst);
                        p.ReceiveChat($"{inst.Template.Name} 副本超时，已结束。", ChatType.System);

                        Map targetMap = null;
                        Point targetPoint;

                        if (p.Info != null && p.BindMapIndex > 0)
                            targetMap = _envir.GetMap(p.BindMapIndex);

                        if (targetMap == null)
                        {
                            targetMap = inst.Map ?? p.CurrentMap;
                            targetPoint = targetMap?.Info?.SafeZones?.FirstOrDefault()?.Location ?? p.CurrentLocation;
                        }
                        else
                        {
                            targetPoint = p.BindLocation;
                        }

                        if (targetMap != null)
                            p.Teleport(targetMap, targetPoint);
                    }
                }

                // 通关后延时传出副本（通关倒计时）
                foreach (var inst in _instances.Where(i => i != null && i.Completed && i.ExitTime.HasValue).ToList())
                {
                    if (_envir.Now < inst.ExitTime.Value) continue;

                    foreach (var player in inst.Players.Where(p => p != null && !p.Dead && p.CurrentMap == inst.Map).ToList())
                    {
                        ExpireExitTimer(player, inst);

                        Map targetMap = null;
                        Point targetPoint;

                        if (player.Info != null && player.BindMapIndex > 0)
                            targetMap = _envir.GetMap(player.BindMapIndex);

                        if (targetMap == null)
                        {
                            targetMap = inst.Map ?? player.CurrentMap;
                            targetPoint = targetMap?.Info?.SafeZones?.FirstOrDefault()?.Location ?? player.CurrentLocation;
                        }
                        else
                        {
                            targetPoint = player.BindLocation;
                        }

                        if (targetMap != null)
                            player.Teleport(targetMap, targetPoint);
                    }
                }

                // 简单清理已经结束且无人副本的实例（同时从 MapList 移除地图）
                for (int i = _instances.Count - 1; i >= 0; i--)
                {
                    var inst = _instances[i];
                    if (inst == null || !inst.Completed) continue;

                    // 所有关联玩家都不在这张地图上时，移除实例和地图
                    if (inst.Players.All(p => p == null || p.CurrentMap != inst.Map))
                    {
                        CleanupInstance(inst);
                        if (inst.Map != null)
                            _envir.MapList.Remove(inst.Map);

                        _instances.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageQueue.Instance.Enqueue($"[InstanceActivity] Process 异常: {ex}");
            }
        }

        private void CleanupInstance(InstanceActivityInstance inst)
        {
            if (inst == null) return;

            // 清理怪物（避免实例地图被移除后对象仍残留引用）
            try
            {
                if (inst.Monsters != null)
                {
                    foreach (var m in inst.Monsters.Where(x => x != null).ToList())
                    {
                        try
                        {
                            if (!m.Dead) m.Die();
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                    inst.Monsters.Clear();
                }

                if (inst.Boss != null)
                {
                    try
                    {
                        if (!inst.Boss.Dead) inst.Boss.Die();
                    }
                    catch
                    {
                        // ignore
                    }
                    inst.Boss = null;
                }
            }
            catch
            {
                // ignore
            }

            // 清理地图上残留的法术/动态 NPC（避免 SpellObject 没机会 Process 而长期保留）
            var map = inst.Map;
            if (map != null)
            {
                try
                {
                    foreach (var spell in map.Spells.Where(s => s != null).ToList())
                    {
                        try
                        {
                            map.RemoveObject(spell);
                            spell.Despawn();
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                }
                catch
                {
                    // ignore
                }

                try
                {
                    foreach (var npc in map.NPCs.Where(n => n != null).ToList())
                    {
                        try
                        {
                            map.RemoveObject(npc);
                            npc.Die();
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                }
                catch
                {
                    // ignore
                }
            }

            // 断开引用
            try { inst.Players?.Clear(); } catch { /* ignore */ }
        }

        public bool IsActivityRunning(InstanceActivityInfo info, DateTime now)
        {
            if (!info.Enabled) return false;

            foreach (var slot in info.TimeSlots)
            {
                if (slot.Day != now.DayOfWeek) continue;

                var t = now.TimeOfDay;
                if (t >= slot.StartTime && t <= slot.EndTime)
                    return true;
            }

            // 启用了、配置了时间段，但当前时间不在任何时间段内时，只提示一次
            if (!_loggedConfigIssues.Contains(info))
            {
                MessageQueue.Instance.Enqueue($"[InstanceActivity] 活动当前未在时间段内: {info.Name} Today={now.DayOfWeek} Now={now:HH:mm}");
                _loggedConfigIssues.Add(info);
            }

            return false;
        }

        public InstanceActivityInstance CreateInstance(InstanceActivityInfo info, PlayerObject player)
        {
            if (info.InstanceMapIndices == null || info.InstanceMapIndices.Count == 0)
                return null;

            var mapIndex = info.InstanceMapIndices[_envir.Random.Next(info.InstanceMapIndices.Count)];
            var mapInfo = _envir.GetMapInfo(mapIndex);
            if (mapInfo == null)
                return null;

            // 每次进入都创建一张独立地图实例（与世界地图完全隔离）
            var map = new Map(mapInfo) { IsInstance = true };
            if (!map.Load()) return null;
            _envir.MapList.Add(map);

            var instance = new InstanceActivityInstance
            {
                Template = info,
                Map = map,
                StartTime = _envir.Now,
                InstanceId = _nextInstanceId++
            };
            instance.TimerKey = BuildInstanceTimerKey(instance.InstanceId);

            _instances.Add(instance);
            MessageQueue.Instance.Enqueue(
                $"[InstanceActivity] 创建实例: 活动={info.Name}, InstanceId={instance.InstanceId}, 地图={map.Info.FileName}({map.Info.Title})");

            // 记录参与玩家（简化：玩家本人 + 其组队成员）
            if (player.GroupMembers != null && player.GroupMembers.Count > 0)
            {
                foreach (var member in player.GroupMembers)
                {
                    if (member == null) continue;
                    if (!instance.Players.Contains(member))
                        instance.Players.Add(member);
                }
            }
            else
            {
                instance.Players.Add(player);
            }

            // 刷怪：支持小怪多波次（清完一波才刷下一波），最后一波清完再刷 BOSS
            instance.CurrentWave = 0;
            StartNextWaveOrBoss(instance);

            Point spawnPoint;
            if (map.WalkableCells != null && map.WalkableCells.Count > 0)
            {
                spawnPoint = map.WalkableCells[_envir.Random.Next(map.WalkableCells.Count)];
            }
            else
            {
                spawnPoint = map.Info?.SafeZones?.FirstOrDefault()?.Location ?? new Point(0, 0);
            }

            foreach (var member in instance.Players.Where(p => p != null).ToList())
            {
                member.Teleport(map, spawnPoint);
                member.ReceiveChat(
                    $"{info.Name} 副本已创建 (InstanceId={instance.InstanceId})。",
                    ChatType.System);

                // 下发倒计时（如果配置了副本限时）
                SendOrUpdateInstanceTimer(member, instance);
            }

            return instance;
        }

        private void StartNextWaveOrBoss(InstanceActivityInstance instance)
        {
            if (instance == null || instance.Template == null) return;

            // 如果配置为进本立即刷 BOSS，则不走波次逻辑
            if (!instance.Template.BossAfterClearMonsters)
            {
                SpawnMonstersWave(instance);
                if (instance.Boss == null)
                    SpawnBoss(instance);
                return;
            }

            var maxWaves = GetMonsterWaveCount(instance.Template);

            // 可能出现“本波没刷出任何怪”的情况，直接跳过该波次
            while (!instance.Completed && instance.Boss == null)
            {
                if (instance.CurrentWave < maxWaves)
                {
                    instance.CurrentWave++;
                    var spawned = SpawnMonstersWave(instance);

                    // 如果本波刷出了怪，就等待玩家清怪；否则继续推进到下一波/最终BOSS
                    if (spawned > 0) break;
                    continue;
                }

                // 所有波次都结束了，刷新最终 BOSS
                SpawnBoss(instance);
                break;
            }
        }

        private int SpawnMonstersWave(InstanceActivityInstance instance)
        {
            var map = instance.Map;
            var info = instance.Template;

            if (info.MonsterPools == null || info.MonsterPools.Count == 0)
                return 0;

            var spawnedTotal = 0;

            foreach (var pool in info.MonsterPools)
            {
                var entry = RollEntry(pool);
                if (entry == null) continue;

                var monsterInfo = _envir.GetMonsterInfo(entry.MonsterInfoIndex);
                if (monsterInfo == null) continue;

                var count = _envir.Random.Next(entry.MinCount, entry.MaxCount + 1);
                for (int i = 0; i < count; i++)
                {
                    var point = GetRandomPoint(map, entry);
                    var monster = MonsterObject.GetMonster(monsterInfo);
                    if (monster == null) continue;

                    monster.Direction = 0;
                    monster.ActionTime = _envir.Time + 1000;
                    monster.Spawn(map, point);

                    // 记录为本副本的小怪
                    instance.Monsters.Add(monster);
                    spawnedTotal++;
                }
            }

            // 给在副本内的玩家一个提示（只在刷出怪时提示）
            if (spawnedTotal > 0 && instance.Template.BossAfterClearMonsters)
            {
                foreach (var player in instance.Players.Where(p => p != null && !p.Dead && p.CurrentMap == instance.Map))
                    player.ReceiveChat($"{instance.Template.Name} 第 {instance.CurrentWave} 波小怪已刷新。", ChatType.System);
            }

            return spawnedTotal;
        }

        private void SpawnBoss(InstanceActivityInstance instance)
        {
            var map = instance.Map;
            var info = instance.Template;

            if (info.BossPools == null || info.BossPools.Count == 0)
                return;

            var pool = info.BossPools[_envir.Random.Next(info.BossPools.Count)];
            var entry = RollEntry(pool);
            if (entry == null) return;

            var monsterInfo = _envir.GetMonsterInfo(entry.MonsterInfoIndex);
            if (monsterInfo == null) return;

            var boss = MonsterObject.GetMonster(monsterInfo);
            if (boss == null) return;

            var point = GetRandomPoint(map, entry);

            boss.Direction = 0;
            boss.ActionTime = _envir.Time + 1000;
            boss.Spawn(map, point);

            instance.Boss = boss;

            MessageQueue.Instance.Enqueue(
                $"[InstanceActivity] 刷新BOSS: 活动={info.Name}, InstanceId={instance.InstanceId}, 怪物={monsterInfo.Name}, 坐标=({point.X},{point.Y})");
        }

        public void OnMonsterDied(MonsterObject monster, PlayerObject killer)
        {
            if (monster == null) return;

            // 先判断是否是某个副本实例内的怪物/ BOSS
            var inst = _instances.FirstOrDefault(i =>
                (i.Boss == monster) || (i.Monsters != null && i.Monsters.Contains(monster)));
            if (inst == null) return;

            // 普通小怪死亡：从列表移除，并在全部清光时刷新 BOSS
            if (inst.Boss != monster && inst.Monsters != null && inst.Monsters.Contains(monster))
            {
                inst.Monsters.Remove(monster);

                if (!inst.Completed && inst.Boss == null && inst.Template != null && inst.Template.BossAfterClearMonsters &&
                    (inst.Monsters == null || inst.Monsters.Count == 0))
                {
                    // 当前波次小怪已清空：推进到下一波；若所有波次完成，则刷新最终 BOSS
                    StartNextWaveOrBoss(inst);
                }

            }

            // 如果不是 BOSS，或者 BOSS 已经被标记完成，则不走通关逻辑
            if (inst.Boss != monster || inst.Completed)
                return;

            inst.Completed = true;
            inst.EndTime = _envir.Now;

            var duration = (inst.EndTime.Value - inst.StartTime).TotalSeconds;
            inst.Score = duration <= 300 ? 3 : duration <= 600 ? 2 : 1;

            GiveRewards(inst, killer);

            var exitDelay = GetExitDelaySeconds(inst.Template);

            // 通关倒计时：先停止副本时间限制计时器，再启动一个短的离开倒计时
            inst.ExitTime = _envir.Now.AddSeconds(exitDelay);

            foreach (var player in inst.Players.Where(p => p != null && !p.Dead && p.CurrentMap == inst.Map))
            {
                // 结束副本总时长计时
                ExpireInstanceTimer(player, inst);

                // 启动通关倒计时（例如 10 秒后自动传送出本）
                SendExitTimer(player, inst, exitDelay);

                player.ReceiveChat(
                    $"{inst.Template.Name} 副本已通关，{exitDelay} 秒后将自动传送离开副本。",
                    ChatType.System);
            }
        }

        private static ActivityMonsterEntry RollEntry(ActivityMonsterPool pool)
        {
            if (pool.Entries == null || pool.Entries.Count == 0)
                return null;

            var totalWeight = pool.Entries.Sum(e => Math.Max(1, e.Weight));
            var roll = RandomProvider.GetThreadRandom().Next(totalWeight);

            var cumulative = 0;
            foreach (var entry in pool.Entries)
            {
                cumulative += Math.Max(1, entry.Weight);
                if (roll < cumulative)
                    return entry;
            }

            return pool.Entries[pool.Entries.Count - 1];
        }

        private Point GetRandomPoint(Map map, ActivityMonsterEntry entry)
        {
            if (map == null) return Point.Empty;
            if (entry.Area.Width > 0 && entry.Area.Height > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    var x = _envir.Random.Next(entry.Area.X, entry.Area.Right);
                    var y = _envir.Random.Next(entry.Area.Y, entry.Area.Bottom);
                    if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
                    {
                        if (map.Cells[x, y].Attribute == CellAttribute.Walk)
                            return new Point(x, y);
                    }
                }
            }

            if (map.WalkableCells != null && map.WalkableCells.Count > 0)
                return map.WalkableCells[_envir.Random.Next(map.WalkableCells.Count)];

            var safe = map.Info?.SafeZones?.FirstOrDefault()?.Location ?? Point.Empty;
            return safe;
        }

        private void GiveRewards(InstanceActivityInstance instance, PlayerObject killer)
        {
            if (instance.Template.Rewards == null || instance.Template.Rewards.Count == 0)
                return;

            var score = instance.Score;
            var reward = instance.Template.Rewards
                .FirstOrDefault(r => score >= r.MinScore && score <= r.MaxScore);

            if (reward == null) return;

            // 简化：给击杀者发奖励，后续可以扩展到队伍内所有玩家
            var player = killer;
            if (player == null) return;

            if (reward.Exp > 0)
                player.WinExp(reward.Exp, player.Level);

            if (reward.Gold > 0)
                player.GainGold(reward.Gold);

            // 物品奖励：从候选列表中随机 1 件发放（不是全部都给）
            if (reward.Items != null && reward.Items.Count > 0)
            {
                var candidates = reward.Items
                    .Where(i => i != null && i.Count > 0 && _envir.GetItemInfo(i.ItemIndex) != null)
                    .ToList();

                if (candidates.Count > 0)
                {
                    var picked = candidates[_envir.Random.Next(candidates.Count)];
                    var info = _envir.GetItemInfo(picked.ItemIndex);
                    if (info != null)
                    {
                        var remaining = picked.Count;
                        while (remaining > 0)
                        {
                            var userItem = _envir.CreateDropItem(info);
                            if (userItem == null) break;

                            var stackSize = Math.Max(1, (int)info.StackSize);
                            var stack = (ushort)Math.Min(remaining, (uint)stackSize);
                            if (stack <= 0) break;
                            userItem.Count = stack;

                            if (!player.CanGainItem(userItem))
                                break;

                            player.GainItem(userItem);
                            remaining -= stack;
                        }
                    }
                }
            }
        }

        private void SpawnEntranceNpc(InstanceActivityInfo info)
        {
            if (info == null) return;

            var count = info.EntranceNpcCount > 0 ? info.EntranceNpcCount : 1;

            // 副本开启时全服广播
            _envir.Broadcast(new S.Chat
            {
                Message = $"[活动] {info.Name} 副本已开启，入口NPC已出现在各入口地图，请前往参与！",
                Type = ChatType.Announcement
            });

            SpawnEntranceNpcInternal(info, count);
        }

        /// <summary>
        /// 实际刷入口 NPC 的方法，可指定数量，并控制是否在活动期间补刷单个入口。
        /// 不做广播，由调用方决定是否广播。
        /// </summary>
        private void SpawnEntranceNpcInternal(InstanceActivityInfo info, int count)
        {
            if (info == null) return;
            if (info.EntranceMapIndices == null || info.EntranceMapIndices.Count == 0)
                return;

            if (count <= 0) count = 1;

            for (int i = 0; i < count; i++)
            {
                var mapIndex = info.EntranceMapIndices[_envir.Random.Next(info.EntranceMapIndices.Count)];

                var map = _envir.MapList.FirstOrDefault(x => x.Info.Index == mapIndex);
                if (map == null || map.WalkableCells == null || map.WalkableCells.Count == 0)
                    continue;

                var location = map.WalkableCells[_envir.Random.Next(map.WalkableCells.Count)];

                var npcInfo = new NPCInfo
                {
                    Index = 0, // 临时 NPC，不写入 DB
                    MapIndex = map.Info.Index,
                    Location = location,
                    FileName = Settings.DefaultNPCFilename,
                    Name = $"{info.Name} 入口",
                    Image = 0,
                    Rate = 100,
                    ShowOnBigMap = false,
                    CanTeleportTo = false,
                    ConquestVisible = true
                };

                var npc = new NPCObject(npcInfo)
                {
                    CurrentMap = map
                };

                map.AddObject(npc);

                if (!_entranceNpcs.TryGetValue(info, out var list))
                {
                    list = new List<NPCObject>();
                    _entranceNpcs[info] = list;
                }
                list.Add(npc);

                _entranceNpcLookup[npc.ObjectID] = info;

                // 监控：入口 NPC 刷新信息写入调试日志（不刷屏主服日志）
                MessageQueue.Instance.EnqueueDebugging(
                    $"[InstanceActivity] 入口NPC刷新: 活动={info.Name}, 地图={map.Info.FileName}({map.Info.Title}), 坐标=({location.X},{location.Y})");
            }
        }

        private void DespawnEntranceNpcs(InstanceActivityInfo info)
        {
            if (!_entranceNpcs.TryGetValue(info, out var list) || list.Count == 0)
                return;

            // 清理该活动下所有入口 NPC
            foreach (var npc in list.ToList())
                RemoveEntranceNpc(info, npc);
        }

        /// <summary>
        /// 只移除某一个入口 NPC，其余入口 NPC 保留
        /// </summary>
        private void RemoveEntranceNpc(InstanceActivityInfo info, NPCObject npc)
        {
            if (info == null || npc == null) return;

            if (_entranceNpcs.TryGetValue(info, out var list))
            {
                if (list.Contains(npc))
                    list.Remove(npc);

                if (list.Count == 0)
                    _entranceNpcs.Remove(info);
            }

            _entranceNpcLookup.Remove(npc.ObjectID);

            try
            {
                npc.Die();
            }
            catch
            {
                // ignore
            }
        }

        public bool TryEnterActivity(PlayerObject player, NPCObject npc)
        {
            if (player == null || npc == null) return false;

            if (!_entranceNpcLookup.TryGetValue(npc.ObjectID, out var info))
                return false;

            // 仅在活动时间内允许进入
            if (!IsActivityRunning(info, _envir.Now))
                return false;

            // 次数限制：按角色每天进入次数控制
            if (info.DailyEnterLimit > 0)
            {
                if (player.Info == null)
                {
                    player.ReceiveChat($"无法进入 {info.Name}：角色信息未就绪。", ChatType.System);
                    return false;
                }

                var charIndex = player.Info.Index;
                var today = _envir.Now.Date;

                if (!_enterCounts.TryGetValue(info.Index, out var actDict))
                {
                    actDict = new Dictionary<int, (DateTime Date, int Count)>();
                    _enterCounts[info.Index] = actDict;
                }

                if (actDict.TryGetValue(charIndex, out var entry))
                {
                    if (entry.Date.Date != today)
                    {
                        entry = (today, 0);
                    }

                    if (entry.Count >= info.DailyEnterLimit)
                    {
                        player.ReceiveChat($"今日进入 {info.Name} 的次数已达上限({info.DailyEnterLimit}次)。", ChatType.System);
                        return false;
                    }

                    entry.Count++;
                    actDict[charIndex] = entry;
                }
                else
                {
                    actDict[charIndex] = (today, 1);
                }
            }

            // 如果已经有该活动的实例并记录了该玩家，则重传送回副本
            var existing = _instances.FirstOrDefault(i => i.Template == info && i.Players.Contains(player) && !i.Completed);
            if (existing != null)
            {
                if (existing.Map != null && existing.Map.WalkableCells != null && existing.Map.WalkableCells.Count > 0)
                {
                    var point = existing.Map.WalkableCells[_envir.Random.Next(existing.Map.WalkableCells.Count)];
                    player.Teleport(existing.Map, point);
                }

                // 重新进入时刷新倒计时
                SendOrUpdateInstanceTimer(player, existing);
                return true;
            }

            var inst = CreateInstance(info, player);

            // 新建副本实例成功后，仅移除当前点击的这个入口 NPC，
            // 并在活动仍然开启的情况下，立即补刷一个新的入口 NPC，保持入口数量不变。
            if (inst != null)
            {
                RemoveEntranceNpc(info, npc);

                // 仍在活动时间内，补刷 1 个入口 NPC（不再重新广播）
                if (IsActivityRunning(info, _envir.Now))
                {
                    SpawnEntranceNpcInternal(info, 1);
                }
            }

            return true;
        }
    }
}

