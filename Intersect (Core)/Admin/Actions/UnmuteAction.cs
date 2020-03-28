using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class UnmuteAction : AdminAction
    {

        public UnmuteAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.UnMute;

        public string Name { get; set; }

    }

}
