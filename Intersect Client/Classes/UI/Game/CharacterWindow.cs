/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;

namespace Intersect_Client.Classes.UI.Game
{
    public class CharacterWindow
    {
        //Controls
        private WindowControl _characterWindow;
        private ScrollControl _equipmentContainer;

        private Label _characterName;
        private Label _characterLevel;

        private string _characterPortraitImg = "";
        private ImagePanel _characterPortrait;
        private GameRenderTexture _spriteTex;
        private string _currentSprite = "";

        private GameRenderTexture _equipmentBG;
        private int[] _emptyStatBoost = new int[Options.MaxStats];

        //Stats
        Label _attackLabel;
        Label _defenseLabel;
        Label _speedLabel;
        Label _abilityPwrLabel;
        Label _magicRstLabel;
        Label _pointsLabel;
        Button _addAttackBtn;
        Button _addDefenseBtn;
        Button _addAbilityPwrBtn;
        Button _addMagicResistBtn;
        Button _addSpeedBtn;


        //Location
        public int X;
        public int Y;

        //Equipment List
        public List<EquipmentItem> Items = new List<EquipmentItem>();

        //Init
        public CharacterWindow(Canvas _gameCanvas)
        {
            _characterWindow = new WindowControl(_gameCanvas, "Character");
            _characterWindow.SetSize(200, 300);
            _characterWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 210, GameGraphics.Renderer.GetScreenHeight() - 500);
            _characterWindow.DisableResizing();
            _characterWindow.Margin = Margin.Zero;
            _characterWindow.Padding = Padding.Zero;
            _characterWindow.IsHidden = true;

            _characterName = new Label(_characterWindow);
            _characterName.SetPosition(4, 4);
            _characterName.AutoSizeToContents = false;
            _characterName.Alignment = Pos.Center;
            _characterName.SetSize(200, 12);
            _characterName.SetText("Name");

            _characterLevel = new Label(_characterWindow);
            _characterLevel.SetPosition(4, 18);
            _characterLevel.AutoSizeToContents = false;
            _characterLevel.Alignment = Pos.Center;
            _characterLevel.SetSize(200, 12);
            _characterLevel.SetText("Level: " + 1);

            _characterPortrait = new ImagePanel(_characterWindow);
            _characterPortrait.SetSize(100, 100);
            _characterPortrait.SetPosition(200 / 2 - 100 / 2, 36);

            Label equipmentLabel = new Label(_characterWindow);
            equipmentLabel.SetPosition(4, 146);
            equipmentLabel.SetText("Equipment:");

            _equipmentContainer = new ScrollControl(_characterWindow);
            _equipmentContainer.SetPosition(5, 156);
            _equipmentContainer.SetSize(_characterWindow.Width - 10, 38);

            Label statsLabel = new Label(_characterWindow);
            statsLabel.SetPosition(4, 216);
            statsLabel.SetText("Stats: ");

            _attackLabel = new Label(_characterWindow);
            _attackLabel.SetPosition(4, 230);
            _attackLabel.SetText("Attack: ");

            _addAttackBtn = new Button(_characterWindow);
            _addAttackBtn.SetSize(12, 12);
            _addAttackBtn.SetText("+");
            _addAttackBtn.SetPosition(90 - 20, 230);
            _addAttackBtn.Clicked += _addAttackBtn_Clicked;

            _defenseLabel = new Label(_characterWindow);
            _defenseLabel.SetPosition(4, 244);
            _defenseLabel.SetText("Defense: ");

            _addDefenseBtn = new Button(_characterWindow);
            _addDefenseBtn.SetSize(12, 12);
            _addDefenseBtn.SetText("+");
            _addDefenseBtn.SetPosition(90 - 20, 244);
            _addDefenseBtn.Clicked += _addDefenseBtn_Clicked;

            _speedLabel = new Label(_characterWindow);
            _speedLabel.SetPosition(4, 258);
            _speedLabel.SetText("Speed: ");

