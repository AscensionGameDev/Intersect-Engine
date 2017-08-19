using Intersect.Enums;

namespace Intersect.Models
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