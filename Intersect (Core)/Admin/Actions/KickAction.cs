using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class KickAction : AdminAction
    {

        public KickAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.Kick;

        public string Name { get; set; }

    }

}
