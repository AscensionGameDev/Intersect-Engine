using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;
// ReSharper disable MemberCanBePrivate.Global

namespace Intersect.Framework.Reflection;

/// <summary>
/// Extension methods for <see cref="Assembly"/>.
/// </summary>
public static partial class AssemblyExtensions
{
    /// <inheritdoc cref="CreateInstanceOf{TParentType}(Assembly, Func{Type, bool}, object[])"/>
    public static TParentType CreateInstanceOf<TParentType>(this Assembly assembly, params object[] args) =>
        assembly.CreateInstanceOf<TParentType>(_ => true, args);

    /// <summary>
    /// Creates an instance of <typeparamref name="TParentType"/> given <paramref name="args"/>.
    /// </summary>
    /// <typeparam name="TParentType">the type to search for subtypes and create an instance of</typeparam>
    /// <param name="assembly">the <see cref="Assembly"/> to search for subtypes in</param>
    /// <param name="predicate">a function to filter subtypes with</param>
    /// <param name="args">the arguments to create the instance with</param>
    /// <returns>an instance of <typeparamref name="TParentType"/></returns>
    /// <exception cref="InvalidOperationException">if no matching subtypes are found, or instance creation fails</exception>
    public static TParentType CreateInstanceOf<TParentType>(
        this Assembly assembly,
        Func<Type, bool> predicate,
        params object[] args
    )
    {
        var validTypes = assembly.FindDefinedSubtypesOf<TParentType>();
        var type = validTypes.FirstOrDefault(predicate);

        if (type == default)
        {
            throw new InvalidOperationException(
                string.Format(ReflectionStrings.AssemblyExtensions_NoMatchingSubtype, typeof(TParentType).FullName)
            );
        }

        if (Activator.CreateInstance(type, args) is TParentType instance)
        {
            return instance;
        }

        throw new InvalidOperationException(
            string.Format(ReflectionStrings.AssemblyExtensions_FailedToCreateInstance, typeof(TParentType).FullName)
        );
    }

    public static string GetMetadataVersion(this Assembly assembly, bool excludeCommitSha = false)
    {
        var assemblyName = assembly.GetName();
        return GetMetadataVersionFrom(assembly, assemblyName, excludeCommitSha);
    }

    public static string GetMetadataVersionName(this Assembly assembly, bool excludeCommitSha = false) =>
        $"v{GetMetadataVersion(assembly: assembly, excludeCommitSha: excludeCommitSha)}";

    private static string GetMetadataVersionFrom(Assembly assembly, AssemblyName assemblyName, bool excludeCommitSha)
    {
        var assemblyVersion = assemblyName.Version ?? new Version(0, 0, 0, 0);
        var assemblyMetadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .ToDictionary(attribute => attribute.Key, attribute => attribute.Value);

        if (assemblyMetadata.TryGetValue("BuildNumber", out var rawBuildNumber))
        {
            if (int.TryParse(rawBuildNumber, CultureInfo.InvariantCulture, out var buildNumber))
            {
                assemblyVersion = new Version(
                    assemblyVersion.Major,
                    assemblyVersion.Minor,
                    assemblyVersion.Build,
                    buildNumber
                );
            }
        }

        var metadataVersion = $"{assemblyVersion.ToString()}";
        if (assemblyMetadata.TryGetValue("VersionNameSuffix", out var versionNameSuffix))
        {
            metadataVersion = $"{metadataVersion}-{versionNameSuffix}";
        }

        if (!excludeCommitSha && assemblyMetadata.TryGetValue("CommitSha", out var commitSha))
        {
            metadataVersion = $"{metadataVersion}+{commitSha}";
        }

        return metadataVersion;
    }

    public static string GetMetadataName(this Assembly assembly, bool excludeCommitSha = false)
    {
        var assemblyName = assembly.GetName();
        var prettyName = $"{assemblyName.Name} v{GetMetadataVersionFrom(assembly, assemblyName, excludeCommitSha)}";
        return prettyName;
    }

    public static string GetVersionName(this Assembly assembly)
    {
        var version = assembly.GetName().Version;

        if (version == default)
        {
            return "???";
        }

        var versionMajorMinor = $"{version.Major}.{version.Minor}";
        var versionSuffix = version.Major == 0 ? "-beta" : string.Empty;
        return versionMajorMinor + versionSuffix;
    }

    public static IEnumerable<Type> FindAbstractSubtypesOf(this Assembly assembly, Type type) =>
        assembly.FindSubtypesOf(type).Where(subtype => subtype.IsAbstract);

