namespace Intersect.Framework.Threading;

partial class ActionQueue<TActionQueue, TEnqueueState>
{
    public TReturn EnqueueReturn<TState0, TState1, TReturn>(
        Func<TState0, TState1, TReturn> func,
        TState0 state0,
        TState1 state1
    )
    {
        if (IsActive)
        {
            return func(state0, state1);
        }

        Return<TState0, TState1, TReturn> @return = new(func);
        Enqueue(
            Return<TState0, TState1, TReturn>.Wrapper,
            new State<TState0, TState1, Return<TState0, TState1, TReturn>>(
                Return<TState0, TState1, TReturn>.FuncWrapper,
                state0,
                state1,
                @return
            )
        );
        return @return.Value;
    }

    private sealed record Return<TState0, TState1, TReturn>(Func<TState0, TState1, TReturn> Func)
    {
        public static readonly Action<State<TState0, TState1, Return<TState0, TState1, TReturn>>>
            Wrapper =
                state => state.Action(state.State0, state.State1, state.State2);

        public static readonly Action<TState0, TState1, Return<TState0, TState1, TReturn>> FuncWrapper =
            (state0, state1, returnValue) =>
            {
                returnValue.Value = returnValue.Func(state0, state1);
            };

        public TReturn Value { get; private set; } = default!;
    }
}