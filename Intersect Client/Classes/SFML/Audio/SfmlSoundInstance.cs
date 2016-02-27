/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using IntersectClientExtras.Audio;
using Intersect_Client.Classes.General;
using SFML.Audio;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Audio
{
    public class SfmlSoundInstance : GameAudioInstance
    {
        private Sound _sound;
        private bool _disposed;
        private int _volume;
        public SfmlSoundInstance(GameAudioSource audio) : base(audio)
        {
            _sound = new Sound(((SfmlSoundSource)audio).GetBuffer());
            _disposed = false;
        }

        public override void Play()
        {
            _sound.Play();
        }

        public override void Pause()
        {
            _sound.Pause();
        }

        public override void Stop()
        {
            _sound.Stop();
        }

        public override void SetVolume(int volume, bool isMusic = false)
        {
            _volume = volume;
            _sound.Volume = _volume * (float)(Globals.Database.SoundVolume / 100f);
        }

        public override int GetVolume()
        {
            return _volume;
        }

        public override void SetLoop(bool val)
        {
            _sound.Loop = val;
        }

        public override AudioInstanceState GetState()
        {
            if (_disposed) return AudioInstanceState.Disposed;
            if (_sound.Status == SoundStatus.Playing) return AudioInstanceState.Playing;
            if (_sound.Status == SoundStatus.Stopped ) return AudioInstanceState.Stopped;
            if (_sound.Status == SoundStatus.Paused) return AudioInstanceState.Paused;
            return AudioInstanceState.Disposed;
        }

        public override void Dispose()
        {
            _disposed = true;
            _sound.Stop();
            _sound.Dispose();
        }
    }
}
