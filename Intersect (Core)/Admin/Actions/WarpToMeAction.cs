using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class WarpToMeAction : AdminAction
    {

        public WarpToMeAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.WarpToMe;

        public string Name { get; set; }

    }

}
