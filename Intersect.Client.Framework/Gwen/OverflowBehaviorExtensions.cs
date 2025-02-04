using Intersect.Framework;

namespace Intersect.Client.Framework.Gwen;

public static class OverflowBehaviorExtensions
{
    public static bool AllowsScrolling(this OverflowBehavior overflowBehavior) => overflowBehavior switch
    {
        OverflowBehavior.Auto => true,
        OverflowBehavior.Visible => false,
        OverflowBehavior.Hidden => false,
        OverflowBehavior.Scroll => true,
        _ => throw Exceptions.UnreachableInvalidEnum(overflowBehavior),
    };

    public static OverflowBehavior SetAllowScrolling(
        this OverflowBehavior overflowBehavior,
        bool allowScrolling = true
    ) => overflowBehavior switch
    {
        OverflowBehavior.Auto => allowScrolling ? OverflowBehavior.Auto : OverflowBehavior.Hidden,
        OverflowBehavior.Visible => allowScrolling ? OverflowBehavior.Scroll : OverflowBehavior.Visible,
        OverflowBehavior.Hidden => allowScrolling ? OverflowBehavior.Scroll : OverflowBehavior.Hidden,
        OverflowBehavior.Scroll => allowScrolling ? OverflowBehavior.Scroll : OverflowBehavior.Hidden,
        _ => throw Exceptions.UnreachableInvalidEnum(overflowBehavior),
    };

    public static OverflowBehavior SetAutoHide(this OverflowBehavior overflowBehavior, bool autoHide = true) =>
        overflowBehavior switch
        {
            OverflowBehavior.Auto => autoHide ? OverflowBehavior.Auto : OverflowBehavior.Scroll,
            OverflowBehavior.Visible => OverflowBehavior.Visible,
            OverflowBehavior.Hidden => OverflowBehavior.Hidden,
            OverflowBehavior.Scroll => autoHide ? OverflowBehavior.Auto : OverflowBehavior.Scroll,
            _ => throw Exceptions.UnreachableInvalidEnum(overflowBehavior),
        };
}