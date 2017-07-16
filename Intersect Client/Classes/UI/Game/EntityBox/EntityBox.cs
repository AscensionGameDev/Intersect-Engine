using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect.Client.Classes.UI.Game.EntityBox;

namespace Intersect_Client.Classes.UI.Game
{
    public class EntityBox
    {
        //Controls
        public ImagePanel _entityWindow;
        public ImagePanel _entityInfoPanel;
        public ImagePanel _entityStatusPanel;
        public ImagePanel _entityFace;
        public ImagePanel _entityFaceContainer;
        public ImagePanel[] _paperdollPanels;
        public string[] _paperdollTextures;
        public Label _entityLevel;
        public Label _entityName;
        public RichLabel _eventDesc;
        public ImagePanel _expBackground;
        public ImagePanel _expBar;
        public Label _expLbl;
        public Label _expTitle;
        public ImagePanel _hpBackground;
        public ImagePanel _hpBar;
        public Label _hpLbl;
        public Label _hpTitle;
        public ImagePanel _mpBackground;
        public ImagePanel _mpBar;
        public Label _mpLbl;
        public Label _mpTitle;

        private static int StatusXPadding = 2;
        private static int StatusYPadding = 2;

        public float _curEXPWidth;

        public float _curHPWidth;
        public float _curMPWidth;
        private string _currentSprite = "";

        //Spell List
        public List<SpellStatus> Items = new List<SpellStatus>();

        public Entity _myEntity;
        public EntityTypes _entityType;
        public bool PlayerBox;

        private long lastUpdateTime;
        public Button PartyLabel;
        public Button TradeLabel;

        private ImagePanel _statusTemplate;

        public bool UpdateStatuses = false;
        private bool _initialized = false;

        //Init
        public EntityBox(Canvas _gameCanvas, EntityTypes entityType, Entity myEntity, bool playerBox = false)
        {
            _myEntity = myEntity;
            _entityType = entityType;
            PlayerBox = playerBox;
            if (playerBox)
            {
                _entityWindow = new ImagePanel(_gameCanvas,"PlayerBox");
            }
            else
            {
                _entityWindow = new ImagePanel(_gameCanvas,"TargetBox");
            }

            _entityInfoPanel = new ImagePanel(_entityWindow,"EntityInfoPanel");

            _entityName = new Label(_entityInfoPanel,"EntityNameLabel");
            _entityName.SetText(myEntity.MyName);

            _entityFaceContainer = new ImagePanel(_entityInfoPanel,"EntityGraphicContainer");

            _entityFace = new ImagePanel(_entityFaceContainer);
            _entityFace.SetSize(64, 64);
            _entityFace.AddAlignment(Alignments.Center);

            _paperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            _paperdollTextures = new string[Options.EquipmentSlots.Count];
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                _paperdollPanels[i] = new ImagePanel(_entityFaceContainer);
                _paperdollTextures[i] = "";
                _paperdollPanels[i].Hide();
            }

            _eventDesc = new RichLabel(_entityInfoPanel,"EventDescLabel");

            _entityLevel = new Label(_entityInfoPanel,"EntityLevelLabel");

            _hpBackground = new ImagePanel(_entityInfoPanel, "HPBarBackground");
            _hpBar = new ImagePanel(_entityInfoPanel,"HPBar");
            _hpTitle = new Label(_entityInfoPanel,"HPTitle");
            _hpTitle.SetText(Strings.Get("entitybox", "vital0"));
            _hpLbl = new Label(_entityInfoPanel, "HPLabel");

            _mpBackground = new ImagePanel(_entityInfoPanel, "MPBackground");
            _mpBar = new ImagePanel(_entityInfoPanel,"MPBar");
            _mpTitle = new Label(_entityInfoPanel,"MPTitle");
            _mpTitle.SetText(Strings.Get("entitybox", "vital1"));
            _mpLbl = new Label(_entityInfoPanel, "MPLabel");

            _expBackground = new ImagePanel(_entityInfoPanel, "EXPBackground");
            _expBar = new ImagePanel(_entityInfoPanel,"EXPBar");
            _expTitle = new Label(_entityInfoPanel,"EXPTitle");
            _expTitle.SetText(Strings.Get("entitybox", "exp"));
            _expLbl = new Label(_entityInfoPanel, "EXPLabel");

            TradeLabel = new Button(_entityInfoPanel,"TradeButton");
            TradeLabel.SetText(Strings.Get("entitybox", "trade"));
            TradeLabel.SetToolTipText(Strings.Get("entitybox", "tradetip", _myEntity.MyName));
            TradeLabel.Clicked += tradeRequest_Clicked;

