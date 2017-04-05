using Intersect.Collections;
using Intersect.Enums;
using Intersect.Models;
using System;
using System.Linq;

namespace Intersect
{
    public abstract class DatabaseObject<TObject> : IDatabaseObject where TObject : DatabaseObject<TObject>
    {
        public static DatabaseObjectLookup Lookup => Models.LookupUtils.GetLookup(typeof(TObject));

        public GameObjectType Type => Models.LookupUtils.GetGameObjectType(typeof(TObject));

        public string DatabaseTable => Type.GetTable();

        private byte[] mBackup;

        protected DatabaseObject(int index) : this(Guid.NewGuid(), index)
        {
        }

        protected DatabaseObject(Guid id) : this(id, Lookup?.NextIndex ?? -1)
        {
        }

        protected DatabaseObject(Guid id, int index)
        {
            //if (index < 0)throw new ArgumentOutOfRangeException();

            Guid = id;
            Index = index;
        }

        public Guid Guid { get; }
        public int Index { get; }
        public string Name { get; set; }

        public abstract void Load(byte[] packet);

        public void MakeBackup() => mBackup = BinaryData;
        public void DeleteBackup() => mBackup = null;
        public void RestoreBackup()
        {
            if (mBackup != null)
            {
                Load(mBackup);
            }
        }

        public abstract byte[] BinaryData { get; }
        public virtual void Delete() => Lookup?.Delete((TObject)this);

        public static string GetName(int index) =>
            Lookup?.Get(index)?.Name ?? "ERR_DELETED";

        public static string[] GetNameList() =>
            Lookup?.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();
    }
}
