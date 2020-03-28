using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Input
{

    public abstract class GameInput
    {

        public enum KeyboardType
        {

            Normal,

            Password,

            Email,

            Numberic,

            Pin

        }

        public enum MouseButtons
        {

            None = -1,

            Left = 0,

            Right,

            Middle

        }

        public abstract bool MouseButtonDown(MouseButtons mb);

        public abstract bool KeyDown(Keys key);

        public abstract Pointf GetMousePosition();

        public abstract void Update();

        public abstract void OpenKeyboard(
            KeyboardType type,
            string text,
            bool autoCorrection,
            bool multiLine,
            bool secure
        );

    }

}
