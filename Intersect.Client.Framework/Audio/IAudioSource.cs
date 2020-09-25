using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Audio
{
    /// <summary>
    /// Declaration of the audio source API.
    /// </summary>
    public interface IAudioSource : IAsset
    {
        /// <summary>
        /// The type of audio source.
        /// </summary>
        AudioType AudioType { get; }

        /// <summary>
        /// Create an audio playback instance from this source.
        /// </summary>
        /// <returns>an instance of <see cref="IAudioInstance"/></returns>
        IAudioInstance CreateInstance();
    }
}
