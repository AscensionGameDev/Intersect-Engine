using Intersect.Time;

namespace Intersect.Client.Framework.Input;

public class InputManager
{
    private Keyboard _keyboard;
    private Mouse _mouse;

    public Keyboard Keyboard
    {
        get => _keyboard ?? throw new InvalidOperationException();
        set => _keyboard = value;
    }

    public Mouse Mouse
    {
        get => _mouse ?? throw new InvalidOperationException();
        set => _mouse = value;
    }

    public void Update(FrameTime frameTime)
    {
        _keyboard?.Update(frameTime);
        _mouse?.Update(frameTime);
    }
}
