using System.Runtime.CompilerServices;

namespace Intersect.Framework.Threading;

public sealed partial class ThreadQueue : ActionQueue<ThreadQueue, ManualResetEventSlim>
{
    public static readonly ThreadQueue Default = new();

    private readonly object _lock = new();
    private readonly Stack<ManualResetEventSlim> _resetEventPool = [];
    private readonly int? _spinCount;

    private int _mainThreadId;

    public ThreadQueue(int? spinCount = null) : base(beginInvokePending: BeginInvokePending, endInvokePending: null)
    {
        _spinCount = spinCount;

        SetMainThreadId();
    }

    public ThreadQueue(ThreadQueue parent) : base(beginInvokePending: BeginInvokePending, endInvokePending: null)
    {
        _spinCount = parent._spinCount;
        _mainThreadId = parent._mainThreadId;
    }

    protected override bool IsActive => IsOnMainThread;

    public bool IsOnMainThread
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _mainThreadId == Environment.CurrentManagedThreadId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BeginInvokePending(ThreadQueue @this) => @this.ThrowIfNotOnMainThread();

    protected override ManualResetEventSlim EnqueueCreateState() => ResetEventPoolPop();

    protected override void EnqueueSuccessful(ManualResetEventSlim resetEvent) => resetEvent.Wait();

    protected override void EnqueueFinally(ManualResetEventSlim resetEvent) => ResetEventPoolPush(resetEvent);

    protected override Action<ManualResetEventSlim> PostInvocationAction { get; } = static resetEvent => resetEvent.Set();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunOnMainThread(Action action) => Enqueue(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunOnMainThread<TState>(Action<TState> action, TState state) => Enqueue(action, state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn RunOnMainThread<TState, TReturn>(Func<TState, TReturn> func, TState state) => EnqueueReturn(func, state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunOnMainThread<TState0, TState1>(Action<TState0, TState1> action, TState0 state0, TState1 state1) =>
        Enqueue(action, state0, state1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn RunOnMainThread<TState0, TState1, TReturn>(Func<TState0, TState1, TReturn> func, TState0 state0, TState1 state1) =>
        EnqueueReturn(func, state0, state1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunOnMainThread<TState0, TState1, TState2>(
        Action<TState0, TState1, TState2> action,
        TState0 state0,
        TState1 state1,
        TState2 state2
    ) => Enqueue(action, state0, state1, state2);

    private ManualResetEventSlim ResetEventPoolPop()
    {
        lock (_resetEventPool)
        {
            if (_resetEventPool.TryPop(out var resetEvent))
            {
                return resetEvent;
            }
        }

        return _spinCount.HasValue
            ? new ManualResetEventSlim(false, _spinCount.Value)
            : new ManualResetEventSlim();
    }

    private void ResetEventPoolPush(ManualResetEventSlim resetEvent)
    {
        resetEvent.Reset();
        lock (_resetEventPool)
        {
            _resetEventPool.Push(resetEvent);
        }
    }

    public void SetMainThreadId(int? mainThreadId = null)
    {
        lock (_lock)
        {
            _mainThreadId = mainThreadId ?? Environment.CurrentManagedThreadId;
        }
    }

    public void SetMainThreadId(ThreadQueue other)
    {
        lock (_lock)
        {
            _mainThreadId = other._mainThreadId;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ThrowIfNotOnMainThread()
    {
        if (IsOnMainThread)
        {
            return;
        }

        throw new InvalidOperationException("Operation was not called on the main thread.");
    }
}