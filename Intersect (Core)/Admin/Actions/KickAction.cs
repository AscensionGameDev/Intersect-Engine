using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class KickAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.Kick;
        public string Name { get; set; }

        public KickAction(string name)
        {
            Name = name;
        }
    }
}
