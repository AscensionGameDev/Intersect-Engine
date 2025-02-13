namespace Intersect.Client.Interface.Data;

public abstract partial class DelayedDataProvider<TValue> : DataProvider<TValue>
{
    public static readonly TimeSpan MinimumDelay = TimeSpan.FromMilliseconds(10);

    private TimeSpan _delay;
    private TimeSpan _nextUpdate;

    protected DelayedDataProvider() : this(MinimumDelay) { }

    protected DelayedDataProvider(TimeSpan delay)
    {
        if (delay < MinimumDelay)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), delay, $"Delay must be at least {MinimumDelay}");
        }

        _delay = delay;
    }

    public TimeSpan Delay
    {
        get => _delay;
        set
        {
            if (value == _delay)
            {
                return;
            }

            if (value < MinimumDelay)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Delay must be at least {MinimumDelay}");
            }

            _delay = value;
        }
    }

    public override bool TryUpdate(TimeSpan elapsed, TimeSpan total)
    {
        if (_nextUpdate > total)
        {
            return false;
        }

        var valueChanged = TryDelayedUpdate(elapsed, total);
        _nextUpdate = total + _delay;
        return valueChanged;
    }

    protected abstract bool TryDelayedUpdate(TimeSpan elapsed, TimeSpan total);
}