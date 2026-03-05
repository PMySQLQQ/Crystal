using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Server.MirEnvir;

namespace Server.MirDatabase
{
    public class InstanceActivityInfo
    {
        protected static Envir Envir => Envir.Main;
        protected static Envir EditEnvir => Envir.Edit;

        public int Index;
        public string Name = string.Empty;
        public bool Enabled;

        public int EntranceNpcCount = 1;

        // 每个角色每天最多可进入次数，0 或负数表示不限制
        public int DailyEnterLimit = 0;

        // 副本限时（秒），0 或负数表示不限制/不显示倒计时
        public int InstanceTimeLimitSeconds = 0;

        // BOSS 刷新时机：true=清光小怪后才刷BOSS；false=进本立即刷BOSS
        public bool BossAfterClearMonsters = true;

        // 小怪波数（>=1）。清完一波才会刷新下一波，最后一波清完再刷新 BOSS（当 BossAfterClearMonsters=true 时生效）
        public int MonsterWaveCount = 1;

        // 通关后在副本内停留时间（秒），0 或负数表示使用全局设置
        public int ExitDelaySeconds = 0;

        public List<ActivityTimeSlot> TimeSlots = new List<ActivityTimeSlot>();
        public List<int> EntranceMapIndices = new List<int>();
        public List<int> InstanceMapIndices = new List<int>();

        public List<ActivityMonsterPool> MonsterPools = new List<ActivityMonsterPool>();
        public List<ActivityMonsterPool> BossPools = new List<ActivityMonsterPool>();

        public List<ActivityReward> Rewards = new List<ActivityReward>();

        public InstanceActivityInfo()
        {
        }

        public InstanceActivityInfo(BinaryReader reader, int version, int customVersion)
        {
            Index = reader.ReadInt32();
            Name = reader.ReadString();
            Enabled = reader.ReadBoolean();

            try
            {
                if (version >= 119)
                {
                    // 新版：入口NPC数量 + 每日次数上限 + 副本限时 + BOSS刷新时机 + 小怪波数 + 通关停留秒数
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = reader.ReadInt32();
                    InstanceTimeLimitSeconds = reader.ReadInt32();
                    BossAfterClearMonsters = reader.ReadBoolean();
                    MonsterWaveCount = reader.ReadInt32();
                    ExitDelaySeconds = reader.ReadInt32();
                }
                else if (version >= 119)
                {
                    // 新版：入口NPC数量 + 每日次数上限 + 副本限时 + BOSS刷新时机 + 通关停留秒数
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = reader.ReadInt32();
                    InstanceTimeLimitSeconds = reader.ReadInt32();
                    BossAfterClearMonsters = reader.ReadBoolean();
                    MonsterWaveCount = 1;
                    ExitDelaySeconds = reader.ReadInt32();
                }
                else if (version >= 119)
                {
                    // 新版：入口NPC数量 + 每日次数上限 + 副本限时 + BOSS刷新时机
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = reader.ReadInt32();
                    InstanceTimeLimitSeconds = reader.ReadInt32();
                    BossAfterClearMonsters = reader.ReadBoolean();
                    MonsterWaveCount = 1;
                    ExitDelaySeconds = 0;
                }
                else if (version >= 119)
                {
                    // 新版：入口NPC数量 + 每日次数上限 + 副本限时
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = reader.ReadInt32();
                    InstanceTimeLimitSeconds = reader.ReadInt32();
                    BossAfterClearMonsters = true;
                    MonsterWaveCount = 1;
                    ExitDelaySeconds = 0;
                }
                else if (version >= 119)
                {
                    // 旧版(121)：入口NPC数量 + 每日次数上限
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = reader.ReadInt32();
                    InstanceTimeLimitSeconds = 0;
                    MonsterWaveCount = 1;
                    ExitDelaySeconds = 0;
                }
                else if (version >= 119)
                {
                    // 旧版：只保存了入口NPC数量
                    EntranceNpcCount = reader.ReadInt32();
                    DailyEnterLimit = 0;
                    InstanceTimeLimitSeconds = 0;
                    MonsterWaveCount = 1;
                    ExitDelaySeconds = 0;
                }

                var count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    TimeSlots.Add(new ActivityTimeSlot(reader));

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    EntranceMapIndices.Add(reader.ReadInt32());

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    InstanceMapIndices.Add(reader.ReadInt32());

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    MonsterPools.Add(new ActivityMonsterPool(reader));

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    BossPools.Add(new ActivityMonsterPool(reader));

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    Rewards.Add(new ActivityReward(reader));
            }
            catch (EndOfStreamException)
            {
                // 兼容旧版或损坏的数据库：读到文件末尾就停止，保持已读取的数据
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Name);
            writer.Write(Enabled);

            writer.Write(EntranceNpcCount);
            writer.Write(DailyEnterLimit);
            writer.Write(InstanceTimeLimitSeconds);
            writer.Write(BossAfterClearMonsters);
            writer.Write(MonsterWaveCount);
            writer.Write(ExitDelaySeconds);

            writer.Write(TimeSlots.Count);
            for (int i = 0; i < TimeSlots.Count; i++)
                TimeSlots[i].Save(writer);

            writer.Write(EntranceMapIndices.Count);
            for (int i = 0; i < EntranceMapIndices.Count; i++)
                writer.Write(EntranceMapIndices[i]);

            writer.Write(InstanceMapIndices.Count);
            for (int i = 0; i < InstanceMapIndices.Count; i++)
                writer.Write(InstanceMapIndices[i]);

            writer.Write(MonsterPools.Count);
            for (int i = 0; i < MonsterPools.Count; i++)
                MonsterPools[i].Save(writer);

            writer.Write(BossPools.Count);
            for (int i = 0; i < BossPools.Count; i++)
                BossPools[i].Save(writer);

            writer.Write(Rewards.Count);
            for (int i = 0; i < Rewards.Count; i++)
                Rewards[i].Save(writer);
        }

