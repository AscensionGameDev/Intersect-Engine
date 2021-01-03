using System;

using Intersect.Client.General;

using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio
{

    public class MonoMusicInstance : MonoAudioInstance<MonoMusicSource>
    {

        public static MonoMusicInstance Instance = null;

        private readonly DynamicSoundEffectInstance mSong;

        private readonly MonoMusicSource mSource;

        private bool mDisposed;

        private int mVolume;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoMusicInstance(MonoMusicSource source) : base(source)
        {
            //Only allow one music player at a time
            if (Instance != null)
            {
                Instance.Stop();
                Instance.Dispose();
                Instance = null;
            }

            Instance = this;
            mSource = source;


            mSong = source.LoadSong();
        }

        public override AudioInstanceState State
        {
            get
            {
                if (mSong == null || mSong.IsDisposed)
                {
                    return AudioInstanceState.Disposed;
                }

                switch (mSong.State)
                {
                    case SoundState.Playing:
                        return AudioInstanceState.Playing;
                    case SoundState.Paused:
                        return AudioInstanceState.Paused;
                    case SoundState.Stopped:
                        return AudioInstanceState.Stopped;
                    default:
                        return AudioInstanceState.Disposed;
                }
            }
        }

        public override void Play()
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                mSong.Play();
            }
        }

        public override void Pause()
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                mSong.Pause();
            }
        }

        public override void Stop()
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                mSong.Stop();
            }
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                mVolume = volume;
                try
                {
                    mSong.Volume = mVolume * (Globals.Database.MusicVolume / 100f) / 100f;
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
        }

        public override int GetVolume()
        {
            return mVolume;
        }

        protected override void InternalLoopSet()
        {
            if (mSong != null)
            {
                //mSong.IsLooped = IsLooping;
            }
        }

        public override void Dispose()
        {
            mDisposed = true;
            try
            {
                mSong?.Stop();
                //Closing the source will lock the processing thread and then properly dispose of the song.
                mSource.Close();
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