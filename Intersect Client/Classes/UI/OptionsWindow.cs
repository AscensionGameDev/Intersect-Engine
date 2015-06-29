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
using Gwen;
using Gwen.Control;
using Intersect_Client.Classes.UI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI
{
    public class OptionsWindow : IGUIElement
    {
        //Controls
        private Base _optionsMenu;
        private Label _resolutionLabel;
        private ComboBox _resolutionList;
        private LabeledCheckBox _fullscreen;
        private Label _soundLabel;
        private LabeledCheckBox _enableSound;
        private LabeledCheckBox _disableSound;
        private Label _musicLabel;
        private LabeledCheckBox _enableMusic;
        private LabeledCheckBox _disableMusic;
        private Button _applyBtn;
        private Button _backBtn;

        private bool _gameWindow = false;
        private MainMenu _mainMenu = null;

        //Init
        public OptionsWindow(Base _parent, Boolean InGame, MainMenu mainMenu = null)
        {
            _gameWindow = InGame;
            _mainMenu = mainMenu;

            //Options Window
            if (_gameWindow)
            {
                _optionsMenu = new WindowControl(_parent, "Options");
                _optionsMenu.SetSize(200, 200);
                _optionsMenu.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
                ((WindowControl)_optionsMenu).DisableResizing();
                _optionsMenu.Margin = Margin.Zero;
                _optionsMenu.Padding = Padding.Zero;
                _optionsMenu.IsHidden = true;
            }
            else
            {
                _optionsMenu = _parent;
            }

            //Options - Resolution Label
            _resolutionLabel = new Label(_optionsMenu);
            _resolutionLabel.SetText("Resolution:");
            _resolutionLabel.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 12);

            _resolutionList = new ComboBox(_optionsMenu);
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                _resolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
            }
            _resolutionList.SetSize(120, 14);
            _resolutionList.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 28);
            _resolutionList.IsHidden = true;

            //Options - Fullscreen Checkbox
            _fullscreen = new LabeledCheckBox(_optionsMenu) { Text = "Fullscreen" };
            _fullscreen.SetSize(120, 14);
            _fullscreen.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 46);

            //Options - Sound Label
            _soundLabel = new Label(_optionsMenu);
            _soundLabel.SetText("Sound:");
            _soundLabel.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 64);

            //Options - Sound On Checkbox
            _enableSound = new LabeledCheckBox(_optionsMenu) { Text = "On" };
            _enableSound.SetSize(56, 14);
            _enableSound.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 82);
            _enableSound.Checked += EnableSound_Checked;

            //Options - Sound Off Checkbox
            _disableSound = new LabeledCheckBox(_optionsMenu) { Text = "Off" };
            _disableSound.SetSize(56, 14);
            _disableSound.SetPosition(_optionsMenu.Width / 2 + 4, 82);
            _disableSound.Checked += DisableSound_Checked;

            //Options - Music Label
            _musicLabel = new Label(_optionsMenu);
            _musicLabel.SetText("Music:");
            _musicLabel.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 100);

            //Options - Music On Checkbox
            _enableMusic = new LabeledCheckBox(_optionsMenu) { Text = "On" };
            _enableMusic.SetSize(56, 14);
            _enableMusic.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 118);
            _enableMusic.Checked += EnableMusic_Checked;

            //Options - Music Off Checkbox
            _disableMusic = new LabeledCheckBox(_optionsMenu) { Text = "Off" };
            _disableMusic.SetSize(56, 14);
            _disableMusic.SetPosition(_optionsMenu.Width / 2 + 4, 118);
            _disableMusic.Checked += DisableMusic_Checked;

            //Options - Apply Button
            _applyBtn = new Button(_optionsMenu);
            _applyBtn.SetText("Apply");
            _applyBtn.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 136);
            _applyBtn.SetSize(120, 32);
            _applyBtn.Clicked += ApplyBtn_Clicked;

            if (!InGame)
            {
                _applyBtn.SetPosition(_optionsMenu.Width / 2 - 120 / 2, 136);
                _applyBtn.SetSize(56, 32);

                //Options - Back Button
                _backBtn = new Button(_optionsMenu);
                _backBtn.SetText("Back");
                _backBtn.SetPosition(_optionsMenu.Width / 2 + 4, 136);
                _backBtn.SetSize(56, 32);
                _backBtn.IsHidden = true;
                _backBtn.Clicked += BackBtn_Clicked;

            }
        }

        //Methods
        public void Update()
        {

        }
        public void Show()
        {
            _resolutionList.SelectByText(Graphics.GetValidVideoModes()[Graphics.DisplayMode].Width + "x" + Graphics.GetValidVideoModes()[Graphics.DisplayMode].Height);
            _fullscreen.IsChecked = Graphics.FullScreen;
            _enableSound.IsChecked = Globals.SoundEnabled;
            _disableSound.IsChecked = !Globals.SoundEnabled;
            _enableMusic.IsChecked = Globals.MusicEnabled;
            _disableMusic.IsChecked = !Globals.MusicEnabled;
            if (_gameWindow) { _optionsMenu.IsHidden = false; }
            _resolutionLabel.IsHidden = false;
            _resolutionList.IsHidden = false;
            _fullscreen.IsHidden = false;
            _soundLabel.IsHidden = false;
            _enableSound.IsHidden = false;
            _disableSound.IsHidden = false;
            _musicLabel.IsHidden = false;
            _enableMusic.IsHidden = false;
            _disableMusic.IsHidden = false;
            _applyBtn.IsHidden = false;
            if (!_gameWindow) { _backBtn.IsHidden = false; }
        }

        public bool IsVisible()
        {
            return !_resolutionList.IsHidden;
        }

        public void Hide()
        {
            if (_gameWindow) { _optionsMenu.IsHidden = true; }
            _resolutionLabel.IsHidden = true;
            _resolutionList.IsHidden = true;
            _fullscreen.IsHidden = true;
            _soundLabel.IsHidden = true;
            _enableSound.IsHidden = true;
            _disableSound.IsHidden = true;
            _musicLabel.IsHidden = true;
            _enableMusic.IsHidden = true;
            _disableMusic.IsHidden = true;
            _applyBtn.IsHidden = true;
            if (!_gameWindow) { _backBtn.IsHidden = true; }
        }

        //Input Handlers
        void DisableMusic_Checked(Base sender, EventArgs arguments)
        {
            _enableMusic.IsChecked = false;
        }
        void EnableMusic_Checked(Base sender, EventArgs arguments)
        {
            _disableMusic.IsChecked = false;
        }
        void DisableSound_Checked(Base sender, EventArgs arguments)
        {
            _enableSound.IsChecked = false;
        }
        void EnableSound_Checked(Base sender, EventArgs arguments)
        {
            _disableSound.IsChecked = false;
        }
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _mainMenu.Reset();
        }
        void ApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var mi = _resolutionList.SelectedItem;
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                if (mi.Text != myModes[i].Width + "x" + myModes[i].Height) continue;
                Graphics.DisplayMode = i;
                Graphics.MustReInit = true;
            }
            if (Graphics.FullScreen != _fullscreen.IsChecked)
            {
                Graphics.FullScreen = _fullscreen.IsChecked;
                Graphics.MustReInit = true;
            }
            Globals.MusicEnabled = _enableMusic.IsChecked;
            Globals.SoundEnabled = _enableSound.IsChecked;
            Database.SaveOptions();
        }
    }
}
