namespace Intersect.Framework.Threading;

public sealed partial class ThreadQueue
{
    public void RunOnMainThread<TState0, TState1>(Action<TState0, TState1> action, TState0 state0, TState1 state1)
    {
        if (IsOnMainThread)
        {
            action(state0, state1);
            return;
        }

        RunOnMainThread(
            State<TState0, TState1>.Wrapper,
            new State<TState0, TState1>(action, state0, state1)
        );
    }

    private record struct State<TState0, TState1>(Action<TState0, TState1> Action, TState0 State0, TState1 State1)
    {
        public static readonly Action<State<TState0, TState1>> Wrapper = state => state.Action(
            state.State0,
            state.State1
        );
    }
}