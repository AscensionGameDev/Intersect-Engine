using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
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

    public EscapeMenu(Canvas gameCanvas) : base(gameCanvas, nameof(EscapeMenu))
    {
        Interface.InputBlockingElements?.Add(this);

        Width = gameCanvas.Width;
        Height = gameCanvas.Height;

        // Create the container
        var container = new ImagePanel(this, nameof(EscapeMenu));

        // Title Label
        _ = new Label(container, "TitleLabel")
        {
            Text = Strings.EscapeMenu.Title,
        };

        // Settings Window and Button
        _settingsWindow = new SettingsWindow(gameCanvas, null, this);

        var buttonSettings = new Button(container, "SettingsButton")
        {
            Text = Strings.EscapeMenu.Settings,
        };
        buttonSettings.Clicked += (s, e) => OpenSettingsWindow(true);

        // Character Select Button
        _buttonCharacterSelect = new Button(container, "CharacterSelectButton")
        {
            Text = Strings.EscapeMenu.CharacterSelect,
        };
        _buttonCharacterSelect.Clicked += _buttonCharacterSelect_Clicked;

        // Logout Button
        var buttonLogout = new Button(container, "LogoutButton")
        {
            Text = Strings.EscapeMenu.Logout,
        };
        buttonLogout.Clicked += buttonLogout_Clicked;

        // Exit to Desktop Button
        var buttonQuit = new Button(container, "ExitToDesktopButton")
        {
            Text = Strings.EscapeMenu.ExitToDesktop,
        };
        buttonQuit.Clicked += buttonQuit_Clicked;

        // Close Button
        var buttonContinue = new Button(container, "CloseButton")
        {
            Text = Strings.EscapeMenu.Close,
        };
        buttonContinue.Clicked += (s, e) => Hide();

        container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        if (Options.Player.MaxCharacters <= 1)
        {
            _buttonCharacterSelect.IsDisabled = true;
        }
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
            Interface.GameUi.GameCanvas.MouseInputEnabled = true;
        }
    }

    /// <inheritdoc />
    public override void ToggleHidden()
    {
        if (!_settingsWindow.IsHidden)
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
        _settingsWindow.Show(returnToMenu);
        Interface.GameUi?.EscapeMenu?.Hide();
    }

    private void _buttonCharacterSelect_Clicked(Base sender, ClickedEventArgs arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            //Show Logout in Combat Warning
            _ = new InputBox(
                title: Strings.Combat.WarningTitle,
                prompt: Strings.Combat.WarningCharacterSelect,
                inputType: InputBox.InputType.YesNo,
                onSuccess: LogoutToCharacterSelect
            );
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

    private void buttonLogout_Clicked(Base sender, ClickedEventArgs arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            //Show Logout in Combat Warning
            _ = new InputBox(
                title: Strings.Combat.WarningTitle,
                prompt: Strings.Combat.WarningLogout,
                inputType: InputBox.InputType.YesNo,
                onSuccess: LogoutToMainMenu
            );
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

    private void buttonQuit_Clicked(Base sender, ClickedEventArgs arguments)
    {
        ToggleHidden();
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            //Show Logout in Combat Warning
            _ = new InputBox(
                title: Strings.Combat.WarningTitle,
                prompt: Strings.Combat.WarningExitDesktop,
                inputType: InputBox.InputType.YesNo,
                onSuccess: ExitToDesktop
            );
        }
        else
        {
            ExitToDesktop(null, null);
        }
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
