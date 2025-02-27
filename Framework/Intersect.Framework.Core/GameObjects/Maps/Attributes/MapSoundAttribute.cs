using Intersect.GameObjects.Annotations;

namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

public partial class MapSoundAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Sound;

    [EditorLabel("Attributes", "Sound")]
    [EditorDisplay(
        EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty,
        StringBehavior = StringBehavior.Trim
    )]
    public string File { get; set; }

    [EditorLabel("Attributes", "SoundDistance")]
    [EditorFormatted("Attributes", "DistanceFormat")]
    public byte Distance { get; set; }

    [EditorLabel("Attributes", "SoundInterval")]
    [EditorTime]
    public int LoopInterval { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapSoundAttribute) base.Clone();
        att.File = File;
        att.Distance = Distance;
        att.LoopInterval = LoopInterval;

        return att;
    }
}