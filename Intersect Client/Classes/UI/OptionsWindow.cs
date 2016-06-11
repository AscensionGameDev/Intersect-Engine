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
using System.Linq;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Menu;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;

namespace Intersect_Client.Classes.UI
{
    public class OptionsWindow
    {
        //Controls
        private ImagePanel _menuPanel;
        private Label _menuHeader;

        private ImagePanel _resolutionBackground;
        private Label _resolutionLabel;
        private ComboBox _resolutionList;

        private ImagePanel _fpsBackground;
        private Label _fpsLabel;
        private ComboBox _fpsList;

        private LabeledCheckBox _fullscreen;
        private Label _soundLabel;
        private HorizontalSlider _soundSlider;
        private Label _musicLabel;
        private HorizontalSlider _musicSlider;
        private Button _applyBtn;
        private Button _backBtn;

        private int _previousSoundVolume;
        private int _previousMusicVolume;

        private bool _gameWindow = false;
        private MainMenu _mainMenu = null;

        //Init
        public OptionsWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _menuPanel = new ImagePanel(parent);
            _menuPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png");
            _menuPanel.SetSize(512, 393);
            if (_mainMenu != null && parentPanel != null)
            {
                _menuPanel.SetPosition(parentPanel.X, parentPanel.Y);
            }
            else
            {
                _menuPanel.SetPosition(parent.Width / 2 - _menuPanel.Width / 2, parent.Height / 2 - _menuPanel.Height / 2);
            }
            _menuPanel.IsHidden = true;

            //Menu Header
            _menuHeader = new Label(_menuPanel);
            _menuHeader.AutoSizeToContents = false;
            _menuHeader.SetText("Options");
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 24);
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

            //Resolution Background
            _resolutionBackground = new ImagePanel(_menuPanel);
            _resolutionBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _resolutionBackground.SetSize(_resolutionBackground.Texture.GetWidth(), _resolutionBackground.Texture.GetHeight());
            _resolutionBackground.SetPosition(_menuPanel.Width / 2 - _resolutionBackground.Width / 2, 44);

