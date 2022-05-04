using Intersect.Client.Framework.Input;

using Microsoft.Xna.Framework.Input;

namespace Intersect.Editor.MonoGame.Input;

using ButtonState = Client.Framework.Input.ButtonState;
using Keyboard = Client.Framework.Input.Keyboard;
using KeyboardState = Client.Framework.Input.KeyboardState;
using MGKeyboard = Microsoft.Xna.Framework.Input.Keyboard;

internal sealed class MonoGameKeyboard : Keyboard
{
    private static readonly Keys[] AvailableKeys = Enum.GetValues<Keys>();
    private static readonly int KeyArraySize = (int)Enum.GetValues<Key>().Max() + 1;

    protected override KeyboardState GetCurrentState()
    {
        var state = MGKeyboard.GetState();

        KeyModifiers keyModifiers = default;

        if (state.CapsLock)
        {
            keyModifiers |= KeyModifiers.CapsLock;
        }

        if (state.NumLock)
        {
            keyModifiers |= KeyModifiers.CapsLock;
        }

        if (state.IsKeyDown(Keys.LeftAlt))
        {
            keyModifiers |= KeyModifiers.LeftAlt;
        }

        if (state.IsKeyDown(Keys.RightAlt))
        {
            keyModifiers |= KeyModifiers.RightAlt;
        }

        if (state.IsKeyDown(Keys.LeftControl))
        {
            keyModifiers |= KeyModifiers.LeftCtrl;
        }

        if (state.IsKeyDown(Keys.RightControl))
        {
            keyModifiers |= KeyModifiers.RightCtrl;
        }

        if (state.IsKeyDown(Keys.LeftShift))
        {
            keyModifiers |= KeyModifiers.LeftShift;
        }

        if (state.IsKeyDown(Keys.RightShift))
        {
            keyModifiers |= KeyModifiers.RightShift;
        }

        if (state.IsKeyDown(Keys.LeftWindows))
        {
            keyModifiers |= KeyModifiers.LeftGui;
        }

        if (state.IsKeyDown(Keys.RightWindows))
        {
            keyModifiers |= KeyModifiers.RightGui;
        }

        var keyStates = AvailableKeys.Aggregate(
            new ButtonState[KeyArraySize],
            (keyStates, keys) =>
            {
                keyStates[(int)keys.FromMonoGame()] = state.IsKeyDown(keys) ? ButtonState.Pressed : ButtonState.Released;
                return keyStates;
            }
        );

        return new KeyboardState(
            keyStates,
            keyModifiers
        );
    }
}
