using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class AnimationBase : DatabaseObject<AnimationBase>
    {
        //Lower Animation
        public string LowerAnimSprite { get; set; } = "";
        public int LowerAnimFrameCount { get; set; } = 1;
        public int LowerAnimXFrames { get; set; } = 1;
        public int LowerAnimYFrames { get; set; } = 1;
        public int LowerAnimFrameSpeed { get; set; } = 100;
        public int LowerAnimLoopCount { get; set; }
        public bool DisableLowerRotations { get; set; }
        [Column("LowerLights")]
        public string LowerLightsJson
        {
            get => JsonConvert.SerializeObject(LowerLights);
            set => LowerLights = JsonConvert.DeserializeObject<LightBase[]>(value);
        }
        [NotMapped]
        public LightBase[] LowerLights { get; set; }


        //Upper Animation
        public string UpperAnimSprite { get; set; }
        public int UpperAnimFrameCount { get; set; } = 1;
        public int UpperAnimXFrames { get; set; } = 1;
        public int UpperAnimYFrames { get; set; } = 1;
        public int UpperAnimFrameSpeed { get; set; } = 100;
        public int UpperAnimLoopCount { get; set; }
        public bool DisableUpperRotations { get; set; }
        [Column("UpperLights")]
        public string UpperLightsJson
        {
            get => JsonConvert.SerializeObject(UpperLights);
            set => UpperLights = JsonConvert.DeserializeObject<LightBase[]>(value);
        }
        [NotMapped]
        public LightBase[] UpperLights { get; set; }

        //Misc
        public string Sound { get; set; }


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