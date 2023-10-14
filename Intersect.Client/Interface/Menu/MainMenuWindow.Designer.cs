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

        IsClosable = false;
        DisableResizing();
        Padding = Padding.Zero;
        InnerPanelPadding = new Padding(8, 8, 8, 8);
        TitleBar.MouseInputEnabled = false;

        Button[] visibleButtons = new []
        {
            _buttonStart,
            _buttonLogin,
            _buttonRegister,
            _buttonSettings,
            _buttonCredits,
            _buttonExit,
        }.Where(button => button.IsVisible).ToArray();

        const int spacerX = 0;
        const int spacerY = 4;
        const int defaultWidth = 87;
        const int defaultHeight = 154;

        var innerWidth = (spacerX + defaultWidth) * visibleButtons.Length - spacerX;
        var innerHeight = spacerY * 2 + defaultHeight;

        SetSize(
            innerWidth + InnerPanelPadding.Left + InnerPanelPadding.Right,
            innerHeight + TitleBarBounds.Bottom + InnerPanelPadding.Top + InnerPanelPadding.Bottom
        );

        AddAlignment(Alignments.Center);
        AlignmentDistance = new Padding(0, 40, 0, 0);
        ProcessAlignments();

        TitleLabel.TextColor = Color.White;
        TitleLabel.TextColorOverride = Color.White;
        TitleLabel.FontName = "sourcesansproblack";
        TitleLabel.FontSize = 12;
        TitleLabel.Padding = new Padding(8, 4, 8, 4);
        TitleLabel.TextPadding = new Padding(8, 4, 8, 4);
        TitleLabel.SizeToContents();

        TitleBar.SetBounds(0, 0, Width, TitleLabel.Height);

        var x = InnerPanelPadding.Left;
        foreach (var button in visibleButtons)
        {
            button.SetBounds(x, spacerY, defaultWidth, defaultHeight);
            x += defaultWidth + spacerX;

            button.FontName = "sourcesansproblack";
            button.FontSize = 12;
            button.TextColor = Color.White;
            button.TextColorOverride = Color.White;
            button.TextPadding = new Padding(0, 8, 0, 0);
            button.SetHoverSound("octave-tap-resonant.wav");

            var buttonName = button.Name;
            button.SetImage(
                $"mainmenu{buttonName}.png",
                Button.ControlState.Normal
            );
            button.SetImage(
                $"mainmenu{buttonName}_clicked.png",
                Button.ControlState.Clicked
            );
            button.SetImage(
                $"mainmenu{buttonName}_disabled.png",
                Button.ControlState.Disabled
            );
            button.SetImage(
                $"mainmenu{buttonName}_hovered.png",
                Button.ControlState.Hovered
            );
        }

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
    }
}