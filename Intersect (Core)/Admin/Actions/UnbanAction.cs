using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class UnbanAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.UnBan;
        public string Name { get; set; }

        public UnbanAction(string name)
        {
            Name = name;
        }
    }
}
