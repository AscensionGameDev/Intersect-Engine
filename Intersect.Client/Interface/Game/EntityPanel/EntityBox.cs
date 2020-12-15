using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;

using JetBrains.Annotations;

namespace Intersect.Client.Interface.Game.EntityPanel
{

    public class EntityBox
    {

        private static int sStatusXPadding = 2;

        private static int sStatusYPadding = 2;

        [NotNull] public readonly Framework.Gwen.Control.Label EntityLevel;

        [NotNull] public readonly Framework.Gwen.Control.Label EntityMap;

        [NotNull] public readonly Framework.Gwen.Control.Label EntityName;

        [NotNull] public readonly Framework.Gwen.Control.Label EntityNameAndLevel;

        //Controls
        [NotNull] public readonly ImagePanel EntityWindow;

        public float CurExpWidth = -1;

        public float CurHpWidth = -1;

        public float CurMpWidth = -1;

        public float CurShieldWidth = -1;

        public ImagePanel EntityFace;

        public ImagePanel EntityFaceContainer;

        public ImagePanel EntityInfoPanel;

        public ImagePanel EntityStatusPanel;

        public EntityTypes EntityType;

        public RichLabel EventDesc;

        public ImagePanel ExpBackground;

        public ImagePanel ExpBar;

        public Framework.Gwen.Control.Label ExpLbl;

        public Framework.Gwen.Control.Label ExpTitle;

        public Button FriendLabel;

        public ImagePanel HpBackground;

        public ImagePanel HpBar;

        public Framework.Gwen.Control.Label HpLbl;

        public Framework.Gwen.Control.Label HpTitle;

        private Dictionary<Guid, SpellStatus> mActiveStatuses = new Dictionary<Guid, SpellStatus>();

        private string mCurrentSprite = "";

        private bool mInitialized;

        private long mLastUpdateTime;

        public ImagePanel MpBackground;

        public ImagePanel MpBar;

        public Framework.Gwen.Control.Label MpLbl;

        public Framework.Gwen.Control.Label MpTitle;

        public Entity MyEntity;

        public ImagePanel[] PaperdollPanels;

        public string[] PaperdollTextures;

        public Button PartyLabel;

        public bool PlayerBox;

        public ImagePanel ShieldBar;

        public Button TradeLabel;

        public bool UpdateStatuses;

        //Init
        public EntityBox(Canvas gameCanvas, EntityTypes entityType, Entity myEntity, bool playerBox = false)
        {
            MyEntity = myEntity;
            EntityType = entityType;
            PlayerBox = playerBox;
            EntityWindow =
                playerBox ? new ImagePanel(gameCanvas, "PlayerBox") : new ImagePanel(gameCanvas, "TargetBox");

            EntityWindow.ShouldCacheToTexture = true;

            EntityInfoPanel = new ImagePanel(EntityWindow, "EntityInfoPanel");

            EntityName = new Framework.Gwen.Control.Label(EntityInfoPanel, "EntityNameLabel") {Text = myEntity?.Name};
            EntityLevel = new Framework.Gwen.Control.Label(EntityInfoPanel, "EntityLevelLabel");
            EntityNameAndLevel = new Framework.Gwen.Control.Label(EntityInfoPanel, "NameAndLevelLabel")
                {IsHidden = true};

            EntityMap = new Framework.Gwen.Control.Label(EntityInfoPanel, "EntityMapLabel");

            PaperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            PaperdollTextures = new string[Options.EquipmentSlots.Count];
            var i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
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
            ShieldBar = new ImagePanel(EntityInfoPanel, "ShieldBar");
            HpTitle = new Framework.Gwen.Control.Label(EntityInfoPanel, "HPTitle");
            HpTitle.SetText(Strings.EntityBox.vital0);
            HpLbl = new Framework.Gwen.Control.Label(EntityInfoPanel, "HPLabel");

            MpBackground = new ImagePanel(EntityInfoPanel, "MPBackground");
            MpBar = new ImagePanel(EntityInfoPanel, "MPBar");
            MpTitle = new Framework.Gwen.Control.Label(EntityInfoPanel, "MPTitle");
            MpTitle.SetText(Strings.EntityBox.vital1);
            MpLbl = new Framework.Gwen.Control.Label(EntityInfoPanel, "MPLabel");

            ExpBackground = new ImagePanel(EntityInfoPanel, "EXPBackground");
            ExpBar = new ImagePanel(EntityInfoPanel, "EXPBar");
            ExpTitle = new Framework.Gwen.Control.Label(EntityInfoPanel, "EXPTitle");
            ExpTitle.SetText(Strings.EntityBox.exp);
            ExpLbl = new Framework.Gwen.Control.Label(EntityInfoPanel, "EXPLabel");

            TradeLabel = new Button(EntityInfoPanel, "TradeButton");
            TradeLabel.SetText(Strings.EntityBox.trade);
            TradeLabel.SetToolTipText(Strings.EntityBox.tradetip.ToString(MyEntity.Name));
            TradeLabel.Clicked += tradeRequest_Clicked;

            PartyLabel = new Button(EntityInfoPanel, "PartyButton");
            PartyLabel.SetText(Strings.EntityBox.party);
            PartyLabel.SetToolTipText(Strings.EntityBox.partytip.ToString(MyEntity.Name));
            PartyLabel.Clicked += invite_Clicked;

            FriendLabel = new Button(EntityInfoPanel, "FriendButton");
            FriendLabel.SetText(Strings.EntityBox.friend);
            FriendLabel.SetToolTipText(Strings.EntityBox.friendtip.ToString(MyEntity.Name));
            FriendLabel.Clicked += friendRequest_Clicked;
            FriendLabel.IsHidden = true;

            EntityStatusPanel = new ImagePanel(EntityWindow, "StatusArea");

            SetEntity(myEntity);

            EntityWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            UpdateSpellStatus();

            i = 0;
            for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
            {
                if (Options.PaperdollOrder[1][z] == "Player")
                {
                    EntityFace.RenderColor = EntityFaceContainer.RenderColor;
                }
                else
                {
                    PaperdollPanels[i].RenderColor = EntityFaceContainer.RenderColor;
                    i++;
                }
            }

            EntityWindow.Hide();

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
                        FriendLabel.Hide();
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
                    FriendLabel.Hide();
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
                    FriendLabel.Hide();
                    EntityMap.Hide();

                    break;
            }

