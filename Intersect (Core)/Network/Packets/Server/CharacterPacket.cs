using System;

namespace Intersect.Network.Packets.Server
{

    public class CharacterPacket : CerasPacket
    {

        public CharacterPacket(
            Guid id,
            string name,
            string sprite,
            string face,
            int level,
            string className,
            string[] equipment
        )
        {
            Id = id;
            Name = name;
            Sprite = sprite;
            Face = face;
            Level = level;
            ClassName = className;
            Equipment = equipment;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Sprite { get; set; }

        public string Face { get; set; }

        public int Level { get; set; }

        public string ClassName { get; set; }

        public string[] Equipment { get; set; }

    }

}
