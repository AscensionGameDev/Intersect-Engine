using Intersect.Framework;

namespace Intersect.Models;

public interface IFolderable
{
    Id<Folder>? ParentId { get; set; }

    Folder? Parent { get; set; }

    bool Matches(Guid guid, bool matchParent = false, bool matchChildren = false);

    bool Matches(string @string, StringComparison stringComparison, bool matchParent = false, bool matchChildren = false);
}
