using Intersect.Core;
using Intersect.Enums;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Entities.Combat;


public partial class Dash
{

    public Direction Direction;

    public Direction Facing;

    public int Range;

    public Dash(
        Entity en,
        int range,
        Direction direction,
        bool blockPass = false,
        bool activeResourcePass = false,
        bool deadResourcePass = false,
        bool zdimensionPass = false
    )
    {
        Direction = direction;
        Facing = en.Dir;

        CalculateRange(en, range, blockPass, activeResourcePass, deadResourcePass, zdimensionPass);
        if (Range <= 0)
        {
            return;
        } //Remove dash instance if no where to dash

        PacketSender.SendEntityDash(
            en, en.MapId, (byte) en.X, (byte) en.Y, (int) (Options.MaxDashSpeed * (Range / 10f)),
            Direction == Facing ? Direction : Direction.None
        );

        en.MoveTimer = Timing.Global.Milliseconds + Options.MaxDashSpeed;
    }

    public void CalculateRange(
        Entity en,
        int range,
        bool blockPass = false,
        bool activeResourcePass = false,
        bool deadResourcePass = false,
        bool zDimensionPass = false
    )
    {
        Range = 0;
        if (en == default)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                new ArgumentNullException(
                    nameof(en),
                    "Entity was null when calling CalcuateRange(), this isn't supported."
                ),
                "Error calculating range"
            );
            return;
        }

        en.MoveTimer = 0;
        for (var i = 1; i <= range; i++)
        {
            if (!en.CanMoveInDirection(Direction, out var blockerType, out var blockingEntityType))
            {
                switch (blockerType)
                {
                    case MovementBlockerType.OutOfBounds:
                        return;

                    case MovementBlockerType.MapAttribute:
                        if (!blockPass)
                        {
                            return;
                        }

                        break;

                    case MovementBlockerType.ZDimension:
                        if (!zDimensionPass)
                        {
                            return;
                        }

                        break;

                    case MovementBlockerType.Entity:
                        switch (blockingEntityType)
                        {
                            case EntityType.Resource:
                                if (activeResourcePass || deadResourcePass)
                                {
                                    break;
                                }

                                return;

                            case EntityType.Event:
                            case EntityType.Player:
                                return;

                            case EntityType.GlobalEntity:
                            case EntityType.Projectile:
                                break;

                            default:
                                throw new NotImplementedException($"{blockingEntityType} not implemented.");
                        }

                        break;

                    case MovementBlockerType.NotBlocked:
                    case MovementBlockerType.Slide:
                        break;

                    default:
                        throw new NotImplementedException($"{blockerType} not implemented.");
                }
            }

            en.Move(Direction, null, true);
            en.Dir = Facing;

            Range = i;
        }
    }

}
