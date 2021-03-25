using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class KickAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public KickAction() : base(AdminActions.Kick)
        {

        }

        public KickAction(string name) : base(AdminActions.Kick)
        {
            Name = name;
        }

        [Key(1)]
        public string Name { get; set; }

    }

}
