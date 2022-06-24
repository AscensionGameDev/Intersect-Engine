using System;

using Intersect.Client.General;
using Intersect.Client.Framework.Maps;

namespace Intersect.Client.Maps
{
    public partial class MapGrid : IMapGrid
    {
        /// <inheritdoc>
        public Guid[,] Content => Globals.MapGrid;

        /// <inheritdoc>
        public long Height => Globals.MapGridHeight;

        /// <inheritdoc>
        public long Width => Globals.MapGridWidth;

    }
}
