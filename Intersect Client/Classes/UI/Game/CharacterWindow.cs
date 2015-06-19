using Gwen;
using Gwen.Control;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private System.Drawing.Bitmap _spriteImg;
        private Texture _spriteTex;
        private string _currentSprite = "";

        private System.Drawing.Bitmap _equipmentBG;

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
            _characterWindow.SetPosition(Graphics.ScreenWidth - 210, Graphics.ScreenHeight - 500);
            _characterWindow.DisableResizing();
            _characterWindow.Margin = Margin.Zero;
            _characterWindow.Padding = Padding.Zero;
            _characterWindow.IsHidden = true;

            _characterName = new Label(_characterWindow);
            _characterName.SetPosition(4, 4);
            _characterName.SetText("Name");

            _characterLevel = new Label(_characterWindow);
            _characterLevel.SetPosition(4, 18);
            _characterLevel.SetText("Level: " + 1);

            _characterPortrait = new ImagePanel(_characterWindow);
            _characterPortrait.SetSize(100,100);
            _characterPortrait.SetPosition(200 / 2 - 100 / 2, 36);

            Label equipmentLabel = new Label(_characterWindow);
            equipmentLabel.SetPosition(4, 146);
            equipmentLabel.SetText("Equipment:");

            _equipmentContainer = new ScrollControl(_characterWindow);
            _equipmentContainer.SetPosition(10, 156);
            _equipmentContainer.SetSize(_characterWindow.Width - 20, 38);

            //Create equipment background image
            _equipmentBG = new System.Drawing.Bitmap(34, 34);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_equipmentBG);
            g.Clear(System.Drawing.Color.Transparent);
            g.DrawLine(System.Drawing.Pens.Black, new System.Drawing.Point(0, 0), new System.Drawing.Point(33, 0));
            g.DrawLine(System.Drawing.Pens.Black, new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 33));
            g.DrawLine(System.Drawing.Pens.Black, new System.Drawing.Point(33, 0), new System.Drawing.Point(33, 33));
            g.DrawLine(System.Drawing.Pens.Black, new System.Drawing.Point(0, 33), new System.Drawing.Point(33, 33));
            g.Dispose();

            InitEquipmentContainer();
        }

        private void InitEquipmentContainer()
        {
            int x = 0;
            int w = 38 * Enums.EquipmentSlots.Count;
            if (w > _characterWindow.Width - 20)
            {
                _equipmentContainer.EnableScroll(true, false);
                _equipmentContainer.SetSize(_characterWindow.Width - 20, 38 + 16);
            }
            else
            {
                x = (_characterWindow.Width - 20)/2 - (w - 4) / 2;
                _equipmentContainer.EnableScroll(false, false);
            }
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                Items.Add(new EquipmentItem(i,_equipmentBG, _characterWindow));
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
                        _characterPortrait.Texture = CreateTextureFromSprite();
                    }
                }
            }
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                Items[i].Update();
            }
        }
        private Texture CreateTextureFromSprite()
        {
            System.Drawing.Bitmap sprite = new System.Drawing.Bitmap("Resources/Entities/" + Globals.Me.MySprite + ".png");
            _spriteImg = new System.Drawing.Bitmap(sprite.Width / 4, sprite.Height / 4);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_spriteImg);
            g.DrawImage(sprite, new System.Drawing.Rectangle(0, 0, sprite.Width / 4, sprite.Height / 4), new System.Drawing.Rectangle(0, 0, sprite.Width / 4, sprite.Height / 4), System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();
            sprite.Dispose();
            return Gui.BitmapToGwenTexture(_spriteImg);
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
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;
        private int myindex;
        private long ClickTime = 0;
        private int _currentItem = -1;
        private bool _texLoaded = false;
        private System.Drawing.Bitmap _equipmentBG;
        private WindowControl _characterWindow;

        public EquipmentItem(int index, System.Drawing.Bitmap equipmentBG, WindowControl characterWindow)
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
            pnl.Clicked += pnl_Clicked;
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Environment.TickCount + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryDropItem(myindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            _descWindow.Dispose();
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            MouseOver = true;
            _descWindow = new ItemDescWindow(_currentItem, 1, _characterWindow.X - 220, _characterWindow.Y, Globals.Me.Inventory[0].StatBoost, "Equipment Slot: " + Enums.EquipmentSlots[myindex]);
        }

        public System.Drawing.Rectangle RenderBounds()
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
            rect.X = pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X;
            rect.Y = pnl.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y;
            rect.Width = pnl.Width;
            rect.Height = pnl.Height;
            return rect;
        }



        public void Update()
        {
            //check if current item != the current equipment item
            if (!_texLoaded)
            {
                pnl.Texture = Gui.BitmapToGwenTexture(_equipmentBG);
                _texLoaded = true;
            }
        }
    }
}
