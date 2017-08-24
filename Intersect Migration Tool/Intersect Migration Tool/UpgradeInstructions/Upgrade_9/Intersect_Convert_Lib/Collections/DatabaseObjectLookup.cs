using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Models;
using Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Utilities;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Collections
{
    public class DatabaseObjectLookup : IIndexLookup<IDatabaseObject>
    {
        private readonly Dictionary<Guid, IDatabaseObject> mIdMap;
        private readonly Dictionary<int, IDatabaseObject> mIndexMap;
        private readonly object mLock;

        public DatabaseObjectLookup()
        {
            mLock = new object();

            mIdMap = new Dictionary<Guid, IDatabaseObject>();
            mIndexMap = new Dictionary<int, IDatabaseObject>();
        }

        public string[] Names =>
            this.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();

        public virtual IDatabaseObject this[Guid id]
        {
            get { return Get(id); }
            set { Set(id, value); }
        }

        public virtual IDatabaseObject this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        public List<int> IndexList => IndexKeys?.ToList();
        public List<IDatabaseObject> ValueList => IndexValues?.ToList();

        public Type KeyType => typeof(Guid);
        public Type IndexKeyType => typeof(int);
        public Type ValueType => typeof(IDatabaseObject);

        public virtual int Count
        {
            get
            {
                if (mIdMap == null || mIndexMap == null)
                    throw new ArgumentNullException();

                if (mIdMap.Count != mIndexMap.Count)
                    throw new ArgumentOutOfRangeException();

                return mIdMap.Count;
            }
        }

        public virtual IDictionary<Guid, IDatabaseObject> Clone
        {
            get
            {
                if (mLock == null) throw new ArgumentNullException();
                lock (mLock)
                {
                    return mIdMap?.ToDictionary(pair => pair.Key, pair => pair.Value);
                }
            }
        }

        public virtual ICollection<KeyValuePair<Guid, IDatabaseObject>> Pairs => Clone;
        public virtual ICollection<Guid> Keys => mIdMap?.Keys;
        public virtual ICollection<IDatabaseObject> Values => mIdMap?.Values;

        public virtual int NextIndex
        {
            get
            {
                if (mLock == null) throw new ArgumentNullException();
                lock (mLock) return (mIndexMap?.Keys.Max() + 1) ?? -1;
            }
        }

        public IDictionary<int, IDatabaseObject> IndexClone
        {
            get
            {
                if (mLock == null) throw new ArgumentNullException();
                lock (mLock)
                {
                    return mIndexMap?.ToDictionary(pair => pair.Key, pair => pair.Value);
                }
            }
        }

        public ICollection<KeyValuePair<int, IDatabaseObject>> IndexPairs => IndexClone;
        public ICollection<int> IndexKeys => mIndexMap?.Keys;
        public ICollection<IDatabaseObject> IndexValues => mIndexMap?.Values;

        public virtual IDatabaseObject Get(Guid id) => TryGetValue(id, out IDatabaseObject value)
            ? value
            : default(IDatabaseObject);

        public virtual IDatabaseObject Get(int index) => TryGetValue(index, out IDatabaseObject value)
            ? value
            : default(IDatabaseObject);

        public virtual TObject Get<TObject>(Guid id)
            where TObject : IDatabaseObject => TryGetValue<TObject>(id, out TObject value) ? value : default(TObject);

        public virtual TObject Get<TObject>(int index)
            where TObject : IDatabaseObject => TryGetValue<TObject>(index, out TObject value)
            ? value
            : default(TObject);

        public virtual bool TryGetValue<TObject>(Guid id, out TObject value) where TObject : IDatabaseObject
        {
            if (TryGetValue(id, out IDatabaseObject baseObject))
            {
                value = (TObject) baseObject;
                return true;
            }

            value = default(TObject);
            return false;
        }

        public virtual bool TryGetValue(Guid id, out IDatabaseObject value)
        {
            if (!IsIdValid(id))
            {
                value = default(IDatabaseObject);
                return false;
            }

            if (mLock == null) throw new ArgumentNullException();
            if (mIdMap == null) throw new ArgumentNullException();

            lock (mLock)
            {
                return mIdMap.TryGetValue(id, out value);
            }
        }

        public virtual bool TryGetValue<TObject>(int index, out TObject value) where TObject : IDatabaseObject
        {
            if (TryGetValue(index, out IDatabaseObject baseObject))
            {
                value = (TObject) baseObject;
                return true;
            }

            value = default(TObject);
            return false;
        }

        public virtual bool TryGetValue(int index, out IDatabaseObject value)
        {
            if (!IsIndexValid(index))
            {
                value = default(IDatabaseObject);
                return false;
            }

            if (mLock == null) throw new ArgumentNullException();
            if (mIndexMap == null) throw new ArgumentNullException();

            lock (mLock)
            {
                return mIndexMap.TryGetValue(index, out value);
            }
        }

        public bool Add(IDatabaseObject value) => InternalSet(value, false);

        public IDatabaseObject AddNew(Type type, Guid id)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), @"No type specified.");

            var mixedConstructor = type.GetConstructor(new[] {KeyType, IndexKeyType});
            if (mixedConstructor != null) return AddNew(type, id, NextIndex);

            var idConstructor = type.GetConstructor(new[] {KeyType});
            if (idConstructor == null)
                throw new ArgumentNullException(nameof(idConstructor),
                    MessageNoConstructor(type, KeyType?.Name ?? @"<NULL_KT>"));

            var value = (IDatabaseObject) idConstructor.Invoke(new object[] {id});
            if (value == null)
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType?.Name}' with the ({KeyType?.Name ?? @"<NULL_KT>"}) constructor.");
            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        public IDatabaseObject AddNew(Type type, int index)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), @"No type specified.");

            var mixedConstructor = type.GetConstructor(new[] {KeyType, IndexKeyType});
            if (mixedConstructor != null) return AddNew(type, Guid.NewGuid(), index);

            var indexConstructor = type.GetConstructor(new[] {IndexKeyType});
            if (indexConstructor == null)
                throw new ArgumentNullException(nameof(indexConstructor),
                    MessageNoConstructor(type, IndexKeyType?.Name ?? @"<NULL_IKT>"));

            var value = (IDatabaseObject) indexConstructor.Invoke(new object[] {index});
            if (value == null)
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType?.Name}' with the ({IndexKeyType?.Name ?? @"<NULL_IKT>"}) constructor.");
            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        public IDatabaseObject AddNew(Type type, Guid id, int index)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), @"No type specified.");

            var mixedConstructor = ValueType?.GetConstructor(new[] {KeyType, IndexKeyType});
            if (mixedConstructor == null)
                throw new ArgumentNullException(nameof(mixedConstructor),
                    MessageNoConstructor(type, KeyType?.Name ?? @"<NULL_KT>", IndexKeyType?.Name ?? @"<NULL_IKT>"));

            var value = (IDatabaseObject) mixedConstructor.Invoke(new object[] {id, index});
            if (value == null)
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType?.Name}' with the ({KeyType?.Name ?? @"<NULL_KT>"}, {IndexKeyType?.Name ?? @"<NULL_IKT>"}) constructor.");
            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        public virtual bool Set(Guid key, IDatabaseObject value)
        {
            if (key != (value?.Guid ?? Guid.Empty))
                throw new ArgumentException("Provided Guid does not match value.Guid.");
            return InternalSet(value, true);
        }

        public virtual bool Set(int index, IDatabaseObject value)
        {
            if (index != (value?.Index ?? -1))
                throw new ArgumentException("Provided index does not match value.Index.");
            return InternalSet(value, true);
        }

        public virtual bool Delete(IDatabaseObject value)
        {
            if (value == null) throw new ArgumentNullException();
            if (!IsIdValid(value.Guid)) throw new ArgumentOutOfRangeException();
            if (!IsIndexValid(value.Index)) throw new ArgumentOutOfRangeException();

            if (mLock == null) throw new ArgumentNullException();
            if (mIdMap == null) throw new ArgumentNullException();
            if (mIndexMap == null) throw new ArgumentNullException();

            lock (mLock)
            {
                return mIdMap.Remove(value.Guid) && mIndexMap.Remove(value.Index);
            }
        }

        public virtual bool DeleteAt(Guid guid)
        {
            if (guid == null) throw new ArgumentNullException();
            if (!IsIdValid(guid)) throw new ArgumentOutOfRangeException();

            if (mLock == null) throw new ArgumentNullException();
            if (mIdMap == null) throw new ArgumentNullException();

            IDatabaseObject obj;

            lock (mLock)
            {
                if (!mIdMap.TryGetValue(guid, out obj)) return false;
            }

            return Delete(obj);
        }

        public virtual bool DeleteAt(int index)
        {
            if (!IsIndexValid(index)) throw new ArgumentOutOfRangeException();

            if (mLock == null) throw new ArgumentNullException();
            if (mIndexMap == null) throw new ArgumentNullException();

            IDatabaseObject obj;

            lock (mLock)
            {
                if (!mIndexMap.TryGetValue(index, out obj)) return false;
            }

            return Delete(obj);
        }

        public virtual void Clear()
        {
            if (mLock == null) throw new ArgumentNullException();
            if (mIdMap == null) throw new ArgumentNullException();
            if (mIndexMap == null) throw new ArgumentNullException();

            lock (mLock)
            {
                mIdMap.Clear();
                mIndexMap.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual IEnumerator<KeyValuePair<Guid, IDatabaseObject>> GetEnumerator()
        {
            if (Clone != null) return Clone.GetEnumerator();
            throw new ArgumentNullException();
        }

        protected virtual bool IsIdValid(Guid id) => (id != Guid.Empty);
        protected virtual bool IsIndexValid(int index) => (index > -1);

        internal virtual bool InternalSet(IDatabaseObject value, bool overwrite)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!IsIdValid(value.Guid)) throw new ArgumentOutOfRangeException(nameof(value.Guid));
            if (!IsIndexValid(value.Index)) throw new ArgumentOutOfRangeException(nameof(value.Index));

            if (mLock == null) throw new ArgumentNullException(nameof(mLock));
            if (mIdMap == null) throw new ArgumentNullException(nameof(mIdMap));
            if (mIndexMap == null) throw new ArgumentNullException(nameof(mIndexMap));

            lock (mLock)
            {
                if (!overwrite)
                {
                    if (mIdMap.ContainsKey(value.Guid)) return false;
                    if (mIndexMap.ContainsKey(value.Index)) return false;
                }
                else if (mIdMap.ContainsKey(value.Guid))
                {
                    mIndexMap.Remove(mIdMap[value.Guid].Index);
                }
                else if (mIndexMap.ContainsKey(value.Index))
                {
                    mIdMap.Remove(mIndexMap[value.Index].Guid);
                }

                mIdMap[value.Guid] = value;
                mIndexMap[value.Index] = value;
                return true;
            }
        }

        private string MessageNoConstructor(Type type, params string[] constructorMessage)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                $@"No ({
                        string.Join(",", constructorMessage ?? new string[] { })
                    }) constructor for type '{type?.Name}'.");
            builder.AppendLine(ReflectionUtils.StringifyConstructors(type));
            return builder.ToString();
        }

        public virtual IEnumerator<KeyValuePair<int, IDatabaseObject>> GetIndexEnumerator()
        {
            if (IndexClone != null) return IndexClone.GetEnumerator();
            throw new ArgumentNullException();
        }
    }
}