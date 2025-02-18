namespace Intersect.Framework.Threading;

public abstract partial class ActionQueue<TActionQueue, TEnqueueState>
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

            deferredAction.Action(deferredAction.ActionState);
            deferredAction.PostInvocationAction(deferredAction.EnqueueState);
        }

        public record struct DeferredAction(
            Action<TState> Action,
            TState ActionState,
            Action<TEnqueueState> PostInvocationAction,
            TEnqueueState EnqueueState
        );
    }
}