using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class WarpToMeAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToMeAction()
        {

        }

        public WarpToMeAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.WarpToMe;

        [Key(2)]
        public string Name { get; set; }

    }

}
