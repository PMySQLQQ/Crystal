using Client.MirGraphics;
using Client.MirScenes;
using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.MirObjects
{
    public class DamageInfo
    {
   
        private static readonly Dictionary<char, int> CharIndexMap = new Dictionary<char, int>
        {
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4},
            {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9},
            {'+', 10}, {'-', 11}
        };

   
        public int DamageValue;
        public DamageType Type;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public TimeSpan AppearDelay { get; set; } = TimeSpan.FromMilliseconds(500);
        public TimeSpan ShowDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan HideDelay { get; set; } = TimeSpan.FromMilliseconds(250);
        public int DrawY;
        public float Opacity;
        public bool Visible = true;
        public const int DrawHeight = 80; // Default drawing height (pixels)
        public int Bottom => DrawY - 20;

 
        private string _damageText;
        private Color _cachedColor;
        private int _lastOpacityInt;


        public DamageInfo(int damageValue, DamageType type)
        {
            DamageValue = damageValue;
            Type = type;
            StartTime = DateTime.Now;
            DrawY = 0;
            Opacity = 0;
            Visible = true;
            _damageText = damageValue.ToString();
        }

        // Constructor compatible with legacy code
        public DamageInfo(string text, int damageValue, DamageType type, int verticalIndex = 0)
        {
            DamageValue = damageValue;
            Type = type;
            StartTime = DateTime.Now;
            DrawY = 0;
            Opacity = 0;
            Visible = true;
            _damageText = text;
        }

        // Handle animation logic
        public void Process(DamageInfo previous)
        {
            if (Libraries.Prguse3 == null) return;

            DateTime now = DateTime.Now;
            TimeSpan visibleTime = now - StartTime;

            int oldY = DrawY;

            if (visibleTime < AppearDelay)
            {
                // Appearance phase: move from bottom to top, opacity from 0 to 1
                float percent = (float)visibleTime.TotalMilliseconds / (float)AppearDelay.TotalMilliseconds;
                DrawY = (int)(DrawHeight * percent);
                Opacity = Math.Min(1.0f, percent * 3);
            }
            else if (visibleTime < AppearDelay + ShowDelay)
            {
                // Hold phase: stay at a fixed height, opacity remains at 1
                DrawY = DrawHeight;
                Opacity = 1;
            }
            else if (visibleTime < AppearDelay + ShowDelay + HideDelay)
            {
                // Disappear phase: continue moving upward, opacity from 1 to 0
                TimeSpan hideTime = visibleTime - (AppearDelay + ShowDelay);
                float percent = (float)hideTime.TotalMilliseconds / (float)HideDelay.TotalMilliseconds;
                DrawY = DrawHeight + (int)(40 * percent);
                Opacity = 1 - percent;
            }
            else
            {
                // Fade out completely
                Visible = false;
                return;
            }

            // Fix the overlapping issue of multiple damage numbers.
            if (previous != null && previous.Visible)
                DrawY = Math.Min(DrawY, previous.Bottom);

            // Clear the cached color and recalculate it when the opacity changes.
            int opacityInt = (int)(Opacity * 255);
            if (opacityInt != _lastOpacityInt)
            {
                _lastOpacityInt = opacityInt;
                _cachedColor = Color.FromArgb(opacityInt, Color.White);
            }
        }

        // Draw damage numbers
        public void Draw(int drawX, int drawY)
        {
            if (Libraries.Prguse3 == null) return;

            // Vertical offset: move upward by DrawY pixels, then subtract an additional 20 pixels
            drawY -= DrawY + 20;

            // Horizontally centered: offset by 24 pixels based on the object center.
            drawX += 24;

            // Handle special cases
            if (Type == DamageType.Miss)
            {
                Libraries.Prguse3.Draw(76, new Point(drawX, drawY), _cachedColor);
                return;
            }

            // Handle critical strike
            if (Type == DamageType.Critical)
            {
                // Draw critical hit marker
                Libraries.Prguse3.Draw(78, new Point(drawX, drawY), _cachedColor);
                // Calculate the position after the critical hit marker
                drawX += Libraries.Prguse3.GetSize(78).Width;
            }

            // Draw numbers
            int imageIndex = GetImageIndex(DamageValue, Type);
            int charWidth = GetCharacterWidth(imageIndex);
            int imageHeight = Libraries.Prguse3.GetSize(imageIndex).Height;

            // Draw character by character
            foreach (char c in _damageText)
            {
                if (CharIndexMap.TryGetValue(c, out int charIndex))
                {
                    // Calculate the position of the character in the image.
                    Rectangle section = new Rectangle(charIndex * charWidth, 0, charWidth, imageHeight);
                    // Draw single character
                    Libraries.Prguse3.Draw(imageIndex, section, new Point(drawX, drawY), _cachedColor, false);
                    // Move to the position of the next character.
                    drawX += charWidth;
                }
            }
        }

        // Get the image index based on damage value and type.
        private int GetImageIndex(int damageValue, DamageType type)
        {
            switch (type)
            {
                case DamageType.Miss:
                    return 76; // Use image No. 76 for miss.
                default:
                    if (damageValue > 0) 
                        return 71; // Use image No. 71 for healing.
                    else if (damageValue <= -1000) 
                        return 75; // Use image No. 75 for extra large damage.
                    else if (damageValue <= -500) 
                        return 74; // Use image No. 74 for large damage.
                    else if (damageValue <= -100) 
                        return 73; // Use image No. 73 for medium damage.
                    else 
                        return 72; // Use image No. 72 for small damage.
            }
        }

        // Get character width
        private int GetCharacterWidth(int imageIndex)
        {
            switch (imageIndex)
            {
                case 71: // Blue numbers
                case 72: // Red numbers
                    return 9;
                case 73: // Green numbers
                    return 11;
                case 74: // Orange numbers
                    return 13;
                case 75: // White numbers
                    return 20;
                default:
                    return 0;
            }
        }
    }
}