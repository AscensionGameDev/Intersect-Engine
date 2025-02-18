namespace Intersect.Framework.Threading;

public abstract partial class ActionQueue<TActionQueue, TEnqueueState> where TActionQueue : ActionQueue<TActionQueue, TEnqueueState>
{
    private readonly Queue<Action> _actionQueue = [];
    private readonly Action<Action> _statelessAction = action => action();

    protected abstract bool IsActive { get; }

    protected abstract Action<TEnqueueState> PostInvocationAction { get; }

    public void InvokePending()
    {
        BeginInvokePending();

        lock (_actionQueue)
        {
            while (_actionQueue.TryDequeue(out var deferredAction))
            {
                deferredAction();
            }
        }

        EndInvokePending();
    }

    protected abstract void BeginInvokePending();

    protected abstract void EndInvokePending();

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