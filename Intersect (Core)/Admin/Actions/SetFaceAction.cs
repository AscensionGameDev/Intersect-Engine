using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class SetFaceAction : AdminAction
    {

        public SetFaceAction(string name, string face)
        {
            Name = name;
            Face = face;
        }

        public override AdminActions Action { get; } = AdminActions.SetFace;

        public string Name { get; set; }

        public string Face { get; set; }

    }

}
