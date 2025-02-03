using Intersect.Async;

namespace Intersect.Client.Framework.Gwen.Control.Data;

public partial class ValueTableCellDataProvider<TValue> : ITableCellDataProvider
{
    public ValueTableCellDataProvider(
        Func<TValue> provideValue,
        int delay = 100,
        Func<Task<bool>>? waitPredicate = default
    ) : this(
        _ => provideValue(),
        delay: delay,
        waitPredicate: waitPredicate
    )
    {
    }

    public ValueTableCellDataProvider(
        Func<CancellationToken, TValue> provideValue,
        int delay = 100,
        Func<Task<bool>>? waitPredicate = default
    )
    {
        _delay = delay;
        _providerValue = provideValue;
        _waitPredicate = waitPredicate;
        Generator = new CancellableGenerator<TValue>(CreateValueGenerator);
    }

    private AsyncValueGenerator<TValue> CreateValueGenerator(CancellationToken cancellationToken)
    {
        return new AsyncValueGenerator<TValue>(
            () => WaitAndProvideValue(cancellationToken),
            value => DataChanged?.Invoke(this, new CellDataChangedEventArgs(default, value)),
            cancellationToken
        );
    }

    private async Task<TValue> WaitAndProvideValue(CancellationToken cancellationToken)
    {
        if (_waitPredicate != null)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_delay, cancellationToken);

                if (await _waitPredicate())
                {
                    break;
                }
            }
        }
        else
        {
            await Task.Delay(_delay, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        return _providerValue(cancellationToken);
    }

    public event TableCellDataChangedEventHandler? DataChanged;

    private readonly int _delay;
    private readonly Func<CancellationToken, TValue> _providerValue;
    private readonly Func<Task<bool>>? _waitPredicate;

    public CancellableGenerator<TValue> Generator { get; }
}
