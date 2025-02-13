namespace Intersect.Client.Interface.Data;

public sealed class DelegateDataProvider<TValue> : DataProvider<TValue>
{
    private readonly TryProviderValueDelegate<TValue> _providerDelegate;

    public DelegateDataProvider(Func<TValue> simpleProvider) : this(CreateTryProviderValueDelegateFrom(simpleProvider))
    {
    }

    public DelegateDataProvider(Func<TValue> simpleProvider, int delayMilliseconds) : this(
        CreateTryProviderValueDelegateFrom(simpleProvider),
        TimeSpan.FromMilliseconds(delayMilliseconds)
    )
    {
    }

    public DelegateDataProvider(Func<TValue> simpleProvider, TimeSpan delay) : this(
        CreateTryProviderValueDelegateFrom(simpleProvider),
        delay
    )
    {
    }

    public DelegateDataProvider(TryProviderValueDelegate<TValue> providerDelegate)
    {
        _providerDelegate = providerDelegate;
    }

    public DelegateDataProvider(TryProviderValueDelegate<TValue> providerDelegate, TimeSpan delay)
    {
        _providerDelegate = providerDelegate;
        DelayProvider = new DelayProvider(delay);
    }

    public DelayProvider? DelayProvider { get; set; }

    public override bool TryUpdate(TimeSpan elapsed, TimeSpan total)
    {
        if (DelayProvider is { } delayProvider && !delayProvider.TryUpdate(elapsed, total))
        {
            return false;
        }

        return _providerDelegate(elapsed, total, out var value) && TrySetValue(value);
    }

    public static TryProviderValueDelegate<TValue> CreateTryProviderValueDelegateFrom(Func<TValue> simpleProvider)
    {
        return (TimeSpan _, TimeSpan _, out TValue value) =>
        {
            value = simpleProvider();
            return true;
        };
    }
}