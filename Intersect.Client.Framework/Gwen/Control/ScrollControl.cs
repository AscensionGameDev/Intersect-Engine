using System;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Base for controls whose interior can be scrolled.
    /// </summary>
    public class ScrollControl : Base
    {

        protected readonly ScrollBar mHorizontalScrollBar;

        protected readonly ScrollBar mVerticalScrollBar;

        private bool mAutoHideBars;

        private bool mCanScrollH;

        private bool mCanScrollV;

        private bool mUpdatingScrollbars;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScrollControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ScrollControl(Base parent, string name = "") : base(parent, name)
        {
            MouseInputEnabled = false;

            mVerticalScrollBar = new VerticalScrollBar(this);
            mVerticalScrollBar.Dock = Pos.Right;
            mVerticalScrollBar.BarMoved += VBarMoved;
            mCanScrollV = true;
            mVerticalScrollBar.NudgeAmount = 30;

            mHorizontalScrollBar = new HorizontalScrollBar(this);
            mHorizontalScrollBar.Dock = Pos.Bottom;
            mHorizontalScrollBar.BarMoved += HBarMoved;
            mCanScrollH = true;
            mHorizontalScrollBar.NudgeAmount = 30;

            mInnerPanel = new Base(this);
            mInnerPanel.SetPosition(0, 0);
            mInnerPanel.Margin = Margin.Five;
            mInnerPanel.SendToBack();
            mInnerPanel.MouseInputEnabled = false;

            mAutoHideBars = false;
        }

        public int VerticalScroll => mInnerPanel.Y;

        public int HorizontalScroll => mInnerPanel.X;

        public Base InnerPanel => mInnerPanel;

        /// <summary>
        ///     Indicates whether the control can be scrolled horizontally.
        /// </summary>
        public bool CanScrollH => mCanScrollH;

        /// <summary>
        ///     Indicates whether the control can be scrolled vertically.
        /// </summary>
        public bool CanScrollV => mCanScrollV;

        /// <summary>
        ///     Determines whether the scroll bars should be hidden if not needed.
        /// </summary>
        public bool AutoHideBars
        {
            get => mAutoHideBars;
            set => mAutoHideBars = value;
        }

        protected bool HScrollRequired
        {
            set
            {
                if (value)
                {
                    mHorizontalScrollBar.SetScrollAmount(0, true);
                    mHorizontalScrollBar.IsDisabled = true;
                    if (mAutoHideBars)
                    {
                        mHorizontalScrollBar.IsHidden = true;
                    }
                }
                else
                {
                    mHorizontalScrollBar.IsHidden = false;
                    mHorizontalScrollBar.IsDisabled = false;
                }
            }
        }

        protected bool VScrollRequired
        {
            set
            {
                if (value)
                {
                    mVerticalScrollBar.SetScrollAmount(0, true);
                    mVerticalScrollBar.IsDisabled = true;
                    if (mAutoHideBars)
                    {
                        mVerticalScrollBar.IsHidden = true;
                    }
                }
                else
                {
                    mVerticalScrollBar.IsHidden = false;
                    mVerticalScrollBar.IsDisabled = false;
                }
            }
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("CanScrollH", mCanScrollH);
            obj.Add("CanScrollV", mCanScrollV);
            obj.Add("AutoHideBars", mAutoHideBars);
            obj.Add("InnerPanel", mInnerPanel.GetJson());
            obj.Add("HorizontalScrollBar", mHorizontalScrollBar.GetJson());
            obj.Add("VerticalScrollBar", mVerticalScrollBar.GetJson());

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["CanScrollH"] != null)
            {
                mCanScrollH = (bool) obj["CanScrollH"];
            }

            if (obj["CanScrollV"] != null)
            {
                mCanScrollV = (bool) obj["CanScrollV"];
            }

            if (obj["AutoHideBars"] != null)
            {
                mAutoHideBars = (bool) obj["AutoHideBars"];
            }

            if (obj["InnerPanel"] != null)
            {
                mInnerPanel.LoadJson(obj["InnerPanel"]);
            }

            if (obj["HorizontalScrollBar"] != null)
            {
                mHorizontalScrollBar.LoadJson(obj["HorizontalScrollBar"]);
            }

            if (obj["VerticalScrollBar"] != null)
            {
                mVerticalScrollBar.LoadJson(obj["VerticalScrollBar"]);
            }
        }

        /// <summary>
        ///     Enables or disables inner scrollbars.
        /// </summary>
        /// <param name="horizontal">Determines whether the horizontal scrollbar should be enabled.</param>
        /// <param name="vertical">Determines whether the vertical scrollbar should be enabled.</param>
        public virtual void EnableScroll(bool horizontal, bool vertical)
        {
            mCanScrollV = vertical;
            mCanScrollH = horizontal;
            mVerticalScrollBar.IsHidden = !mCanScrollV;
            mHorizontalScrollBar.IsHidden = !mCanScrollH;
        }

        public virtual void SetInnerSize(int width, int height)
        {
            mInnerPanel.SetSize(width, height);
        }

        protected virtual void VBarMoved(Base control, EventArgs args)
        {
            Invalidate();
        }

        protected virtual void HBarMoved(Base control, EventArgs args)
        {
            Invalidate();
        }

        /// <summary>
        ///     Handler invoked when control children's bounds change.
        /// </summary>
        /// <param name="oldChildBounds"></param>
        /// <param name="child"></param>
        protected override void OnChildBoundsChanged(Rectangle oldChildBounds, Base child)
        {
            //UpdateScrollBars();
        }

        /// <summary>
        ///     Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Layout(Skin.Base skin)
        {
            UpdateScrollBars();
            base.Layout(skin);
        }

        /// <summary>
        ///     Handler invoked on mouse wheel event.
        /// </summary>
        /// <param name="delta">Scroll delta.</param>
        /// <returns></returns>
        protected override bool OnMouseWheeled(int delta)
        {
            if (CanScrollV && mVerticalScrollBar.IsVisible)
            {
                if (mVerticalScrollBar.SetScrollAmount(
                    mVerticalScrollBar.ScrollAmount - mVerticalScrollBar.NudgeAmount * (delta / 60.0f), true
                ))
                {
                    return true;
                }
            }

            if (CanScrollH && mHorizontalScrollBar.IsVisible)
            {
                if (mHorizontalScrollBar.SetScrollAmount(
                    mHorizontalScrollBar.ScrollAmount - mHorizontalScrollBar.NudgeAmount * (delta / 60.0f), true
                ))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Handler invoked on mouse wheel event.
        /// </summary>
        /// <param name="delta">Scroll delta.</param>
        /// <returns></returns>
        protected override bool OnMouseHWheeled(int delta)
        {

            if (CanScrollH && mHorizontalScrollBar.IsVisible)
            {
                var scrollValue = mHorizontalScrollBar.ScrollAmount - mHorizontalScrollBar.NudgeAmount * (delta / 60.0f);
                if (mHorizontalScrollBar.SetScrollAmount(scrollValue, true))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
#if false // Debug render - this shouldn't render ANYTHING REALLY - it should be up to the parent!
    Gwen::Rect rect = GetRenderBounds();
    Gwen::Renderer::Base* render = skin->GetRender();

    render->SetDrawColor( Gwen::Color( 255, 255, 0, 100 ) );
    render->DrawFilledRect( rect );

    render->SetDrawColor( Gwen::Color( 255, 0, 0, 100 ) );
    render->DrawFilledRect( m_InnerPanel->GetBounds() );

    render->RenderText( skin->GetDefaultFont(), Gwen::Point( 0, 0 ), Utility::Format( L"Offset: %i %i", m_InnerPanel->X(), m_InnerPanel->Y() ) );
#endif
        }

        public virtual void UpdateScrollBars()
        {
            if (!mUpdatingScrollbars)
            {
                if (mInnerPanel == null)
                {
                    return;
                }

                mUpdatingScrollbars = true;

                //Get the max size of all our children together
                var childrenWidth = Children.Count > 0 ? Children.Max(x => x.Right) : 0;
                var childrenHeight = Children.Count > 0 ? Children.Max(x => x.Bottom) : 0;

                if (mCanScrollH)
                {
                    mInnerPanel.SetSize(Math.Max(Width, childrenWidth), Math.Max(Height, childrenHeight));
                }
                else
                {
                    mInnerPanel.SetSize(
                        Width - (mVerticalScrollBar.IsHidden ? 0 : mVerticalScrollBar.Width),
                        Math.Max(Height, childrenHeight)
                    );
                }

                var wPercent = Width /
                               (float) (childrenWidth + (mVerticalScrollBar.IsHidden ? 0 : mVerticalScrollBar.Width));

                var hPercent = Height /
                               (float) (childrenHeight +
                                        (mHorizontalScrollBar.IsHidden ? 0 : mHorizontalScrollBar.Height));

                if (mCanScrollV)
                {
                    VScrollRequired = hPercent >= 1;
                }
                else
                {
                    mVerticalScrollBar.IsHidden = true;
                }

                if (mCanScrollH)
                {
                    HScrollRequired = wPercent >= 1;
                }
                else
                {
                    mHorizontalScrollBar.IsHidden = true;
                }

                mVerticalScrollBar.ContentSize = mInnerPanel.Height;
                mVerticalScrollBar.ViewableContentSize =
                    Height - (mHorizontalScrollBar.IsHidden ? 0 : mHorizontalScrollBar.Height);

                mHorizontalScrollBar.ContentSize = mInnerPanel.Width;
                mHorizontalScrollBar.ViewableContentSize =
                    Width - (mVerticalScrollBar.IsHidden ? 0 : mVerticalScrollBar.Width);

                var newInnerPanelPosX = 0;
                var newInnerPanelPosY = 0;

                if (CanScrollV && !mVerticalScrollBar.IsHidden)
                {
                    newInnerPanelPosY =
                        (int) (-(mInnerPanel.Height -
                                 Height +
                                 (mHorizontalScrollBar.IsHidden ? 0 : mHorizontalScrollBar.Height)) *
                               mVerticalScrollBar.ScrollAmount);
                }

                if (CanScrollH && !mHorizontalScrollBar.IsHidden)
                {
                    newInnerPanelPosX =
                        (int) (-(mInnerPanel.Width -
                                 Width +
                                 (mVerticalScrollBar.IsHidden ? 0 : mVerticalScrollBar.Width)) *
                               mHorizontalScrollBar.ScrollAmount);
                }

                mInnerPanel.SetPosition(newInnerPanelPosX, newInnerPanelPosY);
                mUpdatingScrollbars = false;
            }
        }

        public virtual void ScrollToBottom()
        {
            if (!CanScrollV)
            {
                return;
            }

            UpdateScrollBars();
            mVerticalScrollBar.ScrollToBottom();
        }

        public virtual void ScrollToTop()
        {
            if (CanScrollV)
            {
                UpdateScrollBars();
                mVerticalScrollBar.ScrollToTop();
            }
        }

        public virtual void ScrollToLeft()
        {
            if (CanScrollH)
            {
                UpdateScrollBars();
                mVerticalScrollBar.ScrollToLeft();
            }
        }

        public virtual void ScrollToRight()
        {
            if (CanScrollH)
            {
                UpdateScrollBars();
                mVerticalScrollBar.ScrollToRight();
            }
        }

        public virtual void DeleteAll()
        {
            mInnerPanel.DeleteAllChildren();
        }

        public ScrollBar GetVerticalScrollBar()
        {
            return mVerticalScrollBar;
        }

        public ScrollBar GetHorizontalScrollBar()
        {
            return mHorizontalScrollBar;
        }

    }

}
