using Intersect.Client.Framework.Maps;
using System;
using System.Collections.Generic;

namespace Intersect.Client.Framework.Entities
{
    public interface ICritter
    {
        Guid CurrentMap { get; set; }
        byte X { get; set; }
        byte Y { get; set; }
        byte Z { get; set; }
        bool Passable { get; set; }
        HashSet<IEntity> DetermineRenderOrder(HashSet<IEntity> renderList, IMapInstance map);
        float GetMovementTime();
        bool PlayerOnTile(Guid mapId, int x, int y);
        bool Update();
    }
}