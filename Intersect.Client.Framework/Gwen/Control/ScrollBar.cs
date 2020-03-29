using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Base class for scrollbars.
    /// </summary>
    public class ScrollBar : Base
    {

        protected readonly ScrollBarBar mBar;

        protected readonly ScrollBarButton[] mScrollButton;

        private string mBackgroundTemplateFilename;

        private GameTexture mBackgroundTemplateTex;

        protected float mContentSize;

        protected bool mDepressed;

        protected float mNudgeAmount;

        protected float mScrollAmount;

        protected float mViewableContentSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScrollBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        protected ScrollBar(Base parent) : base(parent)
        {
            mScrollButton = new ScrollBarButton[2];
            mScrollButton[0] = new ScrollBarButton(this);
            mScrollButton[1] = new ScrollBarButton(this);

            mBar = new ScrollBarBar(this);

            SetBounds(0, 0, 15, 15);
            mDepressed = false;

            mScrollAmount = 0;
            mContentSize = 0;
            mViewableContentSize = 0;

            NudgeAmount = 20;
        }

        /// <summary>
        ///     Bar size (in pixels).
        /// </summary>
        public virtual int BarSize { get; set; }

        /// <summary>
        ///     Bar position (in pixels).
        /// </summary>
        public virtual int BarPos => 0;

        /// <summary>
        ///     Button size (in pixels).
        /// </summary>
        public virtual int ButtonSize => 0;

        public virtual float NudgeAmount
        {
            get => mNudgeAmount / mContentSize;
            set => mNudgeAmount = value;
        }

        public float ScrollAmount => mScrollAmount;

        public float ContentSize
        {
            get => mContentSize;
            set
            {
                if (mContentSize != value)
                {
                    Invalidate();
                }

                mContentSize = value;
            }
        }

        public float ViewableContentSize
        {
            get => mViewableContentSize;
            set
            {
                if (mViewableContentSize != value)
                {
                    Invalidate();
                }

                mViewableContentSize = value;
            }
        }

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public virtual bool IsHorizontal => false;

        /// <summary>
        ///     Invoked when the bar is moved.
        /// </summary>
        public event GwenEventHandler<EventArgs> BarMoved;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("BackgroundTemplate", mBackgroundTemplateFilename);
            obj.Add("UpOrLeftButton", mScrollButton[0].GetJson());
            obj.Add("Bar", mBar.GetJson());
            obj.Add("DownOrRightButton", mScrollButton[1].GetJson());

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["BackgroundTemplate"] != null)
            {
                SetBackgroundTemplate(
                    GameContentManager.Current.GetTexture(
                        GameContentManager.TextureType.Gui, (string) obj["BackgroundTemplate"]
                    ), (string) obj["BackgroundTemplate"]
                );
            }

            if (obj["UpOrLeftButton"] != null)
            {
                mScrollButton[0].LoadJson(obj["UpOrLeftButton"]);
            }

            if (obj["Bar"] != null)
            {
                mBar.LoadJson(obj["Bar"]);
            }

            if (obj["DownOrRightButton"] != null)
            {
                mScrollButton[1].LoadJson(obj["DownOrRightButton"]);
            }
        }

        public GameTexture GetTemplate()
        {
            return mBackgroundTemplateTex;
        }

        public void SetBackgroundTemplate(GameTexture texture, string fileName)
        {
            if (texture == null && !string.IsNullOrWhiteSpace(fileName))
            {
                texture = GameContentManager.Current?.GetTexture(GameContentManager.TextureType.Gui, fileName);
            }

            mBackgroundTemplateFilename = fileName;
            mBackgroundTemplateTex = texture;
        }

        /// <summary>
        ///     Sets the scroll amount (0-1).
        /// </summary>
        /// <param name="value">Scroll amount.</param>
        /// <param name="forceUpdate">Determines whether the control should be updated.</param>
        /// <returns>True if control state changed.</returns>
        public virtual bool SetScrollAmount(float value, bool forceUpdate = false)
        {
            if (mScrollAmount == value && !forceUpdate)
            {
                return false;
            }

            mScrollAmount = value;
            Invalidate();
            OnBarMoved(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
        {
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            skin.DrawScrollBar(this, IsHorizontal, mDepressed);
        }

        /// <summary>
        ///     Handler for the BarMoved event.
        /// </summary>
        /// <param name="control">The control.</param>
        protected virtual void OnBarMoved(Base control, EventArgs args)
        {
            if (BarMoved != null)
            {
                BarMoved.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual float CalculateScrolledAmount()
        {
            return 0;
        }

        protected virtual int CalculateBarSize()
        {
            return 0;
        }

        public virtual void ScrollToLeft()
        {
        }

        public virtual void ScrollToRight()
        {
        }

        public virtual void ScrollToTop()
        {
        }

        public virtual void ScrollToBottom()
        {
        }

        public ScrollBarButton GetScrollBarButton(Pos direction)
        {
            for (var i = 0; i < mScrollButton.Length; i++)
            {
                if (mScrollButton[i].GetDirection() == direction)
                {
                    return mScrollButton[i];
                }
            }

            return null;
        }

        public void SetScrollBarImage(GameTexture texture, string fileName, Dragger.ControlState state)
        {
            mBar.SetImage(texture, fileName, state);
        }

        public GameTexture GetScrollBarImage(Dragger.ControlState state)
        {
            return mBar.GetImage(state);
        }

    }

}
