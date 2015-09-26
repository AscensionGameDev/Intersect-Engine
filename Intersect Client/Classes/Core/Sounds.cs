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
using System.Drawing;

namespace Intersect_Client.Classes
{
    public static class Sounds
    {
        private static bool _isInitialized;

        public static List<string> SoundFiles;
        public static List<string> MusicFiles;

        //Music
        private static string _queuedMusic = "";
        private static string _currentSong = "";
        private static float _fadeRate;
        private static long _fadeTimer;
        private static Music _myMusic;
        private static bool _musicLoop;
        private static bool _fadingOut = false;

        //Sounds
        private static List<MapSound> _gameSounds = new List<MapSound>();


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
            for (int i = 0; i < _gameSounds.Count; i++)
            {
                _gameSounds[i].Update();
                if (!_gameSounds[i].Loaded) { 
                    _gameSounds.RemoveAt(i); }
            }
        }

        //Music
        public static void PlayMusic(string filename, float fadeout = 0f, float fadein = 0f, bool loop = false)
        {
            if (Globals.MusicVolume == 0) { return; }
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
                    if (_currentSong.ToLower() != filename.ToLower())
                    {
                        _fadeRate = (float)_myMusic.Volume / fadeout;
                        _fadeTimer = Environment.TickCount + (long)(_fadeRate / 1000);
                        _fadingOut = true;
                        _queuedMusic = filename;
                    }
                }
            }
            else
            {
                StartMusic(filename, fadein);
            }
            _musicLoop = loop;
        }
        private static void StartMusic(string filename, float fadein = 0f)
        {
            if (Globals.MusicVolume == 0) { return; }
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

        //Sounds
        public static MapSound AddMapSound(string filename, int x, int y, int map, bool loop, int distance)
        {
            if (_gameSounds.Count <= 128)
            {
                MapSound newSound = new MapSound(filename, x, y, map, loop, distance);
                if (newSound != null) { _gameSounds.Add(newSound); }
                return newSound;
            }
            return null;
        }
        public static void StopSound(MapSound sound)
        {
            sound.Stop();
        }
        public static void StopAllSounds()
        {
            for (int i = 0; i < _gameSounds.Count; i++)
            {
                if (_gameSounds[i] != null)
                {
                    _gameSounds[i].Stop();
                }
            }
        }


    }

    public class MapSound
    {
        private Sound _sound;
        private SoundBuffer _soundBuffer;
        private string _filename;
        private int _x;
        private int _y;
        private int _map;
        private bool _loop;
        private int _distance;
        private float _volume;
        private int _localMap;
        public bool Loaded;

        public MapSound(string filename, int x, int y, int map, bool loop, int distance)
        {
            _filename = filename;
            _x = x;
            _y = y;
            _map = map;
            _loop = loop;
            _distance = distance;
            if (Sounds.SoundFiles.IndexOf(filename) > -1 && Globals.SoundVolume > 0)
            {
                _soundBuffer = new SoundBuffer("Resources/Sounds/" + filename);
                _sound = new Sound(_soundBuffer);
                _sound.Loop = _loop;
                _sound.Play();
                Loaded = true;
            }
        }

        public void Update()
        {
            if (Loaded)
            {
                if (!_loop && _sound.Status == SoundStatus.Stopped)
                {
                    Stop();
                }
                else
                {
                    UpdateSoundVolume();
                }
            }
        }

        private void UpdateSoundVolume()
        {
            for (int i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] > -1)
                {
                    if (Globals.LocalMaps[i] == _map)
                    {
                        _localMap = i;
                        break;
                    }
                    else
                    {
                        if (i == 8)
                        {
                            Stop();
                        }
                    }
                }
            }
            if ((_x == -1 || _y == -1 || _distance == 0) && _map == Globals.Me.CurrentMap)
            {
                _sound.Volume = 100;
            }
            else
            {
                if (_distance > 0)
                {
                    float volume = 100 - ((100 / _distance) * CalculateSoundDistance());
                    if (volume < 0) {volume = 0f;}
                    _sound.Volume = volume;
                }
                else
                {
                    _sound.Volume = 0;
                }
            }
            
        }

        private float CalculateSoundDistance()
        {
            float distance = 0f;
            int playerx = (int)Globals.Me.GetCenterPos(4).X;
            int playery = (int)Globals.Me.GetCenterPos(4).Y;
            int soundx = 0;
            int soundy = 0;
            if (_localMap != 4) {
                if (_x == -1 || _y == -1 || _distance == -1)
                {
                    Point player = new Point();
                    player.X = playerx;
                    player.Y = playery;
                    Rectangle mapRect = new Rectangle(Graphics.CalcMapOffsetX(_localMap,true), Graphics.CalcMapOffsetY(_localMap,true), Constants.MapWidth * Globals.TileWidth, Constants.MapHeight * Globals.TileHeight);
                    distance = (float)DistancePointToRectangle(player,mapRect) / 32f;
                }
                else
                {
                    soundx = Graphics.CalcMapOffsetX(_localMap, true) + _x * Globals.TileWidth + 16;
                    soundy = Graphics.CalcMapOffsetY(_localMap, true) + _y * Globals.TileHeight + 16;
                    distance = (float)Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) / 32f;
                }
            }
            else {
                soundx = Graphics.CalcMapOffsetX(4, true) + _x * Globals.TileWidth + 16;
                soundy = Graphics.CalcMapOffsetY(4, true) + _y * Globals.TileHeight + 16;
                distance = (float)Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) / 32f;
            }
            return distance;
        }

        //Code Courtesy of  Philip Peterson. -- Released under MIT license.
        //Obtained, 06/27/2015 from http://wiki.unity3d.com/index.php/Distance_from_a_point_to_a_rectangle
        public static float DistancePointToRectangle(Point point, Rectangle rect)
        {
            //  Calculate a distance between a point and a rectangle.
            //  The area around/in the rectangle is defined in terms of
            //  several regions:
            //
            //  O--x
            //  |
            //  y
            //
            //
            //        I   |    II    |  III
            //      ======+==========+======   --yMin
            //       VIII |  IX (in) |  IV
            //      ======+==========+======   --yMax
            //       VII  |    VI    |   V
            //
            //
            //  Note that the +y direction is down because of Unity's GUI coordinates.

            if (point.X < rect.X)
            { // Region I, VIII, or VII
                if (point.Y < rect.Y)
                { // I
                    point.X = point.X - rect.X;
                    point.Y = point.Y - rect.Y;
                    return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else if (point.Y > rect.Y + rect.Height)
                { // VII
                    point.X = point.X - rect.X;
                    point.Y = point.Y - (rect.Y + rect.Height);
                    return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else
                { // VIII
                    return rect.X - point.X;
                }
            }
            else if (point.X > rect.X + rect.Width)
            { // Region III, IV, or V
                if (point.Y < rect.Y)
                { // III
                    point.X = point.X - (rect.X + rect.Width);
                    point.Y = point.Y - rect.Y;
                    return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else if (point.Y > rect.Y + rect.Height)
                { // V
                    point.X = point.X - (rect.X + rect.Width);
                    point.Y = point.Y - (rect.Y + rect.Height);
                    return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
                }
                else
                { // IV
                    return point.X - rect.X + rect.Width;
                }
            }
            else
            { // Region II, IX, or VI
                if (point.Y < rect.Y)
                { // II
                    return rect.Y - point.Y;
                }
                else if (point.Y > rect.Y + rect.Height)
                { // VI
                    return point.Y - rect.Y + rect.Height;
                }
                else
                { // IX
                    return 0f;
                }
            }
        }

        public void Stop()
        {
            _soundBuffer.Dispose();
            _sound.Dispose();
            Loaded = false;
        }
    }
}