            //Options - Resolution Label
            _resolutionLabel = new Label(_resolutionBackground);
            _resolutionLabel.SetText("Resolution:");
            _resolutionLabel.AutoSizeToContents = false;
            _resolutionLabel.SetSize(178, 60);
            _resolutionLabel.Alignment = Pos.Center;
            _resolutionLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _resolutionLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            _resolutionList = new ComboBox(_resolutionBackground);
            _resolutionList.SetPosition(190, 8);
            _resolutionList.SetSize(260, 38);
            _resolutionList.Alignment = Pos.Center;
            _resolutionList.ShouldDrawBackground = false;
            _resolutionList.SetMenuBackgroundColor(new Color(220,0,0,0));
            _resolutionList.SetMenuMaxSize(260, 200);
            _resolutionList.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _resolutionList.SetTextColor(new Color(255, 220, 220, 220), Label.ControlState.Hovered);
            _resolutionList.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
               var item =  _resolutionList.AddItem(myModes[i]);
                item.Alignment = Pos.Center;
                /*if (Globals.GameBorderStyle == 1)
                {
                    if (myModes[i].Width <= Options.TileWidth*Options.MapWidth &&
                        myModes[i].Height <= Options.TileHeight*Options.MapHeight)
                    {
                        _resolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
                    }
                }
                else
                {
                    int maxx = (Options.MapWidth - 1) * Options.TileWidth * 2;
                    int maxy = (Options.MapHeight - 1) * Options.TileHeight * 2;
                    if (myModes[i].Width <= maxx &&
                        myModes[i].Height <= maxy)
                    {
                        _resolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
                    }
                }*/
            }

            //FPS Background
            _fpsBackground = new ImagePanel(_menuPanel);
            _fpsBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _fpsBackground.SetSize(_fpsBackground.Texture.GetWidth(), _fpsBackground.Texture.GetHeight());
            _fpsBackground.SetPosition(_menuPanel.Width / 2 - _fpsBackground.Width / 2, _resolutionBackground.Bottom + 16);

            //Options - FPS Label
            _fpsLabel = new Label(_fpsBackground);
            _fpsLabel.SetText("Target FPS:");
            _fpsLabel.AutoSizeToContents = false;
            _fpsLabel.SetSize(176, 55);
            _fpsLabel.Alignment = Pos.Center;
            _fpsLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _fpsLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Options - FPS List
            _fpsList = new ComboBox(_fpsBackground);
            _fpsList.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _fpsList.SetTextColor(new Color(255, 220, 220, 220), Label.ControlState.Hovered);
            _fpsList.AddItem("V-Sync");
            _fpsList.AddItem("30");
            _fpsList.AddItem("60");
            _fpsList.AddItem("90");
            _fpsList.AddItem("120");
            _fpsList.AddItem("No Limit");
            _fpsList.SetPosition(190, 8);
            _fpsList.SetSize(260, 38);
            _fpsList.Alignment = Pos.Center;
            _fpsList.ShouldDrawBackground = false;
            _fpsList.SetMenuBackgroundColor(new Color(220, 0, 0, 0));
            _fpsList.SetMenuMaxSize(260, 200);
            _fpsList.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Options - Fullscreen Checkbox
            _fullscreen = new LabeledCheckBox(_menuPanel) { Text = "Fullscreen" };
            _fullscreen.SetSize(300, 36);
            _fullscreen.SetPosition(_fpsBackground.X + 24, _fpsBackground.Bottom + 16);
            _fullscreen.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxempty.png"), CheckBox.ControlState.Normal);
            _fullscreen.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxfull.png"), CheckBox.ControlState.CheckedNormal);
            _fullscreen.SetCheckSize(32, 32);
            _fullscreen.SetLabelDistance(12);
            _fullscreen.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _fullscreen.SetTextColor(new Color(255, 140, 140, 140), Label.ControlState.Hovered);
            _fullscreen.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 20));

            //Options - Sound Label
            _soundLabel = new Label(_menuPanel);
            _soundLabel.SetText("Sound Volume: 100%");
            _soundLabel.SetSize(210, 32);
            _soundLabel.AutoSizeToContents = false;
            _soundLabel.SetPosition(_resolutionBackground.X,_fullscreen.Bottom + 16);
            _soundLabel.Alignment = Pos.Center;
            _soundLabel.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _soundLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 16);

            //Options - Sound Slider
            _soundSlider = new HorizontalSlider(_menuPanel);
            _soundSlider.SetSize(210, 25);
            _soundSlider.SetPosition(_soundLabel.X,_soundLabel.Bottom + 8);
            _soundSlider.Min = 0;
            _soundSlider.Max = 100;
            _soundSlider.ValueChanged += _soundSlider_ValueChanged;
            _soundSlider.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sliderbar.png"));
            _soundSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggerv.png"),Dragger.ControlState.Normal);
            _soundSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervclick.png"), Dragger.ControlState.Clicked);
            _soundSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervhover.png"), Dragger.ControlState.Hovered);
            
            _soundSlider.SetDraggerSize(14, 26);

            //Options - Music Label
            _musicLabel = new Label(_menuPanel);
            _musicLabel.SetText("Music Volume: 100%");
            _musicLabel.SetSize(210, 32);
            _musicLabel.AutoSizeToContents = false;
            _musicLabel.SetPosition(_resolutionBackground.Right - _musicLabel.Width, _fullscreen.Bottom + 16);
            _musicLabel.Alignment = Pos.Center;
            _musicLabel.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _musicLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 16);

            //Options - Music Slider
            _musicSlider = new HorizontalSlider(_menuPanel);
            _musicSlider.SetSize(210, 24);
            _musicSlider.SetPosition(_musicLabel.X, _musicLabel.Bottom + 8);
            _musicSlider.Min = 0;
            _musicSlider.Max = 100;
            _musicSlider.ValueChanged += _musicSlider_ValueChanged;
            _musicSlider.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sliderbar.png"));
            _musicSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggerv.png"), Dragger.ControlState.Normal);
            _musicSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervclick.png"), Dragger.ControlState.Clicked);
            _musicSlider.SetDraggerImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervhover.png"), Dragger.ControlState.Hovered);
            _musicSlider.SetDraggerSize(14, 26);

            //Options - Apply Button
            _applyBtn = new Button(_menuPanel);
            _applyBtn.SetText("Apply");
            _applyBtn.SetPosition(_resolutionBackground.X, _soundSlider.Bottom + 16);
            _applyBtn.SetSize(56, 32);
            _applyBtn.Clicked += ApplyBtn_Clicked;
            _applyBtn.SetSize(211, 61);
            _applyBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"), Button.ControlState.Normal);
            _applyBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"), Button.ControlState.Hovered);
            _applyBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"), Button.ControlState.Clicked);
            _applyBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _applyBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _applyBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _applyBtn.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);


            //Options - Back Button
            _backBtn = new Button(_menuPanel);
            _backBtn.SetText("Cancel");
            _backBtn.SetSize(211, 61);
            _backBtn.SetPosition(_resolutionBackground.Right - _backBtn.Width, _soundSlider.Bottom + 16);
            _backBtn.Clicked += BackBtn_Clicked;
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"), Button.ControlState.Normal);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"), Button.ControlState.Hovered);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"), Button.ControlState.Clicked);
            _backBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _backBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _backBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _backBtn.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);
        }


        //Methods
        public void Update()
        {

        }
        public void Show()
        {
            if (_mainMenu == null)
            {
                _menuPanel.MakeModal(true);
            }
            _previousMusicVolume = Globals.Database.MusicVolume;
            _previousSoundVolume = Globals.Database.SoundVolume;
            if (GameGraphics.Renderer.GetValidVideoModes().Count > 0)
            {
                _resolutionList.SelectByText(GameGraphics.Renderer.GetValidVideoModes()[Globals.Database.TargetResolution]);
            }
            switch (Globals.Database.TargetFps)
            {
                case -1: //Unlimited
                    _fpsList.SelectByText("No Limit");
                    break;
                case 0: //VSYNC
                    _fpsList.SelectByText("V-Sync");
                    break;
                case 1:
                    _fpsList.SelectByText("30");
                    break;
                case 2:
                    _fpsList.SelectByText("60");
                    break;
                case 3:
                    _fpsList.SelectByText("90");
                    break;
                case 4:
                    _fpsList.SelectByText("120");
                    break;
                default:
                    _fpsList.SelectByText("V-Sync");
                    break;
            }
            _fullscreen.IsChecked = Globals.Database.FullScreen;
            _musicSlider.Value = Globals.Database.MusicVolume;
            _soundSlider.Value = Globals.Database.SoundVolume;
            _musicLabel.Text = "Music Volume: " + (int)_musicSlider.Value + "%";
            _soundLabel.Text = "Sound Volume: " + (int)_soundSlider.Value + "%";
            _menuPanel.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_menuPanel.IsHidden;
        }

        public void Hide()
        {
            if (_mainMenu == null)
            {
                _menuPanel.RemoveModal();
            }
            _menuPanel.IsHidden = true;
        }

        //Input Handlers
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Database.MusicVolume = _previousMusicVolume;
            Globals.Database.SoundVolume = _previousSoundVolume;
            GameAudio.UpdateGlobalVolume();
            if (Globals.GameState == GameStates.Menu)
            {
                Hide();
                _mainMenu.Show();
            }
            else
            {
                Hide();
            }
        }
        void _musicSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            _musicLabel.Text = "Music Volume: " + (int)_musicSlider.Value + "%";
            Globals.Database.MusicVolume = (int)_musicSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void _soundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            _soundLabel.Text = "Sound Volume: " + (int)_soundSlider.Value + "%";
            Globals.Database.SoundVolume = (int)_soundSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }
        void ApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            bool shouldReset = false;
            var resolution = _resolutionList.SelectedItem;
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                if (resolution.Text == myModes[i])
                {
                    if (Globals.Database.TargetResolution != i) shouldReset = true;
                    Globals.Database.TargetResolution = i;
                }
            }
            if (Globals.Database.FullScreen != _fullscreen.IsChecked)
            {
                Globals.Database.FullScreen = _fullscreen.IsChecked;
                shouldReset = true;
            }
            var newFps = 0;
            switch (_fpsList.SelectedItem.Text)
            {
                case "V-Sync":
                    //Stick with 0
                    break;
                case "No Limit":
                    newFps = -1;
                    break;
                case "30":
                    newFps = 1;
                    break;
                case "60":
                    newFps = 2;
                    break;
                case "90":
                    newFps = 3;
                    break;
                case "120":
                    newFps = 4;
                    break;

            }
            if (newFps != Globals.Database.TargetFps)
            {
                shouldReset = true;
                Globals.Database.TargetFps = newFps;
            }
            Globals.Database.MusicVolume = (int)_musicSlider.Value;
            Globals.Database.SoundVolume = (int)_soundSlider.Value;
            GameAudio.UpdateGlobalVolume();
            Globals.Database.SavePreferences();
            if (shouldReset) GameGraphics.Renderer.Init();
            if (Globals.GameState == GameStates.InGame)
            {
                this.Hide();
            }
            else
            {
                Hide();
                _mainMenu.Show();
            }
        }
    }
}
