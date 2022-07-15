using System;
using System.Collections.Generic;

using Intersect.GameObjects;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities
{

    public partial class ProjectileSpawn
    {

        public byte Dir;

        public int Distance;

        public Guid MapId;

        public Projectile Parent;

        public ProjectileBase ProjectileBase;

        public long TransmittionTimer = Timing.Global.Milliseconds;

        public float X;

        public float Y;

        public byte Z;

        public bool Dead;

        public Guid MapInstanceId;

        private List<Guid> mEntitiesCollided = new List<Guid>();

        public ProjectileSpawn(
            byte dir,
            byte x,
            byte y,
            byte z,
            Guid mapId,
            Guid mapInstanceId,
            ProjectileBase projectileBase,
            Projectile parent
        )
        {
            MapId = mapId;
            MapInstanceId = mapInstanceId;
            X = x;
            Y = y;
            Z = z;
            Dir = dir;
            ProjectileBase = projectileBase;
            Parent = parent;
            TransmittionTimer = Timing.Global.Milliseconds +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public bool IsAtLocation(Guid mapId, int x, int y, int z)
        {
            return MapId == mapId && X == x && Y == y && Z == z;
        }

        public bool HitEntity(Entity targetEntity)
        {
            Player targetPlayer = targetEntity as Player;

            if (targetEntity != null && targetEntity != Parent.Owner)
            {
                // Have we collided with this entity before? If so, cancel out.
                if (mEntitiesCollided.Contains(targetEntity.Id))
                {
                    if (!Parent.Base.PierceTarget)
                    {
                        if(targetPlayer != null)
                        {
                            if(targetPlayer.Map.ZoneType == Enums.MapZones.Safe ||
                                Parent.Owner is Player plyr && plyr.InParty(targetPlayer))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                mEntitiesCollided.Add(targetEntity.Id);

                if (targetPlayer != null)
                {
                    if (Parent.Owner != Parent.Target)
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);

                        if (Dir <= 3 && ShouldHook(targetEntity) && !Parent.HasGrappled)
                        {
                            HookEntity();
                        }

                        if (!Parent.Base.PierceTarget)
                        {
                            if (targetPlayer.Map.ZoneType == Enums.MapZones.Safe ||
                                Parent.Owner is Player plyr && plyr.InParty(targetPlayer))
                            {
                                return false;
                            }

                            return true;
                        }
                    }
                }
                else if (targetEntity is Resource targetResource)
                {
                    if(targetResource.IsDead())
                    {
                        if(!ProjectileBase.IgnoreExhaustedResources)
                        {
                            return true;
                        }
                    }
                    else if (!ProjectileBase.IgnoreActiveResources)
                    {
                        Parent.Owner.TryAttack(targetResource, Parent.Base, Parent.Spell, Parent.Item, Dir);

                        if (Dir <= 3 && ShouldHook(targetResource) && !Parent.HasGrappled)
                        {
                            HookEntity();
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

                        if (Dir <= 3 && ShouldHook(targetEntity) && !Parent.HasGrappled) 
                        {
                            HookEntity();
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

        /// <summary>
        /// Returns whether or not to hook the player to the target
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public bool ShouldHook(Entity en)
        {
            if(en == null)
            {
                return false;
            }

            switch(en)
            {
                case Player _:
                    return ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOptions.Player);

                case Npc _:
                    return ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOptions.NPC);

                case Resource _:
                    return ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOptions.Resource);

                default:
                    throw new ArgumentException($"Unsupported entity type {en.GetType().FullName}", nameof(en));
            }
        }

        /// <summary>
        /// Hook the player to the target
        /// </summary>
        public void HookEntity()
        {
            //Don't handle directional projectile grapplehooks
            Parent.HasGrappled = true;
            Parent.Owner.Dir = Dir;
            var _ = new Dash(
                Parent.Owner, Distance, (byte)Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks,
                Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources,
                Parent.Base.IgnoreZDimension
            );
        }

    }

}
