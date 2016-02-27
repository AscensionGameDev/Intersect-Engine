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
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
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
    }
}
