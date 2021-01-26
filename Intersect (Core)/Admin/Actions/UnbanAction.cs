using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class UnbanAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public UnbanAction()
        {

        }

        public UnbanAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.UnBan;

        [Key(2)]
        public string Name { get; set; }

    }

}
