using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Interface.Data;

public interface IDataProvider
{
    event DataProviderEventHandler<ValueChangedEventArgs<object?>>? ValueChanged;

    object? Value { get; }
}