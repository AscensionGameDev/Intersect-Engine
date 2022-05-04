using Intersect.Client.Framework.Input;

namespace Intersect.Editor.MonoGame.Input;

using MGMouse = Microsoft.Xna.Framework.Input.Mouse;

internal sealed class MonoGameMouse : Mouse
{
    protected override MouseState GetCurrentState()
    {
        var state = MGMouse.GetState();
        return new MouseState(
            state.X,
            state.Y,
            state.HorizontalScrollWheelValue,
            state.ScrollWheelValue,
            new[]
            {
                state.LeftButton.FromMonoGame(),
                state.RightButton.FromMonoGame(),
                state.MiddleButton.FromMonoGame(),
                state.XButton1.FromMonoGame(),
                state.XButton2.FromMonoGame(),
            }
        );
    }
}
