using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Server.MirDatabase;
using Server.MirEnvir;

namespace Server
{
    public partial class InstanceActivityForm : Form
    {
        public Envir Envir => SMain.EditEnvir;

        private List<InstanceActivityInfo> _selectedActivities;

        public InstanceActivityForm()
        {
            InitializeComponent();
            if (InstanceTimeLimitTextBox != null)
                InstanceTimeLimitTextBox.TextChanged += InstanceTimeLimitTextBox_TextChanged;
            if (ExitDelaySecondsTextBox != null)
                ExitDelaySecondsTextBox.TextChanged += ExitDelaySecondsTextBox_TextChanged;
            if (BossAfterClearMonstersCheckBox != null)
                BossAfterClearMonstersCheckBox.CheckedChanged += BossAfterClearMonstersCheckBox_CheckedChanged;
            if (MonsterWaveCountTextBox != null)
                MonsterWaveCountTextBox.TextChanged += MonsterWaveCountTextBox_TextChanged;
            DayComboBox.Items.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<object>().ToArray());
            RewardItemComboBox.DisplayMember = "Name";
            RewardItemComboBox.ValueMember = "Index";
            foreach (var item in Envir.ItemInfoList)
                RewardItemComboBox.Items.Add(item);

            MonsterInfoComboBox.DisplayMember = "Name";
            MonsterInfoComboBox.ValueMember = "Index";
            foreach (var mon in Envir.MonsterInfoList)
                MonsterInfoComboBox.Items.Add(mon);

            UpdateInterface(true);
        }

