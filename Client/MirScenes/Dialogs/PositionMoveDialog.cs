using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public class PlayerTeleportInfo
    {

        public string Name { get; set; }
        public Point Location { get; set; }
        public int ColorIndex { get; set; }
        public int MapIndex { get; set; }
    }
    public class PositionMoveDialog : MirImageControl
    {
        #region 控件定义
        public MirButton RememberButton { get; private set; }
        public MirButton MoveButton { get; private set; }
        public MirButton DeleteButton { get; private set; }
        public MirButton CloseButton { get; private set; }
        public MirButton UpButton { get; private set; }
        public MirButton DownButton { get; private set; }
        public MirButton ScrollBar { get; private set; }
        #endregion

        #region 字段定义
        private MoveCell[] _moveCells;
        public static List<PlayerTeleportInfo> TeleportList { get; set; }
        public static int SelectedIndex { get; set; }
        private int _scrollIndex;
        private int _displayCount;
        private const int PageRows = 24;
        private const int ScrollBarBaseX = 717;
        private string _positionName;
        #endregion
        public PositionMoveDialog()
        {
            Index = 871;
            Library = Libraries.Title;
            Movable = true;
            Sort = true;
            Location = Center;
            BeforeDraw += (sender, e) => OnBeforeDraw();
            InitializeControls();
            TeleportList = new List<PlayerTeleportInfo>();
        }
        private void InitializeControls()
        {
            UpdatePositionName();
            RememberButton = new MirButton
            {
                Index = 880,
                HoverIndex = 881,
                PressedIndex = 882,
                Location = new Point(10, 270),
                Library = Client.MirGraphics.Libraries.Title,
                Parent = this,
                Hint = "Input the location coordinates",
                Sound = Client.MirSounds.SoundList.ButtonC
            };
            RememberButton.Click += (sender, e) => 
            {
                UpdatePositionName();
                new RememberMoveDialog(_positionName);
            };

            MoveButton = new MirButton
            {
                Index = 883,
                HoverIndex = 884,
                PressedIndex = 885,
                Location = new Point(70, 270),
                Library = Client.MirGraphics.Libraries.Title,
                Parent = this,
                Hint = "Teleport to the marked location",
                Sound = Client.MirSounds.SoundList.ButtonC
            };
            MoveButton.Click += (sender, e) => OnMoveButtonClick();
            DeleteButton = new MirButton
            {
                Index = 886,
                HoverIndex = 887,
                PressedIndex = 858,
                Location = new Point(130, 270),
                Library = Client.MirGraphics.Libraries.Title,
                Parent = this,
                Hint = "Remove the designated point",
                Sound = Client.MirSounds.SoundList.ButtonC
            };
            DeleteButton.Click += (sender, e) => OnDeleteButtonClick();
            CloseButton = new MirButton
            {
                Index = 360,
                HoverIndex = 361,
                PressedIndex = 362,
                Location = new Point(174, 5),
                Library = Client.MirGraphics.Libraries.Prguse2,
                Parent = this,
                Hint = string.Empty,
                Sound = Client.MirSounds.SoundList.ButtonA
            };
            CloseButton.Click += (sender, e) => Hide();
            UpButton = new MirButton
            {
                Index = 197,
                HoverIndex = 198,
                PressedIndex = 199,
                Location = new Point(ScrollBarBaseX - 540, 34),
                Library = Client.MirGraphics.Libraries.Prguse2,
                Parent = this,
                Hint = string.Empty,
                Sound = Client.MirSounds.SoundList.ButtonA
            };
            UpButton.Size = new Size(16, 14);
            UpButton.Click += (sender, e) => OnUpButtonClick();
            DownButton = new MirButton
            {
                Index = 207,
                HoverIndex = 208,
                PressedIndex = 209,
                Location = new Point(ScrollBarBaseX - 540, 250),
                Library = Client.MirGraphics.Libraries.Prguse2,
                Parent = this,
                Hint = string.Empty,
                Sound = Client.MirSounds.SoundList.ButtonA
            };
            DownButton.Size = new Size(16, 14);
            DownButton.Click += (sender, e) => OnDownButtonClick();
            ScrollBar = new MirButton
            {
                Index = 206,
                Library = Client.MirGraphics.Libraries.Prguse2,
                Location = new Point(ScrollBarBaseX - 540, 46),
                Parent = this,
                Movable = true,
                Sound = Client.MirSounds.SoundList.None
            };
            ScrollBar.OnMoving += ScrollBar_OnMoving;
        }
        private void UpdatePositionName()
        {
            MapControl map = GameScene.Scene.MapControl;

            if (map != null)
            {
                _positionName = map.Title;
            }
            else
            {
                _positionName = "Unknown Location";
            }
        }
        private void OnBeforeDraw()
        {
            MapControl map = GameScene.Scene.MapControl;

            if (map == null || !Visible)
            {
                return;
            }

            UpdatePositionName();
        }
        private void OnMoveButtonClick()
        {
            MirMessageBox messageBox = new MirMessageBox("Do you want to move to the selected map location?\nThis teleport will cost 3000 gold.", MirMessageBoxButtons.YesNo);
            messageBox.Show();
            messageBox.YesButton.Click += (sender, e) =>
            {
                Network.Enqueue(new PositionMove { SelectIndex = SelectedIndex });
                Hide();
            };
        }
        private void OnDeleteButtonClick()
        {
            // 快速返回，避免无效操作
            if (TeleportList == null || TeleportList.Count == 0)
            {
                return;
            }
            
            // 保存当前选中的索引
            int deleteIndex = SelectedIndex;
            
            // 如果没有选中的索引，默认选中第一个
            if (deleteIndex < 0 || deleteIndex >= TeleportList.Count)
            {
                deleteIndex = 0;
                SelectedIndex = 0;
            }
            
            // 立即创建并显示消息框，不做任何可能导致延迟的操作
            MirMessageBox messageBox = new MirMessageBox("Are you sure you want to delete the selected location?\nThis action cannot be undone.", MirMessageBoxButtons.YesNo);
            messageBox.Show();
            
            // 为确认按钮添加点击事件处理
            messageBox.YesButton.Click += (sender, e) =>
            {
                // 立即从本地列表中移除并更新UI，提供即时视觉反馈
                if (TeleportList != null && deleteIndex >= 0 && deleteIndex < TeleportList.Count)
                {
                    // 保存要删除的传送点信息，以便在更新后重新选择合适的索引
                    int oldCount = TeleportList.Count;
                    
                    // 立即从本地列表中移除
                    TeleportList.RemoveAt(deleteIndex);
                    
                    // 立即更新UI，提供即时视觉反馈
                    FastUpdateTeleportList();
                    
                    // 调整选中索引
                    if (SelectedIndex >= TeleportList.Count && TeleportList.Count > 0)
                    {
                        SelectedIndex = TeleportList.Count - 1;
                    }
                }
                
                // 发送删除请求到服务器
                Network.Enqueue(new DeleteMemoryLocation { SelectIndex = deleteIndex });
            };
        }
        private void OnUpButtonClick()
        {
            if (_scrollIndex > 0)
            {
                _scrollIndex--;
                UpdateMoveCells();
                UpdateScrollPosition();
            }
        }
        private void OnDownButtonClick()
        {
            if (_scrollIndex < _displayCount - PageRows)
            {
                _scrollIndex++;
                UpdateMoveCells();
                UpdateScrollPosition();
            }
        }
        private DateTime _lastScrollUpdateTime = DateTime.MinValue;
        private const int ScrollUpdateInterval = 16; // 约60fps
        public void ScrollBar_OnMoving(object sender, MouseEventArgs e)
        {
            int newPositionY = ScrollBar.Location.Y;
            
            if (newPositionY >= DownButton.Location.Y - 15)
            {
                newPositionY = DownButton.Location.Y - 15;
            }
            if (newPositionY < UpButton.Location.Y + 15)
            {
                newPositionY = UpButton.Location.Y + 15;
            }
            
            int adjustedPositionY = newPositionY - 62;
            int scrollRatio = _displayCount > PageRows ? 366 / (_displayCount - PageRows) : 1;
            int newScrollIndex = (int)Math.Floor((double)(adjustedPositionY / scrollRatio));
            newScrollIndex = Math.Max(0, Math.Min(newScrollIndex, Math.Max(0, _displayCount - PageRows)));
            
            DateTime currentTime = DateTime.Now;
            if (newScrollIndex != _scrollIndex && (currentTime - _lastScrollUpdateTime).TotalMilliseconds >= ScrollUpdateInterval)
            {
                _scrollIndex = newScrollIndex;
                UpdateMoveCells();
                _lastScrollUpdateTime = currentTime;
            }
            
            ScrollBar.Location = new Point(ScrollBarBaseX - 540, newPositionY);
        }

        private DateTime _lastMouseWheelTime = DateTime.MinValue;
        private const int MouseWheelUpdateInterval = 16; 
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            if ((currentTime - _lastMouseWheelTime).TotalMilliseconds < MouseWheelUpdateInterval)
            {
                return;
            }
            
            int scrollSteps = e.Delta / SystemInformation.MouseWheelScrollDelta;
            
            bool isAtTop = _scrollIndex == 0 && scrollSteps >= 0;
            bool isAtBottom = _scrollIndex == Math.Max(0, _displayCount - PageRows) && scrollSteps <= 0;
            
            if (!isAtTop && !isAtBottom)
            {
                _scrollIndex -= scrollSteps > 0 ? 1 : -1;
                _scrollIndex = Math.Max(0, Math.Min(_scrollIndex, Math.Max(0, _displayCount - PageRows)));
                UpdateMoveCells();
                UpdateScrollPosition();
                _lastMouseWheelTime = currentTime;
            }
        }
        private void UpdateScrollPosition()
        {
            int scrollableRows = Math.Max(0, _displayCount - PageRows);
            int scrollRatio = scrollableRows > 0 ? 366 / scrollableRows : 1;
            int newPositionY = 62 + _scrollIndex * scrollRatio;
            newPositionY = Math.Max(UpButton.Location.Y + 15, Math.Min(newPositionY, DownButton.Location.Y - 15));
            ScrollBar.Location = new Point(ScrollBarBaseX - 540, newPositionY);
        }
        private int _cachedScrollIndex = -1;
        public void UpdateMoveCells()
        {
            if (_displayCount < PageRows)
            {
                _scrollIndex = 0;
            }
            if (_scrollIndex == _cachedScrollIndex || _moveCells == null)
            {
                return; 
            }
            _cachedScrollIndex = _scrollIndex;
            
            int startIndex = _scrollIndex;
            int endIndex = Math.Min(_scrollIndex + PageRows, _moveCells.Length);
            
            // 只更新可见区域内的单元格，减少不必要的计算
            for (int i = startIndex; i < endIndex; i++)
            {
                MoveCell cell = _moveCells[i];
                if (cell == null || cell.IsDisposed)
                {
                    continue;
                }
                
                Point newLocation = new Point(15, 50 + i * 20 - _scrollIndex * 20 - 10);
                
                if (cell.Location != newLocation)
                {
                    cell.Location = newLocation;
                }
                
                if (!cell.Visible)
                {
                    cell.Visible = true;
                }
            }
            
            // 隐藏超出可见区域的单元格
            for (int i = 0; i < startIndex; i++)
            {
                MoveCell cell = _moveCells[i];
                if (cell != null && !cell.IsDisposed && cell.Visible)
                {
                    cell.Visible = false;
                }
            }
            
            for (int i = endIndex; i < _moveCells.Length; i++)
            {
                MoveCell cell = _moveCells[i];
                if (cell != null && !cell.IsDisposed && cell.Visible)
                {
                    cell.Visible = false;
                }
            }
        }

        private void ClearTeleportList()
        {
            if (_moveCells == null)
            {
                return;
            }
            foreach (MoveCell cell in _moveCells)
            {
                if (cell != null && !cell.IsDisposed)
                {
                    cell.MouseWheel -= OnMouseWheel;
                    cell.Visible = false;
                }
            }
            
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    foreach (MoveCell cell in _moveCells)
                    {
                        if (cell != null && !cell.IsDisposed)
                        {
                            cell.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Dispose cell error: " + ex.Message);
                }
            });
        }

        public void ReloadTeleportList()
        {
            try
            {
                if (TeleportList == null || TeleportList.Count == 0)
                {
                    if (_moveCells != null)
                    {
                        foreach (MoveCell cell in _moveCells)
                        {
                            if (cell != null && !cell.IsDisposed)
                            {
                                cell.MouseWheel -= OnMouseWheel;
                                cell.Visible = false;
                            }
                        }
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            try
                            {
                                foreach (MoveCell cell in _moveCells)
                                {
                                    if (cell != null && !cell.IsDisposed)
                                    {
                                        cell.Dispose();
                                    }
                                }
                            }
                            catch { }
                        });
                    }
                    _moveCells = null;
                    _displayCount = 0;
                    return;
                }
                
                int count = TeleportList.Count;
                _displayCount = count;
                
                // 优化：如果_moveCells数组大小与传送点数量不匹配，才重新创建数组
                if (_moveCells == null || _moveCells.Length != count)
                {
                    if (_moveCells != null)
                    {
                        foreach (MoveCell cell in _moveCells)
                        {
                            if (cell != null && !cell.IsDisposed)
                            {
                                cell.MouseWheel -= OnMouseWheel;
                                cell.Visible = false;
                            }
                        }
                        
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            try
                            {
                                foreach (MoveCell cell in _moveCells)
                                {
                                    if (cell != null && !cell.IsDisposed)
                                    {
                                        cell.Dispose();
                                    }
                                }
                            }
                            catch { }
                        });
                    }
                    
                    _moveCells = new MoveCell[count];
                }
                
                // 快速更新或创建MoveCell对象
                for (int i = 0; i < count; i++)
                {
                    var teleportInfo = TeleportList[i];
                    
                    MoveCell cell = _moveCells[i];
                    if (cell == null || cell.IsDisposed)
                    {
                        // 快速创建新的MoveCell对象
                        cell = new MoveCell
                        {
                            Parent = this,
                            Location = new Point(15, 50 + i * 20 - 10),
                            Size = new Size(150, 20),
                            MoveLocation = teleportInfo.Location,
                            Name = teleportInfo.Name,
                            ColorIndex = teleportInfo.ColorIndex,
                            Index = i
                        };
                        cell.MouseWheel += OnMouseWheel;
                        _moveCells[i] = cell;
                    }
                    else
                    {
                        // 快速更新现有MoveCell对象
                        cell.MoveLocation = teleportInfo.Location;
                        cell.Name = teleportInfo.Name;
                        cell.ColorIndex = teleportInfo.ColorIndex;
                        cell.Index = i;
                        cell.Location = new Point(15, 50 + i * 20 - 10);
                        cell.Size = new Size(150, 20);
                        cell.Visible = true;
                    }
                }
                
                // 快速更新MoveCell对象的位置和可见性
                UpdateMoveCells();
                
                // 调整选中索引
                if (SelectedIndex >= count)
                {
                    SelectedIndex = count > 0 ? count - 1 : -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reload teleport list error: " + ex.Message);
                _displayCount = 0;
            }
        }
        
        /// <summary>
        /// 快速更新传送点列表，用于删除单个传送点的情况
        /// </summary>
        public void FastUpdateTeleportList()
        {
            try
            {
                if (TeleportList == null || TeleportList.Count == 0)
                {
                    // 快速清空列表
                    if (_moveCells != null)
                    {
                        foreach (MoveCell cell in _moveCells)
                        {
                            if (cell != null && !cell.IsDisposed)
                            {
                                cell.Visible = false;
                            }
                        }
                    }
                    _displayCount = 0;
                    SelectedIndex = -1;
                    UpdateMoveCells();
                    return;
                }
                
                int count = TeleportList.Count;
                _displayCount = count;
                
                // 优化：即使_moveCells数组大小与传送点数量不匹配，也尝试快速更新
                if (_moveCells == null || _moveCells.Length != count)
                {
                    // 只在必要时创建新的MoveCell对象
                    MoveCell[] newMoveCells = new MoveCell[count];
                    
                    // 复用现有MoveCell对象
                    int reuseCount = Math.Min(_moveCells?.Length ?? 0, count);
                    for (int i = 0; i < reuseCount; i++)
                    {
                        if (_moveCells != null && _moveCells[i] != null && !_moveCells[i].IsDisposed)
                        {
                            newMoveCells[i] = _moveCells[i];
                        }
                    }
                    
                    // 创建新的MoveCell对象
                    for (int i = reuseCount; i < count; i++)
                    {
                        var teleportInfo = TeleportList[i];
                        MoveCell cell = new MoveCell
                        {
                            Parent = this,
                            Size = new Size(150, 20),
                            MoveLocation = teleportInfo.Location,
                            ColorIndex = teleportInfo.ColorIndex,
                            Index = i
                        };
                        // 延迟设置Name和Location，减少初始化时的开销
                        cell.Name = teleportInfo.Name;
                        cell.Location = new Point(15, 50 + i * 20 - 10);
                        cell.MouseWheel += OnMouseWheel;
                        newMoveCells[i] = cell;
                    }
                    
                    // 隐藏并清理多余的MoveCell对象
                    if (_moveCells != null && _moveCells.Length > count)
                    {
                        for (int i = count; i < _moveCells.Length; i++)
                        {
                            if (_moveCells[i] != null && !_moveCells[i].IsDisposed)
                            {
                                _moveCells[i].Visible = false;
                                // 异步清理
                                System.Threading.Tasks.Task.Run(() =>
                                {
                                    try
                                    {
                                        _moveCells[i].Dispose();
                                    }
                                    catch { }
                                });
                            }
                        }
                    }
                    
                    _moveCells = newMoveCells;
                }
                else
                {
                    // 快速更新现有MoveCell对象
                    for (int i = 0; i < count; i++)
                    {
                        var teleportInfo = TeleportList[i];
                        MoveCell cell = _moveCells[i];
                        
                        if (cell != null && !cell.IsDisposed)
                        {
                            // 只更新必要的属性，避免不必要的计算
                            if (cell.MoveLocation != teleportInfo.Location)
                            {
                                cell.MoveLocation = teleportInfo.Location;
                            }
                            if (cell.Name != teleportInfo.Name)
                            {
                                cell.Name = teleportInfo.Name;
                            }
                            if (cell.ColorIndex != teleportInfo.ColorIndex)
                            {
                                cell.ColorIndex = teleportInfo.ColorIndex;
                            }
                            if (cell.Index != i)
                            {
                                cell.Index = i;
                            }
                            // 增加宽度以显示完整的坐标和地图名
                            if (cell.Size.Width != 150)
                            {
                                cell.Size = new Size(150, 20);
                            }
                            // 计算新位置
                            Point newLocation = new Point(15, 50 + i * 20 - 10);
                            if (cell.Location != newLocation)
                            {
                                cell.Location = newLocation;
                            }
                            if (!cell.Visible)
                            {
                                cell.Visible = true;
                            }
                        }
                    }
                    
                    // 隐藏超出范围的MoveCell对象
                    for (int i = count; i < _moveCells.Length; i++)
                    {
                        MoveCell cell = _moveCells[i];
                        if (cell != null && !cell.IsDisposed && cell.Visible)
                        {
                            cell.Visible = false;
                        }
                    }
                }
                
                // 快速更新MoveCell对象的位置和可见性
                UpdateMoveCells();
                
                // 调整选中索引
                if (SelectedIndex >= count)
                {
                    SelectedIndex = count > 0 ? count - 1 : -1;
                }
                
                // 调整滚动位置
                if (_scrollIndex > Math.Max(0, _displayCount - PageRows))
                {
                    _scrollIndex = Math.Max(0, _displayCount - PageRows);
                    UpdateScrollPosition();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fast update teleport list error: " + ex.Message);
                // 如果快速更新失败，使用完整的重新加载
                ReloadTeleportList();
            }
        }
        public new void Show()
        {
            Visible = true;
        }

        public new void Hide()
        {
            Visible = false;
        }
    }
    public sealed class MoveCell : MirControl
    {
        public int Index { get; set; }
        public Point MoveLocation { get; set; }
        private MirLabel _nameLabel;
        public int ColorIndex { get; set; }
        private string _cachedName;
        private Point _cachedLocation;
        private string _cachedDisplayText;
        public string Name
        {
            get { return _nameLabel?.Text ?? string.Empty; }
            set
            {
                try
                {
                    if (value == _cachedName)
                    {
                        return;
                    }

                    _cachedName = value;
                    string fullText = value;
                    bool isTruncated;
                    string displayText = GetTruncatedText(fullText, out isTruncated) + (isTruncated ? ".." : "");
                    
                    if (_cachedDisplayText != displayText)
                    {
                        _cachedDisplayText = displayText;
                        if (_nameLabel != null)
                        {
                            _nameLabel.Text = displayText;
                            if (Size.Width > 0)
                            {
                                _nameLabel.Size = new Size(Size.Width - 4, Size.Height - 4);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Set name error: " + ex.Message);
                }
            }
        }
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                if (_nameLabel != null && value.Width > 0)
                {
                    _nameLabel.Size = new Size(value.Width - 4, value.Height - 4);
                }
            }
        }
        public MoveCell()
        {
            Border = false;
            BorderColour = Color.Gold;
            BeforeDraw += OnBeforeDraw;
            Click += (sender, e) => PositionMoveDialog.SelectedIndex = Index;

            _nameLabel = new MirLabel
            {
                Text = "text",
                Location = new Point(0, 0),
                Parent = this,
                AutoSize = false,
                Size = new Size(146, 16),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8.5f),
                ForeColour = Color.White,
                NotControl = true
            };
        }
        private string GetTruncatedText(string text, out bool isTruncated)
        {
            isTruncated = false;
            
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            char[] charBuffer = text.ToCharArray();
            int byteCount = 0;
            int i = 0;
            while (i < charBuffer.Length)
            {
                char c = charBuffer[i];
                if (c <= 0x7F)
                {
                    byteCount += 1;
                }
                else if (c <= 0x7FF)
                {
                    byteCount += 2;
                }
                else if (c <= 0xFFFF)
                {
                    byteCount += 3;
                }
                else
                {
                    byteCount += 4;
                }
                
                if (byteCount >= 50)
                {
                    isTruncated = true;
                    return new string(charBuffer, 0, i);
                }
                
                i++;
            }
            
            return text;
        }
        private void OnBeforeDraw(object sender, EventArgs e)
        {
            bool isSelected = PositionMoveDialog.SelectedIndex == Index;
            Border = IsMouseOver(Client.CMain.MPoint) || isSelected;
            
            if (GameScene.User != null && _nameLabel != null)
            {
                _nameLabel.ForeColour = GetDisplayColor();
            }
        }
        private Color GetDisplayColor()
        {
            Color userColor;
            switch (ColorIndex)
            {
                case 0: userColor = Color.White;
                    break;
                case 1: userColor = Color.Gray;
                    break;
                case 2: userColor = Color.Orange;
                    break;
                case 3: userColor = Color.Red;
                    break;
                case 4: userColor = Color.DeepPink;
                    break;
                case 5: userColor = Color.Yellow;
                    break;
                case 6: userColor = Color.Pink;
                    break;
                case 7: userColor = Color.LimeGreen;
                    break;
                case 8: userColor = Color.Green;
                    break;
                case 9: userColor = Color.SkyBlue;
                    break;
                case 10: userColor = Color.DarkBlue;
                    break;
                case 11: userColor = Color.Purple;
                    break;
                case 12: userColor = Color.DarkViolet;
                    break;
                default: userColor = Color.White;
                    break;
            }
            if (PositionMoveDialog.SelectedIndex == Index)
            {
                int r = Math.Min(255, userColor.R + 50);
                int g = Math.Min(255, userColor.G + 50);
                int b = Math.Min(255, userColor.B + 50);
                return Color.FromArgb(r, g, b);
            }
            
            return userColor;
        }
    }
    public sealed class RememberMoveDialog : MirImageControl
    {

        public readonly MirButton OKButton;
        public readonly MirButton CancelButton;
        public readonly MirTextBox InputTextBox;
        public readonly MirLabel CaptionLabel;
        public MirControl[] ColorIcons;
        public static int SelectedColorIndex { get; set; } = 0; 
        private void InitializeColorSelection()
        {
            RememberMoveDialog.SelectedColorIndex = 0;
        }
        public RememberMoveDialog(string name)
        {
            Index = 872;
            Library = Libraries.Title;
            Parent = GameScene.Scene;
            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);
            Modal = true;
            Visible = true;

            CaptionLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.WordBreak,
                Location = new Point(12, 35),
                Size = new Size(235, 40),
                Parent = this,
                Text = "Please specify the name of the location",
            };


            InputTextBox = new MirTextBox
            {
                Parent = this,
                Location = new Point(17, 57),
                Size = new Size(215, 5),
                MaxLength = 50,
                Text = name,
                Font = new Font(Settings.FontName, 8F),
            };
            InputTextBox.SetFocus();
            InputTextBox.TextBox.KeyPress += OnKeyPress;

            OKButton = new MirButton
            {
                HoverIndex = 201,
                Index = 200,
                Library = Libraries.Title,
                Location = new Point(60, 123),
                Parent = this,
                PressedIndex = 202,
            };
            OKButton.Click += OnOKButtonClick;

            CancelButton = new MirButton
            {
                HoverIndex = 204,
                Index = 203,
                Library = Libraries.Title,
                Location = new Point(160, 123),
                Parent = this,
                PressedIndex = 205,
            };
            CancelButton.Click += (sender, e) => Dispose();


            ColorIcons = new MirControl[13];
            
           
            int iconSize = 15;
            int spacing = 2;
            int totalWidth = ColorIcons.Length * iconSize + (ColorIcons.Length - 1) * spacing;
            int startX = 24; 
            int startY = 85; 
            
            for (int i = 0; i < ColorIcons.Length; i++)
            {
                ColorIcon colorIcon = new ColorIcon
                {
                    Parent = this,
                    Location = new Point(startX + i * (iconSize + spacing), startY),
                    Size = new Size(iconSize, iconSize),
                    ColorIndex = i,
                };
                
                ColorIcons[i] = colorIcon;
            }
            
          
            InitializeColorSelection();
        }
        private void OnOKButtonClick(object sender, EventArgs e)
        {
            if (PositionMoveDialog.TeleportList != null && PositionMoveDialog.TeleportList.Count >= Globals.MaxPositionMove)
            {
                MirMessageBox messageBox = new MirMessageBox("Cannot save more locations");
                messageBox.Show();
                return;
            }

            string name = InputTextBox.Text;
            Point location = Point.Empty;
            if (GameScene.User != null && GameScene.Scene.MapControl != null)
            {
                location = GameScene.User.CurrentLocation;
                name = InputTextBox.Text + "   " + location.X + " : " + location.Y;
            }
            
            // 发送请求到服务器，等待服务器响应后再更新UI
            Network.Enqueue(new MemoryLocation { Name = name, ColorIndex = RememberMoveDialog.SelectedColorIndex });
            Dispose();
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (OKButton != null && !OKButton.IsDisposed)
                {
                    OKButton.InvokeMouseClick(EventArgs.Empty);
                }
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                if (CancelButton != null && !CancelButton.IsDisposed)
                {
                    CancelButton.InvokeMouseClick(EventArgs.Empty);
                }
                e.Handled = true;
            }
        }
    }


    public sealed class ColorIcon : MirControl
    {

        public int ColorIndex { get; set; }
        private MirLabel _colorLabel;
        public ColorIcon()
        {
            Border = true;
            BorderColour = Color.Gold;
            BeforeDraw += OnBeforeDraw;
            Click += (sender, e) => RememberMoveDialog.SelectedColorIndex = ColorIndex;

            _colorLabel = new MirLabel
            {
                Text = "",
                Location = new Point(2, 2),
                Parent = this,
                AutoSize = false,
                Size = new Size(10, 10),
                NotControl = true
            };
        }

        private void OnBeforeDraw(object sender, EventArgs e)
        {

            bool isSelected = RememberMoveDialog.SelectedColorIndex == ColorIndex;
            bool isMouseOver = IsMouseOver(Client.CMain.MPoint);
            

            if (isSelected)
            {
                Border = true;
                BorderColour = Color.Gold;
            }
            else if (isMouseOver)
            {
                Border = true;
                BorderColour = Color.Silver;
            }

            else
            {
                Border = false;
            }

            if (_colorLabel != null)
            {
                _colorLabel.BackColour = GetIconColor();
            }
        }

        private Color GetIconColor()
        {
            switch (ColorIndex)
            {
                case 0: return Color.White;
                case 1: return Color.Gray;
                case 2: return Color.Orange;
                case 3: return Color.Red;
                case 4: return Color.DeepPink;
                case 5: return Color.Yellow;
                case 6: return Color.Pink;
                case 7: return Color.LimeGreen;
                case 8: return Color.Green;
                case 9: return Color.SkyBlue;
                case 10: return Color.DarkBlue;
                case 11: return Color.Purple;
                case 12: return Color.DarkViolet;
                default: return Color.White;
            }
        }
    }
}