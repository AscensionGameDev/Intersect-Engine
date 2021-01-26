using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class SetFaceAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetFaceAction()
        {

        }

        public SetFaceAction(string name, string face)
        {
            Name = name;
            Face = face;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.SetFace;

        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public string Face { get; set; }

    }

}
