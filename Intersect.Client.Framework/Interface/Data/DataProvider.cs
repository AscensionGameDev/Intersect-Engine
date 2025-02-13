using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Data;

public abstract partial class DataProvider<TValue> : IDataProvider<TValue>, IUpdatableDataProvider
{
    public event DataProviderEventHandler<ValueChangedEventArgs<TValue>>? ValueChanged;

    public TValue Value { get; private set; } = default!;

    protected bool TrySetValue(TValue value)
    {
        var oldValue = Value;
        if (value?.Equals(oldValue) ?? oldValue?.Equals(value) ?? true)
        {
            return false;
        }

        Value = value;

        EmitValueChanged(value, oldValue);

        return true;
    }

    protected void EmitValueChanged(TValue value, TValue oldValue)
    {
        try
        {
            ValueChanged?.Invoke(
                this,
                new ValueChangedEventArgs<TValue>
                {
                    Value = value, OldValue = oldValue,
                }
            );
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in ValueChanged event handler"
            );
        }
    }

    public abstract bool TryUpdate(TimeSpan elapsed, TimeSpan total);
}