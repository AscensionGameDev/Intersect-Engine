#if DIAGNOSTIC
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Intersect.Logging;
#endif

namespace Intersect.Threading;

public sealed class LockHelper(string? name = default)
{
    private readonly object _lock = new();

    private bool _lockTaken;
    private DateTime _takenAt;

    public void Lock(Action work, string? debugInfo = default)
    {
        if (_lockTaken)
        {
            return;
        }

#if DIAGNOSTIC
        DateTime takenAt = default;
#endif

        try
        {
#if DIAGNOSTIC
            var waitStartedAt = DateTime.UtcNow;
#endif

            Monitor.Enter(_lock, ref _lockTaken);

#if DIAGNOSTIC
            var waitElapsed = DateTime.UtcNow - waitStartedAt;

            DebugPrint($"[{name}] Waited for {waitElapsed.TotalMilliseconds}ms to acquire the lock [{debugInfo}]");
#endif

            if (!_lockTaken)
            {
                return;
            }

#if DIAGNOSTIC
            _takenAt = takenAt = DateTime.UtcNow;
#endif

            work();
        }
        finally
        {
            if (_lockTaken)
            {
                Monitor.Exit(_lock);
                _lockTaken = false;

#if DIAGNOSTIC
                var elapsedTaken = DateTime.UtcNow - takenAt;

                DebugPrint($"[{name}] Held lock for {elapsedTaken.TotalMilliseconds}ms [{debugInfo}]");
#endif
            }
            else
            {
#pragma warning disable CA2219
                throw new InvalidOperationException(
                    $"[LockHelper][{name ?? "?"}] Failed to take lock {debugInfo ?? string.Empty}"
                );
#pragma warning restore CA2219
            }
        }
    }

    public bool TryAcquireLock(out Reference lockHelperReference) =>
        TryAcquireLock(null, out lockHelperReference);

    public bool TryAcquireLock(string? debugInfo, out Reference lockHelperReference)
    {
        bool lockTaken = false;

#if DIAGNOSTIC
        DebugPrint($"TryAcquireLock from: {debugInfo}");

        if (_lockTaken)
        {
            DebugPrint($"We will be waiting on a lock from: {debugInfo}");
        }

        var waitStartedAt = DateTime.UtcNow;
#endif

        Monitor.Enter(_lock, ref lockTaken);

#if DIAGNOSTIC
        var waitElapsed = DateTime.UtcNow - waitStartedAt;

        DebugPrint($"[{name}] Waited for {waitElapsed.TotalMilliseconds}ms to acquire the lock [{debugInfo}]");

        _takenAt = DateTime.UtcNow;
#endif

        if (lockTaken)
        {
            _lockTaken = lockTaken;
            lockHelperReference = new Reference(name, _lock, _takenAt, ref _lockTaken);
            return true;
        }

        lockHelperReference = default;
        return false;
    }

    public bool ReleaseLock(string? debugInfo = default)
    {
        if (!_lockTaken)
        {
            return false;
        }

        Monitor.Exit(_lock);
        _lockTaken = false;

#if DIAGNOSTIC
        var elapsedTaken = DateTime.UtcNow - _takenAt;

        DebugPrint($"[{name}] Held lock for {elapsedTaken.TotalMilliseconds}ms [{debugInfo}]");
#endif

        return true;
    }

#if DIAGNOSTIC
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DebugPrint(string message)
    {
        // if (LegacyLogging.Logger is { } logger)
        // {
        //     logger.Debug(message);
        // }
        // else
        // {
        //     Console.WriteLine(message);
        // }
    }
#endif

    public ref struct Reference(string name, object lockObject, DateTime takenAt, ref bool lockTaken) : IDisposable
    {
        private readonly ref bool _lockTaken = ref lockTaken;

        public void Dispose()
        {
            if (!_lockTaken)
            {
                return;
            }

            Monitor.Exit(lockObject);

#if DIAGNOSTIC
            var elapsedTaken = DateTime.UtcNow - takenAt;

            DebugPrint($"[{name}] Held lock for {elapsedTaken.TotalMilliseconds}ms [from ref]");
#endif
        }
    }
}