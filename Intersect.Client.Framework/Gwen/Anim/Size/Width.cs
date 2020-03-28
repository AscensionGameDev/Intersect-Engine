using System;

namespace Intersect.Client.Framework.Gwen.Anim.Size
{

    class Width : TimedAnimation
    {

        private int mDelta;

        private bool mHide;

        private int mStartSize;

        public Width(int startSize, int endSize, float length, bool hide = false, float delay = 0.0f, float ease = 1.0f)
            : base(length, delay, ease)
        {
            mStartSize = startSize;
            mDelta = endSize - mStartSize;
            mHide = hide;
        }

        protected override void OnStart()
        {
            base.OnStart();
            mControl.Width = mStartSize;
        }

        protected override void Run(float delta)
        {
            base.Run(delta);
            mControl.Width = (int) Math.Round(mStartSize + mDelta * delta);
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            mControl.Width = mStartSize + mDelta;
            mControl.IsHidden = mHide;
        }

    }

}
