using System;
using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Models
{
    public interface IDatabaseObject : IGameObject
    {
        GameObjectType Type { get; }
        string DatabaseTable { get; }

        string Name { get; set; }
        long TimeCreated { get; set; }
        string JsonData { get; }
        void Load(string json);

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }

    public class DbList<T> : List<Guid>
    {
        public List<T> GetAll()
        {
            var list = new List<T>();
            foreach (var l in ToArray())
            {
                list.Add((T)LookupUtils.LookupMap[typeof(T)].Get(l));
            }
            return list;
        }

        public T Get(Guid id)
        {
            return (T)LookupUtils.LookupMap[typeof(T)].Get(id);
        }
    }

}