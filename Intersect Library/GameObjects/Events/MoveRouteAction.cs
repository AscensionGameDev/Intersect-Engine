using System;

namespace Intersect.GameObjects.Events
{
    public class MoveRouteAction
    {
        public Guid AnimationId;
        public EventGraphic Graphic;
        public MoveRouteEnum Type;

        public void Save(ByteBuffer myBuffer)
        {
            myBuffer.WriteInteger((int) Type);
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic.Save(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                myBuffer.WriteGuid(AnimationId);
            }
        }

        public void Load(ByteBuffer myBuffer)
        {
            Type = (MoveRouteEnum) myBuffer.ReadInteger();
            if (Type == MoveRouteEnum.SetGraphic)
            {
                Graphic = new EventGraphic();
                Graphic.Load(myBuffer);
            }
            else if (Type == MoveRouteEnum.SetAnimation)
            {
                AnimationId = myBuffer.ReadGuid();
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
                copy.AnimationId = AnimationId;
            }
            return copy;
        }
    }
}