            EntityName.SetText(MyEntity.Name);
            ShieldBar.Hide();
        }

        //Update
        public void Update()
        {
            if (MyEntity == null)
            {
                if (!EntityWindow.IsHidden)
                {
                    EntityWindow.Hide();
                }

                return;
            }

            if (EntityWindow.IsHidden)
            {
                EntityWindow.Show();
            }

            if (MyEntity.IsDisposed())
            {
                Dispose();
            }

            if (!mInitialized)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (EntityType == EntityTypes.Event)
                {
                    EventDesc.AddText(((Event) MyEntity).Desc, Color.White);
                    EventDesc.SizeToChildren(false, true);
                }

                mInitialized = true;
            }

            //Time since this window was last updated (for bar animations)
            var elapsedTime = (Globals.System.GetTimeMs() - mLastUpdateTime) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            if (EntityType != EntityTypes.Event)
            {
                UpdateLevel();
                UpdateMap();
                UpdateHpBar(elapsedTime);
                UpdateMpBar(elapsedTime);
            }
            else
            {
                if (!EntityNameAndLevel.IsHidden)
                {
                    EntityNameAndLevel.Text = MyEntity.Name;
                }
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

            foreach (var itm in mActiveStatuses)
            {
                itm.Value.Update();
            }

            mLastUpdateTime = Globals.System.GetTimeMs();
        }

        public void UpdateSpellStatus()
        {
            //This is shit code that should be removed asap
            //foreach (SpellStatus s in Items)
            //{
            //    s.Pnl.Texture = null;
            //    s.Container.Hide();
            //    s.Container.Texture = null;
            //    EntityStatusPanel.RemoveChild(s.Container, true);
            //    s.pnl_HoverLeave(null, null);
            //}
            //Items.Clear();

            //Remove 'Dead' Statuses
            var statuses = mActiveStatuses.Keys.ToArray();
            foreach (var status in statuses)
            {
                if (!MyEntity.StatusActive(status))
                {
                    var s = mActiveStatuses[status];
                    s.Pnl.Texture = null;
                    s.Container.Hide();
                    s.Container.Texture = null;
                    EntityStatusPanel.RemoveChild(s.Container, true);
                    s.pnl_HoverLeave(null, null);
                    mActiveStatuses.Remove(status);
                }
                else
                {
                    mActiveStatuses[status].UpdateStatus(MyEntity.GetStatus(status));
                }
            }

            //Add all of the spell status effects
            for (var i = 0; i < MyEntity.Status.Count; i++)
            {
                var id = MyEntity.Status[i].SpellId;
                SpellStatus itm = null;
                if (!mActiveStatuses.ContainsKey(id))
                {
                    itm = new SpellStatus(this, MyEntity.Status[i]);
                    if (PlayerBox)
                    {
                        itm.Container = new ImagePanel(EntityStatusPanel, "PlayerStatusIcon");
                    }
                    else
                    {
                        itm.Container = new ImagePanel(EntityStatusPanel, "TargetStatusIcon");
                    }

                    itm.Setup();

                    itm.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                    itm.Container.Name = "";
                    mActiveStatuses.Add(id, itm);
                }
                else
                {
                    itm = mActiveStatuses[id];
                }

                var xPadding = itm.Container.Margin.Left + itm.Container.Margin.Right;
                var yPadding = itm.Container.Margin.Top + itm.Container.Margin.Bottom;

                itm.Container.SetPosition(
                    i %
                    (EntityStatusPanel.Width /
                     Math.Max(1, EntityStatusPanel.Width / (itm.Container.Width + xPadding))) *
                    (itm.Container.Width + xPadding) +
                    xPadding,
                    i /
                    Math.Max(1, EntityStatusPanel.Width / (itm.Container.Width + xPadding)) *
                    (itm.Container.Height + yPadding) +
                    yPadding
                );
            }
        }