            PartyLabel = new Button(_entityInfoPanel,"PartyButton");
            PartyLabel.SetText(Strings.Get("entitybox", "party"));
            PartyLabel.SetToolTipText(Strings.Get("entitybox", "partytip", _myEntity.MyName));
            PartyLabel.Clicked += invite_Clicked;

            _entityStatusPanel = new ImagePanel(_entityWindow,"StatusArea");

            if (playerBox)
            {
                _statusTemplate = new ImagePanel(_entityStatusPanel, "PlayerStatusTemplate");
            }
            else
            {
                _statusTemplate = new ImagePanel(_entityStatusPanel, "TargetStatusTemplate");
            }


            var _itemIcon = new ImagePanel(_statusTemplate, "StatusIcon");

            UpdateSpellStatus();

            SetEntity(myEntity);

            //TODO: Make this more efficient
            if (!PlayerBox) Gui.LoadRootUIData(_entityWindow, "InGame.xml");

            lastUpdateTime = Globals.System.GetTimeMS();
        }

        public void SetEntity(Entity entity)
        {
            _myEntity = entity;
            if (_myEntity != null) {
                SetupEntityElements();
            }
        }

        public void SetupEntityElements()
        {
            switch (_entityType)
            {
                case EntityTypes.Player:
                    if (Globals.Me != null && Globals.Me == _myEntity)
                    {
                        TradeLabel.Hide();
                        PartyLabel.Hide();
                    }
                    else
                    {
                        _expBackground.Hide();
                        _expBar.Hide();
                        _expLbl.Hide();
                        _expTitle.Hide();
                    }
                    _eventDesc.Hide();
                    break;
                case EntityTypes.GlobalEntity:
                    _eventDesc.Hide();
                    _expBackground.Hide();
                    _expBar.Hide();
                    _expLbl.Hide();
                    _expTitle.Hide();
                    TradeLabel.Hide();
                    PartyLabel.Hide();
                    break;
                case EntityTypes.Event:
                    _expBackground.Hide();
                    _expBar.Hide();
                    _expLbl.Hide();
                    _expTitle.Hide();
                    _mpBackground.Hide();
                    _mpBar.Hide();
                    _mpTitle.Hide();
                    _mpLbl.Hide();
                    _hpBackground.Hide();
                    _hpBar.Hide();
                    _hpLbl.Hide();
                    _hpTitle.Hide();
                    TradeLabel.Hide();
                    PartyLabel.Hide();
                    _entityLevel.Hide();
                    break;
            }
            _entityName.SetText(_myEntity.MyName);
        }

        //Update
        public void Update()
        {
            if (_myEntity == null)
            {
                if (!_entityWindow.IsHidden) _entityWindow.Hide();
                return;
            }
            else
            {
                if (_entityWindow.IsHidden)_entityWindow.Show();
            }
            if (_myEntity.IsDisposed()) Dispose();
            if (!_initialized)
            {
                SetupEntityElements();
                UpdateSpellStatus();
                if (_entityType == EntityTypes.Event)
                {
                    _eventDesc.AddText(((Event)_myEntity).Desc, Color.White);
                    _eventDesc.SizeToChildren(false, true);
                }
                _initialized = true;
            }

            //Time since this window was last updated (for bar animations)
            float elapsedTime = ((float)(Globals.System.GetTimeMS() - lastUpdateTime)) / 1000.0f;

            //Update the event/entity face.
            UpdateImage();

            if (_entityType != EntityTypes.Event)
            {
                UpdateLevel();
                UpdateHpBar(elapsedTime);
                UpdateMpBar(elapsedTime);
            }


            //If player draw exp bar
            if (_myEntity == Globals.Me)
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

            lastUpdateTime = Globals.System.GetTimeMS();
        }

        public void UpdateSpellStatus()
        {
            foreach (SpellStatus s in Items)
            {
                s.pnl.Texture = null;
                s.container.Texture = null;
                _entityStatusPanel.RemoveChild(s.container, true);
                s.pnl_HoverLeave(null, null);
            }
            Items.Clear();

            //Add all of the spell status effects
            for (int i = 0; i < _myEntity.Status.Count; i++)
            {
                Items.Add(new SpellStatus(this, i));
                if (PlayerBox)
                {
                    Items[i].container = new ImagePanel(_entityStatusPanel,"PlayerStatusTemplate");
                }
                else
                {
                    Items[i].container = new ImagePanel(_entityStatusPanel,"TargetStatusTemplate");
                }
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].container, "InGame.xml");

