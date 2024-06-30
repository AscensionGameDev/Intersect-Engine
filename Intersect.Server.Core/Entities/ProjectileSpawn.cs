using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Utilities;

namespace Intersect.Server.Entities
{

    public partial class ProjectileSpawn
    {

        public Direction Dir;

        public int Distance;

        public Guid MapId;

        public Projectile Parent;

        public ProjectileBase ProjectileBase;

        public long TransmissionTimer = Timing.Global.Milliseconds;

        public float X;

        public float Y;

        public byte Z;

        public bool Dead;

        public Guid MapInstanceId;

        private List<Guid> _entitiesCollided = new List<Guid>();

        public ProjectileSpawn(
            Direction dir,
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
            TransmissionTimer = Timing.Global.Milliseconds +
                                (long)(ProjectileBase.Speed / (float)ProjectileBase.Range);
        }

        public bool IsAtLocation(Guid mapId, int x, int y, int z)
        {
            return MapId == mapId && X == x && Y == y && Z == z;
        }

        public bool HitEntity(Entity targetEntity)
        {
            if (targetEntity is EventPageInstance)
            {
                return false;
            }

            Player targetPlayer = targetEntity as Player;

            if (targetEntity != null && targetEntity != Parent.Owner)
            {
                // Have we collided with this entity before? If so, cancel out.
                if (_entitiesCollided.Contains(targetEntity.Id))
                {
                    if (!Parent.Base.PierceTarget)
                    {
                        if (targetPlayer != null)
                        {
                            if (targetPlayer.Map.ZoneType == Enums.MapZone.Safe ||
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
                _entitiesCollided.Add(targetEntity.Id);

                if (targetPlayer != null)
                {
                    if (Parent.Owner != Parent.Target)
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);

                        if (Dir <= Direction.Right && ShouldHook(targetEntity) && !Parent.HasGrappled)
                        {
                            HookEntity();
                        }

                        if (!Parent.Base.PierceTarget)
                        {
                            if (targetPlayer.Map.ZoneType == Enums.MapZone.Safe ||
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
                    if (targetResource.IsDead())
                    {
                        if (!ProjectileBase.IgnoreExhaustedResources)
                        {
                            return true;
                        }
                    }
                    else if (!ProjectileBase.IgnoreActiveResources)
                    {
                        Parent.Owner.TryAttack(targetResource, Parent.Base, Parent.Spell, Parent.Item, Dir);

                        if (Dir <= Direction.Right && ShouldHook(targetResource) && !Parent.HasGrappled)
                        {
                            HookEntity();
                        }

                        return true;
                    }
                }
                else //Any other Parent.Target
                {
                    if (Parent.Owner is not Npc ownerNpc ||
                        ownerNpc.CanNpcCombat(targetEntity, Parent.Spell != null && Parent.Spell.Combat.Friendly))
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);

                        if (Dir <= Direction.Right && ShouldHook(targetEntity) && !Parent.HasGrappled)
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
            if (en == null)
            {
                return false;
            }

            return en switch
            {
                Player => ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOption.Player),
                Npc => ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOption.NPC),
                Resource => ProjectileBase.GrappleHookOptions.Contains(Enums.GrappleOption.Resource),
                _ => throw new ArgumentException($"Unsupported entity type {en.GetType().FullName}", nameof(en)),
            };
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
                Parent.Owner, Distance, Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks,
                Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources,
                Parent.Base.IgnoreZDimension
            );
        }

    }

}
