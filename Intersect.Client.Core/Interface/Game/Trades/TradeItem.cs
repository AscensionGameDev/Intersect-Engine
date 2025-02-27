using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Trades;


public partial class TradeItem
{

    private static int sItemXPadding = 4;

    private static int sItemYPadding = 4;

    public ImagePanel Container;

    private Guid mCurrentItemId;

    private ItemDescriptionWindow mDescWindow;

    private Draggable mDragIcon;

    //Mouse Event Variables
    private bool mMouseOver;

    private int mMouseX = -1;

    private int mMouseY = -1;

    private int mMySide;

    //Slot info
    private int mMySlot;

    //Drag/Drop References
    private TradingWindow mTradeWindow;

    public ImagePanel Pnl;

    public TradeItem(TradingWindow tradeWindow, int index, int side)
    {
        mTradeWindow = tradeWindow;
        mMySlot = index;
        mMySide = side;
    }

    public void Setup()
    {
        Pnl = new ImagePanel(Container, "TradeIcon");
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
        Pnl.Clicked += Pnl_RightClicked;
        Pnl.DoubleClicked += Pnl_DoubleClicked;
    }

    private void Pnl_RightClicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton != MouseButton.Right)
        {
            return;
        }

        // Are we operating our own side? if not ignore it.
        if (mMySide != 0)
        {
            return;
        }

        if (ClientConfiguration.Instance.EnableContextMenus)
        {
            mTradeWindow.OpenContextMenu(mMySide, mMySlot);
        }
        else
        {
            Pnl_DoubleClicked(sender, arguments);
        }
    }

    private void Pnl_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        // Are we operating our own side? if not ignore it.
        if (mMySide != 0)
        {
            return;
        }

        if (Globals.InTrade)
        {
            Globals.Me.TryCancelOfferToTradeItem(mMySlot);
        }
    }

    void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        mMouseOver = false;
        mMouseX = -1;
        mMouseY = -1;
        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }
    }

    void pnl_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        mMouseOver = true;

        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }

        if (ItemDescriptor.Get(Globals.Trade[mMySide, mMySlot].ItemId) != null)
        {
            mDescWindow = new ItemDescriptionWindow(
                Globals.Trade[mMySide, mMySlot].Descriptor, Globals.Trade[mMySide, mMySlot].Quantity, mTradeWindow.X,
                mTradeWindow.Y, Globals.Trade[mMySide, mMySlot].ItemProperties
            );
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = Pnl.ToCanvas(new Point(0, 0)).X,
            Y = Pnl.ToCanvas(new Point(0, 0)).Y,
            Width = Pnl.Width,
            Height = Pnl.Height
        };

        return rect;
    }

    public void Update()
    {
        var n = mMySide;
        if (Globals.Trade[n, mMySlot].ItemId != mCurrentItemId)
        {
            mCurrentItemId = Globals.Trade[n, mMySlot].ItemId;
            var item = ItemDescriptor.Get(Globals.Trade[n, mMySlot].ItemId);
            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    Pnl.Texture = itemTex;
                    Pnl.RenderColor = item.Color;
                }
                else
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }
                }
            }
            else
            {
                if (Pnl.Texture != null)
                {
                    Pnl.Texture = null;
                }
            }
        }
    }

}
