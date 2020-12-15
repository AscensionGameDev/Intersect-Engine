using JetBrains.Annotations;

namespace Intersect.Client.Framework.Content
{
    public interface IAsset
    {
        [NotNull] string Name { get; }
    }
}
