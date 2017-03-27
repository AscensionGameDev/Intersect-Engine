using Intersect.Enums;

namespace Intersect.Models
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