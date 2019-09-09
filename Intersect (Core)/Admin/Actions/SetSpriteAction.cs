using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class SetSpriteAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.SetSprite;
        public string Name { get; set; }
        public string Sprite { get; set; }

        public SetSpriteAction(string name, string sprite)
        {
            Name = name;
            Sprite = sprite;
        }
    }
}
