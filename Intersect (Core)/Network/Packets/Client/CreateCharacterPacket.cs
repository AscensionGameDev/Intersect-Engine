using Intersect.Collections;
using Intersect.GameObjects;

using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Client
{
    public class CreateCharacterPacket : CerasPacket
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public int Sprite { get; set; }

        public CreateCharacterPacket(string name, Guid classId, int sprite)
        {
            Name = name;
            ClassId = classId;
            Sprite = sprite;
        }

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
