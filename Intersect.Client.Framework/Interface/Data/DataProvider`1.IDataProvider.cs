using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Interface.Data;

public abstract partial class DataProvider<TValue>
{
    private readonly
        Dictionary<DataProviderEventHandler<ValueChangedEventArgs<object?>>,
            DataProviderEventHandler<ValueChangedEventArgs<TValue>>> _wrappedDelegates = [];

    event DataProviderEventHandler<ValueChangedEventArgs<object?>>? IDataProvider.ValueChanged
    {
        add
        {
            if (value == null)
            {
                return;
            }

            if (_wrappedDelegates.TryGetValue(value, out var wrappedDelegate))
            {
                return;
            }

            wrappedDelegate = (sender, args) => value?.Invoke(
                sender,
                new ValueChangedEventArgs<object?>
                {
                    Value = args.Value, OldValue = args.OldValue,
                }
            );

            ValueChanged += wrappedDelegate;
            _wrappedDelegates[value] = wrappedDelegate;
        }

        remove
        {
            if (value == null)
            {
                return;
            }

            if (!_wrappedDelegates.Remove(value, out var wrappedDelegate))
            {
                return;
            }

            ValueChanged -= wrappedDelegate;
        }
    }
}