using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class AnimationLayer
    {
        public AnimationLayer()
        {
            Lights = new LightBase[FrameCount];

            for (var frame = 0; frame < FrameCount; ++frame)
            {
                Lights[frame] = new LightBase();
            }
        }

        public string Sprite { get; set; } = "";

        public int FrameCount { get; set; } = 1;

        public int XFrames { get; set; } = 1;

        public int YFrames { get; set; } = 1;

        public int FrameSpeed { get; set; } = 100;

        public int LoopCount { get; set; }

        public bool DisableRotations { get; set; }

        [NotMapped]
        public LightBase[] Lights { get; set; }
    }

    public class AnimationBase : DatabaseObject<AnimationBase>
    {
        public AnimationLayer Lower { get; set; }

        public AnimationLayer Upper { get; set; }

        [Column("Lower_Lights")]
        public string JsonLowerLights
        {
            get => JsonConvert.SerializeObject(Lower.Lights);
            set => Lower.Lights = JsonConvert.DeserializeObject<LightBase[]>(value);
        }

        [Column("Upper_Lights")]
        public string JsonUpperLights
        {
            get => JsonConvert.SerializeObject(Upper.Lights);
            set => Upper.Lights = JsonConvert.DeserializeObject<LightBase[]>(value);
        }

        //Misc
        public string Sound { get; set; }


        [JsonConstructor]
        public AnimationBase(int index) : base(index)
        {
            // TODO: localize this
            Name = "New Animation";
            Lower = new AnimationLayer();
            Upper = new AnimationLayer();
        }
    }
}