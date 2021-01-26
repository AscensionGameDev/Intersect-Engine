using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class KillAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public KillAction()
        {

        }

        public KillAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.Kill;

        [Key(2)]
        public string Name { get; set; }

    }

}
