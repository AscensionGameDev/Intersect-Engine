using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class KillAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public KillAction() : base(AdminActions.Kill)
        {

        }

        public KillAction(string name) : base(AdminActions.Kill)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
