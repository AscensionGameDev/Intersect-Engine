using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Models;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    [Owned]
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

        public bool AlternateRenderLayer { get; set; }

        [JsonIgnore]
        public string Light
        {
            get => JsonConvert.SerializeObject(Lights);
            set => Lights = JsonConvert.DeserializeObject<LightBase[]>(value);
        }

        [NotMapped]
        public LightBase[] Lights { get; set; }

    }

    public class AnimationBase : DatabaseObject<AnimationBase>, IFolderable
    {

        [JsonConstructor]
        public AnimationBase(Guid id) : base(id)
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

        public AnimationLayer Lower { get; set; }

        public AnimationLayer Upper { get; set; }

        //Misc
        public string Sound { get; set; }

        public bool CompleteSound { get; set; }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

    }

}
