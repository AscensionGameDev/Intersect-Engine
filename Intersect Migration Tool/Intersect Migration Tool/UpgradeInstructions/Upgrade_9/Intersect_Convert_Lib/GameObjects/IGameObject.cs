namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.GameObjects
{
    public interface IGameObject<out TKey, TValue> where TValue : IGameObject<TKey, TValue>
    {
        TKey Index { get; }
    }
}