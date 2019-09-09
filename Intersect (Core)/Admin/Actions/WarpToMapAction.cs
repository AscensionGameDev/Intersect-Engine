using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class WarpToMapAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.WarpTo;
        public Guid MapId { get; set; }

        public WarpToMapAction(Guid mapId)
        {
            MapId = mapId;
        }
    }
}
