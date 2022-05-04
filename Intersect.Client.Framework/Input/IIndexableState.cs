namespace Intersect.Client.Framework.Input;

public interface IIndexableState<TButton>
{
    ButtonState this[TButton button] { get; }
}
