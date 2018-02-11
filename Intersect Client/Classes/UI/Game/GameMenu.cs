using Intersect.Localization;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    class GameMenu
    {
        private ImagePanel mCharacterBackground;
        private Button mCharacterButton;
        private CharacterWindow mCharacterWindow;
        private ImagePanel mCloseBackground;
        private Button mCloseButton;
        private ImagePanel mFriendsBackground;
        private Button mFriendsButton;
        private FriendsWindow mFriendsWindow;

        //Canvas instance
        Canvas mGameCanvas;

        private ImagePanel mInventoryBackground;

        //Control Variables
        private Button mInventoryButton;

        private InventoryWindow mInventoryWindow;

        //Menu Container
        private ImagePanel mMenuContainer;

        private ImagePanel mOptionsBackground;
        private Button mOptionsButton;

        //Window References
        private OptionsWindow mOptionsWindow;

        private ImagePanel mPartyBackground;
        private Button mPartyButton;
        private PartyWindow mPartyWindow;
        private ImagePanel mQuestsBackground;
        private Button mQuestsButton;
        private QuestsWindow mQuestsWindow;
        private ImagePanel mSpellsBackground;
        private Button mSpellsButton;
        private SpellsWindow mSpellsWindow;
        private int mBackgroundHeight = 42;
        private int mBackgroundWidth = 42;
        private int mButtonHeight = 34;
        private int mButtonMargin = 8;

        private int mButtonWidth = 34;

        //Init
        public GameMenu(Canvas gameCanvas)
        {
            mGameCanvas = gameCanvas;

            mMenuContainer = new ImagePanel(gameCanvas, "MenuContainer");

            mInventoryBackground = new ImagePanel(mMenuContainer, "InventoryContainer");
            mInventoryButton = new Button(mInventoryBackground, "InventoryButton");
            mInventoryButton.SetToolTipText(Strings.Get("gamemenu", "items"));
            mInventoryButton.Clicked += InventoryButton_Clicked;

            mSpellsBackground = new ImagePanel(mMenuContainer, "SpellsContainer");
            mSpellsButton = new Button(mSpellsBackground, "SpellsButton");
            mSpellsButton.SetToolTipText(Strings.Get("gamemenu", "spells"));
            mSpellsButton.Clicked += SpellsButton_Clicked;

            mCharacterBackground = new ImagePanel(mMenuContainer, "CharacterContainer");
            mCharacterButton = new Button(mCharacterBackground, "CharacterButton");
            mCharacterButton.SetToolTipText(Strings.Get("gamemenu", "character"));
            mCharacterButton.Clicked += CharacterButton_Clicked;

            mQuestsBackground = new ImagePanel(mMenuContainer, "QuestsContainer");
            mQuestsButton = new Button(mQuestsBackground, "QuestsButton");
            mQuestsButton.SetToolTipText(Strings.Get("gamemenu", "quest"));
            mQuestsButton.Clicked += QuestBtn_Clicked;

            mFriendsBackground = new ImagePanel(mMenuContainer, "FriendsContainer");
            mFriendsButton = new Button(mFriendsBackground, "FriendsButton");
            mFriendsButton.SetToolTipText(Strings.Get("gamemenu", "friends"));
            mFriendsButton.Clicked += FriendsBtn_Clicked;

            mPartyBackground = new ImagePanel(mMenuContainer, "PartyContainer");
            mPartyButton = new Button(mPartyBackground, "PartyButton");
            mPartyButton.SetToolTipText(Strings.Get("gamemenu", "party"));
            mPartyButton.Clicked += PartyBtn_Clicked;

            mOptionsBackground = new ImagePanel(mMenuContainer, "OptionsContainer");
            mOptionsButton = new Button(mOptionsBackground, "OptionsButton");
            mOptionsButton.SetToolTipText(Strings.Get("gamemenu", "options"));
            mOptionsButton.Clicked += OptionBtn_Clicked;

            //Go in reverse order from the right
            mCloseBackground = new ImagePanel(mMenuContainer, "ExitGameContainer");
            mCloseButton = new Button(mCloseBackground, "ExitGameButton");
            mCloseButton.SetToolTipText(Strings.Get("gamemenu", "exit"));
            mCloseButton.Clicked += CloseBtn_Clicked;

            //Assign Window References
            mOptionsWindow = new OptionsWindow(gameCanvas, null, null);
            mPartyWindow = new PartyWindow(gameCanvas);
            mFriendsWindow = new FriendsWindow(gameCanvas);
            mInventoryWindow = new InventoryWindow(gameCanvas);
            mSpellsWindow = new SpellsWindow(gameCanvas);
            mCharacterWindow = new CharacterWindow(gameCanvas);
            mQuestsWindow = new QuestsWindow(gameCanvas);
        }

        //Methods
        public void Update(bool updateQuestLog)
        {
            mInventoryWindow.Update();
            mSpellsWindow.Update();
            mCharacterWindow.Update();
            mPartyWindow.Update();
            mFriendsWindow.Update();
            mQuestsWindow.Update(updateQuestLog);
            mOptionsWindow.Update();
        }

        public void UpdateFriendsList()
        {
            mFriendsWindow.UpdateList();
        }

        //Input Handlers
        void CloseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }

        void OptionBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mOptionsWindow.IsVisible())
            {
                mOptionsWindow.Hide();
            }
            else
            {
                if (!Globals.Me.IsBusy())
                {
                    mOptionsWindow.Show();
                }
            }
        }

        void PartyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mPartyWindow.IsVisible())
            {
                mPartyWindow.Hide();
            }
            else
            {
                mPartyWindow.Show();
            }
        }

        void FriendsBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mFriendsWindow.IsVisible())
            {
                mFriendsWindow.Hide();
            }
            else
            {
                mFriendsWindow.Show();
                PacketSender.RequestFriends();
            }
        }

        void QuestBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mQuestsWindow.IsVisible())
            {
                mQuestsWindow.Hide();
            }
            else
            {
                mQuestsWindow.Show();
            }
        }

        void InventoryButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mInventoryWindow.IsVisible())
            {
                mInventoryWindow.Hide();
            }
            else
            {
                mInventoryWindow.Show();
            }
        }

        void SpellsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mSpellsWindow.IsVisible())
            {
                mSpellsWindow.Hide();
            }
            else
            {
                mSpellsWindow.Show();
            }
        }

        void CharacterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (mCharacterWindow.IsVisible())
            {
                mCharacterWindow.Hide();
            }
            else
            {
                mCharacterWindow.Show();
            }
        }
    }
}