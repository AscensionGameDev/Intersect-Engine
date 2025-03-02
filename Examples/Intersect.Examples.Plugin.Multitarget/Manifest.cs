using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests.Types;
using Semver;

namespace Intersect.Examples.Plugin.Multitarget;

/// <summary>
///     Defines a plugin manifest in code rather than an embedded manifest.json file.
/// </summary>
public struct Manifest : IManifestHelper, IEquatable<IManifestHelper>, IEquatable<Manifest>
{
    // ReSharper disable once AssignNullToNotNullAttribute This will not be null.
    /// <inheritdoc />
    public string Name => typeof(Manifest).Namespace ?? throw new InvalidOperationException();

    // ReSharper disable once AssignNullToNotNullAttribute This will not be null.
    /// <inheritdoc />
    public string Key => typeof(Manifest).Namespace ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public SemVersion Version => new(1);

    /// <inheritdoc />
    public Authors Authors =>
        "Ascension Game Dev <admin@ascensiongamedev.com> (https://github.com/AscensionGameDev/Intersect-Engine)";

    /// <inheritdoc />
    public string Homepage => "https://github.com/AscensionGameDev/Intersect-Engine";

    public override bool Equals(object? obj)
    {
        return (obj is Manifest other && Equals(other)) ||
               (obj is IManifestHelper otherManifestHelper &&
                Equals(otherManifestHelper));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Key, Version, Authors, Homepage);
    }

    public static bool operator ==(Manifest left, Manifest right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Manifest left, Manifest right)
    {
        return !(left == right);
    }

    public bool Equals(Manifest other)
    {
        return Equals(other as IManifestHelper);
    }

    public bool Equals(IManifestHelper? other)
    {
        return other != null &&
               string.Equals(Name, other.Name, StringComparison.Ordinal) &&
               string.Equals(Key, other.Key, StringComparison.Ordinal) &&
               Version.Equals(other.Version) &&
               Authors.Equals(other.Authors as IEnumerable<Author>) &&
               string.Equals(
                   Homepage,
                   other.Homepage,
                   StringComparison.OrdinalIgnoreCase
               );
    }
}