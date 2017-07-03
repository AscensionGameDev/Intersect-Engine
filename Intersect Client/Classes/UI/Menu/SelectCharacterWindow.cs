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
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Menu
{
    public class SelectCharacterWindow
    {
		public List<Character> Characters = new List<Character>();

		private ImagePanel _characterContainer;
		private ImagePanel _characterPortrait;

		//Image
		private string _characterPortraitImg = "";
        private Label _charnameLabel;
        private Label _infoLabel;

		//Parent
		private MainMenu _mainMenu;
        private Label _menuHeader;
		private ImagePanel _menuPanel;

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
            _menuPanel = new ImagePanel(parent)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png")
            };
            _menuPanel.SetSize(512, 393);
            if (_mainMenu != null && parentPanel != null)
            {
                _menuPanel.SetPosition(parentPanel.X, parentPanel.Y);
            }
            else
            {
                _menuPanel.SetPosition(parent.Width / 2 - _menuPanel.Width / 2,
                    parent.Height / 2 - _menuPanel.Height / 2);
            }
            _menuPanel.IsHidden = true;

            //Menu Header
            _menuHeader = new Label(_menuPanel)
            {
                AutoSizeToContents = false
            };
            _menuHeader.SetText(Strings.Get("characterselection", "title"));
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 24);
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

			//Character Name
			_charnameLabel = new Label(_menuPanel);
			_charnameLabel.SetText(Strings.Get("characterselection", "nochar"));
			_charnameLabel.AutoSizeToContents = false;
			_charnameLabel.SetSize(178, 60);
			_charnameLabel.Alignment = Pos.Center;
			_charnameLabel.SetPosition(_menuPanel.Width / 2, (_menuPanel.Height / 2) - 60);
			_charnameLabel.TextColorOverride = new Color(255, 30, 30, 30);
			_charnameLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

			//Character Info
			_infoLabel = new Label(_menuPanel);
			_infoLabel.SetText(Strings.Get("characterselection", "new"));
			_infoLabel.AutoSizeToContents = false;
			_infoLabel.SetSize(178, 60);
			_infoLabel.Alignment = Pos.Center;
			_infoLabel.SetPosition(_menuPanel.Width / 2, (_menuPanel.Height / 2) - 30);
			_infoLabel.TextColorOverride = new Color(255, 30, 30, 30);
			_infoLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

			//Character Container
			_characterContainer = new ImagePanel(_menuPanel);
			_characterContainer.SetSize(74, 74);
			_characterContainer.SetPosition((_menuPanel.Width / 2) - (_characterContainer.Width / 2), _menuPanel.Height / 2);
			_characterContainer.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,"charview.png");

			//Character sprite
			_characterPortrait = new ImagePanel(_characterContainer);
			_characterPortrait.SetSize(48, 48);

			//Next char Button
			_nextCharButton = new Button(_characterContainer);
			_nextCharButton.SetSize(15, 15);
			_nextCharButton.SetPosition(74 - 15, _characterContainer.Height / 2 - 15 / 2);
			_nextCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrownormal.png"),
				Button.ControlState.Normal);
			_nextCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrowhover.png"),
				Button.ControlState.Hovered);
			_nextCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "rightarrowclicked.png"),
				Button.ControlState.Clicked);
			_nextCharButton.Clicked += _nextCharButton_Clicked;

			//Prev Char Button
			_prevCharButton = new Button(_characterContainer);
			_prevCharButton.SetSize(15, 15);
			_prevCharButton.SetPosition(0, _characterContainer.Height / 2 - 15 / 2);
			_prevCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrownormal.png"),
				Button.ControlState.Normal);
			_prevCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowhover.png"),
				Button.ControlState.Hovered);
			_prevCharButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowclicked.png"),
				Button.ControlState.Clicked);
			_prevCharButton.Clicked += _prevCharButton_Clicked;

			//Play Button
			_playButton = new Button(_menuPanel);
			_playButton.SetText(Strings.Get("charselection", "play"));
			_playButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
			_playButton.SetSize(211, 61);
			_playButton.SetPosition(_menuPanel.Width / 2 - _playButton.Width / 2, (_menuPanel.Height / 2) + 100);
			_playButton.Clicked += _playButton_Clicked;
			_playButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
				Button.ControlState.Normal);
			_playButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
				Button.ControlState.Hovered);
			_playButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
				Button.ControlState.Clicked);
			_playButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
			_playButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
			_playButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
			_playButton.Hide();

			//Delete Button
			_deleteButton = new Button(_menuPanel);
			_deleteButton.SetText(Strings.Get("charselection", "delete"));
			_deleteButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
			_deleteButton.SetSize(211, 61);
			_deleteButton.SetPosition(_menuPanel.Width / 2 - _deleteButton.Width / 2, (_menuPanel.Height / 2) + 150);
			_deleteButton.Clicked += _deleteButton_Clicked;
			_deleteButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
				Button.ControlState.Normal);
			_deleteButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
				Button.ControlState.Hovered);
			_deleteButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
				Button.ControlState.Clicked);
			_deleteButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
			_deleteButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
			_deleteButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
			_deleteButton.Hide();

			//Creat new char Button
			_newButton = new Button(_menuPanel);
			_newButton.SetText(Strings.Get("charselection", "new"));
			_newButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
			_newButton.SetSize(211, 61);
			_newButton.SetPosition(_menuPanel.Width / 2 - _newButton.Width / 2, (_menuPanel.Height / 2) + 200);
			_newButton.Clicked += _newButton_Clicked;
			_newButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
				Button.ControlState.Normal);
			_newButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
				Button.ControlState.Hovered);
			_newButton.SetImage(
				Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
				Button.ControlState.Clicked);
			_newButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
			_newButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
			_newButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

			Update();
        }

        //Methods
        public void Update()
        {
			//Show arrows when user has more than 1 character
			if (Characters.Count > 1)
			{
				_nextCharButton.Show();
				_prevCharButton.Show();
			}
			else
			{
				_nextCharButton.Hide();
				_prevCharButton.Hide();
			}

			if (Characters.Count > 0)
			{
				_charnameLabel.SetText(Characters[selectedChar].Name);
				_infoLabel.SetText(Strings.Get("characterselection", "info", Characters[selectedChar].Level, Characters[selectedChar].Class));
				_playButton.Show();
				_deleteButton.Show();

				_characterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Face, Characters[selectedChar].Face);
				if (_characterPortrait.Texture == null)
				{
					_characterPortrait.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, Characters[selectedChar].Sprite);
				}
			}
        }

        public void Show()
        {
            _menuPanel.Show();
        }

        public void Hide()
        {
            _menuPanel.Hide();
        }

		private void _prevCharButton_Clicked(Base sender, ClickedEventArgs arguments)
		{
			selectedChar--;
			if (selectedChar < 0) { selectedChar = Characters.Count - 1; }
		}

		private void _nextCharButton_Clicked(Base sender, ClickedEventArgs arguments)
		{
			selectedChar++;
			if (selectedChar >= Characters.Count) { selectedChar = 0; }
		}

		private void _playButton_Clicked(Base sender, ClickedEventArgs arguments)
		{
			PacketSender.PlayGame(Characters[selectedChar].Slot);
		}

		private void _deleteButton_Clicked(Base sender, ClickedEventArgs arguments)
		{
			PacketSender.DeleteChar(Characters[selectedChar].Slot);
			Characters.RemoveAt(selectedChar);
		}

		private void _newButton_Clicked(Base sender, ClickedEventArgs arguments)
		{
			PacketSender.CreateNewCharacter();
		}
	}

	public class Character
	{
		public int Slot = 1;
		public string Name = "";
		public string Sprite = "";
		public string Face = "";
		public int Level = 1;
		public int Class = 0;
		public int[] Equipment = new int[Options.EquipmentSlots.Count];

		public Character(int slot, string name, string sprite, string face, int level, int charClass)
		{
			for (int i = 0; i < Options.EquipmentSlots.Count; i++)
			{
				Equipment[i] = -1;
			}
			Slot = slot;
			Name = name;
			Sprite = sprite;
			Face = face;
			Level = level;
			Class = charClass;
		}

		public Character()
		{
			for (int i = 0; i < Options.EquipmentSlots.Count; i++)
			{
				Equipment[i] = -1;
			}
		}
	}
}