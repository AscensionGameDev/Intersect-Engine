using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen;


/// <summary>
///     Tooltip handling.
/// </summary>
public static partial class ToolTip
{
    private static Base? _activeTooltipParent;

    public static bool IsActiveTooltip(Base tooltipComponent)
    {
        if (tooltipComponent.Parent is not { } parent)
        {
            return false;
        }

        return parent == _activeTooltipParent;
    }

    /// <summary>
    ///     Enables tooltip display for the specified control.
    /// </summary>
    /// <param name="control">Target control.</param>
    public static void Enable(Base control)
    {
        if (null == control.Tooltip)
        {
            return;
        }

        _activeTooltipParent = control;
    }

    /// <summary>
    ///     Disables tooltip display for the specified control.
    /// </summary>
    /// <param name="control">Target control.</param>
    public static void Disable(Base control)
    {
        if (_activeTooltipParent != control)
        {
            return;
        }

        _activeTooltipParent = null;
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
        if (_activeTooltipParent?.Tooltip == default)
        {
            return;
        }

        var canvas = _activeTooltipParent.Canvas;
        if (canvas == default)
        {
            return;
        }

        var render = skin.Renderer;

        var oldRenderOffset = render.RenderOffset;
        var mousePos = Input.InputHandler.MousePosition;
        var bounds = _activeTooltipParent.Tooltip.Bounds;

        var offset = Util.FloatRect(
            mousePos.X - bounds.Width * 0.5f,
            mousePos.Y - bounds.Height,
            bounds.Width,
            bounds.Height
        );

        offset = Util.ClampRectToRect(offset, canvas.Bounds);

        //Calculate offset on screen bounds
        render.AddRenderOffset(offset);
        render.EndClip();

        skin.DrawToolTip(_activeTooltipParent.Tooltip);
        _activeTooltipParent.Tooltip.DoRender(skin);

        render.RenderOffset = oldRenderOffset;
    }

}
