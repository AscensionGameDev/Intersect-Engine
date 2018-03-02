using Intersect.Enums;

namespace Intersect.Models
{
    public interface IDatabaseObject : IIndexedGameObject
    {
        GameObjectType Type { get; }
        string DatabaseTable { get; }

        string Name { get; set; }
        string JsonData { get; }
        void Load(string json);

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }
}