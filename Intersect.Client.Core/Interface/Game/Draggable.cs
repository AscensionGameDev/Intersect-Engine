using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Interface.Game;

public partial class Draggable(Base parent, string name) : ImagePanel(parent, name)
{
    public bool IsDragging => DragAndDrop.CurrentPackage?.DrawControl == this;

    // TODO: Fix drag and drop names
    public override bool DragAndDrop_Draggable()
    {
        return true;
    }

    public override bool DragAndDrop_CanAcceptPackage(Package package)
    {
        // Important: Icon is the topmost hovered control, so it must be allowed to "receive" the drop, BUT: we forward handling to SlotItem.
        return true;
    }

    public override Package? DragAndDrop_GetPackage(int x, int y)
    {
        return new Package()
        {
            IsDraggable = true,
            DrawControl = this,
            Name = Name,
            HoldOffset = ToLocal(InputHandler.MousePosition.X, InputHandler.MousePosition.Y),
            UserData = Parent, // Expected to be SlotItem (InventoryItem/BankItem/etc)
        };
    }

    public override void DragAndDrop_StartDragging(Package package, int x, int y)
    {
        IsVisibleInParent = false;
    }

    public override void DragAndDrop_EndDragging(bool success, int x, int y)
    {
        IsVisibleInParent = true;
    }

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        // Never allow Base.DragAndDrop_HandleDrop() to run on the icon because it reparents SourceControl (Base.cs)
        // Forward drop handling to the slot (or nearest SlotItem ancestor).
        var node = Parent;
        while (node != null)
        {
            if (node is SlotItem)
            {
                return node.DragAndDrop_HandleDrop(package, x, y);
            }

            node = node.Parent;
        }

        return false;
    }
}