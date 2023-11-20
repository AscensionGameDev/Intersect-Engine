namespace Intersect.Framework.Eventing;

public sealed record AggregateSender : IAggregateSender
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public AggregateSender(object currentSender, object? originalSender)
    {
        CurrentSender = currentSender;
        OriginalSender = originalSender;
    }

    /// <inheritdoc />
    public object CurrentSender { get; }

    /// <inheritdoc />
    public object? OriginalSender { get; }
}