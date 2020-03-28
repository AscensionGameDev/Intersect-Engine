using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.DragDrop
{

    /// <summary>
    ///     Drag and drop handling.
    /// </summary>
    public static class DragAndDrop
    {

        public static Package CurrentPackage;

        public static Base HoveredControl;

        private static Base sLastPressedControl;

        private static Point sLastPressedPos;

        private static int sMouseX;

        private static int sMouseY;

        private static Base sNewHoveredControl;

        public static Base SourceControl;

        private static bool OnDrop(int x, int y)
        {
            var success = false;

            if (HoveredControl != null)
            {
                HoveredControl.DragAndDrop_HoverLeave(CurrentPackage);
                success = HoveredControl.DragAndDrop_HandleDrop(CurrentPackage, x, y);
            }

            // Report back to the source control, to tell it if we've been successful.
            SourceControl.DragAndDrop_EndDragging(success, x, y);

            CurrentPackage = null;
            SourceControl = null;

            return true;
        }

        private static bool ShouldStartDraggingControl(int x, int y)
        {
            // We're not holding a control down..
            if (sLastPressedControl == null)
            {
                return false;
            }

            // Not been dragged far enough
            var length = Math.Abs(x - sLastPressedPos.X) + Math.Abs(y - sLastPressedPos.Y);
            if (length < 5)
            {
                return false;
            }

            // Create the dragging package

            CurrentPackage = sLastPressedControl.DragAndDrop_GetPackage(sLastPressedPos.X, sLastPressedPos.Y);

            // We didn't create a package!
            if (CurrentPackage == null)
            {
                sLastPressedControl = null;
                SourceControl = null;

                return false;
            }

            // Now we're dragging something!
            SourceControl = sLastPressedControl;
            InputHandler.MouseFocus = null;
            sLastPressedControl = null;
            CurrentPackage.DrawControl = null;

            // Some controls will want to decide whether they should be dragged at that moment.
            // This function is for them (it defaults to true)
            if (!SourceControl.DragAndDrop_ShouldStartDrag())
            {
                SourceControl = null;
                CurrentPackage = null;

                return false;
            }

            SourceControl.DragAndDrop_StartDragging(CurrentPackage, sLastPressedPos.X, sLastPressedPos.Y);

            return true;
        }

        private static void UpdateHoveredControl(Base control, int x, int y)
        {
            //
            // We use this global variable to represent our hovered control
            // That way, if the new hovered control gets deleted in one of the
            // Hover callbacks, we won't be left with a hanging pointer.
            // This isn't ideal - but it's minimal.
            //
            sNewHoveredControl = control;

            // Nothing to change..
            if (HoveredControl == sNewHoveredControl)
            {
                return;
            }

            // We changed - tell the old hovered control that it's no longer hovered.
            if (HoveredControl != null && HoveredControl != sNewHoveredControl)
            {
                HoveredControl.DragAndDrop_HoverLeave(CurrentPackage);
            }

            // If we're hovering where the control came from, just forget it.
            // By changing it to null here we're not going to show any error cursors
            // it will just do nothing if you drop it.
            if (sNewHoveredControl == SourceControl)
            {
                sNewHoveredControl = null;
            }

            // Check to see if the new potential control can accept this type of package.
            // If not, ignore it and show an error cursor.
            while (sNewHoveredControl != null && !sNewHoveredControl.DragAndDrop_CanAcceptPackage(CurrentPackage))
            {
                // We can't drop on this control, so lets try to drop
                // onto its parent..
                sNewHoveredControl = sNewHoveredControl.Parent;

                // Its parents are dead. We can't drop it here.
                // Show the NO WAY cursor.
                if (sNewHoveredControl == null)
                {
                    Platform.Neutral.SetCursor(Cursors.No);
                }
            }

            // Become out new hovered control
            HoveredControl = sNewHoveredControl;

            // If we exist, tell us that we've started hovering.
            if (HoveredControl != null)
            {
                HoveredControl.DragAndDrop_HoverEnter(CurrentPackage, x, y);
            }

            sNewHoveredControl = null;
        }

        public static bool Start(Base control, Package package)
        {
            if (CurrentPackage != null)
            {
                return false;
            }

            CurrentPackage = package;
            SourceControl = control;

            return true;
        }

        public static bool OnMouseButton(Base hoveredControl, int x, int y, bool down)
        {
            if (!down)
            {
                sLastPressedControl = null;

                // Not carrying anything, allow normal actions
                if (CurrentPackage == null)
                {
                    return false;
                }

                // We were carrying something, drop it.
                OnDrop(x, y);

                return true;
            }

            if (hoveredControl == null)
            {
                return false;
            }

            if (!hoveredControl.DragAndDrop_Draggable())
            {
                return false;
            }

            // Store the last clicked on control. Don't do anything yet, 
            // we'll check it in OnMouseMoved, and if it moves further than
            // x pixels with the mouse down, we'll start to drag.
            sLastPressedPos = new Point(x, y);
            sLastPressedControl = hoveredControl;

            return false;
        }

        public static void OnMouseMoved(Base hoveredControl, int x, int y)
        {
            // Always keep these up to date, they're used to draw the dragged control.
            sMouseX = x;
            sMouseY = y;

            // If we're not carrying anything, then check to see if we should
            // pick up from a control that we're holding down. If not, then forget it.
            if (CurrentPackage == null && !ShouldStartDraggingControl(x, y))
            {
                return;
            }

            // Swap to this new hovered control and notify them of the change.
            UpdateHoveredControl(hoveredControl, x, y);

            if (HoveredControl == null)
            {
                return;
            }

            // Update the hovered control every mouse move, so it can show where
            // the dropped control will land etc..
            HoveredControl.DragAndDrop_Hover(CurrentPackage, x, y);

            // Override the cursor - since it might have been set my underlying controls
            // Ideally this would show the 'being dragged' control. TODO
            Platform.Neutral.SetCursor(Cursors.Default);

            hoveredControl.Redraw();
        }

        public static void RenderOverlay(Canvas canvas, Skin.Base skin)
        {
            if (CurrentPackage == null)
            {
                return;
            }

            if (CurrentPackage.DrawControl == null)
            {
                return;
            }

            var old = skin.Renderer.RenderOffset;

            skin.Renderer.AddRenderOffset(
                new Rectangle(
                    sMouseX - SourceControl.X - CurrentPackage.HoldOffset.X,
                    sMouseY - SourceControl.Y - CurrentPackage.HoldOffset.Y, 0, 0
                )
            );

            CurrentPackage.DrawControl.DoRender(skin);

            skin.Renderer.RenderOffset = old;
        }

        public static void ControlDeleted(Base control)
        {
            if (SourceControl == control)
            {
                SourceControl = null;
                CurrentPackage = null;
                HoveredControl = null;
                sLastPressedControl = null;
            }

            if (sLastPressedControl == control)
            {
                sLastPressedControl = null;
            }

            if (HoveredControl == control)
            {
                HoveredControl = null;
            }

            if (sNewHoveredControl == control)
            {
                sNewHoveredControl = null;
            }
        }

    }

}
