using System;
using System.Collections.Generic;
using Intersect;
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
using Intersect_Client.Classes.UI.Game;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Menu
{
    public class SelectCharacterWindow
    {
        public List<Character> Characters = new List<Character>();

        private ImagePanel _characterContainer;
        private ImagePanel _characterPortrait;
        private ImagePanel[] _paperdollPortraits;

        //Image
        private string _characterPortraitImg = "";
        private Label _charnameLabel;
        private Label _infoLabel;

        //Parent
        private MainMenu _mainMenu;
        private Label _characterSelectionHeader;
        private ImagePanel _characterSelectionPanel;

        //Controls
        private Button _nextCharButton;
        private Button _prevCharButton;
        private Button _playButton;
        private Button _deleteButton;
        private Button _newButton;

        //Selected Char
        private int selectedChar = 0;

        //Init
        public SelectCharacterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _characterSelectionPanel = new ImagePanel(parent, "CharacterSelectionWindow");

            //Menu Header
            _characterSelectionHeader = new Label(_characterSelectionPanel, "CharacterSelectionHeader");
            _characterSelectionHeader.SetText(Strings.Get("CharacterSelection", "title"));

            //Character Name
            _charnameLabel = new Label(_characterSelectionPanel,"CharacterNameLabel");
            _charnameLabel.SetText(Strings.Get("CharacterSelection", "nochar"));

            //Character Info
            _infoLabel = new Label(_characterSelectionPanel, "CharacterInfoLabel");
            _infoLabel.SetText(Strings.Get("CharacterSelection", "new"));

            //Character Container
            _characterContainer = new ImagePanel(_characterSelectionPanel, "CharacterContainer");

            //Character sprite
            _characterPortrait = new ImagePanel(_characterContainer);
            _characterPortrait.SetSize(48, 48);

            //Next char Button
            _nextCharButton = new Button(_characterContainer, "NextCharacterButton");
            _nextCharButton.Clicked += _nextCharButton_Clicked;

            //Prev Char Button
            _prevCharButton = new Button(_characterContainer, "PreviousCharacterButton");
            _prevCharButton.Clicked += _prevCharButton_Clicked;

            //Play Button
            _playButton = new Button(_characterSelectionPanel, "PlayButton");
            _playButton.SetText(Strings.Get("CharacterSelection", "play"));
            _playButton.Clicked += _playButton_Clicked;
            _playButton.Hide();

            //Delete Button
            _deleteButton = new Button(_characterSelectionPanel, "DeleteButton");
            _deleteButton.SetText(Strings.Get("CharacterSelection", "delete"));
            _deleteButton.Clicked += _deleteButton_Clicked;
            _deleteButton.Hide();

            //Create new char Button
            _newButton = new Button(_characterSelectionPanel, "NewButton");
            _newButton.SetText(Strings.Get("CharacterSelection", "new"));
            _newButton.Clicked += _newButton_Clicked;
        }

        //Methods
        public void Update()
        {
            var isFace = true;

            //Show and hide options based on the character count
            if (Characters.Count > 1)
            {
                _nextCharButton.Show();
                _prevCharButton.Show();
            }

            if (Characters.Count <= 1)
            {
                _nextCharButton.Hide();
                _prevCharButton.Hide();
            }


            if (_paperdollPortraits == null)
            {
                _paperdollPortraits = new ImagePanel[Options.EquipmentSlots.Count];
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    _paperdollPortraits[i] = new ImagePanel(_characterContainer);
                }
                _nextCharButton.BringToFront();
                _prevCharButton.BringToFront();
            }
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                _paperdollPortraits[i].Hide();
            }

            if (Characters[selectedChar].Id > -1)
            {
                _charnameLabel.SetText(Strings.Get("CharacterSelection","name",Characters[selectedChar].Name));
                _infoLabel.SetText(Strings.Get("CharacterSelection", "info", Characters[selectedChar].Level, Characters[selectedChar].Class));
                _infoLabel.Show();
                _playButton.Show();
                _deleteButton.Show();
                _newButton.Hide();

                _characterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, Characters[selectedChar].Face);
                if (_characterPortrait.Texture == null)
                {
                    _characterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, Characters[selectedChar].Sprite);
                    isFace = false;
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

                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            _paperdollPortraits[i].Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll, Characters[selectedChar].Equipment[i]);
                            if (_paperdollPortraits[i].Texture != null)
                            {
                                _paperdollPortraits[i].Show();
                                _paperdollPortraits[i].SetTextureRect(0, 0,
                                    _paperdollPortraits[i].Texture.GetWidth() / 4,
                                    _paperdollPortraits[i].Texture.GetHeight() / 4);
                                _paperdollPortraits[i].SetSize(_paperdollPortraits[i].Texture.GetWidth() / 4,
                                    _paperdollPortraits[i].Texture.GetHeight() / 4);
                                _paperdollPortraits[i].SetPosition(
                                    _characterContainer.Width / 2 - _paperdollPortraits[i].Width / 2,
                                    _characterContainer.Height / 2 - _paperdollPortraits[i].Height / 2);
                            }
                        }
                    }
                }
            }
            else
            {
                _playButton.Hide();
                _deleteButton.Hide();
                _newButton.Show();

                _charnameLabel.SetText(Strings.Get("CharacterSelection", "empty"));
                _infoLabel.Hide();

                _characterPortrait.Texture = null;
            }
        }

        public void Show()
        {
            selectedChar = 0;
            Update();
            _characterSelectionPanel.Show();
        }

        public void Hide()
        {
            _characterSelectionPanel.Hide();
        }

        private void _prevCharButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            selectedChar--;
            if (selectedChar < 0) { selectedChar = Characters.Count - 1; }
            Update();
        }

        private void _nextCharButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            selectedChar++;
            if (selectedChar >= Characters.Count) { selectedChar = 0; }
            Update();
        }

        private void _playButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.PlayGame(Characters[selectedChar].Id);
        }

        private void _deleteButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            InputBox iBox = new InputBox(Strings.Get("characterselection", "deletetitle",Characters[selectedChar].Name),
                        Strings.Get("characterselection", "deleteprompt", Characters[selectedChar].Name),
                        true, InputBox.InputType.YesNo, DeleteCharacter, null, Characters[selectedChar].Id,_characterSelectionPanel.Parent);
            
        }

        private void DeleteCharacter(Object sender, EventArgs e)
        {
            PacketSender.DeleteChar(((InputBox)sender).UserData);
            selectedChar = 0;
            Update();
        }

        private void _newButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.CreateNewCharacter();
        }
    }

    public class Character
    {
        public bool Exists = false;
        public int Id = -1;
        public string Name = "";
        public string Sprite = "";
        public string Face = "";
        public int Level = 1;
        public string Class = "";
        public string[] Equipment = new string[Options.EquipmentSlots.Count];

        public Character(int id)
        {
            Id = id;
        }

        public Character(int id, string name, string sprite, string face, int level, string charClass)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = "";
            }
            Id = id;
            Name = name;
            Sprite = sprite;
            Face = face;
            Level = level;
            Class = charClass;
            Exists = true;
        }

        public Character()
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = "";
            }
        }
    }
}