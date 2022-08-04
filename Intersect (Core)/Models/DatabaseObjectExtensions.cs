namespace Intersect.Models;

public static class DatabaseObjectExtensions
{
    public static bool Matches(this IDatabaseObject databaseObject, string? searchQuery, SearchFlags searchFlags = SearchFlags.Name)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            return true;
        }

        if (searchFlags.HasFlag(SearchFlags.Name))
        {
            if (databaseObject.Name?.Contains(searchQuery) ?? false)
            {
                return true;
            }
        }

        if (searchFlags.HasFlag(SearchFlags.Folder) && databaseObject is IFolderable folderableObject)
        {
            if (folderableObject.Folder?.Contains(searchQuery) ?? false)
            {
                return true;
            }
        }

        return false;
    }
}

