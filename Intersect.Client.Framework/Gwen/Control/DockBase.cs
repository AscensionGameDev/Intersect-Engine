using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.DragDrop;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Base for dockable containers.
    /// </summary>
    public class DockBase : Base
    {

        private DockBase mBottom;

        // Only CHILD dockpanels have a tabcontrol.
        private DockedTabControl mDockedTabControl;

        private bool mDrawHover;

        private bool mDropFar;

        private Rectangle mHoverRect;

        private DockBase mLeft;

        private DockBase mRight;

        private Resizer mSizer;

        private DockBase mTop;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DockBase(Base parent) : base(parent)
        {
            Padding = Padding.One;
            SetSize(200, 200);
        }

        // todo: dock events?

        /// <summary>
        ///     Control docked on the left side.
        /// </summary>
        public DockBase LeftDock => GetChildDock(Pos.Left);

        /// <summary>
        ///     Control docked on the right side.
        /// </summary>
        public DockBase RightDock => GetChildDock(Pos.Right);

        /// <summary>
        ///     Control docked on the top side.
        /// </summary>
        public DockBase TopDock => GetChildDock(Pos.Top);

        /// <summary>
        ///     Control docked on the bottom side.
        /// </summary>
        public DockBase BottomDock => GetChildDock(Pos.Bottom);

        public TabControl TabControl => mDockedTabControl;

        /// <summary>
        ///     Indicates whether the control contains any docked children.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                if (mDockedTabControl != null && mDockedTabControl.TabCount > 0)
                {
                    return false;
                }

                if (mLeft != null && !mLeft.IsEmpty)
                {
                    return false;
                }

                if (mRight != null && !mRight.IsEmpty)
                {
                    return false;
                }

                if (mTop != null && !mTop.IsEmpty)
                {
                    return false;
                }

                if (mBottom != null && !mBottom.IsEmpty)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            // No action on space (default button action is to press)
            return false;
        }

        /// <summary>
        ///     Initializes an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos">Dock position.</param>
        protected virtual void SetupChildDock(Pos pos)
        {
            if (mDockedTabControl == null)
            {
                mDockedTabControl = new DockedTabControl(this);
                mDockedTabControl.TabRemoved += OnTabRemoved;
                mDockedTabControl.TabStripPosition = Pos.Bottom;
                mDockedTabControl.TitleBarVisible = true;
            }

            Dock = pos;

            Pos sizeDir;
            if (pos == Pos.Right)
            {
                sizeDir = Pos.Left;
            }
            else if (pos == Pos.Left)
            {
                sizeDir = Pos.Right;
            }
            else if (pos == Pos.Top)
            {
                sizeDir = Pos.Bottom;
            }
            else if (pos == Pos.Bottom)
            {
                sizeDir = Pos.Top;
            }
            else
            {
                throw new ArgumentException("Invalid dock", "pos");
            }

            if (mSizer != null)
            {
                mSizer.Dispose();
            }

            mSizer = new Resizer(this);
            mSizer.Dock = sizeDir;
            mSizer.ResizeDir = sizeDir;
            mSizer.SetSize(2, 2);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
        }

        /// <summary>
        ///     Gets an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected virtual DockBase GetChildDock(Pos pos)
        {
            // todo: verify
            DockBase dock = null;
            switch (pos)
            {
                case Pos.Left:
                    if (mLeft == null)
                    {
                        mLeft = new DockBase(this);
                        mLeft.SetupChildDock(pos);
                    }

                    dock = mLeft;

                    break;

                case Pos.Right:
                    if (mRight == null)
                    {
                        mRight = new DockBase(this);
                        mRight.SetupChildDock(pos);
                    }

                    dock = mRight;

                    break;

                case Pos.Top:
                    if (mTop == null)
                    {
                        mTop = new DockBase(this);
                        mTop.SetupChildDock(pos);
                    }

                    dock = mTop;

                    break;

                case Pos.Bottom:
                    if (mBottom == null)
                    {
                        mBottom = new DockBase(this);
                        mBottom.SetupChildDock(pos);
                    }

                    dock = mBottom;

                    break;
            }

            if (dock != null)
            {
                dock.IsHidden = false;
            }

            return dock;
        }

        /// <summary>
        ///     Calculates dock direction from dragdrop coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Dock direction.</returns>
        protected virtual Pos GetDroppedTabDirection(int x, int y)
        {
            var w = Width;
            var h = Height;
            var top = y / (float) h;
            var left = x / (float) w;
            var right = (w - x) / (float) w;
            var bottom = (h - y) / (float) h;
            var minimum = Math.Min(Math.Min(Math.Min(top, left), right), bottom);

            mDropFar = minimum < 0.2f;

            if (minimum > 0.3f)
            {
                return Pos.Fill;
            }

            if (top == minimum && (null == mTop || mTop.IsHidden))
            {
                return Pos.Top;
            }

            if (left == minimum && (null == mLeft || mLeft.IsHidden))
            {
                return Pos.Left;
            }

            if (right == minimum && (null == mRight || mRight.IsHidden))
            {
                return Pos.Right;
            }

            if (bottom == minimum && (null == mBottom || mBottom.IsHidden))
            {
                return Pos.Bottom;
            }

            return Pos.Fill;
        }

        public override bool DragAndDrop_CanAcceptPackage(Package p)
        {
            // A TAB button dropped 
            if (p.Name == "TabButtonMove")
            {
                return true;
            }

            // a TAB window dropped
            if (p.Name == "TabWindowMove")
            {
                return true;
            }

            return false;
        }

        public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
        {
            var pos = CanvasPosToLocal(new Point(x, y));
            var dir = GetDroppedTabDirection(pos.X, pos.Y);

            var addTo = mDockedTabControl;
            if (dir == Pos.Fill && addTo == null)
            {
                return false;
            }

            if (dir != Pos.Fill)
            {
                var dock = GetChildDock(dir);
                addTo = dock.mDockedTabControl;

                if (!mDropFar)
                {
                    dock.BringToFront();
                }
                else
                {
                    dock.SendToBack();
                }
            }

            if (p.Name == "TabButtonMove")
            {
                var tabButton = DragAndDrop.SourceControl as TabButton;
                if (null == tabButton)
                {
                    return false;
                }

                addTo.AddPage(tabButton);
            }

            if (p.Name == "TabWindowMove")
            {
                var tabControl = DragAndDrop.SourceControl as DockedTabControl;
                if (null == tabControl)
                {
                    return false;
                }

                if (tabControl == addTo)
                {
                    return false;
                }

                tabControl.MoveTabsTo(addTo);
            }

            Invalidate();

            return true;
        }

        protected virtual void OnTabRemoved(Base control, EventArgs args)
        {
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        protected virtual void DoRedundancyCheck()
        {
            if (!IsEmpty)
            {
                return;
            }

            var pDockParent = Parent as DockBase;
            if (null == pDockParent)
            {
                return;
            }

            pDockParent.OnRedundantChildDock(this);
        }

        protected virtual void DoConsolidateCheck()
        {
            if (IsEmpty)
            {
                return;
            }

            if (null == mDockedTabControl)
            {
                return;
            }

            if (mDockedTabControl.TabCount > 0)
            {
                return;
            }

            if (mBottom != null && !mBottom.IsEmpty)
            {
                mBottom.mDockedTabControl.MoveTabsTo(mDockedTabControl);

                return;
            }

            if (mTop != null && !mTop.IsEmpty)
            {
                mTop.mDockedTabControl.MoveTabsTo(mDockedTabControl);

                return;
            }

            if (mLeft != null && !mLeft.IsEmpty)
            {
                mLeft.mDockedTabControl.MoveTabsTo(mDockedTabControl);

                return;
            }

            if (mRight != null && !mRight.IsEmpty)
            {
                mRight.mDockedTabControl.MoveTabsTo(mDockedTabControl);

                return;
            }
        }

        protected virtual void OnRedundantChildDock(DockBase dock)
        {
            dock.IsHidden = true;
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        public override void DragAndDrop_HoverEnter(Package p, int x, int y)
        {
            mDrawHover = true;
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            mDrawHover = false;
        }

        public override void DragAndDrop_Hover(Package p, int x, int y)
        {
            var pos = CanvasPosToLocal(new Point(x, y));
            var dir = GetDroppedTabDirection(pos.X, pos.Y);

            if (dir == Pos.Fill)
            {
                if (null == mDockedTabControl)
                {
                    mHoverRect = Rectangle.Empty;

                    return;
                }

                mHoverRect = InnerBounds;

                return;
            }

            mHoverRect = RenderBounds;

            var helpBarWidth = 0;

            if (dir == Pos.Left)
            {
                helpBarWidth = (int) (mHoverRect.Width * 0.25f);
                mHoverRect.Width = helpBarWidth;
            }

            if (dir == Pos.Right)
            {
                helpBarWidth = (int) (mHoverRect.Width * 0.25f);
                mHoverRect.X = mHoverRect.Width - helpBarWidth;
                mHoverRect.Width = helpBarWidth;
            }

            if (dir == Pos.Top)
            {
                helpBarWidth = (int) (mHoverRect.Height * 0.25f);
                mHoverRect.Height = helpBarWidth;
            }

            if (dir == Pos.Bottom)
            {
                helpBarWidth = (int) (mHoverRect.Height * 0.25f);
                mHoverRect.Y = mHoverRect.Height - helpBarWidth;
                mHoverRect.Height = helpBarWidth;
            }

            if ((dir == Pos.Top || dir == Pos.Bottom) && !mDropFar)
            {
                if (mLeft != null && mLeft.IsVisible)
                {
                    mHoverRect.X += mLeft.Width;
                    mHoverRect.Width -= mLeft.Width;
                }

                if (mRight != null && mRight.IsVisible)
                {
                    mHoverRect.Width -= mRight.Width;
                }
            }

            if ((dir == Pos.Left || dir == Pos.Right) && !mDropFar)
            {
                if (mTop != null && mTop.IsVisible)
                {
                    mHoverRect.Y += mTop.Height;
                    mHoverRect.Height -= mTop.Height;
                }

                if (mBottom != null && mBottom.IsVisible)
                {
                    mHoverRect.Height -= mBottom.Height;
                }
            }
        }

        /// <summary>
        ///     Renders over the actual control (overlays).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderOver(Skin.Base skin)
        {
            if (!mDrawHover)
            {
                return;
            }

            var render = skin.Renderer;
            render.DrawColor = Color.FromArgb(20, 255, 200, 255);
            render.DrawFilledRect(RenderBounds);

            if (mHoverRect.Width == 0)
            {
                return;
            }

            render.DrawColor = Color.FromArgb(100, 255, 200, 255);
            render.DrawFilledRect(mHoverRect);

            render.DrawColor = Color.FromArgb(200, 255, 200, 255);
            render.DrawLinedRect(mHoverRect);
        }

    }

}
