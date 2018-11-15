using System;
using System.IO;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Localization;
using Intersect.Client.UI.Game.Chat;
using Intersect.Logging;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoSoundSource : GameAudioSource
    {
        private ContentManager mContentManager;
        private readonly string mFilename;
        private int mInstanceCount;
        private SoundEffect mSound;

        public MonoSoundSource(string filename, ContentManager contentManager)
        {
            mFilename = filename;
            mContentManager = contentManager;
        }

        public override GameAudioInstance CreateInstance()
        {
            mInstanceCount++;
            return new MonoSoundInstance(this);
        }

        public void ReleaseEffect()
        {
            mInstanceCount--;
            if (mInstanceCount == 0)
            {
                mSound.Dispose();
                mSound = null;
            }
        }

        public SoundEffect GetEffect()
        {
            if (mSound == null)
            {
                LoadSound();
            }
            return mSound;
        }

        private void LoadSound()
        {
            using (var fileStream = new FileStream(mFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    mSound = SoundEffect.FromStream(fileStream);
                }
                catch (Exception exception)
                {
                    Log.Error($"Error loading '{mFilename}'.",exception);
                    ChatboxMsg.AddMessage(new ChatboxMsg($"{Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound)} [{mFilename}]", new Framework.GenericClasses.Color(0xBF, 0x0, 0x0)));
                }
            }
        }
    }
}