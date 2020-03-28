using System;

namespace Intersect.Network.Packets.Server
{

    public class EntityPacket : CerasPacket
    {

        public Guid EntityId { get; set; }

        public Guid MapId { get; set; }

        public string Name { get; set; }

        public string Sprite { get; set; }

        public string Face { get; set; }

        public int Level { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public byte Z { get; set; }

        public byte Dir { get; set; }

        public bool Passable { get; set; }

        public bool HideName { get; set; }

        public bool HideEntity { get; set; }

        public Guid[] Animations { get; set; }

        public int[] Vital { get; set; }

        public int[] MaxVital { get; set; }

        public int[] Stats { get; set; }

        public StatusPacket[] StatusEffects { get; set; }

        public bool IsSelf { get; set; }

        public Color NameColor { get; set; }

        public LabelPacket HeaderLabel { get; set; }

        public LabelPacket FooterLabel { get; set; }

    }

}
