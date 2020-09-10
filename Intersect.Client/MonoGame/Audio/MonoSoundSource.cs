using System;
using System.IO;

using Intersect.Client.Framework.Audio;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Logging;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Intersect.Client.MonoGame.Audio
{

    public class MonoSoundSource : GameAudioSource
    {

        private readonly string mFilename;

        private int mInstanceCount;

        private SoundEffect mSound;

        public MonoSoundSource(string filename, ContentManager contentManager)
        {
            mFilename = filename;
        }

        public SoundEffect Effect
        {
            get
            {
                if (mSound == null)
                {
                    LoadSound();
                }

                return mSound;
            }
        }

        public override GameAudioInstance CreateInstance()
        {
            mInstanceCount++;

            return new MonoSoundInstance(this);
        }

        public void ReleaseEffect()
        {
            if (--mInstanceCount > 0)
            {
                return;
            }

            mSound?.Dispose();
            mSound = null;
        }

        private void LoadSound()
        {
            try
            {
                if (Globals.ContentManager.SoundPacks != null && Globals.ContentManager.SoundPacks.Contains(mFilename))
                {
                    using (var stream = Globals.ContentManager.SoundPacks.GetAsset(mFilename))
                    {
                        mSound = SoundEffect.FromStream(stream);
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(mFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        mSound = SoundEffect.FromStream(fileStream);
                    }  
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error loading '{mFilename}'.", exception);
                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        $"{Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound)} [{mFilename}]",
                        new Color(0xBF, 0x0, 0x0)
                    )
                );
            }

        }

    }

}
