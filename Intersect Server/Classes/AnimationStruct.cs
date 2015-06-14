using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Intersect_Server.Classes
{
    public class AnimationStruct
    {
        public string Name = "";
        public string Sound = "";

        //Lower Animation
        public string LowerAnimSprite = "";
        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount = 1;

        //Upper Animation
        public string UpperAnimSprite = "";
        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount = 1;

        public void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Sound = myBuffer.ReadString();

            //Lower Animation
            LowerAnimSprite = myBuffer.ReadString();
            LowerAnimXFrames = myBuffer.ReadInteger();
            LowerAnimYFrames = myBuffer.ReadInteger();
            LowerAnimFrameCount = myBuffer.ReadInteger();
            LowerAnimFrameSpeed = myBuffer.ReadInteger();
            LowerAnimLoopCount = myBuffer.ReadInteger();

            //Upper Animation
            UpperAnimSprite = myBuffer.ReadString();
            UpperAnimXFrames = myBuffer.ReadInteger();
            UpperAnimYFrames = myBuffer.ReadInteger();
            UpperAnimFrameCount = myBuffer.ReadInteger();
            UpperAnimFrameSpeed = myBuffer.ReadInteger();
            UpperAnimLoopCount = myBuffer.ReadInteger();

            myBuffer.Dispose();
        }

        public byte[] AnimData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Sound);

            //Lower Animation
            myBuffer.WriteString(LowerAnimSprite);
            myBuffer.WriteInteger(LowerAnimXFrames);
            myBuffer.WriteInteger(LowerAnimYFrames);
            myBuffer.WriteInteger(LowerAnimFrameCount);
            myBuffer.WriteInteger(LowerAnimFrameSpeed);
            myBuffer.WriteInteger(LowerAnimLoopCount);

            //Upper Animation
            myBuffer.WriteString(UpperAnimSprite);
            myBuffer.WriteInteger(UpperAnimXFrames);
            myBuffer.WriteInteger(UpperAnimYFrames);
            myBuffer.WriteInteger(UpperAnimFrameCount);
            myBuffer.WriteInteger(UpperAnimFrameSpeed);
            myBuffer.WriteInteger(UpperAnimLoopCount);

            return myBuffer.ToArray();
        }

        public void Save(int index)
        {
            File.WriteAllBytes("Resources/Animations/" + index + ".anim", AnimData());
        }

    }
}
