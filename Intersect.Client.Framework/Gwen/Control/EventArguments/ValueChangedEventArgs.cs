namespace Intersect.Client.Framework.Gwen.Control.EventArguments;

public class ValueChangedEventArgs<TValue> : EventArgs
{
    public ValueChangedEventArgs() { }

    public required TValue Value { get; init; }
}