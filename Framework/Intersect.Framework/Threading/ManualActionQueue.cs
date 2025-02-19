namespace Intersect.Framework.Threading;

public interface IManualActionQueueParent
{
    bool IsExecuting { get; }
}

public sealed class ManualActionQueueParent : IManualActionQueueParent
{
    public bool IsExecuting { get; set; }
}

public sealed class ManualActionQueue : ActionQueue<ManualActionQueue, bool>
{
    private readonly object _lock = new();

    private bool _active;
    private readonly IManualActionQueueParent? _parent;

    public ManualActionQueue(IManualActionQueueParent? parent = null) : base(
        beginInvokePending: BeginInvokePending,
        endInvokePending: EndInvokePending
    )
    {
        _parent = parent;
    }

    // Can't actually convert it because we can't add a private setter here
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    protected override bool IsActive => _active;

    protected override Action<bool> PostInvocationAction => static _ => { };

    private static void BeginInvokePending(ManualActionQueue @this)
    {
        if (@this._parent is { IsExecuting: false } || !Monitor.TryEnter(@this._lock))
        {
            throw new InvalidOperationException("Tried to invoke pending actions from an invalid source");
        }

        @this._active = true;
    }

    private static void EndInvokePending(ManualActionQueue @this)
    {
        @this._active = false;

        Monitor.Exit(@this._lock);
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