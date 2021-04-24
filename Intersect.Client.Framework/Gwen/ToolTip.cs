using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen
{

    /// <summary>
    ///     Tooltip handling.
    /// </summary>
    public static class ToolTip
    {

        private static Base sG_toolTip;

        /// <summary>
        ///     Enables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Enable(Base control)
        {
            if (null == control.ToolTip)
            {
                return;
            }

            sG_toolTip = control;
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Disable(Base control)
        {
            if (sG_toolTip == control)
            {
                sG_toolTip = null;
            }
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void ControlDeleted(Base control)
        {
            Disable(control);
        }

        /// <summary>
        ///     Renders the currently visible tooltip.
        /// </summary>
        /// <param name="skin"></param>
        public static void RenderToolTip(Skin.Base skin)
        {
            if (sG_toolTip == null || sG_toolTip.ToolTip == null)
            {
                return;
            }

            var render = skin.Renderer;

            var oldRenderOffset = render.RenderOffset;
            var mousePos = Input.InputHandler.MousePosition;
            var bounds = sG_toolTip.ToolTip.Bounds;

            var offset = Util.FloatRect(
                mousePos.X - bounds.Width * 0.5f, mousePos.Y - bounds.Height - 10, bounds.Width, bounds.Height
            );

            offset = Util.ClampRectToRect(offset, sG_toolTip.GetCanvas().Bounds);

            //Calculate offset on screen bounds
            render.AddRenderOffset(offset);
            render.EndClip();

            skin.DrawToolTip(sG_toolTip.ToolTip);
            sG_toolTip.ToolTip.DoRender(skin);

            render.RenderOffset = oldRenderOffset;
        }

    }

}
