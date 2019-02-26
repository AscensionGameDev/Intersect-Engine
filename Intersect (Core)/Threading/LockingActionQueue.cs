using JetBrains.Annotations;
using System;
using System.Threading;

namespace Intersect.Threading
{
    public class LockingActionQueue
    {
        [NotNull] private readonly object mLockObject;

        private Action mNextAction;

        [CanBeNull]
        public Action NextAction
        {
            get
            {
                lock (mLockObject)
                {
                    Monitor.Wait(mLockObject);
                    return mNextAction;
                }
            }

            set
            {
                lock (mLockObject)
                {
                    mNextAction = value;
                    Monitor.PulseAll(mLockObject);
                }
            }
        }

        public LockingActionQueue()
            : this(new object())
        {
        }

        public LockingActionQueue([NotNull] object lockObjectObject)
        {
            mLockObject = lockObjectObject;
        }
    }
}
