using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class WarpToMeAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.WarpToMe;
        public string Name { get; set; }

        public WarpToMeAction(string name)
        {
            Name = name;
        }
    }
}
