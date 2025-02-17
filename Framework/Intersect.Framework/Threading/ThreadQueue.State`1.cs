namespace Intersect.Framework.Threading;

public sealed partial class ThreadQueue
{
    private record struct State<TState>
    {
        public static readonly Queue<DeferredAction> ActionQueue = [];

        // ReSharper disable once StaticMemberInGenericType
        public static readonly Action Next = InternalNext;

        private static void InternalNext()
        {
            if (!ActionQueue.TryDequeue(out var deferredAction))
            {
                throw new InvalidOperationException("Action queue is not being properly synchronized");
            }

            deferredAction.Action(deferredAction.State);
            deferredAction.ResetEvent.Set();
        }

        public record struct DeferredAction(Action<TState> Action, ManualResetEventSlim ResetEvent, TState State);
    }
}