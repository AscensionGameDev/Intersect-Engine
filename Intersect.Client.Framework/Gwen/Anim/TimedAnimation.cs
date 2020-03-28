using System;

namespace Intersect.Client.Framework.Gwen.Anim
{

    // Timed animation. Provides a useful base for animations.
    public class TimedAnimation : Animation
    {

        private float mEase;

        private float mEnd;

        private bool mFinished;

        private float mStart;

        private bool mStarted;

        public TimedAnimation(float length, float delay = 0.0f, float ease = 1.0f)
        {
            mStart = Platform.Neutral.GetTimeInSeconds() + delay;
            mEnd = mStart + length;
            mEase = ease;
            mStarted = false;
            mFinished = false;
        }

        public override bool Finished => mFinished;

        protected override void Think()
        {
            //base.Think();

            if (mFinished)
            {
                return;
            }

            var current = Platform.Neutral.GetTimeInSeconds();
            var secondsIn = current - mStart;
            if (secondsIn < 0.0)
            {
                return;
            }

            if (!mStarted)
            {
                mStarted = true;
                OnStart();
            }

            var delta = secondsIn / (mEnd - mStart);
            if (delta < 0.0f)
            {
                delta = 0.0f;
            }

            if (delta > 1.0f)
            {
                delta = 1.0f;
            }

            Run((float) Math.Pow(delta, mEase));

            if (delta == 1.0f)
            {
                mFinished = true;
                OnFinish();
            }
        }

        // These are the magic functions you should be overriding

        protected virtual void OnStart()
        {
        }

        protected virtual void Run(float delta)
        {
        }

        protected virtual void OnFinish()
        {
        }

    }

}
