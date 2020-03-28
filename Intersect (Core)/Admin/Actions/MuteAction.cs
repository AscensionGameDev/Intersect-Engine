using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class MuteAction : AdminAction
    {

        public MuteAction(string name, int durationDays, string reason, bool banIp)
        {
            Name = name;
            DurationDays = durationDays;
            Reason = reason;
            BanIp = banIp;
        }

        public override AdminActions Action { get; } = AdminActions.Mute;

        public string Name { get; set; }

        public int DurationDays { get; set; }

        public string Reason { get; set; }

        public bool BanIp { get; set; }

    }

}
