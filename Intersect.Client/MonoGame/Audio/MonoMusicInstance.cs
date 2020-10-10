using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;
using Intersect.Client.General;

using Microsoft.Xna.Framework.Audio;

using System;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoMusicInstance : MonoAudioInstance<MonoMusicSource>
    {
        public static MonoMusicInstance Instance { get; set; } = null;

        private readonly DynamicSoundEffectInstance mSong;

        // ReSharper disable once SuggestBaseTypeForParameter
        public MonoMusicInstance(IGameContext gameContext, MonoMusicSource source) : base(gameContext, source)
        {
            //Only allow one music player at a time
            if (Instance != null)
            {
                Instance.Stop();
                Instance.Dispose();
                Instance = null;
            }

            Instance = this;

            mSong = AudioSource.LoadSong();
        }

        public override AudioState State
        {
            get
            {
                if (mSong == null || mSong.IsDisposed)
                {
                    return AudioState.Disposed;
                }

                switch (mSong.State)
                {
                    case SoundState.Playing:
                        return AudioState.Playing;

                    case SoundState.Paused:
                        return AudioState.Paused;

                    case SoundState.Stopped:
                        return AudioState.Stopped;

                    default:
                        return AudioState.Disposed;
                }
            }
        }

        public override void Play()
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                if (State == AudioState.Paused)
                {
                    mSong.Resume();
                }
                else
                {
                    mSong.Play();
                }
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

        protected override void VolumeSet()
        {
            if (mSong != null && !mSong.IsDisposed)
            {
                try
                {
                    mSong.Volume = Volume * GameContext.Storage.Preferences.MusicVolume / 10000f;
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

        protected override void IsLoopingSet()
        {
            if (mSong != null)
            {
                //mSong.IsLooped = IsLooping;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    mSong?.Stop();
                    (AudioSource as MonoMusicSource).Close();
                }
                catch
                {
                    /* This is just to catch any B.S. errors that MonoGame shouldn't be throwing to us. */
                }
            }
        }

        ~MonoMusicInstance()
        {
            Dispose(false);
        }
    }
}
