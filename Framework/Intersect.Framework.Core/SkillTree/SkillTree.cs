using System;
using System.Collections.Generic;
using MessagePack;

namespace Intersect.Framework.SkillTree
{
    [MessagePackObject]
    public class SkillTree
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public List<SkillNode> Nodes { get; set; } = new List<SkillNode>();
    }

    [MessagePackObject]
    public class SkillNode
    {
        [Key(0)]
        public Guid SpellId { get; set; }

        [Key(1)]
        public int X { get; set; }

        [Key(2)]
        public int Y { get; set; }

        [Key(3)]
        public int Cost { get; set; } = 1;

        [Key(4)]
        public int MaxLevel { get; set; } = 1;

        [Key(5)]
        public List<Guid> Prerequisites { get; set; } = new List<Guid>();
    }
}
