using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.General;

using System;

namespace Intersect.Client.Core.Sounds
{

    public class Sound
    {

        public bool Loaded;

        protected string mFilename;

        protected bool mLoop;

        protected IAudioInstance mSound;

        protected float mVolume;

        private static IGameContext GameContext { get; set; }

        public Sound(IGameContext gameContext, string filename, bool loop)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return;
            }

            mFilename = GameContentManager.RemoveExtension(filename).ToLower();
            mLoop = loop;
            var sound = GameContext.ContentManager.LoadSound(mFilename);
            if (sound != null)
            {
                mSound = sound.CreateInstance();
                mSound.IsLooping = mLoop;
                mSound.Volume = GameContext.Storage.Preferences.SoundVolume;
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

            if (mLoop || mSound?.State != AudioState.Stopped)
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
