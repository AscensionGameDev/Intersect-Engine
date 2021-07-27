using System;

using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Manages game audio for the client instance.
    /// </summary>
    public interface IGameAudioManager
    {
        IMapSound AddMapSound(string filename, int x, int y, Guid mapId, bool loop, int loopInterval, int distance, IEntity parent = null);

        ISound AddGameSound(string filename, bool loop);

        void StopSound(ISound sound);

        void StopSound(IMapSound sound);

        void StopAllSounds();

        void PlayMusic(string filename, float fadeout = 0f, float fadein = 0f, bool loop = false);

        void StopMusic(float fadeout = 0f);
    }
}
