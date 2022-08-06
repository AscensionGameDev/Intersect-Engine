using Intersect.Framework;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intersect.Server.Database;

internal class IdBinaryConverter<T> : ValueConverter<Id<T>, byte[]>
{
    public IdBinaryConverter() : base(
        id => id.Guid.ToByteArray(),
        bytes => new Id<T>(new(bytes))
    )
    { }
}
