using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Input;


/// <summary>
///     Keyboard state.
/// </summary>
public partial class KeyData
{

    public readonly bool[] KeyState;

    public readonly float[] NextRepeat;

    public Dictionary<MouseButton, bool> MouseButtonState = [];

    public Base Target;

    public KeyData()
    {
        KeyState = new bool[(int) Key.Count];
        NextRepeat = new float[(int) Key.Count];

        // everything is initialized to 0 by default
    }

    public bool IsMouseButtonDown(MouseButton mouseButton) => MouseButtonState.GetValueOrDefault(mouseButton, false);

    /// <summary>
    ///
    /// </summary>
    /// <param name="mouseButton"></param>
    /// <param name="pressedState"></param>
    /// <returns>Returns true if it was a supported mouse button, false if it is an invalid mouse button.</returns>
    public bool SetMouseButtonState(MouseButton mouseButton, bool pressedState)
    {
        switch (mouseButton)
        {
            case MouseButton.Left:
            case MouseButton.Right:
            case MouseButton.Middle:
            case MouseButton.X1:
            case MouseButton.X2:
                MouseButtonState[mouseButton] = pressedState;
                return true;
            case MouseButton.None:
            default:
                return false;
        }
    }

}
