using System;
using System.Collections.Generic;

using Intersect.Collections;
using Intersect.GameObjects;

namespace Intersect.Network.Packets.Client
{

    public class CreateCharacterPacket : CerasPacket
    {

        public CreateCharacterPacket(string name, Guid classId, int sprite)
        {
            Name = name;
            ClassId = classId;
            Sprite = sprite;
        }

        public string Name { get; set; }

        public Guid ClassId { get; set; }

        public int Sprite { get; set; }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            base.Sanitize();

            var sanitizer = new Sanitizer();

            var classDescriptor = ClassBase.Get(ClassId);
            if (classDescriptor != null)
            {
                Sprite = sanitizer.Clamp(nameof(Sprite), Sprite, 0, classDescriptor.Sprites?.Count ?? 0);
            }

            return sanitizer.Sanitized;
        }

    }

}
