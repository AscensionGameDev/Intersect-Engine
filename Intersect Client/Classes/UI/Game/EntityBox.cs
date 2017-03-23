using System;
using Intersect;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Game
{
    public class EntityBox
    {
        public float _curEXPWidth;

        public float _curHPWidth;
        public float _curMPWidth;
        private string _currentSprite = "";
        //Controls
        public ImagePanel _entityBox;
        public ImagePanel _entityFace;
        public ImagePanel _entityFaceContainer;
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

        private Entity _myEntity;

        private long lastUpdateTime;
        public Label PartyLabel;
        public Label TradeLabel;

        //Init
        public EntityBox(Canvas _gameCanvas, Entity myEntity, int x, int y)
        {
            _myEntity = myEntity;

            _entityBox = new ImagePanel(_gameCanvas);
            _entityBox.SetSize(314, 126);
            _entityBox.SetPosition(x, y);
            _entityBox.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "entitybox.png");

            _entityName = new Label(_entityBox);
            _entityName.SetPosition(16, 7);
            _entityName.SetText(myEntity.MyName);
            _entityName.SetTextColor(Color.White, Label.ControlState.Normal);
            _entityName.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 14);

            _entityFaceContainer = new ImagePanel(_entityBox);
            _entityFaceContainer.SetSize(64, 64);
            _entityFaceContainer.SetPosition(18, 39);

            _entityFace = new ImagePanel(_entityFaceContainer);
            _entityFace.SetSize(64, 64);
            _entityFace.SetPosition(0, 0);

            if (myEntity.GetType() == typeof(Event))
            {
                _eventDesc = new RichLabel(_entityBox);
                _eventDesc.SetPosition(93, 37);
                _eventDesc.Width = 207;
                _eventDesc.AddText(((Event) _myEntity).Desc, Color.White);
                _eventDesc.SizeToChildren(false, true);
            }
            else
            {
                _entityLevel = new Label(_entityBox);
                _entityLevel.SetPosition(0, 0);
                _entityLevel.SetTextColor(Color.White, Label.ControlState.Normal);
                _entityLevel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);
            }

            if (myEntity.GetType() != typeof(Event))
            {
                _hpBackground = new ImagePanel(_entityBox)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _hpBackground.SetSize(183, 25);
                _hpBackground.SetPosition(117, 32);

                _hpBar = new ImagePanel(_entityBox);
                _hpBar.SetSize(183, 25);
                _hpBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "lifebar.png");
                _hpBar.SetPosition(117, 32);
                _hpBar.IsHidden = true;

                _hpTitle = new Label(_entityBox);
                _hpTitle.SetText(Strings.Get("entitybox", "vital0"));
                _hpTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _hpTitle.SetPosition(93, 37);

                _hpLbl = new Label(_entityBox)
                {
                    Alignment = Pos.Center,
                    AutoSizeToContents = false
                };
                _hpLbl.SetPosition(_hpBackground.X, _hpBackground.Y);
                _hpLbl.SetSize(_hpBackground.Width, _hpBackground.Height);
                _hpLbl.SetTextColor(Color.White, Label.ControlState.Normal);

                _mpBackground = new ImagePanel(_entityBox)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _mpBackground.SetSize(183, 25);
                _mpBackground.SetPosition(117, 58);

                _mpBar = new ImagePanel(_entityBox);
                _mpBar.SetSize(183, 25);
                _mpBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "manabar.png");
                _mpBar.SetPosition(117, 58);
                _mpBar.IsHidden = true;

                _mpTitle = new Label(_entityBox);
                _mpTitle.SetText(Strings.Get("entitybox", "vital1"));
                _mpTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _mpTitle.SetPosition(93, 63);

                _mpLbl = new Label(_entityBox)
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
                _expBackground = new ImagePanel(_entityBox)
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "emptybar.png")
                };
                _expBackground.SetSize(183, 25);
                _expBackground.SetPosition(117, 84);

                _expBar = new ImagePanel(_entityBox);
                _expBar.SetSize(183, 25);
                _expBar.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "expbar.png");
                _expBar.SetPosition(117, 84);
                _expBar.IsHidden = true;

                _expTitle = new Label(_entityBox);
                _expTitle.SetText(Strings.Get("entitybox", "exp"));
                _expTitle.SetTextColor(Color.White, Label.ControlState.Normal);
                _expTitle.SetPosition(88, 89);

                _expLbl = new Label(_entityBox)
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
                TradeLabel = new Label(_entityBox);
                TradeLabel.SetText(Strings.Get("entitybox", "trade"));
                TradeLabel.SetToolTipText(Strings.Get("entitybox", "tradetip", _myEntity.MyName));
                TradeLabel.SetPosition(117, 89);
                TradeLabel.TextColorOverride = Color.White;
                TradeLabel.MouseInputEnabled = true;
                TradeLabel.Clicked += tradeRequest_Clicked;

                PartyLabel = new Label(_entityBox);
                PartyLabel.SetText(Strings.Get("entitybox", "party"));
                PartyLabel.SetToolTipText(Strings.Get("entitybox", "partytip", _myEntity.MyName));
                PartyLabel.SetPosition(165, 89);
                PartyLabel.TextColorOverride = Color.White;
                PartyLabel.MouseInputEnabled = true;
                PartyLabel.Clicked += invite_Clicked;
                PartyLabel.IsHidden = Globals.Me.IsInMyParty(_myEntity);
            }

            lastUpdateTime = Globals.System.GetTimeMS();
        }

        //Update
        public void Update()
        {
            if (_myEntity.IsDisposed()) Dispose();
            float elapsedTime = ((float) (Globals.System.GetTimeMS() - lastUpdateTime)) / 1000.0f;

            //Update the event/entity face.
            GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, _myEntity.Face);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                _myEntity.MySprite);
            if (faceTex != null)
            {
                _entityFace.Texture = faceTex;
                _entityFace.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                _entityFace.SizeToContents();
                Align.Center(_entityFace);
                _currentSprite = _myEntity.Face;
                _entityFace.IsHidden = false;
            }
            else if (entityTex != null)
            {
                _entityFace.Texture = entityTex;
                _entityFace.SetTextureRect(0, 0, entityTex.GetWidth() / 4, entityTex.GetHeight() / 4);
                _entityFace.SizeToContents();
                Align.Center(_entityFace);
                _currentSprite = _myEntity.MySprite;
                _entityFace.IsHidden = false;
            }
            else if (_myEntity.MySprite != _currentSprite && _myEntity.Face != _currentSprite)
            {
                _entityFace.IsHidden = true;
            }

            //If not an event, update the hp/mana bars.
            if (_myEntity.GetType() != typeof(Event))
            {
                _entityLevel.SetText(Strings.Get("entitybox", "level", _myEntity.Level));
                _entityLevel.SetPosition(_entityBox.Width - 20 - _entityLevel.Width, 9);
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

            //Eventually draw icons for buffs and debuffs?
            lastUpdateTime = Globals.System.GetTimeMS();
        }

        public void Dispose()
        {
            _entityBox.Hide();
            Gui.GameUI.GameCanvas.RemoveChild(_entityBox, false);
            _entityBox.Dispose();
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