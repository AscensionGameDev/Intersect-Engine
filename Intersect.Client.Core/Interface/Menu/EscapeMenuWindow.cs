using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame;
using Intersect.Framework.Core;

namespace Intersect.Client.Interface.Menu;

public partial class EscapeMenuWindow : Window
{
    private readonly Button _openSettingsButton;
    private readonly Button _returnToCharacterSelectionButton;
    private readonly Button _logoutButton;
    private readonly Button _exitToDesktopButton;
    private readonly Button _returnToGameButton;

    private readonly Panel _versionPanel;

    private readonly Func<SettingsWindow> _settingsWindowProvider;

    public EscapeMenuWindow(Canvas canvas, Func<SettingsWindow> settingsWindowProvider)
        : base(canvas, Strings.EscapeMenu.Title, false,
            $"{nameof(EscapeMenuWindow)}_{(ClientContext.IsSinglePlayer ? "singleplayer" : "online")}")
    {
        _settingsWindowProvider = settingsWindowProvider;

        Alignment = [Alignments.Center];

        IsClosable = false;
        IsResizable = false;
        Padding = Padding.Zero;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;

        _openSettingsButton = new Button(this, nameof(_openSettingsButton))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.Settings,
        };
        _openSettingsButton.Clicked += OpenSettingsButtonOnClicked;

        _returnToCharacterSelectionButton = new Button(this, nameof(_returnToCharacterSelectionButton))
        {
            IsVisibleInParent = Options.Instance.Player.MaxCharacters > 1,
            IsTabable = true,
            Text = Strings.EscapeMenu.CharacterSelect,
        };
        _returnToCharacterSelectionButton.Clicked += ReturnToCharacterSelectionButtonOnClicked;

        _logoutButton = new Button(this, nameof(_logoutButton))
        {
            IsVisibleInParent = !ClientContext.IsSinglePlayer,
            IsTabable = true,
            Text = Strings.EscapeMenu.Logout,
        };
        _logoutButton.Clicked += LogoutButtonOnClicked;

        _exitToDesktopButton = new Button(this, nameof(_exitToDesktopButton))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.ExitToDesktop,
        };
        _exitToDesktopButton.Clicked += ExitToDesktopButtonOnClicked;

        _returnToGameButton = new Button(this, nameof(_returnToGameButton))
        {
            IsTabable = true,
            Text = Strings.EscapeMenu.Close,
        };
        _returnToGameButton.Clicked += ReturnToGameButtonOnClicked;

        if (!string.IsNullOrEmpty(Strings.MainMenu.SettingsTooltip))
        {
            _openSettingsButton.SetToolTipText(Strings.MainMenu.SettingsTooltip);
        }

        _versionPanel = new VersionPanel(this, name: nameof(_versionPanel));

        Interface.InputBlockingComponents.Add(this);
    }

    private void ReturnToGameButtonOnClicked(Base s, MouseButtonState e)
    {
        Hide();
    }

    private void OpenSettingsButtonOnClicked(Base s, MouseButtonState e)
    {
        OpenSettingsWindow(true);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        Interface.InputBlockingComponents.Remove(this);
    }

    protected override void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnVisibilityChanged(sender, eventArgs);

        if (eventArgs.IsVisibleInTree)
        {
            var modal = MakeModal(dim: true);
            _versionPanel.Parent = modal;
        }
        else
        {
            _versionPanel.Parent = this;
            RemoveModal();
        }
    }

    /// <inheritdoc />
    public override void ToggleHidden()
    {
        var settingsWindow = _settingsWindowProvider();
        if (settingsWindow.IsVisibleInTree)
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

        _returnToCharacterSelectionButton.IsDisabled = Globals.Me?.CombatTimer > Timing.Global.Milliseconds;
    }

    public void OpenSettingsWindow(bool returnToMenu = false)
    {
        var settingsWindow = _settingsWindowProvider();
        settingsWindow.Show(returnToMenu ? this : null);
        Hide();
    }

    private void ReturnToCharacterSelectionButtonOnClicked(Base sender, MouseButtonState arguments)
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

    private void LogoutButtonOnClicked(Base sender, MouseButtonState arguments)
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

    private void ExitToDesktopButtonOnClicked(Base sender, MouseButtonState arguments)
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
        if (IntersectGame._isShowingExitConfirmation)
        {
            return;
        }

        AlertWindow.Open(
            Strings.General.QuitPrompt,
            Strings.General.QuitTitle,
            AlertType.Warning,
            inputType: InputType.YesNo,
            handleSubmit: (_, _) =>
            {
                if (Globals.Me != null)
                {
                    Globals.Me.CombatTimer = 0;
                }

                IntersectGame._isShowingExitConfirmation = false;
                Globals.IsRunning = false;
            },
            handleCancel: (_, _) =>
            {
                IntersectGame._isShowingExitConfirmation = false;
            }
        );

        IntersectGame._isShowingExitConfirmation = true;
    }
}