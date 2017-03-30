using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

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
        Label _magicRstLabel;
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
            _characterWindow = new WindowControl(_gameCanvas, Strings.Get("character", "title"));
            _characterWindow.SetSize(228, 320);
            _characterWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210,
                GameGraphics.Renderer.GetScreenHeight() - 500);
            _characterWindow.DisableResizing();
            _characterWindow.Margin = Margin.Zero;
            _characterWindow.Padding = new Padding(8, 5, 9, 11);
            _characterWindow.IsHidden = true;

            _characterWindow.SetTitleBarHeight(24);
            _characterWindow.SetCloseButtonSize(20, 20);
            _characterWindow.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "characteractive.png"),
                WindowControl.ControlState.Active);
            _characterWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"),
                Button.ControlState.Normal);
            _characterWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"),
                Button.ControlState.Hovered);
            _characterWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"),
                Button.ControlState.Clicked);
            _characterWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _characterWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _characterName = new Label(_characterWindow);
            _characterName.SetPosition(4, 4);
            _characterName.AutoSizeToContents = false;
            _characterName.Alignment = Pos.Center;
            _characterName.SetSize(200, 24);
            _characterName.SetText("");
            _characterName.SetTextColor(Color.White, Label.ControlState.Normal);
            _characterName.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);

            _characterLevelAndClass = new Label(_characterWindow);
            _characterLevelAndClass.SetPosition(4, 28);
            _characterLevelAndClass.AutoSizeToContents = false;
            _characterLevelAndClass.Alignment = Pos.Center;
            _characterLevelAndClass.SetSize(200, 12);
            _characterLevelAndClass.SetText("");
            _characterLevelAndClass.SetTextColor(Color.White, Label.ControlState.Normal);
            _characterLevelAndClass.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 10);

            _characterContainer = new ImagePanel(_characterWindow);
            _characterContainer.SetSize(100, 100);
            _characterContainer.SetPosition(200 / 2 - 100 / 2, 36);

            _characterPortrait = new ImagePanel(_characterContainer);
            _characterPortrait.SetSize(100, 100);
            _characterPortrait.SetPosition(200 / 2 - 100 / 2, 36);

            Label equipmentLabel = new Label(_characterWindow);
            equipmentLabel.SetPosition(4, 126);
            equipmentLabel.SetText(Strings.Get("character", "equipment"));
            equipmentLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _equipmentContainer = new ScrollControl(_characterWindow);
            _equipmentContainer.SetPosition(5, 144);
            _equipmentContainer.SetSize(
                _characterWindow.Width - _characterWindow.Padding.Left - _characterWindow.Padding.Right - 10, 38);

            Label statsLabel = new Label(_characterWindow);
            statsLabel.SetPosition(4, 204);
            statsLabel.SetText(Strings.Get("character", "stats"));
            statsLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _attackLabel = new Label(_characterWindow);
            _attackLabel.SetPosition(4, 220);
            _attackLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _addAttackBtn = new Button(_characterWindow);
            _addAttackBtn.SetSize(15, 15);
            _addAttackBtn.SetPosition(76, 220);
            _addAttackBtn.Clicked += _addAttackBtn_Clicked;
            _addAttackBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatnormal.png"),
                Button.ControlState.Normal);
            _addAttackBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstathover.png"),
                Button.ControlState.Hovered);
            _addAttackBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatclicked.png"),
                Button.ControlState.Clicked);

            _defenseLabel = new Label(_characterWindow);
            _defenseLabel.SetPosition(4, 238);
            _defenseLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _addDefenseBtn = new Button(_characterWindow);
            _addDefenseBtn.SetSize(15, 15);
            _addDefenseBtn.SetPosition(76, 238);
            _addDefenseBtn.Clicked += _addDefenseBtn_Clicked;
            _addDefenseBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatnormal.png"),
                Button.ControlState.Normal);
            _addDefenseBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstathover.png"),
                Button.ControlState.Hovered);
            _addDefenseBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatclicked.png"),
                Button.ControlState.Clicked);

            _speedLabel = new Label(_characterWindow);
            _speedLabel.SetPosition(4, 256);
            _speedLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _addSpeedBtn = new Button(_characterWindow);
            _addSpeedBtn.SetSize(15, 15);
            _addSpeedBtn.SetPosition(76, 256);
            _addSpeedBtn.Clicked += _addSpeedBtn_Clicked;
            _addSpeedBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatnormal.png"),
                Button.ControlState.Normal);
            _addSpeedBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstathover.png"),
                Button.ControlState.Hovered);
            _addSpeedBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatclicked.png"),
                Button.ControlState.Clicked);

            _abilityPwrLabel = new Label(_characterWindow);
            _abilityPwrLabel.SetPosition(96, 220);
            _abilityPwrLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _addAbilityPwrBtn = new Button(_characterWindow);
            _addAbilityPwrBtn.SetSize(15, 15);
            _addAbilityPwrBtn.SetPosition(196, 220);
            _addAbilityPwrBtn.Clicked += _addAbilityPwrBtn_Clicked;
            _addAbilityPwrBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatnormal.png"),
                Button.ControlState.Normal);
            _addAbilityPwrBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstathover.png"),
                Button.ControlState.Hovered);
            _addAbilityPwrBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatclicked.png"),
                Button.ControlState.Clicked);

            _magicRstLabel = new Label(_characterWindow);
            _magicRstLabel.SetPosition(96, 238);
            _magicRstLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            _addMagicResistBtn = new Button(_characterWindow);
            _addMagicResistBtn.SetSize(15, 15);
            _addMagicResistBtn.SetPosition(196, 238);
            _addMagicResistBtn.Clicked += _addMagicResistBtn_Clicked;
            _addMagicResistBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatnormal.png"),
                Button.ControlState.Normal);
            _addMagicResistBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstathover.png"),
                Button.ControlState.Hovered);
            _addMagicResistBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "addstatclicked.png"),
                Button.ControlState.Clicked);

            _pointsLabel = new Label(_characterWindow);
            _pointsLabel.SetPosition(96, 256);
            _pointsLabel.SetTextColor(Color.White, Label.ControlState.Normal);

            InitEquipmentContainer();
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
            int x = 0;
            int w = 38 * Options.EquipmentSlots.Count;
            if (w > _characterWindow.Width - _characterWindow.Padding.Left - _characterWindow.Padding.Right - 10)
            {
                _equipmentContainer.EnableScroll(true, false);
                _equipmentContainer.SetSize(
                    _characterWindow.Width - _characterWindow.Padding.Left - _characterWindow.Padding.Right - 10,
                    38 + 16);
            }
            else
            {
                _equipmentContainer.SetSize(w, 38 + 16);
                _equipmentContainer.SetPosition(
                    (_characterWindow.Width - _characterWindow.Padding.Right) / 2 - _equipmentContainer.Width / 2 + 5,
                    _equipmentContainer.Y);
                _equipmentContainer.EnableScroll(false, false);
            }
            var scrollbar = _equipmentContainer.GetHorizontalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"),
                Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"),
                Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"),
                Dragger.ControlState.Clicked);

            var leftButton = scrollbar.GetScrollBarButton(Pos.Left);
            leftButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrownormal.png"),
                Button.ControlState.Normal);
            leftButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowclicked.png"),
                Button.ControlState.Clicked);
            leftButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowhover.png"),
                Button.ControlState.Hovered);
            var rightButton = scrollbar.GetScrollBarButton(Pos.Right);
            rightButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrownormal.png"),
                Button.ControlState.Normal);
            rightButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrowclicked.png"),
                Button.ControlState.Clicked);
            rightButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrowhover.png"),
                Button.ControlState.Hovered);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Items.Add(new EquipmentItem(i,
                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "equipmentitem.png"),
                    _characterWindow));
                Items[i].pnl = new ImagePanel(_equipmentContainer);
                Items[i].pnl.SetSize(34, 34);
                Items[i].pnl.SetPosition(x + i * 36, 2);
                Items[i].pnl.IsHidden = false;
                Items[i].Setup();
            }
        }

        //Methods
        public void Update()
        {
            if (_characterWindow.IsHidden)
            {
                return;
            }
            _characterName.Text = Globals.Me.MyName;
            _characterLevelAndClass.Text = Strings.Get("character", "levelandclass", Globals.Me.Level,
                ClassBase.GetName(Globals.Me.Class));

            //Load Portrait
            GameTexture faceTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, Globals.Me.Face);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                Globals.Me.MySprite);
            if (Globals.Me.Face != "" && Globals.Me.Face != _currentSprite && faceTex != null)
            {
                _characterPortrait.Texture = faceTex;
                _characterPortrait.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                _characterPortrait.SizeToContents();
                Align.Center(_characterPortrait);
                _characterPortrait.IsHidden = false;
            }
            else if (Globals.Me.MySprite != "" && Globals.Me.MySprite != _currentSprite && entityTex != null)
            {
                _characterPortrait.Texture = entityTex;
                _characterPortrait.SetTextureRect(0, 0, entityTex.GetWidth() / 4, entityTex.GetHeight() / 4);
                _characterPortrait.SizeToContents();
                Align.Center(_characterPortrait);
                _characterPortrait.IsHidden = false;
            }
            else if (Globals.Me.MySprite != _currentSprite && Globals.Me.Face != _currentSprite)
            {
                _characterPortrait.IsHidden = true;
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

    public class EquipmentItem
    {
        private WindowControl _characterWindow;
        private int _currentItem = -1;
        private ItemDescWindow _descWindow;
        private GameTexture _equipmentBG;
        private int[] _statBoost = new int[Options.MaxStats];
        private bool _texLoaded = false;
        public ImagePanel contentPanel;
        private int myindex;
        public ImagePanel pnl;

        public EquipmentItem(int index, GameTexture equipmentBG, WindowControl characterWindow)
        {
            myindex = index;
            _equipmentBG = equipmentBG;
            _characterWindow = characterWindow;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;

            contentPanel = new ImagePanel(pnl);
            contentPanel.SetSize(32, 32);
            contentPanel.SetPosition(2, 2);
            contentPanel.MouseInputEnabled = false;
            pnl.SetToolTipText(Options.EquipmentSlots[myindex]);

            pnl.Texture = _equipmentBG;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUnequipItem(myindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            if (ItemBase.Lookup.Get<ItemBase>(_currentItem) == null) return;
            _descWindow = new ItemDescWindow(_currentItem, 1, _characterWindow.X - 255, _characterWindow.Y, _statBoost,
                "Equipment Slot: " + Options.EquipmentSlots[myindex]);
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

        public void Update(int currentItem, int[] statBoost)
        {
            if (currentItem != _currentItem || !_texLoaded)
            {
                _currentItem = currentItem;
                _statBoost = statBoost;
                var item = ItemBase.Lookup.Get<ItemBase>(_currentItem);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Pic);
                    if (itemTex != null)
                    {
                        contentPanel.Show();
                        contentPanel.Texture = itemTex;
                    }
                    else
                    {
                        contentPanel.Hide();
                    }
                }
                else
                {
                    contentPanel.Hide();
                }
                _texLoaded = true;
            }
        }
    }
}