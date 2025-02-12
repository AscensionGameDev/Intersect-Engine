namespace Intersect.Client.Interface.Data;

public sealed class DelayProvider(TimeSpan delay) : DelayedDataProvider<bool>(delay)
{
    protected override bool TryDelayedUpdate(TimeSpan elapsed, TimeSpan total)
    {
        EmitValueChanged(true, true);
        return true;
    }
}