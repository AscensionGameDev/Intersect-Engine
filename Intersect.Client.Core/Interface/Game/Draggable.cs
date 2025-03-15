using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Interface.Game;

public partial class Draggable(Base parent, string name) : ImagePanel(parent, name)
{
    public bool IsDragging => DragAndDrop.CurrentPackage?.DrawControl == this;

    public override bool DragAndDrop_Draggable()
    {
        return true;
    }

    public override Package DragAndDrop_GetPackage(int x, int y)
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
    }
}