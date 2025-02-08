namespace Intersect.Client.Framework.Gwen.Control;

public interface ITextContainer
{
    string? Text { get; set; }

    Padding TextPadding { get; set; }

    Color? TextPaddingDebugColor { get; set; }
}