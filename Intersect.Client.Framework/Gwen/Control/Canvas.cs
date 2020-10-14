using System;
using System.Collections.Generic;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Anim;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control
{

    /// <summary>
    ///     Canvas control. It should be the root parent for all other controls.
    /// </summary>
    public class Canvas : Base
    {

        private readonly List<IDisposable> mDisposeQueue; // dictionary for faster access?

        private Color mBackgroundColor;

        // [omeg] these are not created by us, so no disposing
        internal Base mFirstTab;

        private bool mNeedsRedraw;

        internal Base mNextTab;

        private float mScale;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Canvas" /> class.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        public Canvas(Skin.Base skin, string name) : base(null, name)
        {
            SetBounds(0, 0, 10000, 10000);
            SetSkin(skin);
            Scale = 1.0f;
            BackgroundColor = Color.White;
            ShouldDrawBackground = false;

            mDisposeQueue = new List<IDisposable>();
        }

        /// <summary>
        ///     Scale for rendering.
        /// </summary>
        public float Scale
        {
            get => mScale;
            set
            {
                if (mScale == value)
                {
                    return;
                }

                mScale = value;

                if (Skin != null && Skin.Renderer != null)
                {
                    Skin.Renderer.Scale = mScale;
                }

                OnScaleChanged();
                Redraw();
            }
        }

        /// <summary>
        ///     Background color.
        /// </summary>
        public Color BackgroundColor
        {
            get => mBackgroundColor;
            set => mBackgroundColor = value;
        }

        /// <summary>
        ///     In most situations you will be rendering the canvas every frame.
        ///     But in some situations you will only want to render when there have been changes.
        ///     You can do this by checking NeedsRedraw.
        /// </summary>
        public bool NeedsRedraw
        {
            get => mNeedsRedraw;
            set => mNeedsRedraw = value;
        }

        public override void Dispose()
        {
            ProcessDelayedDeletes();
            base.Dispose();
        }

        /// <summary>
        ///     Re-renders the control, invalidates cached texture.
        /// </summary>
        public override void Redraw()
        {
            NeedsRedraw = true;
            base.Redraw();
        }

        // Children call parent.GetCanvas() until they get to 
        // this top level function.
        public override Canvas GetCanvas()
        {
            return this;
        }

        /// <summary>
        ///     Additional initialization (which is sometimes not appropriate in the constructor)
        /// </summary>
        protected void Initialize()
        {
        }

        /// <summary>
        ///     Renders the canvas. Call in your rendering loop.
        /// </summary>
        public void RenderCanvas()
        {
            DoThink();

            var render = Skin.Renderer;

            render.Begin();

            RecurseLayout(Skin);

            render.ClipRegion = Bounds;

            //render.RenderOffset = new Point(X,Y);
            render.Scale = Scale;

            if (ShouldDrawBackground)
            {
                render.DrawColor = mBackgroundColor;
                render.DrawFilledRect(RenderBounds);
            }

            DoRender(Skin);

            DragAndDrop.RenderOverlay(this, Skin);

            Gwen.ToolTip.RenderToolTip(Skin);

            render.EndClip();

            render.End();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(Skin.Base skin)
        {
            //skin.Renderer.rnd = new Random(1);
            base.Render(skin);
            mNeedsRedraw = false;
        }

        /// <summary>
        ///     Handler invoked when control's bounds change.
        /// </summary>
        /// <param name="oldBounds">Old bounds.</param>
        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            base.OnBoundsChanged(oldBounds);
            InvalidateChildren(true);
        }

        /// <summary>
        ///     Processes input and layout. Also purges delayed delete queue.
        /// </summary>
        private void DoThink()
        {
            if (IsHidden)
            {
                return;
            }

            Animation.GlobalThink();

            // Reset tabbing
            mNextTab = null;
            mFirstTab = null;

            ProcessDelayedDeletes();

            // Check has focus etc..
            RecurseLayout(Skin);

            // If we didn't have a next tab, cycle to the start.
            if (mNextTab == null)
            {
                mNextTab = mFirstTab;
            }

            InputHandler.OnCanvasThink(this);
        }

        /// <summary>
        ///     Adds given control to the delete queue and detaches it from canvas. Don't call from Dispose, it modifies child
        ///     list.
        /// </summary>
        /// <param name="control">Control to delete.</param>
        public void AddDelayedDelete(Base control)
        {
            if (!mDisposeQueue.Contains(control))
            {
                mDisposeQueue.Add(control);
                RemoveChild(control, false);
            }
#if DEBUG
            else
            {
                throw new InvalidOperationException("Control deleted twice");
            }
#endif
        }

        private void ProcessDelayedDeletes()
        {
            //if (m_DisposeQueue.Count > 0)
            //    System.Diagnostics.//debug.print("Canvas.ProcessDelayedDeletes: {0} items", m_DisposeQueue.Count);
            foreach (var control in mDisposeQueue)
            {
                control.Dispose();
            }

            mDisposeQueue.Clear();
        }

        /// <summary>
        ///     Handles mouse movement events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_MouseMoved(int x, int y, int dx, int dy)
        {
            if (IsHidden)
            {
                return false;
            }

            // Todo: Handle scaling here..
            var fScale = 1.0f / Scale;
            x = (int) (x * fScale);
            y = (int) (y * fScale);
            dx = (int) (dx * fScale);
            dy = (int) (dy * fScale);

            InputHandler.OnMouseMoved(this, x, y, dx, dy);

            if (InputHandler.HoveredControl == null)
            {
                return false;
            }

            if (InputHandler.HoveredControl == this)
            {
                return false;
            }

            if (InputHandler.HoveredControl.GetCanvas() != this)
            {
                return false;
            }

            InputHandler.HoveredControl.InputMouseMoved(x, y, dx, dy);
            InputHandler.HoveredControl.UpdateCursor();

            DragAndDrop.OnMouseMoved(InputHandler.HoveredControl, x, y);

            return true;
        }

        /// <summary>
        ///     Handles mouse button events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_MouseButton(int button, bool down)
        {
            if (IsHidden)
            {
                return false;
            }

            return InputHandler.OnMouseClicked(this, button, down);
        }

        /// <summary>
        ///     Handles mouse button events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_MouseScroll(int deltaX, int deltaY)
        {
            if (IsHidden)
            {
                return false;
            }

            return InputHandler.OnMouseScroll(this, deltaX, deltaY);
        }

        /// <summary>
        ///     Handles keyboard events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_Key(Key key, bool down)
        {
            if (IsHidden)
            {
                return false;
            }

            if (key <= Key.Invalid)
            {
                return false;
            }

            if (key >= Key.Count)
            {
                return false;
            }

            return InputHandler.OnKeyEvent(this, key, down);
        }

        /// <summary>
        ///     Handles keyboard events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_Character(char chr)
        {
            if (IsHidden)
            {
                return false;
            }

            if (char.IsControl(chr))
            {
                return false;
            }

            //Handle Accelerators
            if (InputHandler.HandleAccelerator(this, chr))
            {
                return true;
            }

            //Handle characters
            if (InputHandler.KeyboardFocus == null)
            {
                return false;
            }

            if (InputHandler.KeyboardFocus.GetCanvas() != this)
            {
                return false;
            }

            if (!InputHandler.KeyboardFocus.IsVisible)
            {
                return false;
            }

            //if (InputHandler.IsControlDown) return false;

            return InputHandler.KeyboardFocus.InputChar(chr);
        }

        /// <summary>
        ///     Handles the mouse wheel events. Called from Input subsystems.
        /// </summary>
        /// <returns>True if handled.</returns>
        public bool Input_MouseWheel(int val)
        {
            if (IsHidden)
            {
                return false;
            }

            if (InputHandler.HoveredControl == null)
            {
                return false;
            }

            if (InputHandler.HoveredControl == this)
            {
                return false;
            }

            if (InputHandler.HoveredControl.GetCanvas() != this)
            {
                return false;
            }

            return InputHandler.HoveredControl.InputMouseWheeled(val);
        }

    }

}
