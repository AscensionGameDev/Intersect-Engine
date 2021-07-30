using System;

namespace Intersect.Client.Framework.Core.Sounds
{
    public interface IMapSound : ISound
    {
        void UpdatePosition(int x, int y, Guid mapId);
    }
}