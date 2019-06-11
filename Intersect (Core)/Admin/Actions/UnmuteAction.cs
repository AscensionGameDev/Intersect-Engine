using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class UnmuteAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.UnMute;
        public string Name { get; set; }

        public UnmuteAction(string name)
        {
            Name = name;
        }
    }
}
