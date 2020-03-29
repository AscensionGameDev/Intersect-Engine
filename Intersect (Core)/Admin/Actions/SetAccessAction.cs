using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class SetAccessAction : AdminAction
    {

        public SetAccessAction(string name, string power)
        {
            Name = name;
            Power = power;
        }

        public override AdminActions Action { get; } = AdminActions.SetAccess;

        public string Name { get; set; }

        public string Power { get; set; }

    }

}
