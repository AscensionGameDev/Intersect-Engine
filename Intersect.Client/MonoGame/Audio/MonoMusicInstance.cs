using System;

using Intersect.Client.General;

using JetBrains.Annotations;

using Microsoft.Xna.Framework.Media;

namespace Intersect.Client.MonoGame.Audio
{

    public class MonoMusicInstance : MonoAudioInstance<MonoMusicSource>
    {

        private readonly Song mSong;

        private bool mDisposed;

        private int mVolume;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoMusicInstance([NotNull] MonoMusicSource source) : base(source)
        {
            mSong = source.LoadSong();
        }

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

        public override void Play()
        {
            if (mSong != null)
            {
                MediaPlayer.Play(mSong);
            }
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
                MediaPlayer.Volume = mVolume * (Globals.Database.MusicVolume / 100f) / 100f;
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

        protected override void InternalLoopSet()
        {
            MediaPlayer.IsRepeating = IsLooping;
        }

        public override void Dispose()
        {
            mDisposed = true;
            try
            {
                MediaPlayer.Stop();
                mSong.Dispose();
            }
            catch
            {
                /* This is just to catch any B.S. errors that MonoGame shouldn't be throwing to us. */
            }
        }

        ~MonoMusicInstance()
        {
            mDisposed = true;

            //We don't want to call MediaPlayer.Stop() here because it's static and another song has likely started playing.
        }

    }

}