        private void UpdateLevel()
        {
            var levelString = Strings.EntityBox.level.ToString(MyEntity.Level);
            if (!EntityLevel.IsHidden)
            {
                EntityLevel.Text = levelString;
            }

            if (!EntityNameAndLevel.IsHidden)
            {
                EntityNameAndLevel.Text = Strings.EntityBox.NameAndLevel.ToString(MyEntity.Name, levelString);
            }
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
            var targetHpWidth = 0f;
            var targetShieldWidth = 0f;
            if (MyEntity.MaxVital[(int) Vitals.Health] > 0)
            {
                var maxVital = MyEntity.MaxVital[(int) Vitals.Health];
                var shieldSize = 0;

                //Check for shields
                foreach (var status in MyEntity.Status)
                {
                    if (status.Type == StatusTypes.Shield)
                    {
                        shieldSize += status.Shield[(int) Vitals.Health];
                    }
                }

                if (shieldSize + MyEntity.Vital[(int) Vitals.Health] > maxVital)
                {
                    maxVital = shieldSize + MyEntity.Vital[(int) Vitals.Health];
                }

                var width = HpBackground.Width;

                var hpfillRatio = (float) MyEntity.Vital[(int) Vitals.Health] / maxVital;
                hpfillRatio = Math.Min(1, Math.Max(0, hpfillRatio));
                targetHpWidth = (float) Math.Ceiling(hpfillRatio * width);

                var shieldfillRatio = (float) shieldSize / maxVital;
                shieldfillRatio = Math.Min(1, Math.Max(0, shieldfillRatio));
                targetShieldWidth = (float) Math.Floor(shieldfillRatio * width);

                //Fix the Labels
                HpLbl.Text = Strings.EntityBox.vital0val.ToString(
                    MyEntity.Vital[(int) Vitals.Health], MyEntity.MaxVital[(int) Vitals.Health]
                );
            }
            else
            {
                HpLbl.Text = Strings.EntityBox.vital0val.ToString(0, 0);
                targetHpWidth = HpBackground.Width;
            }

            if ((int) targetHpWidth != CurHpWidth)
            {
                if ((int) targetHpWidth > CurHpWidth)
                {
                    CurHpWidth += 100f * elapsedTime;
                    if (CurHpWidth > (int) targetHpWidth)
                    {
                        CurHpWidth = targetHpWidth;
                    }
                }
                else
                {
                    CurHpWidth -= 100f * elapsedTime;
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

            if ((int) targetShieldWidth != CurShieldWidth)
            {
                if ((int) targetShieldWidth > CurShieldWidth)
                {
                    CurShieldWidth += 100f * elapsedTime;
                    if (CurShieldWidth > (int) targetShieldWidth)
                    {
                        CurShieldWidth = targetShieldWidth;
                    }
                }
                else
                {
                    CurShieldWidth -= 100f * elapsedTime;
                    if (CurShieldWidth < targetShieldWidth)
                    {
                        CurShieldWidth = targetShieldWidth;
                    }
                }

                if (CurShieldWidth == 0)
                {
                    ShieldBar.IsHidden = true;
                }
                else
                {
                    ShieldBar.Width = (int) CurShieldWidth;
                    ShieldBar.SetBounds(CurHpWidth + HpBar.X, HpBar.Y, CurShieldWidth, ShieldBar.Height);
                    ShieldBar.SetTextureRect(
                        (int) (HpBackground.Width - CurShieldWidth), 0, (int) CurShieldWidth, ShieldBar.Height
                    );

                    ShieldBar.IsHidden = false;
                }
            }
            else
            {
                ShieldBar.SetPosition(HpBar.X + CurHpWidth, HpBar.Y);
            }
        }

        private void UpdateMpBar(float elapsedTime)
        {
            var targetMpWidth = 0f;
            if (MyEntity.MaxVital[(int) Vitals.Mana] > 0)
            {
                targetMpWidth = MyEntity.Vital[(int) Vitals.Mana] / (float) MyEntity.MaxVital[(int) Vitals.Mana];
                targetMpWidth = Math.Min(1, Math.Max(0, targetMpWidth));
                MpLbl.Text = Strings.EntityBox.vital1val.ToString(
                    MyEntity.Vital[(int) Vitals.Mana], MyEntity.MaxVital[(int) Vitals.Mana]
                );

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
                    CurMpWidth += 100f * elapsedTime;
                    if (CurMpWidth > (int) targetMpWidth)
                    {
                        CurMpWidth = targetMpWidth;
                    }
                }
                else
                {
                    CurMpWidth -= 100f * elapsedTime;
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

                ExpLbl.Text = Strings.EntityBox.expval.ToString(
                    ((Player) MyEntity)?.Experience, ((Player) MyEntity)?.GetNextLevelExperience()
                );
            }
            else
            {
                targetExpWidth = 1f;
                ExpLbl.Text = Strings.EntityBox.maxlevel;
            }

            targetExpWidth *= ExpBackground.Width;
            if (Math.Abs((int) targetExpWidth - CurExpWidth) < 0.01)
            {
                return;
            }

            if ((int) targetExpWidth > CurExpWidth)
            {
                CurExpWidth += 100f * elapsedTime;
                if (CurExpWidth > (int) targetExpWidth)
                {
                    CurExpWidth = targetExpWidth;
                }
            }
            else
            {
                CurExpWidth -= 100f * elapsedTime;
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
            var faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, MyEntity.Face);
            var entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MyEntity.MySprite);
            if (faceTex != null && faceTex != EntityFace.Texture)
            {
                EntityFace.Texture = faceTex;
                EntityFace.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                Align.Center(EntityFace);
                mCurrentSprite = MyEntity.Face;
                EntityFace.IsHidden = false;
                var i = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    if (Options.PaperdollOrder[1][z] != "Player")
                    {
                        if (PaperdollPanels == null)
                        {
                            Log.Warn($@"{nameof(PaperdollPanels)} is null.");
                        }
                        else if (PaperdollPanels[i] == null)
                        {
                            Log.Warn($@"{nameof(PaperdollPanels)}[{i}] is null.");
                        }

                        PaperdollPanels?[i]?.Hide();
                        i++;
                    }
                }
            }
            else if (entityTex != null && faceTex == null || faceTex != null && faceTex != EntityFace.Texture)
            {
                if (entityTex != EntityFace.Texture)
                {
                    EntityFace.Texture = entityTex;
                    EntityFace.SetTextureRect(0, 0, entityTex.GetWidth() / Options.Instance.Sprites.NormalFrames, entityTex.GetHeight() / Options.Instance.Sprites.Directions);
                    EntityFace.SizeToContents();
                    Align.Center(EntityFace);
                    mCurrentSprite = MyEntity.MySprite;
                    EntityFace.IsHidden = false;
                }

                var equipment = MyEntity.Equipment;
                if (MyEntity == Globals.Me)
                {
                    for (var i = 0; i < MyEntity.MyEquipment.Length; i++)
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

                var n = 0;
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
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
                        var paperdollTex = Globals.ContentManager.GetTexture(
                            GameContentManager.TextureType.Paperdoll, paperdoll
                        );

                        PaperdollPanels[n].Texture = paperdollTex;
                        if (paperdollTex != null)
                        {
                            PaperdollPanels[n]
                                .SetTextureRect(
                                    0, 0, PaperdollPanels[n].Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    PaperdollPanels[n].Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            PaperdollPanels[n]
                                .SetSize(
                                    PaperdollPanels[n].Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    PaperdollPanels[n].Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            PaperdollPanels[n]
                                .SetPosition(
                                    EntityFaceContainer.Width / 2 - PaperdollPanels[n].Width / 2,
                                    EntityFaceContainer.Height / 2 - PaperdollPanels[n].Height / 2
                                );
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
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    PaperdollPanels[i].Hide();
                }
            }
        }

        public void Dispose()
        {
            EntityWindow.Hide();
            Interface.GameUi.GameCanvas.RemoveChild(EntityWindow, false);
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

        //Input Handlers
        void friendRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                PacketSender.SendAddFriend(MyEntity.Name);
            }
        }

    }

}
