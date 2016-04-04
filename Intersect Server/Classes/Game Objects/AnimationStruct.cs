/*
    Intersect Game Engine (Server)
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
using System.IO;

namespace Intersect_Server.Classes
{
    public class AnimationStruct
    {
        public const string Version = "0.0.0.1";
        public string Name = "";
        public string Sound = "";

        //Lower Animation
        public string LowerAnimSprite = "";
        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount = 1;
        public Light[] LowerLights;

        //Upper Animation
        public string UpperAnimSprite = "";
        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount = 1;
        public Light[] UpperLights;

        public AnimationStruct()
        {
            LowerLights = new Light[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new Light();
            }
            UpperLights = new Light[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new Light();
            }
        }

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);

            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Animation #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);

            Name = myBuffer.ReadString();
            Sound = myBuffer.ReadString();

            //Lower Animation
            LowerAnimSprite = myBuffer.ReadString();
            LowerAnimXFrames = myBuffer.ReadInteger();
            LowerAnimYFrames = myBuffer.ReadInteger();
            LowerAnimFrameCount = myBuffer.ReadInteger();
            LowerAnimFrameSpeed = myBuffer.ReadInteger();
            LowerAnimLoopCount = myBuffer.ReadInteger();
            LowerLights = new Light[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new Light(myBuffer);
            }

            //Upper Animation
            UpperAnimSprite = myBuffer.ReadString();
            UpperAnimXFrames = myBuffer.ReadInteger();
            UpperAnimYFrames = myBuffer.ReadInteger();
            UpperAnimFrameCount = myBuffer.ReadInteger();
            UpperAnimFrameSpeed = myBuffer.ReadInteger();
            UpperAnimLoopCount = myBuffer.ReadInteger();
            UpperLights = new Light[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new Light(myBuffer);
            }

            myBuffer.Dispose();
        }

        public byte[] AnimData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Version);
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Sound);

            //Lower Animation
            myBuffer.WriteString(LowerAnimSprite);
            myBuffer.WriteInteger(LowerAnimXFrames);
            myBuffer.WriteInteger(LowerAnimYFrames);
            myBuffer.WriteInteger(LowerAnimFrameCount);
            myBuffer.WriteInteger(LowerAnimFrameSpeed);
            myBuffer.WriteInteger(LowerAnimLoopCount);
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(LowerLights[i].LightData());
            }

            //Upper Animation
            myBuffer.WriteString(UpperAnimSprite);
            myBuffer.WriteInteger(UpperAnimXFrames);
            myBuffer.WriteInteger(UpperAnimYFrames);
            myBuffer.WriteInteger(UpperAnimFrameCount);
            myBuffer.WriteInteger(UpperAnimFrameSpeed);
            myBuffer.WriteInteger(UpperAnimLoopCount);
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(UpperLights[i].LightData());
            }

            return myBuffer.ToArray();
        }

        public void Save(int index)
        {
            File.WriteAllBytes("resources/animations/" + index + ".anim", AnimData());
        }

    }
}
