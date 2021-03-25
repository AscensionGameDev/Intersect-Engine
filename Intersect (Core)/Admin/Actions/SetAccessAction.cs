using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class SetAccessAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetAccessAction() : base(AdminActions.SetAccess)
        {

        }

        public SetAccessAction(string name, string power) : base(AdminActions.SetAccess)
        {
            Name = name;
            Power = power;
        }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public string Power { get; set; }

    }

}
