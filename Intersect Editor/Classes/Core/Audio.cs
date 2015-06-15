using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Intersect_Editor.Classes
{
    public static class Audio
    {

        //Sound Files
        public static string[] SoundFileNames;

        //Music Files
        public static string[] MusicFileNames;

        public static void LoadAudio()
        {
            LoadSounds();
            LoadMusic();
        }

        private static void LoadSounds()
        {
            if (!Directory.Exists("Resources/Sounds")) { Directory.CreateDirectory("Resources/Sounds"); }
            List<string> items = new List<string>();
            items.AddRange(Directory.GetFiles("Resources/Sounds", "*.wav"));
            items.AddRange(Directory.GetFiles("Resources/Sounds", "*.ogg"));
            items.Sort();
            SoundFileNames = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                SoundFileNames[i] = items[i].Replace("Resources/Sounds\\", "");
                //TODO, Load Sound into SFML Sound Object for Playback
            }
        }

        private static void LoadMusic()
        {
            if (!Directory.Exists("Resources/Music")) { Directory.CreateDirectory("Resources/Music"); }
            List<string> items = new List<string>();
            items.AddRange(Directory.GetFiles("Resources/Music", "*.wav"));
            items.AddRange(Directory.GetFiles("Resources/Music", "*.ogg"));
            items.Sort();
            MusicFileNames = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                MusicFileNames[i] = items[i].Replace("Resources/Music\\", "");
                //TODO, Load Music into SFML Muisc Object for Playback
            }
        }
    }
}
