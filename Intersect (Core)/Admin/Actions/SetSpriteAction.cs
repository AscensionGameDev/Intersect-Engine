using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class SetSpriteAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public SetSpriteAction() : base(AdminActions.SetSprite)
        {

        }

        public SetSpriteAction(string name, string sprite) : base(AdminActions.SetSprite)
        {
            Name = name;
            Sprite = sprite;
        }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public string Sprite { get; set; }

    }

}
