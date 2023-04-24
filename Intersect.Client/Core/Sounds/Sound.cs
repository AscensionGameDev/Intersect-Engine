using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.General;
using Intersect.Utilities;

namespace Intersect.Client.Core.Sounds
{

    public partial class Sound : ISound
    {

        public bool Loaded { get; set; }

        protected string mFilename;

        public string Filename => mFilename;

        protected bool mLoop;

        protected int mLoopInterval;

        protected GameAudioInstance mSound;

        protected float mVolume;

        private long mStoppedTime = -1;

        public Sound(string filename, bool loop, int loopInterval)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            mFilename = GameContentManager.RemoveExtension(filename).ToLower();
            mLoop = loop;
            mLoopInterval = loopInterval;
            var sound = Globals.ContentManager.GetSound(mFilename);
            if (sound != null)
            {
                mSound = sound.CreateInstance();
                mSound.IsLooping = mLoop && mLoopInterval <= 0;
               
                mSound.SetVolume(100);
                mSound.Play();
                Loaded = true;
            }
        }

        public bool Loop
        {
            get => mLoop;
            set
            {
                mLoop = value;
                if (mSound != null)
                {
                    mSound.IsLooping = mLoop;
                }
            }
        }

        public virtual bool Update()
        {
            if (!Loaded)
            {
                return false;
            }

            if (mLoop && mLoopInterval > 0 && mSound?.State == GameAudioInstance.AudioInstanceState.Stopped)
            {
                if (mStoppedTime == -1)
                {
                    mStoppedTime = Timing.Global.MillisecondsUtc;
                }
                else
                {
                    if (mStoppedTime + mLoopInterval < Timing.Global.MillisecondsUtc)
                    {
                        mSound.Play();
                        mStoppedTime = -1;
                    }
                }
                return true;
            }
            else if (mLoop || mSound?.State != GameAudioInstance.AudioInstanceState.Stopped)
            {
                return true;
            }

            Stop();

            return false;
        }

        public virtual void Stop()
        {
            if (!Loaded)
            {
                return;
            }

            mSound?.Dispose();
            Loaded = false;
        }

    }

}
