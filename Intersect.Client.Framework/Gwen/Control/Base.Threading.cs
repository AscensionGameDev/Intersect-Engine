using Intersect.Core;
using Intersect.Framework.Threading;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;

public partial class Base
{
    private readonly ThreadQueue _threadQueue;
    private readonly ManualActionQueueParent _preLayoutActionsParent = new();
    public readonly ManualActionQueue PreLayout;
    private readonly ManualActionQueueParent _postLayoutActionsParent = new();
    public readonly ManualActionQueue PostLayout;

    private int _pendingThreadQueues;

    private void UpdatePendingThreadQueues(int increment)
    {
        _pendingThreadQueues = Math.Max(0, _pendingThreadQueues + increment);
        Parent?.UpdatePendingThreadQueues(increment);
    }

    public void RunOnMainThread(Action action)
    {
        Invalidate();
        _threadQueue.RunOnMainThread(action);
    }

    public void RunOnMainThread(Action<Base> action) => RunOnMainThread(action, this);

    public void RunOnMainThread<TState>(Action<TState> action, TState state)
    {
        Invalidate();
        _threadQueue.RunOnMainThread(action, state);
    }

    public TReturn RunOnMainThread<TReturn>(Func<Base, TReturn> func)
    {
        Invalidate();
        return _threadQueue.RunOnMainThread(func, this);
    }

    public void RunOnMainThread<TState>(Action<Base, TState> action, TState state)
    {
        Invalidate();
        _threadQueue.RunOnMainThread(action, this, state);
    }

    public TReturn RunOnMainThread<TState, TReturn>(
        Func<Base, TState, TReturn> func,
        TState state,
        bool invalidate = true
    )
    {
        if (invalidate)
        {
            Invalidate();
        }

        return _threadQueue.RunOnMainThread(func, this, state);
    }

    public void RunOnMainThread<TState0, TState1>(Action<TState0, TState1> action, TState0 state0, TState1 state1)
    {
        if (_disposed)
        {
            if (_disposeCompleted)
            {
                // TODO: Turn this into a LogError() after we see some reports
                ObjectDisposedException.ThrowIf(true, this);
            }

            ApplicationContext.CurrentContext.Logger.LogTrace(
                "RunOnMainThread() called after {NodeName} was disposed",
                CanonicalName
            );
            return;
        }

        Invalidate();
        _threadQueue.RunOnMainThread(action, state0, state1);
    }

    public void RunOnMainThread<TState0, TState1>(Action<Base, TState0, TState1> action, TState0 state0, TState1 state1)
    {
        Invalidate();
        _threadQueue.RunOnMainThread(action, this, state0, state1);
    }

    public void RunOnMainThread<TState0, TState1, TState2>(
        Action<TState0, TState1, TState2> action,
        TState0 state0,
        TState1 state1,
        TState2 state2
    )
    {
        Invalidate();
        _threadQueue.RunOnMainThread(action, state0, state1, state2);
    }
}