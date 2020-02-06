using System;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Localization;
using Intersect.Client.UI.Game.Chat;
using Intersect.Logging;
using Microsoft.Xna.Framework.Media;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoMusicSource : GameAudioSource
    {
        private readonly string mPath;
        private Song mSong;

        public MonoMusicSource(string path)
        {
            mPath = path;
        }

        public override GameAudioInstance CreateInstance() => new MonoMusicInstance(this);

        private void Load()
        {
            if (string.IsNullOrWhiteSpace(mPath))
            {
                return;
            }

            try
            {
                mSong = Song.FromUri(mPath, new Uri(mPath, UriKind.Relative));
            }
            catch (Exception exception)
            {
                Log.Error($"Error loading '{mPath}'.", exception);
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound), new Color(0xBF, 0x0, 0x0)));
            }
        }

        public Song Song
        {
            get
            {
                if (mSong == null)
                {
                    Load();
                }

                return mSong;
            }
        }

    }
}