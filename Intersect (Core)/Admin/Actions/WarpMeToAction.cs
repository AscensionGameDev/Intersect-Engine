using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class WarpMeToAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.WarpMeTo;
        public string Name { get; set; }

        public WarpMeToAction(string name)
        {
            Name = name;
        }
    }
}
