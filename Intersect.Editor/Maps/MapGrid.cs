using System;

using Intersect.Editor.General;
using Intersect.Client.Framework.Maps;

namespace Intersect.Editor.Maps
{
    public class MapGrid : IMapGrid
    {
        /// <inheritdoc>
        public Guid[,] Content => Globals.MapGrid;

        /// <inheritdoc>
        public long Height => Globals.MapGridHeight;

        /// <inheritdoc>
        public long Width => Globals.MapGridWidth;

    }
}
