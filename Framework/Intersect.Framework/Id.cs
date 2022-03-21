using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using Intersect.Framework.Converters.Json;

namespace Intersect.Framework;

/// <summary>
/// Represents a type-bound unique identifier.
/// </summary>
/// <typeparam name="T">the type this Id is for</typeparam>
/// <param name="Guid">the generic unique identifier value</param>
[Serializable]
[JsonConverter(typeof(IdJsonConverterFactory))]
public record struct Id<T>(Guid Guid)
{
    /// <summary>
    /// Initializes a new <see cref="Id{T}"/> value.
    /// </summary>
    /// <returns>a new unique <see cref="Id{T}"/> for <typeparamref name="T"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Id<T> New() => new(Guid.NewGuid());

    /// <summary>
    /// Converts an <see cref="Id{T}"/> to the contained <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">the <see cref="Id{T}"/> to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Guid(Id<T> id) => id.Guid;
}
