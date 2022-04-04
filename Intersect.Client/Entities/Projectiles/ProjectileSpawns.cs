using System;

using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Entities.Projectiles
{

    public partial class ProjectileSpawns
    {

        public Animation Anim;

        public bool AutoRotate;

        public int Dir;

        public int Distance;

        public Guid MapId;

        //Clientside variables
        public float OffsetX;

        public float OffsetY;

        public ProjectileBase ProjectileBase;

        public Guid SpawnMapId;

        public long SpawnTime = Timing.Global.Milliseconds;

        public int SpawnX;

        public int SpawnY;

        public long TransmittionTimer = Timing.Global.Milliseconds;

        public int X;

        public int Y;

        public int Z;

        public ProjectileSpawns(
            int dir,
            int x,
            int y,
            int z,
            Guid mapId,
            AnimationBase animBase,
            bool autoRotate,
            ProjectileBase projectileBase,
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
            Anim = new Animation(animBase, true, autoRotate, Z, parent);
            AutoRotate = autoRotate;
            ProjectileBase = projectileBase;
            TransmittionTimer = Timing.Global.Milliseconds +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public void Dispose()
        {
            Anim.DisposeNextDraw();
        }

    }

}
