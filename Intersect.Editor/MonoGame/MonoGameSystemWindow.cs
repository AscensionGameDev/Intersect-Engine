using Intersect.Client.Framework.Windows;

using Microsoft.Xna.Framework;

namespace Intersect.Editor.MonoGame;

internal sealed class MonoGameSystemWindow : SystemWindow
{
    private readonly GameWindow _gameWindow;

    internal MonoGameSystemWindow(GameWindow gameWindow)
    {
        _gameWindow = gameWindow ?? throw new ArgumentNullException(nameof(gameWindow));

        _gameWindow.TextInput += (sender, args) => OnTextInput(args.FromMonoGame());
    }

    public override string Title
    {
        get => _gameWindow.Title;
        set => _gameWindow.Title = value;
    }
}