            _addSpeedBtn = new Button(_characterWindow);
            _addSpeedBtn.SetSize(12, 12);
            _addSpeedBtn.SetText("+");
            _addSpeedBtn.SetPosition(90 - 20, 258);
            _addSpeedBtn.Clicked += _addSpeedBtn_Clicked;

            _abilityPwrLabel = new Label(_characterWindow);
            _abilityPwrLabel.SetPosition(90, 230);
            _abilityPwrLabel.SetText("Ability Pwr: ");

            _addAbilityPwrBtn = new Button(_characterWindow);
            _addAbilityPwrBtn.SetSize(12, 12);
            _addAbilityPwrBtn.SetText("+");
            _addAbilityPwrBtn.SetPosition(200 - 20, 230);
            _addAbilityPwrBtn.Clicked += _addAbilityPwrBtn_Clicked;

            _magicRstLabel = new Label(_characterWindow);
            _magicRstLabel.SetPosition(90, 244);
            _magicRstLabel.SetText("Magic Resist: ");

            _addMagicResistBtn = new Button(_characterWindow);
            _addMagicResistBtn.SetSize(12, 12);
            _addMagicResistBtn.SetText("+");
            _addMagicResistBtn.SetPosition(200 - 20, 244);
            _addMagicResistBtn.Clicked += _addMagicResistBtn_Clicked;

            _pointsLabel = new Label(_characterWindow);
            _pointsLabel.SetPosition(90, 258);
            _pointsLabel.SetText("Points: ");


