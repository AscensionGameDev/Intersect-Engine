using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class UnmuteAction : AdminAction
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
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.UnMute;

        [Key(2)]
        public string Name { get; set; }

    }

}
