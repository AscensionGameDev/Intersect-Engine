using Intersect.GameObjects;
using System;

namespace Intersect.Client.Framework.Entities
{
    public interface IAnimation : IDisposable
    {
        bool AutoRotate { get; set; }
        bool Hidden { get; set; }
        bool InfiniteLoop { get; set; }
        AnimationBase MyBase { get; set; }
        Point Size { get; }

        void Hide();
        void SetDir(int dir);
        void SetPosition(float worldX, float worldY, int mapx, int mapy, Guid mapId, int dir, int z = 0);
        void Show();
    }
}