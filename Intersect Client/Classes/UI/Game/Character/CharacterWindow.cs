using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.UI.Game.Character;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Game
{
    public class CharacterWindow
    {
        Label _abilityPwrLabel;
        Button _addAbilityPwrBtn;
        Button _addAttackBtn;
        Button _addDefenseBtn;
        Button _addMagicResistBtn;
        Button _addSpeedBtn;

        //Stats
        Label _attackLabel;

        private ImagePanel _characterContainer;
        private Label _characterLevelAndClass;

        private Label _characterName;
        private ImagePanel _characterPortrait;

        private string _characterPortraitImg = "";

        //Controls
        private WindowControl _characterWindow;

        private string _currentSprite = "";
        Label _defenseLabel;
        private int[] _emptyStatBoost = new int[Options.MaxStats];
        private ScrollControl _equipmentContainer;

        //Initialization
        private bool _initialized = false;

        private ImagePanel _itemTemplate;
        Label _magicRstLabel;
        public ImagePanel[] _paperdollPanels;
        public string[] _paperdollTextures;
        Label _pointsLabel;
        Label _speedLabel;

        //Equipment List
        public List<EquipmentItem> Items = new List<EquipmentItem>();

        //Location
        public int X;

        public int Y;

        //Init
        public CharacterWindow(Canvas _gameCanvas)
        {
            _characterWindow = new WindowControl(_gameCanvas, Strings.Get("character", "title"), false,
                "CharacterWindow");
            _characterWindow.DisableResizing();

            _characterName = new Label(_characterWindow, "CharacterNameLabel");
            _characterName.SetTextColor(Color.White, Label.ControlState.Normal);

            _characterLevelAndClass = new Label(_characterWindow, "ChatacterInfoLabel");
            _characterLevelAndClass.SetText("");

            _characterContainer = new ImagePanel(_characterWindow, "CharacterContainer");

            _characterPortrait = new ImagePanel(_characterContainer);

            _paperdollPanels = new ImagePanel[Options.EquipmentSlots.Count];
            _paperdollTextures = new string[Options.EquipmentSlots.Count];
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                _paperdollPanels[i] = new ImagePanel(_characterContainer);
                _paperdollTextures[i] = "";
                _paperdollPanels[i].Hide();
            }

            Label equipmentLabel = new Label(_characterWindow, "EquipmentLabel");
            equipmentLabel.SetText(Strings.Get("character", "equipment"));

            _equipmentContainer = new ScrollControl(_characterWindow, "EquipmentContainer");
            _equipmentContainer.EnableScroll(true, false);

            _itemTemplate = new ImagePanel(_equipmentContainer, "EquipmentBox");
            new ImagePanel(_itemTemplate, "EquipmentIcon");

            Label statsLabel = new Label(_characterWindow, "StatsLabel");
            statsLabel.SetText(Strings.Get("character", "stats"));

            _attackLabel = new Label(_characterWindow, "AttackLabel");

            _addAttackBtn = new Button(_characterWindow, "IncreaseAttackButton");
            _addAttackBtn.Clicked += _addAttackBtn_Clicked;

            _defenseLabel = new Label(_characterWindow, "DefenseLabel");
            _addDefenseBtn = new Button(_characterWindow, "IncreaseDefenseButton");
            _addDefenseBtn.Clicked += _addDefenseBtn_Clicked;

            _speedLabel = new Label(_characterWindow, "SpeedLabel");
            _addSpeedBtn = new Button(_characterWindow, "IncreaseSpeedButton");
            _addSpeedBtn.Clicked += _addSpeedBtn_Clicked;

            _abilityPwrLabel = new Label(_characterWindow, "AbilityPowerLabel");
            _addAbilityPwrBtn = new Button(_characterWindow, "IncreaseAbilityPowerButton");
            _addAbilityPwrBtn.Clicked += _addAbilityPwrBtn_Clicked;

            _magicRstLabel = new Label(_characterWindow, "MagicResistLabel");
            _addMagicResistBtn = new Button(_characterWindow, "IncreaseMagicResistButton");
            _addMagicResistBtn.Clicked += _addMagicResistBtn_Clicked;

            _pointsLabel = new Label(_characterWindow, "PointsLabel");
        }

        //Update Button Event Handlers
        void _addMagicResistBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stats.MagicResist);
        }

        void _addAbilityPwrBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stats.AbilityPower);
        }

        void _addSpeedBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stats.Speed);
        }

        void _addDefenseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stats.Defense);
        }

        void _addAttackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stats.Attack);
        }

        private void InitEquipmentContainer()
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Items.Add(new EquipmentItem(i, _characterWindow));
                Items[i].pnl = new ImagePanel(_equipmentContainer, "EquipmentBox");
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].pnl, "InGame.xml");

                Items[i].pnl
                    .SetPosition(
                        Items[i].pnl.Padding.Left + (i * (Items[i].pnl.Padding.Left + Items[i].pnl.Padding.Right +
                                                          Items[i].pnl.Width)), Items[i].pnl.Padding.Top);
            }
            _itemTemplate.Hide();
        }

        //Methods
        public void Update()
        {
            if (!_initialized)
            {
                InitEquipmentContainer();
                _initialized = true;
            }
            if (_characterWindow.IsHidden)
            {
                return;
            }
            _characterName.Text = Globals.Me.MyName;
            _characterLevelAndClass.Text = Strings.Get("character", "levelandclass", Globals.Me.Level,
                ClassBase.GetName(Globals.Me.Class));

            //Load Portrait
            //UNCOMMENT THIS LINE IF YOU'D RATHER HAVE A FACE HERE GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, Globals.Me.Face);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                Globals.Me.MySprite);
            /* UNCOMMENT THIS BLOCK IF YOU"D RATHER HAVE A FACE HERE if (Globals.Me.Face != "" && Globals.Me.Face != _currentSprite && faceTex != null)
             {
                 _characterPortrait.Texture = faceTex;
                 _characterPortrait.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                 _characterPortrait.SizeToContents();
                 Align.Center(_characterPortrait);
                 _characterPortrait.IsHidden = false;
                 for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                 {
                     _paperdollPanels[i].Hide();
                 }
             }
             else */
            if (Globals.Me.MySprite != "" && Globals.Me.MySprite != _currentSprite && entityTex != null)
            {
                _characterPortrait.Texture = entityTex;
                _characterPortrait.SetTextureRect(0, 0, entityTex.GetWidth() / 4, entityTex.GetHeight() / 4);
                _characterPortrait.SizeToContents();
                Align.Center(_characterPortrait);
                _characterPortrait.IsHidden = false;
                for (int z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    var paperdoll = "";
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                    {
                        var Equipment = Globals.Me.Equipment;
                        if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 &&
                            Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] <
                            Options.MaxInvItems)
                        {
                            var itemNum = Globals.Me.Inventory[
                                    Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])
                                    ]]
                                .ItemNum;
                            if (ItemBase.Lookup.Get<ItemBase>(itemNum) != null)
                            {
                                var itemdata = ItemBase.Lookup.Get<ItemBase>(itemNum);
                                if (Globals.Me.Gender == 0)
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
                        var _paperdollTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll,
                            paperdoll);
                        _paperdollPanels[z].Texture = _paperdollTex;
                        if (_paperdollTex != null)
                        {
                            _paperdollPanels[z].SetTextureRect(0, 0,
                                _paperdollPanels[z].Texture.GetWidth() / 4,
                                _paperdollPanels[z].Texture.GetHeight() / 4);
                            _paperdollPanels[z].SetSize(_paperdollPanels[z].Texture.GetWidth() / 4,
                                _paperdollPanels[z].Texture.GetHeight() / 4);
                            _paperdollPanels[z].SetPosition(
                                _characterContainer.Width / 2 - _paperdollPanels[z].Width / 2,
                                _characterContainer.Height / 2 - _paperdollPanels[z].Height / 2);
                        }
                        _paperdollPanels[z].Show();
                        _paperdollTextures[z] = paperdoll;
                    }
                }
            }
            else if (Globals.Me.MySprite != _currentSprite && Globals.Me.Face != _currentSprite)
            {
                _characterPortrait.IsHidden = true;
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPanels[i].Hide();
                }
            }

            _attackLabel.SetText(Strings.Get("character", "stat0", Strings.Get("combat", "stat0"),
                Globals.Me.Stat[(int) Stats.Attack]));
            _defenseLabel.SetText(Strings.Get("character", "stat2", Strings.Get("combat", "stat2"),
                Globals.Me.Stat[(int) Stats.Defense]));
            _speedLabel.SetText(Strings.Get("character", "stat4", Strings.Get("combat", "stat4"),
                Globals.Me.Stat[(int) Stats.Speed]));
            _abilityPwrLabel.SetText(Strings.Get("character", "stat1", Strings.Get("combat", "stat1"),
                Globals.Me.Stat[(int) Stats.AbilityPower]));
            _magicRstLabel.SetText(Strings.Get("character", "stat3", Strings.Get("combat", "stat3"),
                Globals.Me.Stat[(int) Stats.MagicResist]));
            _pointsLabel.SetText(Strings.Get("character", "points", Globals.Me.StatPoints));
            _addAbilityPwrBtn.IsHidden = (Globals.Me.StatPoints == 0 ||
                                          Globals.Me.Stat[(int) Stats.AbilityPower] == Options.MaxStatValue);
            _addAttackBtn.IsHidden = (Globals.Me.StatPoints == 0 ||
                                      Globals.Me.Stat[(int) Stats.Attack] == Options.MaxStatValue);
            _addDefenseBtn.IsHidden = (Globals.Me.StatPoints == 0 ||
                                       Globals.Me.Stat[(int) Stats.Defense] == Options.MaxStatValue);
            _addMagicResistBtn.IsHidden = (Globals.Me.StatPoints == 0 ||
                                           Globals.Me.Stat[(int) Stats.MagicResist] == Options.MaxStatValue);
            _addSpeedBtn.IsHidden = (Globals.Me.StatPoints == 0 ||
                                     Globals.Me.Stat[(int) Stats.Speed] == Options.MaxStatValue);

            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.Equipment[i] > -1 && Globals.Me.Equipment[i] < Options.MaxInvItems)
                {
                    if (Globals.Me.Inventory[Globals.Me.Equipment[i]].ItemNum > -1)
                    {
                        Items[i].Update(Globals.Me.Inventory[Globals.Me.Equipment[i]].ItemNum,
                            Globals.Me.Inventory[Globals.Me.Equipment[i]].StatBoost);
                    }
                    else
                    {
                        Items[i].Update(-1, _emptyStatBoost);
                    }
                }
                else
                {
                    Items[i].Update(-1, _emptyStatBoost);
                }
            }
        }

        public void Show()
        {
            _characterWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_characterWindow.IsHidden;
        }

        public void Hide()
        {
            _characterWindow.IsHidden = true;
        }
    }
}