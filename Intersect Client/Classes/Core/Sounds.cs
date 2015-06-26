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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SFML.Audio;

namespace Intersect_Client.Classes
{
    public static class Sounds
    {
        private static bool _isInitialized;

        public static List<string> SoundFiles;
        public static List<string> MusicFiles;

        private static string _queuedMusic = "";
        private static string _currentSong = "";
        private static float _fadeRate;
        private static long _fadeTimer;
        private static Music _myMusic;
        private static bool _musicLoop;
        private static bool _fadingOut = false;

        //Init
        public static void Init()
        {
            if (_isInitialized == true) { return; }
            if (!Directory.Exists("Resources/Sounds")) { Directory.CreateDirectory("Resources/Sounds"); }
            var sounds = Directory.GetFiles("Resources/Sounds", "*.ogg");
            var wavsounds = Directory.GetFiles("Resources/Sounds", "*.wav");
            SoundFiles = new List<string>();
            for (int i = 0; i < sounds.Length; i++)
            {
                SoundFiles.Add(sounds[i].Replace("Resources/Sounds\\", "").ToLower());
            }
            for (int i = 0; i < wavsounds.Length; i++)
            {
                SoundFiles.Add(wavsounds[i].Replace("Resources/Sounds\\", "").ToLower());
            }
            if (!Directory.Exists("Resources/Music")) { Directory.CreateDirectory("Resources/Music"); }
            var music = Directory.GetFiles("Resources/Music", "*.ogg");
            MusicFiles = new List<string>();
            for (int i = 0; i < music.Length; i++)
            {
                MusicFiles.Add(music[i].Replace("Resources/Music\\", "").ToLower());
            }
            _isInitialized = true;
        }

        //Update
        public static void Update()
        {
            if (_myMusic != null)
            {
                if (_fadeTimer != 0 && _fadeTimer < Environment.TickCount)
                {
                    if (_fadingOut)
                    {
                        _myMusic.Volume -= 1;
                        if (_myMusic.Volume <= 1)
                        {
                            StopMusic();
                        }
                        else
                        {
                            _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                        }
                    }
                    else
                    {
                        _myMusic.Volume += 1;
                        if (_myMusic.Volume < 100)
                        {
                            _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                        }
                        else
                        {
                            _fadeTimer = 0;
                        }
                    }
                    
                }
                else
                {
                    if (_myMusic.Status == SoundStatus.Stopped)
                    {
                        if (_musicLoop) { _myMusic.Play(); }
                    }
                }
            }
        }

        //Music
        public static void PlayMusic(string filename, float fadeout= 0f, float fadein = 0f, bool loop = false)
        {
            if (Globals.MusicEnabled == false) { return; }
            if (_myMusic != null)
            {
                if (fadeout == 0 || _myMusic.Status == SoundStatus.Stopped || _myMusic.Status == SoundStatus.Paused || _myMusic.Volume == 0)
                {
                    StopMusic();
                    StartMusic(filename);
                }
                else
                {
                    //Start fadeout
                    if (_currentSong.ToLower() != filename.ToLower()) {
                        _fadeRate = (float)_myMusic.Volume / fadeout;
                        _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                        _fadingOut = true;
                        _queuedMusic = filename;
                    }
                }
            }
            else
            {
                StartMusic(filename,fadein);
            }
            _musicLoop = loop;
        }

        private static void StartMusic(string filename, float fadein = 0f)
        {
            if (Globals.MusicEnabled == false) { return; }
            if (MusicFiles.IndexOf(filename.ToLower()) > -1)
            {
                _myMusic = new Music("Resources/Music/" + filename);
                _currentSong = filename;
                _myMusic.Play();
                _myMusic.Volume = 0;
                _fadeRate = (float)100 / fadein;
                _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                _fadingOut = false;
            }
        }

        public static void StopMusic(float fadeout = 0f)
        {
            if (_myMusic != null)
            {
                if (fadeout == 0 || _myMusic.Status == SoundStatus.Stopped || _myMusic.Status == SoundStatus.Paused || _myMusic.Volume == 0)
                {
                    _currentSong = "";
                    _myMusic.Stop();
                    _myMusic.Dispose();
                    _myMusic = null;
                    _fadeTimer = 0;
                }
                else
                {
                    //Start fadeout
                    _fadeRate = (float)_myMusic.Volume / fadeout;
                    _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                    _fadingOut = true;
                }
            }
        }


    }
}
