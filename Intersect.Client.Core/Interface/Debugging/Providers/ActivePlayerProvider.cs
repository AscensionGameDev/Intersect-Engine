using Intersect.Client.Entities;
using Intersect.Client.General;
using Intersect.Client.Interface.Data;

namespace Intersect.Client.Interface.Debugging.Providers;

public sealed class ActivePlayerProvider(TimeSpan delay) : DelayedDataProvider<Player?>(delay)
{
    public ActivePlayerProvider() : this(MinimumDelay) { }

    protected override bool TryDelayedUpdate(TimeSpan elapsed, TimeSpan total)
    {
        var newValue = Globals.Me;
        return TrySetValue(newValue);
    }
}