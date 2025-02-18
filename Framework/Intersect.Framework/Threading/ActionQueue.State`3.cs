namespace Intersect.Framework.Threading;

partial class ActionQueue<TActionQueue, TEnqueueState>
{
    public void Enqueue<TState0, TState1, TState2>(
        Action<TState0, TState1, TState2> action,
        TState0 state0,
        TState1 state1,
        TState2 state2
    )
    {
        if (IsActive)
        {
            action(state0, state1, state2);
            return;
        }

        Enqueue(
            State<TState0, TState1, TState2>.Wrapper,
            new State<TState0, TState1, TState2>(action, state0, state1, state2)
        );
    }

    private record struct State<TState0, TState1, TState2>(
        Action<TState0, TState1, TState2> Action,
        TState0 State0,
        TState1 State1,
        TState2 State2
    )
    {
        public static readonly Action<State<TState0, TState1, TState2>> Wrapper = state => state.Action(
            state.State0,
            state.State1,
            state.State2
        );
    }
}