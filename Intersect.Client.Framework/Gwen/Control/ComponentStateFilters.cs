namespace Intersect.Client.Framework.Gwen.Control;

[Flags]
public enum ComponentStateFilters
{
    None,

    IncludeHidden = 1,
    IncludeMouseInputDisabled = 2,
    IncludeText = 4,
}