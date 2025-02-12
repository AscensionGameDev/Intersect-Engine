namespace Intersect.Client.Framework.Gwen.Control;

[Flags]
public enum NodeFilter
{
    None,

    IncludeHidden = 1,
    IncludeMouseInputDisabled = 2,
    IncludeText = 4,
}