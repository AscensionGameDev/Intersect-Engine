using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.General;

using System;
using System.IO;

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

            mFilename = Path.GetFileNameWithoutExtension(filename);
            mLoop = loop;
            var sound = GameContext.ContentManager.Find<IAudioSource>(ContentType.Sound, mFilename);
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
