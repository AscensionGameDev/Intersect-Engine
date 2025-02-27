using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.GameObjects;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Utilities;

namespace Intersect.Server.Entities;


public partial class ProjectileSpawn
{

    public Direction Dir;

    public int Distance;

    public Guid MapId;

    public Projectile Parent;

    public ProjectileDescriptor ProjectileDescriptor;

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
        ProjectileDescriptor projectileDescriptor,
        Projectile parent
    )
    {
        MapId = mapId;
        MapInstanceId = mapInstanceId;
        X = x;
        Y = y;
        Z = z;
        Dir = dir;
        ProjectileDescriptor = projectileDescriptor;
        Parent = parent;
        TransmissionTimer = Timing.Global.Milliseconds +
                            (long)(ProjectileDescriptor.Speed / (float)ProjectileDescriptor.Range);
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
                if (!Parent.Descriptor.PierceTarget)
                {
                    if (targetPlayer != null)
                    {
                        if (targetPlayer.Map.ZoneType == MapZone.Safe ||
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
                    Parent.Owner.TryAttack(targetEntity, Parent.Descriptor, Parent.Spell, Parent.Item, Dir);

                    if (Dir <= Direction.Right && ShouldHook(targetEntity) && !Parent.HasGrappled)
                    {
                        HookEntity();
                    }

                    if (!Parent.Descriptor.PierceTarget)
                    {
                        if (targetPlayer.Map.ZoneType == MapZone.Safe ||
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
                if (targetResource.IsDead)
                {
                    if (!ProjectileDescriptor.IgnoreExhaustedResources)
                    {
                        return true;
                    }
                }
                else if (!ProjectileDescriptor.IgnoreActiveResources)
                {
                    Parent.Owner.TryAttack(targetResource, Parent.Descriptor, Parent.Spell, Parent.Item, Dir);

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
                    Parent.Owner.TryAttack(targetEntity, Parent.Descriptor, Parent.Spell, Parent.Item, Dir);

                    if (Dir <= Direction.Right && ShouldHook(targetEntity) && !Parent.HasGrappled)
                    {
                        HookEntity();
                    }

                    if (!Parent.Descriptor.PierceTarget)
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
            Player => ProjectileDescriptor.GrappleHookOptions.Contains(Enums.GrappleOption.Player),
            Npc => ProjectileDescriptor.GrappleHookOptions.Contains(Enums.GrappleOption.NPC),
            Resource => ProjectileDescriptor.GrappleHookOptions.Contains(Enums.GrappleOption.Resource),
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
            Parent.Owner, Distance, Parent.Owner.Dir, Parent.Descriptor.IgnoreMapBlocks,
            Parent.Descriptor.IgnoreActiveResources, Parent.Descriptor.IgnoreExhaustedResources,
            Parent.Descriptor.IgnoreZDimension
        );
    }

}
