using System.ComponentModel.DataAnnotations;
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
        [JsonIgnore]
        [Column("Lower")]
        public string LowerJson
        {
            get => JsonConvert.SerializeObject(Lower, Formatting.None);
            protected set => Lower = JsonConvert.DeserializeObject<AnimationLayer>(value);
        }
        [NotMapped]
        public AnimationLayer Lower { get; set; }

        [JsonIgnore]
        [Column("Upper")]
        public string UpperJson
        {
            get => JsonConvert.SerializeObject(Upper, Formatting.None);
            protected set => Upper = JsonConvert.DeserializeObject<AnimationLayer>(value);
        }
        [NotMapped]
        public AnimationLayer Upper { get; set; }

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

        //EF Parameterless Constructor
        public AnimationBase()
        {
            // TODO: localize this
            Name = "New Animation";
            Lower = new AnimationLayer();
            Upper = new AnimationLayer();
        }
    }
}