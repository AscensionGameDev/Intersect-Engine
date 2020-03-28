using System;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.General;

namespace Intersect.Client.Core.Sounds
{

    public class Sound
    {

        public bool Loaded;

        protected string mFilename;

        protected bool mLoop;

        protected GameAudioInstance mSound;

        protected float mVolume;

        public Sound(string filename, bool loop)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return;
            }

            mFilename = GameContentManager.RemoveExtension(filename).ToLower();
            mLoop = loop;
            var sound = Globals.ContentManager.GetSound(mFilename);
            if (sound != null)
            {
                mSound = sound.CreateInstance();
                mSound.IsLooping = mLoop;
                mSound.SetVolume(Globals.Database.SoundVolume);
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

            if (mLoop || mSound?.State != GameAudioInstance.AudioInstanceState.Stopped)
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
