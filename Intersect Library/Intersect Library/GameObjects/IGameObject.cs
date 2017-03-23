namespace Intersect.GameObjects
{
    public interface IGameObject<K, V> where V : IGameObject<K, V>
    {
        K Id { get; }
    }
}