using System;
using System.Collections.Generic;

using Intersect.Enums;

namespace Intersect.Models
{

    public interface IDatabaseObject : INamedObject
    {

        GameObjectType Type { get; }

        string DatabaseTable { get; }

        long TimeCreated { get; set; }

        string JsonData { get; }

        void Load(string json, bool keepCreationTime = false);

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
                list.Add((T) LookupUtils.LookupMap[typeof(T)].Get(l));
            }

            return list;
        }

        public T Get(Guid id)
        {
            return (T) LookupUtils.LookupMap[typeof(T)].Get(id);
        }

    }

}
