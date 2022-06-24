using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using System;

namespace Intersect.Client.Plugins.Audio
{
    public partial class AudioManager : IAudioManager
    {
        /// <inheritdoc />
        public IMapSound PlayMapSound(string filename, int x, int y, Guid mapId, bool loop, int loopInterval, int distance, IEntity parent = null) => Core.Audio.AddMapSound(filename, x, y, mapId, loop, loopInterval, distance, parent);

        /// <inheritdoc />
        public ISound PlaySound(string filename, bool loop) => Core.Audio.AddGameSound(filename, loop);

        /// <inheritdoc />
        public void StopSound(ISound sound) => Core.Audio.StopSound(sound as IMapSound);

        /// <inheritdoc />
        public void StopSound(IMapSound sound) => Core.Audio.StopSound(sound);

        /// <inheritdoc />
        public void StopAllSounds() => Core.Audio.StopAllSounds();

        /// <inheritdoc />
        public void PlayMusic(string filename, int fadeout = 0, int fadein = 0, bool loop = false) => Core.Audio.PlayMusic(filename, fadeout, fadein, loop);

        /// <inheritdoc />
        public void StopMusic(int fadeout = 0) => Core.Audio.StopMusic(fadeout);
    }
}
