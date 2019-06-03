using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using JetBrains.Annotations;

namespace Intersect.Client.UI.Game
{
    public class EscapeMenu : ImagePanel
    {
        [NotNull] private readonly ImagePanel mContainer;

        [NotNull] private readonly Label mTitle;

        [NotNull] private readonly OptionsWindow mOptionsWindow;

        [NotNull] private readonly Button mOptions;
        [NotNull] private readonly Button mGoToCharacterSelect;
        [NotNull] private readonly Button mLogout;
        [NotNull] private readonly Button mExitToDesktop;
        [NotNull] private readonly Button mClose;

        public EscapeMenu([NotNull] Canvas gameCanvas) : base(gameCanvas, "EscapeMenu")
        {
            Gui.InputBlockingElements?.Add(this);

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

        
            mContainer.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());

            if (Options.Player.MaxCharacters <= 1)
            {
                mGoToCharacterSelect.IsDisabled = true;
            }
        }

        public override void Invalidate()
        {
            if (IsHidden) RemoveModal();
            else MakeModal(true);
            base.Invalidate();
            if (Gui.GameUi != null && Gui.GameUi.GameCanvas != null)
            {
                Gui.GameUi.GameCanvas.MouseInputEnabled = false;
                Gui.GameUi.GameCanvas.MouseInputEnabled = true;
            }
        }

        public void Update()
        {
            if (!IsHidden) BringToFront();
        }

        private void Options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mOptionsWindow.Show();
            Gui.GameUi?.EscapeMenu?.Hide();
        }

        public void OpenSettings()
        {
            mOptionsWindow.Show();
            Gui.GameUi?.EscapeMenu?.Hide();
        }

        private static void GoToCharacterSelect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameMain.Logout(true);
        }

        private static void Logout_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameMain.Logout(false);
        }

        private static void ExitToDesktop_Clicked(Base sender, ClickedEventArgs arguments) => Globals.IsRunning = false;

        private void Close_Clicked(Base sender, ClickedEventArgs arguments) => Hide();
    }
}
