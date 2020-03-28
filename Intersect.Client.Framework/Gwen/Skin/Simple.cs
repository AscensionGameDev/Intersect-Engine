using System;

using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Skin
{

    /// <summary>
    ///     Simple skin (non-textured). Deprecated and incomplete, do not use.
    /// </summary>
    [Obsolete]
    public class Simple : Skin.Base
    {

        private readonly Color mColBgDark;

        private readonly Color mColBorderColor;

        private readonly Color mColControl;

        private readonly Color mColControlBright;

        private readonly Color mColControlDark;

        private readonly Color mColControlDarker;

        private readonly Color mColControlOutlineLight;

        private readonly Color mColControlOutlineLighter;

        private readonly Color mColControlOutlineNormal;

        private readonly Color mColHighlightBg;

        private readonly Color mColHighlightBorder;

        private readonly Color mColModal;

        private readonly Color mColToolTipBackground;

        private readonly Color mColToolTipBorder;

        public Simple(Renderer.Base renderer) : base(renderer)
        {
            mColBorderColor = Color.FromArgb(255, 80, 80, 80);

            //m_colBG = Color.FromArgb(255, 248, 248, 248);
            mColBgDark = Color.FromArgb(255, 235, 235, 235);

            mColControl = Color.FromArgb(255, 240, 240, 240);
            mColControlBright = Color.FromArgb(255, 255, 255, 255);
            mColControlDark = Color.FromArgb(255, 214, 214, 214);
            mColControlDarker = Color.FromArgb(255, 180, 180, 180);

            mColControlOutlineNormal = Color.FromArgb(255, 112, 112, 112);
            mColControlOutlineLight = Color.FromArgb(255, 144, 144, 144);
            mColControlOutlineLighter = Color.FromArgb(255, 210, 210, 210);

            mColHighlightBg = Color.FromArgb(255, 192, 221, 252);
            mColHighlightBorder = Color.FromArgb(255, 51, 153, 255);

            mColToolTipBackground = Color.FromArgb(255, 255, 255, 225);
            mColToolTipBorder = Color.FromArgb(255, 0, 0, 0);

            mColModal = Color.FromArgb(150, 25, 25, 25);
        }

        #region UI elements

        public override void DrawButton(Control.Base control, bool depressed, bool hovered, bool disabled)
        {
            var w = control.Width;
            var h = control.Height;

            DrawButton(w, h, depressed, hovered);
        }

        public override void DrawMenuItem(Control.Base control, bool submenuOpen, bool isChecked)
        {
            var rect = control.RenderBounds;
            if (submenuOpen || control.IsHovered)
            {
                mRenderer.DrawColor = mColHighlightBg;
                mRenderer.DrawFilledRect(rect);

                mRenderer.DrawColor = mColHighlightBorder;
                mRenderer.DrawLinedRect(rect);
            }

            if (isChecked)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 0, 0, 0);

                var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);
                DrawCheck(r);
            }
        }

        public override void DrawMenuStrip(Control.Base control)
        {
            var w = control.Width;
            var h = control.Height;

            mRenderer.DrawColor = Color.FromArgb(255, 246, 248, 252);
            mRenderer.DrawFilledRect(new Rectangle(0, 0, w, h));

            mRenderer.DrawColor = Color.FromArgb(150, 218, 224, 241);

            mRenderer.DrawFilledRect(Util.FloatRect(0, h * 0.4f, w, h * 0.6f));
            mRenderer.DrawFilledRect(Util.FloatRect(0, h * 0.5f, w, h * 0.5f));
        }

        public override void DrawMenu(Control.Base control, bool paddingDisabled)
        {
            var w = control.Width;
            var h = control.Height;

            mRenderer.DrawColor = mColControlBright;
            mRenderer.DrawFilledRect(new Rectangle(0, 0, w, h));

            if (!paddingDisabled)
            {
                mRenderer.DrawColor = mColControl;
                mRenderer.DrawFilledRect(new Rectangle(1, 0, 22, h));
            }

            mRenderer.DrawColor = mColControlOutlineNormal;
            mRenderer.DrawLinedRect(new Rectangle(0, 0, w, h));
        }

        public override void DrawShadow(Control.Base control)
        {
            var w = control.Width;
            var h = control.Height;

            var x = 4;
            var y = 6;

            mRenderer.DrawColor = Color.FromArgb(10, 0, 0, 0);

            mRenderer.DrawFilledRect(new Rectangle(x, y, w, h));
            x += 2;
            mRenderer.DrawFilledRect(new Rectangle(x, y, w, h));
            y += 2;
            mRenderer.DrawFilledRect(new Rectangle(x, y, w, h));
        }

        public virtual void DrawButton(int w, int h, bool depressed, bool bHovered, bool bSquared = false)
        {
            if (depressed)
            {
                mRenderer.DrawColor = mColControlDark;
            }
            else if (bHovered)
            {
                mRenderer.DrawColor = mColControlBright;
            }
            else
            {
                mRenderer.DrawColor = mColControl;
            }

            mRenderer.DrawFilledRect(new Rectangle(1, 1, w - 2, h - 2));

            if (depressed)
            {
                mRenderer.DrawColor = mColControlDark;
            }
            else if (bHovered)
            {
                mRenderer.DrawColor = mColControl;
            }
            else
            {
                mRenderer.DrawColor = mColControlDark;
            }

            mRenderer.DrawFilledRect(Util.FloatRect(1, h * 0.5f, w - 2, h * 0.5f - 2));

            if (!depressed)
            {
                mRenderer.DrawColor = mColControlBright;
            }
            else
            {
                mRenderer.DrawColor = mColControlDarker;
            }

            mRenderer.DrawShavedCornerRect(new Rectangle(1, 1, w - 2, h - 2), bSquared);

            // Border
            mRenderer.DrawColor = mColControlOutlineNormal;
            mRenderer.DrawShavedCornerRect(new Rectangle(0, 0, w, h), bSquared);
        }

        public override void DrawRadioButton(Control.Base control, bool selected, bool depressed)
        {
            var rect = control.RenderBounds;

            // Inside colour
            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 220, 242, 254);
            }
            else
            {
                mRenderer.DrawColor = mColControlBright;
            }

            mRenderer.DrawFilledRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

            // Border
            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 85, 130, 164);
            }
            else
            {
                mRenderer.DrawColor = mColControlOutlineLight;
            }

            mRenderer.DrawShavedCornerRect(rect);

            mRenderer.DrawColor = Color.FromArgb(15, 0, 50, 60);
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4));
            mRenderer.DrawFilledRect(Util.FloatRect(rect.X + 2, rect.Y + 2, rect.Width * 0.3f, rect.Height - 4));
            mRenderer.DrawFilledRect(Util.FloatRect(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height * 0.3f));

            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 121, 198, 249);
            }
            else
            {
                mRenderer.DrawColor = Color.FromArgb(50, 0, 50, 60);
            }

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 3, 1, rect.Height - 5));
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 3, rect.Y + 2, rect.Width - 5, 1));

            if (selected)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 40, 230, 30);
                mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4));
            }
        }

        public override void DrawCheckBox(Control.Base control, bool selected, bool depressed)
        {
            var rect = control.RenderBounds;

            // Inside colour
            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 220, 242, 254);
            }
            else
            {
                mRenderer.DrawColor = mColControlBright;
            }

            mRenderer.DrawFilledRect(rect);

            // Border
            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 85, 130, 164);
            }
            else
            {
                mRenderer.DrawColor = mColControlOutlineLight;
            }

            mRenderer.DrawLinedRect(rect);

            mRenderer.DrawColor = Color.FromArgb(15, 0, 50, 60);
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4));
            mRenderer.DrawFilledRect(Util.FloatRect(rect.X + 2, rect.Y + 2, rect.Width * 0.3f, rect.Height - 4));
            mRenderer.DrawFilledRect(Util.FloatRect(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height * 0.3f));

            if (control.IsHovered)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 121, 198, 249);
            }
            else
            {
                mRenderer.DrawColor = Color.FromArgb(50, 0, 50, 60);
            }

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 2, 1, rect.Height - 4));
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, 1));

            if (depressed)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 100, 100, 100);
                var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);
                DrawCheck(r);
            }
            else if (selected)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 0, 0, 0);
                var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);
                DrawCheck(r);
            }
        }

        public override void DrawGroupBox(Control.Base control, int textStart, int textHeight, int textWidth)
        {
            var rect = control.RenderBounds;

            rect.Y += (int) (textHeight * 0.5f);
            rect.Height -= (int) (textHeight * 0.5f);

            var colDarker = Color.FromArgb(50, 0, 50, 60);
            var colLighter = Color.FromArgb(150, 255, 255, 255);

            mRenderer.DrawColor = colLighter;

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, textStart - 3, 1));
            mRenderer.DrawFilledRect(
                new Rectangle(rect.X + 1 + textStart + textWidth, rect.Y + 1, rect.Width - textStart + textWidth - 2, 1)
            );

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.Width - 2, 1));

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height));
            mRenderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 2, rect.Y + 1, 1, rect.Height - 1));

            mRenderer.DrawColor = colDarker;

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, textStart - 3, 1));
            mRenderer.DrawFilledRect(
                new Rectangle(rect.X + 1 + textStart + textWidth, rect.Y, rect.Width - textStart - textWidth - 2, 1)
            );

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.Width - 2, 1));

            mRenderer.DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, 1, rect.Height - 1));
            mRenderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y + 1, 1, rect.Height - 1));
        }

        public override void DrawTextBox(Control.Base control)
        {
            var rect = control.RenderBounds;
            var bHasFocus = control.HasFocus;

            // Box inside
            mRenderer.DrawColor = Color.FromArgb(255, 255, 255, 255);
            mRenderer.DrawFilledRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

            mRenderer.DrawColor = mColControlOutlineLight;
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, rect.Width - 2, 1));
            mRenderer.DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, 1, rect.Height - 2));

            mRenderer.DrawColor = mColControlOutlineLighter;
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.Width - 2, 1));
            mRenderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y + 1, 1, rect.Height - 2));

            if (bHasFocus)
            {
                mRenderer.DrawColor = Color.FromArgb(150, 50, 200, 255);
                mRenderer.DrawLinedRect(rect);
            }
        }

        public override void DrawTabButton(Control.Base control, bool active, Pos dir)
        {
            var rect = control.RenderBounds;
            var bHovered = control.IsHovered;

            if (bHovered)
            {
                mRenderer.DrawColor = mColControlBright;
            }
            else
            {
                mRenderer.DrawColor = mColControl;
            }

            mRenderer.DrawFilledRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 1));

            if (bHovered)
            {
                mRenderer.DrawColor = mColControl;
            }
            else
            {
                mRenderer.DrawColor = mColControlDark;
            }

            mRenderer.DrawFilledRect(Util.FloatRect(1, rect.Height * 0.5f, rect.Width - 2, rect.Height * 0.5f - 1));

            mRenderer.DrawColor = mColControlBright;
            mRenderer.DrawShavedCornerRect(new Rectangle(1, 1, rect.Width - 2, rect.Height));

            mRenderer.DrawColor = mColBorderColor;

            mRenderer.DrawShavedCornerRect(new Rectangle(0, 0, rect.Width, rect.Height));
        }

        public override void DrawTabControl(Control.Base control)
        {
            var rect = control.RenderBounds;

            mRenderer.DrawColor = mColControl;
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawLinedRect(rect);

            //m_Renderer.DrawColor = m_colControl;
            //m_Renderer.DrawFilledRect(CurrentButtonRect);
        }

        public override void DrawWindow(Control.Base control, int topHeight, bool inFocus)
        {
            var rect = control.RenderBounds;

            // Titlebar
            if (inFocus)
            {
                mRenderer.DrawColor = Color.FromArgb(230, 87, 164, 232);
            }
            else
            {
                mRenderer.DrawColor = Color.FromArgb(230, (int) (87 * 0.70f), (int) (164 * 0.70f), (int) (232 * 0.70f));
            }

            var iBorderSize = 5;
            mRenderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, topHeight - 1));
            mRenderer.DrawFilledRect(
                new Rectangle(rect.X + 1, rect.Y + topHeight - 1, iBorderSize, rect.Height - 2 - topHeight)
            );

            mRenderer.DrawFilledRect(
                new Rectangle(
                    rect.X + rect.Width - iBorderSize, rect.Y + topHeight - 1, iBorderSize, rect.Height - 2 - topHeight
                )
            );

            mRenderer.DrawFilledRect(
                new Rectangle(rect.X + 1, rect.Y + rect.Height - iBorderSize, rect.Width - 2, iBorderSize)
            );

            // Main inner
            mRenderer.DrawColor = Color.FromArgb(230, mColControlDark.R, mColControlDark.G, mColControlDark.B);
            mRenderer.DrawFilledRect(
                new Rectangle(
                    rect.X + iBorderSize + 1, rect.Y + topHeight, rect.Width - iBorderSize * 2 - 2,
                    rect.Height - topHeight - iBorderSize - 1
                )
            );

            // Light inner border
            mRenderer.DrawColor = Color.FromArgb(100, 255, 255, 255);
            mRenderer.DrawShavedCornerRect(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));

            // Dark line between titlebar and main
            mRenderer.DrawColor = mColBorderColor;

            // Inside border
            mRenderer.DrawColor = mColControlOutlineNormal;
            mRenderer.DrawLinedRect(
                new Rectangle(
                    rect.X + iBorderSize, rect.Y + topHeight - 1, rect.Width - 10,
                    rect.Height - topHeight - (iBorderSize - 1)
                )
            );

            // Dark outer border
            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawShavedCornerRect(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
        }

        public override void DrawWindowCloseButton(Control.Base control, bool depressed, bool hovered, bool disabled)
        {
            // TODO
            DrawButton(control, depressed, hovered, disabled);
        }

        public override void DrawHighlight(Control.Base control)
        {
            var rect = control.RenderBounds;
            mRenderer.DrawColor = Color.FromArgb(255, 255, 100, 255);
            mRenderer.DrawFilledRect(rect);
        }

        public override void DrawScrollBar(Control.Base control, bool horizontal, bool depressed)
        {
            var rect = control.RenderBounds;
            if (depressed)
            {
                mRenderer.DrawColor = mColControlDarker;
            }
            else
            {
                mRenderer.DrawColor = mColControlBright;
            }

            mRenderer.DrawFilledRect(rect);
        }

        public override void DrawScrollBarBar(Control.Base control, bool depressed, bool hovered, bool horizontal)
        {
            //TODO: something specialized
            DrawButton(control, depressed, hovered, false);
        }

        public override void DrawTabTitleBar(Control.Base control)
        {
            var rect = control.RenderBounds;

            mRenderer.DrawColor = Color.FromArgb(255, 177, 193, 214);
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColBorderColor;
            rect.Height += 1;
            mRenderer.DrawLinedRect(rect);
        }

        public override void DrawProgressBar(Control.Base control, bool horizontal, float progress)
        {
            var rect = control.RenderBounds;
            var fillColour = Color.FromArgb(255, 0, 211, 40);

            if (horizontal)
            {
                //Background
                mRenderer.DrawColor = mColControlDark;
                mRenderer.DrawFilledRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

                //Right half
                mRenderer.DrawColor = fillColour;
                mRenderer.DrawFilledRect(Util.FloatRect(1, 1, rect.Width * progress - 2, rect.Height - 2));

                mRenderer.DrawColor = Color.FromArgb(150, 255, 255, 255);
                mRenderer.DrawFilledRect(Util.FloatRect(1, 1, rect.Width - 2, rect.Height * 0.45f));
            }
            else
            {
                //Background 
                mRenderer.DrawColor = mColControlDark;
                mRenderer.DrawFilledRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

                //Top half
                mRenderer.DrawColor = fillColour;
                mRenderer.DrawFilledRect(
                    Util.FloatRect(1, 1 + rect.Height * (1 - progress), rect.Width - 2, rect.Height * progress - 2)
                );

                mRenderer.DrawColor = Color.FromArgb(150, 255, 255, 255);
                mRenderer.DrawFilledRect(Util.FloatRect(1, 1, rect.Width * 0.45f, rect.Height - 2));
            }

            mRenderer.DrawColor = Color.FromArgb(150, 255, 255, 255);
            mRenderer.DrawShavedCornerRect(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

            mRenderer.DrawColor = Color.FromArgb(70, 255, 255, 255);
            mRenderer.DrawShavedCornerRect(new Rectangle(2, 2, rect.Width - 4, rect.Height - 4));

            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawShavedCornerRect(rect);
        }

        public override void DrawListBox(Control.Base control)
        {
            var rect = control.RenderBounds;

            mRenderer.DrawColor = mColControlBright;
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawLinedRect(rect);
        }

        public override void DrawListBoxLine(Control.Base control, bool selected, bool even)
        {
            var rect = control.RenderBounds;

            if (selected)
            {
                mRenderer.DrawColor = mColHighlightBorder;
                mRenderer.DrawFilledRect(rect);
            }
            else if (control.IsHovered)
            {
                mRenderer.DrawColor = mColHighlightBg;
                mRenderer.DrawFilledRect(rect);
            }
        }

        public override void DrawSlider(Control.Base control, bool horizontal, int numNotches, int barSize)
        {
            var rect = control.RenderBounds;
            var notchRect = rect;

            if (horizontal)
            {
                rect.Y += (int) (rect.Height * 0.4f);
                rect.Height -= (int) (rect.Height * 0.8f);
            }
            else
            {
                rect.X += (int) (rect.Width * 0.4f);
                rect.Width -= (int) (rect.Width * 0.8f);
            }

            mRenderer.DrawColor = mColBgDark;
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColControlDarker;
            mRenderer.DrawLinedRect(rect);
        }

        public override void DrawComboBox(Control.Base control, bool down, bool open)
        {
            DrawTextBox(control);
        }

        public override void DrawKeyboardHighlight(Control.Base control, Rectangle r, int iOffset)
        {
            var rect = r;

            rect.X += iOffset;
            rect.Y += iOffset;
            rect.Width -= iOffset * 2;
            rect.Height -= iOffset * 2;

            //draw the top and bottom
            var skip = true;
            for (var i = 0; i < rect.Width * 0.5; i++)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 0, 0, 0);
                if (!skip)
                {
                    mRenderer.DrawPixel(rect.X + i * 2, rect.Y);
                    mRenderer.DrawPixel(rect.X + i * 2, rect.Y + rect.Height - 1);
                }
                else
                {
                    skip = false;
                }
            }

            for (var i = 0; i < rect.Height * 0.5; i++)
            {
                mRenderer.DrawColor = Color.FromArgb(255, 0, 0, 0);
                mRenderer.DrawPixel(rect.X, rect.Y + i * 2);
                mRenderer.DrawPixel(rect.X + rect.Width - 1, rect.Y + i * 2);
            }
        }

        public override void DrawToolTip(Control.Base control)
        {
            var rct = control.RenderBounds;
            rct.X -= 3;
            rct.Y -= 3;
            rct.Width += 6;
            rct.Height += 6;

            mRenderer.DrawColor = mColToolTipBackground;
            mRenderer.DrawFilledRect(rct);

            mRenderer.DrawColor = mColToolTipBorder;
            mRenderer.DrawLinedRect(rct);
        }

        public override void DrawScrollButton(
            Control.Base control,
            Pos direction,
            bool depressed,
            bool hovered,
            bool disabled
        )
        {
            DrawButton(control, depressed, false, false);

            mRenderer.DrawColor = Color.FromArgb(240, 0, 0, 0);

            var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);

            if (direction == Pos.Top)
            {
                DrawArrowUp(r);
            }
            else if (direction == Pos.Bottom)
            {
                DrawArrowDown(r);
            }
            else if (direction == Pos.Left)
            {
                DrawArrowLeft(r);
            }
            else
            {
                DrawArrowRight(r);
            }
        }

        public override void DrawComboBoxArrow(
            Control.Base control,
            bool hovered,
            bool depressed,
            bool open,
            bool disabled
        )
        {
            //DrawButton( control.Width, control.Height, depressed, false, true );

            mRenderer.DrawColor = Color.FromArgb(240, 0, 0, 0);

            var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);
            DrawArrowDown(r);
        }

        public override void DrawNumericUpDownButton(Control.Base control, bool depressed, bool up)
        {
            //DrawButton( control.Width, control.Height, depressed, false, true );

            mRenderer.DrawColor = Color.FromArgb(240, 0, 0, 0);

            var r = new Rectangle(control.Width / 2 - 2, control.Height / 2 - 2, 5, 5);

            if (up)
            {
                DrawArrowUp(r);
            }
            else
            {
                DrawArrowDown(r);
            }
        }

        public override void DrawTreeButton(Control.Base control, bool open)
        {
            var rect = control.RenderBounds;
            rect.X += 2;
            rect.Y += 2;
            rect.Width -= 4;
            rect.Height -= 4;

            mRenderer.DrawColor = mColControlBright;
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawLinedRect(rect);

            mRenderer.DrawColor = mColBorderColor;

            if (!open) // ! because the button shows intention, not the current state
            {
                mRenderer.DrawFilledRect(new Rectangle(rect.X + rect.Width / 2, rect.Y + 2, 1, rect.Height - 4));
            }

            mRenderer.DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + rect.Height / 2, rect.Width - 4, 1));
        }

        public override void DrawTreeControl(Control.Base control)
        {
            var rect = control.RenderBounds;

            mRenderer.DrawColor = mColControlBright;
            mRenderer.DrawFilledRect(rect);

            mRenderer.DrawColor = mColBorderColor;
            mRenderer.DrawLinedRect(rect);
        }

        public override void DrawTreeNode(
            Control.Base ctrl,
            bool open,
            bool selected,
            int labelHeight,
            int labelWidth,
            int halfWay,
            int lastBranch,
            bool isRoot
        )
        {
            if (selected)
            {
                Renderer.DrawColor = Color.FromArgb(100, 0, 150, 255);
                Renderer.DrawFilledRect(new Rectangle(17, 0, labelWidth + 2, labelHeight - 1));
                Renderer.DrawColor = Color.FromArgb(200, 0, 150, 255);
                Renderer.DrawLinedRect(new Rectangle(17, 0, labelWidth + 2, labelHeight - 1));
            }

            base.DrawTreeNode(ctrl, open, selected, labelHeight, labelWidth, halfWay, lastBranch, isRoot);
        }

        public override void DrawStatusBar(Control.Base control)
        {
            // todo
        }

        public override void DrawColorDisplay(Control.Base control, Color color)
        {
            var rect = control.RenderBounds;

            if (color.A != 255)
            {
                Renderer.DrawColor = Color.FromArgb(255, 255, 255, 255);
                Renderer.DrawFilledRect(rect);

                Renderer.DrawColor = Color.FromArgb(128, 128, 128, 128);

                Renderer.DrawFilledRect(Util.FloatRect(0, 0, rect.Width * 0.5f, rect.Height * 0.5f));
                Renderer.DrawFilledRect(
                    Util.FloatRect(rect.Width * 0.5f, rect.Height * 0.5f, rect.Width * 0.5f, rect.Height * 0.5f)
                );
            }

            Renderer.DrawColor = color;
            Renderer.DrawFilledRect(rect);

            Renderer.DrawColor = Color.FromArgb(255, 0, 0, 0);
            Renderer.DrawLinedRect(rect);
        }

        public override void DrawModalControl(Control.Base control)
        {
            if (control.ShouldDrawBackground)
            {
                var rect = control.RenderBounds;
                Renderer.DrawColor = mColModal;
                Renderer.DrawFilledRect(rect);
            }
        }

        public override void DrawMenuDivider(Control.Base control)
        {
            var rect = control.RenderBounds;
            Renderer.DrawColor = mColBgDark;
            Renderer.DrawFilledRect(rect);
            Renderer.DrawColor = mColControlDarker;
            Renderer.DrawLinedRect(rect);
        }

        public override void DrawMenuRightArrow(Control.Base control)
        {
            DrawArrowRight(control.RenderBounds);
        }

        public override void DrawSliderButton(Control.Base control, bool depressed, bool horizontal)
        {
            DrawButton(control, depressed, control.IsHovered, control.IsDisabled);
        }

        public override void DrawCategoryHolder(Control.Base control)
        {
            // todo
        }

        public override void DrawCategoryInner(Control.Base control, bool collapsed)
        {
            // todo
        }

        #endregion

    }

}
