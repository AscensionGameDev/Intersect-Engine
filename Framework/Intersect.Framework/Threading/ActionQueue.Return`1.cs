namespace Intersect.Framework.Threading;

partial class ActionQueue<TActionQueue, TEnqueueState>
{
    public TReturn EnqueueReturn<TState, TReturn>(Func<TState, TReturn> func, TState state)
    {
        if (IsActive)
        {
            return func(state);
        }

        Return<TState, TReturn> @return = new(func);
        Enqueue(
            Return<TState, TReturn>.Wrapper,
            new State<TState, Return<TState, TReturn>>(
                Return<TState, TReturn>.FuncWrapper,
                state,
                @return
            )
        );
        return @return.Value;
    }

    private sealed record Return<TState, TReturn>(Func<TState, TReturn> Func)
    {
        public static readonly Action<State<TState, Return<TState, TReturn>>> Wrapper = state =>
            state.Action(
                state.State0,
                state.State1
            );

        public static readonly Action<TState, Return<TState, TReturn>> FuncWrapper =
            (state, returnValue) =>
            {
                returnValue.Value = returnValue.Func(state);
            };

        public TReturn Value { get; private set; } = default!;
    }
}