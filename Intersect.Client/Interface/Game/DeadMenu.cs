using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;

using JetBrains.Annotations;

namespace Intersect.Client.Interface.Game
{

    public class DeadMenu : ImagePanel
    {
        [NotNull] private readonly ImagePanel mContainer;

        [NotNull] private readonly Button mRespawn;

        [NotNull] private readonly Label mTitle;

        [NotNull] private readonly Label mSec;

        public DeadMenu([NotNull] Canvas gameCanvas) : base(gameCanvas, "DeadMenu")
        {
            Interface.InputBlockingElements?.Add(this);

            Width = gameCanvas.Width;
            Height = gameCanvas.Height;

            mContainer = new ImagePanel(this, "DeadMenu");

            mTitle = new Label(mContainer, "TitleLabel")
            {
                Text = Strings.DeadMenu.Title,
            };

            mSec = new Label(mContainer, "SecLabel")
            {
                Text = ""
            };

            mRespawn = new Button(mContainer, "Respawn")
            {
                Text = Strings.DeadMenu.Respawn
            };

            mRespawn.Clicked += GoHospital_Clicked;

            mContainer.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
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
            if (Globals.Me.IsDead)
            {
                mSec.Text = (Globals.Me.DeathCounter + 1).ToString() + "...";
                mRespawn.IsDisabled = Globals.Me.DeathCounter >= 55;
            }
            
            if (!IsHidden)
            {
                BringToFront();
            }
        }

        private void GoHospital_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.FinalDeath(true);
        }
    }
}
