using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Input;


public abstract partial class GameInput : IGameInput
{

    public abstract bool MouseButtonDown(MouseButtons mb);

    public abstract bool KeyDown(Keys key);

    public Pointf MousePosition => GetMousePosition();

    public abstract Pointf GetMousePosition();

    public abstract void Update(TimeSpan elapsed);

    public abstract void OpenKeyboard(
        KeyboardType type,
        string text,
        bool autoCorrection,
        bool multiLine,
        bool secure
    );

    public abstract void OpenKeyboard(
        KeyboardType keyboardType,
        Action<string?> inputHandler,
        string description,
        string text,
        bool multiline = false,
        uint maxLength = 1024,
        Rectangle? inputBounds = default
    );

}
