using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Platform;
using Intersect.Numerics;

using Game = Microsoft.Xna.Framework.Game;
using GameWindow = Microsoft.Xna.Framework.GameWindow;

namespace Intersect.EndUser.Runtime.MonoGame.Platform;

public sealed class MonoGamePlatformWindow : PlatformWindow
{
    private readonly Game _game;
    private readonly GameWindow _gameWindow;

    public MonoGamePlatformWindow(
        Game game,
        GraphicsDevice graphicsDevice,
        GameWindow window
    )
    {
        _game = game;
        _gameWindow = window;
        _gameWindow.ClientSizeChanged += (_, e) => OnResize(e);

        GraphicsDevice = graphicsDevice;
    }

    public override Rectangle Bounds
    {
        get => new(Position, Size);
        set
        {
            Position = value.Position;
            Size = value.Size;
        }
    }

    public override GraphicsDevice GraphicsDevice { get; }

    public override Numerics.Point Position
    {
        get => new(_gameWindow.Position.X, _gameWindow.Position.Y);
        set => _gameWindow.Position = new(value.X, value.Y);
    }

    public override Numerics.Point Size
    {
        get => new(_gameWindow.ClientBounds.Size.X, _gameWindow.ClientBounds.Size.Y);
        set => GraphicsDevice.BackBufferSize = value;
    }

    public override string Title
    {
        get => _gameWindow.Title;
        set => _gameWindow.Title = value;
    }

    public override Viewport Viewport
    {
        get => GraphicsDevice.Viewport;
        set => GraphicsDevice.Viewport = value;
    }

    public override void Close() => _game.Exit();
}
