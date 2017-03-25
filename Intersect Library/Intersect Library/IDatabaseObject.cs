using Intersect.Enums;

namespace Intersect
{
    public interface IDatabaseObject
    {
        GameObjectType Type { get; }
        string DatabaseTable { get; }

        int Id { get; }
        string Name { get; set; }

        void Load(byte[] packet);
        byte[] BinaryData { get; }

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }
}