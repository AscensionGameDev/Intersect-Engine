using Intersect.Framework;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intersect.Server.Database;

internal class IdGuidConverter<T> : ValueConverter<Id<T>, Guid>
{
    public IdGuidConverter() : base(
        id => id.Guid,
        guid => new Id<T>(guid)
    )
    { }
}
