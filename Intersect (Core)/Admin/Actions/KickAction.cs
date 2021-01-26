using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class KickAction : AdminAction
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
        public override AdminActions Action { get; } = AdminActions.Kick;

        [Key(2)]
        public string Name { get; set; }

    }

}
