using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Windows;

/// <summary>
/// The immutable event arguments for text input events from the system window.
/// </summary>
public struct TextInputEventArgs
{
    /// <summary>
    /// The character representation of the pressed key.
    /// </summary>
    public readonly char Character;

    /// <summary>
    /// The key that triggered this event.
    /// </summary>
    public readonly Key Key;

    public TextInputEventArgs(char character, Key key = Key.None)
    {
        Character = character;
        Key = key;
    }
}
