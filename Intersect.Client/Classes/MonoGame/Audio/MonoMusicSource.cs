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
        private string mPath;
        private readonly Song mSong;

        public MonoMusicSource(string path)
        {
            mPath = path;

            if (mPath == null) return;

            try
            {
                mSong = Song.FromUri(path, new Uri(path, UriKind.Relative));
            }
            catch (Exception exception)
            {
                Log.Error($"Error loading '{path}'.", exception);
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound), new Framework.GenericClasses.Color(0xBF, 0x0, 0x0)));
            }
        }

        public override GameAudioInstance CreateInstance()
        {
            return new MonoMusicInstance(this);
        }

        public Song GetSource()
        {
            return mSong;
        }
    }
}