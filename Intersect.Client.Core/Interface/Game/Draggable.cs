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
        };
    }

    public override void DragAndDrop_StartDragging(Package package, int x, int y)
    {
        IsVisibleInParent = false;
    }

    public override void DragAndDrop_EndDragging(bool success, int x, int y)
    {
        IsVisibleInParent = true;

        // The slot drop handlers (inventory, bank, spells, ...) run on the slot the item was dragged FROM and find the
        // target themselves. They are only reached when the drag system's "hovered" control happens to be that slot,
        // which is timing dependent (it depends on mouse focus and on tooltips popping up under the cursor, and it
        // differs between builds). If the drop was not handled and the source slot was not given the drop, hand it to
        // the source slot explicitly so the result no longer depends on that.
        if (!success &&
            Parent is SlotItem sourceSlot &&
            !ReferenceEquals(DragAndDrop.HoveredControl, sourceSlot) &&
            DragAndDrop.CurrentPackage is { } package)
        {
            _ = sourceSlot.DragAndDrop_HandleDrop(package, x, y);
        }
    }
}