using Intersect.Models;
using Intersect.Utilities;

namespace Intersect.GameObjects
{
    public class AnimationBase : DatabaseObject<AnimationBase>
    {
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount;
        public bool DisableLowerRotations;

        //Lower Animation
        public string LowerAnimSprite = "";

        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public LightBase[] LowerLights;

        public string Sound = "";
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount;
        public bool DisableUpperRotations;

        //Upper Animation
        public string UpperAnimSprite = "";

        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public LightBase[] UpperLights;

        public AnimationBase(int id) : base(id)
        {
            Name = "New Animation";
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (var i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new LightBase();
            }
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (var i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase();
            }
        }

        public override byte[] BinaryData => AnimData();

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
            DisableLowerRotations = myBuffer.ReadBoolean();
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (var i = 0; i < LowerAnimFrameCount; i++)
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
            DisableUpperRotations = myBuffer.ReadBoolean();
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (var i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase(myBuffer);
            }

            myBuffer.Dispose();
        }

        private byte[] AnimData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(TextUtils.SanitizeNone(Sound));

            //Lower Animation
            myBuffer.WriteString(TextUtils.SanitizeNone(LowerAnimSprite));
            myBuffer.WriteInteger(LowerAnimXFrames);
            myBuffer.WriteInteger(LowerAnimYFrames);
            myBuffer.WriteInteger(LowerAnimFrameCount);
            myBuffer.WriteInteger(LowerAnimFrameSpeed);
            myBuffer.WriteInteger(LowerAnimLoopCount);
            myBuffer.WriteBoolean(DisableLowerRotations);
            for (var i = 0; i < LowerAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(LowerLights[i].LightData());
            }

            //Upper Animation
            myBuffer.WriteString(TextUtils.SanitizeNone(UpperAnimSprite));
            myBuffer.WriteInteger(UpperAnimXFrames);
            myBuffer.WriteInteger(UpperAnimYFrames);
            myBuffer.WriteInteger(UpperAnimFrameCount);
            myBuffer.WriteInteger(UpperAnimFrameSpeed);
            myBuffer.WriteInteger(UpperAnimLoopCount);
            myBuffer.WriteBoolean(DisableUpperRotations);
            for (var i = 0; i < UpperAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(UpperLights[i].LightData());
            }

            return myBuffer.ToArray();
        }
    }
}