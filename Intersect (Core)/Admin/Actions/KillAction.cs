using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class KillAction : AdminAction
    {

        public KillAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.Kill;

        public string Name { get; set; }

    }

}
