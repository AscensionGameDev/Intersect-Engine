using System.Collections.Generic;
using System.Threading;

namespace Intersect.Utilities
{

    public static class CSharpFeatures
    {

        public delegate IDictionary<TKey, TValue> CreateConcurrentDictionaryAction<TKey, TValue>(int capacity = 0);

        public delegate void ThreadYieldAction();

        public static IDictionary<TKey, TValue> CreateConcurrentDictionary<TKey, TValue>(
            CreateConcurrentDictionaryAction<TKey, TValue> createConcurrentDictionaryAction,
            int capacity = 0
        )
        {
            return createConcurrentDictionaryAction == null
                ? new Dictionary<TKey, TValue>()
                : createConcurrentDictionaryAction(capacity);
        }

        public static void ThreadYield(ThreadYieldAction threadYieldAction)
        {
            if (threadYieldAction == null)
            {
                Thread.Sleep(0);

                return;
            }

            threadYieldAction();
        }

    }

}
