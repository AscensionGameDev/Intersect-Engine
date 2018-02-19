using System;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using JetBrains.Annotations;
using UIColor = IntersectClientExtras.GenericClasses.Color;

namespace Intersect.Client.Classes.UI.Game
{
    public class IngameMenuWindow : ImagePanel
    {
        [NotNull] private readonly ImagePanel mContainer;

        [NotNull] private readonly Label mTitle;

        [NotNull] private readonly OptionsWindow mOptionsWindow;

        [NotNull] private readonly Button mOptions;
        [NotNull] private readonly Button mGoToCharacterSelect;
        [NotNull] private readonly Button mLogout;
        [NotNull] private readonly Button mExitToDesktop;
        [NotNull] private readonly Button mClose;

        public bool GoToCharacterSelectEnabled => false;
        public bool LogoutEnabled => false;

        public IngameMenuWindow([NotNull] Canvas gameCanvas)
            : base(gameCanvas, "IngameMenuWindow")
        {
            Gui.InputBlockingElements?.Add(this);

            Width = 600;
            Height = gameCanvas.Height;

            AddAlignment(Alignments.CenterH);
            ProcessAlignments();

            mContainer = new ImagePanel(this, "IngameMenu")
            {
                BoundsOutlineColor = UIColor.Red,
                Width = 600,
                Height = Height,
                Texture = GameContentManager.Current.GetTexture(GameContentManager.TextureType.Gui, "ingamemenubg.png")
            };

            mTitle = new Label(mContainer, "TitleLabel")
            {
                AutoSizeToContents = true,
                Font = GameContentManager.Current?.GetFont("arial", 24),
                Padding = new Padding(0, 60, 0, 0),
                Text = Strings.IngameMenu.Title,
            };
            mTitle.SetTextColor(new UIColor(255, 255, 255, 255), Label.ControlState.Normal);
            mTitle.AddAlignment(Alignments.CenterH);
            mTitle.AddAlignment(Alignments.Top);
            mTitle.ProcessAlignments();

            var buttonRegionTop = mTitle.Bottom + 120;
            var availableHeight = Height - buttonRegionTop - 60;
            var visibleButtons = 3;
            if (GoToCharacterSelectEnabled) ++visibleButtons;
            if (LogoutEnabled) ++visibleButtons;
            var heightPerButton = availableHeight / visibleButtons;
            var buttonWidth = 280;
            var buttonHeight = 36;

            mOptionsWindow = new OptionsWindow(gameCanvas, null, null);

            mOptions = new Button(mContainer, "OptionsButton")
            {
                Font = GameContentManager.Current?.GetFont("arial", 16),
                Text = Strings.IngameMenu.Options
            };
            mOptions.SetBounds(0, buttonRegionTop  + (heightPerButton * 0) + (heightPerButton - buttonHeight) / 2, buttonWidth, buttonHeight);
            mOptions.SetTextColor(new UIColor(255, 30, 30, 30), Label.ControlState.Normal);
            mOptions.SetImage(null, "controlnormal.png", Button.ControlState.Normal);
            mOptions.SetImage(null, "controlhover.png", Button.ControlState.Hovered);
            mOptions.AddAlignment(Alignments.CenterH);
            mOptions.ProcessAlignments();
            mOptions.Clicked += Options_Clicked;

            mGoToCharacterSelect = new Button(mContainer, "GoToCharacterSelectButton")
            {
                Font = GameContentManager.Current?.GetFont("arial", 16),
                IsHidden = !GoToCharacterSelectEnabled,
                Text = Strings.IngameMenu.GoToCharacterSelect
            };
            mGoToCharacterSelect.SetBounds(0, buttonRegionTop + (heightPerButton * 1) + (heightPerButton - buttonHeight) / 2, buttonWidth, buttonHeight);
            mGoToCharacterSelect.SetTextColor(new UIColor(255, 30, 30, 30), Label.ControlState.Normal);
            mGoToCharacterSelect.SetImage(null, "controlnormal.png", Button.ControlState.Normal);
            mGoToCharacterSelect.SetImage(null, "controlhover.png", Button.ControlState.Hovered);
            mGoToCharacterSelect.AddAlignment(Alignments.CenterH);
            mGoToCharacterSelect.ProcessAlignments();
            mGoToCharacterSelect.Clicked += GoToCharacterSelect_Clicked;

            mLogout = new Button(mContainer, "LogoutButton")
            {
                Font = GameContentManager.Current?.GetFont("arial", 16),
                IsHidden = !LogoutEnabled,
                Text = Strings.IngameMenu.Logout
            };
            mLogout.SetBounds(0, buttonRegionTop + (heightPerButton * (visibleButtons - 3)) + (heightPerButton - buttonHeight) / 2, buttonWidth, buttonHeight);
            mLogout.SetTextColor(new UIColor(255, 30, 30, 30), Label.ControlState.Normal);
            mLogout.SetImage(null, "controlnormal.png", Button.ControlState.Normal);
            mLogout.SetImage(null, "controlhover.png", Button.ControlState.Hovered);
            mLogout.AddAlignment(Alignments.CenterH);
            mLogout.ProcessAlignments();
            mLogout.Clicked += Logout_Clicked;

            mExitToDesktop = new Button(mContainer, "ExitToDesktopButton")
            {
                Font = GameContentManager.Current?.GetFont("arial", 16),
                Text = Strings.IngameMenu.ExitToDesktop
            };
            mExitToDesktop.SetBounds(0, buttonRegionTop + (heightPerButton * (visibleButtons - 2)) + (heightPerButton - buttonHeight) / 2, buttonWidth, buttonHeight);
            mExitToDesktop.SetTextColor(new UIColor(255, 30, 30, 30), Label.ControlState.Normal);
            mExitToDesktop.SetImage(null, "controlnormal.png", Button.ControlState.Normal);
            mExitToDesktop.SetImage(null, "controlhover.png", Button.ControlState.Hovered);
            mExitToDesktop.AddAlignment(Alignments.CenterH);
            mExitToDesktop.ProcessAlignments();
            mExitToDesktop.Clicked += ExitToDesktop_Clicked;

            mClose = new Button(mContainer, "CloseButton")
            {
                Font = GameContentManager.Current?.GetFont("arial", 16),
                Text = Strings.IngameMenu.Close
            };
            mClose.SetBounds(0, buttonRegionTop + (heightPerButton * (visibleButtons - 1)) + (heightPerButton - buttonHeight) / 2, buttonWidth, buttonHeight);
            mClose.SetTextColor(new UIColor(255, 30, 30, 30), Label.ControlState.Normal);
            mClose.SetImage(null, "controlnormal.png", Button.ControlState.Normal);
            mClose.SetImage(null, "controlhover.png", Button.ControlState.Hovered);
            mClose.AddAlignment(Alignments.CenterH);
            mClose.ProcessAlignments();
            mClose.Clicked += Close_Clicked;

            mContainer.LoadJsonUi(GameContentManager.UI.InGame);

            mGoToCharacterSelect.IsHidden = !GoToCharacterSelectEnabled;
            mLogout.IsHidden = !LogoutEnabled;
        }

        public override void Invalidate()
        {
            if (IsHidden) RemoveModal();
            else MakeModal(true);
            base.Invalidate();
        }

        private void Options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mOptionsWindow.Show();
        }

        private static void GoToCharacterSelect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            
        }

        private static void Logout_Clicked(Base sender, ClickedEventArgs arguments)
        {
            
        }

        private static void ExitToDesktop_Clicked(Base sender, ClickedEventArgs arguments) => Globals.IsRunning = false;

        private void Close_Clicked(Base sender, ClickedEventArgs arguments) => Hide();
    }
}
