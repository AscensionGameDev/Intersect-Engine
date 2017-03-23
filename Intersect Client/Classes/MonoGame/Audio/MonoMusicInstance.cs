using System;
using IntersectClientExtras.Audio;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework.Media;

namespace Intersect_MonoGameDx.Classes.SFML.Audio
{
    public class MonoMusicInstance : GameAudioInstance
    {
        private bool _disposed;
        private Song _song;
        private int _volume;

        public MonoMusicInstance(GameAudioSource music) : base(music)
        {
            _song = ((MonoMusicSource) music).GetSource();
        }

        public override void Play()
        {
            MediaPlayer.Play(_song);
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
            _volume = volume;
            try
            {
                MediaPlayer.Volume = (_volume * (float) (Globals.Database.MusicVolume / 100f) / 100f);
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
            return _volume;
        }

        public override void SetLoop(bool val)
        {
            MediaPlayer.IsRepeating = val;
        }

        public override AudioInstanceState GetState()
        {
            if (_disposed) return AudioInstanceState.Disposed;
            if (MediaPlayer.State == MediaState.Playing) return AudioInstanceState.Playing;
            if (MediaPlayer.State == MediaState.Stopped) return AudioInstanceState.Stopped;
            if (MediaPlayer.State == MediaState.Paused) return AudioInstanceState.Paused;
            return AudioInstanceState.Disposed;
        }

        public override void Dispose()
        {
            _disposed = true;
            MediaPlayer.Stop();
        }
    }
}