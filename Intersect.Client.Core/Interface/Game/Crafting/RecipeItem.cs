using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Crafting;


public partial class RecipeItem
{

    public ImagePanel? Container;

    public bool IsDragging;

    //Dragging
    private bool mCanDrag;

    //References
    private CraftingWindow mCraftingWindow;

    private Draggable? mDragIcon;

    //Slot info
    CraftingRecipeIngredient mIngredient;

    //Mouse Event Variables
    private bool mMouseOver;

    private int mMouseX = -1;

    private int mMouseY = -1;

    public ImagePanel? Pnl;

    public RecipeItem(CraftingWindow craftingWindow, CraftingRecipeIngredient ingredient)
    {
        mCraftingWindow = craftingWindow;
        mIngredient = ingredient;
    }

    public void Setup(string name)
    {
        Pnl = new ImagePanel(Container, name);
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
    }

    public void LoadItem()
    {
        var item = ItemDescriptor.Get(mIngredient.ItemId);

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

    void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        mMouseOver = false;
        mMouseX = -1;
        mMouseY = -1;
        Interface.GameUi.ItemDescriptionWindow.Hide();
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

        if (mIngredient != null && ItemDescriptor.TryGet(mIngredient.ItemId, out var itemDescriptor))
        {
            Interface.GameUi.ItemDescriptionWindow.Show(itemDescriptor, mIngredient.Quantity);
        }
    }

}
