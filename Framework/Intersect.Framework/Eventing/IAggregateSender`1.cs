namespace Intersect.Framework.Eventing;

public interface IAggregateSender<out T> : IAggregateSender where T : notnull
{
    new T CurrentSender { get; }

    object IAggregateSender.CurrentSender => CurrentSender;
}