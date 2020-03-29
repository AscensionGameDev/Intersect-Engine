using System;

using Intersect.Client.Framework.Gwen.ControlInternal;

using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Base resizable control.
    /// </summary>
    public class ResizableControl : Base
    {

        private readonly Resizer[] mResizer;

        private bool mClampMovement;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResizableControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ResizableControl(Base parent, string name = "") : base(parent, name)
        {
            mResizer = new Resizer[10];
            MinimumSize = new Point(5, 5);
            mClampMovement = false;

            mResizer[2] = new Resizer(this);
            mResizer[2].Dock = Pos.Bottom;
            mResizer[2].ResizeDir = Pos.Bottom;
            mResizer[2].Resized += OnResized;
            mResizer[2].Target = this;

            mResizer[1] = new Resizer(mResizer[2]);
            mResizer[1].Dock = Pos.Left;
            mResizer[1].ResizeDir = Pos.Bottom | Pos.Left;
            mResizer[1].Resized += OnResized;
            mResizer[1].Target = this;

            mResizer[3] = new Resizer(mResizer[2]);
            mResizer[3].Dock = Pos.Right;
            mResizer[3].ResizeDir = Pos.Bottom | Pos.Right;
            mResizer[3].Resized += OnResized;
            mResizer[3].Target = this;

            mResizer[8] = new Resizer(this);
            mResizer[8].Dock = Pos.Top;
            mResizer[8].ResizeDir = Pos.Top;
            mResizer[8].Resized += OnResized;
            mResizer[8].Target = this;

            mResizer[7] = new Resizer(mResizer[8]);
            mResizer[7].Dock = Pos.Left;
            mResizer[7].ResizeDir = Pos.Left | Pos.Top;
            mResizer[7].Resized += OnResized;
            mResizer[7].Target = this;

            mResizer[9] = new Resizer(mResizer[8]);
            mResizer[9].Dock = Pos.Right;
            mResizer[9].ResizeDir = Pos.Right | Pos.Top;
            mResizer[9].Resized += OnResized;
            mResizer[9].Target = this;

            mResizer[4] = new Resizer(this);
            mResizer[4].Dock = Pos.Left;
            mResizer[4].ResizeDir = Pos.Left;
            mResizer[4].Resized += OnResized;
            mResizer[4].Target = this;

            mResizer[6] = new Resizer(this);
            mResizer[6].Dock = Pos.Right;
            mResizer[6].ResizeDir = Pos.Right;
            mResizer[6].Resized += OnResized;
            mResizer[6].Target = this;
        }

        /// <summary>
        ///     Determines whether control's position should be restricted to its parent bounds.
        /// </summary>
        public bool ClampMovement
        {
            get => mClampMovement;
            set => mClampMovement = value;
        }

        /// <summary>
        ///     Invoked when the control has been resized.
        /// </summary>
        public event GwenEventHandler<EventArgs> Resized;

        public override JObject GetJson()
        {
            var obj = base.GetJson();
            obj.Add("ClampMovement", ClampMovement);

            return base.FixJson(obj);
        }

        public override void LoadJson(JToken obj)
        {
            base.LoadJson(obj);
            if (obj["ClampMovement"] != null)
            {
                ClampMovement = (bool) obj["ClampMovement"];
            }
        }

        /// <summary>
        ///     Handler for the resized event.
        /// </summary>
        /// <param name="control">Event source.</param>
        protected virtual void OnResized(Base control, EventArgs args)
        {
            if (Resized != null)
            {
                Resized.Invoke(this, EventArgs.Empty);
            }
        }

        protected Resizer GetResizer(int i)
        {
            return mResizer[i];
        }

        /// <summary>
        ///     Disables resizing.
        /// </summary>
        public virtual void DisableResizing()
        {
            for (var i = 0; i < 10; i++)
            {
                if (mResizer[i] == null)
                {
                    continue;
                }

                mResizer[i].MouseInputEnabled = false;
                mResizer[i].IsHidden = true;
                Padding = new Padding(mResizer[i].Width, mResizer[i].Width, mResizer[i].Width, mResizer[i].Width);
            }
        }

        /// <summary>
        ///     Enables resizing.
        /// </summary>
        public void EnableResizing()
        {
            for (var i = 0; i < 10; i++)
            {
                if (mResizer[i] == null)
                {
                    continue;
                }

                mResizer[i].MouseInputEnabled = true;
                mResizer[i].IsHidden = false;
                Padding = new Padding(0, 0, 0, 0); // todo: check if ok
            }
        }

        /// <summary>
        ///     Sets the control bounds.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <returns>
        ///     True if bounds changed.
        /// </returns>
        public override bool SetBounds(int x, int y, int width, int height)
        {
            var minSize = MinimumSize;

            // Clamp Minimum Size
            if (width < minSize.X)
            {
                width = minSize.X;
            }

            if (height < minSize.Y)
            {
                height = minSize.Y;
            }

            // Clamp to parent's window
            var parent = Parent;
            if (parent != null && mClampMovement)
            {
                if (x + width > parent.Width)
                {
                    x = parent.Width - width;
                }

                if (x < 0)
                {
                    x = 0;
                }

                if (y + height > parent.Height)
                {
                    y = parent.Height - height;
                }

                if (y < 0)
                {
                    y = 0;
                }
            }

            return base.SetBounds(x, y, width, height);
        }

        /// <summary>
        ///     Sets the control size.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        /// <returns>True if bounds changed.</returns>
        public override bool SetSize(int width, int height)
        {
            var changed = base.SetSize(width, height);
            if (changed)
            {
                OnResized(this, EventArgs.Empty);
            }

            return changed;
        }

    }

}