        private void InstanceActivityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Envir.SaveDB();
        }

        private void UpdateInterface(bool refresh = false)
        {
            ActivityListBox.SelectedIndexChanged -= ActivityListBox_SelectedIndexChanged;

            if (refresh || ActivityListBox.Items.Count != Envir.InstanceActivityList.Count)
            {
                var selected = ActivityListBox.SelectedItem as InstanceActivityInfo;

                ActivityListBox.Items.Clear();

                foreach (var info in Envir.InstanceActivityList)
                    ActivityListBox.Items.Add(info);

                if (selected != null && ActivityListBox.Items.Contains(selected))
                    ActivityListBox.SelectedItem = selected;
            }

            ActivityListBox.SelectedIndexChanged += ActivityListBox_SelectedIndexChanged;

            _selectedActivities = ActivityListBox.SelectedItems.Cast<InstanceActivityInfo>().ToList();

            if (_selectedActivities.Count == 0)
            {
                ActivityNameTextBox.Text = string.Empty;
                EnabledCheckBox.Checked = false;
                TimeSlotListBox.Items.Clear();
                EntranceMapsCheckedListBox.Items.Clear();
                InstanceMapsCheckedListBox.Items.Clear();
                return;
            }

            var a = _selectedActivities[0];
            ActivityNameTextBox.Text = a.Name;
            EnabledCheckBox.Checked = a.Enabled;

            // 时间段列表
            TimeSlotListBox.Items.Clear();
            foreach (var slot in a.TimeSlots)
                TimeSlotListBox.Items.Add(slot);

            // 地图列表
            EntranceMapsCheckedListBox.Items.Clear();
            InstanceMapsCheckedListBox.Items.Clear();

            foreach (var mapInfo in Envir.MapInfoList)
            {
                var idx = EntranceMapsCheckedListBox.Items.Add(mapInfo);
                EntranceMapsCheckedListBox.SetItemChecked(idx, a.EntranceMapIndices.Contains(mapInfo.Index));

                idx = InstanceMapsCheckedListBox.Items.Add(mapInfo);
                InstanceMapsCheckedListBox.SetItemChecked(idx, a.InstanceMapIndices.Contains(mapInfo.Index));
            }

            if (a.EntranceNpcCount <= 0)
                a.EntranceNpcCount = 1;

            if (EntranceNpcCountTextBox != null)
                EntranceNpcCountTextBox.Text = a.EntranceNpcCount.ToString();

            if (DailyLimitTextBox != null)
                DailyLimitTextBox.Text = a.DailyEnterLimit > 0 ? a.DailyEnterLimit.ToString() : "";

            if (InstanceTimeLimitTextBox != null)
            {
                var minutes = a.InstanceTimeLimitSeconds > 0 ? (a.InstanceTimeLimitSeconds / 60) : 0;
                InstanceTimeLimitTextBox.Text = minutes > 0 ? minutes.ToString() : "";
            }

            if (BossAfterClearMonstersCheckBox != null)
                BossAfterClearMonstersCheckBox.Checked = a.BossAfterClearMonsters;

            if (ExitDelaySecondsTextBox != null)
                ExitDelaySecondsTextBox.Text = a.ExitDelaySeconds > 0 ? a.ExitDelaySeconds.ToString() : "";

            if (MonsterWaveCountTextBox != null)
                MonsterWaveCountTextBox.Text = a.MonsterWaveCount > 1 ? a.MonsterWaveCount.ToString() : "1";

            // 奖励列表
            RewardListBox.Items.Clear();
            foreach (var reward in a.Rewards)
                RewardListBox.Items.Add(reward);

            RefreshRewardItems();

            RefreshMonsterPools();
            RefreshBossPools();
        }

        private void ActivityListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var info = new InstanceActivityInfo
            {
                Index = ++Envir.InstanceActivityIndex,
                Name = "New Activity",
                Enabled = false
            };

            Envir.InstanceActivityList.Add(info);
            UpdateInterface(true);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            foreach (var a in _selectedActivities)
                Envir.InstanceActivityList.Remove(a);

            UpdateInterface(true);
        }

        private void ActivityNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            foreach (var a in _selectedActivities)
                a.Name = ActivityNameTextBox.Text;

            ActivityListBox.Refresh();
        }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            foreach (var a in _selectedActivities)
                a.Enabled = EnabledCheckBox.Checked;
        }

        private void DailyLimitTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (!int.TryParse(DailyLimitTextBox.Text, out var value))
            {
                // 允许空或非法值视为不限制
                value = 0;
            }
            if (value < 0) value = 0;

            var a = _selectedActivities[0];
            a.DailyEnterLimit = value;

            ActivityListBox.Refresh();
        }

        private void InstanceTimeLimitTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            if (!int.TryParse(InstanceTimeLimitTextBox.Text, out var minutes))
                minutes = 0;

            if (minutes < 0) minutes = 0;

            // 0=不限制；大于0则换算成秒（限制上限，避免溢出）
            var seconds = minutes <= 0 ? 0 : Math.Min(24 * 60 * 60, minutes * 60);

            var a = _selectedActivities[0];
            a.InstanceTimeLimitSeconds = seconds;
        }

        private void BossAfterClearMonstersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            var value = BossAfterClearMonstersCheckBox.Checked;
            foreach (var a in _selectedActivities)
                a.BossAfterClearMonsters = value;
        }

        private void ExitDelaySecondsTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            if (!int.TryParse(ExitDelaySecondsTextBox.Text, out var seconds))
                seconds = 0;

            if (seconds < 0) seconds = 0;
            if (seconds > 24 * 60 * 60) seconds = 24 * 60 * 60;

            foreach (var a in _selectedActivities)
                a.ExitDelaySeconds = seconds;
        }

        private void MonsterWaveCountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            if (!int.TryParse(MonsterWaveCountTextBox.Text, out var waves))
                waves = 1;

            if (waves < 1) waves = 1;
            if (waves > 50) waves = 50;

            foreach (var a in _selectedActivities)
                a.MonsterWaveCount = waves;
        }

        private void EntranceNpcCountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (!int.TryParse(EntranceNpcCountTextBox.Text, out var value)) return;
            if (value <= 0) value = 1;

            var a = _selectedActivities[0];
            a.EntranceNpcCount = value;
        }

        private void AddTimeSlotButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (DayComboBox.SelectedItem == null) return;

            if (!int.TryParse(StartHourTextBox.Text, out var sh)) return;
            if (!int.TryParse(StartMinuteTextBox.Text, out var sm)) return;
            if (!int.TryParse(EndHourTextBox.Text, out var eh)) return;
            if (!int.TryParse(EndMinuteTextBox.Text, out var em)) return;

            var a = _selectedActivities[0];

            var slot = new ActivityTimeSlot
            {
                Day = (DayOfWeek)DayComboBox.SelectedItem,
                StartTime = new TimeSpan(sh, sm, 0),
                EndTime = new TimeSpan(eh, em, 0)
            };

            // 如果当前选中了一个已有的时间段，就视为修改该时间段；否则新增
            if (TimeSlotListBox.SelectedItem is ActivityTimeSlot existing)
            {
                existing.Day = slot.Day;
                existing.StartTime = slot.StartTime;
                existing.EndTime = slot.EndTime;
            }
            else
            {
                a.TimeSlots.Add(slot);
            }

            UpdateInterface();
        }

        private void RemoveTimeSlotButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (TimeSlotListBox.SelectedItem is not ActivityTimeSlot slot) return;

            var a = _selectedActivities[0];
            a.TimeSlots.Remove(slot);
            UpdateInterface();
        }

        private void TimeSlotListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeSlotListBox.SelectedItem is not ActivityTimeSlot slot) return;

            // 选中某个时间段时，把它的配置回填到下面的编辑控件，方便修改
            DayComboBox.SelectedItem = slot.Day;
            StartHourTextBox.Text = slot.StartTime.Hours.ToString("00");
            StartMinuteTextBox.Text = slot.StartTime.Minutes.ToString("00");
            EndHourTextBox.Text = slot.EndTime.Hours.ToString("00");
            EndMinuteTextBox.Text = slot.EndTime.Minutes.ToString("00");
        }

        private void EntranceMapsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (EntranceMapsCheckedListBox.Items[e.Index] is not MapInfo mapInfo) return;

            var a = _selectedActivities[0];

            if (e.NewValue == CheckState.Checked)
            {
                if (!a.EntranceMapIndices.Contains(mapInfo.Index))
                    a.EntranceMapIndices.Add(mapInfo.Index);
            }
            else
            {
                a.EntranceMapIndices.Remove(mapInfo.Index);
            }
        }

        private void InstanceMapsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (InstanceMapsCheckedListBox.Items[e.Index] is not MapInfo mapInfo) return;

            var a = _selectedActivities[0];

            if (e.NewValue == CheckState.Checked)
            {
                if (!a.InstanceMapIndices.Contains(mapInfo.Index))
                    a.InstanceMapIndices.Add(mapInfo.Index);
            }
            else
            {
                a.InstanceMapIndices.Remove(mapInfo.Index);
            }
        }

        private void AddRewardButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;

            if (!int.TryParse(RewardMinScoreTextBox.Text, out var minScore)) return;
            if (!int.TryParse(RewardMaxScoreTextBox.Text, out var maxScore)) return;
            if (!uint.TryParse(RewardExpTextBox.Text, out var exp)) return;
            if (!uint.TryParse(RewardGoldTextBox.Text, out var gold)) return;

            var reward = new ActivityReward
            {
                MinScore = minScore,
                MaxScore = maxScore,
                Exp = exp,
                Gold = gold
            };

            var a = _selectedActivities[0];
            a.Rewards.Add(reward);
            UpdateInterface();
        }

        private void RemoveRewardButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            if (RewardListBox.SelectedItem is not ActivityReward reward) return;

            var a = _selectedActivities[0];
            a.Rewards.Remove(reward);
            UpdateInterface();
        }

        private ActivityReward GetSelectedReward()
        {
            return RewardListBox.SelectedItem as ActivityReward;
        }

        private void RefreshRewardItems()
        {
            RewardItemsListBox.Items.Clear();
            var reward = GetSelectedReward();
            if (reward == null) return;

            foreach (var ri in reward.Items)
            {
                var info = Envir.ItemInfoList.FirstOrDefault(x => x.Index == ri.ItemIndex);
                var name = info != null ? info.Name : ri.ItemIndex.ToString();
                RewardItemsListBox.Items.Add($"{name} x{ri.Count}");
            }
        }

        private void RewardListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshRewardItems();
        }

        private void AddRewardItemButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var reward = GetSelectedReward();
            if (reward == null) return;
            if (RewardItemComboBox.SelectedItem is not ItemInfo itemInfo) return;
            if (!uint.TryParse(RewardItemCountTextBox.Text, out var count) || count == 0) return;

            reward.Items.Add(new ActivityRewardItem
            {
                ItemIndex = itemInfo.Index,
                Count = count
            });

            RefreshRewardItems();
        }

        private void AddRewardItemsBatchButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var reward = GetSelectedReward();
            if (reward == null) return;

            using var form = new Form
            {
                Text = "批量添加奖励物品",
                StartPosition = FormStartPosition.CenterParent,
                Width = 520,
                Height = 420,
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var tip = new Label
            {
                Parent = form,
                AutoSize = true,
                Left = 12,
                Top = 12,
                Text = "每行一条：物品名 数量  或  物品Index 数量，例如：\r\n金条 2\r\n1001 5\r\n（空行会忽略）"
            };

            var text = new TextBox
            {
                Parent = form,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Left = 12,
                Top = tip.Bottom + 8,
                Width = form.ClientSize.Width - 24,
                Height = form.ClientSize.Height - 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var ok = new Button
            {
                Parent = form,
                Text = "确定",
                DialogResult = DialogResult.OK,
                Width = 90,
                Height = 28,
                Left = form.ClientSize.Width - 12 - 90 - 90 - 8,
                Top = form.ClientSize.Height - 50,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            var cancel = new Button
            {
                Parent = form,
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Width = 90,
                Height = 28,
                Left = form.ClientSize.Width - 12 - 90,
                Top = form.ClientSize.Height - 50,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };

            form.AcceptButton = ok;
            form.CancelButton = cancel;

            if (form.ShowDialog(this) != DialogResult.OK) return;

            var lines = (text.Text ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var added = 0;
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                var parts = line.Split(new[] { ' ', '\t', ',', '，', 'x', 'X', '*' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                var key = parts[0].Trim();
                if (!uint.TryParse(parts[1], out var count) || count == 0) continue;

                ItemInfo itemInfo = null;
                if (int.TryParse(key, out var itemIndex))
                {
                    itemInfo = Envir.ItemInfoList.FirstOrDefault(i => i.Index == itemIndex);
                }
                else
                {
                    itemInfo = Envir.ItemInfoList.FirstOrDefault(i => string.Equals(i.Name, key, StringComparison.OrdinalIgnoreCase));
                }

                if (itemInfo == null) continue;

                var existing = reward.Items.FirstOrDefault(i => i.ItemIndex == itemInfo.Index);
                if (existing != null)
                    existing.Count += count;
                else
                    reward.Items.Add(new ActivityRewardItem { ItemIndex = itemInfo.Index, Count = count });

                added++;
            }

            RefreshRewardItems();

            if (added == 0)
                MessageBox.Show("没有成功解析任何物品，请检查格式或物品名/Index 是否正确。", "批量添加", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RemoveRewardItemButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var reward = GetSelectedReward();
            if (reward == null) return;
            if (RewardItemsListBox.SelectedIndex < 0) return;
            if (RewardItemsListBox.SelectedIndex >= reward.Items.Count) return;

            reward.Items.RemoveAt(RewardItemsListBox.SelectedIndex);
            RefreshRewardItems();
        }

        private ActivityMonsterPool GetSelectedMonsterPool()
        {
            return MonsterPoolListBox.SelectedItem as ActivityMonsterPool;
        }

        private ActivityMonsterEntry GetSelectedMonsterEntry()
        {
            return MonsterEntryListBox.SelectedItem as ActivityMonsterEntry;
        }

        private ActivityMonsterPool GetSelectedBossPool()
        {
            return BossPoolListBox.SelectedItem as ActivityMonsterPool;
        }

        private ActivityMonsterEntry GetSelectedBossEntry()
        {
            return BossEntryListBox.SelectedItem as ActivityMonsterEntry;
        }

        private void RefreshMonsterPools()
        {
            MonsterPoolListBox.Items.Clear();
            MonsterEntryListBox.Items.Clear();

            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];

            foreach (var pool in a.MonsterPools)
                MonsterPoolListBox.Items.Add(pool);

            if (MonsterPoolListBox.Items.Count > 0)
            {
                MonsterPoolListBox.SelectedIndex = 0;
                RefreshMonsterEntries();
            }
        }

        private void RefreshMonsterEntries()
        {
            MonsterEntryListBox.Items.Clear();
            var pool = GetSelectedMonsterPool();
            if (pool == null) return;

            foreach (var entry in pool.Entries)
                MonsterEntryListBox.Items.Add(entry);
        }

        private void RefreshBossPools()
        {
            BossPoolListBox.Items.Clear();
            BossEntryListBox.Items.Clear();

            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];

            foreach (var pool in a.BossPools)
                BossPoolListBox.Items.Add(pool);

            if (BossPoolListBox.Items.Count > 0)
            {
                BossPoolListBox.SelectedIndex = 0;
                RefreshBossEntries();
            }
        }

        private void RefreshBossEntries()
        {
            BossEntryListBox.Items.Clear();
            var pool = GetSelectedBossPool();
            if (pool == null) return;

            foreach (var entry in pool.Entries)
                BossEntryListBox.Items.Add(entry);
        }

        private void AddMonsterPoolButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];

            var pool = new ActivityMonsterPool { Name = "怪物池" + (a.MonsterPools.Count + 1) };
            a.MonsterPools.Add(pool);

            RefreshMonsterPools();
        }

        private void RemoveMonsterPoolButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];
            var pool = GetSelectedMonsterPool();
            if (pool == null) return;

            a.MonsterPools.Remove(pool);
            RefreshMonsterPools();
        }

        private void AddMonsterEntryButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var pool = GetSelectedMonsterPool();
            if (pool == null) return;
            if (MonsterInfoComboBox.SelectedItem is not MonsterInfo monInfo) return;
            if (!int.TryParse(MonsterMinCountTextBox.Text, out var min)) return;
            if (!int.TryParse(MonsterMaxCountTextBox.Text, out var max)) return;
            if (!int.TryParse(MonsterWeightTextBox.Text, out var weight)) return;
            if (min < 1) min = 1;
            if (max < min) max = min;
            if (weight < 1) weight = 1;

            var entry = new ActivityMonsterEntry
            {
                MonsterInfoIndex = monInfo.Index,
                MinCount = min,
                MaxCount = max,
                Weight = weight
            };

            pool.Entries.Add(entry);
            MonsterEntryListBox.Items.Add(entry);
        }

        private void RemoveMonsterEntryButton_Click(object sender, EventArgs e)
        {
            var pool = GetSelectedMonsterPool();
            var entry = GetSelectedMonsterEntry();
            if (pool == null || entry == null) return;

            pool.Entries.Remove(entry);
            MonsterEntryListBox.Items.Remove(entry);
        }

        private void MonsterPoolListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshMonsterEntries();
        }

        private void BossPoolListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshBossEntries();
        }

        private void AddBossPoolButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];

            var pool = new ActivityMonsterPool { Name = "BOSS池" + (a.BossPools.Count + 1) };
            a.BossPools.Add(pool);
            RefreshBossPools();
        }

        private void RemoveBossPoolButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var a = _selectedActivities[0];
            var pool = GetSelectedBossPool();
            if (pool == null) return;

            a.BossPools.Remove(pool);
            RefreshBossPools();
        }

        private void AddBossEntryButton_Click(object sender, EventArgs e)
        {
            if (_selectedActivities == null || _selectedActivities.Count == 0) return;
            var pool = GetSelectedBossPool();
            if (pool == null) return;
            if (MonsterInfoComboBox.SelectedItem is not MonsterInfo monInfo) return;
            if (!int.TryParse(MonsterWeightTextBox.Text, out var weight)) return;
            if (weight < 1) weight = 1;

            // BOSS 固定 1 只，通过权重随机
            var entry = new ActivityMonsterEntry
            {
                MonsterInfoIndex = monInfo.Index,
                MinCount = 1,
                MaxCount = 1,
                Weight = weight
            };

            pool.Entries.Add(entry);
            BossEntryListBox.Items.Add(entry);
        }

        private void RemoveBossEntryButton_Click(object sender, EventArgs e)
        {
            var pool = GetSelectedBossPool();
            var entry = GetSelectedBossEntry();
            if (pool == null || entry == null) return;

            pool.Entries.Remove(entry);
            BossEntryListBox.Items.Remove(entry);
        }

        private void grpReward_Enter(object sender, EventArgs e)
        {

        }
    }
}

