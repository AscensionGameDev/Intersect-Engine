using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

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

        [JsonConstructor]
        public AnimationBase(int index) : base(index)
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
    }
}