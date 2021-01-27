using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Intersect.Logging;
using Intersect.Models;
using Intersect.Utilities;

namespace Intersect.Collections
{

    public class DatabaseObjectLookup : IGameObjectLookup<IDatabaseObject>
    {

        private readonly SortedDictionary<Guid, IDatabaseObject> mIdMap;

        private Dictionary<Guid, IDatabaseObject> mCachedClone;

        private bool mIsDirty = true;

        private readonly object mLock;

        public DatabaseObjectLookup(Type storedType)
        {
            mLock = new object();
            mIdMap = new SortedDictionary<Guid, IDatabaseObject>();

            StoredType = storedType;
        }

        public Type StoredType { get; }

        public virtual IDatabaseObject this[Guid id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        public List<Guid> KeyList
        {
            get
            {
                lock (mLock)
                {
                    return mIdMap.Keys.ToList();
                }
            }
        }

        public List<IDatabaseObject> ValueList
        {
            get
            {
                lock (mLock)
                {
                    try
                    {
                        return mIdMap.Values.OrderBy(databaseObject => databaseObject?.TimeCreated).ToList();
                    }
                    catch (Exception exception)
                    {
                        Log.Warn(
                            exception,
                            $@"{StoredType.Name}[Count={mIdMap.Count},NullCount={mIdMap.Count(pair => pair.Value == null)}]"
                        );

                        throw;
                    }
                }
            }
        }

        public Type IndexKeyType => typeof(int);

        public bool IsEmpty => Count < 1;

        public Type KeyType => typeof(Guid);

        public Type ValueType => typeof(IDatabaseObject);

        public virtual int Count
        {
            get
            {
                lock (mLock)
                {
                    return mIdMap.Count;
                }
            }
        }

        public virtual IDictionary<Guid, IDatabaseObject> Clone
        {
            get
            {
                if (mIsDirty || mCachedClone == null)
                {
                    lock (mLock)
                    {
                        mCachedClone = mIdMap.ToDictionary(pair => pair.Key, pair => pair.Value);
                        mIsDirty = false;
                    }
                }
                return mCachedClone;
            }
        }

        public virtual ICollection<KeyValuePair<Guid, IDatabaseObject>> Pairs => Clone;

        public virtual ICollection<Guid> Keys
        {
            get
            {
                lock (mLock)
                {
                    return mIdMap.Keys;
                }
            }
        }

        public virtual ICollection<IDatabaseObject> Values => ValueList;

        public virtual IDatabaseObject Get(Guid id)
        {
            return TryGetValue(id, out var value) ? value : default(IDatabaseObject);
        }

        public virtual TObject Get<TObject>(Guid id) where TObject : IDatabaseObject
        {
            return TryGetValue(id, out TObject value) ? value : default(TObject);
        }

        public virtual bool TryGetValue<TObject>(Guid id, out TObject value) where TObject : IDatabaseObject
        {
            if (TryGetValue(id, out var baseObject))
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

            lock (mLock)
            {
                return mIdMap.TryGetValue(id, out value);
            }
        }

        public bool Add(IDatabaseObject value)
        {
            return InternalSet(value, false);
        }

        public IDatabaseObject AddNew(Type type, Guid id)
        {
            var idConstructor = type.GetConstructor(new[] {KeyType});
            if (idConstructor == null)
            {
                throw new ArgumentNullException(nameof(idConstructor), MessageNoConstructor(type, KeyType.Name));
            }

            var value = (IDatabaseObject) idConstructor.Invoke(new object[] {id});
            if (value == null)
            {
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType.Name}' with the ({KeyType.Name}) constructor."
                );
            }

            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        public virtual bool Set(Guid key, IDatabaseObject value)
        {
            if (key != (value?.Id ?? Guid.Empty))
            {
                throw new ArgumentException(@"Key does not match the Guid of the value.", nameof(key));
            }

            return InternalSet(value, true);
        }

        public virtual bool Delete(IDatabaseObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsIdValid(value.Id))
            {
                throw new ArgumentOutOfRangeException();
            }

            lock (mLock)
            {
                mIsDirty = true;
                return mIdMap.Remove(value.Id);
            }
        }

        public virtual bool DeleteAt(Guid guid)
        {
            if (guid == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsIdValid(guid))
            {
                throw new ArgumentOutOfRangeException();
            }

            IDatabaseObject obj;

            lock (mLock)
            {
                if (!mIdMap.TryGetValue(guid, out obj))
                {
                    return false;
                }
            }

            return Delete(obj);
        }

        public virtual void Clear()
        {
            lock (mLock)
            {
                mIsDirty = true;
                mIdMap.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<KeyValuePair<Guid, IDatabaseObject>> GetEnumerator()
        {
            return Clone.GetEnumerator();
        }

        public IDatabaseObject AddNew(Type type, int index)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), @"No type specified.");
            }

            var mixedConstructor = type.GetConstructor(new[] {KeyType, IndexKeyType});
            if (mixedConstructor != null)
            {
                return AddNew(type, Guid.NewGuid(), index);
            }

            var indexConstructor = type.GetConstructor(new[] {IndexKeyType});
            if (indexConstructor == null)
            {
                throw new ArgumentNullException(
                    nameof(indexConstructor), MessageNoConstructor(type, IndexKeyType.Name)
                );
            }

            var value = (IDatabaseObject) indexConstructor.Invoke(new object[] {index});
            if (value == null)
            {
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType.Name}' with the ({IndexKeyType.Name}) constructor."
                );
            }

            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        public IDatabaseObject AddNew(Type type, Guid id, int index)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), @"No type specified.");
            }

            var mixedConstructor = ValueType.GetConstructor(new[] {KeyType, IndexKeyType});
            if (mixedConstructor == null)
            {
                throw new ArgumentNullException(
                    nameof(mixedConstructor), MessageNoConstructor(type, KeyType.Name, IndexKeyType.Name)
                );
            }

            var value = (IDatabaseObject) mixedConstructor.Invoke(new object[] {id, index});
            if (value == null)
            {
                throw new ArgumentNullException(
                    $"Failed to create instance of '{ValueType.Name}' with the ({KeyType.Name}, {IndexKeyType.Name}) constructor."
                );
            }

            return InternalSet(value, false) ? value : default(IDatabaseObject);
        }

        protected virtual bool IsIdValid(Guid id)
        {
            return id != Guid.Empty;
        }

        protected virtual bool IsIndexValid(int index)
        {
            return index > -1;
        }

        internal virtual bool InternalSet(IDatabaseObject value, bool overwrite)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!IsIdValid(value.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(value.Id));
            }

            lock (mLock)
            {
                if (!overwrite && mIdMap.ContainsKey(value.Id))
                {
                    return false;
                }

                mIsDirty = true;
                mIdMap[value.Id] = value;

                return true;
            }
        }

        private static string MessageNoConstructor(Type type, params string[] constructorMessage)
        {
            var joinedConstructorMessage = string.Join(",", constructorMessage ?? new string[] { });
            var builder = new StringBuilder();
            builder.AppendLine($@"No ({joinedConstructorMessage}) constructor for type '{type?.Name}'.");
            builder.AppendLine(ReflectionUtils.StringifyConstructors(type));

            return builder.ToString();
        }

    }

}
