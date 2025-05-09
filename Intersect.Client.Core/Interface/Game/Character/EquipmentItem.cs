using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Character;

public partial class EquipmentItem
{
    public ImagePanel ContentPanel;

    private WindowControl mCharacterWindow;

    private Guid mCurrentItemId;

    private ItemProperties mItemProperties = null;

    private bool mTexLoaded;

    private int mYindex;

    public ImagePanel Pnl;

    public EquipmentItem(int index, WindowControl characterWindow)
    {
        mYindex = index;
        mCharacterWindow = characterWindow;
    }

    public void Setup()
    {
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
        Pnl.Clicked += pnl_RightClicked;

        ContentPanel = new ImagePanel(Pnl, "EquipmentIcon");
        ContentPanel.MouseInputEnabled = false;
        Pnl.SetToolTipText(Options.Instance.Equipment.Slots[mYindex]);
    }

    void pnl_RightClicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton != MouseButton.Right)
        {
            return;
        }

        if (ClientConfiguration.Instance.EnableContextMenus)
        {
            var window = Interface.GameUi.GameMenu.GetInventoryWindow();
            if (window != null)
            {
                var invSlot = Globals.Me.MyEquipment[mYindex];
                if (invSlot >= 0 && invSlot < Options.Instance.Player.MaxInventory)
                {
                    window.OpenContextMenu(invSlot);
                }
            }
        }
        else
        {
            PacketSender.SendUnequipItem(mYindex);
        }

    }

    void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        Interface.GameUi.ItemDescriptionWindow?.Hide();
    }

    void pnl_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        var item = ItemDescriptor.Get(mCurrentItemId);
        if (item == null)
        {
            return;
        }

        Interface.GameUi.ItemDescriptionWindow?.Show(item, 1, mItemProperties);
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

    public void Update(Guid currentItemId, ItemProperties itemProperties)
    {
        if (currentItemId != mCurrentItemId || !mTexLoaded)
        {
            mCurrentItemId = currentItemId;
            mItemProperties = itemProperties;
            var item = ItemDescriptor.Get(mCurrentItemId);
            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    ContentPanel.Show();
                    ContentPanel.Texture = itemTex;
                    ContentPanel.RenderColor = item.Color;
                }
                else
                {
                    ContentPanel.Hide();
                }
            }
            else
            {
                ContentPanel.Hide();
            }

            mTexLoaded = true;
        }
    }

}
