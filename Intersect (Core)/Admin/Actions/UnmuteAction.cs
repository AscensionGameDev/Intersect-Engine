using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class UnmuteAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public UnmuteAction()
        {

        }

        public UnmuteAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.UnMute;

        [Key(2)]
        public string Name { get; set; }

    }

}
