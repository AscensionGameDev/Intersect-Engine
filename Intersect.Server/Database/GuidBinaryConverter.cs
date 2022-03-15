using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intersect.Server.Database;
internal class GuidBinaryConverter : ValueConverter<Guid, byte[]>
{
    public GuidBinaryConverter() : base(
        guid => guid.ToByteArray(),
        bytes => new Guid(bytes)
    ) { }
}
