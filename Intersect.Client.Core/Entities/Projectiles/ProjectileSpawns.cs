using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Entities.Projectiles;

public partial class ProjectileSpawns
{

    public Animation Anim;

    public bool AutoRotate;

    public Direction Dir;

    public int Distance;

    public Guid MapId;

    //Clientside variables
    public float OffsetX;

    public float OffsetY;

    public ProjectileDescriptor ProjectileDescriptor;

    public Guid SpawnMapId;

    public long SpawnTime = Timing.Global.Milliseconds;

    public int SpawnX;

    public int SpawnY;

    public long TransmissionTimer = Timing.Global.Milliseconds;

    public int X;

    public int Y;

    public int Z;

    public ProjectileSpawns(
        Direction dir,
        int x,
        int y,
        int z,
        Guid mapId,
        AnimationDescriptor animationDescriptor,
        bool autoRotate,
        ProjectileDescriptor projectileDescriptor,
        Entity parent
    )
    {
        X = x;
        Y = y;
        SpawnX = X;
        SpawnY = Y;
        Z = z;
        MapId = mapId;
        SpawnMapId = MapId;
        Dir = dir;
        Anim = new Animation(animationDescriptor, true, autoRotate, Z, parent);
        AutoRotate = autoRotate;
        ProjectileDescriptor = projectileDescriptor;
        TransmissionTimer = Timing.Global.Milliseconds + (ProjectileDescriptor.Speed / ProjectileDescriptor.Range);
    }

    public void Dispose()
    {
        Anim.DisposeNextDraw();
    }
}
