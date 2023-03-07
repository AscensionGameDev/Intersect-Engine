using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class SetAccessAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetAccessAction()
        {

        }

        public SetAccessAction(string name, string power)
        {
            Name = name;
            Power = power;
        }

        [Key(1)]
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.SetAccess;

        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public string Power { get; set; }

    }

}
