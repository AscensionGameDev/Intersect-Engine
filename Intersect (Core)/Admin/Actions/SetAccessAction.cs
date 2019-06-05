using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class SetAccessAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.SetAccess;
        public string Name { get; set; }
        public string Power { get; set; }

        public SetAccessAction(string name, string power)
        {
            Name = name;
            Power = power;
        }
    }
}
