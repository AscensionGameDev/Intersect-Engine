using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class BanAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public BanAction() : base(AdminActions.Ban)
        {

        }

        public BanAction(string name, int durationDays, string reason, bool banIp) : base(AdminActions.Ban)
        {
            Name = name;
            DurationDays = durationDays;
            Reason = reason;
            BanIp = banIp;
        }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public int DurationDays { get; set; }

        [Key(3)]
        public string Reason { get; set; }

        [Key(4)]
        public bool BanIp { get; set; }

    }

}
