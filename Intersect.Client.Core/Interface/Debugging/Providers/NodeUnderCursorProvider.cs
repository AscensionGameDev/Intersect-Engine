using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Data;

namespace Intersect.Client.Interface.Debugging.Providers;

public sealed class NodeUnderCursorProvider(TimeSpan delay) : DelayedDataProvider<Base?>(delay)
{
    public NodeUnderCursorProvider() : this(MinimumDelay) { }

    public NodeFilter Filter { get; set; } = NodeFilter.IncludeMouseInputDisabled;

    protected override bool TryDelayedUpdate(TimeSpan elapsed, TimeSpan total)
    {
        var newValue = Interface.FindComponentUnderCursor(Filter);
        return TrySetValue(newValue);
    }
}