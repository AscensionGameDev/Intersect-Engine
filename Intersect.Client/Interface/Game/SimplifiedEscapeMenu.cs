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

public sealed partial class SimplifiedEscapeMenu : Framework.Gwen.Control.Menu
{
    private readonly SettingsWindow _settingsWindow;
    private readonly MenuItem _settings;
    private readonly MenuItem _character;
    private readonly MenuItem _logout;
    private readonly MenuItem _exit;

    public SimplifiedEscapeMenu(Canvas gameCanvas) : base(gameCanvas, nameof(SimplifiedEscapeMenu))
    {
        IsHidden = true;
        IconMarginDisabled = true;
        _settingsWindow = new SettingsWindow(gameCanvas, null, null);

        Children.Clear();

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
        if (!_settingsWindow.IsHidden || target == null)
        {
            return;
        }

        if (this.IsHidden)
        {
            // Position the context menu within the game canvas if near borders.
            var menuPosX = target.LocalPosToCanvas(new Point(0, 0)).X;
            var menuPosY = target.LocalPosToCanvas(new Point(0, 0)).Y;
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

    private void LogoutToCharacterSelectSelectClicked(Base sender, ClickedEventArgs arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
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

    private void LogoutToMainToMainMenuClicked(Base sender, ClickedEventArgs arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
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

    private void ExitToDesktopToDesktopClicked(Base sender, ClickedEventArgs arguments)
    {
        if (Globals.Me?.CombatTimer > Timing.Global.Milliseconds)
        {
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

    private void OpenSettingsWindow(object? sender, EventArgs? e)
    {
        if (!_settingsWindow.IsHidden)
        {
            return;
        }

        _settingsWindow.Show();
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