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
            var n = 0;
            en.MoveTimer = 0;
            Range = 0;
            for (var i = 1; i <= range; i++)
            {
                n = en.CanMove(Direction);
                if (n == -5) //Check for out of bounds
                {
                    return;
                } //Check for blocks

                if (n == -2 && blockPass == false)
                {
                    return;
                } //Check for ZDimensionTiles

                if (n == -3 && zdimensionPass == false)
                {
                    return;
                } //Check for active resources

                if (n == (int) EntityType.Resource && activeResourcePass == false)
                {
                    return;
                } //Check for dead resources

                if (n == (int) EntityType.Resource && deadResourcePass == false)
                {
                    return;
                } //Check for players and solid events

                if (n == (int) EntityType.Player || n == (int) EntityType.Event)
                {
                    return;
                }

                en.Move(Direction, null, true);
                en.Dir = Facing;

                Range = i;
            }
        }

    }

}
