using Intersect.Time;

namespace Intersect.Client.Framework.Input;

public abstract class Stateful<TState, TButton> where TState : IIndexableState<TButton>
{
    public ButtonState this[TButton button] => State[button];

    public TState PreviousState { get; private set; }

    public TState State { get; private set; }

    public void Update(FrameTime frameTime)
    {
        PreviousState = State;
        State = GetCurrentState();
    }

    protected abstract TState GetCurrentState();
}
