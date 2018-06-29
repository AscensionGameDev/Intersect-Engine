using System;
using System.Collections.Generic;
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

    public class DbList<T> : List<int>
    {
        public List<T> GetAll()
        {
            var list = new List<T>();
            foreach (var l in ToArray())
            {
                list.Add((T)LookupUtils.LookupMap[typeof(T)][base[l]]);
            }
            return list;
        }

        public T Get(int index)
        {
            return (T)LookupUtils.LookupMap[typeof(T)][base[index]];
        }
    }

}