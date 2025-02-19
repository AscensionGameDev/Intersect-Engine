namespace Intersect.Framework.Threading;

public abstract partial class ActionQueue<TActionQueue, TEnqueueState> where TActionQueue : ActionQueue<TActionQueue, TEnqueueState>
{
    private readonly TActionQueue @this;
    private readonly Queue<Action> _actionQueue = [];
    private readonly Action<Action> _statelessAction = action => action();
    private readonly Action<TActionQueue>? _beginInvokePending;
    private readonly Action<TActionQueue>? _endInvokePending;

    private bool _empty;

    protected ActionQueue(Action<TActionQueue>? beginInvokePending, Action<TActionQueue>? endInvokePending)
    {
        @this = this as TActionQueue ?? throw new InvalidCastException();
        _beginInvokePending = beginInvokePending;
        _endInvokePending = endInvokePending;
    }

    public event Action? QueueNotEmpty;

    protected abstract bool IsActive { get; }

    protected abstract Action<TEnqueueState> PostInvocationAction { get; }

    /// <summary>
    ///
    /// </summary>
    /// <returns>Returns true if the queue was non-empty and is now empty.</returns>
    public bool InvokePending()
    {
        if (_empty)
        {
            return false;
        }

        _beginInvokePending?.Invoke(@this);

        _empty = true;

        lock (_actionQueue)
        {
            while (_actionQueue.TryDequeue(out var deferredAction))
            {
                deferredAction();
            }
        }

        _endInvokePending?.Invoke(@this);
        return _actionQueue.Count < 1;
    }

    public void Enqueue(Action action)
    {
        if (IsActive)
        {
            action();
            return;
        }

        Enqueue(_statelessAction, action);
    }

    public void Enqueue<TState>(Action<TState> action, TState state)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (IsActive)
        {
            action(state);
            return;
        }

        var enqueueState = EnqueueCreateState();

        try
        {
            State<TState>.DeferredAction deferredAction = new(
                action,
                state,
                PostInvocationAction,
                enqueueState
            );

            lock (_actionQueue)
            {
                State<TState>.ActionQueue.Enqueue(deferredAction);
                _actionQueue.Enqueue(State<TState>.Next);
                try
                {
                    if (_empty)
                    {
                        QueueNotEmpty?.Invoke();
                    }
                }
                catch
                {
                    // Ignore, application context not available here
                }
                _empty = false;
            }

            EnqueueSuccessful(enqueueState);
        }
        finally
        {
            EnqueueFinally(enqueueState);
        }
    }

    protected abstract TEnqueueState EnqueueCreateState();

    protected abstract void EnqueueSuccessful(TEnqueueState enqueueState);

    protected abstract void EnqueueFinally(TEnqueueState enqueueState);
}