using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Bag;


public partial class BagItem
{

    private static int sItemXPadding = 4;

    private static int sItemYPadding = 4;

    public ImagePanel Container;

    public bool IsDragging;

    //Drag/Drop References
    private BagWindow mBagWindow;

    //Dragging
    private bool mCanDrag;

    private long mClickTime;

    private Guid mCurrentItemId;

    private ItemDescriptionWindow mDescWindow;

    private Draggable mDragIcon;

    //Mouse Event Variables
    private bool mMouseOver;

    private int mMouseX = -1;

    private int mMouseY = -1;

    //Slot info
    private int mMySlot;

    public ImagePanel Pnl;

    public BagItem(BagWindow bagWindow, int index)
    {
        mBagWindow = bagWindow;
        mMySlot = index;
    }

    public void Setup()
    {
        Pnl = new ImagePanel(Container, "BagItemIcon");
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
        Pnl.DoubleClicked += Pnl_DoubleClicked;
        Pnl.Clicked += pnl_Clicked;
    }

    private void Pnl_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.InBag)
        {
            Globals.Me.TryRetrieveItemFromBag(mMySlot, -1);
        }
    }

    void pnl_Clicked(Base sender, MouseButtonState arguments)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (arguments.MouseButton)
        {
            case MouseButton.Right:
                if (ClientConfiguration.Instance.EnableContextMenus)
                {
                    mBagWindow.OpenContextMenu(mMySlot);
                }
                else
                {
                    Pnl_DoubleClicked(sender, arguments);
                }
                break;

            case MouseButton.Left:
                mClickTime = Timing.Global.MillisecondsUtc + 500;
                break;
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
        mCanDrag = true;
        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            mCanDrag = false;

            return;
        }

        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }

        if (Globals.BagSlots[mMySlot]?.Descriptor != null)
        {
            mDescWindow = new ItemDescriptionWindow(
                Globals.BagSlots[mMySlot].Descriptor, Globals.BagSlots[mMySlot].Quantity, mBagWindow.X, mBagWindow.Y,
                Globals.BagSlots[mMySlot].ItemProperties
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
        if (Globals.BagSlots[mMySlot].ItemId != mCurrentItemId)
        {
            mCurrentItemId = Globals.BagSlots[mMySlot].ItemId;
            var item = ItemDescriptor.Get(Globals.BagSlots[mMySlot].ItemId);
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

        if (!IsDragging)
        {
            if (mMouseOver)
            {
                if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
                {
                    mCanDrag = true;
                    mMouseX = -1;
                    mMouseY = -1;
                    if (Timing.Global.MillisecondsUtc < mClickTime)
                    {
                        mClickTime = 0;
                    }
                }
                else
                {
                    if (mCanDrag && Draggable.Active == null)
                    {
                        if (mMouseX == -1 || mMouseY == -1)
                        {
                            mMouseX = InputHandler.MousePosition.X - Pnl.ToCanvas(new Point(0, 0)).X;
                            mMouseY = InputHandler.MousePosition.Y - Pnl.ToCanvas(new Point(0, 0)).Y;
                        }
                        else
                        {
                            var xdiff = mMouseX -
                                        (InputHandler.MousePosition.X - Pnl.ToCanvas(new Point(0, 0)).X);

                            var ydiff = mMouseY -
                                        (InputHandler.MousePosition.Y - Pnl.ToCanvas(new Point(0, 0)).Y);

                            if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                            {
                                IsDragging = true;
                                mDragIcon = new Draggable(
                                    Pnl.ToCanvas(new Point(0, 0)).X + mMouseX,
                                    Pnl.ToCanvas(new Point(0, 0)).X + mMouseY, Pnl.Texture, Pnl.RenderColor
                                );
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (mDragIcon.Update())
            {
                //Drug the item and now we stopped
                IsDragging = false;
                var dragRect = new FloatRect(
                    mDragIcon.X - sItemXPadding / 2, mDragIcon.Y - sItemYPadding / 2, sItemXPadding / 2 + 32,
                    sItemYPadding / 2 + 32
                );

                float bestIntersect = 0;
                var bestIntersectIndex = -1;

                //So we picked up an item and then dropped it. Lets see where we dropped it to.
                //Check inventory first.
                if (mBagWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    for (var i = 0; i < Globals.BagSlots.Length; i++)
                    {
                        if (mBagWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                        {
                            if (FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Width *
                                FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Height >
                                bestIntersect)
                            {
                                bestIntersect =
                                    FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Height;

                                bestIntersectIndex = i;
                            }
                        }
                    }

                    if (bestIntersectIndex > -1)
                    {
                        if (mMySlot != bestIntersectIndex)
                        {
                            //Try to swap....
                            PacketSender.SendMoveBagItems(mMySlot, bestIntersectIndex);
                        }
                    }
                }
                else
                {
                    var invWindow = Interface.GameUi.GameMenu.GetInventoryWindow();

                    if (invWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.Instance.Player.MaxInventory; i++)
                        {
                            if (invWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.TryRetrieveItemFromBag(mMySlot, bestIntersectIndex);
                        }
                    }
                }

                mDragIcon.Dispose();
            }
        }
    }

}
