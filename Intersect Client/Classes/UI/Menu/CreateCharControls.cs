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
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class CreateCharControls : IGUIElement
    {
        //Controls
        private Label _charnameLabel;
        private TextBox _charnameTextbox;
        private Label _classLabel;
        private ComboBox _classCombobox;
        private Label _spriteLabel;
        private ComboBox _spriteCombobox;
        private Label _genderLabel;
        private LabeledCheckBox _maleChk;
        private LabeledCheckBox _femaleChk;
        private Button _createButton;

        //Image
        private string _characterPortraitImg = "";
        private ImagePanel _characterPortrait;
        private GameRenderTexture _spriteTex;
        private string _currentSprite = "";

        //Parent
        private WindowControl _Parent;

        //Init
        public CreateCharControls(WindowControl _parent)
        {
            int i = 0;

            _Parent = _parent;

            //Character name Label
            _charnameLabel = new Label(_parent);
            _charnameLabel.SetText("Character Name:");
            _charnameLabel.SetPosition(_parent.Width / 2 - 120 / 2, 12);
            _charnameLabel.IsHidden = true;

            //Character name Textbox
            _charnameTextbox = new TextBox(_parent);
            _charnameTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 24);
            _charnameTextbox.SetSize(90, 14);
            _charnameTextbox.IsHidden = true;
            _charnameTextbox.SubmitPressed += CharnameTextbox_SubmitPressed;

            //Class Label
            _classLabel = new Label(_parent);
            _classLabel.SetText("Class:");
            _classLabel.SetPosition(_parent.Width / 2 - 120 / 2, 42);
            _classLabel.IsHidden = true;

            //Class Combobox
            _classCombobox = new ComboBox(_parent);
            
            while (Globals.GameClasses[i].Name != "")
            {
                _classCombobox.AddItem(Globals.GameClasses[i].Name);
                i = i + 1;
                if (i >= Options.MaxClasses)
                {
                    break;
                }
            }

            _classCombobox.SetPosition(_parent.Width / 2 - 120 / 2, 54);
            _classCombobox.SetSize(90, 14);
            _classCombobox.IsHidden = true;
            _classCombobox.ItemSelected += classCombobox_ItemSelected;

            //Sprite Label
            _spriteLabel = new Label(_parent);
            _spriteLabel.SetText("Sprite:");
            _spriteLabel.SetPosition(_parent.Width / 2 - 120 / 2, 72);
            _spriteLabel.IsHidden = true;

            //Sprite Combobox
            UpdateSpriteCombobox(0);
            _spriteCombobox.IsHidden = true;

            //Character sprite
            _characterPortrait = new ImagePanel(_parent);
            _characterPortrait.SetSize(48, 48);
            _characterPortrait.SetPosition(_parent.Width / 2 + (_charnameTextbox.Width / 2), 12);

            //Gender Label
            _genderLabel = new Label(_parent);
            _genderLabel.SetText("Gender:");
            _genderLabel.SetPosition(_parent.Width / 2 - 120 / 2, 102);
            _genderLabel.IsHidden = true;

            //Male Checkbox
            _maleChk = new LabeledCheckBox(_parent) { Text = "Male" };
            _maleChk.SetSize(56, 14);
            _maleChk.SetPosition(_parent.Width / 2 - 120 / 2, 114);
            _maleChk.IsChecked = true;
            _maleChk.Checked += maleChk_Checked;
            _maleChk.UnChecked += femaleChk_Checked; // If you notice this, feel free to hate us ;)

            //Female Checkbox
            _femaleChk = new LabeledCheckBox(_parent) { Text = "Female" };
            _femaleChk.SetSize(56, 14);
            _femaleChk.SetPosition(_parent.Width / 2 + 4, 114);
            _femaleChk.IsChecked = false;
            _femaleChk.Checked += femaleChk_Checked;
            _femaleChk.UnChecked += maleChk_Checked;

            //Register - Send Registration Button
            _createButton = new Button(_parent);
            _createButton.SetText("Create");
            _createButton.SetPosition(_parent.Width / 2 - 90 / 2, 132);
            _createButton.SetSize(56, 32);
            _createButton.IsHidden = true;
            _createButton.Clicked += CreateButton_Clicked;

            Update();
        }

        //Methods
        public void Update()
        {
            int i = 0;

            // Find the gender.
            if (_femaleChk.IsChecked == true)
            {
                i = 1;
            }

            if (System.IO.File.Exists("Resources/Faces/" + GetSprite(i)))
            {
                _characterPortrait.ImageName = "Resources/Faces/" + GetSprite(i);
            }
            if (Globals.GameClasses[GetClass()].Sprites.Count > 0)
            {
                string test = Globals.GameClasses[GetClass()].Sprites[GetSprite(i)].Sprite;
                _spriteTex = Gui.CreateTextureFromSprite(Globals.GameClasses[GetClass()].Sprites[GetSprite(i)].Sprite, _characterPortrait.Width, _characterPortrait.Height);
                _characterPortrait.Texture = Gui.ToGwenTexture(_spriteTex);
                _currentSprite = Globals.GameClasses[GetClass()].Sprites[GetSprite(i)].Sprite;
            }
        }

        public void Show()
        {
            _charnameLabel.IsHidden = false;
            _charnameTextbox.IsHidden = false;
            _spriteLabel.IsHidden = false;
            _spriteCombobox.IsHidden = false;
            _classLabel.IsHidden = false;
            _classCombobox.IsHidden = false;
            _genderLabel.IsHidden = false;
            _maleChk.IsHidden = false;
            _femaleChk.IsHidden = false;
            _createButton.IsHidden = false;
        }

        public void Hide()
        {
            _charnameLabel.IsHidden = true;
            _charnameTextbox.IsHidden = true;
            _spriteLabel.IsHidden = true;
            _spriteCombobox.IsHidden = true;
            _classLabel.IsHidden = true;
            _classCombobox.IsHidden = true;
            _genderLabel.IsHidden = true;
            _maleChk.IsHidden = true;
            _femaleChk.IsHidden = true;
            _createButton.IsHidden = true;
        }

        private int GetClass()
        {
            for (int i = 0; i < Options.MaxClasses; i++)
            {
                if (Globals.GameClasses[i].Name == _classCombobox.SelectedItem.Text)
                {
                    return i;
                }
            }
            return 0;
        }

        private int GetSprite(int Gender)
        {
            int i = 0;

            for (int n = 0; n < Globals.GameClasses[GetClass()].Sprites.Count; n++)
            {
                if (Globals.GameClasses[GetClass()].Sprites[n].Gender == Gender)
                {
                    i = i + 1;
                    if (Convert.ToString(i) == _spriteCombobox.SelectedItem.Text)
                    {
                        return n;
                    }
                }
            }
            return 0;
        }

        private void UpdateSpriteCombobox(int Gender)
        {
            int i = 0;
            _spriteCombobox = new ComboBox(_Parent);

            for (int n = 0; n < Globals.GameClasses[GetClass()].Sprites.Count; n++)
            {
                if (Globals.GameClasses[GetClass()].Sprites[n].Gender == Gender)
                {
                    i = i + 1;
                    _spriteCombobox.AddItem(Convert.ToString(i));
                }
            }

            _spriteCombobox.SetPosition(_Parent.Width / 2 - 120 / 2, 84);
            _spriteCombobox.SetSize(90, 14);
            _spriteCombobox.ItemSelected += spriteCombobox_ItemSelected;
        }

        void TryCreateCharacter(int Gender)
        {
            if (Globals.WaitingOnServer) { return; }
            if (FieldChecking.IsValidName(_charnameTextbox.Text))
            {
                GameFade.FadeOut();
                PacketSender.SendCreateCharacter(_charnameTextbox.Text, GetClass(), GetSprite(Gender));
                Globals.WaitingOnServer = true;
            }
            else
            {
                Gui.MsgboxErrors.Add("Character name is invalid. Please user alphanumeric characters with a length between 2 and 20");
            }
        }

        //Input Handlers
        void CharnameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            if (_maleChk.IsChecked == true)
            {
                TryCreateCharacter(0);
            }
            else
            {
                TryCreateCharacter(1);
            }
        }
        void classCombobox_ItemSelected(Base control, ItemSelectedEventArgs args)
        {
            _spriteCombobox.Dispose();
            if (_maleChk.IsChecked == true)
            {
                UpdateSpriteCombobox(0);
            }
            else
            {
                UpdateSpriteCombobox(1);
            }
            Update();
        }
        void spriteCombobox_ItemSelected(Base control, ItemSelectedEventArgs args)
        {
            Update();
        }
        void maleChk_Checked(Base sender, EventArgs arguments)
        {
            _maleChk.IsChecked = true;
            _spriteCombobox.Dispose();
            UpdateSpriteCombobox(0);
            _femaleChk.IsChecked = false;
            Update();
        }
        void femaleChk_Checked(Base sender, EventArgs arguments)
        {
            _femaleChk.IsChecked = true;
            _spriteCombobox.Dispose();
            UpdateSpriteCombobox(1);
            _maleChk.IsChecked = false;
            Update();
        }
        void CreateButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_maleChk.IsChecked == true)
            {
                TryCreateCharacter(0);
            }
            else
            {
                TryCreateCharacter(1);
            }
        }
    }
}
