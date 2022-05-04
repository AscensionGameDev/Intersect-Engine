using Intersect.Client.Framework.UserInterface;
using Intersect.Editor.MonoGame.Graphics;
using Intersect.Editor.MonoGame.Input;

using Microsoft.Xna.Framework;

namespace Intersect.Editor.MonoGame.UserInterface;

internal sealed class MonoGameImGuiRenderer : ImGuiRenderer
{
    public MonoGameImGuiRenderer(Game game)
        : base(new MonoGameGraphicsDevice(game.GraphicsDevice), new MonoGameSystemWindow(game.Window), new()
        {
            Keyboard = new MonoGameKeyboard(),
            Mouse = new MonoGameMouse(),
        })
    {
    }
}
