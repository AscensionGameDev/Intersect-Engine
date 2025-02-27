using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.PlayerClass;

public partial class ClassSprite
{
    public string Face { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public string Sprite { get; set; } = string.Empty;
}