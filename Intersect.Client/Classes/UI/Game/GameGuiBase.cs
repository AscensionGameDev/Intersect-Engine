using System;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Networking;
using Intersect.Client.UI.Game.Bag;
using Intersect.Client.UI.Game.Bank;
using Intersect.Client.UI.Game.Chat;
using Intersect.Client.UI.Game.Crafting;
using Intersect.Client.UI.Game.EntityPanel;
using Intersect.Client.UI.Game.Hotbar;
using Intersect.Client.UI.Game.Shop;
using Intersect.Client.UI.Game.Trades;
using Intersect.Enums;
using Intersect.GameObjects;
using JetBrains.Annotations;

namespace Intersect.Client.UI.Game
{
    public class GameGuiBase
    {
        [NotNull]
        public Canvas GameCanvas { get; }

        [NotNull]
        public EscapeMenu EscapeMenu { get; }

        private AdminWindow mAdminWindow;
        private BagWindow mBagWindow;
        private BankWindow mBankWindow;
        private Chatbox mChatBox;
        private CraftingWindow mCraftingWindow;
        private PictureWindow mPictureWindow;
        private DebugMenu mDebugMenu;

        private EventWindow mEventWindow;
        public EntityBox PlayerBox;
        private QuestOfferWindow mQuestOfferWindow;
        private ShopWindow mShopWindow;
        private bool mShouldCloseBag;
        private bool mShouldCloseBank;
        private bool mShouldCloseCraftingTable;
        private bool mShouldCloseShop;
        private bool mShouldCloseTrading;
        private bool mShouldOpenAdminWindow;
        private bool mShouldOpenBag;
        private bool mShouldOpenBank;
        private bool mShouldOpenCraftingTable;
        private bool mShouldOpenShop;
        private bool mShouldOpenTrading;
        private bool mShouldUpdateQuestLog = true;
        private Guid mTradingTargetId;
        private TradingWindow mTradingWindow;
        public bool FocusChat;

        public GameMenu GameMenu { get; private set; }

        //Public Components - For clicking/dragging
        public HotBarWindow Hotbar;

        public GameGuiBase([NotNull] Canvas myCanvas)
        {
            GameCanvas = myCanvas;
            EscapeMenu = new EscapeMenu(GameCanvas) { IsHidden = true };

            InitGameGui();
        }

        public void InitGameGui()
        {
            mEventWindow = new EventWindow(GameCanvas);
            mChatBox = new Chatbox(GameCanvas, this);
            GameMenu = new GameMenu(GameCanvas);
            Hotbar = new HotBarWindow(GameCanvas);
            mDebugMenu = new DebugMenu(GameCanvas);
            mQuestOfferWindow = new QuestOfferWindow(GameCanvas);
            PlayerBox = new EntityBox(GameCanvas, EntityTypes.Player, Globals.Me, true);
        }

        //Chatbox
        public void SetChatboxText(string msg)
        {
            mChatBox.SetChatboxText(msg);
        }

        //Friends Window
        public void UpdateFriendsList()
        {
            GameMenu.UpdateFriendsList();
        }

        //Admin Window
        public void NotifyOpenAdminWindow()
        {
            mShouldOpenAdminWindow = true;
        }

        public void OpenAdminWindow()
        {
            if (mAdminWindow == null)
            {
                mAdminWindow = new AdminWindow(GameCanvas);
            }
            else
            {
                if (mAdminWindow.IsVisible())
                {
                    mAdminWindow.Hide();
                }
                else
                {
                    mAdminWindow.Show();
                }
            }
            mShouldOpenAdminWindow = false;
        }

        //Shop
        public void NotifyOpenShop()
        {
            mShouldOpenShop = true;
        }

        public void NotifyCloseShop()
        {
            mShouldCloseShop = true;
        }

        public void OpenShop()
        {
            if (mShopWindow != null) mShopWindow.Close();
            mShopWindow = new ShopWindow(GameCanvas);
            mShouldOpenShop = false;
        }

        //Bank
        public void NotifyOpenBank()
        {
            mShouldOpenBank = true;
        }

        public void NotifyCloseBank()
        {
            mShouldCloseBank = true;
        }

        public void OpenBank()
        {
            if (mBankWindow != null) mBankWindow.Close();
            mBankWindow = new BankWindow(GameCanvas);
            mShouldOpenBank = false;
            Globals.InBank = true;
        }

        //Bag
        public void NotifyOpenBag()
        {
            mShouldOpenBag = true;
        }

        public void NotifyCloseBag()
        {
            mShouldCloseBag = true;
        }

        public void OpenBag()
        {
            if (mBagWindow != null) mBagWindow.Close();
            mBagWindow = new BagWindow(GameCanvas);
            mShouldOpenBag = false;
            Globals.InBag = true;
        }

        //Crafting
        public void NotifyOpenCraftingTable()
        {
            mShouldOpenCraftingTable = true;
        }

        public void NotifyCloseCraftingTable()
        {
            mShouldCloseCraftingTable = true;
        }

        public void OpenCraftingTable()
        {
            if (mCraftingWindow != null) mCraftingWindow.Close();
            mCraftingWindow = new CraftingWindow(GameCanvas);
            mShouldOpenCraftingTable = false;
            Globals.InCraft = true;
        }

