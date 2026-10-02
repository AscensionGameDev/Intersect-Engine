using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.ControlInternal;

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
        }.Where(button => button.IsVisibleInTree).ToArray();

        const int listWidth = 320;
        const int buttonHeight = 54;

        Size = new Point(
            listWidth + InnerPanelPadding.Left + InnerPanelPadding.Right,
            buttonHeight * visibleButtons.Length + InnerPanelPadding.Top + InnerPanelPadding.Bottom
        );

        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        var normalTextColor = new Color(255, 214, 210, 198);
        var hoverTextColor = new Color(255, 235, 196, 110);
        var disabledTextColor = new Color(150, 140, 140, 140);

        foreach (var button in visibleButtons)
        {
            button.Size = new Point(listWidth, buttonHeight);
            button.FontName = "sourcesansproblack";
            button.FontSize = 19;
            button.Padding = new Padding(8, 0, 8, 0);
            button.Dock = Pos.Top;
            button.TextAlign = Pos.Left | Pos.CenterV;
            button.ShouldDrawBackground = false;

            button.SetTextColor(normalTextColor, ComponentState.Normal);
            button.SetTextColor(hoverTextColor, ComponentState.Hovered);
            button.SetTextColor(hoverTextColor, ComponentState.Active);
            button.SetTextColor(disabledTextColor, ComponentState.Disabled);
        }

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
    }
}