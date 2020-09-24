using System;

namespace Intersect.Client.Framework.Audio
{
    public interface IAudioInstance : IDisposable
    {
        IAudioSource AudioSource { get; }

        AudioType AudioType { get; }

        AudioState State { get; }

        bool IsLooping { get; set; }

        int Volume { get; set; }

        void Play();

        void Pause();

        void Stop();

        void Dispose();
    }
}