            //Create equipment background image=
            GameRenderTexture rtEquipment = GameGraphics.Renderer.CreateRenderTexture(34, 34);
            rtEquipment.Begin();
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1), new FloatRect(0, 0, 1, 34),
                Color.Black, rtEquipment);
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1), new FloatRect(33, 0, 1, 34),
                Color.Black, rtEquipment);
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1), new FloatRect(0, 0, 34, 1),
                Color.Black, rtEquipment);
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1), new FloatRect(0, 33, 34, 1),
                Color.Black, rtEquipment);
            rtEquipment.End();
            _equipmentBG = rtEquipment;

            InitEquipmentContainer();
        }


        //Update Button Event Handlers
        void _addMagicResistBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Enums.Stats.MagicResist);
        }

        void _addAbilityPwrBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Enums.Stats.AbilityPower);
        }

        void _addSpeedBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Enums.Stats.Speed);
        }

        void _addDefenseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Enums.Stats.Defense);
        }

        void _addAttackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Enums.Stats.Attack);
        }

        private void InitEquipmentContainer()
        {
            int x = 0;
            int w = 38 * Options.EquipmentSlots.Count;
            if (w > _characterWindow.Width - 10)
            {
                _equipmentContainer.EnableScroll(true, false);
                _equipmentContainer.SetSize(_characterWindow.Width - 10, 38 + 16);
            }
            else
            {
                x = (_characterWindow.Width - 10) / 2 - (w - 4) / 2;
                _equipmentContainer.EnableScroll(false, false);
            }
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Items.Add(new EquipmentItem(i, _equipmentBG, _characterWindow));
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
            if (_characterWindow.IsHidden) { return; }
            _characterName.Text = Globals.Me.MyName;
            _characterLevel.Text = "Level: " + Globals.Me.Level;
            if (Globals.Me.Face != "")
            {
                if (_characterPortrait.ImageName != "Resources/Faces/" + Globals.Me.Face)
                {
                    _characterPortrait.ImageName = "Resources/Faces/" + Globals.Me.Face;
                }
            }
            else
            {
                if (Globals.Me.MySprite != "")
                {
                    if (_currentSprite != Globals.Me.MySprite)
                    {
                        _spriteTex = Gui.CreateTextureFromSprite(Globals.Me.MySprite, _characterPortrait.Width,
                            _characterPortrait.Height);
                        _characterPortrait.Texture = Gui.ToGwenTexture(_spriteTex);
                        _currentSprite = Globals.Me.MySprite;
                    }
                }
            }

            _attackLabel.SetText("Attack: " + Globals.Me.Stat[(int)Enums.Stats.Attack]);
            _defenseLabel.SetText("Defense: " + Globals.Me.Stat[(int)Enums.Stats.Defense]);
            _speedLabel.SetText("Speed: " + Globals.Me.Stat[(int)Enums.Stats.Speed]);
            _abilityPwrLabel.SetText("Ability Pwr: " + Globals.Me.Stat[(int)Enums.Stats.AbilityPower]);
            _magicRstLabel.SetText("Magic Resist: " + Globals.Me.Stat[(int)Enums.Stats.MagicResist]);
            _pointsLabel.SetText("Points: " + Globals.Me.StatPoints);
            _addAbilityPwrBtn.IsHidden = (Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int)Enums.Stats.AbilityPower] == Options.MaxStatValue);
            _addAttackBtn.IsHidden = (Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int)Enums.Stats.Attack] == Options.MaxStatValue);
            _addDefenseBtn.IsHidden = (Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int)Enums.Stats.Defense] == Options.MaxStatValue);
            _addMagicResistBtn.IsHidden = (Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int)Enums.Stats.MagicResist] == Options.MaxStatValue);
            _addSpeedBtn.IsHidden = (Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int)Enums.Stats.Speed] == Options.MaxStatValue);

            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.Equipment[i] > -1)
                {
                    if (Globals.Me.Inventory[Globals.Me.Equipment[i]].ItemNum > -1)
                    {
                        Items[i].Update(Globals.Me.Inventory[Globals.Me.Equipment[i]].ItemNum, Globals.Me.Inventory[Globals.Me.Equipment[i]].StatBoost);
                    }
                    else
                    {
                        Items[i].Update(-1,_emptyStatBoost);
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
        public ImagePanel pnl;
        private ItemDescWindow _descWindow;
        private int myindex;
        private int _currentItem = -1;
        private int[] _statBoost = new int[Options.MaxStats];
        private bool _texLoaded = false;
        private GameRenderTexture _equipmentBG;
        private GameRenderTexture _texImg;
        private WindowControl _characterWindow;

        public EquipmentItem(int index, GameRenderTexture equipmentBG, WindowControl characterWindow)
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
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUnequipItem(myindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)) { return; }
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            _descWindow = new ItemDescWindow(_currentItem, 1, _characterWindow.X - 220, _characterWindow.Y, _statBoost, "Equipment Slot: " + Options.EquipmentSlots[myindex]);
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = pnl.LocalPosToCanvas(new Point(0, 0)).X;
            rect.Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y;
            rect.Width = pnl.Width;
            rect.Height = pnl.Height;
            return rect;
        }

        private void CreateTexImage()
        {
            if (_texImg == null)
            {
                GameRenderTexture rtItem = GameGraphics.Renderer.CreateRenderTexture(34, 34);
                _texImg = rtItem;
                _texImg.Begin();
            }
            else
            {
                _texImg.Begin();
                _texImg.Clear(Color.Transparent);
            }


            GameGraphics.DrawGameTexture(_equipmentBG, 0, 0, _texImg);
            if (_currentItem != -1)
            {
                if (GameGraphics.ItemFileNames.Contains(Globals.GameItems[_currentItem].Pic))
                {
                    GameGraphics.DrawGameTexture(GameGraphics.ItemTextures[GameGraphics.ItemFileNames.IndexOf(Globals.GameItems[_currentItem].Pic)], 1, 1, _texImg);
                }
            }
            _texImg.End();
        }



        public void Update(int currentItem, int[] statBoost)
        {
            if (currentItem != _currentItem || !_texLoaded)
            {
                _currentItem = currentItem;
                _statBoost = statBoost;
                CreateTexImage();
                pnl.Texture = Gui.ToGwenTexture(_texImg);
                _texLoaded = true;
            }
        }
    }
}
