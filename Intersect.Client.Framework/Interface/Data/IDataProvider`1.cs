using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Interface.Data;

public interface IDataProvider<TValue> : IDataProvider
{
    new event DataProviderEventHandler<ValueChangedEventArgs<TValue>>? ValueChanged;

    object? IDataProvider.Value => Value;

    new TValue Value { get; }
}