using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToMeAction : AdminAction
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
        public override AdminActions Action { get; } = AdminActions.WarpToMe;

        [Key(2)]
        public string Name { get; set; }

    }

}
