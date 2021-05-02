using System;

using Intersect.Client.General;
using Intersect.GameObjects;

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

        public long SpawnTime = Globals.System.GetTimeMs();

        public int SpawnX;

        public int SpawnY;

        public long TransmittionTimer = Globals.System.GetTimeMs();

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
            TransmittionTimer = Globals.System.GetTimeMs() +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public void Dispose()
        {
            Anim.DisposeNextDraw();
        }

    }

}
