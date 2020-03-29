using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal
{

    /// <summary>
    ///     Grab point for resizing.
    /// </summary>
    public class Resizer : Dragger
    {

        private Pos mResizeDir;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Resizer" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Resizer(Base parent) : base(parent)
        {
            mResizeDir = Pos.Left;
            MouseInputEnabled = true;
            SetSize(6, 6);
            Target = parent;
        }

        /// <summary>
        ///     Gets or sets the sizing direction.
        /// </summary>
        public Pos ResizeDir
        {
            set
            {
                mResizeDir = value;

                if (0 != (value & Pos.Left) && 0 != (value & Pos.Top) ||
                    0 != (value & Pos.Right) && 0 != (value & Pos.Bottom))
                {
                    Cursor = Cursors.SizeNwse;

                    return;
                }

                if (0 != (value & Pos.Right) && 0 != (value & Pos.Top) ||
                    0 != (value & Pos.Left) && 0 != (value & Pos.Bottom))
                {
                    Cursor = Cursors.SizeNesw;

                    return;
                }

                if (0 != (value & Pos.Right) || 0 != (value & Pos.Left))
                {
                    Cursor = Cursors.SizeWe;

                    return;
                }

                if (0 != (value & Pos.Top) || 0 != (value & Pos.Bottom))
                {
                    Cursor = Cursors.SizeNs;

                    return;
                }
            }
        }

        /// <summary>
        ///     Invoked when the control has been resized.
        /// </summary>
        public event GwenEventHandler<EventArgs> Resized;

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(int x, int y, int dx, int dy)
        {
            if (null == mTarget)
            {
                return;
            }

            if (!mHeld)
            {
                return;
            }

            var oldBounds = mTarget.Bounds;
            var bounds = mTarget.Bounds;

            var min = mTarget.MinimumSize;

            var pCursorPos = mTarget.CanvasPosToLocal(new Point(x, y));

            var delta = mTarget.LocalPosToCanvas(mHoldPos);
            delta.X -= x;
            delta.Y -= y;

            if (0 != (mResizeDir & Pos.Left))
            {
                bounds.X -= delta.X;
                bounds.Width += delta.X;

                // Conform to minimum size here so we don't
                // go all weird when we snap it in the base conrt

                if (bounds.Width < min.X)
                {
                    var diff = min.X - bounds.Width;
                    bounds.Width += diff;
                    bounds.X -= diff;
                }
            }

            if (0 != (mResizeDir & Pos.Top))
            {
                bounds.Y -= delta.Y;
                bounds.Height += delta.Y;

                // Conform to minimum size here so we don't
                // go all weird when we snap it in the base conrt

                if (bounds.Height < min.Y)
                {
                    var diff = min.Y - bounds.Height;
                    bounds.Height += diff;
                    bounds.Y -= diff;
                }
            }

            if (0 != (mResizeDir & Pos.Right))
            {
                // This is complicated.
                // Basically we want to use the HoldPos, so it doesn't snap to the edge of the control
                // But we need to move the HoldPos with the window movement. Yikes.
                // I actually think this might be a big hack around the way this control works with regards
                // to the holdpos being on the parent panel.

                var woff = bounds.Width - mHoldPos.X;
                var diff = bounds.Width;
                bounds.Width = pCursorPos.X + woff;
                if (bounds.Width < min.X)
                {
                    bounds.Width = min.X;
                }

                diff -= bounds.Width;

                mHoldPos.X -= diff;
            }

            if (0 != (mResizeDir & Pos.Bottom))
            {
                var hoff = bounds.Height - mHoldPos.Y;
                var diff = bounds.Height;
                bounds.Height = pCursorPos.Y + hoff;
                if (bounds.Height < min.Y)
                {
                    bounds.Height = min.Y;
                }

                diff -= bounds.Height;

                mHoldPos.Y -= diff;
            }

            mTarget.SetBounds(bounds);

            if (Resized != null)
            {
                Resized.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
