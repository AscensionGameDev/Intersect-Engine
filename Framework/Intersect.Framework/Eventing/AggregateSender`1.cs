namespace Intersect.Framework.Eventing;

public sealed class AggregateSender<T> : IAggregateSender<T> where T : notnull
{
    public AggregateSender(T currentSender, object? originalSender)
    {
        CurrentSender = currentSender;
        OriginalSender = originalSender;
    }

    /// <inheritdoc />
    public T CurrentSender { get; }

    /// <inheritdoc />
    public object? OriginalSender { get; }
}