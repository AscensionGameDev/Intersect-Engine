using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Menu;

public partial class MainMenuWindow
{
    protected override void EnsureInitialized()
    {
        var canvas = Canvas ?? throw new InvalidOperationException($"Not attached to a {nameof(Canvas)}");

        Button[] visibleButtons = new []
        {
            _buttonStart,
            _buttonLogin,
            _buttonRegister,
            _buttonSettings,
            _buttonCredits,
            _buttonExit,
        }.Where(button => button.IsVisible).ToArray();

        const int defaultWidth = 87;
        const int defaultHeight = 154;

        Size = new Point(
            defaultWidth * visibleButtons.Length + InnerPanelPadding.Left + InnerPanelPadding.Right,
            defaultHeight + TitleBarBounds.Bottom + InnerPanelPadding.Top + InnerPanelPadding.Bottom
        );

        TitleLabel.TextColor = Color.White;
        TitleLabel.FontName = "sourcesansproblack";
        TitleLabel.FontSize = 14;

        Titlebar.SetBounds(0, 0, Width, TitleLabel.Height);

        foreach (var button in visibleButtons)
        {
            button.Size = new Point(defaultWidth, defaultHeight);
            button.FontName = "sourcesansproblack";
            button.FontSize = 12;
            button.Padding = new Padding(0, 24, 0, 0);
            button.Dock = Pos.Left;
            button.SetHoverSound("octave-tap-resonant.wav");

            var buttonName = button.Name;
            button.SetStateTexture($"mainmenu{buttonName}.png", ComponentState.Normal);
            button.SetStateTexture($"mainmenu{buttonName}_clicked.png", ComponentState.Active);
            button.SetStateTexture($"mainmenu{buttonName}_disabled.png", ComponentState.Disabled);
            button.SetStateTexture($"mainmenu{buttonName}_hovered.png", ComponentState.Hovered);
        }

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
    }
}