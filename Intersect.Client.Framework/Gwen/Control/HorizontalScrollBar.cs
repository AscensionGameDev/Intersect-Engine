using System;

using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Horizontal scrollbar.
    /// </summary>
    public class HorizontalScrollBar : ScrollBar
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="HorizontalScrollBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalScrollBar(Base parent) : base(parent)
        {
            mBar.IsHorizontal = true;

            mScrollButton[0].SetDirectionLeft();
            mScrollButton[0].Clicked += NudgeLeft;

            mScrollButton[1].SetDirectionRight();
            mScrollButton[1].Clicked += NudgeRight;

            mBar.Dragged += OnBarMoved;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public override int BarSize
        {
            get => mBar.Width;
            set => mBar.Width = value;
        }

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public override int BarPos => mBar.X - Height;

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public override bool IsHorizontal => true;

        /// <summary>
        ///     Button size (in pixels).
        /// </summary>
        public override int ButtonSize => Height;

        public override float NudgeAmount
        {
            get
            {
                if (mDepressed)
                {
                    return mViewableContentSize / mContentSize;
                }
                else
                {
                    return base.NudgeAmount;
                }
            }
            set => base.NudgeAmount = value;
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            base.Layout(skin);

            mScrollButton[0].Width = Height;
            mScrollButton[0].Dock = Pos.Left;

            mScrollButton[1].Width = Height;
            mScrollButton[1].Dock = Pos.Right;

            mBar.Height = ButtonSize;
            mBar.Padding = new Padding(ButtonSize, 0, ButtonSize, 0);

            var barWidth = mViewableContentSize / mContentSize * (Width - ButtonSize * 2);

            if (barWidth < ButtonSize * 0.5f)
            {
                barWidth = (int) (ButtonSize * 0.5f);
            }

            mBar.Width = (int) barWidth;
            mBar.IsHidden = Width - ButtonSize * 2 <= barWidth;

            //Based on our last scroll amount, produce a position for the bar
            if (!mBar.IsHeld)
            {
                SetScrollAmount(ScrollAmount, true);
            }
        }

        public void NudgeLeft(Base control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount - NudgeAmount, true);
                base.PlaySound(mBar.GetMouseUpSound());
            }
        }

        public void NudgeRight(Base control, EventArgs args)
        {
            if (!IsDisabled)
            {
                SetScrollAmount(ScrollAmount + NudgeAmount, true);
                base.PlaySound(mBar.GetMouseUpSound());
            }
        }

        public override void ScrollToLeft()
        {
            SetScrollAmount(0, true);
        }

        public override void ScrollToRight()
        {
            SetScrollAmount(1, true);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
        {
            base.OnMouseClickedLeft(x, y, down);
            if (down)
            {
                mDepressed = true;
                InputHandler.MouseFocus = this;
            }
            else
            {
                var clickPos = CanvasPosToLocal(new Point(x, y));
                if (clickPos.X < mBar.X)
                {
                    NudgeLeft(this, EventArgs.Empty);
                }
                else if (clickPos.X > mBar.X + mBar.Width)
                {
                    NudgeRight(this, EventArgs.Empty);
                }

                mDepressed = false;
                InputHandler.MouseFocus = null;
            }
        }

        protected override float CalculateScrolledAmount()
        {
            return (float) (mBar.X - ButtonSize) / (Width - mBar.Width - ButtonSize * 2);
        }

        /// <summary>
        ///     Sets the scroll amount (0-1).
        /// </summary>
        /// <param name="value">Scroll amount.</param>
        /// <param name="forceUpdate">Determines whether the control should be updated.</param>
        /// <returns>
        ///     True if control state changed.
        /// </returns>
        public override bool SetScrollAmount(float value, bool forceUpdate = false)
        {
            value = Util.Clamp(value, 0, 1);

            if (!base.SetScrollAmount(value, forceUpdate))
            {
                return false;
            }

            if (forceUpdate)
            {
                var newX = (int) (ButtonSize + value * (Width - mBar.Width - ButtonSize * 2));
                mBar.MoveTo(newX, mBar.Y);
            }

            return true;
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected override void OnBarMoved(Base control, EventArgs args)
        {
            if (mBar.IsHeld)
            {
                SetScrollAmount(CalculateScrolledAmount(), false);
                base.OnBarMoved(control, args);
            }
            else
            {
                InvalidateParent();
            }
        }

    }

}
