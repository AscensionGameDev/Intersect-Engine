using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Lighting;
using Intersect.GameObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Animations;

[Owned]
public partial class AnimationLayer
{
    public AnimationLayer()
    {
        Lights = new LightDescriptor[FrameCount];

        for (var frame = 0; frame < FrameCount; ++frame)
        {
            Lights[frame] = new LightDescriptor();
        }
    }

    public string Sprite { get; set; } = string.Empty;

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
        set => Lights = JsonConvert.DeserializeObject<LightDescriptor[]>(value);
    }

    [NotMapped]
    public LightDescriptor[] Lights { get; set; }
}