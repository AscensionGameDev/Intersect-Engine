using Intersect.GameObjects;

namespace Intersect.Collections
{
    public class IntObjectLookup<TValue> : AbstractObjectLookup<int, TValue> where TValue : IGameObject<int, TValue>
    {
        protected override bool Validate(int key) => key > 0;
    }
}