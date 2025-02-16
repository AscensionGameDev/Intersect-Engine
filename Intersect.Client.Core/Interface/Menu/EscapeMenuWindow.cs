using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class EscapeMenuWindow : Window
{
    private readonly Button _settings;
    private readonly Button _charselect;
    private readonly Button _logout;
    private readonly Button _exitdesktop;
    private readonly Button _closemenu;

    private readonly Panel _versionPanel;

    private readonly Func<SettingsWindow> _settingsWindowProvider;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public EscapeMenuWindow(Canvas canvas, Func<SettingsWindow> settingsWindowProvider)
        : base(canvas, Strings.EscapeMenu.Title, false,
            $"{nameof(EscapeMenuWindow)}_{(ClientContext.IsSinglePlayer ? "singleplayer" : "online")}")
    {
        _settingsWindowProvider = settingsWindowProvider;

        Alignment = [Alignments.Center];

        IsResizable = false;
        Padding = Padding.Zero;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;

        _settings = new Button(this, nameof(_settings))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.Settings,
        };
        _settings.Clicked += (s, e) => OpenSettingsWindow(true);

        _charselect = new Button(this, nameof(_charselect))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.CharacterSelect,
        };
        _charselect.Clicked += _buttonCharacterSelect_Clicked;

        _logout = new Button(this, nameof(_logout))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.Logout,
        };
        _logout.Clicked += buttonLogout_Clicked;

        _exitdesktop = new Button(this, nameof(_exitdesktop))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.ExitToDesktop,
        };
        _exitdesktop.Clicked += buttonQuit_Clicked;

        _closemenu = new Button(this, nameof(_closemenu))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.Close,
        };
        _closemenu.Clicked += (s, e) => Hide();

        if (!string.IsNullOrEmpty(Strings.MainMenu.SettingsTooltip))
        {
            _settings.SetToolTipText(Strings.MainMenu.SettingsTooltip);
        }

        if (Options.Instance.Player.MaxCharacters <= 1)
        {
            _charselect.IsDisabled = true;
        }

        _versionPanel = new VersionPanel(canvas, name: nameof(_versionPanel));
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
        var settingsWindow = _settingsWindowProvider();
        if (settingsWindow.IsVisible)
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

        _charselect.IsDisabled = Globals.Me?.CombatTimer > Timing.Global.Milliseconds;
    }

    public void OpenSettingsWindow(bool returnToMenu = false)
    {
        var settingsWindow = _settingsWindowProvider();
        settingsWindow.Show(returnToMenu ? this : null);
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