using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class UnbanAction : AdminAction
    {

        public UnbanAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.UnBan;

        public string Name { get; set; }

    }

}
