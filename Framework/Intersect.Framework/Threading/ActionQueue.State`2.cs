namespace Intersect.Framework.Threading;

partial class ActionQueue<TActionQueue, TEnqueueState>
{
    public void Enqueue<TState0, TState1>(Action<TState0, TState1> action, TState0 state0, TState1 state1)
    {
        if (IsActive)
        {
            action(state0, state1);
            return;
        }

        Enqueue(
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