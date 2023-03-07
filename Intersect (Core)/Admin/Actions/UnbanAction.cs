using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class UnbanAction : AdminAction
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
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.UnBan;

        [Key(2)]
        public string Name { get; set; }

    }

}
