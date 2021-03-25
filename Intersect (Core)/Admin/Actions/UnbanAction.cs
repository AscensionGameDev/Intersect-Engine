using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class UnbanAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public UnbanAction() : base(AdminActions.UnBan)
        {

        }

        public UnbanAction(string name) : base(AdminActions.UnBan)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
