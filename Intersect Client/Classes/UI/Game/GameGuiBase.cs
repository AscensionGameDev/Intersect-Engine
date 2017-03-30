using Intersect.GameObjects;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Game
{
    public class GameGuiBase
    {
        private AdminWindow _adminWindow;
        private BagWindow _bagWindow;
        private BankWindow _bankWindow;
        private Chatbox _chatBox;
        private CraftingBenchWindow _CraftingBenchWindow;
        private DebugMenu _debugMenu;

        private EventWindow _eventWindow;
        private EntityBox _playerBox;
        private QuestOfferWindow _questOfferWindow;
        private ShopWindow _shopWindow;
        private bool _shouldCloseBag = false;
        private bool _shouldCloseBank = false;
        private bool _shouldCloseCraftingBench = false;
        private bool _shouldCloseShop = false;
        private bool _shouldCloseTrading = false;
        private bool _shouldOpenAdminWindow = false;
        private bool _shouldOpenBag = false;
        private bool _shouldOpenBank = false;
        private bool _shouldOpenCraftingBench = false;
        private bool _shouldOpenShop = false;
        private bool _shouldOpenTrading = false;
        private bool _shouldUpdateQuestLog = true;
        private int _tradingTarget = -1;
        private TradingWindow _TradingWindow;
        public bool FocusChat;
        public Canvas GameCanvas;
        private GameMenu GameMenu;

        //Public Components - For clicking/dragging
        public HotBarWindow Hotbar;

        public GameGuiBase(Canvas myCanvas)
        {
            GameCanvas = myCanvas;
            InitGameGui();
        }

        public void InitGameGui()
        {
            _eventWindow = new EventWindow(GameCanvas);
            _chatBox = new Chatbox(GameCanvas, this);
            GameMenu = new GameMenu(GameCanvas);
            Hotbar = new HotBarWindow(GameCanvas);
            _debugMenu = new DebugMenu(GameCanvas);
            _questOfferWindow = new QuestOfferWindow(GameCanvas);
            if (Globals.Me != null)
            {
                TryAddPlayerBox();
            }
        }

        //Chatbox
        public void SetChatboxText(string msg)
        {
            _chatBox.SetChatboxText(msg);
        }

        //Friends Window
        public void UpdateFriendsList()
        {
            GameMenu.UpdateFriendsList();
        }

        //Admin Window
        public void NotifyOpenAdminWindow()
        {
            _shouldOpenAdminWindow = true;
        }

        public void OpenAdminWindow()
        {
            if (_adminWindow == null)
            {
                _adminWindow = new AdminWindow(GameCanvas);
            }
            else
            {
                if (_adminWindow.IsVisible())
                {
                    _adminWindow.Hide();
                }
                else
                {
                    _adminWindow.Show();
                }
            }
            _shouldOpenAdminWindow = false;
        }

        //Shop
        public void NotifyOpenShop()
        {
            _shouldOpenShop = true;
        }

        public void NotifyCloseShop()
        {
            _shouldCloseShop = true;
        }

        public void OpenShop()
        {
            if (_shopWindow != null) _shopWindow.Close();
            _shopWindow = new ShopWindow(GameCanvas);
            _shouldOpenShop = false;
        }

        //Bank
        public void NotifyOpenBank()
        {
            _shouldOpenBank = true;
        }

        public void NotifyCloseBank()
        {
            _shouldCloseBank = true;
        }

        public void OpenBank()
        {
            if (_bankWindow != null) _bankWindow.Close();
            _bankWindow = new BankWindow(GameCanvas);
            _shouldOpenBank = false;
            Globals.InBank = true;
        }

        //Bag
        public void NotifyOpenBag()
        {
            _shouldOpenBag = true;
        }

        public void NotifyCloseBag()
        {
            _shouldCloseBag = true;
        }

        public void OpenBag()
        {
            if (_bagWindow != null) _bagWindow.Close();
            _bagWindow = new BagWindow(GameCanvas);
            _shouldOpenBag = false;
            Globals.InBag = true;
        }

        //Crafting
        public void NotifyOpenCraftingBench()
        {
            _shouldOpenCraftingBench = true;
        }

        public void NotifyCloseCraftingBench()
        {
            _shouldCloseCraftingBench = true;
        }

        public void OpenCraftingBench()
        {
            if (_CraftingBenchWindow != null) _CraftingBenchWindow.Close();
            _CraftingBenchWindow = new CraftingBenchWindow(GameCanvas);
            _shouldOpenCraftingBench = false;
            Globals.InCraft = true;
        }

        //Quest Log
        public void NotifyQuestsUpdated()
        {
            _shouldUpdateQuestLog = true;
        }

        //Trading
        public void NotifyOpenTrading(int index)
        {
            _shouldOpenTrading = true;
            _tradingTarget = index;
        }

        public void NotifyCloseTrading()
        {
            _shouldCloseTrading = true;
        }

        public void OpenTrading()
        {
            if (_TradingWindow != null) _TradingWindow.Close();
            _TradingWindow = new TradingWindow(GameCanvas, _tradingTarget);
            _shouldOpenTrading = false;
            Globals.InTrade = true;
        }

        public void TryAddPlayerBox()
        {
            if (_playerBox != null || Globals.Me == null)
            {
                return;
            }
            _playerBox = new EntityBox(GameCanvas, Globals.Me, 4, 4);
        }

        public void ShowHideDebug()
        {
            if (_debugMenu.IsVisible())
            {
                _debugMenu.Hide();
            }
            else
            {
                _debugMenu.Show();
            }
        }

        public void ShowAdminWindow()
        {
            if (_adminWindow == null)
            {
                _adminWindow = new AdminWindow(GameCanvas);
            }
            _adminWindow.Show();
        }

        public bool AdminWindowOpen()
        {
            if (_adminWindow != null && _adminWindow.IsVisible()) return true;
            return false;
        }

        public void AdminWindowSelectName(string name)
        {
            _adminWindow.SetName(name);
        }

        public void Draw()
        {
            if (Globals.Me != null)
            {
                TryAddPlayerBox();
            }
            _eventWindow.Update();
            _chatBox.Update();
            GameMenu.Update(_shouldUpdateQuestLog);
            _shouldUpdateQuestLog = false;
            Hotbar.Update();
            _debugMenu.Update();
            if (_playerBox != null)
            {
                _playerBox.Update();
            }

            if (Globals.QuestOffers.Count > 0)
            {
                var quest = QuestBase.Lookup.Get(Globals.QuestOffers[0]);
                _questOfferWindow.Update(quest);
            }
            else
            {
                _questOfferWindow.Hide();
            }

            //Admin window update
            if (_shouldOpenAdminWindow)
            {
                OpenAdminWindow();
            }

            //Shop Update
            if (_shouldOpenShop) OpenShop();
            if (_shopWindow != null && (!_shopWindow.IsVisible() || _shouldCloseShop))
            {
                PacketSender.SendCloseShop();
                Globals.GameShop = null;
                _shopWindow.Close();
                _shopWindow = null;
            }
            _shouldCloseShop = false;

            //Bank Update
            if (_shouldOpenBank) OpenBank();
            if (_bankWindow != null)
            {
                if (!_bankWindow.IsVisible() || _shouldCloseBank)
                {
                    PacketSender.SendCloseBank();
                    _bankWindow.Close();
                    _bankWindow = null;
                    Globals.InBank = false;
                }
                else
                {
                    _bankWindow.Update();
                }
            }
            _shouldCloseBank = false;

            //Bag Update
            if (_shouldOpenBag) OpenBag();
            if (_bagWindow != null)
            {
                if (!_bagWindow.IsVisible() || _shouldCloseBag)
                {
                    PacketSender.SendCloseBag();
                    _bagWindow.Close();
                    _bagWindow = null;
                    Globals.InBag = false;
                }
                else
                {
                    _bagWindow.Update();
                }
            }
            _shouldCloseBag = false;

            //Crafting station update
            if (_shouldOpenCraftingBench) OpenCraftingBench();
            if (_CraftingBenchWindow != null)
            {
                if (!_CraftingBenchWindow.IsVisible() || _shouldCloseCraftingBench)
                {
                    PacketSender.SendCloseCraftingBench();
                    _CraftingBenchWindow.Close();
                    _CraftingBenchWindow = null;
                    Globals.InCraft = false;
                }
                else
                {
                    _CraftingBenchWindow.UpdateCraftBar();
                }
            }
            _shouldCloseCraftingBench = false;

            //Trading update
            if (_shouldOpenTrading) OpenTrading();
            if (_TradingWindow != null)
            {
                if (_shouldCloseTrading)
                {
                    _TradingWindow.Close();
                    _TradingWindow = null;
                    Globals.InTrade = false;
                    _shouldCloseTrading = false;
                }
                else
                {
                    if (!_TradingWindow.IsVisible())
                    {
                        PacketSender.SendDeclineTrade();
                        _TradingWindow.Close();
                        _TradingWindow = null;
                        Globals.InTrade = false;
                    }
                    else
                    {
                        _TradingWindow.Update();
                    }
                }
            }
            _shouldCloseTrading = false;

            if (FocusChat)
            {
                _chatBox.Focus();
                FocusChat = false;
            }
            GameCanvas.RenderCanvas();
        }
    }
}