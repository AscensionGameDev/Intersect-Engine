using System;
using Intersect.Enums;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities.Combat
{

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
            bool zdimensionPass = false
        )
        {
            en.MoveTimer = 0;
            Range = 0;
            for (var i = 1; i <= range; i++)
            {
                if (!en.CanMoveInDirection(Direction, out var blockerType, out var entityType))
                {
                    switch (blockerType)
                    {
                        case MovementBlockerType.OutOfBounds:
                            return;

                        case MovementBlockerType.MapAttribute:
                            if (blockPass)
                            {
                                break;
                            }

                            return;

                        case MovementBlockerType.ZDimension:
                            if (zdimensionPass)
                            {
                                break;
                            }

                            return;

                        case MovementBlockerType.Entity:
                            switch (entityType)
                            {
                                case EntityType.Resource when activeResourcePass == false:
                                case EntityType.Resource when deadResourcePass == false:
                                case EntityType.Event:
                                case EntityType.Player:
                                case EntityType.GlobalEntity:
                                    return;
                                case EntityType.Projectile:
                                    break;
                                default:
                                    throw new InvalidOperationException(
                                        "Unreachable"); // TODO: Change to \UnreachableException` in .NET 7`
                            }

                            break;

                        case MovementBlockerType.NotBlocked:
                        case MovementBlockerType.Slide:
                            break;

                        default:
                            throw
                                new InvalidOperationException(
                                    "Unreachable"); // TODO: Change to \UnreachableException` in .NET 7`
                    }
                }

                en.Move(Direction, null, true);
                en.Dir = Facing;

                Range = i;
            }
        }

    }

}
