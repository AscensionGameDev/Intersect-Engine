using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class KillAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.Kill;
        public string Name { get; set; }

        public KillAction(string name)
        {
            Name = name;
        }
    }
}