        public override string ToString()
        {
            var limitText = DailyEnterLimit > 0 ? $" 次数:{DailyEnterLimit}/天" : "";
            return $"{Index}: {Name} (入口NPC:{EntranceNpcCount}{limitText})";
        }
    }

    public class ActivityTimeSlot
    {
        public DayOfWeek Day;
        public TimeSpan StartTime;
        public TimeSpan EndTime;

        public ActivityTimeSlot()
        {
        }

        public ActivityTimeSlot(BinaryReader reader)
        {
            Day = (DayOfWeek)reader.ReadInt32();
            StartTime = TimeSpan.FromTicks(reader.ReadInt64());
            EndTime = TimeSpan.FromTicks(reader.ReadInt64());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write((int)Day);
            writer.Write(StartTime.Ticks);
            writer.Write(EndTime.Ticks);
        }

        public override string ToString()
        {
            return $"{Day} {StartTime:hh\\:mm}-{EndTime:hh\\:mm}";
        }
    }

    public class ActivityMonsterPool
    {
        public string Name = string.Empty;
        public List<ActivityMonsterEntry> Entries = new List<ActivityMonsterEntry>();

        public ActivityMonsterPool()
        {
        }

        public ActivityMonsterPool(BinaryReader reader)
        {
            Name = reader.ReadString();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Entries.Add(new ActivityMonsterEntry(reader));
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Entries.Count);
            for (int i = 0; i < Entries.Count; i++)
                Entries[i].Save(writer);
        }
    }

    public class ActivityMonsterEntry
    {
        public int MonsterInfoIndex;
        public int MinCount;
        public int MaxCount;
        public int Weight;
        public Rectangle Area;

        public ActivityMonsterEntry()
        {
        }

        public ActivityMonsterEntry(BinaryReader reader)
        {
            MonsterInfoIndex = reader.ReadInt32();
            MinCount = reader.ReadInt32();
            MaxCount = reader.ReadInt32();
            Weight = reader.ReadInt32();

            var hasArea = reader.ReadBoolean();
            if (hasArea)
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var w = reader.ReadInt32();
                var h = reader.ReadInt32();
                Area = new Rectangle(x, y, w, h);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(MonsterInfoIndex);
            writer.Write(MinCount);
            writer.Write(MaxCount);
            writer.Write(Weight);

            var hasArea = Area.Width > 0 && Area.Height > 0;
            writer.Write(hasArea);
            if (hasArea)
            {
                writer.Write(Area.X);
                writer.Write(Area.Y);
                writer.Write(Area.Width);
                writer.Write(Area.Height);
            }
        }

        public override string ToString()
        {
            return $"MonsterIndex={MonsterInfoIndex}, Count={MinCount}-{MaxCount}, Weight={Weight}";
        }
    }

    public class ActivityReward
    {
        public int MinScore;
        public int MaxScore;

        public uint Exp;
        public uint Gold;
        public List<ActivityRewardItem> Items = new List<ActivityRewardItem>();

        public ActivityReward()
        {
        }

        public ActivityReward(BinaryReader reader)
        {
            MinScore = reader.ReadInt32();
            MaxScore = reader.ReadInt32();
            Exp = reader.ReadUInt32();
            Gold = reader.ReadUInt32();

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Items.Add(new ActivityRewardItem(reader));
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(MinScore);
            writer.Write(MaxScore);
            writer.Write(Exp);
            writer.Write(Gold);

            writer.Write(Items.Count);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Save(writer);
        }

        public override string ToString()
        {
            return $"Score {MinScore}-{MaxScore}: Exp={Exp}, Gold={Gold}, Items={Items.Count}";
        }
    }

    public class ActivityRewardItem
    {
        public int ItemIndex;
        public uint Count;

        public ActivityRewardItem()
        {
        }

        public ActivityRewardItem(BinaryReader reader)
        {
            ItemIndex = reader.ReadInt32();
            Count = reader.ReadUInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(ItemIndex);
            writer.Write(Count);
        }
    }
}