                var xPadding = Items[i].container.Padding.Left + Items[i].container.Padding.Right;
                var yPadding = Items[i].container.Padding.Top + Items[i].container.Padding.Bottom;
                Items[i].container.SetPosition(
                    (i % (_entityStatusPanel.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Width + xPadding) + xPadding,
                    (i / (_entityStatusPanel.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Height + yPadding) + yPadding);

            }
            _statusTemplate.Hide();
        }

        private void UpdateLevel()
        {
            _entityLevel.SetText(Strings.Get("entitybox", "level", _myEntity.Level));
        }

        private void UpdateHpBar(float elapsedTime)
        {
            float targetHPWidth = 0f;
            if (_myEntity.MaxVital[(int)Vitals.Health] > 0)
            {
                targetHPWidth = ((float)_myEntity.Vital[(int)Vitals.Health] /
                                 (float)_myEntity.MaxVital[(int)Vitals.Health]);
                targetHPWidth = Math.Min(1, Math.Max(0, targetHPWidth));
                //Fix the Labels
                _hpLbl.Text = Strings.Get("entitybox", "vital0val", _myEntity.Vital[(int)Vitals.Health],
                    _myEntity.MaxVital[(int)Vitals.Health]);
                //Multiply by the width of the bars.
                targetHPWidth *= _hpBackground.Width;
            }
            else
            {
                _hpLbl.Text = Strings.Get("entitybox", "vital0val", 0, 0);
                targetHPWidth = _hpBackground.Width;
            }
            if ((int)targetHPWidth != _curHPWidth)
            {
                if ((int)targetHPWidth > _curHPWidth)
                {
                    _curHPWidth += (100f * elapsedTime);
                    if (_curHPWidth > (int)targetHPWidth)
                    {
                        _curHPWidth = targetHPWidth;
                    }
                }
                else
                {
                    _curHPWidth -= (100f * elapsedTime);
                    if (_curHPWidth < targetHPWidth)
                    {
                        _curHPWidth = targetHPWidth;
                    }
                }
                if (_curHPWidth == 0)
                {
                    _hpBar.IsHidden = true;
                }
                else
                {
                    _hpBar.Width = (int)_curHPWidth;
                    _hpBar.SetTextureRect(0, 0, (int)_curHPWidth, _hpBar.Height);
                    _hpBar.IsHidden = false;
                }
            }
        }

        private void UpdateMpBar(float elapsedTime)
        {
            float targetMPWidth = 0f;
            if (_myEntity.MaxVital[(int)Vitals.Mana] > 0)
            {
                targetMPWidth = ((float)_myEntity.Vital[(int)Vitals.Mana] /
                                 (float)_myEntity.MaxVital[(int)Vitals.Mana]);
                targetMPWidth = Math.Min(1, Math.Max(0, targetMPWidth));
                _mpLbl.Text = Strings.Get("entitybox", "vital1val", _myEntity.Vital[(int)Vitals.Mana],
                    _myEntity.MaxVital[(int)Vitals.Mana]);
                targetMPWidth *= _mpBackground.Width;
            }
            else
            {
                _mpLbl.Text = Strings.Get("entitybox", "vital1val", 0, 0);
                targetMPWidth = _mpBackground.Width;
            }
            if ((int)targetMPWidth != _curMPWidth)
            {
                if ((int)targetMPWidth > _curMPWidth)
                {
                    _curMPWidth += (100f * elapsedTime);
                    if (_curMPWidth > (int)targetMPWidth)
                    {
                        _curMPWidth = targetMPWidth;
                    }
                }
                else
                {
                    _curMPWidth -= (100f * elapsedTime);
                    if (_curMPWidth < targetMPWidth)
                    {
                        _curMPWidth = targetMPWidth;
                    }
                }
                if (_curMPWidth == 0)
                {
                    _mpBar.IsHidden = true;
                }
                else
                {
                    _mpBar.Width = (int)_curMPWidth;
                    _mpBar.SetTextureRect(0, 0, (int)_curMPWidth, _mpBar.Height);
                    _mpBar.IsHidden = false;
                }
            }
        }

