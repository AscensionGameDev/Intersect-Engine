﻿using System.Collections;
using System.Runtime.CompilerServices;

namespace Intersect.Comparison;

public class NullableComparer : IComparer
{
    public NullComparison NullComparison { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool IsNull(object? @object) => @object == null;

    public int Compare(object? x, object? y)
    {
        var comparableX = x as IComparable;
        var comparableY = y as IComparable;

        var xIsNull = IsNull(comparableX);
        var yIsNull = IsNull(comparableY);

        if (!xIsNull && !yIsNull)
        {
            return comparableX!.CompareTo(comparableY!);
        }

        if (xIsNull && yIsNull)
        {
            return 0;
        }

        return NullComparison switch
        {
            NullComparison.NullFirst => xIsNull ? -1 : 1,
            NullComparison.NullLast => xIsNull ? 1 : -1,
            NullComparison.NullEqual => 0,
            _ => throw new NotImplementedException(),
        };
    }
}
