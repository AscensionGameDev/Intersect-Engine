using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics;

public record AtlasReference(
    string Name,
    Rectangle Bounds,
    bool IsRotated,
    Rectangle SourceBounds,
    IGameTexture Texture
)
{
    public string Name { get; } = Name.Replace('\\', '/');

    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<AtlasReference, string>>
        _atlasReferencesByFolderName = new();

    private static readonly ConcurrentDictionary<string, AtlasReference> _atlasReferences = [];

    public static void Add(AtlasReference atlasReference)
    {
        var assetName = atlasReference.Name.Replace('\\', '/');
        _atlasReferences[assetName] = atlasReference;
        _atlasReferences[assetName.ToLowerInvariant()] = atlasReference;

        var assetNameParts = assetName.Split('/');

        if (assetNameParts.Length < 3)
        {
            throw new InvalidOperationException(
                $"Expected 'resources/<folderName>/<assetFileName>' but got '{assetName}'"
            );
        }

        var folderName = assetNameParts[1];

        if (string.IsNullOrWhiteSpace(folderName))
        {
            throw new InvalidOperationException($"Invalid (empty/whitespace) segment in '{assetName}'");
        }

        var normalizedFolderName = folderName.ToLowerInvariant();

        var referencesForFolder = _atlasReferencesByFolderName.GetOrAdd(normalizedFolderName, _ => []);
        _ = referencesForFolder.AddOrUpdate(atlasReference, assetName, AssetNameFrom);
    }

    private static string AssetNameFrom(AtlasReference atlasReference, string assetName) => assetName;

    public static AtlasReference[] GetAllFor(string folderName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderName);

        var normalizedFolderName = folderName.ToLowerInvariant();
        return _atlasReferencesByFolderName.TryGetValue(normalizedFolderName, out var referencesForFolder)
            ? referencesForFolder.Keys.ToArray()
            : [];
    }

    public static bool TryGet(string assetName, [NotNullWhen(true)] out AtlasReference? atlasReference)
    {
        return _atlasReferences.TryGetValue(assetName, out atlasReference) ||
               _atlasReferences.TryGetValue(assetName.ToLowerInvariant(), out atlasReference);
    }
}