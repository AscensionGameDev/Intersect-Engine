using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Intersect_Editor.Classes
{
    public static class Sounds
    {
        private static bool _isInitialized;
        public static List<string> SoundFiles;
        public static List<string> MusicFiles;
        //private static Music _myMusic;

        ////Init
        //public static void Init()
        //{
        //    if (_isInitialized == true) { return; }
        //    if (!Directory.Exists("Resources/Sounds")) { Directory.CreateDirectory("Resources/Sounds"); }
        //    var sounds = Directory.GetFiles("Resources/Sounds", "*.ogg");
        //    var wavsounds = Directory.GetFiles("Resources/Sounds", "*.wav");
        //    SoundFiles = new List<string>();
        //    for (int i = 0; i < sounds.Length; i++)
        //    {
        //        SoundFiles.Add(sounds[i].Replace("Resources/Sounds", "").ToLower());
        //    }
        //    for (int i = 0; i < wavsounds.Length; i++)
        //    {
        //        SoundFiles.Add(wavsounds[i].Replace("Resources/Sounds\\", "").ToLower());
        //    }
        //    if (!Directory.Exists("Resources/Music")) { Directory.CreateDirectory("Resources/Music"); }
        //    var music = Directory.GetFiles("Resources/Music", "*.ogg");
        //    MusicFiles = new List<string>();
        //    for (int i = 0; i < music.Length; i++)
        //    {
        //        MusicFiles.Add(music[i].Replace("Resources/Music\\", "").ToLower());
        //    }
        //    _isInitialized = true;
        //}

        ////Update
        //public static void Update()
        //{

        //}

        ////Music
        //public static void PlayMusic(string filename, bool loop = false)
        //{
        //    if (_myMusic != null)
        //    {
        //        if (_myMusic.Status == SoundStatus.Stopped || _myMusic.Status == SoundStatus.Paused || _myMusic.Volume == 0)
        //        {
        //            StopMusic();
        //            StartMusic(filename);
        //        }
        //    }
        //    else
        //    {
        //        StartMusic(filename);
        //    }
        //}

        //private static void StartMusic(string filename)
        //{
        //    if (MusicFiles.IndexOf(filename.ToLower()) > -1)
        //    {
        //        _myMusic = new Music("Resources/Music/" + filename);
        //        _myMusic.Play();
        //    }
        //}

        //public static void StopMusic()
        //{
        //    if (_myMusic != null)
        //    {
        //        if (_myMusic.Status == SoundStatus.Stopped || _myMusic.Status == SoundStatus.Paused || _myMusic.Volume == 0)
        //        {
        //            _myMusic.Stop();
        //            _myMusic.Dispose();
        //        }
        //    }
        //}
    }
}