    public static IEnumerable<Type> FindAbstractSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindAbstractSubtypesOf(typeof(TParentType));

    public static IEnumerable<Type> FindDefinedSubtypesOf(this Assembly assembly, Type type) => assembly
        .FindSubtypesOf(type)
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        .Where(subtype => !(subtype == null || subtype.IsAbstract || subtype.IsGenericType || subtype.IsInterface));

    public static IEnumerable<Type> FindDefinedSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindDefinedSubtypesOf(typeof(TParentType));

    public static IEnumerable<Type> FindGenericSubtypesOf(this Assembly assembly, Type type) =>
        assembly.FindSubtypesOf(type).Where(subtype => subtype.IsGenericType);

    public static IEnumerable<Type> FindGenericSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindGenericSubtypesOf(typeof(TParentType));

    public static IEnumerable<Type> FindInterfaceSubtypesOf(this Assembly assembly, Type type) =>
        assembly.FindSubtypesOf(type).Where(subtype => subtype.IsInterface);

    public static IEnumerable<Type> FindInterfaceSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindInterfaceSubtypesOf(typeof(TParentType));

    public static IEnumerable<Type> FindSubtypesOf(this Assembly assembly, Type type) =>
        assembly.GetTypes().Where(type.IsAssignableFrom);

    public static IEnumerable<Type> FindSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindGenericSubtypesOf(typeof(TParentType));

    public static IEnumerable<Type> FindValueSubtypesOf(this Assembly assembly, Type type) =>
        assembly.FindSubtypesOf(type).Where(subtype => subtype.IsValueType);

    public static IEnumerable<Type> FindValueSubtypesOf<TParentType>(this Assembly assembly) =>
        assembly.FindValueSubtypesOf(typeof(TParentType));

    public static bool TryFindResource(
        this Assembly assembly,
        string resourceName,
        [NotNullWhen(true)] out string? manifestResourceName
    )
    {
        if (assembly.IsDynamic)
        {
            manifestResourceName = default;
            return false;
        }

        manifestResourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.Contains(resourceName, StringComparison.CurrentCulture));
        return manifestResourceName != default;
    }

    public static void UnpackEmbeddedFile(this Assembly assembly, string resourceName, bool overwrite = false) =>
        UnpackEmbeddedFileAsync(
                assembly: assembly,
                cancellationToken: CancellationToken.None,
                resourceName: resourceName,
                overwrite: overwrite
            )
            .Wait();

    public static void UnpackEmbeddedFile(
        this Assembly assembly,
        string resourceName,
        FileInfo fileInfo,
        bool overwrite = false
    ) => UnpackEmbeddedFileAsync(
            assembly: assembly,
            cancellationToken: CancellationToken.None,
            resourceName: resourceName,
            fileInfo: fileInfo,
            overwrite: overwrite
        )
        .Wait();

    public static Task UnpackEmbeddedFileAsync(
        this Assembly assembly,
        CancellationToken cancellationToken,
        string resourceName,
        bool overwrite = false,
        string? unpackedName = default
    ) => UnpackEmbeddedFileAsync(
        assembly: assembly,
        cancellationToken: cancellationToken,
        resourceName: resourceName,
        fileInfo: new FileInfo(unpackedName ?? resourceName),
        overwrite: overwrite
    );

    public static async Task UnpackEmbeddedFileAsync(
        this Assembly assembly,
        CancellationToken cancellationToken,
        string resourceName,
        FileInfo fileInfo,
        bool overwrite = false
    )
    {
        if (!overwrite && fileInfo.Exists)
        {
            return;
        }

        if (!assembly.TryFindResource(resourceName, out var manifestResourceName))
        {
            throw new MissingManifestResourceException(
                string.Format(ReflectionStrings.UnpackEmbeddedFile_MissingManifestResourceInfo, resourceName)
            );
        }

        await using var manifestResourceStream = assembly.GetManifestResourceStream(manifestResourceName);
        if (manifestResourceStream == default)
        {
            throw new MissingManifestResourceException(
                string.Format(ReflectionStrings.UnpackEmbeddedFile_UnableToOpenStream, manifestResourceName)
            );
        }

        var directoryInfo = fileInfo.Directory;
        if (!(directoryInfo?.Exists ?? true))
        {
            directoryInfo.Create();
        }

        await using var fileStream = fileInfo.OpenWrite();
        await manifestResourceStream.CopyToAsync(fileStream, cancellationToken);
    }
}
