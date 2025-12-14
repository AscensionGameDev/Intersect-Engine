using System.Collections.Generic;
using Intersect.Framework.SkillTree;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class SkillTreeDefinitionPacket : IntersectPacket
    {
        public SkillTreeDefinitionPacket(List<SkillTree> trees)
        {
            Trees = trees;
        }

        public SkillTreeDefinitionPacket()
        {
        }

        [Key(0)]
        public List<SkillTree> Trees { get; set; }
    }
}
