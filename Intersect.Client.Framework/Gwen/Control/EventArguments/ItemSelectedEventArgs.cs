namespace Intersect.Client.Framework.Gwen.Control.EventArguments;


public partial class ItemSelectedEventArgs : EventArgs
{

    internal ItemSelectedEventArgs(Base selectedItem, bool automated = false, object? selectedUserData = null)
    {
        SelectedItem = selectedItem;
        Automated = automated;
        SelectedUserData = selectedUserData;
    }

    public Base SelectedItem { get; private set; }

    public bool Automated { get; private set; }

    public object? SelectedUserData { get; init; }

}
