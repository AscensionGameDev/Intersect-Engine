using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.GameObjects.Events
{
    public class MoveRouteAction
    {
        public MoveRouteEnum Type;
        public EventGraphic Graphic = null;
        public int AnimationIndex = -1;


        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int)Type);
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic.Save(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                myBuffer.WriteInteger(AnimationIndex);
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (MoveRouteEnum)myBuffer.ReadInteger();
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic = new EventGraphic();
                Graphic.Load(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                AnimationIndex = myBuffer.ReadInteger();
            }
        }

        public MoveRouteAction Copy()
        {
            MoveRouteAction copy = new MoveRouteAction()
            {
                Type = Type
            };
            if (Type == MoveRouteEnum.SetGraphic)
            {
                copy.Graphic = new EventGraphic();
                copy.Graphic.CopyFrom(Graphic);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                copy.AnimationIndex = AnimationIndex;
            }
            return copy;
        }
    }
}