        //Picture

        public void ShowPicture(string picture, int size, bool clickable)
        {
            if (mPictureWindow != null) mPictureWindow.Close();
            mPictureWindow = new PictureWindow(GameCanvas, picture, size, clickable);
        }

        public void HidePicture()
        {
            mPictureWindow.Close();
        }

        //Quest Log
        public void NotifyQuestsUpdated()
        {
            mShouldUpdateQuestLog = true;
        }

        //Trading
        public void NotifyOpenTrading(Guid traderId)
        {
            mShouldOpenTrading = true;
            mTradingTargetId = traderId;
        }

        public void NotifyCloseTrading()
        {
            mShouldCloseTrading = true;
        }

        public void OpenTrading()
        {
            if (mTradingWindow != null) mTradingWindow.Close();
            mTradingWindow = new TradingWindow(GameCanvas, mTradingTargetId);
            mShouldOpenTrading = false;
            Globals.InTrade = true;
        }

        public void ShowHideDebug()
        {
            if (mDebugMenu.IsVisible())
            {
                mDebugMenu.Hide();
            }
            else
            {
                mDebugMenu.Show();
            }
        }

        public void ShowAdminWindow()
        {
            if (mAdminWindow == null)
            {
                mAdminWindow = new AdminWindow(GameCanvas);
            }
            mAdminWindow.Show();
        }

        public bool AdminWindowOpen()
        {
            if (mAdminWindow != null && mAdminWindow.IsVisible()) return true;
            return false;
        }

        public void AdminWindowSelectName(string name)
        {
            mAdminWindow.SetName(name);
        }

        public void Draw()
        {
            if (Globals.Me != null && PlayerBox.MyEntity != Globals.Me)
            {
                PlayerBox.SetEntity(Globals.Me);
            }
            mEventWindow.Update();
            mChatBox.Update();
            GameMenu.Update(mShouldUpdateQuestLog);
            mShouldUpdateQuestLog = false;
            Hotbar.Update();
            mDebugMenu.Update();
            EscapeMenu.Update();
            if (PlayerBox != null)
            {
                PlayerBox.Update();
            }

            if (Globals.QuestOffers.Count > 0)
            {
                var quest = QuestBase.Get(Globals.QuestOffers[0]);
                mQuestOfferWindow.Update(quest);
            }
            else
            {
                mQuestOfferWindow.Hide();
            }

            //Admin window update
            if (mShouldOpenAdminWindow)
            {
                OpenAdminWindow();
            }

            //Shop Update
            if (mShouldOpenShop) OpenShop();
            if (mShopWindow != null && (!mShopWindow.IsVisible() || mShouldCloseShop))
            {
                PacketSender.SendCloseShop();
                Globals.GameShop = null;
                mShopWindow.Close();
                mShopWindow = null;
            }
            mShouldCloseShop = false;

            //Bank Update
            if (mShouldOpenBank) OpenBank();
            if (mBankWindow != null)
            {
                if (!mBankWindow.IsVisible() || mShouldCloseBank)
                {
                    PacketSender.SendCloseBank();
                    mBankWindow.Close();
                    mBankWindow = null;
                    Globals.InBank = false;
                }
                else
                {
                    mBankWindow.Update();
                }
            }
            mShouldCloseBank = false;

            //Bag Update
            if (mShouldOpenBag) OpenBag();
            if (mBagWindow != null)
            {
                if (!mBagWindow.IsVisible() || mShouldCloseBag)
                {
                    PacketSender.SendCloseBag();
                    mBagWindow.Close();
                    mBagWindow = null;
                    Globals.InBag = false;
                }
                else
                {
                    mBagWindow.Update();
                }
            }
            mShouldCloseBag = false;

            //Crafting station update
            if (mShouldOpenCraftingTable) OpenCraftingTable();
            if (mCraftingWindow != null)
            {
                if (!mCraftingWindow.IsVisible() || mShouldCloseCraftingTable)
                {
                    PacketSender.SendCloseCraftingTable();
                    mCraftingWindow.Close();
                    mCraftingWindow = null;
                    Globals.InCraft = false;
                }
                else
                {
                    mCraftingWindow.Update();
                }
            }
            mShouldCloseCraftingTable = false;

            //Trading update
            if (mShouldOpenTrading) OpenTrading();
            if (mTradingWindow != null)
            {
                if (mShouldCloseTrading)
                {
                    mTradingWindow.Close();
                    mTradingWindow = null;
                    Globals.InTrade = false;
                    mShouldCloseTrading = false;
                }
                else
                {
                    if (!mTradingWindow.IsVisible())
                    {
                        PacketSender.SendDeclineTrade();
                        mTradingWindow.Close();
                        mTradingWindow = null;
                        Globals.InTrade = false;
                    }
                    else
                    {
                        mTradingWindow.Update();
                    }
                }
            }
            mShouldCloseTrading = false;

            if (FocusChat)
            {
                mChatBox.Focus();
                FocusChat = false;
            }
            GameCanvas.RenderCanvas();
        }
    }
}