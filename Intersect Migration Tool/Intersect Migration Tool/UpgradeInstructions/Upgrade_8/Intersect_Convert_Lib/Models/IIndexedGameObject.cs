namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Models
{
    public interface IIndexedGameObject : IGameObject
    {
        int Index { get; }
    }
}