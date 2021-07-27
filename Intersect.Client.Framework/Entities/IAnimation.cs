using Intersect.GameObjects;
using System;

namespace Intersect.Client.Framework.Entities
{
    public interface IAnimation
    {
        bool AutoRotate { get; set; }
        bool Hidden { get; set; }
        bool InfiniteLoop { get; set; }
        AnimationBase MyBase { get; set; }

        Point AnimationSize();
        void Dispose();
        bool Disposed();
        void DisposeNextDraw();
        void Draw(bool upper = false, bool alternate = false);
        void EndDraw();
        void Hide();
        bool ParentGone();
        void SetDir(int dir);
        void SetPosition(float worldX, float worldY, int mapx, int mapy, Guid mapId, int dir, int z = 0);
        void Show();
        void Update();
    }
}