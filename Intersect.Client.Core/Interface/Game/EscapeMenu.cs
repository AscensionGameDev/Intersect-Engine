using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

public partial class EscapeMenu : ImagePanel
{
    private readonly SettingsWindow _settingsWindow;
    private readonly Button _buttonCharacterSelect;
    private readonly Panel _versionPanel;

    private readonly Framework.Graphics.GameFont _font;

    public EscapeMenu(Canvas gameCanvas, SettingsWindow settingsWindow) : base(gameCanvas, nameof(EscapeMenu))
    {
        _settingsWindow = settingsWindow;

        Interface.InputBlockingComponents.Add(this);

        Width = gameCanvas.Width;
        Height = gameCanvas.Height;

        _font = GameContentManager.Current.GetFont("sourcesansproblack", 12)!;

        // Create the container
        var container = new ImagePanel(this, nameof(EscapeMenu));

        // Title Label
        _ = new Label(container, "TitleLabel")
        {
            Text = Strings.EscapeMenu.Title,
            AutoSizeToContents = false,
            Dock = Pos.Fill | Pos.CenterV,
            MouseInputEnabled = false,
            Padding = new Padding(8, 2, 0, 0),
            TextAlign = Pos.Left | Pos.Top,
            TextColor = Skin.Colors.Window.TitleInactive,
            Font = _font,
            FontSize = 18,
        };

        var buttonSettings = new Button(container, "SettingsButton");
        InitializeButton(buttonSettings, Strings.EscapeMenu.Settings);
        buttonSettings.Clicked += (s, e) => OpenSettingsWindow(true);

        // Character Select Button
        _buttonCharacterSelect = new Button(container, "CharacterSelectButton");
        InitializeButton(_buttonCharacterSelect, Strings.EscapeMenu.CharacterSelect);
        _buttonCharacterSelect.Clicked += _buttonCharacterSelect_Clicked;

        // Logout Button
        var buttonLogout = new Button(container, "LogoutButton");
        InitializeButton(buttonLogout, Strings.EscapeMenu.Logout);
        buttonLogout.Clicked += buttonLogout_Clicked;

        // Exit to Desktop Button
        var buttonQuit = new Button(container, "ExitToDesktopButton");
        InitializeButton(buttonQuit, Strings.EscapeMenu.ExitToDesktop);
        buttonQuit.Clicked += buttonQuit_Clicked;

        // Close Button
        var buttonContinue = new Button(container, "CloseButton");
        InitializeButton(buttonContinue, Strings.EscapeMenu.Close);
        buttonContinue.Clicked += (s, e) => Hide();

        container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        if (Options.Instance.Player.MaxCharacters <= 1)
        {
            _buttonCharacterSelect.IsDisabled = true;
        }

        _versionPanel = new VersionPanel(this, name: nameof(_versionPanel));
    }

    private void InitializeButton(Button button, string text)
    {
        _ = new Label(button)
        {
            Text = text,
            Font = _font,
            Padding = new Padding(0, 20, 0, 0),
            Alignment = [ Alignments.Center, Alignments.CenterV ],
        };
        button.IsTabable = true;
    }

    public override void Invalidate()
    {
        if (IsHidden)
        {
            RemoveModal();
        }
        else
        {
            MakeModal(true);
        }

        base.Invalidate();
        if (Interface.GameUi?.GameCanvas != null)
        {
            Interface.GameUi.GameCanvas.MouseInputEnabled = IsVisible;
        }
    }

    /// <inheritdoc />
    public override void ToggleHidden()
    {
        if (_settingsWindow.IsVisible)
        {
            return;
        }

        base.ToggleHidden();
    }

    public void Update()
    {
        if (!IsHidden)
        {
            BringToFront();
        }

        _buttonCharacterSelect.IsDisabled = Globals.Me?.CombatTimer > Timing.Global.Milliseconds;
    }

    public void OpenSettingsWindow(bool returnToMenu = false)
    {
        _settingsWindow.Show(returnToMenu ? this : null);
        Hide();
    }

    private void _buttonCharacterSelect_Clicked(Base sender, MouseButtonState arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            LogoutToCharacterSelect(null, null);
        }
    }

    private void LogoutToCharacterSelect(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Main.Logout(true);
    }

    private void buttonLogout_Clicked(Base sender, MouseButtonState arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            LogoutToMainMenu(null, null);
        }
    }

    private void LogoutToMainMenu(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Main.Logout(false);
    }

    private void buttonQuit_Clicked(Base sender, MouseButtonState arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            ExitToDesktop(null, null);
        }
    }

    private void ShowCombatWarning()
    {
        AlertWindow.Open(
            Strings.Combat.WarningCharacterSelect,
            Strings.Combat.WarningTitle,
            AlertType.Warning,
            handleSubmit: LogoutToCharacterSelect,
            inputType: InputType.YesNo
        );
    }

    private void ExitToDesktop(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Globals.IsRunning = false;
    }
}
