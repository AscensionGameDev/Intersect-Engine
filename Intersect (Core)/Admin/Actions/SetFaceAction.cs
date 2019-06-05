using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class SetFaceAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.SetFace;
        public string Name { get; set; }
        public string Face { get; set; }

        public SetFaceAction(string name, string face)
        {
            Name = name;
            Face = face;
        }
    }
}
