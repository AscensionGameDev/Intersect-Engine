namespace Intersect.Models
{
    public interface IIndexedGameObject : IGameObject
    {
        int Index { get; }
    }
}
