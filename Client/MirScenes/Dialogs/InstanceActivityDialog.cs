using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.MirControls;
using Client.MirGraphics;
using S = ServerPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class InstanceActivityDialog : MirControl
    {
        private bool _timerStarted;
        private int _timerCounter;
        private long _timerTime;

        private readonly MirImageControl _animation;
        private int _animationFrame;
        private long _animationTime;

        private readonly MirImageControl _m10;
        private readonly MirImageControl _m1;
        private readonly MirImageControl _colon;
        private readonly MirImageControl _s10;
        private readonly MirImageControl _s1;

        private const int DigitOffset = 900; // Prguse2: 900-909=0-9, 910=':'

        private readonly List<ClientTimer> _timers = new List<ClientTimer>();
        private ClientTimer _current;

        public InstanceActivityDialog()
        {
            // 右下角再往上位置：上面一行动画，下面一行倒计时（拉高高度避免重叠）
            Size = new Size(120, 70);
            Location = new Point(Settings.ScreenWidth - Size.Width - 10, Settings.ScreenHeight - Size.Height - 220);
            NotControl = true;
            Movable = false;
            Sort = true;
            Visible = false;

            // 倒计时数字放在更下方一行，避免与动画重叠
            _m10 = CreateDigitControl(new Point(0, 40), DigitOffset + 0);
            _m1 = CreateDigitControl(new Point(18, 40), DigitOffset + 0);
            _colon = CreateDigitControl(new Point(36, 40), DigitOffset + 10);
            _s10 = CreateDigitControl(new Point(48, 40), DigitOffset + 0);
            _s1 = CreateDigitControl(new Point(66, 40), DigitOffset + 0);

            // 动画放在上方一行，居中于倒计时上方
            _animation = new MirImageControl
            {
                Parent = this,
                Library = Libraries.Prguse2,
                Index = 440,
                Location = new Point(15, -20),
                NotControl = true,
                UseOffSet = true,
                Visible = false
            };
        }

        private MirImageControl CreateDigitControl(Point location, int index)
        {
            return new MirImageControl
            {
                Parent = this,
                Library = Libraries.Prguse2,
                Index = index,
                Location = location,
                NotControl = true,
                UseOffSet = true,
                Visible = false
            };
        }

        public void AddTimer(S.SetTimer p)
        {
            if (p == null || string.IsNullOrEmpty(p.Key)) return;

            var existing = _timers.FirstOrDefault(t => t.Key == p.Key);
            if (existing != null)
            {
                existing.Update(p.Seconds, p.Type);
                return;
            }

            _timers.Add(new ClientTimer(p.Key, p.Seconds, p.Type));
        }

        public void ExpireTimer(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            var timer = _timers.FirstOrDefault(t => t.Key == key);
            if (timer == null) return;

            timer.RelativeTime = 0;
            if (timer == _current) _timerCounter = 0;
        }

        public void Process()
        {
            var timer = GetBestTimer();

            if (timer != null)
            {
                if (timer != _current || timer.Refresh)
                {
                    _current = timer;
                    _current.Refresh = false;

                    _timerStarted = true;
                    _timerTime = CMain.Time + 1000;
                    _timerCounter = (int)(_current.RelativeTime - (CMain.Time / 1000));

                    UpdateTimeGraphic();
                }
            }

            if (_current == null || !_timerStarted || CMain.Time < _timerTime) return;

            _timerCounter--;
            _timerTime = CMain.Time + 1000;

            if (_timerCounter < 0)
            {
                HideTimer();
                _timers.Remove(_current);
                _current = null;
                return;
            }

            UpdateTimeGraphic();
        }

        private ClientTimer GetBestTimer()
        {
            return _timers.OrderBy(x => x.RelativeTime).FirstOrDefault();
        }

        private void HideTimer()
        {
            Visible = false;
            _m10.Visible = _m1.Visible = _colon.Visible = _s10.Visible = _s1.Visible = _animation.Visible = false;
            _timerStarted = false;
        }

        private void UpdateTimeGraphic()
        {
            if (_timerCounter < 0)
            {
                HideTimer();
                return;
            }

            var ts = TimeSpan.FromSeconds(_timerCounter);
            var minutes = (int)Math.Min(99, ts.TotalMinutes);
            var seconds = ts.Seconds;

            _m10.Index = DigitOffset + (minutes / 10);
            _m1.Index = DigitOffset + (minutes % 10);
            _s10.Index = DigitOffset + (seconds / 10);
            _s1.Index = DigitOffset + (seconds % 10);

            Visible = true;
            _m10.Visible = _m1.Visible = _colon.Visible = _s10.Visible = _s1.Visible = _animation.Visible = true;

            if (CMain.Time >= _animationTime)
            {
                _animationFrame = (_animationFrame + 1) % 6; // 440-445
                _animation.Index = 440 + _animationFrame;
                _animationTime = CMain.Time + 100;
            }
        }
    }
}

