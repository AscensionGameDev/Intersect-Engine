using System;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Declaration of the audio instance API.
    /// </summary>
    public interface IAudioInstance : IDisposable
    {
        /// <summary>
        /// The <see cref="IAudioSource"/> this instance was created from.
        /// </summary>
        IAudioSource AudioSource { get; }

        /// <summary>
        /// The type of audio instance (music vs sound).
        /// </summary>
        AudioType AudioType { get; }

        /// <summary>
        /// The state of the audio instance.
        /// </summary>
        AudioState State { get; }

        /// <summary>
        /// If the instance is looping.
        /// </summary>
        bool IsLooping { get; set; }

        /// <summary>
        /// The volume (0-100) of the instance.
        /// </summary>
        int Volume { get; set; }

        /// <summary>
        /// Play or resume the audio instance.
        /// </summary>
        void Play();

        /// <summary>
        /// Pause playback of the instance.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stop playback of the instance.
        /// </summary>
        void Stop();

        /// <inheritdoc />
        void Dispose();
    }
}
