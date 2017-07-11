using System;
using System.Collections.Generic;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class CreateCharacterWindow
    {
        private ImagePanel _characterContainer;

        private ImagePanel _characterNameBackground;
        private ImagePanel _characterPortrait;

        //Image
        private string _characterPortraitImg = "";
        private Label _charnameLabel;
        private TextBox _charnameTextbox;

        private ImagePanel _classBackground;
        private ComboBox _classCombobox;
        private Label _classLabel;
        private Button _createButton;
        private int _displaySpriteIndex = -1;
        private LabeledCheckBox _femaleChk;
        private List<KeyValuePair<int, ClassSprite>> _femaleSprites = new List<KeyValuePair<int, ClassSprite>>();

        private ImagePanel _genderBackground;
        private Label _genderLabel;

        //Parent
        private MainMenu _mainMenu;
        private LabeledCheckBox _maleChk;

        //Class Info
        private List<KeyValuePair<int, ClassSprite>> _maleSprites = new List<KeyValuePair<int, ClassSprite>>();
        private Label _charCreationHeader;
        //Controls
        private ImagePanel _charCreationPanel;
        private Button _nextSpriteButton;
        private Button _prevSpriteButton;

        //Init
        public CreateCharacterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _charCreationPanel = new ImagePanel(parent, "CharacterCreationWindow");
            _charCreationPanel.IsHidden = true;

            //Menu Header
            _charCreationHeader = new Label(_charCreationPanel, "CharacterCreationHeader");
            _charCreationHeader.SetText(Strings.Get("charactercreation", "title"));

            //Character Name Background
            _characterNameBackground = new ImagePanel(_charCreationPanel, "CharacterNamePanel");

            //Character name Label
            _charnameLabel = new Label(_characterNameBackground,"CharacterNameLabel");
            _charnameLabel.SetText(Strings.Get("charactercreation", "name"));

            //Character name Textbox
            _charnameTextbox = new TextBox(_characterNameBackground,"CharacterNameField");
            _charnameTextbox.SubmitPressed += CharnameTextbox_SubmitPressed;

            //Class Background
            _classBackground = new ImagePanel(_charCreationPanel, "ClassPanel");

            //Class Label
            _classLabel = new Label(_classBackground,"ClassLabel");
            _classLabel.SetText(Strings.Get("charactercreation", "class"));

            //Class Combobox
            _classCombobox = new ComboBox(_classBackground,"ClassCombobox");
            _classCombobox.ItemSelected += classCombobox_ItemSelected;

            //Character Container
            _characterContainer = new ImagePanel(_charCreationPanel,"CharacterContainer");

            //Character sprite
            _characterPortrait = new ImagePanel(_characterContainer,"CharacterPortait");
            _characterPortrait.SetSize(48, 48);

            //Next Sprite Button
            _nextSpriteButton = new Button(_characterContainer,"NextSpriteButton");
            _nextSpriteButton.Clicked += _nextSpriteButton_Clicked;

            //Prev Sprite Button
            _prevSpriteButton = new Button(_characterContainer,"PreviousSpriteButton");

            //Class Background
            _genderBackground = new ImagePanel(_charCreationPanel, "GeneralPanel");

            //Gender Label
            _genderLabel = new Label(_genderBackground,"GenderLabel");
            _genderLabel.SetText(Strings.Get("charactercreation", "gender"));

            //Male Checkbox
            _maleChk = new LabeledCheckBox(_genderBackground,"MaleCheckbox") {Text = Strings.Get("charactercreation", "male")};
            _maleChk.IsChecked = true;
            _maleChk.Checked += maleChk_Checked;
            _maleChk.UnChecked += femaleChk_Checked; // If you notice this, feel free to hate us ;)

            //Female Checkbox
            _femaleChk = new LabeledCheckBox(_genderBackground,"FemaleCheckbox") {Text = Strings.Get("charactercreation", "female")};
            _femaleChk.Checked += femaleChk_Checked;
            _femaleChk.UnChecked += maleChk_Checked;

            //Register - Send Registration Button
            _createButton = new Button(_charCreationPanel,"CreateButton");
            _createButton.SetText(Strings.Get("charactercreation", "create"));
            _createButton.Clicked += CreateButton_Clicked;
        }

        public void Init()
        {
            _classCombobox.DeleteAll();
            var classCount = 0;
            foreach (ClassBase cls in ClassBase.Lookup.Values)
            {
                if (cls.Locked == 0)
                {
                    _classCombobox.AddItem(cls.Name);
                    classCount++;
                }
            }
            LoadClass();
            Update();
        }

        //Methods
        public void Update()
        {
            var isFace = true;
            if (GetClass() != null && _displaySpriteIndex != -1)
            {
                _characterPortrait.IsHidden = false;
                if (GetClass().Sprites.Count > 0)
                {
                    if (_maleChk.IsChecked)
                    {
                        _characterPortrait.Texture =
                            Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face,
                                _maleSprites[_displaySpriteIndex].Value.Face);
                        if (_characterPortrait.Texture == null)
                        {
                            _characterPortrait.Texture =
                                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                                    _maleSprites[_displaySpriteIndex].Value.Sprite);
                            isFace = false;
                        }
                    }
                    else
                    {
                        _characterPortrait.Texture =
                            Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face,
                                _femaleSprites[_displaySpriteIndex].Value.Face);
                        if (_characterPortrait.Texture == null)
                        {
                            _characterPortrait.Texture =
                                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                                    _femaleSprites[_displaySpriteIndex].Value.Sprite);
                            isFace = false;
                        }
                    }
                    if (_characterPortrait.Texture != null)
                    {
                        if (isFace)
                        {
                            _characterPortrait.SetTextureRect(0, 0, _characterPortrait.Texture.GetWidth(),
                                _characterPortrait.Texture.GetHeight());
                            _characterPortrait.SetSize(64, 64);
                            _characterPortrait.SetPosition(5, 5);
                        }
                        else
                        {
                            _characterPortrait.SetTextureRect(0, 0, _characterPortrait.Texture.GetWidth() / 4,
                                _characterPortrait.Texture.GetHeight() / 4);
                            _characterPortrait.SetSize(_characterPortrait.Texture.GetWidth() / 4,
                                _characterPortrait.Texture.GetHeight() / 4);
                            _characterPortrait.SetPosition(_characterContainer.Width / 2 - _characterPortrait.Width / 2,
                                _characterContainer.Height / 2 - _characterPortrait.Height / 2);
                        }
                    }
                }
            }
            else
            {
                _characterPortrait.IsHidden = true;
            }
        }

        public void Show()
        {
            _charCreationPanel.Show();
        }

        public void Hide()
        {
            _charCreationPanel.Hide();
        }

        private ClassBase GetClass()
        {
            if (_classCombobox.SelectedItem == null) return null;
            foreach (var cls in ClassBase.Lookup)
            {
                if (_classCombobox.SelectedItem.Text == cls.Value.Name && ((ClassBase)cls.Value).Locked == 0)
                {
                    return (ClassBase) cls.Value;
                }
            }
            return null;
        }

        private void LoadClass()
        {
            ClassBase cls = GetClass();
            _maleSprites.Clear();
            _femaleSprites.Clear();
            _displaySpriteIndex = -1;
            if (cls != null)
            {
                for (int i = 0; i < cls.Sprites.Count; i++)
                {
                    if (cls.Sprites[i].Gender == 0)
                    {
                        _maleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                    }
                    else
                    {
                        _femaleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                    }
                }
            }
            ResetSprite();
        }

        private void ResetSprite()
        {
            _nextSpriteButton.IsHidden = true;
            _prevSpriteButton.IsHidden = true;
            if (_maleChk.IsChecked)
            {
                if (_maleSprites.Count > 0)
                {
                    _displaySpriteIndex = 0;
                    if (_maleSprites.Count > 1)
                    {
                        _nextSpriteButton.IsHidden = false;
                        _prevSpriteButton.IsHidden = false;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
            else
            {
                if (_femaleSprites.Count > 0)
                {
                    _displaySpriteIndex = 0;
                    if (_femaleSprites.Count > 1)
                    {
                        _nextSpriteButton.IsHidden = false;
                        _prevSpriteButton.IsHidden = false;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
        }

        private void _prevSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _displaySpriteIndex--;
            if (_maleChk.IsChecked)
            {
                if (_maleSprites.Count > 0)
                {
                    if (_displaySpriteIndex == -1)
                    {
                        _displaySpriteIndex = _maleSprites.Count - 1;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
            else
            {
                if (_femaleSprites.Count > 0)
                {
                    if (_displaySpriteIndex == -1)
                    {
                        _displaySpriteIndex = _femaleSprites.Count - 1;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
            Update();
        }

        private void _nextSpriteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _displaySpriteIndex++;
            if (_maleChk.IsChecked)
            {
                if (_maleSprites.Count > 0)
                {
                    if (_displaySpriteIndex >= _maleSprites.Count)
                    {
                        _displaySpriteIndex = 0;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
            else
            {
                if (_femaleSprites.Count > 0)
                {
                    if (_displaySpriteIndex >= _femaleSprites.Count)
                    {
                        _displaySpriteIndex = 0;
                    }
                }
                else
                {
                    _displaySpriteIndex = -1;
                }
            }
            Update();
        }

        void TryCreateCharacter(int Gender)
        {
            if (Globals.WaitingOnServer || _displaySpriteIndex == -1)
            {
                return;
            }
            if (FieldChecking.IsValidName(_charnameTextbox.Text))
            {
                GameFade.FadeOut();
                if (_maleChk.IsChecked)
                {
                    PacketSender.SendCreateCharacter(_charnameTextbox.Text, GetClass().Index,
                        _maleSprites[_displaySpriteIndex].Key);
                }
                else
                {
                    PacketSender.SendCreateCharacter(_charnameTextbox.Text, GetClass().Index,
                        _femaleSprites[_displaySpriteIndex].Key);
                }
                Globals.WaitingOnServer = true;
            }
            else
            {
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Get("charactercreation", "invalidname")));
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
            LoadClass();
            Update();
        }

        void maleChk_Checked(Base sender, EventArgs arguments)
        {
            _maleChk.IsChecked = true;
            _femaleChk.IsChecked = false;
            ResetSprite();
            Update();
        }

        void femaleChk_Checked(Base sender, EventArgs arguments)
        {
            _femaleChk.IsChecked = true;
            _maleChk.IsChecked = false;
            ResetSprite();
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