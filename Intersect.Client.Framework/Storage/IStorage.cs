using System.IO;

using Intersect.Client.Framework.Content;
using Intersect.Configuration;

namespace Intersect.Client.Framework.Storage
{
    public interface IStorage
    {
        IClientConfiguration Configuration { get; }

        IPreferences Preferences { get; }

        Stream OpenContentRead(ContentType contentType, string contentName);

        Stream OpenEmbeddedResourceRead(string embeddedResourceName);

        Stream OpenFileRead(string filePath);

        Stream OpenFileWrite(string filePath, bool createIfMissing = true);
    }
}
