using System;
using System.Threading;

namespace Intersect.Threading
{

    public class LockingActionQueue
    {

        private readonly object mLockObject;

        private Action mNextAction;

        public LockingActionQueue() : this(new object())
        {
        }

        public LockingActionQueue(object lockObjectObject)
        {
            mLockObject = lockObjectObject;
        }

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

    }

}
