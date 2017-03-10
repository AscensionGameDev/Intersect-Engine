using System;
using IntersectClientExtras.Audio;
using Microsoft.Xna.Framework.Media;

namespace Intersect_MonoGameDx.Classes.SFML.Audio
{
    public class MonoMusicSource : GameAudioSource
    {
        private Song _song;
        private string _path;
        public MonoMusicSource(string path)
        {
            _path = path;
            _song = Song.FromUri(path, new Uri(path, UriKind.Relative));
        }
        public override GameAudioInstance CreateInstance()
        {
            return new MonoMusicInstance(this);
        }

        public Song GetSource()
        {
            return _song;
        }
    }
}
