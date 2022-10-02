using System;
using System.IO;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Logging;

using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio
{

    public partial class MonoSoundSource : GameAudioSource
    {
        private readonly string mPath;
        private readonly string mRealPath;
        private Func<Stream> mCreateStream;

        private int mInstanceCount;

        private SoundEffect mSound;

        public MonoSoundSource(string path, string realPath, string name = default)
        {
            
            mPath = path;
            mRealPath = realPath;
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        }

        public MonoSoundSource(Func<Stream> createStream, string name = default)
        {
            mCreateStream = createStream;
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
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
                if (mCreateStream != null)
                {
                    using (var stream = mCreateStream())
                    {
                        mSound = SoundEffect.FromStream(stream);
                    }
                }
                else if (Globals.ContentManager.SoundPacks != null && Globals.ContentManager.SoundPacks.Contains(mRealPath))
                {
                    using (var stream = Globals.ContentManager.SoundPacks.GetAsset(mRealPath))
                    {
                        mSound = SoundEffect.FromStream(stream);
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(mRealPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        mSound = SoundEffect.FromStream(fileStream);
                    }  

                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Error loading '{mPath}'.");
                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        $"{Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound)} [{mPath}]",
                        new Color(0xBF, 0x0, 0x0),  Enums.ChatMessageType.Error
                    )
                );
            }

        }

    }

}