        private void UpdateXpBar(float elapsedTime)
        {
            float targetExpWidth = 1;
            if (((Player)_myEntity).GetNextLevelExperience() > 0)
            {
                targetExpWidth = (float)((Player)_myEntity).Experience /
                                 (float)((Player)_myEntity).GetNextLevelExperience();
                _expLbl.Text = Strings.Get("entitybox", "expval", ((Player)_myEntity).Experience,
                    ((Player)_myEntity).GetNextLevelExperience());
            }
            else
            {
                targetExpWidth = 1f;
                _expLbl.Text = Strings.Get("entitybox", "maxlevel");
            }
            _expLbl.X = _expBackground.X + _expBackground.Width / 2 - _expLbl.Width / 2;
            targetExpWidth *= _expBackground.Width;
            if ((int)targetExpWidth != _curEXPWidth)
            {
                if ((int)targetExpWidth > _curEXPWidth)
                {
                    _curEXPWidth += (100f * elapsedTime);
                    if (_curEXPWidth > (int)targetExpWidth)
                    {
                        _curEXPWidth = targetExpWidth;
                    }
                }
                else
                {
                    _curEXPWidth -= (100f * elapsedTime);
                    if (_curEXPWidth < targetExpWidth)
                    {
                        _curEXPWidth = targetExpWidth;
                    }
                }
                if (_curEXPWidth == 0)
                {
                    _expBar.IsHidden = true;
                }
                else
                {
                    _expBar.Width = (int)_curEXPWidth;
                    _expBar.SetTextureRect(0, 0, (int)_curEXPWidth, _expBar.Height);
                    _expBar.IsHidden = false;
                }
            }
        }

        private void UpdateImage()
        {
            GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, _myEntity.Face);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, _myEntity.MySprite);
            if (faceTex != null && faceTex != _entityFace.Texture)
            {
                _entityFace.Texture = faceTex;
                _entityFace.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                _entityFace.SizeToContents();
                Align.Center(_entityFace);
                _currentSprite = _myEntity.Face;
                _entityFace.IsHidden = false;
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPanels[i].Hide();
                }
            }
            else if (entityTex != null && faceTex == null || (faceTex != null && faceTex != _entityFace.Texture))
            {
                if (entityTex != _entityFace.Texture)
                {
                    _entityFace.Texture = entityTex;
                    _entityFace.SetTextureRect(0, 0, entityTex.GetWidth() / 4, entityTex.GetHeight() / 4);
                    _entityFace.SizeToContents();
                    Align.Center(_entityFace);
                    _currentSprite = _myEntity.MySprite;
                    _entityFace.IsHidden = false;
                }
                for (int z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    var paperdoll = "";
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                    {
                        var Equipment = _myEntity.Equipment;
                        if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 &&
                            (_myEntity != Globals.Me ||
                             Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] <
                             Options.MaxInvItems))
                        {
                            var itemNum = -1;
                            if (_myEntity == Globals.Me)
                            {
                                itemNum =
                                    Globals.Me.Inventory[
                                            Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])
                                            ]]
                                        .ItemNum;
                            }
                            else
                            {
                                itemNum = Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])];
                            }
                            if (ItemBase.Lookup.Get<ItemBase>(itemNum) != null)
                            {
                                var itemdata = ItemBase.Lookup.Get<ItemBase>(itemNum);
                                if (_myEntity.Gender == 0)
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
                    if (paperdoll == "" && _paperdollTextures[z] != "")
                    {
                        _paperdollPanels[z].Texture = null;
                        _paperdollPanels[z].Hide();
                        _paperdollTextures[z] = "";
                    }
                    else if (paperdoll != "" && paperdoll != _paperdollTextures[z])
                    {
                        var _paperdollTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll, paperdoll);
                        _paperdollPanels[z].Texture = _paperdollTex;
                        _paperdollPanels[z].SetTextureRect(0, 0,
                            _paperdollPanels[z].Texture.GetWidth() / 4,
                            _paperdollPanels[z].Texture.GetHeight() / 4);
                        _paperdollPanels[z].SetSize(_paperdollPanels[z].Texture.GetWidth() / 4,
                            _paperdollPanels[z].Texture.GetHeight() / 4);
                        _paperdollPanels[z].SetPosition(
                            _entityFaceContainer.Width / 2 - _paperdollPanels[z].Width / 2,
                            _entityFaceContainer.Height / 2 - _paperdollPanels[z].Height / 2);
                        _paperdollPanels[z].Show();
                        _paperdollTextures[z] = paperdoll;
                    }
                }
            }
            else if (_myEntity.MySprite != _currentSprite && _myEntity.Face != _currentSprite)
            {
                _entityFace.IsHidden = true;
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPanels[i].Hide();
                }
            }
        }

        public void Dispose()
        {
            _entityWindow.Hide();
            Gui.GameUI.GameCanvas.RemoveChild(_entityWindow, false);
            _entityWindow.Dispose();
        }

        //Input Handlers
        void invite_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me._targetIndex != -1 && Globals.Me._targetIndex != Globals.Me.MyIndex)
            {
                PacketSender.SendPartyInvite(Globals.Me._targetIndex);
            }
        }

        //Input Handlers
        void tradeRequest_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.Me._targetIndex != -1 && Globals.Me._targetIndex != Globals.Me.MyIndex)
            {
                PacketSender.SendTradeRequest(Globals.Me._targetIndex);
            }
        }
    }
}