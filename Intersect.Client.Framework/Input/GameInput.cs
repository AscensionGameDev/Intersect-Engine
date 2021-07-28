using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Input
{

    public abstract class GameInput : IGameInput
    {

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
