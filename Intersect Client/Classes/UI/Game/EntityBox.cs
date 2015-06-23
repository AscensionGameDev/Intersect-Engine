using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class EntityBox
    {
        //Controls
        public WindowControl _entityBox;
        public ImagePanel _entityFace;
        public Label _hpLbl;
        public Label _mpLbl;
        public Label _expLbl;
        public ImagePanel _hpBackground;
        public ImagePanel _hpBar;
        public ImagePanel _mpBackground;
        public ImagePanel _mpBar;
        public ImagePanel _expBackground;
        public ImagePanel _expBar;
        public RichLabel _eventDesc;

        //Bar Images
        System.Drawing.Bitmap _hpBarImg = new System.Drawing.Bitmap("Resources/GUI/HPBar.png");
        System.Drawing.Bitmap _mpBarImg = new System.Drawing.Bitmap("Resources/GUI/ManaBar.png");
        System.Drawing.Bitmap _expBarImg = new System.Drawing.Bitmap("Resources/GUI/EXPBar.png");

        public float _curHPWidth;
        public float _curMPWidth;
        public float _curEXPWidth;

        private long lastUpdateTime;

        private Entity _myEntity;
        private string _currentSprite = "";

        //Init
        public EntityBox(Canvas _gameCanvas, Entity myEntity, int x, int y)
        {
            _myEntity = myEntity;
            
            _entityBox = new WindowControl(_gameCanvas) { Title = "EntityBox"};
            _entityBox.SetSize(244, 94);
            _entityBox.SetPosition(x,y);
            _entityBox.DisableResizing();

            _entityBox.Margin = Margin.Zero;
            _entityBox.Padding = Padding.Zero;
            _entityBox.IsClosable = false;

            _entityFace = new ImagePanel(_entityBox);
            _entityFace.SetSize(64, 64);
            _entityFace.SetPosition(3, 2);

            if (myEntity.GetType() == typeof(Event))
            {
                _eventDesc = new RichLabel(_entityBox);
                _eventDesc.SetPosition(70, 2);
                _eventDesc.Width = 159;
                _eventDesc.AddText("Desc: " + ((Event)_myEntity).Desc,Color.Black);
                _eventDesc.SizeToChildren(false, true);
            }


            if (myEntity.GetType() != typeof(Event))
            {
                _hpBackground = new ImagePanel(_entityBox);
                _hpBackground.ImageName = "Resources/GUI/EmptyBar.png";
                _hpBackground.SetSize(169, 18);
                _hpBackground.SetPosition(70, 2);

                _hpBar = new ImagePanel(_entityBox);
                _hpBar.SetSize(169, 18);
                _hpBar.SetPosition(70, 2);
                _hpBar.IsHidden = true;

                _hpLbl = new Label(_entityBox);
                _hpLbl.SetText("HP: ");
                _hpLbl.SetPosition(120, 5);

                _mpBackground = new ImagePanel(_entityBox);
                _mpBackground.ImageName = "Resources/GUI/EmptyBar.png";
                _mpBackground.SetSize(169, 18);
                _mpBackground.SetPosition(70, 24);

                _mpBar = new ImagePanel(_entityBox);
                _mpBar.SetSize(169, 18);
                _mpBar.SetPosition(70, 24);
                _mpBar.IsHidden = true;

                _mpLbl = new Label(_entityBox);
                _mpLbl.SetText("MP: ");
                _mpLbl.SetPosition(120, 25);
            }

            if (_myEntity.GetType() == typeof(Player))
            {
                _expBackground = new ImagePanel(_entityBox);
                _expBackground.ImageName = "Resources/GUI/EmptyBar.png";
                _expBackground.SetSize(169, 18);
                _expBackground.SetPosition(70, 46);

                _expBar = new ImagePanel(_entityBox);
                _expBar.SetSize(169, 18);
                _expBar.SetPosition(70, 46);
                _expBar.IsHidden = true;

                _expLbl = new Label(_entityBox);
                _expLbl.SetText("Exp: ");
                _expLbl.SetPosition(120, 49);
            }

            lastUpdateTime = Environment.TickCount;
        }

        //Update
        public void Update()
        {
            float elapsedTime = ((float)(Environment.TickCount - lastUpdateTime))/1000.0f;

            //Update the window title
            _entityBox.Title = _myEntity.MyName + ((_myEntity.Level == 0 || _myEntity.GetType() == typeof(Event)) ? "" : " Lv: " + _myEntity.Level) ;

            //Update the event/entity face.
            if (_myEntity.Face != "")
            {
                if (_entityFace.ImageName != "Resources/Faces/" + _myEntity.Face)
                {
                    _entityFace.ImageName = "Resources/Faces/" + _myEntity.Face;
                }
            }
            else
            {
                if (_myEntity.MySprite != "")
                {
                    if (_currentSprite != _myEntity.MySprite)
                    {
                        _entityFace.Texture = Gui.CreateTextureFromSprite(_myEntity.MySprite, _entityFace.Width, _entityFace.Height);
                        _currentSprite = _myEntity.MySprite;
                    }
                }
                else
                {
                    _entityFace.IsHidden = true;
                }
            }

            //If not an event, update the hp/mana bars.
            if (_myEntity.GetType() != typeof(Event) && _myEntity.MaxVital[(int)Enums.Vitals.Health] > 0 && _myEntity.MaxVital[(int)Enums.Vitals.Mana] > 0)
            {
                float targetHPWidth = _myEntity.Vital[(int)Enums.Vitals.Health] / _myEntity.MaxVital[(int)Enums.Vitals.Health];
                float targetMPWidth = _myEntity.Vital[(int)Enums.Vitals.Mana] / _myEntity.MaxVital[(int)Enums.Vitals.Mana];

                //Fix the Labels
                _hpLbl.Text = "HP: " + _myEntity.Vital[(int)Enums.Vitals.Health] + " / " + _myEntity.MaxVital[(int)Enums.Vitals.Health];
                _hpLbl.X = _hpBackground.X + _hpBackground.Width / 2 - _hpLbl.Width / 2;
                _mpLbl.Text = "MP: " + _myEntity.Vital[(int)Enums.Vitals.Mana] + " / " + _myEntity.MaxVital[(int)Enums.Vitals.Mana];
                _mpLbl.X = _mpBackground.X + _mpBackground.Width / 2 - _mpLbl.Width / 2;

                //Multiply by the width of the bars.
                targetHPWidth *= 169;
                targetMPWidth *= 169;

                if ((int)targetHPWidth != _curHPWidth)
                {
                    if ((int)targetHPWidth > _curHPWidth)
                    {
                        _curHPWidth += (100f * elapsedTime);
                        if (_curHPWidth > (int)targetHPWidth) { _curHPWidth = targetHPWidth;}
                    }
                    else
                    {
                        _curHPWidth -= (100f * elapsedTime);
                        if (_curHPWidth < targetHPWidth) { _curHPWidth = targetHPWidth; }
                    }
                    if (_curHPWidth == 0)
                    {
                        _hpBar.IsHidden = true;
                    }
                    else
                    {
                        _hpBar.Width = (int)_curHPWidth;
                        _hpBar.Texture = UpdateBarTexture(_curHPWidth, _hpBarImg);
                        _hpBar.IsHidden = false;
                    }
                }

                if ((int)targetMPWidth != _curMPWidth)
                {
                    if ((int)targetMPWidth > _curMPWidth)
                    {
                        _curMPWidth += (100f * elapsedTime);
                        if (_curMPWidth > (int)targetMPWidth) { _curMPWidth = targetMPWidth;}
                    }
                    else
                    {
                        _curMPWidth -= (100f * elapsedTime);
                        if (_curMPWidth < targetMPWidth) { _curMPWidth = targetMPWidth; }
                    }
                    if (_curMPWidth == 0)
                    {
                        _mpBar.IsHidden = true;
                    }
                    else
                    {
                        _mpBar.Width = (int)_curMPWidth;
                        _mpBar.Texture = UpdateBarTexture(_curMPWidth, _mpBarImg);
                        _mpBar.IsHidden = false;
                    }
                }
            }

            //If player draw exp bar
            if (_myEntity.GetType() == typeof(Player))
            {
                float targetExpWidth = 1;
                if (((Player)_myEntity).GetNextLevelExperience() != 0)
                {
                    targetExpWidth = ((Player)_myEntity).Experience / ((Player)_myEntity).GetNextLevelExperience();
                    _expLbl.Text = "Exp: " + ((Player)_myEntity).Experience + " / " + ((Player)_myEntity).GetNextLevelExperience();
                }
                else
                {
                    _expLbl.Text = "Max Level";
                }
                _expLbl.X = _expBackground.X + _expBackground.Width / 2 - _expLbl.Width / 2;
                targetExpWidth *= 169;
                if ((int)targetExpWidth != _curEXPWidth)
                {
                    if ((int)targetExpWidth > _curEXPWidth)
                    {
                        _curEXPWidth += (.1f * elapsedTime);
                        if (_curEXPWidth > (int)targetExpWidth) { _curEXPWidth = targetExpWidth; }
                    }
                    else
                    {
                        _curEXPWidth -= (.1f * elapsedTime);
                        if (_curEXPWidth < targetExpWidth) { _curEXPWidth = targetExpWidth; }
                    }
                    if (_curEXPWidth == 0)
                    {
                        _expBar.IsHidden = true;
                    }
                    else
                    {
                        _expBar.Width = (int)_curEXPWidth;
                        _expBar.Texture = UpdateBarTexture(_curEXPWidth, _expBarImg);
                        _expBar.IsHidden = false;
                    }
                }
            }

            //Eventually draw icons for buffs and debuffs?
            lastUpdateTime = Environment.TickCount;
        }

        private Gwen.Texture UpdateBarTexture(float width, System.Drawing.Bitmap barImg)
        {
            System.Drawing.Bitmap bar = new System.Drawing.Bitmap((int)Math.Ceiling(width), 18);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bar);
            g.DrawImage(barImg, new Rectangle(0, 0, (int)Math.Ceiling(width), 18), new Rectangle(0, 0, (int)Math.Ceiling(width), 18), GraphicsUnit.Pixel);
            g.Dispose();
            return Gui.BitmapToGwenTexture(bar);
        }

        public void Dispose()
        {
            _entityBox.Close();
            Gui._GameGui.GameCanvas.RemoveChild(_entityBox, false);
            _entityBox.Dispose();
        }

        //Input Handlers
    }
}
