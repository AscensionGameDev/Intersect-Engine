using System.Diagnostics.CodeAnalysis;

namespace Intersect.Framework.Threading;

public sealed partial class ThreadQueue
{
    public static readonly ThreadQueue Default = new();

    private readonly Queue<Action> _actionQueue = [];

    private readonly object _lock = new();
    private readonly Stack<ManualResetEventSlim> _resetEventPool = [];
    private readonly int? _spinCount;
    private readonly Action<Action> _statelessAction = action => action();

    private int _mainThreadId;

    public ThreadQueue(int? spinCount = null)
    {
        _spinCount = spinCount;

        SetMainThreadId();
    }

    public ThreadQueue(ThreadQueue parent)
    {
        Parent = parent;
        _spinCount = parent._spinCount;
        _mainThreadId = parent._mainThreadId;
    }

    public bool IsOnMainThread => Parent?.IsOnMainThread ?? _mainThreadId == Environment.CurrentManagedThreadId;

    public ThreadQueue? Parent { get; set; }

    public void Defer(Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (IsOnMainThread)
        {
            action();
            return;
        }

        Defer(_statelessAction, action);
    }

    public void Defer<TState>(Action<TState> action, TState state)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (IsOnMainThread)
        {
            action(state);
            return;
        }

        var resetEvent = ResetEventPoolPop();

        try
        {
            State<TState>.DeferredAction deferredAction = new(
                action,
                resetEvent,
                state
            );

            lock (_actionQueue)
            {
                State<TState>.ActionQueue.Enqueue(deferredAction);
                _actionQueue.Enqueue(State<TState>.Next);
            }

            resetEvent.Wait();
        }
        finally
        {
            ResetEventPoolPush(resetEvent);
        }
    }

    public void InvokePending()
    {
        ThrowIfNotOnMainThread();

        lock (_actionQueue)
        {
            while (_actionQueue.TryDequeue(out var deferredAction))
            {
                deferredAction();
            }
        }
    }

    private ManualResetEventSlim ResetEventPoolPop()
    {
        lock (_resetEventPool)
        {
            if (_resetEventPool.TryPop(out var resetEvent))
            {
                return resetEvent;
            }
        }

        return _spinCount.HasValue
            ? new ManualResetEventSlim(false, _spinCount.Value)
            : new ManualResetEventSlim();
    }

    private void ResetEventPoolPush(ManualResetEventSlim resetEvent)
    {
        resetEvent.Reset();
        lock (_resetEventPool)
        {
            _resetEventPool.Push(resetEvent);
        }
    }

    public void SetMainThreadId(int? mainThreadId = null)
    {
        lock (_lock)
        {
            _mainThreadId = mainThreadId ?? Environment.CurrentManagedThreadId;
        }
    }

    public void ThrowIfNotOnMainThread()
    {
        if (IsOnMainThread)
        {
            return;
        }

        throw new InvalidOperationException("Operation was not called on the main thread.");
    }
}