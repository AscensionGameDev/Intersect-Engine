using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Framework.Core;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

public sealed partial class SimplifiedEscapeMenu : Framework.Gwen.Control.Menu
{
    private readonly Func<SettingsWindow> _settingsWindowProvider;
    private readonly MenuItem _settings;
    private readonly MenuItem _character;
    private readonly MenuItem _logout;
    private readonly MenuItem _exit;

    public SimplifiedEscapeMenu(Canvas gameCanvas, Func<SettingsWindow> settingsWindowProvider) : base(gameCanvas, nameof(SimplifiedEscapeMenu))
    {
        IsHidden = true;
        IconMarginDisabled = true;
        _settingsWindowProvider = settingsWindowProvider;

        ClearChildren();

        _settings = AddItem(Strings.EscapeMenu.Settings);
        _character = AddItem(Strings.EscapeMenu.CharacterSelect);
        _logout = AddItem(Strings.EscapeMenu.Logout);
        _exit = AddItem(Strings.EscapeMenu.ExitToDesktop);

        _settings.Clicked += OpenSettingsWindow;
        _character.Clicked += LogoutToCharacterSelectSelectClicked;
        _logout.Clicked += LogoutToMainToMainMenuClicked;
        _exit.Clicked += ExitToDesktopToDesktopClicked;

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());
    }

    public void ToggleHidden(Button? target)
    {
        var settingsWindow = _settingsWindowProvider();
        if (!settingsWindow.IsHidden || target == null)
        {
            return;
        }

        if (this.IsHidden)
        {
            // Position the context menu within the game canvas if near borders.
            var menuPosX = target.ToCanvas(new Point(0, 0)).X;
            var menuPosY = target.ToCanvas(new Point(0, 0)).Y;
            var newX = menuPosX;
            var newY = menuPosY + target.Height + 6;

            if (newX + Width >= Canvas?.Width)
            {
                newX = menuPosX - Width + target.Width;
            }

            if (newY + Height >= Canvas?.Height)
            {
                newY = menuPosY - Height - 6;
            }

            SizeToChildren();
            Open(Pos.None);
            SetPosition(newX, newY);
        }
        else
        {
            Close();
        }
    }

    private void LogoutToCharacterSelectSelectClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            LogoutToCharacterSelect(null, null);
        }
    }

    private void LogoutToMainToMainMenuClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            LogoutToMainMenu(null, null);
        }
    }

    private void ExitToDesktopToDesktopClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
            ShowCombatWarning();
        }
        else
        {
            ExitToDesktop(null, null);
        }
    }

    private static void ShowCombatWarning()
    {
        AlertWindow.Open(
            Strings.Combat.WarningCharacterSelect,
            Strings.Combat.WarningTitle,
            AlertType.Warning,
            handleSubmit: LogoutToCharacterSelect,
            inputType: InputType.YesNo
        );
    }

    private void OpenSettingsWindow(object? sender, EventArgs? e)
    {
        var settingsWindow = _settingsWindowProvider();
        if (settingsWindow.IsVisible)
        {
            return;
        }

        settingsWindow.Show();
    }

    private static void LogoutToCharacterSelect(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Main.Logout(true);
    }

    private static void LogoutToMainMenu(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Main.Logout(false);
    }

    private static void ExitToDesktop(object? sender, EventArgs? e)
    {
        if (Globals.Me != null)
        {
            Globals.Me.CombatTimer = 0;
        }

        Globals.IsRunning = false;
    }
}