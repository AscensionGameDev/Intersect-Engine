using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game
{

    public class EscapeMenu : ImagePanel
    {

        private readonly Button mClose;

        private readonly ImagePanel mContainer;

        private readonly Button mExitToDesktop;

        private readonly Button mGoToCharacterSelect;

        private readonly Button mLogout;

        private readonly Button mOptions;

        private readonly OptionsWindow mOptionsWindow;

        private readonly Label mTitle;

        public EscapeMenu(Canvas gameCanvas) : base(gameCanvas, "EscapeMenu")
        {
            Interface.InputBlockingElements?.Add(this);

            Width = gameCanvas.Width;
            Height = gameCanvas.Height;

            mContainer = new ImagePanel(this, "EscapeMenu");

            mTitle = new Label(mContainer, "TitleLabel")
            {
                Text = Strings.EscapeMenu.Title,
            };

            mOptionsWindow = new OptionsWindow(gameCanvas, null, null);

            mOptions = new Button(mContainer, "OptionsButton")
            {
                Text = Strings.EscapeMenu.Options
            };

            mOptions.Clicked += Options_Clicked;

            mGoToCharacterSelect = new Button(mContainer, "CharacterSelectButton")
            {
                Text = Strings.EscapeMenu.CharacterSelect
            };

            mGoToCharacterSelect.Clicked += GoToCharacterSelect_Clicked;

            mLogout = new Button(mContainer, "LogoutButton")
            {
                Text = Strings.EscapeMenu.Logout
            };

            mLogout.Clicked += Logout_Clicked;

            mExitToDesktop = new Button(mContainer, "ExitToDesktopButton")
            {
                Text = Strings.EscapeMenu.ExitToDesktop
            };

            mExitToDesktop.Clicked += ExitToDesktop_Clicked;

            mClose = new Button(mContainer, "CloseButton")
            {
                Text = Strings.EscapeMenu.Close
            };

            mClose.Clicked += Close_Clicked;

            mContainer.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            if (Options.Player.MaxCharacters <= 1)
            {
                mGoToCharacterSelect.IsDisabled = true;
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
            if (Interface.GameUi != null && Interface.GameUi.GameCanvas != null)
            {
                Interface.GameUi.GameCanvas.MouseInputEnabled = false;
                Interface.GameUi.GameCanvas.MouseInputEnabled = true;
            }
        }

        public void Update()
        {
            if (!IsHidden)
            {
                BringToFront();
            }

            mGoToCharacterSelect.IsDisabled = Globals.Me?.CombatTimer > Globals.System.GetTimeMs();
        }

        private void Options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mOptionsWindow.Show();
            Interface.GameUi?.EscapeMenu?.Hide();
        }

        public void OpenSettings()
        {
            mOptionsWindow.Show();
            Interface.GameUi?.EscapeMenu?.Hide();
        }

        /// <inheritdoc />
        public override void ToggleHidden()
        {
            if (mOptionsWindow.IsVisible())
            {
                return;
            }

            base.ToggleHidden();
        }

        private void LogoutToCharacterSelect(object sender, EventArgs e)
        {
            if (Globals.Me != null)
            {
                Globals.Me.CombatTimer = 0;
            }

            Main.Logout(true);
        }

        private void LogoutToMainMenu(object sender, EventArgs e)
        {
            if (Globals.Me != null)
            {
                Globals.Me.CombatTimer = 0;
            }

            Main.Logout(false);
        }

        private void ExitToDesktop(object sender, EventArgs e)
        {
            if (Globals.Me != null)
            {
                Globals.Me.CombatTimer = 0;
            }

            Globals.IsRunning = false;
        }

        private void GoToCharacterSelect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ToggleHidden();
            if (Globals.Me.CombatTimer > Globals.System.GetTimeMs())
            {
                //Show Logout in Combat Warning
                var box = new InputBox(
                    Strings.Combat.warningtitle, Strings.Combat.warningcharacterselect, true, InputBox.InputType.YesNo,
                    LogoutToCharacterSelect, null, null
                );
            }
            else
            {
                LogoutToCharacterSelect(null, null);
            }
        }

        private void Logout_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ToggleHidden();
            if (Globals.Me.CombatTimer > Globals.System.GetTimeMs())
            {
                //Show Logout in Combat Warning
                var box = new InputBox(
                    Strings.Combat.warningtitle, Strings.Combat.warninglogout, true, InputBox.InputType.YesNo,
                    LogoutToMainMenu, null, null
                );
            }
            else
            {
                LogoutToMainMenu(null, null);
            }
        }

        private void ExitToDesktop_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ToggleHidden();
            if (Globals.Me.CombatTimer > Globals.System.GetTimeMs())
            {
                //Show Logout in Combat Warning
                var box = new InputBox(
                    Strings.Combat.warningtitle, Strings.Combat.warningexitdesktop, true, InputBox.InputType.YesNo,
                    ExitToDesktop, null, null
                );
            }
            else
            {
                ExitToDesktop(null, null);
            }
        }

        private void Close_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
        }

    }

}
