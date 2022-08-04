using System.Runtime.CompilerServices;

namespace Intersect.Comparison;

public sealed class NullHandlingStringComparer : NullableComparer<string>
{
    public EmptyStringComparison EmptyStringComparison { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsNull(string nullable) =>
        nullable == null || EmptyStringComparison == EmptyStringComparison.AsNull && string.IsNullOrEmpty(nullable);
}
