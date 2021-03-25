using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class SetFaceAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetFaceAction() : base(AdminActions.SetFace)
        {

        }

        public SetFaceAction(string name, string face) : base(AdminActions.SetFace)
        {
            Name = name;
            Face = face;
        }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public string Face { get; set; }

    }

}
