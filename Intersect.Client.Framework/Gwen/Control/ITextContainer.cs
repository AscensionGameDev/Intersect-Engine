namespace Intersect.Client.Framework.Gwen.Control;

public interface ITextContainer
{
    Padding Padding { get; set; }

    string? Text { get; set; }

    Color? TextPaddingDebugColor { get; set; }
}
