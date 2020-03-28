using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class WarpMeToAction : AdminAction
    {

        public WarpMeToAction(string name)
        {
            Name = name;
        }

        public override AdminActions Action { get; } = AdminActions.WarpMeTo;

        public string Name { get; set; }

    }

}
