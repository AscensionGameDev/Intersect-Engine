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

namespace Intersect_Client.Classes.UI.Game
{
    public class EntityBox
    {
        private static int StatusXPadding = 2;
        private static int StatusYPadding = 2;

        public float _curEXPWidth;

        public float _curHPWidth;
        public float _curMPWidth;
        private string _currentSprite = "";
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

		//Spell List
		public List<SpellStatus> Items = new List<SpellStatus>();

		public Entity _myEntity;

        private long lastUpdateTime;
        public Label PartyLabel;
        public Label TradeLabel;

        public bool UpdateStatuses = false;

        //Init
        public EntityBox(Canvas _gameCanvas, Entity myEntity, int x, int y)
        {
            _myEntity = myEntity;

            _entityWindow = new ImagePanel(_gameCanvas);
            _entityWindow.SetSize(314, 192);
            _entityWindow.SetPosition(x, y);

            _entityInfoPanel = new ImagePanel(_entityWindow);
            _entityInfoPanel.SetSize(314, 126);
            _entityInfoPanel.SetPosition(0, 0);
            _entityInfoPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "entitybox.png");

            _entityName = new Label(_entityInfoPanel);
            _entityName.SetPosition(16, 7);
            _entityName.SetText(myEntity.MyName);
            _entityName.SetTextColor(Color.White, Label.ControlState.Normal);
            _entityName.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 14);

            _entityFaceContainer = new ImagePanel(_entityInfoPanel);
            _entityFaceContainer.SetSize(64, 64);
            _entityFaceContainer.SetPosition(18, 39);

            _entityFace = new ImagePanel(_entityFaceContainer);
            _entityFace.SetSize(64, 64);
            _entityFace.SetPosition(0, 0);

