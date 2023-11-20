using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Intersect.Framework.Converters.Json;
using MessagePack;
using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;
using SystemJsonConverter = System.Text.Json.Serialization.JsonConverterAttribute;

namespace Intersect.Framework;

/// <summary>
/// Represents a type-bound unique identifier.
/// </summary>
/// <param name="Guid">the generic unique identifier value</param>
/// <typeparam name="T">the type this Id is for</typeparam>
[DataContract]
[MessagePackObject]
[NewtonsoftJsonConverter(typeof(IdNewtonsoftJsonConverter))]
[SystemJsonConverter(typeof(IdSystemJsonConverterFactory))]
[Serializable]
public record struct Id<T>([property: Key(0)] Guid Guid)
{
    /// <summary>
    /// Empty ID with zeroed internal <see cref="System.Guid"/>.
    /// </summary>
    public static readonly Id<T> None = default;

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
