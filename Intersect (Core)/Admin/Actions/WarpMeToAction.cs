using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpMeToAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpMeToAction()
        {

        }

        public WarpMeToAction(string name)
        {
            Name = name;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.WarpMeTo;

        [Key(2)]
        public string Name { get; set; }

    }

}
