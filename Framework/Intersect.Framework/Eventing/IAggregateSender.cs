namespace Intersect.Framework.Eventing;

public interface IAggregateSender
{
    object CurrentSender { get; }

    object? OriginalSender { get; }
}