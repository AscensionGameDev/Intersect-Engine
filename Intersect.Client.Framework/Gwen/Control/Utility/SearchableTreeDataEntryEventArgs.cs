namespace Intersect.Client.Framework.Gwen.Control.Utility;

public class SearchableTreeDataEntryEventArgs : EventArgs {
    public required SearchableTreeDataEntry? Entry { get; init; }
}