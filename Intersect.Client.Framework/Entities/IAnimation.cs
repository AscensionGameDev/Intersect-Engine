using Intersect.GameObjects;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Animations;

namespace Intersect.Client.Framework.Entities;

public interface IAnimation : IDisposable
{
    bool AutoRotate { get; set; }
    bool Hidden { get; set; }
    bool InfiniteLoop { get; set; }
    AnimationDescriptor? Descriptor { get; set; }
    Point Size { get; }

    void Hide();
    void SetDir(Direction dir);
    void SetPosition(float worldX, float worldY, int mapx, int mapy, Guid mapId, Direction dir, int z = 0);
    void Show();
}
