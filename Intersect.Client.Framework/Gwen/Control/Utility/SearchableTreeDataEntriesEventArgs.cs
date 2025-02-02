namespace Intersect.Client.Framework.Gwen.Control.Utility;

public class SearchableTreeDataEntriesEventArgs : EventArgs {
    public required SearchableTreeDataEntry[] Entries { get; init; }
}