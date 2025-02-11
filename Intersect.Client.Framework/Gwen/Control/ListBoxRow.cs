using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     List box row (selectable).
/// </summary>
public partial class ListBoxRow : TableRow
{

    private bool mSelected;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ListBoxRow" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="columns"></param>
    /// <param name="columnWidths"></param>
    public ListBoxRow(Base parent, int columns, int[]? columnWidths = null) : base(
        parent: parent,
        columnCount: columns,
        columnWidths: columnWidths
    )
    {
        MouseInputEnabled = true;
        IsSelected = false;
    }

    /// <summary>
    ///     Indicates whether the control is selected.
    /// </summary>
    public bool IsSelected
    {
        get => mSelected;
        set => mSelected = value;
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawListBoxLine(this, IsSelected, EvenRow);
    }

    protected override void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseDown(mouseButton, mousePosition, userAction);
        OnRowSelected();
    }

}
