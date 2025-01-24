using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Items;
using Intersect.Client.Framework.Maps;
using Intersect.Enums;

namespace Intersect.Client.Framework.Entities;

public interface IEntity : IDisposable
{
    Guid Id { get; }
    EntityType Type { get; }
    string Name { get; }
    Gender Gender { get; }
    Color? NameColor { get; }
    Label FooterLabel { get; }
    Label HeaderLabel { get; }
    bool IsHidden { get; }
    string Sprite { get; }
    string TransformedSprite { get; }
    string Face { get; }
    GameTexture? Texture { get; }
    Color Color { get; }
    FloatRect WorldPos { get; }
    float OffsetX { get; }
    float OffsetY { get; }
    Pointf Center { get; }
    Pointf Origin { get; }
    bool IsMoving { get; }
    bool IsStealthed { get; }
    bool IsBlocking { get; }
    bool IsDashing { get; }
    IDash? CurrentDash { get; }
    bool IsCasting { get; }
    bool InView { get; }
    IMapInstance? MapInstance { get; }
    Guid MapId { get; }
    Direction Dir { get; }
    byte X { get; }
    byte Y { get; }
    byte Z { get; }
    int Level { get; }
    IReadOnlyDictionary<Stat, int> Stats { get; }
    IReadOnlyDictionary<Vital, long> Vitals { get; }
    IReadOnlyDictionary<Vital, long> MaxVitals { get; }
    IReadOnlyList<IItem> Items { get; }
    IReadOnlyList<int> EquipmentSlots { get; }
    IReadOnlyList<Guid> Spells { get; }
    IReadOnlyList<IStatus> Status { get; }
    NpcAggression Aggression { get; }

    void AddChatBubble(string text);
    float GetLabelLocation(LabelType type);
    float GetTop(int overrideHeight = 0);

    long ShieldSize { get; }

    bool IsDisposed { get; }
}
