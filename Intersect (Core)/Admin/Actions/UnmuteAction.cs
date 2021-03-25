using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class UnmuteAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public UnmuteAction() : base(AdminActions.UnMute)
        {

        }

        public UnmuteAction(string name) : base(AdminActions.UnMute)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
