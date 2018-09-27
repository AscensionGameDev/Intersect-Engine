namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects
{
    public interface IGameObject<out TKey, TValue> where TValue : IGameObject<TKey, TValue>
    {
        TKey Id { get; }
    }
}