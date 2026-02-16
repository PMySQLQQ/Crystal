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

            if (SelectedIndex < 0 || TeleportList == null || SelectedIndex >= TeleportList.Count)
            {
                return;
            }
            
   
            MirMessageBox messageBox = new MirMessageBox("Are you sure you want to delete the selected location?\nThis action cannot be undone.", MirMessageBoxButtons.YesNo);
            messageBox.Show();
   
            int deleteIndex = SelectedIndex;
            
            messageBox.YesButton.Click += (sender, e) =>
            {
                try
                {
                    Network.Enqueue(new DeleteMemoryLocation { SelectIndex = deleteIndex });
                    
              
                    if (TeleportList != null && deleteIndex >= 0 && deleteIndex < TeleportList.Count)
                    {
                        TeleportList.RemoveAt(deleteIndex);
                        ReloadTeleportList();
                    }
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine("Delete location error: " + ex.Message);
                }
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
            if (_scrollIndex == _cachedScrollIndex)
            {
                return; 
            }
            _cachedScrollIndex = _scrollIndex;
            if (_moveCells != null)
            {
                int startIndex = _scrollIndex;
                int endIndex = Math.Min(_scrollIndex + PageRows, _moveCells.Length);
                
                for (int i = Math.Max(0, startIndex - 1); i < Math.Min(endIndex + 1, _moveCells.Length); i++)
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
                    
                    bool shouldBeVisible = i >= startIndex && i < endIndex;
                    if (cell.Visible != shouldBeVisible)
                    {
                        cell.Visible = shouldBeVisible;
                    }
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
                
                for (int i = 0; i < count; i++)
                {
                    var teleportInfo = TeleportList[i];
                    
                    MoveCell cell = _moveCells[i];
                    if (cell == null || cell.IsDisposed)
                    {
                        cell = new MoveCell
                        {
                            Parent = this,
                            Location = new Point(15, 50 + i * 20 - 10),
                            Size = new Size(140, 20),
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
                        cell.MoveLocation = teleportInfo.Location;
                        cell.Name = teleportInfo.Name;
                        cell.ColorIndex = teleportInfo.ColorIndex;
                        cell.Index = i;
                        cell.Location = new Point(15, 50 + i * 20 - 10);
                        cell.Size = new Size(140, 20);
                        cell.Visible = true;
                    }
                }
               
                UpdateMoveCells();
                
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
                Size = new Size(136, 16),
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
                
                if (byteCount >= 30)
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
            if (GameScene.User != null && GameScene.Scene.MapControl != null)
            {
                Point location = GameScene.User.CurrentLocation;
                name = InputTextBox.Text + "   " + location.X + " : " + location.Y;
            }
            Network.Enqueue(new MemoryLocation { Name = name, ColorIndex = RememberMoveDialog.SelectedColorIndex });
            Dispose();
            GameScene.Scene.PositionMoveDialog.UpdateMoveCells();
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