using System;
using System.Collections.Generic;
using Intersect.Client.Classes.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using JetBrains.Annotations;
using Intersect.Client.Classes.Core;

namespace Intersect.Client.Classes.UI.Game.EntityPanel
{
    public class EntityBox
    {
        private static int sStatusXPadding = 2;
        private static int sStatusYPadding = 2;

        public float CurExpWidth;

        public float CurHpWidth;
        public float CurMpWidth;
        private string mCurrentSprite = "";
        public ImagePanel EntityFace;
        public ImagePanel EntityFaceContainer;
        public ImagePanel EntityInfoPanel;

        [NotNull]
        public readonly Label EntityMap;

        [NotNull]
        public readonly Label EntityLevel;

        [NotNull]
        public readonly Label EntityNameAndLevel;

        [NotNull]
        public readonly Label EntityName;

        public ImagePanel EntityStatusPanel;

        public EntityTypes EntityType;

        //Controls
        [NotNull]
        public readonly ImagePanel EntityWindow;

        public RichLabel EventDesc;
        public ImagePanel ExpBackground;
        public ImagePanel ExpBar;
        public Label ExpLbl;
        public Label ExpTitle;
        public ImagePanel HpBackground;
        public ImagePanel HpBar;
        public Label HpLbl;
        public Label HpTitle;
        private bool mInitialized;
        public ImagePanel MpBackground;
        public ImagePanel MpBar;
        public Label MpLbl;
        public Label MpTitle;

        public Entity MyEntity;
        public ImagePanel[] PaperdollPanels;
        public string[] PaperdollTextures;

        //Spell List
        public List<SpellStatus> Items = new List<SpellStatus>();

        private long mLastUpdateTime;
        public Button PartyLabel;
        public bool PlayerBox;
        public Button TradeLabel;

        public bool UpdateStatuses;

        //Init
        public EntityBox(Canvas gameCanvas, EntityTypes entityType, Entity myEntity, bool playerBox = false)
        {
            MyEntity = myEntity;
            EntityType = entityType;
            PlayerBox = playerBox;
            EntityWindow = playerBox ? new ImagePanel(gameCanvas, "PlayerBox") : new ImagePanel(gameCanvas, "TargetBox");

            EntityInfoPanel = new ImagePanel(EntityWindow, "EntityInfoPanel");

            EntityName = new Label(EntityInfoPanel, "EntityNameLabel") { Text = myEntity?.Name };
            EntityLevel = new Label(EntityInfoPanel, "EntityLevelLabel");
            EntityNameAndLevel = new Label(EntityInfoPanel, "NameAndLevelLabel") { IsHidden = true };
            EntityMap = new Label(EntityInfoPanel, "EntityMapLabel");

            PaperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            PaperdollTextures = new string[Options.EquipmentSlots.Count];
			int i = 0;
			for (int z = 0; z < Options.PaperdollOrder[1].Count; z++)
			{
				if (Options.PaperdollOrder[1][z] == "Player")
				{
					EntityFaceContainer = new ImagePanel(EntityInfoPanel, "EntityGraphicContainer");

					EntityFace = new ImagePanel(EntityFaceContainer);
					EntityFace.SetSize(64, 64);
					EntityFace.AddAlignment(Alignments.Center);
				}
				else
				{
					PaperdollPanels[i] = new ImagePanel(EntityFaceContainer);
					PaperdollTextures[i] = "";
					PaperdollPanels[i].Hide();
					i++;
				}
            }

            EventDesc = new RichLabel(EntityInfoPanel, "EventDescLabel");

            HpBackground = new ImagePanel(EntityInfoPanel, "HPBarBackground");
            HpBar = new ImagePanel(EntityInfoPanel, "HPBar");
            HpTitle = new Label(EntityInfoPanel, "HPTitle");
            HpTitle.SetText(Strings.EntityBox.vital0);
            HpLbl = new Label(EntityInfoPanel, "HPLabel");

            MpBackground = new ImagePanel(EntityInfoPanel, "MPBackground");
            MpBar = new ImagePanel(EntityInfoPanel, "MPBar");
            MpTitle = new Label(EntityInfoPanel, "MPTitle");
            MpTitle.SetText(Strings.EntityBox.vital1);
            MpLbl = new Label(EntityInfoPanel, "MPLabel");

            ExpBackground = new ImagePanel(EntityInfoPanel, "EXPBackground");
            ExpBar = new ImagePanel(EntityInfoPanel, "EXPBar");
            ExpTitle = new Label(EntityInfoPanel, "EXPTitle");
            ExpTitle.SetText(Strings.EntityBox.exp);
            ExpLbl = new Label(EntityInfoPanel, "EXPLabel");

            TradeLabel = new Button(EntityInfoPanel, "TradeButton");
            TradeLabel.SetText(Strings.EntityBox.trade);
            TradeLabel.SetToolTipText(Strings.EntityBox.tradetip.ToString( MyEntity.Name));
            TradeLabel.Clicked += tradeRequest_Clicked;

            PartyLabel = new Button(EntityInfoPanel, "PartyButton");
            PartyLabel.SetText(Strings.EntityBox.party);
            PartyLabel.SetToolTipText(Strings.EntityBox.partytip.ToString(MyEntity.Name));
            PartyLabel.Clicked += invite_Clicked;

            EntityStatusPanel = new ImagePanel(EntityWindow, "StatusArea");

            UpdateSpellStatus();

            SetEntity(myEntity);

            EntityWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());

