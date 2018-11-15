using System;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{
    /// <summary>
    ///     Base for controls that can be dragged by mouse.
    /// </summary>
    public class Dragger : Base
    {
        public enum ControlState
        {
            Normal = 0,
            Hovered,
            Clicked,
            Disabled,
        }

        private GameTexture mClickedImage;
        private GameTexture mDisabledImage;
        private string mClickedImageFilename;
        private string mDisabledImageFilename;

        protected bool mHeld;
        protected Point mHoldPos;
        private GameTexture mHoverImage;
        private GameTexture mNormalImage;
        private string mHoverImageFilename;
        private string mNormalImageFilename;
        protected Base mTarget;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dragger" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Dragger(Base parent, string name = "") : base(parent, name)
        {
            MouseInputEnabled = true;
            mHeld = false;
        }

        internal Base Target
        {
            get => mTarget;
            set => mTarget = value;
        }

        /// <summary>
        ///     Indicates if the control is being dragged.
        /// </summary>
        public bool IsHeld => mHeld;

        /// <summary>
        ///     Event invoked when the control position has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Dragged;

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            if (null == mTarget) return;

            if (down)
            {
                mHeld = true;
                mHoldPos = mTarget.CanvasPosToLocal(new Point(x, y));
                InputHandler.MouseFocus = this;
            }
            else
            {
                mHeld = false;

                InputHandler.MouseFocus = null;
            }
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(int x, int y, int dx, int dy)
        {
            if (null == mTarget) return;
            if (!mHeld) return;

            Point p = new Point(x - mHoldPos.X, y - mHoldPos.Y);

            // Translate to parent
            if (mTarget.Parent != null)
                p = mTarget.Parent.CanvasPosToLocal(p);

            //m_Target->SetPosition( p.x, p.y );
            mTarget.MoveTo(p.X, p.Y);
            if (Dragged != null)
                Dragged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
        }

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("NormalImage", GetImageFilename(ControlState.Normal));
            obj.Add("HoveredImage", GetImageFilename(ControlState.Hovered));
            obj.Add("ClickedImage", GetImageFilename(ControlState.Clicked));
            obj.Add("DisabledImage", GetImageFilename(ControlState.Disabled));
            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["NormalImage"] != null) SetImage(GameContentManager.Current.GetTexture(GameContentManager.TextureType.Gui, (string)obj["NormalImage"]), (string)obj["NormalImage"], ControlState.Normal);
            if (obj["HoveredImage"] != null) SetImage(GameContentManager.Current.GetTexture(GameContentManager.TextureType.Gui, (string)obj["HoveredImage"]), (string)obj["HoveredImage"], ControlState.Hovered);
            if (obj["ClickedImage"] != null) SetImage(GameContentManager.Current.GetTexture(GameContentManager.TextureType.Gui, (string)obj["ClickedImage"]), (string)obj["ClickedImage"], ControlState.Clicked);
            if (obj["DisabledImage"] != null) SetImage(GameContentManager.Current.GetTexture(GameContentManager.TextureType.Gui, (string)obj["DisabledImage"]), (string)obj["DisabledImage"], ControlState.Disabled);
        }

        /// <summary>
        ///     Sets the button's image.
        /// </summary>
        /// <param name="textureName">Texture name. Null to remove.</param>
        public virtual void SetImage(GameTexture texture, string fileName, ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    mNormalImageFilename = fileName;
                    mNormalImage = texture;
                    break;
                case ControlState.Hovered:
                    mHoverImageFilename = fileName;
                    mHoverImage = texture;
                    break;
                case ControlState.Clicked:
                    mClickedImageFilename = fileName;
                    mClickedImage = texture;
                    break;
                case ControlState.Disabled:
                    mDisabledImageFilename = fileName;
                    mDisabledImage = texture;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public virtual GameTexture GetImage(ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    return mNormalImage;
                case ControlState.Hovered:
                    return mHoverImage;
                case ControlState.Clicked:
                    return mClickedImage;
                case ControlState.Disabled:
                    return mDisabledImage;
                default:
                    return null;
            }
        }

        public virtual string GetImageFilename(ControlState state)
        {
            switch (state)
            {
                case ControlState.Normal:
                    return mNormalImageFilename;
                case ControlState.Hovered:
                    return mHoverImageFilename;
                case ControlState.Clicked:
                    return mClickedImageFilename;
                case ControlState.Disabled:
                    return mDisabledImageFilename;
                default:
                    return null;
            }
        }
    }
}