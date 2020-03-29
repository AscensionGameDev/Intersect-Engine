namespace Intersect.GameObjects
{

    public interface IGameObject<out TKey, TValue> where TValue : IGameObject<TKey, TValue>
    {

        TKey Id { get; }

    }

}
