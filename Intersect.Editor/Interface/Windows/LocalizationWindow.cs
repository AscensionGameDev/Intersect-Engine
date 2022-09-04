using Intersect.Client.Framework.UserInterface.Components;
using Intersect.Editor.Localization;
using Intersect.Time;

namespace Intersect.Editor.Interface.Windows;

internal partial class LocalizationWindow : Window
{
    public LocalizationWindow() : base(nameof(LocalizationWindow))
    {
        SizeConstraintMinimum = new(600, 400);
        Title = Strings.Windows.Localization.Title;
    }

    protected override bool LayoutBegin(FrameTime frameTime)
    {
        if (!base.LayoutBegin(frameTime))
        {
            return false;
        }

        if (!LayoutTable(frameTime))
        {
            return false;
        }

        return true;
    }
}
