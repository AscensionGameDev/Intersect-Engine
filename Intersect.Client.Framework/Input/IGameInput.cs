using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Input
{
    public interface IGameInput
    {
        Pointf MousePosition { get; }
        bool KeyDown(Keys key);
        bool MouseButtonDown(MouseButtons mb);
    }
}