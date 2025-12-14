using System.Collections.Generic;
using System.IO;
using Intersect.Framework.SkillTree;
using Newtonsoft.Json;
using Intersect.Server.Core;

namespace Intersect.Server.Systems
{
    public static class SkillTreeSystem
    {
        public static List<SkillTree> SkillTrees { get; private set; } = new List<SkillTree>();

        public static void Initialize()
        {
            var path = Path.Combine("resources", "skill_trees.json");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                SkillTrees = JsonConvert.DeserializeObject<List<SkillTree>>(json);
            }
        }
    }
}
