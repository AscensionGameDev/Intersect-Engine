using System;

namespace Intersect.Client.Framework.Core.Sounds
{
    public interface IMapSound : ISound
    {
        bool Update();
        void UpdatePosition(int x, int y, Guid mapId);
    }
}