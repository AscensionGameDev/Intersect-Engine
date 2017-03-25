using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect
{
    public abstract class DatabaseObject<TObject>
        : IDatabaseObject, IGameObject<int, TObject>
        where TObject : DatabaseObject<TObject>
    {
        private static IntObjectLookup<TObject> sLookup;
        private static Dictionary<Type, GameObjectType> sEnumMap;

        public static IntObjectLookup<TObject> Lookup =>
            (sLookup = (sLookup ?? new IntObjectLookup<TObject>()));

        private static Dictionary<Type, GameObjectType> EnumMap =>
            (sEnumMap = (sEnumMap ?? new Dictionary<Type, GameObjectType>()));

        public GameObjectType Type
        {
            get
            {
                if (EnumMap.ContainsKey(typeof(TObject)))
                    return EnumMap[typeof(TObject)];

                var values = Enum.GetValues(typeof(GameObjectType));
                foreach (GameObjectType gameObjectType in values)
                {
                    if (typeof(TObject) != gameObjectType.GetObjectType())
                        continue;

                    EnumMap[typeof(TObject)] = gameObjectType;
                    return gameObjectType;
                }

                return default(GameObjectType);
            }
        }

        public string DatabaseTable => Type.GetTable();

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