            mLastUpdateTime = Globals.System.GetTimeMs();
        }

        public void SetEntity(Entity entity)
        {
            MyEntity = entity;
            if (MyEntity != null)
            {
                SetupEntityElements();
            }
        }

        public void SetupEntityElements()
        {
            switch (EntityType)
            {
                case EntityTypes.Player:
                    if (Globals.Me != null && Globals.Me == MyEntity)
                    {
                        TradeLabel.Hide();
                        PartyLabel.Hide();
                    }
                    else
                    {
                        ExpBackground.Hide();
                        ExpBar.Hide();
                        ExpLbl.Hide();
                        ExpTitle.Hide();
                        EntityMap.Hide();
                    }
                    EventDesc.Hide();
                    break;
                case EntityTypes.GlobalEntity:
                    EventDesc.Hide();
                    ExpBackground.Hide();
                    ExpBar.Hide();
                    ExpLbl.Hide();
                    ExpTitle.Hide();
                    TradeLabel.Hide();
                    PartyLabel.Hide();
                    EntityMap.Hide();
                    break;
                case EntityTypes.Event:
                    ExpBackground.Hide();
                    ExpBar.Hide();
                    ExpLbl.Hide();
                    ExpTitle.Hide();
                    MpBackground.Hide();
                    MpBar.Hide();
                    MpTitle.Hide();
                    MpLbl.Hide();
                    HpBackground.Hide();
                    HpBar.Hide();
                    HpLbl.Hide();
                    HpTitle.Hide();
                    TradeLabel.Hide();
                    PartyLabel.Hide();
                    EntityMap.Hide();
                    break;
            }
            EntityName.SetText(MyEntity.Name);
        }

        //Update
        public void Update()
        {
            if (MyEntity == null)
            {
                if (!EntityWindow.IsHidden) EntityWindow.Hide();
                return;
            }
            if (EntityWindow.IsHidden) EntityWindow.Show();
            if (MyEntity.IsDisposed()) Dispose();
            if (!mInitialized)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (EntityType == EntityTypes.Event)
                {
                    EventDesc.AddText(((Event) MyEntity).Desc, IntersectClientExtras.GenericClasses.Color.White);
                    EventDesc.SizeToChildren(false, true);
                }
                mInitialized = true;
            }

