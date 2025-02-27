using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Interface.Menu;

public partial class EscapeMenuWindow
{
    protected override void EnsureInitialized()
    {
        var canvas = Canvas ?? throw new InvalidOperationException($"Not attached to a {nameof(Canvas)}");

        Button[] visibleButtons = new[]
        {
            _openSettingsButton,
            _returnToCharacterSelectionButton,
            _logoutButton,
            _exitToDesktopButton,
            _returnToGameButton,
        }.Where(button => button.IsVisibleInParent).ToArray();

        const int defaultWidth = 97;
        const int defaultHeight = 154;

        Size = new Point(
            defaultWidth * visibleButtons.Length + InnerPanelPadding.Left + InnerPanelPadding.Right,
            defaultHeight + TitleBarBounds.Bottom + InnerPanelPadding.Top + InnerPanelPadding.Bottom
        );

        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        foreach (var button in visibleButtons)
        {
            button.Size = new Point(defaultWidth, defaultHeight);
            button.FontName = "sourcesansproblack";
            button.FontSize = 12;
            button.Padding = new Padding(0, 24, 0, 0);
            button.Dock = Pos.Left;

            var buttonName = button.Name;
            button.SetStateTexture(ComponentState.Normal, $"ingame{buttonName}.png");
            button.SetStateTexture(ComponentState.Active, $"ingame{buttonName}_clicked.png");
            button.SetStateTexture(ComponentState.Disabled, $"ingame{buttonName}_disabled.png");
            button.SetStateTexture(ComponentState.Hovered, $"ingame{buttonName}_hovered.png");
        }

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }
}