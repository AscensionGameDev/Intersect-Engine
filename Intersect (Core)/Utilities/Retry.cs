using System;
using System.Collections.Generic;
using System.Threading;

using Intersect.Logging;

namespace Intersect.Utilities
{

    public static class Retry
    {

        public delegate bool TryParameterlessAction<TResult>(out TResult result);

        private static void DumpExceptions(List<Exception> exceptions)
        {
            exceptions?.ForEach(exception => Log.Error(exception));
        }

        public static TResult Execute<TResult>(
            TryParameterlessAction<TResult> action,
            TimeSpan retryInterval,
            long retryCount = -1,
            bool suppressExceptions = true,
            int consecutiveFailuresUntilAbort = 10
        )
        {
            if (retryInterval.TotalMilliseconds < 1000)
            {
                throw new InvalidOperationException("You should not use this class for small intervals.");
            }

            if (retryInterval.TotalMilliseconds < 2000)
            {
                Log.Warn("You should probably not be using Retry if you need such short intervals.");
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var result = default(TResult);

            var caughtExceptions = new List<Exception>();
            var consecutiveExceptions = 0;
            var retriesRemaining = retryCount;
            while (retriesRemaining == -1 || 0 < retriesRemaining)
            {
                try
                {
                    Thread.Sleep(retryInterval);

                    if (action(out result))
                    {
                        return result;
                    }

                    consecutiveExceptions = 0;
                    caughtExceptions.Clear();
                }
                catch (Exception exception)
                {
                    caughtExceptions.Add(exception);
                    if (++consecutiveExceptions >= consecutiveFailuresUntilAbort)
                    {
                        DumpExceptions(caughtExceptions);

                        throw new OperationCanceledException("Failed too many times consecutively, aborting.");
                    }
                }

                if (--retriesRemaining == 0)
                {
                    break;
                }
            }

            if (caughtExceptions.Count <= 0 || suppressExceptions)
            {
                return result;
            }

            DumpExceptions(caughtExceptions);

            throw caughtExceptions.FindLast(e => true) ?? new InvalidOperationException("Bad Retry state!");
        }

        public static TResult Execute<TResult>(
            TryParameterlessAction<TResult> action,
            long retryIntervalMs,
            long retryCount = -1,
            bool suppressExceptions = true,
            int consecutiveFailuresUntilAbort = 10
        )
        {
            return Execute(
                action, new TimeSpan(retryIntervalMs * TimeSpan.TicksPerMillisecond), retryCount, suppressExceptions,
                consecutiveFailuresUntilAbort
            );
        }

    }

}
