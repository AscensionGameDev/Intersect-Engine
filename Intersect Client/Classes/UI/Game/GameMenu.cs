using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class GameMenu
    {
        //Menu Container
        private ImagePanel _menuContainer;

        //Canvas instance
        Canvas _GameCanvas;
        private ImagePanel _inventoryBackground;
        //Control Variables
        private Button _inventoryButton;
        private InventoryWindow _inventoryWindow;
        private ImagePanel _optionsBackground;
        private Button _optionsButton;

        //Window References
        private OptionsWindow _optionsWindow;
        private ImagePanel _partyBackground;
        private Button _partyButton;
        private PartyWindow _partyWindow;
        private ImagePanel _friendsBackground;
        private Button _friendsButton;
        private FriendsWindow _friendsWindow;
        private ImagePanel _questsBackground;
        private Button _questsButton;
        private QuestsWindow _questsWindow;
        private ImagePanel _spellsBackground;
        private Button _spellsButton;
        private SpellsWindow _spellsWindow;
        private ImagePanel _characterBackground;
        private Button _characterButton;
        private CharacterWindow _characterWindow;
        private ImagePanel _closeBackground;
        private Button _closeButton;
        private int backgroundHeight = 42;
        private int backgroundWidth = 42;
        private int buttonHeight = 34;
        private int buttonMargin = 8;

        private int buttonWidth = 34;

        //Init
        public GameMenu(Canvas _gameCanvas)
        {
            _GameCanvas = _gameCanvas;

            _menuContainer = new ImagePanel(_gameCanvas,"MenuContainer");
            
            _inventoryBackground = new ImagePanel(_menuContainer, "InventoryContainer");
            _inventoryButton = new Button(_inventoryBackground,"InventoryButton");
            _inventoryButton.SetToolTipText(Strings.Get("gamemenu", "items"));
            _inventoryButton.Clicked += InventoryButton_Clicked;

            _spellsBackground = new ImagePanel(_menuContainer, "SpellsContainer");
            _spellsButton = new Button(_spellsBackground, "SpellsButton");
            _spellsButton.SetToolTipText(Strings.Get("gamemenu", "spells"));
            _spellsButton.Clicked += SpellsButton_Clicked;

            _characterBackground = new ImagePanel(_menuContainer, "CharacterContainer");
            _characterButton = new Button(_characterBackground, "CharacterButton");
            _characterButton.SetToolTipText(Strings.Get("gamemenu", "character"));
            _characterButton.Clicked += CharacterButton_Clicked;

            _questsBackground = new ImagePanel(_menuContainer, "QuestsContainer");
            _questsButton = new Button(_questsBackground, "QuestsButton");
            _questsButton.SetToolTipText(Strings.Get("gamemenu", "quest"));
            _questsButton.Clicked += QuestBtn_Clicked;

            _friendsBackground = new ImagePanel(_menuContainer, "FriendsContainer");
            _friendsButton = new Button(_friendsBackground, "FriendsButton");
            _friendsButton.SetToolTipText(Strings.Get("gamemenu", "friends"));
            _friendsButton.Clicked += FriendsBtn_Clicked;

            _partyBackground = new ImagePanel(_menuContainer, "PartyContainer");
            _partyButton = new Button(_partyBackground, "PartyButton");
            _partyButton.SetToolTipText(Strings.Get("gamemenu", "party"));
            _partyButton.Clicked += PartyBtn_Clicked;

            _optionsBackground = new ImagePanel(_menuContainer, "OptionsContainer");
            _optionsButton = new Button(_optionsBackground, "OptionsButton");
            _optionsButton.SetToolTipText(Strings.Get("gamemenu", "options"));
            _optionsButton.Clicked += OptionBtn_Clicked;

            //Go in reverse order from the right
            _closeBackground = new ImagePanel(_menuContainer, "ExitGameContainer");
            _closeButton = new Button(_closeBackground, "ExitGameButton");
            _closeButton.SetToolTipText(Strings.Get("gamemenu", "exit"));
            _closeButton.Clicked += CloseBtn_Clicked;

            //Assign Window References
            _optionsWindow = new OptionsWindow(_gameCanvas, null, null);
            _partyWindow = new PartyWindow(_gameCanvas);
            _friendsWindow = new FriendsWindow(_gameCanvas);
            _inventoryWindow = new InventoryWindow(_gameCanvas);
            _spellsWindow = new SpellsWindow(_gameCanvas);
            _characterWindow = new CharacterWindow(_gameCanvas);
            _questsWindow = new QuestsWindow(_gameCanvas);
        }

        //Methods
        public void Update(bool updateQuestLog)
        {
            _inventoryWindow.Update();
            _spellsWindow.Update();
            _characterWindow.Update();
            _partyWindow.Update();
            _friendsWindow.Update();
            _questsWindow.Update(updateQuestLog);
            _optionsWindow.Update();
        }

        public void UpdateFriendsList()
        {
            _friendsWindow.updateList();
        }

        //Input Handlers
        void CloseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }

        void OptionBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_optionsWindow.IsVisible())
            {
                _optionsWindow.Hide();
            }
            else
            {
                if (!Globals.Me.IsBusy())
                {
                    _optionsWindow.Show();
                }
            }
        }

        void PartyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_partyWindow.IsVisible())
            {
                _partyWindow.Hide();
            }
            else
            {
                _partyWindow.Show();
            }
        }

        void FriendsBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_friendsWindow.IsVisible())
            {
                _friendsWindow.Hide();
            }
            else
            {
                _friendsWindow.Show();
                PacketSender.RequestFriends();
            }
        }

        void QuestBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_questsWindow.IsVisible())
            {
                _questsWindow.Hide();
            }
            else
            {
                _questsWindow.Show();
            }
        }

        void InventoryButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_inventoryWindow.IsVisible())
            {
                _inventoryWindow.Hide();
            }
            else
            {
                _inventoryWindow.Show();
            }
        }

        void SpellsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_spellsWindow.IsVisible())
            {
                _spellsWindow.Hide();
            }
            else
            {
                _spellsWindow.Show();
            }
        }

        void CharacterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_characterWindow.IsVisible())
            {
                _characterWindow.Hide();
            }
            else
            {
                _characterWindow.Show();
            }
        }
    }
}