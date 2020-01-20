using Intersect.Client.General;

using JetBrains.Annotations;

using Microsoft.Xna.Framework.Media;

using System;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoMusicInstance : MonoAudioInstance<MonoMusicSource>
    {
        private bool mDisposed;
        private readonly Song mSong;
        private int mVolume;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoMusicInstance([NotNull] MonoMusicSource source) : base(source)
        {
            mSong = source.Song;
        }

        public override void Play()
        {
            MediaPlayer.Play(mSong);
        }

        public override void Pause()
        {
            MediaPlayer.Pause();
        }

        public override void Stop()
        {
            MediaPlayer.Stop();
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            mVolume = volume;
            try
            {
                MediaPlayer.Volume = (mVolume * (Globals.Database.MusicVolume / 100f) / 100f);
            }
            catch (NullReferenceException)
            {
                // song changed while changing volume
            }
            catch (Exception)
            {
                // device not ready
            }
        }

        public override int GetVolume()
        {
            return mVolume;
        }

        protected override void InternalLoopSet() => MediaPlayer.IsRepeating = IsLooping;

        public override AudioInstanceState State
        {
            get
            {
                if (mDisposed)
                {
                    return AudioInstanceState.Disposed;
                }

                switch (MediaPlayer.State)
                {
                    case MediaState.Playing:
                        return AudioInstanceState.Playing;
                    case MediaState.Stopped:
                        return AudioInstanceState.Stopped;
                    case MediaState.Paused:
                        return AudioInstanceState.Paused;
                    default:
                        return AudioInstanceState.Disposed;
                }
            }
        }

        public override void Dispose()
        {
            mDisposed = true;
            MediaPlayer.Stop();
        }

        ~MonoMusicInstance()
        {
            Dispose();
        }
    }
}