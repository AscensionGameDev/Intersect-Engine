using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;

namespace Intersect.Client.Interface.Game
{
    public class PartyMemberRow
    {
        private ImagePanel mMemberContainer;

        private ImagePanel mSpriteContainer;

        private ImagePanel mSprite;

        private ImagePanel mHealthContainer;

        private ImagePanel mHealth;

        private ImagePanel mShield;

        private Label mHealthLabel;

        private ImagePanel mManaContainer;

        private ImagePanel mMana;

        private Label mManaLabel;

        private ImagePanel mLeaderImage;

        private Button mKickButton;

        private Label mNameLabel;

        private ImagePanel mStatusContainer;

        private Base mParent;

        private GameRenderTexture mCurrentTex;

        public int Index { get; private set; }

        public PartyMemberRow(Base parent, int index)
        {
            mParent = parent;
            Index = index;

            GenerateControls();
        }

        private void GenerateControls()
        {
            mMemberContainer = new ImagePanel(mParent, "PartyMember");
            mSpriteContainer = new ImagePanel(mMemberContainer, "SpriteContainer");
            mSprite = new ImagePanel(mSpriteContainer, "Sprite");
            mNameLabel = new Label(mMemberContainer, "Name");
            mHealthContainer = new ImagePanel(mMemberContainer, "HealthContainer");
            mHealth = new ImagePanel(mHealthContainer, "Health");
            mShield = new ImagePanel(mHealthContainer, "Shield");
            mHealthLabel = new Label(mHealthContainer, "HealthLabel");
            mManaContainer = new ImagePanel(mMemberContainer, "ManaContainer");
            mMana = new ImagePanel(mManaContainer, "Mana");
            mManaLabel = new Label(mManaContainer, "ManaLabel");
            mLeaderImage = new ImagePanel(mMemberContainer, "LeaderImage");
            mKickButton = new Button(mMemberContainer, "KickButton");
            mStatusContainer = new ImagePanel(mMemberContainer, "StatusContainer");

            mMemberContainer.Clicked += MMemberContainer_Clicked;
            mKickButton.Clicked += MKickButton_Clicked;

            mMemberContainer.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        private void MKickButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.IsPartyLeader)
            {
                PacketSender.SendPartyKick(Globals.Me.Party[Index].Id);
            }
        }

        private void MMemberContainer_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var entity = GetEntity();
            if (entity != null)
            {
                Globals.Me.TryTarget(entity);
            }
        }

        public void Update()
        {
            if (!mMemberContainer.IsVisible)
            {
                return;
            }

            if (!Globals.Me.IsInParty() || Globals.Me.Party.Count < Index)
            {
                return;
            }

            UpdateName();
            UpdateSprite();
            UpdateVitalBars();

            // Set up leader and kick buttons.. Index 0 is always party leader!
            if (Index == 0)
            {
                mKickButton.Hide();
                mLeaderImage.Show();
            }
            else
            {
                mLeaderImage.Hide();
                if (Globals.Me.IsPartyLeader)
                {
                    mKickButton.Show();
                }
                else
                {
                    mKickButton.Hide();
                }
            }

            // TODO: Show/hide kick button based on context menu enabled. Needs https://github.com/AscensionGameDev/Intersect-Engine/pull/959
        }

        private void UpdateName()
        {
            mNameLabel.SetText(Strings.Parties.Name.ToString(Globals.Me.Party[Index].Name, Globals.Me.Party[Index].Level));
        }

        private void UpdateSprite()
        {
            var entity = GetEntity();
            if (entity != null)
            {
                // TODO: Add sprite renderer
            }
            else
            {
                mSprite.Hide();
            }
        }

        private void UpdateVitalBars()
        {
            var health = Globals.Me.Party[Index].Vital[(int) Vitals.Health];
            var maxHealth = Globals.Me.Party[Index].MaxVital[(int) Vitals.Health];
            var mana = Globals.Me.Party[Index].Vital[(int) Vitals.Mana];
            var maxMana = Globals.Me.Party[Index].Vital[(int) Vitals.Mana];

            var healthSize = (int) Math.Floor(mHealthContainer.Width * ((float) health / maxHealth));
            var manaSize = (int) Math.Floor(mManaContainer.Width * ((float)mana / maxMana));

            mHealth.SetSize(healthSize, mHealthContainer.Height);
            mHealthLabel.SetText(Strings.Parties.Health.ToString(health, maxHealth));

            mMana.SetSize(manaSize, mManaContainer.Height);
            mManaLabel.SetText(Strings.Parties.Mana.ToString(mana, maxMana));

            var entity = GetEntity();
            if (entity != null)
            {
                var shield = entity.GetShieldSize();
                if (shield > 0)
                {
                    if (shield < maxHealth)
                    {
                        var scaleFactor = (float)shield / maxHealth;
                        var shieldSize = (int)Math.Floor(mHealthContainer.Width * scaleFactor);

                        if (shieldSize == 0)
                        {
                            mShield.Hide();
                        }
                        else
                        {
                            mShield.SetSize(shieldSize, mHealthContainer.Height);
                            mShield.SetPosition(mHealthContainer.Width - mShield.Width, mShield.Y);

                            var width = (int)Math.Floor(mShield.Texture.Width * scaleFactor);
                            mShield.SetTextureRect(mShield.Texture.Width - width, 0, width, mShield.Texture.Height);
                            mShield.Show();
                        }
                    }
                    else if (shield >= maxHealth)
                    {
                        mShield.SetSize(mHealthContainer.Width, mHealthContainer.Height);
                        mShield.SetPosition(0, mShield.Y);

                        mShield.SetTextureRect(0, 0, mShield.Texture.Width, mShield.Texture.Height);
                        mShield.Show();
                    }

                }
                else
                {
                    mShield.Hide();
                }
            }
            else
            {
                mShield.Hide();
            }
        }

        private Entity GetEntity()
        {
            if (!Globals.Me.IsInParty() || Globals.Me.Party.Count < Index)
            {
                return default;
            }

            Globals.Entities.TryGetValue(Globals.Me.Party[Index].Id, out var entity);
            return entity;
        }

        public void SetPosition(int x, int y) => mMemberContainer.SetPosition(x, y);

        public int Height => mMemberContainer.Height;

        public int Width => mMemberContainer.Width; 

        public void Hide() => mMemberContainer.Hide();

        public void Show() => mMemberContainer.Show();

        public bool IsVisible => mMemberContainer.IsVisible;

        public void Dispose()
        {
            mParent.RemoveChild(mMemberContainer, true);
        }
    }
}
