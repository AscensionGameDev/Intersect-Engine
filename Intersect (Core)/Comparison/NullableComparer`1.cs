using System.Runtime.CompilerServices;

namespace Intersect.Comparison;

public class NullableComparer<TNullable> : NullableComparer, IComparer<TNullable>
    where TNullable : IComparable<TNullable>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsNull(object @object) =>
        @object is TNullable nullable ? IsNull(nullable) : base.IsNull(@object);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool IsNull(TNullable? nullable) => nullable == null;

    public virtual int Compare(TNullable? x, TNullable? y) => base.Compare(x, y);
}
