using System;
using Intersect.Localization;
using Intersect.Logging;
using IntersectClientExtras.Audio;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.UI.Game.Chat;
using Microsoft.Xna.Framework.Media;

namespace Intersect_MonoGameDx.Classes.SFML.Audio
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
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Get("errors", "loadfile", Strings.Get("words", "lcase_sound")), new Color(0xBF, 0x0, 0x0)));
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