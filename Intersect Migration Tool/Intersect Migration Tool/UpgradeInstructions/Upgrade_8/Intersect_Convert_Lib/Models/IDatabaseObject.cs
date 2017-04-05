using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Enums;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Models
{
    public interface IDatabaseObject : IIndexedGameObject
    {
        GameObjectType Type { get; }
        string DatabaseTable { get; }
        
        string Name { get; set; }

        void Load(byte[] packet);
        byte[] BinaryData { get; }

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }
}