            _paperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            _paperdollTextures = new string[Options.EquipmentSlots.Count];
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                _paperdollPanels[i] = new ImagePanel(_entityFaceContainer);
                _paperdollTextures[i] = "";
                _paperdollPanels[i].Hide();
            }

            if (myEntity.GetType() == typeof(Event))
            {
                _eventDesc = new RichLabel(_entityInfoPanel);
                _eventDesc.SetPosition(93, 37);
                _eventDesc.Width = 207;
                _eventDesc.AddText(((Event) _myEntity).Desc, Color.White);
                _eventDesc.SizeToChildren(false, true);
            }
            else
            {
                _entityLevel = new Label(_entityInfoPanel);
                _entityLevel.SetPosition(0, 0);
                _entityLevel.SetTextColor(Color.White, Label.ControlState.Normal);
                _entityLevel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 12);
            }

            if (myEntity.GetType() != typeof(Event))
            {
                _hpBackground = new ImagePanel(_entityInfoPanel)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _hpBackground.SetSize(183, 25);
                _hpBackground.SetPosition(117, 32);

                _hpBar = new ImagePanel(_entityInfoPanel);
                _hpBar.SetSize(183, 25);
                _hpBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "lifebar.png");
                _hpBar.SetPosition(117, 32);
                _hpBar.IsHidden = true;

                _hpTitle = new Label(_entityInfoPanel);
                _hpTitle.SetText(Strings.Get("entitybox", "vital0"));
                _hpTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _hpTitle.SetPosition(93, 37);

                _hpLbl = new Label(_entityInfoPanel)
                {
                    Alignment = Pos.Center,
                    AutoSizeToContents = false
                };
                _hpLbl.SetPosition(_hpBackground.X, _hpBackground.Y);
                _hpLbl.SetSize(_hpBackground.Width, _hpBackground.Height);
                _hpLbl.SetTextColor(Color.White, Label.ControlState.Normal);

                _mpBackground = new ImagePanel(_entityInfoPanel)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _mpBackground.SetSize(183, 25);
                _mpBackground.SetPosition(117, 58);

                _mpBar = new ImagePanel(_entityInfoPanel);
                _mpBar.SetSize(183, 25);
                _mpBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "manabar.png");
                _mpBar.SetPosition(117, 58);
                _mpBar.IsHidden = true;

                _mpTitle = new Label(_entityInfoPanel);
                _mpTitle.SetText(Strings.Get("entitybox", "vital1"));
                _mpTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _mpTitle.SetPosition(93, 63);

                _mpLbl = new Label(_entityInfoPanel)
                {
                    Alignment = Pos.Center,
                    AutoSizeToContents = false
                };
                _mpLbl.SetPosition(_mpBackground.X, _mpBackground.Y);
                _mpLbl.SetSize(_mpBackground.Width, _mpBackground.Height);
                _mpLbl.SetTextColor(Color.White, Label.ControlState.Normal);
            }

            if (_myEntity == Globals.Me)
            {
                _expBackground = new ImagePanel(_entityInfoPanel)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _expBackground.SetSize(183, 25);
                _expBackground.SetPosition(117, 84);

                _expBar = new ImagePanel(_entityInfoPanel);
                _expBar.SetSize(183, 25);
                _expBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "expbar.png");
                _expBar.SetPosition(117, 84);
                _expBar.IsHidden = true;

                _expTitle = new Label(_entityInfoPanel);
                _expTitle.SetText(Strings.Get("entitybox", "exp"));
                _expTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _expTitle.SetPosition(88, 89);

                _expLbl = new Label(_entityInfoPanel)
                {
                    Alignment = Pos.Center,
                    AutoSizeToContents = false
                };
                _expLbl.SetPosition(_expBackground.X, _expBackground.Y);
                _expLbl.SetSize(_expBackground.Width, _expBackground.Height);
                _expLbl.SetTextColor(Color.White, Label.ControlState.Normal);
            }
            else if (_myEntity.GetType() == typeof(Player))
            {
                TradeLabel = new Label(_entityInfoPanel);
                TradeLabel.SetText(Strings.Get("entitybox", "trade"));
                TradeLabel.SetToolTipText(Strings.Get("entitybox", "tradetip", _myEntity.MyName));
                TradeLabel.SetPosition(117, 89);
                TradeLabel.TextColorOverride = Color.White;
                TradeLabel.MouseInputEnabled = true;
                TradeLabel.Clicked += tradeRequest_Clicked;

                PartyLabel = new Label(_entityInfoPanel);
                PartyLabel.SetText(Strings.Get("entitybox", "party"));
                PartyLabel.SetToolTipText(Strings.Get("entitybox", "partytip", _myEntity.MyName));
                PartyLabel.SetPosition(165, 89);
                PartyLabel.TextColorOverride = Color.White;
                PartyLabel.MouseInputEnabled = true;
                PartyLabel.Clicked += invite_Clicked;
                PartyLabel.IsHidden = Globals.Me.IsInMyParty(_myEntity);
            }

            _entityStatusPanel = new ImagePanel(_entityWindow);
            _entityStatusPanel.SetBounds(6, 118, 306, 72);
			UpdateSpellStatus();

			lastUpdateTime = Globals.System.GetTimeMS();
        }

        //Update
        public void Update()
        {
            if (_myEntity.IsDisposed()) Dispose();
            float elapsedTime = ((float) (Globals.System.GetTimeMS() - lastUpdateTime)) / 1000.0f;

            //Update the event/entity face.
            GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, _myEntity.Face);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,_myEntity.MySprite);
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
            else if (entityTex != null)
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

            //If not an event, update the hp/mana bars.
            if (_myEntity.GetType() != typeof(Event))
            {
                _entityLevel.SetText(Strings.Get("entitybox", "level", _myEntity.Level));
                _entityLevel.SetPosition(_entityInfoPanel.Width - 20 - _entityLevel.Width, 9);
                float targetHPWidth = 0f;
                if (_myEntity.MaxVital[(int) Vitals.Health] > 0)
                {
                    targetHPWidth = ((float) _myEntity.Vital[(int) Vitals.Health] /
                                     (float) _myEntity.MaxVital[(int) Vitals.Health]);
                    targetHPWidth = Math.Min(1, Math.Max(0, targetHPWidth));
                    //Fix the Labels
                    _hpLbl.Text = Strings.Get("entitybox", "vital0val", _myEntity.Vital[(int) Vitals.Health],
                        _myEntity.MaxVital[(int) Vitals.Health]);
                    //Multiply by the width of the bars.
                    targetHPWidth *= _hpBackground.Width;
                }
                else
                {
                    _hpLbl.Text = Strings.Get("entitybox", "vital0val", 0, 0);
                    targetHPWidth = _hpBackground.Width;
                }
                if ((int) targetHPWidth != _curHPWidth)
                {
                    if ((int) targetHPWidth > _curHPWidth)
                    {
                        _curHPWidth += (100f * elapsedTime);
                        if (_curHPWidth > (int) targetHPWidth)
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
                        _hpBar.Width = (int) _curHPWidth;
                        _hpBar.SetTextureRect(0, 0, (int) _curHPWidth, _hpBar.Height);
                        _hpBar.IsHidden = false;
                    }
                }
                float targetMPWidth = 0f;
                if (_myEntity.MaxVital[(int) Vitals.Mana] > 0)
                {
                    targetMPWidth = ((float) _myEntity.Vital[(int) Vitals.Mana] /
                                     (float) _myEntity.MaxVital[(int) Vitals.Mana]);
                    targetMPWidth = Math.Min(1, Math.Max(0, targetMPWidth));
                    _mpLbl.Text = Strings.Get("entitybox", "vital1val", _myEntity.Vital[(int) Vitals.Mana],
                        _myEntity.MaxVital[(int) Vitals.Mana]);
                    targetMPWidth *= _mpBackground.Width;
                }
                else
                {
                    _mpLbl.Text = Strings.Get("entitybox", "vital1val", 0, 0);
                    targetMPWidth = _mpBackground.Width;
                }
                if ((int) targetMPWidth != _curMPWidth)
                {
                    if ((int) targetMPWidth > _curMPWidth)
                    {
                        _curMPWidth += (100f * elapsedTime);
                        if (_curMPWidth > (int) targetMPWidth)
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
                        _mpBar.Width = (int) _curMPWidth;
                        _mpBar.SetTextureRect(0, 0, (int) _curMPWidth, _mpBar.Height);
                        _mpBar.IsHidden = false;
                    }
                }
            }

            //If player draw exp bar
            if (_myEntity == Globals.Me)
            {
                float targetExpWidth = 1;
                if (((Player) _myEntity).GetNextLevelExperience() > 0)
                {
                    targetExpWidth = (float) ((Player) _myEntity).Experience /
                                     (float) ((Player) _myEntity).GetNextLevelExperience();
                    _expLbl.Text = Strings.Get("entitybox", "expval", ((Player) _myEntity).Experience,
                        ((Player) _myEntity).GetNextLevelExperience());
                }
                else
                {
                    targetExpWidth = 1f;
                    _expLbl.Text = Strings.Get("entitybox", "maxlevel");
                }
                _expLbl.X = _expBackground.X + _expBackground.Width / 2 - _expLbl.Width / 2;
                targetExpWidth *= _expBackground.Width;
                if ((int) targetExpWidth != _curEXPWidth)
                {
                    if ((int) targetExpWidth > _curEXPWidth)
                    {
                        _curEXPWidth += (100f * elapsedTime);
                        if (_curEXPWidth > (int) targetExpWidth)
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
                        _expBar.Width = (int) _curEXPWidth;
                        _expBar.SetTextureRect(0, 0, (int) _curEXPWidth, _expBar.Height);
                        _expBar.IsHidden = false;
                    }
                }
            }
            else
            {
                if (PartyLabel != null)
                    PartyLabel.IsHidden = Globals.Me.IsInMyParty(_myEntity);
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
			    _entityStatusPanel.RemoveChild(s.container,true);
			    s.pnl_HoverLeave(null, null);
			}
			Items.Clear();

			//Add all of the spell status effects
			for (int i = 0; i < _myEntity.Status.Count; i++)
			{
				Items.Add(new SpellStatus(this, i));
				Items[i].container = new ImagePanel(_entityStatusPanel);
				Items[i].container.SetSize(34, 34);
                Items[i].container.SetPosition(
                    (i % (_entityStatusPanel.Width / (34 + StatusXPadding))) * (34 + StatusXPadding) + StatusXPadding,
                    (i / (_entityStatusPanel.Width / (34 + StatusXPadding))) * (34 + StatusYPadding) + StatusYPadding);
                Items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "skillitem.png");
				Items[i].Setup();
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

	public class SpellStatus
	{
		private SpellDescWindow _descWindow;

		//Drag/Drop References
		private EntityBox _entityBox;

		public ImagePanel container;
		private int currentSpell = -1;

		private int myindex;
		public ImagePanel pnl;
	    public Label timeLabel;

		private string texLoaded = "";

		public SpellStatus(EntityBox entityBox, int index)
		{
			_entityBox = entityBox;
			myindex = index;
		}

		public void Setup()
		{
			pnl = new ImagePanel(container);
			pnl.SetSize(32, 32);
			pnl.SetPosition(1, 1);
			pnl.IsHidden = true;
			pnl.HoverEnter += pnl_HoverEnter;
			pnl.HoverLeave += pnl_HoverLeave;
		}

		public void pnl_HoverLeave(Base sender, EventArgs arguments)
		{
			if (_descWindow != null)
			{
				_descWindow.Dispose();
				_descWindow = null;
			}
		}

		void pnl_HoverEnter(Base sender, EventArgs arguments)
		{
			if (myindex >= _entityBox._myEntity.Status.Count) { return; }
			if (_descWindow != null)
			{
				_descWindow.Dispose();
				_descWindow = null;
			}
			_descWindow = new SpellDescWindow(_entityBox._myEntity.Status[myindex].SpellNum, _entityBox._entityWindow.X + 316, _entityBox._entityWindow.Y);
		}

		public FloatRect RenderBounds()
		{
			FloatRect rect = new FloatRect()
			{
				X = pnl.LocalPosToCanvas(new Point(0, 0)).X,
				Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y,
				Width = pnl.Width,
				Height = pnl.Height
			};
			return rect;
		}

		public void Update()
		{
			var spell = SpellBase.Lookup.Get<SpellBase>(_entityBox._myEntity.Status[myindex].SpellNum);
		    var timeDiff = Globals.System.GetTimeMS() - _entityBox._myEntity.Status[myindex].TimeRecevied;
		    var remaining = _entityBox._myEntity.Status[myindex].TimeRemaining - timeDiff;
		    var fraction = (float) ((float) remaining / (float) _entityBox._myEntity.Status[myindex].TotalDuration);
            pnl.RenderColor = new Color((int)(fraction * 255f),255,255,255);
			if ((texLoaded != "" && spell == null) || (spell != null && texLoaded != spell.Pic) ||
				 currentSpell != _entityBox._myEntity.Status[myindex].SpellNum)
			{
				if (spell != null)
				{
					GameTexture spellTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Pic);
					if (spellTex != null)
					{
						pnl.Texture = spellTex;
						pnl.IsHidden = false;
					}
					else
					{
						if (pnl.Texture != null)
						{
							pnl.Texture = null;
						}
					}
					texLoaded = spell.Pic;
					currentSpell = _entityBox._myEntity.Status[myindex].SpellNum;
				}
				else
				{
					if (pnl.Texture != null)
					{
						pnl.Texture = null;
					}
					texLoaded = "";
				}
			}
		}
	}
}