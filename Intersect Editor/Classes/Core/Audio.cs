/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
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