            //Time since this window was last updated (for bar animations)
            float elapsedTime = (Globals.System.GetTimeMs() - mLastUpdateTime) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            if (EntityType != EntityTypes.Event)
            {
                UpdateLevel();
                UpdateMap();
                UpdateHpBar(elapsedTime);
                UpdateMpBar(elapsedTime);
            }

            //If player draw exp bar
            if (MyEntity == Globals.Me)
            {
                UpdateXpBar(elapsedTime);
            }

            if (UpdateStatuses)
            {
                UpdateSpellStatus();
                UpdateStatuses = false;
            }

            //Update each status item
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Update();
            }

            mLastUpdateTime = Globals.System.GetTimeMs();
        }

        public void UpdateSpellStatus()
        {
            foreach (SpellStatus s in Items)
            {
                s.Pnl.Texture = null;
                s.Container.Texture = null;
                EntityStatusPanel.RemoveChild(s.Container, true);
                s.pnl_HoverLeave(null, null);
            }
            Items.Clear();

            //Add all of the spell status effects
            for (int i = 0; i < MyEntity.Status.Count; i++)
            {
                Items.Add(new SpellStatus(this, i));
                if (PlayerBox)
                {
                    Items[i].Container = new ImagePanel(EntityStatusPanel, "PlayerStatusIcon");
                }
                else
                {
                    Items[i].Container = new ImagePanel(EntityStatusPanel, "TargetStatusIcon");
                }
                Items[i].Setup();
                
                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
                Items[i].Container.Name = "";

                var xPadding = Items[i].Container.Padding.Left + Items[i].Container.Padding.Right;
                var yPadding = Items[i].Container.Padding.Top + Items[i].Container.Padding.Bottom;
                Items[i].Container.SetPosition(
                    (i % (EntityStatusPanel.Width / (float)(Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Width + xPadding) + xPadding,
                    (i / (EntityStatusPanel.Width / (float)(Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Height + yPadding) + yPadding);
            }
        }

        private void UpdateLevel()
        {
            var levelString = Strings.EntityBox.level.ToString(MyEntity.Level);
            if (!EntityLevel.IsHidden) EntityLevel.Text = levelString;
            if (!EntityNameAndLevel.IsHidden) EntityNameAndLevel.Text = Strings.EntityBox.NameAndLevel.ToString(MyEntity.Name, levelString);
        }

        private void UpdateMap()
        {
            if (Globals.Me.MapInstance != null)
            {
                EntityMap.SetText(Strings.EntityBox.map.ToString(Globals.Me.MapInstance.Name));
            }
            else
            {
                EntityMap.SetText(Strings.EntityBox.map.ToString(""));
            }
        }

        private void UpdateHpBar(float elapsedTime)
        {
            float targetHpWidth = 0f;
            if (MyEntity.MaxVital[(int) Vitals.Health] > 0)
            {
                targetHpWidth = (MyEntity.Vital[(int) Vitals.Health] /
                                 (float) MyEntity.MaxVital[(int) Vitals.Health]);
                targetHpWidth = Math.Min(1, Math.Max(0, targetHpWidth));
                //Fix the Labels
                HpLbl.Text = Strings.EntityBox.vital0val.ToString(MyEntity.Vital[(int) Vitals.Health],
                    MyEntity.MaxVital[(int) Vitals.Health]);
                //Multiply by the width of the bars.
                targetHpWidth *= HpBackground.Width;
            }
            else
            {
                HpLbl.Text = Strings.EntityBox.vital0val.ToString( 0, 0);
                targetHpWidth = HpBackground.Width;
            }
            if ((int) targetHpWidth != CurHpWidth)
            {
                if ((int) targetHpWidth > CurHpWidth)
                {
                    CurHpWidth += (100f * elapsedTime);
                    if (CurHpWidth > (int) targetHpWidth)
                    {
                        CurHpWidth = targetHpWidth;
                    }
                }
                else
                {
                    CurHpWidth -= (100f * elapsedTime);
                    if (CurHpWidth < targetHpWidth)
                    {
                        CurHpWidth = targetHpWidth;
                    }
                }
                if (CurHpWidth == 0)
                {
                    HpBar.IsHidden = true;
                }
                else
                {
                    HpBar.Width = (int) CurHpWidth;
                    HpBar.SetTextureRect(0, 0, (int) CurHpWidth, HpBar.Height);
                    HpBar.IsHidden = false;
                }
            }
        }

        private void UpdateMpBar(float elapsedTime)
        {
            float targetMpWidth = 0f;
            if (MyEntity.MaxVital[(int) Vitals.Mana] > 0)
            {
                targetMpWidth = (MyEntity.Vital[(int) Vitals.Mana] /
                                 (float) MyEntity.MaxVital[(int) Vitals.Mana]);
                targetMpWidth = Math.Min(1, Math.Max(0, targetMpWidth));
                MpLbl.Text = Strings.EntityBox.vital1val.ToString(MyEntity.Vital[(int) Vitals.Mana],
                    MyEntity.MaxVital[(int) Vitals.Mana]);
                targetMpWidth *= MpBackground.Width;
            }
            else
            {
                MpLbl.Text = Strings.EntityBox.vital1val.ToString(0, 0);
                targetMpWidth = MpBackground.Width;
            }
            if ((int) targetMpWidth != CurMpWidth)
            {
                if ((int) targetMpWidth > CurMpWidth)
                {
                    CurMpWidth += (100f * elapsedTime);
                    if (CurMpWidth > (int) targetMpWidth)
                    {
                        CurMpWidth = targetMpWidth;
                    }
                }
                else
                {
                    CurMpWidth -= (100f * elapsedTime);
                    if (CurMpWidth < targetMpWidth)
                    {
                        CurMpWidth = targetMpWidth;
                    }
                }
                if (CurMpWidth == 0)
                {
                    MpBar.IsHidden = true;
                }
                else
                {
                    MpBar.Width = (int) CurMpWidth;
                    MpBar.SetTextureRect(0, 0, (int) CurMpWidth, MpBar.Height);
                    MpBar.IsHidden = false;
                }
            }
        }

        private void UpdateXpBar(float elapsedTime)
        {
            float targetExpWidth = 1;
            if (((Player) MyEntity).GetNextLevelExperience() > 0)
            {
                targetExpWidth = (float) ((Player) MyEntity).Experience /
                                 (float) ((Player) MyEntity).GetNextLevelExperience();
                ExpLbl.Text = Strings.EntityBox.expval.ToString(((Player) MyEntity)?.Experience,
                    ((Player) MyEntity)?.GetNextLevelExperience());
            }
            else
            {
                targetExpWidth = 1f;
                ExpLbl.Text = Strings.EntityBox.maxlevel;
            }
            targetExpWidth *= ExpBackground.Width;
            if (Math.Abs((int) targetExpWidth - CurExpWidth) < 0.01) return;
            if ((int) targetExpWidth > CurExpWidth)
            {
                CurExpWidth += (100f * elapsedTime);
                if (CurExpWidth > (int) targetExpWidth)
                {
                    CurExpWidth = targetExpWidth;
                }
            }
            else
            {
                CurExpWidth -= (100f * elapsedTime);
                if (CurExpWidth < targetExpWidth)
                {
                    CurExpWidth = targetExpWidth;
                }
            }
            if (CurExpWidth == 0)
            {
                ExpBar.IsHidden = true;
            }
            else
            {
                ExpBar.Width = (int) CurExpWidth;
                ExpBar.SetTextureRect(0, 0, (int) CurExpWidth, ExpBar.Height);
                ExpBar.IsHidden = false;
            }
        }

        private void UpdateImage()
        {
            GameTexture faceTex =
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, MyEntity.Face);
            GameTexture entityTex =
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MyEntity.MySprite);
            if (faceTex != null && faceTex != EntityFace.Texture)
            {
                EntityFace.Texture = faceTex;
                EntityFace.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                EntityFace.SizeToContents();
                Align.Center(EntityFace);
                mCurrentSprite = MyEntity.Face;
                EntityFace.IsHidden = false;
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    PaperdollPanels[i].Hide();
                }
            }
            else if (entityTex != null && faceTex == null || (faceTex != null && faceTex != EntityFace.Texture))
            {
                if (entityTex != EntityFace.Texture)
                {
                    EntityFace.Texture = entityTex;
                    EntityFace.SetTextureRect(0, 0, entityTex.GetWidth() / 4, entityTex.GetHeight() / 4);
                    EntityFace.SizeToContents();
                    Align.Center(EntityFace);
                    mCurrentSprite = MyEntity.MySprite;
                    EntityFace.IsHidden = false;
                }
                var equipment = MyEntity.Equipment;
                if (MyEntity == Globals.Me)
                {
                    for (int i = 0; i < MyEntity.MyEquipment.Length; i++)
                    {
                        var eqp = MyEntity.MyEquipment[i];
                        if (eqp > -1 && eqp < Options.MaxInvItems)
                        {
                            equipment[i] = MyEntity.Inventory[eqp].ItemId;
                        }
                        else
                        {
                            equipment[i] = Guid.Empty;
                        }
                    }
                }
                
				int n = 0;
                for (int z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
					var paperdoll = "";
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1 &&
                        equipment.Length == Options.EquipmentSlots.Count)
                    {
                        if (equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] != Guid.Empty)
                        {
                            var itemId = equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])];
                            if (ItemBase.Get(itemId) != null)
                            {
                                var itemdata = ItemBase.Get(itemId);
                                if (MyEntity.Gender == 0)
                                {
                                    paperdoll = itemdata.MalePaperdoll;
                                }
                                else
                                {
                                    paperdoll = itemdata.FemalePaperdoll;
                                }
                            }
                        }
                    }

					//Check for Player layer
					if (Options.PaperdollOrder[1][z] == "Player")
					{
						continue;
					}

					if (paperdoll == "" && PaperdollTextures[n] != "")
                    {
                        PaperdollPanels[n].Texture = null;
                        PaperdollPanels[n].Hide();
                        PaperdollTextures[n] = "";
                    }
                    else if (paperdoll != "" && paperdoll != PaperdollTextures[n])
                    {
                        var paperdollTex =
                            Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll, paperdoll);
                        PaperdollPanels[n].Texture = paperdollTex;
                        if (paperdollTex != null)
                        {
                            PaperdollPanels[n].SetTextureRect(0, 0,
                                PaperdollPanels[n].Texture.GetWidth() / 4,
                                PaperdollPanels[n].Texture.GetHeight() / 4);
                            PaperdollPanels[n].SetSize(PaperdollPanels[n].Texture.GetWidth() / 4,
                                PaperdollPanels[n].Texture.GetHeight() / 4);
                            PaperdollPanels[n].SetPosition(
                                EntityFaceContainer.Width / 2 - PaperdollPanels[n].Width / 2,
                                EntityFaceContainer.Height / 2 - PaperdollPanels[n].Height / 2);
                        }
                        PaperdollPanels[n].Show();
                        PaperdollTextures[n] = paperdoll;
                    }

					//Check for Player layer
					if (Options.PaperdollOrder[1][z] != "Player")
					{
						n++;
					}
				}
            }
            else if (MyEntity.MySprite != mCurrentSprite && MyEntity.Face != mCurrentSprite)
            {
                EntityFace.IsHidden = true;
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    PaperdollPanels[i].Hide();
                }
            }
        }

        public void Dispose()
        {
            EntityWindow.Hide();
            Gui.GameUi.GameCanvas.RemoveChild(EntityWindow, false);
            EntityWindow.Dispose();
        }

        //Input Handlers
        void invite_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                PacketSender.SendPartyInvite(Globals.Me.TargetIndex);
            }
        }

        //Input Handlers
        void tradeRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                PacketSender.SendTradeRequest(Globals.Me.TargetIndex);
            }
        }
    }
}