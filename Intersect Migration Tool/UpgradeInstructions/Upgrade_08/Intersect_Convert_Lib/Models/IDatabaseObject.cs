using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Enums;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Models
{
    public interface IDatabaseObject : IIndexedGameObject
    {
        GameObjectType Type { get; }
        string DatabaseTable { get; }

        string Name { get; set; }
        byte[] BinaryData { get; }

        void Load(byte[] packet);

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }
}