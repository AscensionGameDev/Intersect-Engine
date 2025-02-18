namespace Intersect.Framework.Threading;

public interface IManualActionQueueParent
{
    bool IsExecuting { get; }
}

public sealed class ManualActionQueueParent : IManualActionQueueParent
{
    public bool IsExecuting { get; set; }
}

public sealed class ManualActionQueue(IManualActionQueueParent? parent = null) : ActionQueue<ManualActionQueue, bool>
{
    private readonly object _lock = new();

    private bool _active;

    // Can't actually convert it because we can't add a private setter here
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    protected override bool IsActive => _active;

    protected override Action<bool> PostInvocationAction => static _ => { };

    protected override void BeginInvokePending()
    {
        if (parent is { IsExecuting: false } || !Monitor.TryEnter(_lock))
        {
            throw new InvalidOperationException("Tried to invoke pending actions from an invalid source");
        }

        _active = true;
    }

    protected override void EndInvokePending()
    {
        _active = false;

        Monitor.Exit(_lock);
    }

    protected override bool EnqueueCreateState()
    {
        var lockTaken = false;
        Monitor.Enter(_lock, ref lockTaken);
        return lockTaken;
    }

    protected override void EnqueueSuccessful(bool lockTaken)
    {
    }

    protected override void EnqueueFinally(bool lockTaken)
    {
        if (lockTaken)
        {
            Monitor.Exit(_lock);
        }
    }
}