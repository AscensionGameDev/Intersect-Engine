using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpMeToAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpMeToAction() : base(AdminActions.WarpMeTo)
        {

        }

        public WarpMeToAction(string name) : base(AdminActions.WarpMeTo)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
