using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class SetSpriteAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetSpriteAction()
        {

        }

        public SetSpriteAction(string name, string sprite)
        {
            Name = name;
            Sprite = sprite;
        }

        [Key(1)]
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.SetSprite;

        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public string Sprite { get; set; }

    }

}
