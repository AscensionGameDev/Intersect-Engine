using System;

namespace Intersect.Client.Framework.Items
{
    public interface IMapItemInstance : IItem
    {
        int TileIndex { get; }
        Guid Id { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}