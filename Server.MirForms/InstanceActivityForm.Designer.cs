using System.Windows.Forms;

namespace Server
{
    partial class InstanceActivityForm
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl MainTabControl;
        private TabPage BasicTabPage;
        private TabPage TimeMapTabPage;
        private TabPage MonstersTabPage;
        private TabPage RewardTabPage;
        private GroupBox grpActivities;
        private ListBox ActivityListBox;
        private GroupBox grpBasic;
        private TextBox ActivityNameTextBox;
        private CheckBox EnabledCheckBox;
        private Button AddButton;
        private Button RemoveButton;
        private Label NameLabel;
        private GroupBox grpTimeMap;
        private ListBox TimeSlotListBox;
        private ComboBox DayComboBox;
        private TextBox StartHourTextBox;
        private TextBox StartMinuteTextBox;
        private TextBox EndHourTextBox;
        private TextBox EndMinuteTextBox;
        private Button AddTimeSlotButton;
        private Button RemoveTimeSlotButton;
        private CheckedListBox EntranceMapsCheckedListBox;
        private CheckedListBox InstanceMapsCheckedListBox;
        private Label TimeSlotsLabel;
        private Label EntranceMapsLabel;
        private Label InstanceMapsLabel;
        private Label EntranceNpcCountLabel;
        private TextBox EntranceNpcCountTextBox;
        private GroupBox grpReward;
        private ListBox RewardListBox;
        private Button AddRewardButton;
        private Button RemoveRewardButton;
        private TextBox RewardMinScoreTextBox;
        private TextBox RewardMaxScoreTextBox;
        private TextBox RewardExpTextBox;
        private TextBox RewardGoldTextBox;
        private Label RewardLabel;
        private Label RewardMinScoreLabel;
        private Label RewardMaxScoreLabel;
        private Label RewardExpLabel;
        private Label RewardGoldLabel;
        private Label RewardItemLabel;
        private Label RewardItemCountLabel;
        private ListBox RewardItemsListBox;
        private Button AddRewardItemButton;
        private Button RemoveRewardItemButton;
        private Button AddRewardItemsBatchButton;
        private ComboBox RewardItemComboBox;
        private TextBox RewardItemCountTextBox;
        private GroupBox grpMonsters;
        private ListBox MonsterPoolListBox;
        private ListBox MonsterEntryListBox;
        private ListBox BossPoolListBox;
        private ListBox BossEntryListBox;
        private Label MonsterInfoLabel;
        private Label MonsterMinLabel;
        private Label MonsterMaxLabel;
        private Label MonsterWeightLabel;
        private ComboBox MonsterInfoComboBox;
        private TextBox MonsterMinCountTextBox;
        private TextBox MonsterMaxCountTextBox;
        private TextBox MonsterWeightTextBox;
        private Button AddMonsterPoolButton;
        private Button RemoveMonsterPoolButton;
        private Button AddMonsterEntryButton;
        private Button RemoveMonsterEntryButton;
        private Button AddBossPoolButton;
        private Button RemoveBossPoolButton;
        private Button AddBossEntryButton;
        private Button RemoveBossEntryButton;
        private Label MonsterPoolsLabel;
        private Label MonsterEntriesLabel;
        private Label BossPoolsLabel;
        private Label BossEntriesLabel;
        private CheckBox BossAfterClearMonstersCheckBox;
        private Label MonsterWaveCountLabel;
        private TextBox MonsterWaveCountTextBox;
        private Label DailyLimitLabel;
        private TextBox DailyLimitTextBox;
        private Label InstanceTimeLimitLabel;
        private TextBox InstanceTimeLimitTextBox;
        private Label ExitDelaySecondsLabel;
        private TextBox ExitDelaySecondsTextBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            MainTabControl = new TabControl();
            BasicTabPage = new TabPage();
            grpBasic = new GroupBox();
            InstanceTimeLimitTextBox = new TextBox();
            InstanceTimeLimitLabel = new Label();
            ExitDelaySecondsTextBox = new TextBox();
            ExitDelaySecondsLabel = new Label();
            DailyLimitTextBox = new TextBox();
            DailyLimitLabel = new Label();
            ActivityNameTextBox = new TextBox();
            EnabledCheckBox = new CheckBox();
            NameLabel = new Label();
            grpActivities = new GroupBox();
            ActivityListBox = new ListBox();
            AddButton = new Button();
            RemoveButton = new Button();
            TimeMapTabPage = new TabPage();
            grpTimeMap = new GroupBox();
            EntranceNpcCountLabel = new Label();
            EntranceNpcCountTextBox = new TextBox();
            TimeSlotsLabel = new Label();
            TimeSlotListBox = new ListBox();
            DayComboBox = new ComboBox();
            StartHourTextBox = new TextBox();
            StartMinuteTextBox = new TextBox();
            EndHourTextBox = new TextBox();
            EndMinuteTextBox = new TextBox();
            AddTimeSlotButton = new Button();
            RemoveTimeSlotButton = new Button();
            EntranceMapsLabel = new Label();
            InstanceMapsLabel = new Label();
            EntranceMapsCheckedListBox = new CheckedListBox();
            InstanceMapsCheckedListBox = new CheckedListBox();
            MonstersTabPage = new TabPage();
            grpMonsters = new GroupBox();
            BossEntriesLabel = new Label();
            BossPoolsLabel = new Label();
            MonsterEntriesLabel = new Label();
            MonsterPoolsLabel = new Label();
            RemoveBossEntryButton = new Button();
            AddBossEntryButton = new Button();
            RemoveBossPoolButton = new Button();
            AddBossPoolButton = new Button();
            BossEntryListBox = new ListBox();
            BossPoolListBox = new ListBox();
            MonsterWeightLabel = new Label();
            MonsterMaxLabel = new Label();
            MonsterMinLabel = new Label();
            MonsterInfoLabel = new Label();
            AddMonsterEntryButton = new Button();
            RemoveMonsterEntryButton = new Button();
            AddMonsterPoolButton = new Button();
            RemoveMonsterPoolButton = new Button();
            MonsterWeightTextBox = new TextBox();
            MonsterMaxCountTextBox = new TextBox();
            MonsterMinCountTextBox = new TextBox();
            MonsterInfoComboBox = new ComboBox();
            MonsterEntryListBox = new ListBox();
            MonsterPoolListBox = new ListBox();
            BossAfterClearMonstersCheckBox = new CheckBox();
            MonsterWaveCountTextBox = new TextBox();
            MonsterWaveCountLabel = new Label();
            RewardTabPage = new TabPage();
            grpReward = new GroupBox();
            RewardItemCountLabel = new Label();
            RewardItemLabel = new Label();
            RewardGoldLabel = new Label();
            RewardExpLabel = new Label();
            RewardMaxScoreLabel = new Label();
            RewardMinScoreLabel = new Label();
            RewardLabel = new Label();
            RewardListBox = new ListBox();
            AddRewardButton = new Button();
            RemoveRewardButton = new Button();
            RewardMinScoreTextBox = new TextBox();
            RewardMaxScoreTextBox = new TextBox();
            RewardExpTextBox = new TextBox();
            RewardGoldTextBox = new TextBox();
            RewardItemsListBox = new ListBox();
            AddRewardItemButton = new Button();
            RemoveRewardItemButton = new Button();
            AddRewardItemsBatchButton = new Button();
            RewardItemComboBox = new ComboBox();
            RewardItemCountTextBox = new TextBox();
            MainTabControl.SuspendLayout();
            BasicTabPage.SuspendLayout();
            grpBasic.SuspendLayout();
            grpActivities.SuspendLayout();
            TimeMapTabPage.SuspendLayout();
            grpTimeMap.SuspendLayout();
            MonstersTabPage.SuspendLayout();
            grpMonsters.SuspendLayout();
            RewardTabPage.SuspendLayout();
            grpReward.SuspendLayout();
            SuspendLayout();
            // 
            // MainTabControl
            // 
            MainTabControl.Controls.Add(BasicTabPage);
            MainTabControl.Controls.Add(TimeMapTabPage);
            MainTabControl.Controls.Add(MonstersTabPage);
            MainTabControl.Controls.Add(RewardTabPage);
            MainTabControl.Location = new Point(5, 5);
            MainTabControl.Name = "MainTabControl";
            MainTabControl.SelectedIndex = 0;
            MainTabControl.Size = new Size(630, 461);
            MainTabControl.TabIndex = 0;
            // 
            // BasicTabPage
            // 
            BasicTabPage.Controls.Add(grpBasic);
            BasicTabPage.Controls.Add(grpActivities);
            BasicTabPage.Location = new Point(4, 26);
            BasicTabPage.Name = "BasicTabPage";
            BasicTabPage.Padding = new Padding(3);
            BasicTabPage.Size = new Size(622, 431);
            BasicTabPage.TabIndex = 0;
            BasicTabPage.Text = "基础";
            BasicTabPage.UseVisualStyleBackColor = true;
            // 
            // grpBasic
            // 
            grpBasic.Controls.Add(InstanceTimeLimitTextBox);
            grpBasic.Controls.Add(InstanceTimeLimitLabel);
            grpBasic.Controls.Add(ExitDelaySecondsTextBox);
            grpBasic.Controls.Add(ExitDelaySecondsLabel);
            grpBasic.Controls.Add(DailyLimitTextBox);
            grpBasic.Controls.Add(DailyLimitLabel);
            grpBasic.Controls.Add(ActivityNameTextBox);
            grpBasic.Controls.Add(EnabledCheckBox);
            grpBasic.Controls.Add(NameLabel);
            grpBasic.Location = new Point(280, 10);
            grpBasic.Name = "grpBasic";
            grpBasic.Size = new Size(324, 220);
            grpBasic.TabIndex = 1;
            grpBasic.TabStop = false;
            grpBasic.Text = "基础信息";
            // 
            // InstanceTimeLimitTextBox
            // 
            InstanceTimeLimitTextBox.Location = new Point(118, 88);
            InstanceTimeLimitTextBox.Name = "InstanceTimeLimitTextBox";
            InstanceTimeLimitTextBox.Size = new Size(60, 23);
            InstanceTimeLimitTextBox.TabIndex = 3;
            // 
            // InstanceTimeLimitLabel
            // 
            InstanceTimeLimitLabel.AutoSize = true;
            InstanceTimeLimitLabel.Location = new Point(20, 91);
            InstanceTimeLimitLabel.Name = "InstanceTimeLimitLabel";
            InstanceTimeLimitLabel.Size = new Size(88, 17);
            InstanceTimeLimitLabel.TabIndex = 7;
            InstanceTimeLimitLabel.Text = "副本限时(分钟)";
            // 
            // ExitDelaySecondsTextBox
            // 
            ExitDelaySecondsTextBox.Location = new Point(118, 117);
            ExitDelaySecondsTextBox.Name = "ExitDelaySecondsTextBox";
            ExitDelaySecondsTextBox.Size = new Size(60, 23);
            ExitDelaySecondsTextBox.TabIndex = 4;
            // 
            // ExitDelaySecondsLabel
            // 
            ExitDelaySecondsLabel.AutoSize = true;
            ExitDelaySecondsLabel.Location = new Point(20, 120);
            ExitDelaySecondsLabel.Name = "ExitDelaySecondsLabel";
            ExitDelaySecondsLabel.Size = new Size(76, 17);
            ExitDelaySecondsLabel.TabIndex = 8;
            ExitDelaySecondsLabel.Text = "通关停留(秒)";
            // 
            // DailyLimitTextBox
            // 
            DailyLimitTextBox.Location = new Point(118, 59);
            DailyLimitTextBox.Name = "DailyLimitTextBox";
            DailyLimitTextBox.Size = new Size(60, 23);
            DailyLimitTextBox.TabIndex = 2;
            DailyLimitTextBox.TextChanged += DailyLimitTextBox_TextChanged;
            // 
            // DailyLimitLabel
            // 
            DailyLimitLabel.AutoSize = true;
            DailyLimitLabel.Location = new Point(20, 62);
            DailyLimitLabel.Name = "DailyLimitLabel";
            DailyLimitLabel.Size = new Size(80, 17);
            DailyLimitLabel.TabIndex = 6;
            DailyLimitLabel.Text = "每日进入次数";
            // 
            // ActivityNameTextBox
            // 
            ActivityNameTextBox.Location = new Point(57, 27);
            ActivityNameTextBox.Margin = new Padding(4);
            ActivityNameTextBox.Name = "ActivityNameTextBox";
            ActivityNameTextBox.Size = new Size(160, 23);
            ActivityNameTextBox.TabIndex = 0;
            ActivityNameTextBox.TextChanged += ActivityNameTextBox_TextChanged;
            // 
            // EnabledCheckBox
            // 
            EnabledCheckBox.AutoSize = true;
            EnabledCheckBox.Location = new Point(228, 34);
            EnabledCheckBox.Margin = new Padding(4);
            EnabledCheckBox.Name = "EnabledCheckBox";
            EnabledCheckBox.Size = new Size(75, 21);
            EnabledCheckBox.TabIndex = 1;
            EnabledCheckBox.Text = "启用活动";
            EnabledCheckBox.UseVisualStyleBackColor = true;
            EnabledCheckBox.CheckedChanged += EnabledCheckBox_CheckedChanged;
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.Location = new Point(20, 30);
            NameLabel.Margin = new Padding(4, 0, 4, 0);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(32, 17);
            NameLabel.TabIndex = 5;
            NameLabel.Text = "名称";
            // 
            // grpActivities
            // 
            grpActivities.Controls.Add(ActivityListBox);
            grpActivities.Controls.Add(AddButton);
            grpActivities.Controls.Add(RemoveButton);
            grpActivities.Location = new Point(10, 10);
            grpActivities.Name = "grpActivities";
            grpActivities.Size = new Size(260, 220);
            grpActivities.TabIndex = 0;
            grpActivities.TabStop = false;
            grpActivities.Text = "活动列表";
            // 
            // ActivityListBox
            // 
            ActivityListBox.Dock = DockStyle.Top;
            ActivityListBox.FormattingEnabled = true;
            ActivityListBox.ItemHeight = 17;
            ActivityListBox.Location = new Point(3, 19);
            ActivityListBox.Margin = new Padding(4);
            ActivityListBox.Name = "ActivityListBox";
            ActivityListBox.SelectionMode = SelectionMode.MultiExtended;
            ActivityListBox.Size = new Size(254, 140);
            ActivityListBox.TabIndex = 0;
            ActivityListBox.SelectedIndexChanged += ActivityListBox_SelectedIndexChanged;
            // 
            // AddButton
            // 
            AddButton.Location = new Point(10, 170);
            AddButton.Margin = new Padding(4);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(70, 30);
            AddButton.TabIndex = 1;
            AddButton.Text = "新增";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // RemoveButton
            // 
            RemoveButton.Location = new Point(90, 170);
            RemoveButton.Margin = new Padding(4);
            RemoveButton.Name = "RemoveButton";
            RemoveButton.Size = new Size(70, 30);
            RemoveButton.TabIndex = 2;
            RemoveButton.Text = "删除";
            RemoveButton.UseVisualStyleBackColor = true;
            RemoveButton.Click += RemoveButton_Click;
            // 
            // TimeMapTabPage
            // 
            TimeMapTabPage.Controls.Add(grpTimeMap);
            TimeMapTabPage.Location = new Point(4, 26);
            TimeMapTabPage.Name = "TimeMapTabPage";
            TimeMapTabPage.Padding = new Padding(3);
            TimeMapTabPage.Size = new Size(622, 431);
            TimeMapTabPage.TabIndex = 1;
            TimeMapTabPage.Text = "时间与地图";
            TimeMapTabPage.UseVisualStyleBackColor = true;
            // 
            // grpTimeMap
            // 
            grpTimeMap.Controls.Add(EntranceNpcCountLabel);
            grpTimeMap.Controls.Add(EntranceNpcCountTextBox);
            grpTimeMap.Controls.Add(TimeSlotsLabel);
            grpTimeMap.Controls.Add(TimeSlotListBox);
            grpTimeMap.Controls.Add(DayComboBox);
            grpTimeMap.Controls.Add(StartHourTextBox);
            grpTimeMap.Controls.Add(StartMinuteTextBox);
            grpTimeMap.Controls.Add(EndHourTextBox);
            grpTimeMap.Controls.Add(EndMinuteTextBox);
            grpTimeMap.Controls.Add(AddTimeSlotButton);
            grpTimeMap.Controls.Add(RemoveTimeSlotButton);
            grpTimeMap.Controls.Add(EntranceMapsLabel);
            grpTimeMap.Controls.Add(InstanceMapsLabel);
            grpTimeMap.Controls.Add(EntranceMapsCheckedListBox);
            grpTimeMap.Controls.Add(InstanceMapsCheckedListBox);
            grpTimeMap.Location = new Point(6, 6);
            grpTimeMap.Name = "grpTimeMap";
            grpTimeMap.Size = new Size(594, 250);
            grpTimeMap.TabIndex = 2;
            grpTimeMap.TabStop = false;
            grpTimeMap.Text = "时间与地图";
            // 
            // EntranceNpcCountLabel
            // 
            EntranceNpcCountLabel.AutoSize = true;
            EntranceNpcCountLabel.Location = new Point(20, 212);
            EntranceNpcCountLabel.Name = "EntranceNpcCountLabel";
            EntranceNpcCountLabel.Size = new Size(81, 17);
            EntranceNpcCountLabel.TabIndex = 13;
            EntranceNpcCountLabel.Text = "入口NPC数量";
            // 
            // EntranceNpcCountTextBox
            // 
            EntranceNpcCountTextBox.Location = new Point(107, 209);
            EntranceNpcCountTextBox.Name = "EntranceNpcCountTextBox";
            EntranceNpcCountTextBox.Size = new Size(44, 23);
            EntranceNpcCountTextBox.TabIndex = 14;
            EntranceNpcCountTextBox.TextChanged += EntranceNpcCountTextBox_TextChanged;
            // 
            // TimeSlotsLabel
            // 
            TimeSlotsLabel.AutoSize = true;
            TimeSlotsLabel.Location = new Point(340, 18);
            TimeSlotsLabel.Name = "TimeSlotsLabel";
            TimeSlotsLabel.Size = new Size(68, 17);
            TimeSlotsLabel.TabIndex = 10;
            TimeSlotsLabel.Text = "时间段配置";
            // 
            // TimeSlotListBox
            // 
            TimeSlotListBox.FormattingEnabled = true;
            TimeSlotListBox.ItemHeight = 17;
            TimeSlotListBox.Location = new Point(340, 36);
            TimeSlotListBox.Name = "TimeSlotListBox";
            TimeSlotListBox.Size = new Size(233, 89);
            TimeSlotListBox.TabIndex = 0;
            TimeSlotListBox.SelectedIndexChanged += TimeSlotListBox_SelectedIndexChanged;
            // 
            // DayComboBox
            // 
            DayComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DayComboBox.FormattingEnabled = true;
            DayComboBox.Location = new Point(340, 136);
            DayComboBox.Name = "DayComboBox";
            DayComboBox.Size = new Size(121, 25);
            DayComboBox.TabIndex = 1;
            // 
            // StartHourTextBox
            // 
            StartHourTextBox.Location = new Point(467, 136);
            StartHourTextBox.Name = "StartHourTextBox";
            StartHourTextBox.Size = new Size(32, 23);
            StartHourTextBox.TabIndex = 2;
            // 
            // StartMinuteTextBox
            // 
            StartMinuteTextBox.Location = new Point(505, 136);
            StartMinuteTextBox.Name = "StartMinuteTextBox";
            StartMinuteTextBox.Size = new Size(32, 23);
            StartMinuteTextBox.TabIndex = 3;
            // 
            // EndHourTextBox
            // 
            EndHourTextBox.Location = new Point(467, 165);
            EndHourTextBox.Name = "EndHourTextBox";
            EndHourTextBox.Size = new Size(32, 23);
            EndHourTextBox.TabIndex = 4;
            // 
            // EndMinuteTextBox
            // 
            EndMinuteTextBox.Location = new Point(505, 165);
            EndMinuteTextBox.Name = "EndMinuteTextBox";
            EndMinuteTextBox.Size = new Size(32, 23);
            EndMinuteTextBox.TabIndex = 5;
            // 
            // AddTimeSlotButton
            // 
            AddTimeSlotButton.Location = new Point(340, 194);
            AddTimeSlotButton.Name = "AddTimeSlotButton";
            AddTimeSlotButton.Size = new Size(70, 27);
            AddTimeSlotButton.TabIndex = 6;
            AddTimeSlotButton.Text = "新增时段";
            AddTimeSlotButton.UseVisualStyleBackColor = true;
            AddTimeSlotButton.Click += AddTimeSlotButton_Click;
            // 
            // RemoveTimeSlotButton
            // 
            RemoveTimeSlotButton.Location = new Point(416, 194);
            RemoveTimeSlotButton.Name = "RemoveTimeSlotButton";
            RemoveTimeSlotButton.Size = new Size(70, 27);
            RemoveTimeSlotButton.TabIndex = 7;
            RemoveTimeSlotButton.Text = "删除时段";
            RemoveTimeSlotButton.UseVisualStyleBackColor = true;
            RemoveTimeSlotButton.Click += RemoveTimeSlotButton_Click;
            // 
            // EntranceMapsLabel
            // 
            EntranceMapsLabel.AutoSize = true;
            EntranceMapsLabel.Location = new Point(20, 32);
            EntranceMapsLabel.Name = "EntranceMapsLabel";
            EntranceMapsLabel.Size = new Size(88, 17);
            EntranceMapsLabel.TabIndex = 11;
            EntranceMapsLabel.Text = "入口地图(随机)";
            // 
            // InstanceMapsLabel
            // 
            InstanceMapsLabel.AutoSize = true;
            InstanceMapsLabel.Location = new Point(166, 32);
            InstanceMapsLabel.Name = "InstanceMapsLabel";
            InstanceMapsLabel.Size = new Size(100, 17);
            InstanceMapsLabel.TabIndex = 12;
            InstanceMapsLabel.Text = "副本地图池(随机)";
            // 
            // EntranceMapsCheckedListBox
            // 
            EntranceMapsCheckedListBox.CheckOnClick = true;
            EntranceMapsCheckedListBox.FormattingEnabled = true;
            EntranceMapsCheckedListBox.Location = new Point(20, 52);
            EntranceMapsCheckedListBox.Name = "EntranceMapsCheckedListBox";
            EntranceMapsCheckedListBox.Size = new Size(130, 148);
            EntranceMapsCheckedListBox.TabIndex = 8;
            EntranceMapsCheckedListBox.ItemCheck += EntranceMapsCheckedListBox_ItemCheck;
            // 
            // InstanceMapsCheckedListBox
            // 
            InstanceMapsCheckedListBox.CheckOnClick = true;
            InstanceMapsCheckedListBox.FormattingEnabled = true;
            InstanceMapsCheckedListBox.Location = new Point(166, 52);
            InstanceMapsCheckedListBox.Name = "InstanceMapsCheckedListBox";
            InstanceMapsCheckedListBox.Size = new Size(130, 148);
            InstanceMapsCheckedListBox.TabIndex = 9;
            InstanceMapsCheckedListBox.ItemCheck += InstanceMapsCheckedListBox_ItemCheck;
            // 
            // MonstersTabPage
            // 
            MonstersTabPage.Controls.Add(grpMonsters);
            MonstersTabPage.Location = new Point(4, 26);
            MonstersTabPage.Name = "MonstersTabPage";
            MonstersTabPage.Padding = new Padding(3);
            MonstersTabPage.Size = new Size(622, 431);
            MonstersTabPage.TabIndex = 2;
            MonstersTabPage.Text = "怪物与BOSS";
            MonstersTabPage.UseVisualStyleBackColor = true;
            // 
            // grpMonsters
            // 
            grpMonsters.Controls.Add(BossEntriesLabel);
            grpMonsters.Controls.Add(BossPoolsLabel);
            grpMonsters.Controls.Add(MonsterEntriesLabel);
            grpMonsters.Controls.Add(MonsterPoolsLabel);
            grpMonsters.Controls.Add(RemoveBossEntryButton);
            grpMonsters.Controls.Add(AddBossEntryButton);
            grpMonsters.Controls.Add(RemoveBossPoolButton);
            grpMonsters.Controls.Add(AddBossPoolButton);
            grpMonsters.Controls.Add(BossEntryListBox);
            grpMonsters.Controls.Add(BossPoolListBox);
            grpMonsters.Controls.Add(MonsterWeightLabel);
            grpMonsters.Controls.Add(MonsterMaxLabel);
            grpMonsters.Controls.Add(MonsterMinLabel);
            grpMonsters.Controls.Add(MonsterInfoLabel);
            grpMonsters.Controls.Add(AddMonsterEntryButton);
            grpMonsters.Controls.Add(RemoveMonsterEntryButton);
            grpMonsters.Controls.Add(AddMonsterPoolButton);
            grpMonsters.Controls.Add(RemoveMonsterPoolButton);
            grpMonsters.Controls.Add(MonsterWeightTextBox);
            grpMonsters.Controls.Add(MonsterMaxCountTextBox);
            grpMonsters.Controls.Add(MonsterMinCountTextBox);
            grpMonsters.Controls.Add(MonsterInfoComboBox);
            grpMonsters.Controls.Add(MonsterEntryListBox);
            grpMonsters.Controls.Add(MonsterPoolListBox);
            grpMonsters.Controls.Add(BossAfterClearMonstersCheckBox);
            grpMonsters.Controls.Add(MonsterWaveCountTextBox);
            grpMonsters.Controls.Add(MonsterWaveCountLabel);
            grpMonsters.Location = new Point(6, 6);
            grpMonsters.Name = "grpMonsters";
            grpMonsters.Size = new Size(594, 410);
            grpMonsters.TabIndex = 4;
            grpMonsters.TabStop = false;
            grpMonsters.Text = "怪物与BOSS";
            // 
            // BossEntriesLabel
            // 
            BossEntriesLabel.AutoSize = true;
            BossEntriesLabel.Location = new Point(290, 224);
            BossEntriesLabel.Name = "BossEntriesLabel";
            BossEntriesLabel.Size = new Size(64, 17);
            BossEntriesLabel.TabIndex = 20;
            BossEntriesLabel.Text = "BOSS列表";
            // 
            // BossPoolsLabel
            // 
            BossPoolsLabel.AutoSize = true;
            BossPoolsLabel.Location = new Point(18, 224);
            BossPoolsLabel.Name = "BossPoolsLabel";
            BossPoolsLabel.Size = new Size(76, 17);
            BossPoolsLabel.TabIndex = 19;
            BossPoolsLabel.Text = "BOSS池列表";
            // 
            // MonsterEntriesLabel
            // 
            MonsterEntriesLabel.AutoSize = true;
            MonsterEntriesLabel.Location = new Point(204, 18);
            MonsterEntriesLabel.Name = "MonsterEntriesLabel";
            MonsterEntriesLabel.Size = new Size(56, 17);
            MonsterEntriesLabel.TabIndex = 18;
            MonsterEntriesLabel.Text = "怪物列表";
            // 
            // MonsterPoolsLabel
            // 
            MonsterPoolsLabel.AutoSize = true;
            MonsterPoolsLabel.Location = new Point(18, 18);
            MonsterPoolsLabel.Name = "MonsterPoolsLabel";
            MonsterPoolsLabel.Size = new Size(68, 17);
            MonsterPoolsLabel.TabIndex = 17;
            MonsterPoolsLabel.Text = "怪物池列表";
            // 
            // RemoveBossEntryButton
            // 
            RemoveBossEntryButton.Location = new Point(376, 372);
            RemoveBossEntryButton.Name = "RemoveBossEntryButton";
            RemoveBossEntryButton.Size = new Size(80, 27);
            RemoveBossEntryButton.TabIndex = 16;
            RemoveBossEntryButton.Text = "删除BOSS";
            RemoveBossEntryButton.UseVisualStyleBackColor = true;
            RemoveBossEntryButton.Click += RemoveBossEntryButton_Click;
            // 
            // AddBossEntryButton
            // 
            AddBossEntryButton.Location = new Point(290, 372);
            AddBossEntryButton.Name = "AddBossEntryButton";
            AddBossEntryButton.Size = new Size(80, 27);
            AddBossEntryButton.TabIndex = 15;
            AddBossEntryButton.Text = "新增BOSS";
            AddBossEntryButton.UseVisualStyleBackColor = true;
            AddBossEntryButton.Click += AddBossEntryButton_Click;
            // 
            // RemoveBossPoolButton
            // 
            RemoveBossPoolButton.Location = new Point(104, 372);
            RemoveBossPoolButton.Name = "RemoveBossPoolButton";
            RemoveBossPoolButton.Size = new Size(80, 27);
            RemoveBossPoolButton.TabIndex = 14;
            RemoveBossPoolButton.Text = "删除BOSS池";
            RemoveBossPoolButton.UseVisualStyleBackColor = true;
            RemoveBossPoolButton.Click += RemoveBossPoolButton_Click;
            // 
            // AddBossPoolButton
            // 
            AddBossPoolButton.Location = new Point(18, 372);
            AddBossPoolButton.Name = "AddBossPoolButton";
            AddBossPoolButton.Size = new Size(80, 27);
            AddBossPoolButton.TabIndex = 13;
            AddBossPoolButton.Text = "新增BOSS池";
            AddBossPoolButton.UseVisualStyleBackColor = true;
            AddBossPoolButton.Click += AddBossPoolButton_Click;
            // 
            // BossEntryListBox
            // 
            BossEntryListBox.FormattingEnabled = true;
            BossEntryListBox.ItemHeight = 17;
            BossEntryListBox.Location = new Point(290, 244);
            BossEntryListBox.Name = "BossEntryListBox";
            BossEntryListBox.Size = new Size(284, 123);
            BossEntryListBox.TabIndex = 12;
            // 
            // BossPoolListBox
            // 
            BossPoolListBox.FormattingEnabled = true;
            BossPoolListBox.ItemHeight = 17;
            BossPoolListBox.Location = new Point(18, 244);
            BossPoolListBox.Name = "BossPoolListBox";
            BossPoolListBox.Size = new Size(266, 123);
            BossPoolListBox.TabIndex = 11;
            BossPoolListBox.SelectedIndexChanged += BossPoolListBox_SelectedIndexChanged;
            // 
            // MonsterWeightLabel
            // 
            MonsterWeightLabel.AutoSize = true;
            MonsterWeightLabel.Location = new Point(486, 80);
            MonsterWeightLabel.Name = "MonsterWeightLabel";
            MonsterWeightLabel.Size = new Size(32, 17);
            MonsterWeightLabel.TabIndex = 24;
            MonsterWeightLabel.Text = "权重";
            // 
            // MonsterMaxLabel
            // 
            MonsterMaxLabel.AutoSize = true;
            MonsterMaxLabel.Location = new Point(440, 80);
            MonsterMaxLabel.Name = "MonsterMaxLabel";
            MonsterMaxLabel.Size = new Size(32, 17);
            MonsterMaxLabel.TabIndex = 23;
            MonsterMaxLabel.Text = "最大";
            // 
            // MonsterMinLabel
            // 
            MonsterMinLabel.AutoSize = true;
            MonsterMinLabel.Location = new Point(394, 80);
            MonsterMinLabel.Name = "MonsterMinLabel";
            MonsterMinLabel.Size = new Size(32, 17);
            MonsterMinLabel.TabIndex = 22;
            MonsterMinLabel.Text = "最小";
            // 
            // MonsterInfoLabel
            // 
            MonsterInfoLabel.AutoSize = true;
            MonsterInfoLabel.Location = new Point(392, 19);
            MonsterInfoLabel.Name = "MonsterInfoLabel";
            MonsterInfoLabel.Size = new Size(56, 17);
            MonsterInfoLabel.TabIndex = 21;
            MonsterInfoLabel.Text = "怪物选择";
            // 
            // AddMonsterEntryButton
            // 
            AddMonsterEntryButton.Location = new Point(204, 170);
            AddMonsterEntryButton.Name = "AddMonsterEntryButton";
            AddMonsterEntryButton.Size = new Size(80, 27);
            AddMonsterEntryButton.TabIndex = 8;
            AddMonsterEntryButton.Text = "新增怪物";
            AddMonsterEntryButton.UseVisualStyleBackColor = true;
            AddMonsterEntryButton.Click += AddMonsterEntryButton_Click;
            // 
            // RemoveMonsterEntryButton
            // 
            RemoveMonsterEntryButton.Location = new Point(290, 170);
            RemoveMonsterEntryButton.Name = "RemoveMonsterEntryButton";
            RemoveMonsterEntryButton.Size = new Size(80, 27);
            RemoveMonsterEntryButton.TabIndex = 9;
            RemoveMonsterEntryButton.Text = "删除怪物";
            RemoveMonsterEntryButton.UseVisualStyleBackColor = true;
            RemoveMonsterEntryButton.Click += RemoveMonsterEntryButton_Click;
            // 
            // AddMonsterPoolButton
            // 
            AddMonsterPoolButton.Location = new Point(18, 170);
            AddMonsterPoolButton.Name = "AddMonsterPoolButton";
            AddMonsterPoolButton.Size = new Size(80, 27);
            AddMonsterPoolButton.TabIndex = 6;
            AddMonsterPoolButton.Text = "新增池";
            AddMonsterPoolButton.UseVisualStyleBackColor = true;
            AddMonsterPoolButton.Click += AddMonsterPoolButton_Click;
            // 
            // RemoveMonsterPoolButton
            // 
            RemoveMonsterPoolButton.Location = new Point(104, 170);
            RemoveMonsterPoolButton.Name = "RemoveMonsterPoolButton";
            RemoveMonsterPoolButton.Size = new Size(80, 27);
            RemoveMonsterPoolButton.TabIndex = 7;
            RemoveMonsterPoolButton.Text = "删除池";
            RemoveMonsterPoolButton.UseVisualStyleBackColor = true;
            RemoveMonsterPoolButton.Click += RemoveMonsterPoolButton_Click;
            // 
            // MonsterWeightTextBox
            // 
            MonsterWeightTextBox.Location = new Point(486, 97);
            MonsterWeightTextBox.Name = "MonsterWeightTextBox";
            MonsterWeightTextBox.Size = new Size(40, 23);
            MonsterWeightTextBox.TabIndex = 5;
            MonsterWeightTextBox.Text = "1";
            // 
            // MonsterMaxCountTextBox
            // 
            MonsterMaxCountTextBox.Location = new Point(440, 97);
            MonsterMaxCountTextBox.Name = "MonsterMaxCountTextBox";
            MonsterMaxCountTextBox.Size = new Size(40, 23);
            MonsterMaxCountTextBox.TabIndex = 4;
            MonsterMaxCountTextBox.Text = "3";
            // 
            // MonsterMinCountTextBox
            // 
            MonsterMinCountTextBox.Location = new Point(394, 97);
            MonsterMinCountTextBox.Name = "MonsterMinCountTextBox";
            MonsterMinCountTextBox.Size = new Size(40, 23);
            MonsterMinCountTextBox.TabIndex = 3;
            MonsterMinCountTextBox.Text = "1";
            // 
            // MonsterInfoComboBox
            // 
            MonsterInfoComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            MonsterInfoComboBox.FormattingEnabled = true;
            MonsterInfoComboBox.Location = new Point(394, 39);
            MonsterInfoComboBox.Name = "MonsterInfoComboBox";
            MonsterInfoComboBox.Size = new Size(140, 25);
            MonsterInfoComboBox.TabIndex = 2;
            // 
            // MonsterEntryListBox
            // 
            MonsterEntryListBox.FormattingEnabled = true;
            MonsterEntryListBox.ItemHeight = 17;
            MonsterEntryListBox.Location = new Point(204, 38);
            MonsterEntryListBox.Name = "MonsterEntryListBox";
            MonsterEntryListBox.Size = new Size(180, 123);
            MonsterEntryListBox.TabIndex = 1;
            // 
            // MonsterPoolListBox
            // 
            MonsterPoolListBox.FormattingEnabled = true;
            MonsterPoolListBox.ItemHeight = 17;
            MonsterPoolListBox.Location = new Point(18, 38);
            MonsterPoolListBox.Name = "MonsterPoolListBox";
            MonsterPoolListBox.Size = new Size(180, 123);
            MonsterPoolListBox.TabIndex = 0;
            MonsterPoolListBox.SelectedIndexChanged += MonsterPoolListBox_SelectedIndexChanged;
            // 
            // BossAfterClearMonstersCheckBox
            // 
            BossAfterClearMonstersCheckBox.AutoSize = true;
            BossAfterClearMonstersCheckBox.Location = new Point(462, 20);
            BossAfterClearMonstersCheckBox.Name = "BossAfterClearMonstersCheckBox";
            BossAfterClearMonstersCheckBox.Size = new Size(107, 21);
            BossAfterClearMonstersCheckBox.TabIndex = 21;
            BossAfterClearMonstersCheckBox.Text = "清怪后刷BOSS";
            BossAfterClearMonstersCheckBox.UseVisualStyleBackColor = true;
            BossAfterClearMonstersCheckBox.CheckedChanged += BossAfterClearMonstersCheckBox_CheckedChanged;
            // 
            // MonsterWaveCountTextBox
            // 
            MonsterWaveCountTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MonsterWaveCountTextBox.Location = new Point(476, 138);
            MonsterWaveCountTextBox.Name = "MonsterWaveCountTextBox";
            MonsterWaveCountTextBox.Size = new Size(50, 23);
            MonsterWaveCountTextBox.TabIndex = 22;
            MonsterWaveCountTextBox.TextAlign = HorizontalAlignment.Right;
            // 
            // MonsterWaveCountLabel
            // 
            MonsterWaveCountLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MonsterWaveCountLabel.AutoSize = true;
            MonsterWaveCountLabel.Location = new Point(394, 144);
            MonsterWaveCountLabel.Name = "MonsterWaveCountLabel";
            MonsterWaveCountLabel.Size = new Size(79, 17);
            MonsterWaveCountLabel.TabIndex = 23;
            MonsterWaveCountLabel.Text = "小怪波数(波):";
            // 
            // RewardTabPage
            // 
            RewardTabPage.Controls.Add(grpReward);
            RewardTabPage.Location = new Point(4, 26);
            RewardTabPage.Name = "RewardTabPage";
            RewardTabPage.Padding = new Padding(3);
            RewardTabPage.Size = new Size(622, 431);
            RewardTabPage.TabIndex = 3;
            RewardTabPage.Text = "通关奖励";
            RewardTabPage.UseVisualStyleBackColor = true;
            // 
            // grpReward
            // 
            grpReward.Controls.Add(RewardItemCountLabel);
            grpReward.Controls.Add(RewardItemLabel);
            grpReward.Controls.Add(RewardGoldLabel);
            grpReward.Controls.Add(RewardExpLabel);
            grpReward.Controls.Add(RewardMaxScoreLabel);
            grpReward.Controls.Add(RewardMinScoreLabel);
            grpReward.Controls.Add(RewardLabel);
            grpReward.Controls.Add(RewardListBox);
            grpReward.Controls.Add(AddRewardButton);
            grpReward.Controls.Add(RemoveRewardButton);
            grpReward.Controls.Add(RewardMinScoreTextBox);
            grpReward.Controls.Add(RewardMaxScoreTextBox);
            grpReward.Controls.Add(RewardExpTextBox);
            grpReward.Controls.Add(RewardGoldTextBox);
            grpReward.Controls.Add(RewardItemsListBox);
            grpReward.Controls.Add(AddRewardItemButton);
            grpReward.Controls.Add(RemoveRewardItemButton);
            grpReward.Controls.Add(AddRewardItemsBatchButton);
            grpReward.Controls.Add(RewardItemComboBox);
            grpReward.Controls.Add(RewardItemCountTextBox);
            grpReward.Location = new Point(10, 10);
            grpReward.Name = "grpReward";
            grpReward.Size = new Size(594, 275);
            grpReward.TabIndex = 3;
            grpReward.TabStop = false;
            grpReward.Text = "通关奖励";
            grpReward.Enter += grpReward_Enter;
            // 
            // RewardItemCountLabel
            // 
            RewardItemCountLabel.AutoSize = true;
            RewardItemCountLabel.Location = new Point(536, 178);
            RewardItemCountLabel.Name = "RewardItemCountLabel";
            RewardItemCountLabel.Size = new Size(32, 17);
            RewardItemCountLabel.TabIndex = 18;
            RewardItemCountLabel.Text = "数量";
            // 
            // RewardItemLabel
            // 
            RewardItemLabel.AutoSize = true;
            RewardItemLabel.Location = new Point(334, 178);
            RewardItemLabel.Name = "RewardItemLabel";
            RewardItemLabel.Size = new Size(56, 17);
            RewardItemLabel.TabIndex = 17;
            RewardItemLabel.Text = "奖励物品";
            // 
            // RewardGoldLabel
            // 
            RewardGoldLabel.AutoSize = true;
            RewardGoldLabel.Location = new Point(522, 81);
            RewardGoldLabel.Name = "RewardGoldLabel";
            RewardGoldLabel.Size = new Size(32, 17);
            RewardGoldLabel.TabIndex = 16;
            RewardGoldLabel.Text = "金币";
            // 
            // RewardExpLabel
            // 
            RewardExpLabel.AutoSize = true;
            RewardExpLabel.Location = new Point(458, 81);
            RewardExpLabel.Name = "RewardExpLabel";
            RewardExpLabel.Size = new Size(32, 17);
            RewardExpLabel.TabIndex = 15;
            RewardExpLabel.Text = "经验";
            // 
            // RewardMaxScoreLabel
            // 
            RewardMaxScoreLabel.AutoSize = true;
            RewardMaxScoreLabel.Location = new Point(394, 80);
            RewardMaxScoreLabel.Name = "RewardMaxScoreLabel";
            RewardMaxScoreLabel.Size = new Size(56, 17);
            RewardMaxScoreLabel.TabIndex = 14;
            RewardMaxScoreLabel.Text = "最大评分";
            // 
            // RewardMinScoreLabel
            // 
            RewardMinScoreLabel.AutoSize = true;
            RewardMinScoreLabel.Location = new Point(332, 81);
            RewardMinScoreLabel.Name = "RewardMinScoreLabel";
            RewardMinScoreLabel.Size = new Size(56, 17);
            RewardMinScoreLabel.TabIndex = 13;
            RewardMinScoreLabel.Text = "最小评分";
            // 
            // RewardLabel
            // 
            RewardLabel.AutoSize = true;
            RewardLabel.Location = new Point(18, 18);
            RewardLabel.Name = "RewardLabel";
            RewardLabel.Size = new Size(80, 17);
            RewardLabel.TabIndex = 7;
            RewardLabel.Text = "通关奖励列表";
            // 
            // RewardListBox
            // 
            RewardListBox.FormattingEnabled = true;
            RewardListBox.ItemHeight = 17;
            RewardListBox.Location = new Point(18, 35);
            RewardListBox.Name = "RewardListBox";
            RewardListBox.Size = new Size(301, 89);
            RewardListBox.TabIndex = 0;
            RewardListBox.SelectedIndexChanged += RewardListBox_SelectedIndexChanged;
            // 
            // AddRewardButton
            // 
            AddRewardButton.Location = new Point(334, 35);
            AddRewardButton.Name = "AddRewardButton";
            AddRewardButton.Size = new Size(70, 27);
            AddRewardButton.TabIndex = 1;
            AddRewardButton.Text = "新增奖励";
            AddRewardButton.UseVisualStyleBackColor = true;
            AddRewardButton.Click += AddRewardButton_Click;
            // 
            // RemoveRewardButton
            // 
            RemoveRewardButton.Location = new Point(458, 35);
            RemoveRewardButton.Name = "RemoveRewardButton";
            RemoveRewardButton.Size = new Size(70, 27);
            RemoveRewardButton.TabIndex = 2;
            RemoveRewardButton.Text = "删除奖励";
            RemoveRewardButton.UseVisualStyleBackColor = true;
            RemoveRewardButton.Click += RemoveRewardButton_Click;
            // 
            // RewardMinScoreTextBox
            // 
            RewardMinScoreTextBox.Location = new Point(334, 101);
            RewardMinScoreTextBox.Name = "RewardMinScoreTextBox";
            RewardMinScoreTextBox.Size = new Size(54, 23);
            RewardMinScoreTextBox.TabIndex = 3;
            RewardMinScoreTextBox.Text = "1";
            // 
            // RewardMaxScoreTextBox
            // 
            RewardMaxScoreTextBox.Location = new Point(394, 100);
            RewardMaxScoreTextBox.Name = "RewardMaxScoreTextBox";
            RewardMaxScoreTextBox.Size = new Size(56, 23);
            RewardMaxScoreTextBox.TabIndex = 4;
            RewardMaxScoreTextBox.Text = "3";
            // 
            // RewardExpTextBox
            // 
            RewardExpTextBox.Location = new Point(456, 100);
            RewardExpTextBox.Name = "RewardExpTextBox";
            RewardExpTextBox.Size = new Size(60, 23);
            RewardExpTextBox.TabIndex = 5;
            RewardExpTextBox.Text = "0";
            // 
            // RewardGoldTextBox
            // 
            RewardGoldTextBox.Location = new Point(522, 101);
            RewardGoldTextBox.Name = "RewardGoldTextBox";
            RewardGoldTextBox.Size = new Size(60, 23);
            RewardGoldTextBox.TabIndex = 6;
            RewardGoldTextBox.Text = "0";
            // 
            // RewardItemsListBox
            // 
            RewardItemsListBox.FormattingEnabled = true;
            RewardItemsListBox.ItemHeight = 17;
            RewardItemsListBox.Location = new Point(18, 134);
            RewardItemsListBox.Name = "RewardItemsListBox";
            RewardItemsListBox.Size = new Size(301, 89);
            RewardItemsListBox.TabIndex = 8;
            // 
            // AddRewardItemButton
            // 
            AddRewardItemButton.Location = new Point(334, 134);
            AddRewardItemButton.Name = "AddRewardItemButton";
            AddRewardItemButton.Size = new Size(88, 27);
            AddRewardItemButton.TabIndex = 9;
            AddRewardItemButton.Text = "新增物品奖励";
            AddRewardItemButton.UseVisualStyleBackColor = true;
            AddRewardItemButton.Click += AddRewardItemButton_Click;
            // 
            // RemoveRewardItemButton
            // 
            RemoveRewardItemButton.Location = new Point(440, 134);
            RemoveRewardItemButton.Name = "RemoveRewardItemButton";
            RemoveRewardItemButton.Size = new Size(88, 27);
            RemoveRewardItemButton.TabIndex = 10;
            RemoveRewardItemButton.Text = "删除物品奖励";
            RemoveRewardItemButton.UseVisualStyleBackColor = true;
            RemoveRewardItemButton.Click += RemoveRewardItemButton_Click;
            // 
            // AddRewardItemsBatchButton
            // 
            AddRewardItemsBatchButton.Location = new Point(428, 167);
            AddRewardItemsBatchButton.Name = "AddRewardItemsBatchButton";
            AddRewardItemsBatchButton.Size = new Size(88, 27);
            AddRewardItemsBatchButton.TabIndex = 19;
            AddRewardItemsBatchButton.Text = "批量添加";
            AddRewardItemsBatchButton.UseVisualStyleBackColor = true;
            AddRewardItemsBatchButton.Click += AddRewardItemsBatchButton_Click;
            // 
            // RewardItemComboBox
            // 
            RewardItemComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RewardItemComboBox.FormattingEnabled = true;
            RewardItemComboBox.Location = new Point(334, 198);
            RewardItemComboBox.Name = "RewardItemComboBox";
            RewardItemComboBox.Size = new Size(182, 25);
            RewardItemComboBox.TabIndex = 11;
            // 
            // RewardItemCountTextBox
            // 
            RewardItemCountTextBox.Location = new Point(536, 198);
            RewardItemCountTextBox.Name = "RewardItemCountTextBox";
            RewardItemCountTextBox.Size = new Size(40, 23);
            RewardItemCountTextBox.TabIndex = 12;
            RewardItemCountTextBox.Text = "1";
            // 
            // InstanceActivityForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(640, 481);
            Controls.Add(MainTabControl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InstanceActivityForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "副本活动配置";
            FormClosed += InstanceActivityForm_FormClosed;
            MainTabControl.ResumeLayout(false);
            BasicTabPage.ResumeLayout(false);
            grpBasic.ResumeLayout(false);
            grpBasic.PerformLayout();
            grpActivities.ResumeLayout(false);
            TimeMapTabPage.ResumeLayout(false);
            grpTimeMap.ResumeLayout(false);
            grpTimeMap.PerformLayout();
            MonstersTabPage.ResumeLayout(false);
            grpMonsters.ResumeLayout(false);
            grpMonsters.PerformLayout();
            RewardTabPage.ResumeLayout(false);
            grpReward.ResumeLayout(false);
            grpReward.PerformLayout();
            ResumeLayout(false);
        }
    }
}

