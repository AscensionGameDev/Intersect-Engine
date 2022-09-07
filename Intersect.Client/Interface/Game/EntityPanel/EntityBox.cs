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
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.EntityPanel
{

    public partial class EntityBox
    {
        public readonly Label EntityLevel;

        public readonly Label EntityMap;

        public readonly Label EntityName;

        public readonly Label EntityNameAndLevel;

        //Controls
        public readonly ImagePanel EntityWindow;

        public float CurExpSize = -1;

        public float CurHpSize = -1;

        public float CurMpSize = -1;

        public float CurShieldSize = -1;

        public ImagePanel EntityFace;

        public ImagePanel EntityFaceContainer;

        public ImagePanel EntityInfoPanel;

        public ImagePanel EntityStatusPanel;

        public EntityTypes EntityType;

        public RichLabel EventDesc;

        public ImagePanel ExpBackground;

        public ImagePanel ExpBar;

        public Label ExpLbl;

        public Label ExpTitle;

        public Button FriendLabel;

        public ImagePanel HpBackground;

        public ImagePanel HpBar;

        public Label HpLbl;

        public Label HpTitle;

        private Dictionary<Guid, SpellStatus> mActiveStatuses = new Dictionary<Guid, SpellStatus>();

        private string mCurrentSprite = "";

        private long mLastUpdateTime;

        public ImagePanel MpBackground;

        public ImagePanel MpBar;

        public Label MpLbl;

        public Label MpTitle;

        public Entity MyEntity;

        public ImagePanel[] PaperdollPanels;

        public string[] PaperdollTextures;

        public Button PartyLabel;

        public bool PlayerBox;

        public ImagePanel ShieldBar;

        public Button TradeLabel;

        public bool UpdateStatuses;

        public bool IsHidden;

        public Button GuildLabel;

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

            EntityName = new Label(EntityInfoPanel, "EntityNameLabel") {Text = myEntity?.Name};
            EntityLevel = new Label(EntityInfoPanel, "EntityLevelLabel");
            EntityNameAndLevel = new Label(EntityInfoPanel, "NameAndLevelLabel")
                {IsHidden = true};

            EntityMap = new Label(EntityInfoPanel, "EntityMapLabel");

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
            TradeLabel.SetToolTipText(Strings.EntityBox.tradetip.ToString(MyEntity?.Name));
            TradeLabel.Clicked += tradeRequest_Clicked;

            PartyLabel = new Button(EntityInfoPanel, "PartyButton");
            PartyLabel.SetText(Strings.EntityBox.party);
            PartyLabel.SetToolTipText(Strings.EntityBox.partytip.ToString(MyEntity?.Name));
            PartyLabel.Clicked += invite_Clicked;

            FriendLabel = new Button(EntityInfoPanel, "FriendButton");
            FriendLabel.SetText(Strings.EntityBox.friend);
            FriendLabel.SetToolTipText(Strings.EntityBox.friendtip.ToString(MyEntity?.Name));
            FriendLabel.Clicked += friendRequest_Clicked;
            FriendLabel.IsHidden = true;

            GuildLabel = new Button(EntityInfoPanel, "GuildButton");
            GuildLabel.SetText(Strings.Guilds.Guild);
            GuildLabel.SetToolTipText(Strings.Guilds.guildtip.ToString(MyEntity?.Name));
            GuildLabel.Clicked += guildRequest_Clicked;
            GuildLabel.IsHidden = true;

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

            mLastUpdateTime = Timing.Global.Milliseconds;
        }

        public void SetEntity(Entity entity)
        {
            MyEntity = entity;
            if (MyEntity != null)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (EntityType == EntityTypes.Event)
                {
                    EventDesc.ClearText();
                    EventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                    EventDesc.SizeToChildren(false, true);
                }
            }
        }

        public void SetEntity(Entity entity, EntityTypes type)
        {
            MyEntity = entity;
            EntityType = type;
            if (MyEntity != null)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (EntityType == EntityTypes.Event)
                {
                    EventDesc.ClearText();
                    EventDesc.AddText(((Event)MyEntity).Desc, Color.White);
                    EventDesc.SizeToChildren(false, true);
                }
            }
        }

        public void ShowAllElements()
        {
            TradeLabel.Show();
            PartyLabel.Show();
            FriendLabel.Show();
            ExpBackground.Show();
            ExpBar.Show();
            ExpLbl.Show();
            ExpTitle.Show();
            EntityMap.Show();
            EventDesc.Show();
            MpBackground.Show();
            MpBar.Show();
            MpTitle.Show();
            MpLbl.Show();
            HpBackground.Show();
            HpBar.Show();
            HpLbl.Show();
            HpTitle.Show();

            TryShowGuildButton();
        }

        public void SetupEntityElements()
        {
            ShowAllElements();

            //Update Bars
            CurHpSize = -1;
            CurShieldSize = -1;
            CurMpSize = -1;
            CurExpSize = -1;
            ShieldBar.Hide();
            UpdateHpBar(0, true);
            UpdateMpBar(0, true);
            if (MyEntity is Player)
            {
                UpdateXpBar(0, true);
            }

            switch (EntityType)
            {
                case EntityTypes.Player:
                    if (Globals.Me != null && Globals.Me == MyEntity)
                    {
                        TradeLabel.Hide();
                        PartyLabel.Hide();
                        FriendLabel.Hide();
                        GuildLabel.Hide();

                        if (!PlayerBox)
                        {
                            ExpBackground.Hide();
                            ExpBar.Hide();
                            ExpLbl.Hide();
                            ExpTitle.Hide();
                            EntityMap.Hide();
                        }
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
                    GuildLabel.Hide();
                    FriendLabel.Hide();
                    EntityMap.Hide();

                    break;
                case EntityTypes.Event:
                    EventDesc.Show();
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
                    GuildLabel.Hide();
                    EntityMap.Hide();

                    break;
            }

            EntityName.SetText(MyEntity.Name);
            ShieldBar.Hide();
        }

        //Update
        public void Update()
        {
            if (MyEntity == null || MyEntity.IsDisposed())
            {
                if (!EntityWindow.IsHidden)
                {
                    EntityWindow.Hide();
                }

                return;
            }
            else
            {
                if (EntityWindow.IsHidden)
                {
                    EntityWindow.Show();
                }
            }

            if (PlayerBox)
            {
                if (EntityWindow.IsHidden)
                {
                    EntityWindow.Show();
                }

                if (MyEntity.IsDisposed())
                {
                    Dispose();
                }
            }

            UpdateSpellStatus();

            //Time since this window was last updated (for bar animations)
            var elapsedTime = (Timing.Global.Milliseconds - mLastUpdateTime) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            IsHidden = true;
            if (EntityType != EntityTypes.Event)
            {
                EntityName.SetText(MyEntity.Name);
                UpdateLevel();
                UpdateMap();
                UpdateHpBar(elapsedTime);
                UpdateMpBar(elapsedTime);
                IsHidden = false;
            }
            else
            {
                if (!EntityNameAndLevel.IsHidden)
                {
                    EntityNameAndLevel.Text = MyEntity.Name;
                }
            }

            //If player draw exp bar
            if (PlayerBox && MyEntity == Globals.Me)
            {
                UpdateXpBar(elapsedTime);
            }

            if (MyEntity.Type == EntityTypes.Player && MyEntity != Globals.Me)
            {
                if (MyEntity.Vital[(int)Vitals.Health] <= 0)
                {
                    TradeLabel.Hide();
                    PartyLabel.Hide();
                    FriendLabel.Hide();
                    GuildLabel.Hide();
                }
                else if (TradeLabel.IsHidden || PartyLabel.IsHidden || FriendLabel.IsHidden)
                {
                    TradeLabel.Show();
                    PartyLabel.Show();
                    FriendLabel.Show();
                    TryShowGuildButton();
                }
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

            mLastUpdateTime = Timing.Global.Milliseconds;
        }

        public void UpdateSpellStatus()
        {
            if (MyEntity == null)
            {
                return;
            }

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
                    mActiveStatuses[status].UpdateStatus(MyEntity.GetStatus(status) as Status);
                }
            }

            //Add all of the spell status effects
            for (var i = 0; i < MyEntity.Status.Count; i++)
            {
                var id = MyEntity.Status[i].SpellId;
                SpellStatus itm = null;
                if (!mActiveStatuses.ContainsKey(id))
                {
                    itm = new SpellStatus(this, MyEntity.Status[i] as Status);
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

        private void UpdateHpBar(float elapsedTime, bool instant = false)
        {
            var targetHpSize = 0f;
            var targetShieldSize = 0f;
            if (MyEntity.MaxVital[(int) Vitals.Health] > 0)
            {
                var maxVital = MyEntity.MaxVital[(int) Vitals.Health];
                var shieldSize = MyEntity.GetShieldSize();

                if (shieldSize + MyEntity.Vital[(int)Vitals.Health] > maxVital)
                {
                    maxVital = shieldSize + MyEntity.Vital[(int)Vitals.Health];
                }

                var width = HpBackground.Width;

                var hpfillRatio = (float) MyEntity.Vital[(int) Vitals.Health] / maxVital;
                hpfillRatio = Math.Min(1, Math.Max(0, hpfillRatio));
                targetHpSize = (float) Math.Ceiling(hpfillRatio * width);

                var shieldfillRatio = (float) shieldSize / maxVital;
                shieldfillRatio = Math.Min(1, Math.Max(0, shieldfillRatio));
                targetShieldSize = (float) Math.Floor(shieldfillRatio * width);

                float hpPercentage = hpfillRatio * 100;
                var hpPercentageText = $"{hpPercentage:0.##}%";
                var hpValueText = Strings.EntityBox.vital0val.ToString(
                    MyEntity.Vital[(int)Vitals.Health], MyEntity.MaxVital[(int)Vitals.Health]
                );

                if (Globals.Database.ShowHealthAsPercentage)
                {
                    HpLbl.Text = hpPercentageText;
                    HpBackground.SetToolTipText(hpValueText);
                }
                else
                {
                    HpLbl.Text = hpValueText;
                    HpBackground.SetToolTipText(hpPercentageText);
                }
            }
            else
            {
                HpLbl.Text = Strings.EntityBox.vital0val.ToString(0, 0);
                targetHpSize = HpBackground.Width;
            }

            if ((int)targetHpSize != CurHpSize)
            {
                if (!instant)
                {
                    if ((int)targetHpSize > CurHpSize)
                    {
                        CurHpSize += 100f * elapsedTime;
                        if (CurHpSize > (int)targetHpSize)
                        {
                            CurHpSize = targetHpSize;
                        }
                    }
                    else
                    {
                        CurHpSize -= 100f * elapsedTime;
                        if (CurHpSize < targetHpSize)
                        {
                            CurHpSize = targetHpSize;
                        }
                    }
                }
                else
                {
                    CurHpSize = (int)targetHpSize;
                }

                if (CurHpSize == 0)
                {
                    HpBar.IsHidden = true;
                }
                else
                {
                    HpBar.Width = (int)CurHpSize;
                    HpBar.SetTextureRect(0, 0, (int)CurHpSize, HpBar.Height);
                    HpBar.IsHidden = false;
                }
            }

            if ((int)targetShieldSize != CurShieldSize)
            {
                if (!instant)
                {
                    if ((int)targetShieldSize > CurShieldSize)
                    {
                        CurShieldSize += 100f * elapsedTime;
                        if (CurShieldSize > (int)targetShieldSize)
                        {
                            CurShieldSize = targetShieldSize;
                        }
                    }
                    else
                    {
                        CurShieldSize -= 100f * elapsedTime;
                        if (CurShieldSize < targetShieldSize)
                        {
                            CurShieldSize = targetShieldSize;
                        }
                    }
                }
                else
                {
                    CurShieldSize = (int)targetShieldSize;
                }

                if (CurShieldSize == 0)
                {
                    ShieldBar.IsHidden = true;
                }
                else
                {
                    ShieldBar.Width = (int)CurShieldSize;
                    ShieldBar.SetBounds(CurHpSize + HpBar.X, HpBar.Y, CurShieldSize, ShieldBar.Height);
                    ShieldBar.SetTextureRect(
                        (int)(HpBackground.Width - CurShieldSize), 0, (int)CurShieldSize, ShieldBar.Height
                    );

                    ShieldBar.IsHidden = false;
                }
            }
            else
            {
                ShieldBar.SetPosition(HpBar.X + CurHpSize, HpBar.Y);
            }
        }

        private void UpdateMpBar(float elapsedTime, bool instant = false)
        {
            var targetMpSize = 0f;
            if (MyEntity.MaxVital[(int)Vitals.Mana] > 0)
            {
                targetMpSize = MyEntity.Vital[(int) Vitals.Mana] / (float) MyEntity.MaxVital[(int) Vitals.Mana];
                targetMpSize = Math.Min(1, Math.Max(0, targetMpSize));
                float mpPercentage = targetMpSize * 100;
                var mpPercentageText = $"{mpPercentage:0.##}%";
                var mpValueText = Strings.EntityBox.vital1val.ToString(
                    MyEntity.Vital[(int)Vitals.Mana], MyEntity.MaxVital[(int)Vitals.Mana]
                );

                if (Globals.Database.ShowManaAsPercentage)
                {
                    MpLbl.Text = mpPercentageText;
                    MpBackground.SetToolTipText(mpValueText);
                }
                else
                {
                    MpLbl.Text = mpValueText;
                    MpBackground.SetToolTipText(mpPercentageText);
                }

                targetMpSize *= MpBackground.Width;
            }
            else
            {
                MpLbl.Text = Strings.EntityBox.vital1val.ToString(0, 0);
                targetMpSize = MpBackground.Width;
            }

            if (!targetMpSize.Equals(CurMpSize))
            {
                if (!instant)
                {
                    if ((int)targetMpSize > CurMpSize)
                    {
                        CurMpSize += 100f * elapsedTime;
                        if (CurMpSize > (int)targetMpSize)
                        {
                            CurMpSize = targetMpSize;
                        }
                    }
                    else
                    {
                        CurMpSize -= 100f * elapsedTime;
                        if (CurMpSize < targetMpSize)
                        {
                            CurMpSize = targetMpSize;
                        }
                    }
                }
                else
                {
                    CurMpSize = (int)targetMpSize;
                }

                if (CurMpSize == 0)
                {
                    MpBar.IsHidden = true;
                }
                else
                {
                    MpBar.Width = (int)CurMpSize;
                    MpBar.SetTextureRect(0, 0, (int)CurMpSize, MpBar.Height);
                    MpBar.IsHidden = false;
                }
            }
        }

        private void UpdateXpBar(float elapsedTime, bool instant = false)
        {
            float targetExpSize = 1;
            if (((Player) MyEntity).GetNextLevelExperience() > 0)
            {
                targetExpSize = (float)((Player)MyEntity).Experience /
                                 (float)((Player)MyEntity).GetNextLevelExperience();
                float expPercentage = targetExpSize * 100;
                var expPercentageText = $"{expPercentage:0.##}%";
                var expValueText = Strings.EntityBox.expval.ToString(
                    ((Player)MyEntity)?.Experience, ((Player)MyEntity)?.GetNextLevelExperience()
                );

                if (Globals.Database.ShowExperienceAsPercentage)
                {
                    ExpLbl.Text = expPercentageText;
                    ExpBackground.SetToolTipText(expValueText);
                }
                else
                {
                    ExpLbl.Text = expValueText;
                    ExpBackground.SetToolTipText(expPercentageText);
                }
            }
            else
            {
                targetExpSize = 1f;
                ExpLbl.Text = Strings.EntityBox.maxlevel;
            }

            targetExpSize *= ExpBackground.Width;
            if (Math.Abs((int) targetExpSize - CurExpSize) < 0.01)
            {
                return;
            }

            if (!instant)
            {
                if ((int)targetExpSize > CurExpSize)
                {
                    CurExpSize += 100f * elapsedTime;
                    if (CurExpSize > (int)targetExpSize)
                    {
                        CurExpSize = targetExpSize;
                    }
                }
                else
                {
                    CurExpSize -= 100f * elapsedTime;
                    if (CurExpSize < targetExpSize)
                    {
                        CurExpSize = targetExpSize;
                    }
                }
            }
            else
            {
                CurExpSize = (int)targetExpSize;
            }

            if (CurExpSize == 0)
            {
                ExpBar.IsHidden = true;
            }
            else
            {
                ExpBar.Width = (int) CurExpSize;
                ExpBar.SetTextureRect(0, 0, (int) CurExpSize, ExpBar.Height);
                ExpBar.IsHidden = false;
            }
        }

        private void UpdateImage()
        {
            var faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, MyEntity.Face);
            var entityTex = MyEntity.Texture;
            if (faceTex != null && faceTex != EntityFace.Texture)
            {
                EntityFace.Texture = faceTex;
                EntityFace.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                EntityFace.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                EntityFace.SizeToContents();
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
                    EntityFace.RenderColor = MyEntity.Color ?? new Color(255, 255, 255, 255);
                    EntityFace.SetTextureRect(0, 0, entityTex.GetWidth() / Options.Instance.Sprites.NormalFrames, entityTex.GetHeight() / Options.Instance.Sprites.Directions);
                    EntityFace.SizeToContents();
                    Align.Center(EntityFace);
                    mCurrentSprite = MyEntity.Sprite;
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
                    var paperdollPanel = PaperdollPanels[n];
                    var paperdoll = string.Empty;
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
                                paperdollPanel.RenderColor = itemdata.Color;
                            }
                        }
                    }

                    //Check for Player layer
                    if (Options.PaperdollOrder[1][z] == "Player")
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(PaperdollTextures[n]))
                    {
                        paperdollPanel.Texture = null;
                        paperdollPanel.Hide();
                        PaperdollTextures[n] = string.Empty;
                    }
                    else if (!string.IsNullOrWhiteSpace(paperdoll) && paperdoll != PaperdollTextures[n])
                    {
                        var paperdollTex = Globals.ContentManager.GetTexture(
                            Framework.Content.TextureType.Paperdoll, paperdoll
                        );

                        paperdollPanel.Texture = paperdollTex;
                        if (paperdollTex != null)
                        {
                            paperdollPanel
                                .SetTextureRect(
                                    0, 0, paperdollPanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    paperdollPanel.Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            paperdollPanel
                                .SetSize(
                                    paperdollPanel.Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    paperdollPanel.Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            paperdollPanel
                                .SetPosition(
                                    (EntityFaceContainer.Width - paperdollPanel.Width) / 2,
                                    (EntityFaceContainer.Height - paperdollPanel.Height) / 2
                                );
                        }

                        paperdollPanel.Show();
                        PaperdollTextures[n] = paperdoll;
                    }

                    //Check for Player layer
                    if (Options.PaperdollOrder[1][z] != "Player")
                    {
                        n++;
                    }
                }
            }
            else if (MyEntity.Sprite != mCurrentSprite && MyEntity.Face != mCurrentSprite)
            {
                EntityFace.IsHidden = true;
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    PaperdollPanels[i]?.Hide();
                }
            }

            if (EntityFace.RenderColor != MyEntity.Color)
            {
                EntityFace.RenderColor = MyEntity.Color;
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
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendPartyInvite(Globals.Me.TargetIndex);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Parties.infight.ToString(), 4);
                }
            }
        }

        //Input Handlers
        void tradeRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendTradeRequest(Globals.Me.TargetIndex);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Trading.infight.ToString(), 4);
                }
            }
        }

        //Input Handlers
        void friendRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me.TargetIndex != Guid.Empty && Globals.Me.TargetIndex != Globals.Me.Id)
            {
                if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                {
                    PacketSender.SendAddFriend(MyEntity.Name);
                }
                else
                {
                    PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                }
            }
        }


        void guildRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (MyEntity is Player plyr && MyEntity != Globals.Me)
            {
                if (string.IsNullOrWhiteSpace(plyr.Guild))
                {
                    if (Globals.Me?.GuildRank?.Permissions?.Invite ?? false)
                    {
                        if (Globals.Me.CombatTimer < Timing.Global.Milliseconds)
                        {
                            PacketSender.SendInviteGuild(MyEntity.Name);
                        }
                        else
                        {
                            PacketSender.SendChatMsg(Strings.Friends.InFight.ToString(), 4);
                        }
                    }
                }
                else
                {
                    Chat.ChatboxMsg.AddMessage(new Chat.ChatboxMsg(Strings.Guilds.InviteAlreadyInGuild, Color.Red, ChatMessageType.Guild));
                }
            }
        }

        void TryShowGuildButton()
        {
            var show = false;
            if (MyEntity is Player plyr && MyEntity != Globals.Me && string.IsNullOrWhiteSpace(plyr.Guild))
            {
                if (Globals.Me?.GuildRank?.Permissions?.Invite ?? false)
                {
                    show = true;
                }
            }

            GuildLabel.IsHidden = !show;
        }


        public void Hide()
        {
            EntityWindow.Hide();
        }

        public void Show()
        {
            EntityWindow.Show();
        }

    }

}
