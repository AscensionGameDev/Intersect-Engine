namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Models
{
    public interface IIndexedGameObject : IGameObject
    {
        int Index { get; }
    }
}