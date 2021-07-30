using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using System;

namespace Intersect.Client.Plugins.Audio
{
    public class AudioManager : IAudioManager
    {
        public IMapSound AddMapSound(string filename, int x, int y, Guid mapId, bool loop, int loopInterval, int distance, IEntity parent = null) => Core.Audio.AddMapSound(filename, x, y, mapId, loop, loopInterval, distance, parent);
        public ISound AddGameSound(string filename, bool loop) => Core.Audio.AddGameSound(filename, loop);

        public void StopSound(ISound sound) => Core.Audio.StopSound(sound as IMapSound);

        public void StopSound(IMapSound sound) => Core.Audio.StopSound(sound);

        public void StopAllSounds() => Core.Audio.StopAllSounds();

        public void PlayMusic(string filename, float fadeout = 0f, float fadein = 0f, bool loop = false) => Core.Audio.PlayMusic(filename, fadeout, fadein, loop);

        public void StopMusic(float fadeout = 0f) => Core.Audio.StopMusic(fadeout);
    }
}
