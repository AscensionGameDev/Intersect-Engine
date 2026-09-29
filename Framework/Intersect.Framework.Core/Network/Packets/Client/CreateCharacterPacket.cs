using Intersect.Collections;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.GameObjects;
using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class CreateCharacterPacket : IntersectPacket
{
    public const int MaxSpriteColors = 8;

    //Parameterless Constructor for MessagePack
    public CreateCharacterPacket()
    {
    }

    public CreateCharacterPacket(string name, Guid classId, int sprite, Color[] spriteColors = null, SpritePart[] spriteParts = null, Guid factionId = default)
    {
        Name = name;
        ClassId = classId;
        Sprite = sprite;
        SpriteColors = spriteColors ?? [];
        SpriteParts = spriteParts ?? [];
        FactionId = factionId;
    }

    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public Guid ClassId { get; set; }

    [Key(2)]
    public int Sprite { get; set; }

    /// <summary>Tint applied to each "_c1", "_c2", ... colour layer of the sprite (Dofus-style customisation).</summary>
    [Key(3)]
    public Color[] SpriteColors { get; set; } = [];

    /// <summary>Interchangeable parts (hair, outfit, ...) picked for the sprite, each with its own colours.</summary>
    [Key(4)]
    public SpritePart[] SpriteParts { get; set; } = [];

    /// <summary>Faction picked at character creation, empty if the server has none defined.</summary>
    [Key(5)]
    public Guid FactionId { get; set; }

    public override Dictionary<string, SanitizedValue<object>> Sanitize()
    {
        base.Sanitize();

        var sanitizer = new Sanitizer();

        var classDescriptor = ClassDescriptor.Get(ClassId);
        if (classDescriptor != null)
        {
            Sprite = sanitizer.Clamp(nameof(Sprite), Sprite, 0, classDescriptor.Sprites?.Count ?? 0);
        }

        if (SpriteColors == null)
        {
            SpriteColors = [];
        }
        else if (SpriteColors.Length > MaxSpriteColors)
        {
            SpriteColors = SpriteColors.Take(MaxSpriteColors).ToArray();
        }

        SpriteParts = SpritePart.SanitizeAll(SpriteParts);

        return sanitizer.Sanitized;
    }

}
