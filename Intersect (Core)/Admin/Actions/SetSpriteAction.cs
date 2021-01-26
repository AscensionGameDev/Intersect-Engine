using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class SetSpriteAction : AdminAction
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
        public override AdminActions Action { get; } = AdminActions.SetSprite;

        [Key(2)]
        public string Name { get; set; }

        [Key(3)]
        public string Sprite { get; set; }

    }

}
