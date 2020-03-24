using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.General;

namespace Intersect.Client.Core.Sounds
{
    public class Sound
    {
        protected GameAudioInstance mSound;
        protected string mFilename;
        protected bool mLoop;
        protected float mVolume;
        public bool Loaded;

        public Sound(string filename, bool loop)
        {
            if (String.IsNullOrEmpty(filename)) return;
            mFilename = GameContentManager.RemoveExtension(filename).ToLower();
            mLoop = loop;
            GameAudioSource sound = Globals.ContentManager.GetSound(mFilename);
            if (sound != null)
            {
                mSound = sound.CreateInstance();
                mSound.IsLooping = mLoop;
                mSound.SetVolume(Globals.Database.SoundVolume);
                mSound.Play();
                Loaded = true;
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
    }
}
