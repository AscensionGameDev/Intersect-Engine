using System;

using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Manages game audio for the client instance.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Start playing a map sound.
        /// </summary>
        /// <param name="filename">The sound file name to start playing.</param>
        /// <param name="x">The X location to start playing this sound at.</param>
        /// <param name="y">The Y location to start playing this sound at.</param>
        /// <param name="mapId">The map Id to start playing this song on.</param>
        /// <param name="loop">Determines whether the sound loops once it's over or not.</param>
        /// <param name="loopInterval">The time (in ms) between the sound stopping and starting again once it loops.</param>
        /// <param name="distance">The maximum distance from which the sound can still be heard.</param>
        /// <param name="parent">The <see cref="IEntity"/> this sound belongs to.</param>
        /// <returns>Returns a new instance of <see cref="IMapSound"/> for the sound that will be played.</returns>
        IMapSound PlayMapSound(string filename, int x, int y, Guid mapId, bool loop, int loopInterval, int distance, IEntity parent = null);

        /// <summary>
        /// Start playing a sound effect.
        /// </summary>
        /// <param name="filename">The sound file name to start playing.</param>
        /// <param name="loop">Determines whether the sound loops once it's over or not.</param>
        /// <returns>Returns a new instance of <see cref="ISound"/> for the sound that will be played.</returns>
        ISound PlaySound(string filename, bool loop);

        /// <summary>
        /// Stop a sound effect.
        /// </summary>
        /// <param name="sound">The sound to stop playing.</param>
        void StopSound(ISound sound);

        /// <summary>
        /// Stop a sound effect.
        /// </summary>
        /// <param name="sound">The sound to stop playing.</param>
        void StopSound(IMapSound sound);

        /// <summary>
        /// Stop all currently playing sound effects.
        /// </summary>
        void StopAllSounds();

        /// <summary>
        /// Play a music track, automatically fading out the old track with the configured values.
        /// </summary>
        /// <param name="filename">The song file name to start playing. A blank filename will only stop the currently playing music track.</param>
        /// <param name="fadein">The time (in ms) it should take to fade in the new music track.</param>
        /// <param name="fadeout">The time (in ms) it should take to fade out the current music track.</param>
        /// <param name="loop">Determines whether the song loops once it's over or not.</param>
        void PlayMusic(string filename, int fadeout = 0, int fadein = 0, bool loop = false);

        /// <summary>
        /// Stops the current playing music track.
        /// </summary>
        /// <param name="fadeout">The time (in ms) it should take to fade out the current music track.</param>
        void StopMusic(int fadeout = 0);
    }
}
