using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Bag;
using Intersect.Client.Interface.Game.Bank;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.Crafting;
using Intersect.Client.Interface.Game.EntityPanel;
using Intersect.Client.Interface.Game.Hotbar;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Interface.Game.Shop;
using Intersect.Client.Interface.Game.Trades;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game
{

    public class GameInterface : MutableInterface
    {

        public bool FocusChat;

        public bool UnfocusChat;

        public bool ChatFocussed => mChatBox.HasFocus;

        //Public Components - For clicking/dragging
        public HotBarWindow Hotbar;

        private AdminWindow mAdminWindow;

        private BagWindow mBagWindow;

        private BankWindow mBankWindow;

        private Chatbox mChatBox;

        private CraftingWindow mCraftingWindow;

        private DebugMenu mDebugMenu;

        private EventWindow mEventWindow;

        private PictureWindow mPictureWindow;

        private QuestOfferWindow mQuestOfferWindow;

        private ShopWindow mShopWindow;

        private MapItemWindow mMapItemWindow;

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

        private bool mShouldUpdateFriendsList;

        private bool mShouldUpdateGuildList;

        private bool mShouldHideGuildWindow;

        private string mTradingTarget;

        private TradingWindow mTradingWindow;

        public EntityBox PlayerBox;

        public GameInterface(Canvas canvas) : base(canvas)
        {
            GameCanvas = canvas;
            EscapeMenu = new EscapeMenu(GameCanvas) {IsHidden = true};
            AnnouncementWindow = new AnnouncementWindow(GameCanvas) { IsHidden = true };

            InitGameGui();
        }

        public Canvas GameCanvas { get; }

        public EscapeMenu EscapeMenu { get; }

        public AnnouncementWindow AnnouncementWindow { get; }

        public Menu GameMenu { get; private set; }

        public void InitGameGui()
        {
            mChatBox = new Chatbox(GameCanvas, this);
            GameMenu = new Menu(GameCanvas);
            Hotbar = new HotBarWindow(GameCanvas);
            PlayerBox = new EntityBox(GameCanvas, EntityTypes.Player, Globals.Me, true);
            PlayerBox.SetEntity(Globals.Me);
            if (mPictureWindow == null)
            {
                mPictureWindow = new PictureWindow(GameCanvas);
            }

            mEventWindow = new EventWindow(GameCanvas);
            mQuestOfferWindow = new QuestOfferWindow(GameCanvas);
            mDebugMenu = new DebugMenu(GameCanvas);
            mMapItemWindow = new MapItemWindow(GameCanvas);
        }

        //Chatbox
        public void SetChatboxText(string msg)
        {
            mChatBox.SetChatboxText(msg);
        }

        //Friends Window
        public void NotifyUpdateFriendsList()
        {
            mShouldUpdateFriendsList = true;
        }

        //Guild Window
        public void NotifyUpdateGuildList()
        {
            mShouldUpdateGuildList = true;
        }

        public void HideGuildWindow()
        {
            mShouldHideGuildWindow = true;
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
            mShopWindow?.Close();

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
            mBankWindow?.Close();

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
            mBagWindow?.Close();

            mBagWindow = new BagWindow(GameCanvas);
            mShouldOpenBag = false;
            Globals.InBag = true;
        }

        public BagWindow GetBag()
        {
            return mBagWindow;
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
            if (mCraftingWindow != null)
            {
                mCraftingWindow.Close();
            }

            mCraftingWindow = new CraftingWindow(GameCanvas);
            mShouldOpenCraftingTable = false;
            Globals.InCraft = true;
        }

        //Quest Log
        public void NotifyQuestsUpdated()
        {
            mShouldUpdateQuestLog = true;
        }

        //Trading
        public void NotifyOpenTrading(string traderName)
        {
            mShouldOpenTrading = true;
            mTradingTarget = traderName;
        }

        public void NotifyCloseTrading()
        {
            mShouldCloseTrading = true;
        }

        public void OpenTrading()
        {
            if (mTradingWindow != null)
            {
                mTradingWindow.Close();
            }

            mTradingWindow = new TradingWindow(GameCanvas, mTradingTarget);
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
            if (mAdminWindow != null && mAdminWindow.IsVisible())
            {
                return true;
            }

            return false;
        }

        public void AdminWindowSelectName(string name)
        {
            mAdminWindow.SetName(name);
        }

        public void Draw()
        {
            if (Globals.Me != null && PlayerBox?.MyEntity != Globals.Me)
            {
                PlayerBox?.SetEntity(Globals.Me);
            }

            mChatBox?.Update();
            GameMenu?.Update(mShouldUpdateQuestLog);
            mShouldUpdateQuestLog = false;
            Hotbar?.Update();
            mDebugMenu?.Update();
            EscapeMenu.Update();
            PlayerBox?.Update();
            mMapItemWindow.Update();
            AnnouncementWindow?.Update();
            mPictureWindow?.Update();

            if (Globals.QuestOffers.Count > 0)
            {
                var quest = QuestBase.Get(Globals.QuestOffers[0]);
                mQuestOfferWindow.Update(quest);
            }
            else
            {
                mQuestOfferWindow.Hide();
            }

            if (Globals.Picture != null)
            {
                if (mPictureWindow.Picture != Globals.Picture.Picture ||
                    mPictureWindow.Size != Globals.Picture.Size ||
                    mPictureWindow.Clickable != Globals.Picture.Clickable)
                {
                    mPictureWindow.Setup(Globals.Picture.Picture, Globals.Picture.Size, Globals.Picture.Clickable);
                }
            }
            else
            {
                if (mPictureWindow != null)
                {
                    mPictureWindow.Close();
                }
            }

            mEventWindow?.Update();

            //Admin window update
            if (mShouldOpenAdminWindow)
            {
                OpenAdminWindow();
            }

            //Shop Update
            if (mShouldOpenShop)
            {
                OpenShop();
                GameMenu.OpenInventory();
            }

            if (mShopWindow != null && (!mShopWindow.IsVisible() || mShouldCloseShop))
            {
                CloseShop();
            }

            mShouldCloseShop = false;

            //Bank Update
            if (mShouldOpenBank)
            {
                OpenBank();
                GameMenu.OpenInventory();
            }

            if (mBankWindow != null)
            {
                if (!mBankWindow.IsVisible() || mShouldCloseBank)
                {
                    CloseBank();
                }
                else
                {
                    mBankWindow.Update();
                }
            }

            mShouldCloseBank = false;

            //Bag Update
            if (mShouldOpenBag)
            {
                OpenBag();
            }

            if (mBagWindow != null)
            {
                if (!mBagWindow.IsVisible() || mShouldCloseBag)
                {
                    CloseBagWindow();
                }
                else
                {
                    mBagWindow.Update();
                }
            }

            mShouldCloseBag = false;

            //Crafting station update
            if (mShouldOpenCraftingTable)
            {
                OpenCraftingTable();
                GameMenu.OpenInventory();
            }

            if (mCraftingWindow != null)
            {
                if (!mCraftingWindow.IsVisible() || mShouldCloseCraftingTable)
                {
                    CloseCraftingTable();
                }
                else
                {
                    mCraftingWindow.Update();
                }
            }

            mShouldCloseCraftingTable = false;

            //Trading update
            if (mShouldOpenTrading)
            {
                OpenTrading();
                GameMenu.OpenInventory();
            }

            if (mTradingWindow != null)
            {
                if (mShouldCloseTrading)
                {
                    CloseTrading();
                    mShouldCloseTrading = false;
                }
                else
                {
                    if (!mTradingWindow.IsVisible())
                    {
                        CloseTrading();
                    }
                    else
                    {
                        mTradingWindow.Update();
                    }
                }
            }

            if (mShouldUpdateFriendsList)
            {
                GameMenu.UpdateFriendsList();
                mShouldUpdateFriendsList = false;
            }

            if (mShouldUpdateGuildList)
            {
                GameMenu.UpdateGuildList();
                mShouldUpdateGuildList = false;
            }

            if (mShouldHideGuildWindow)
            {
                GameMenu.HideGuildWindow();
                mShouldHideGuildWindow = false;
            }

            mShouldCloseTrading = false;

            if (FocusChat)
            {
                mChatBox.Focus();
                FocusChat = false;
            }

            if (UnfocusChat)
            {
                mChatBox.UnFocus();
                UnfocusChat = false;
            }

            GameCanvas.RenderCanvas();
        }

        private void CloseShop()
        {
            Globals.GameShop = null;
            mShopWindow?.Close();
            mShopWindow = null;
            PacketSender.SendCloseShop();
        }

        private void CloseBank()
        {
            mBankWindow?.Close();
            mBankWindow = null;
            Globals.InBank = false;
            PacketSender.SendCloseBank();
        }

        private void CloseBagWindow()
        {
            mBagWindow?.Close();
            mBagWindow = null;
            Globals.InBag = false;
            PacketSender.SendCloseBag();
        }

        private void CloseCraftingTable()
        {
            mCraftingWindow?.Close();
            mCraftingWindow = null;
            Globals.InCraft = false;
            PacketSender.SendCloseCrafting();
        }

        private void CloseTrading()
        {
            mTradingWindow?.Close();
            mTradingWindow = null;
            Globals.InTrade = false;
            PacketSender.SendDeclineTrade();
        }

        public bool CloseAllWindows()
        {
            var closedWindows = false;
            if (mBagWindow != null && mBagWindow.IsVisible())
            {
                CloseBagWindow();
                closedWindows = true;
            }

            if (mTradingWindow != null && mTradingWindow.IsVisible())
            {
                CloseTrading();
                closedWindows = true;
            }

            if (mBankWindow != null && mBankWindow.IsVisible())
            {
                CloseBank();
                closedWindows = true;
            }

            if (mCraftingWindow != null && mCraftingWindow.IsVisible())
            {
                CloseCraftingTable();
                closedWindows = true;
            }

            if (mShopWindow != null && mShopWindow.IsVisible())
            {
                CloseShop();
                closedWindows = true;
            }

            if (GameMenu != null && GameMenu.HasWindowsOpen())
            {
                GameMenu.CloseAllWindows();
                closedWindows = true;
            }

            return closedWindows;
        }

        //Dispose
        public void Dispose()
        {
            CloseBagWindow();
            CloseBank();
            CloseCraftingTable();
            CloseShop();
            CloseTrading();
            GameCanvas.Dispose();
        }

    }

}
