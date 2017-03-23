using System.IO;
using IntersectClientExtras.Audio;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Intersect_MonoGameDx.Classes.SFML.Audio
{
    public class MonoSoundSource : GameAudioSource
    {
        private ContentManager _contentManager;
        private string _filename;
        private int _instanceCount = 0;
        private SoundEffect _sound;

        public MonoSoundSource(string filename, ContentManager contentManager)
        {
            _filename = filename;
            _contentManager = contentManager;
        }

        public override GameAudioInstance CreateInstance()
        {
            _instanceCount++;
            return new MonoSoundInstance(this);
        }

        public void ReleaseEffect()
        {
            _instanceCount--;
            if (_instanceCount == 0)
            {
                _sound.Dispose();
                _sound = null;
            }
        }

        public SoundEffect GetEffect()
        {
            if (_sound == null)
            {
                LoadSound();
            }
            return _sound;
        }

        private void LoadSound()
        {
            using (var fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                _sound = SoundEffect.FromStream(fileStream);
            }
        }
    }
}