using System;

namespace Intersect.Client.Framework.Items
{
    public interface IMapItemInstance : IItem
    {
        int TileIndex { get; }
        Guid UniqueId { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}