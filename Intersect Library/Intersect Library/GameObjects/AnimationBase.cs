using Intersect.Models;

namespace Intersect.GameObjects
{
    public class AnimationBase : DatabaseObject<AnimationBase>
    {
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount = 0;

        //Lower Animation
        public string LowerAnimSprite = "";
        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public LightBase[] LowerLights;

        public string Sound = "";
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount = 0;

        //Upper Animation
        public string UpperAnimSprite = "";
        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public LightBase[] UpperLights;

        public AnimationBase(int id) : base(id)
        {
            Name = "New Animation";
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new LightBase();
            }
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase();
            }
        }

        public override void Load(byte[] packet)
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
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new LightBase(myBuffer);
            }

            //Upper Animation
            UpperAnimSprite = myBuffer.ReadString();
            UpperAnimXFrames = myBuffer.ReadInteger();
            UpperAnimYFrames = myBuffer.ReadInteger();
            UpperAnimFrameCount = myBuffer.ReadInteger();
            UpperAnimFrameSpeed = myBuffer.ReadInteger();
            UpperAnimLoopCount = myBuffer.ReadInteger();
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase(myBuffer);
            }

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

        public override byte[] BinaryData => AnimData();
    }
}