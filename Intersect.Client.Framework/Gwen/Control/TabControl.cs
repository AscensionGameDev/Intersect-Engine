using System;

using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Control with multiple tabs that can be reordered and dragged.
    /// </summary>
    public class TabControl : Base
    {

        private readonly ScrollBarButton[] mScroll;

        private readonly TabStrip mTabStrip;

        private TabButton mCurrentButton;

        private int mScrollOffset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabControl(Base parent) : base(parent)
        {
            mScroll = new ScrollBarButton[2];
            mScrollOffset = 0;

            mTabStrip = new TabStrip(this);
            mTabStrip.StripPosition = Pos.Top;

            // Make this some special control?
            mScroll[0] = new ScrollBarButton(this);
            mScroll[0].SetDirectionLeft();
            mScroll[0].Clicked += ScrollPressedLeft;
            mScroll[0].SetSize(14, 16);

            mScroll[1] = new ScrollBarButton(this);
            mScroll[1].SetDirectionRight();
            mScroll[1].Clicked += ScrollPressedRight;
            mScroll[1].SetSize(14, 16);

            mInnerPanel = new TabControlInner(this);
            mInnerPanel.Dock = Pos.Fill;
            mInnerPanel.SendToBack();

            IsTabable = false;
        }

        /// <summary>
        ///     Determines if tabs can be reordered by dragging.
        /// </summary>
        public bool AllowReorder
        {
            get => mTabStrip.AllowReorder;
            set => mTabStrip.AllowReorder = value;
        }

        /// <summary>
        ///     Currently active tab button.
        /// </summary>
        public TabButton CurrentButton => mCurrentButton;

        /// <summary>
        ///     Current tab strip position.
        /// </summary>
        public Pos TabStripPosition
        {
            get => mTabStrip.StripPosition;
            set => mTabStrip.StripPosition = value;
        }

        /// <summary>
        ///     Tab strip.
        /// </summary>
        public TabStrip TabStrip => mTabStrip;

        /// <summary>
        ///     Number of tabs in the control.
        /// </summary>
        public int TabCount => mTabStrip.Children.Count;

        /// <summary>
        ///     Invoked when a tab has been added.
        /// </summary>
        public event GwenEventHandler<EventArgs> TabAdded;

        /// <summary>
        ///     Invoked when a tab has been removed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TabRemoved;

        /// <summary>
        ///     Adds a new page/tab.
        /// </summary>
        /// <param name="label">Tab label.</param>
        /// <param name="page">Page contents.</param>
        /// <returns>Newly created control.</returns>
        public TabButton AddPage(string label, Base page = null)
        {
            if (null == page)
            {
                page = new Base(this);
            }
            else
            {
                page.Parent = this;
            }

            var button = new TabButton(mTabStrip);
            button.SetText(label);
            button.Page = page;
            button.IsTabable = false;

            AddPage(button);

            return button;
        }

        /// <summary>
        ///     Adds a page/tab.
        /// </summary>
        /// <param name="button">Page to add. (well, it's a TabButton which is a parent to the page).</param>
        public void AddPage(TabButton button)
        {
            var page = button.Page;
            page.Parent = this;
            page.IsHidden = true;
            page.Margin = new Margin(6, 6, 6, 6);
            page.Dock = Pos.Fill;

            button.Parent = mTabStrip;
            button.Dock = Pos.Left;
            button.SizeToContents();
            if (button.TabControl != null)
            {
                button.TabControl.UnsubscribeTabEvent(button);
            }

            button.TabControl = this;
            button.Clicked += OnTabPressed;

            if (null == mCurrentButton)
            {
                button.Press();
            }

            if (TabAdded != null)
            {
                TabAdded.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        private void UnsubscribeTabEvent(TabButton button)
        {
            button.Clicked -= OnTabPressed;
        }

        /// <summary>
        ///     Handler for tab selection.
        /// </summary>
        /// <param name="control">Event source (TabButton).</param>
        internal virtual void OnTabPressed(Base control, EventArgs args)
        {
            var button = control as TabButton;
            if (null == button)
            {
                return;
            }

            var page = button.Page;
            if (null == page)
            {
                return;
            }

            if (mCurrentButton == button)
            {
                return;
            }

            if (null != mCurrentButton)
            {
                var page2 = mCurrentButton.Page;
                if (page2 != null)
                {
                    page2.IsHidden = true;
                }

                mCurrentButton.Redraw();
                mCurrentButton = null;
            }

            mCurrentButton = button;

            page.IsHidden = false;

            mTabStrip.Invalidate();
            Invalidate();
        }

        /// <summary>
        ///     Function invoked after layout.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void PostLayout(Skin.Base skin)
        {
            base.PostLayout(skin);
            HandleOverflow();
        }

        /// <summary>
        ///     Handler for tab removing.
        /// </summary>
        /// <param name="button"></param>
        internal virtual void OnLoseTab(TabButton button)
        {
            if (mCurrentButton == button)
            {
                mCurrentButton = null;
            }

            //TODO: Select a tab if any exist.

            if (TabRemoved != null)
            {
                TabRemoved.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        private void HandleOverflow()
        {
            var tabsSize = mTabStrip.GetChildrenSize();

            // Only enable the scrollers if the tabs are at the top.
            // This is a limitation we should explore.
            // Really TabControl should have derivitives for tabs placed elsewhere where we could specialize 
            // some functions like this for each direction.
            var needed = tabsSize.X > Width && mTabStrip.Dock == Pos.Top;

            mScroll[0].IsHidden = !needed;
            mScroll[1].IsHidden = !needed;

            if (!needed)
            {
                return;
            }

            mScrollOffset = Util.Clamp(mScrollOffset, 0, tabsSize.X - Width + 32);

#if false //
// This isn't frame rate independent. 
// Could be better. Get rid of m_ScrollOffset and just use m_TabStrip.GetMargin().left ?
// Then get a margin animation type and do it properly! 
// TODO!
//
        m_TabStrip.SetMargin( Margin( Gwen::Approach( m_TabStrip.GetMargin().left, m_iScrollOffset * -1, 2 ), 0, 0, 0 ) );
        InvalidateParent();
#else
            mTabStrip.Margin = new Margin(mScrollOffset * -1, 0, 0, 0);
#endif

            mScroll[0].SetPosition(Width - 30, 5);
            mScroll[1].SetPosition(mScroll[0].Right, 5);
        }

        protected virtual void ScrollPressedLeft(Base control, EventArgs args)
        {
            mScrollOffset -= 120;
        }

        protected virtual void ScrollPressedRight(Base control, EventArgs args)
        {
            mScrollOffset += 120;
        }

    }

}
