using System;

using Intersect.GameObjects;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities
{

    public class ProjectileSpawn
    {

        public byte Dir;

        public int Distance;

        public Guid MapId;

        public Projectile Parent;

        public ProjectileBase ProjectileBase;

        public long TransmittionTimer = Globals.Timing.Milliseconds;

        public byte X;

        public byte Y;

        public byte Z;

        public ProjectileSpawn(
            byte dir,
            byte x,
            byte y,
            byte z,
            Guid mapId,
            ProjectileBase projectileBase,
            Projectile parent
        )
        {
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
            Dir = dir;
            ProjectileBase = projectileBase;
            Parent = parent;
            TransmittionTimer = Globals.Timing.Milliseconds +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public bool IsAtLocation(Guid mapId, int x, int y, int z)
        {
            return MapId == mapId && X == x && Y == y && Z == z;
        }

        public bool HitEntity(Entity en)
        {
            var targetEntity = en;
            if (targetEntity is EventPageInstance) return false;
            if (targetEntity != null && targetEntity != Parent.Owner)
            {
                if (targetEntity.GetType() == typeof(Player)) //Player
                {
                    if (Parent.Owner != Parent.Target)
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled
                        ) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new Dash(
                                Parent.Owner, Distance, (byte) Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks,
                                Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources,
                                Parent.Base.IgnoreZDimension
                            );
                        }

                        if (!Parent.Base.PierceTarget)
                        {
                            return true;
                        }
                    }
                }
                else if (targetEntity.GetType() == typeof(Resource))
                {
                    if (((Resource) targetEntity).IsDead && !ProjectileBase.IgnoreExhaustedResources ||
                        !((Resource) targetEntity).IsDead && !ProjectileBase.IgnoreActiveResources)
                    {
                        if (Parent.Owner.GetType() == typeof(Player) && !((Resource) targetEntity).IsDead)
                        {
                            Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                            if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled
                            ) //Don't handle directional projectile grapplehooks
                            {
                                Parent.HasGrappled = true;
                                Parent.Owner.Dir = Dir;
                                new Dash(
                                    Parent.Owner, Distance, (byte) Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks,
                                    Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources,
                                    Parent.Base.IgnoreZDimension
                                );
                            }
                        }

                        return true;
                    }
                }
                else //Any other Parent.Target
                {
                    var ownerNpc = Parent.Owner as Npc;
                    if (ownerNpc == null ||
                        ownerNpc.CanNpcCombat(targetEntity, Parent.Spell != null && Parent.Spell.Combat.Friendly))
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled
                        ) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new Dash(
                                Parent.Owner, Distance, (byte) Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks,
                                Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources,
                                Parent.Base.IgnoreZDimension
                            );
                        }

                        if (!Parent.Base.PierceTarget)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Dispose(int spawnIndex)
        {
            PacketSender.SendRemoveProjectileSpawn(MapId, Parent.Id, spawnIndex);
        }

    }

}
