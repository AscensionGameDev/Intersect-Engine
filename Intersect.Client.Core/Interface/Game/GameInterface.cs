using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Admin;
using Intersect.Client.Interface.Game.Bag;
using Intersect.Client.Interface.Game.Bank;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.Crafting;
using Intersect.Client.Interface.Game.EntityPanel;
using Intersect.Client.Interface.Game.Hotbar;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Interface.Game.Shop;
using Intersect.Client.Interface.Game.Trades;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Enums;
using Intersect.GameObjects;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Game;


public partial class GameInterface : MutableInterface
{

    public bool FocusChat;

    public bool UnfocusChat;

    public bool ChatFocussed => mChatBox.HasFocus;

    //Public Components - For clicking/dragging
    public HotBarWindow Hotbar;

    private AdminWindow? mAdminWindow;

    private BagWindow mBagWindow;

    private BankWindow mBankWindow;

    private Chatbox mChatBox;

    private CraftingWindow? mCraftingWindow;

    private PictureWindow mPictureWindow;

    private QuestOfferWindow mQuestOfferWindow;

    private ShopWindow mShopWindow;

    private MapItemWindow mMapItemWindow;

    private SettingsWindow? _settingsWindow;

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

    private bool mCraftJournal {  get; set; }

    private TradingWindow? mTradingWindow;

    public EntityBox PlayerBox;

    public PlayerStatusWindow PlayerStatusWindow;

    private SettingsWindow GetOrCreateSettingsWindow()
    {
        _settingsWindow ??= new SettingsWindow(GameCanvas)
        {
            IsVisibleInTree = false,
        };

        return _settingsWindow;
    }

    public GameInterface(Canvas canvas) : base(canvas)
    {
        GameCanvas = canvas;

        InitGameGui();
    }

    public Canvas GameCanvas { get; }

    private AnnouncementWindow? _announcementWindow;
    private EscapeMenu? _escapeMenu;
    private SimplifiedEscapeMenu? _simplifiedEscapeMenu;
    private TargetContextMenu? _targetContextMenu;

    public EscapeMenu EscapeMenu => _escapeMenu ??= new EscapeMenu(GameCanvas, GetOrCreateSettingsWindow)
    {
        IsHidden = true,
    };

    public SimplifiedEscapeMenu SimplifiedEscapeMenu => _simplifiedEscapeMenu ??= new SimplifiedEscapeMenu(GameCanvas, GetOrCreateSettingsWindow) {IsHidden = true};

    public TargetContextMenu TargetContextMenu => _targetContextMenu ??= new TargetContextMenu(GameCanvas) {IsHidden = true};

    public AnnouncementWindow AnnouncementWindow => _announcementWindow ??= new AnnouncementWindow(GameCanvas) { IsHidden = true };

    public MenuContainer GameMenu { get; private set; }

    public void InitGameGui()
    {
        mChatBox = new Chatbox(GameCanvas, this);
        GameMenu = new MenuContainer(GameCanvas);
        Hotbar = new HotBarWindow(GameCanvas);
        PlayerBox = new EntityBox(GameCanvas, EntityType.Player, Globals.Me, true);
        PlayerBox.SetEntity(Globals.Me);
        PlayerStatusWindow = new PlayerStatusWindow(GameCanvas);
        if (mPictureWindow == null)
        {
            mPictureWindow = new PictureWindow(GameCanvas);
        }

        mQuestOfferWindow = new QuestOfferWindow(GameCanvas);
        mMapItemWindow = new MapItemWindow(GameCanvas);
        mBankWindow = new BankWindow(GameCanvas);
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
            mAdminWindow ??= new AdminWindow(GameCanvas);
            mAdminWindow.X = GameCanvas.Width - mAdminWindow.OuterWidth;
            mAdminWindow.Y = (GameCanvas.Height - mAdminWindow.OuterHeight) / 2;
        }
        else if (IsAdminWindowOpen)
        {
            mAdminWindow.Hide();
        }
        else
        {
            mAdminWindow.Show();
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
        mBankWindow.Open();
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

    public BagWindow GetBagWindow()
    {
        return mBagWindow;
    }

    public BankWindow GetBankWindow()
    {
        return mBankWindow;
    }

    //Crafting
    public void NotifyOpenCraftingTable(bool journalMode)
    {
        mShouldOpenCraftingTable = true;
        mCraftJournal = journalMode;
    }

    public void NotifyCloseCraftingTable()
    {
        mShouldCloseCraftingTable = true;
        mCraftJournal = false;
    }

    public void OpenCraftingTable()
    {
        if (mCraftingWindow != null)
        {
            mCraftingWindow.Close();
        }

        mCraftingWindow = new CraftingWindow(GameCanvas, mCraftJournal);
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
        mTradingWindow?.Close();
        mTradingWindow = new TradingWindow(GameCanvas, mTradingTarget);
        mShouldOpenTrading = false;
        Globals.InTrade = true;
    }

    public bool IsAdminWindowOpen => !mAdminWindow?.IsHidden ?? false;

    public void AdminWindowSelectName(string playerName)
    {
        if (mAdminWindow is not { } adminWindow)
        {
            return;
        }

        adminWindow.PlayerName = playerName;
    }

    public void Update(TimeSpan elapsed, TimeSpan total)
    {
        if (Globals.Me != null && PlayerBox?.MyEntity != Globals.Me)
        {
            PlayerBox?.SetEntity(Globals.Me);
        }

        GameMenu?.Update(mShouldUpdateQuestLog);
        mShouldUpdateQuestLog = false;
        Hotbar?.Update();
        EscapeMenu.Update();
        PlayerBox?.Update();
        PlayerStatusWindow?.Update();
        mMapItemWindow.Update();
        AnnouncementWindow?.Update();
        mPictureWindow?.Update();

        var questDescriptorId = Globals.QuestOffers.FirstOrDefault();
        if (questDescriptorId == default)
        {
            if (mQuestOfferWindow.IsVisible())
            {
                mQuestOfferWindow.Hide();
            }
        }
        else if (QuestBase.TryGet(questDescriptorId, out var questDescriptor))
        {
            mQuestOfferWindow.Update(questDescriptor);
        }
        else
        {
            ApplicationContext.CurrentContext.Logger.LogWarning("Failed to get quest {QuestId}", questDescriptorId);
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

        EventWindow.ShowOrUpdateDialog(GameCanvas);

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
        else if (mShouldCloseBank)
        {
            CloseBank();
        }
        else
        {
            mBankWindow.Update();
        }



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
            if (!mCraftingWindow.IsVisibleInTree || mShouldCloseCraftingTable)
            {
                CloseCraftingTable();
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
    }

    public void Draw(TimeSpan elapsed, TimeSpan total)
    {
        GameCanvas.RenderCanvas(elapsed, total);
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
        mBankWindow.Close();
        Globals.InBank = false;
        PacketSender.SendCloseBank();
        mShouldCloseBank = false;
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

        if (mCraftingWindow is { IsVisibleInTree: true, IsCrafting: false })
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

        if (TargetContextMenu.IsVisibleInTree)
        {
            TargetContextMenu.ToggleHidden();
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
