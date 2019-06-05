using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Admin.Actions
{
    public class WarpToLocationAction : AdminAction
    {
        public override AdminActions Action { get; } = AdminActions.WarpTo;
        public Guid MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public WarpToLocationAction(Guid mapId, int x, int y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }
    }
}
