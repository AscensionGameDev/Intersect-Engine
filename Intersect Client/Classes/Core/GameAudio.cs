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
using IntersectClientExtras.Audio;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.General;
using Point = Intersect_Client.Classes.Maps.Point;

namespace Intersect_Client.Classes.Core
{
    public static class GameAudio
    {
        private static bool _isInitialized;

        public static List<string> SoundFiles;
        public static List<string> MusicFiles;
        public static GameAudioSource[] SoundSources;
        public static GameAudioSource[] MusicSources;

        //Music
        private static string _queuedMusic = "";
        private static string _currentSong = "";
        private static float _fadeRate;
        private static long _fadeTimer;
        private static GameAudioInstance _myMusic;
        private static bool _musicLoop;
        private static bool _fadingOut = false;
        private static bool _queuedLoop = false;
        private static float _queuedFade = 0f;

        //Sounds
        private static List<MapSound> _gameSounds = new List<MapSound>();


        //Init
        public static void Init()
        {
            if (_isInitialized == true) { return; }
            Globals.ContentManager.LoadAudio();
            _isInitialized = true;
        }

        public static void UpdateGlobalVolume()
        {
            if (_myMusic != null)
            {
                _myMusic.SetVolume(_myMusic.GetVolume(),true);
            }
            for (int i = 0; i < _gameSounds.Count; i++)
            {
                _gameSounds[i].Update();
                if (!_gameSounds[i].Loaded)
                {
                    _gameSounds.RemoveAt(i);
                }
            }
        }

        //Update
        public static void Update()
        {
            if (_myMusic != null)
            {
                if (_fadeTimer != 0 && _fadeTimer < Globals.System.GetTimeMS())
                {
                    if (_fadingOut)
                    {
                        _myMusic.SetVolume(_myMusic.GetVolume() - 1, true);
                        if (_myMusic.GetVolume() <= 1)
                        {
                            StopMusic();
                            if (_queuedMusic != "")
                            {
                                PlayMusic(_queuedMusic, 0f, _queuedFade, _queuedLoop);
                                _queuedMusic = "";
                            }
                        }
                        else
                        {
                            _fadeTimer = Globals.System.GetTimeMS() + (long)(_fadeRate / 1000);
                        }
                    }
                    else
                    {
                        _myMusic.SetVolume(_myMusic.GetVolume() + 1, true);
                        if (_myMusic.GetVolume() < 100)
                        {
                            _fadeTimer = Globals.System.GetTimeMS() + (long)(_fadeRate / 1000);
                        }
                        else
                        {
                            _fadeTimer = 0;
                        }
                    }

                }
                else
                {
                    if (_myMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped)
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
            if (Globals.Database.MusicVolume == 0) { return; }
            filename = GameContentManager.RemoveExtension(filename);
            if (_myMusic != null)
            {
                if (fadeout == 0 || _myMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped || _myMusic.GetState() == GameAudioInstance.AudioInstanceState.Paused || _myMusic.GetVolume() == 0)
                {
                    StopMusic();
                    StartMusic(filename,fadein);
                }
                else
                {
                    //Start fadeout
                    if (_currentSong.ToLower() != filename.ToLower() || _fadingOut)
                    {
                        _fadeRate = (float)_myMusic.GetVolume() / fadeout;
                        _fadeTimer = Globals.System.GetTimeMS() + (long)(_fadeRate / 1000);
                        _fadingOut = true;
                        _queuedMusic = filename;
                        _queuedFade = fadein;
                        _queuedLoop = loop;
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
            if (Globals.Database.MusicVolume == 0) { return; }
            if (MusicFiles.IndexOf(filename.ToLower()) > -1)
            {
                _myMusic = MusicSources[MusicFiles.IndexOf(filename.ToLower())].CreateInstance();
                _currentSong = filename;
                _myMusic.Play();
                _myMusic.SetVolume(0, true);
                _fadeRate = (float)100 / fadein;
                _fadeTimer = Globals.System.GetTimeMS() + (long)(_fadeRate / 1000) + 1;
                _fadingOut = false;
            }
        }
        public static void StopMusic(float fadeout = 0f)
        {
            if (_myMusic != null)
            {
                if (fadeout == 0 || _myMusic.GetState() == GameAudioInstance.AudioInstanceState.Stopped || _myMusic.GetState() == GameAudioInstance.AudioInstanceState.Paused || _myMusic.GetVolume() == 0)
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
                    _fadeRate = (float)_myMusic.GetVolume() / fadeout;
                    _fadeTimer = Globals.System.GetTimeMS() + (long)(_fadeRate / 1000);
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
        private GameAudioInstance _sound;
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
            _filename = GameContentManager.RemoveExtension(filename).ToLower();
            _x = x;
            _y = y;
            _map = map;
            _loop = loop;
            _distance = distance;
            if (GameAudio.SoundFiles.IndexOf(_filename) > -1 && Globals.Database.SoundVolume > 0)
            {
                _sound = GameAudio.SoundSources[GameAudio.SoundFiles.IndexOf(_filename)].CreateInstance();
                Globals.System.Log("Adding map sound: " + _filename);
                _sound.SetLoop(_loop);
                _sound.Play();
                Loaded = true;
            }
        }

        public void Update()
        {
            if (Loaded)
            {
                if (!_loop && _sound.GetState() == GameAudioInstance.AudioInstanceState.Stopped)
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
                _sound.SetVolume(100);
            }
            else
            {
                if (_distance > 0)
                {
                    float volume = 100 - ((100 / _distance) * CalculateSoundDistance());
                    if (volume < 0) {volume = 0f;}
                    _sound.SetVolume((int)volume);
                }
                else
                {
                    _sound.SetVolume(0);
                }
            }
            
        }

        private float CalculateSoundDistance()
        {
            float distance = 0f;
            float playerx = Globals.Me.GetCenterPos().X;
            float playery = Globals.Me.GetCenterPos().Y;
            float soundx = 0;
            float soundy = 0;
            int mapNum = Globals.LocalMaps[_localMap];
            if (Globals.GameMaps.ContainsKey(mapNum))
            {
                if (_x == -1 || _y == -1 || _distance == -1)
                {
                    Point player = new Point();
                    player.X = (int)playerx;
                    player.Y = (int)playery;
                    Rectangle mapRect = new Rectangle((int)Globals.GameMaps[mapNum].GetX(), (int)Globals.GameMaps[mapNum].GetY(), Globals.Database.MapWidth * Globals.Database.TileWidth, Globals.Database.MapHeight * Globals.Database.TileHeight);
                    distance = (float)DistancePointToRectangle(player, mapRect) / 32f;
                }
                else
                {
                    soundx = Globals.GameMaps[mapNum].GetX() + _x * Globals.Database.TileWidth + 16;
                    soundy = Globals.GameMaps[mapNum].GetY() + _y * Globals.Database.TileHeight + 16;
                    distance = (float)Math.Sqrt(Math.Pow(playerx - soundx, 2) + Math.Pow(playery - soundy, 2)) / 32f;
                }
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
            _sound.Dispose();
            Loaded = false;
        }
    }
}
