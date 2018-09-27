namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects
{
    public interface IGameObject<out TKey, TValue> where TValue : IGameObject<TKey, TValue>
    {
        TKey Index { get; }
    }
}