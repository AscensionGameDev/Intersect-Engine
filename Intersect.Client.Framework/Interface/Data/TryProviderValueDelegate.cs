using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Interface.Data;

public delegate bool TryProviderValueDelegate<TValue>(
    TimeSpan elapsed,
    TimeSpan total,
    [NotNullWhen(true)] out TValue newValue
);