namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Models
{
    public interface IIndexedGameObject : IGameObject
    {
        int Index { get; }
    }
}