using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Collections;

namespace Intersect.GameObjects
{
    public interface DatabaseObject
    {
        int Id { get; }
        string Name { get; set; }

        void Load(byte[] packet);
        byte[] BinaryData { get; }
        string DatabaseTableName { get; }
        GameObject GameObjectType { get; }

        void MakeBackup();
        void RestoreBackup();
        void DeleteBackup();

        void Delete();
    }

    public abstract class DatabaseObject<TObject>
        : DatabaseObject, IGameObject<int, TObject>
        where TObject : DatabaseObject<TObject>
    {
        private static IntObjectLookup<TObject> sLookup;

        public static IntObjectLookup<TObject> Lookup =>
            (sLookup = (sLookup ?? new IntObjectLookup<TObject>()));

        public const string DATABASE_TABLE = "";
        public const GameObject OBJECT_TYPE = GameObject.Animation;

        private byte[] mBackup = null;

        protected DatabaseObject(int id)
        {
            Id = id;
        }

        public int Id { get; }
        public string Name { get; set; }

        public abstract void Load(byte[] packet);

        public virtual void MakeBackup() => mBackup = BinaryData;
        public virtual void DeleteBackup() => mBackup = null;
        public virtual void RestoreBackup()
        {
            if (mBackup != null)
            {
                Load(mBackup);
            }
        }

        public abstract byte[] BinaryData { get; }
        public abstract GameObject GameObjectType { get; }
        public abstract string DatabaseTableName { get; }
        public virtual void Delete() => Lookup.Delete((TObject)this);

        public static string GetName(int index) =>
            Lookup.Get(index)?.Name ?? "Deleted";

        public static string[] GetNameList() =>
            Lookup.Select(pair => pair.Value.Name).ToArray();

        public static int GetIdFromListIndex(int index) =>
            (index < 0 || Lookup.Count < index)
            ? -1 : Lookup.Keys.ToList()[index];

        public static int GetIndexFromId(int id) =>
            Lookup.Keys.ToList().IndexOf(id);
    }
}