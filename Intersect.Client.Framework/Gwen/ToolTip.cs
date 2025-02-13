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
        var activeTooltipParent = _activeTooltipParent;
        if (activeTooltipParent?.Tooltip == default)
        {
            return;
        }

        var canvas = activeTooltipParent.Canvas;
        if (canvas == default)
        {
            return;
        }

        var render = skin.Renderer;

        var oldRenderOffset = render.RenderOffset;
        var mousePos = Input.InputHandler.MousePosition;
        var tooltipBounds = activeTooltipParent.Tooltip.Bounds;

        var offset = Util.FloatRect(
            mousePos.X - tooltipBounds.Width * 0.5f,
            mousePos.Y - tooltipBounds.Height,
            tooltipBounds.Width,
            tooltipBounds.Height
        );

        var textContainerMatch = activeTooltipParent.FindMatchingNodes<ITextContainer>();

        if (textContainerMatch is { Highest: { } highest, Closest: { } closest })
        {
            var closestBounds = closest.GlobalBounds;
            var highestBounds = highest.GlobalBounds;
            if (highestBounds.Top > tooltipBounds.Height)
            {
                offset.Y = highestBounds.Top - tooltipBounds.Height;
            }
            else
            {
                // TODO: check if we have enough space below
                offset.Y = highestBounds.Bottom;
            }

            offset.X = highestBounds.X + (highestBounds.Width - tooltipBounds.Width) / 2;
            var closestBoundsX = closestBounds.X + closestBounds.Width * 0.25f;
            if (offset.X > closestBoundsX)
            {
                offset.X = closestBounds.X + (closestBounds.Width - tooltipBounds.Width) / 2;
            }
        }

        var clampedOffset = Util.ClampRectToRect(offset, canvas.Bounds);

        //Calculate offset on screen bounds
        render.AddRenderOffset(clampedOffset);
        render.EndClip();

        skin.DrawToolTip(activeTooltipParent.Tooltip);
        activeTooltipParent.Tooltip.DoRender(skin);

        render.RenderOffset = oldRenderOffset;
    }

}
