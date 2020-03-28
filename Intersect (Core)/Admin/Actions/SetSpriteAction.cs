using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class SetSpriteAction : AdminAction
    {

        public SetSpriteAction(string name, string sprite)
        {
            Name = name;
            Sprite = sprite;
        }

        public override AdminActions Action { get; } = AdminActions.SetSprite;

        public string Name { get; set; }

        public string Sprite { get; set; }

    }

}
