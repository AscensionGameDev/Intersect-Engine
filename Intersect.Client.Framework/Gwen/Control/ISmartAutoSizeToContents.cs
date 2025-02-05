namespace Intersect.Client.Framework.Gwen.Control;

public interface ISmartAutoSizeToContents : IAutoSizeToContents
{
    bool IAutoSizeToContents.AutoSizeToContents
    {
        get => AutoSizeToContentWidth || AutoSizeToContentHeight;
        set
        {
            AutoSizeToContentWidth = value;
            AutoSizeToContentHeight = value;
        }
    }

    bool AutoSizeToContentWidth { get; set; }

    bool AutoSizeToContentHeight { get; set; }

    bool AutoSizeToContentWidthOnChildResize { get; set; }

    bool AutoSizeToContentHeightOnChildResize { get; set; }
}