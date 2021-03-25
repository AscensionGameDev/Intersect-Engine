using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToMeAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToMeAction() : base(AdminActions.WarpToMe)
        {

        }

        public WarpToMeAction(string name) : base(AdminActions.WarpToMe)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
