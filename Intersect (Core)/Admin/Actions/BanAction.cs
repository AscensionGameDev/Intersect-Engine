using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class BanAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public BanAction()
        {

        }

        public BanAction(string name, int durationDays, string reason, bool banIp)
        {
            Name = name;
            DurationDays = durationDays;
            Reason = reason;
            BanIp = banIp;
        }

        [Key(1)]
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.Ban;

        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public int DurationDays { get; set; }

        [Key(4)]
        public string Reason { get; set; }

        [Key(5)]
        public bool BanIp { get; set; }

    }

}
