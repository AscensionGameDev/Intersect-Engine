using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class KickAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public KickAction()
        {

        }

        public KickAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.Kick;

        [Key(2)]
        public string Name { get; set; }
    }
}
