using Intersect.Models;
using Intersect.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.Collections
{
    public class DatabaseObjectLookup : IGameObjectLookup<IDatabaseObject>
    {
        [NotNull] private readonly SortedDictionary<Guid, IDatabaseObject> mIdMap;
        [NotNull] private readonly object mLock;

        public DatabaseObjectLookup()
        {
            mLock = new object();

            mIdMap = new SortedDictionary<Guid, IDatabaseObject>();
        }

        [NotNull]
        public virtual string[] Names => this.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();

        public virtual Guid FromList(int listIndex) => listIndex < 0 ? Guid.Empty : listIndex > Keys.Count ? Guid.Empty : KeyList[listIndex];

        public virtual int ListIndex(Guid id)
        {
            var index = Keys.ToList().IndexOf(id);
            return index;
        }


        public virtual IDatabaseObject this[Guid id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        [NotNull]
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

        [NotNull]
        public List<IDatabaseObject> ValueList
        {
            get
            {
                lock (mLock)
                {
                    return mIdMap.Values.ToList();
                }
            }
        }
        
        [NotNull] public Type KeyType => typeof(Guid);

        [NotNull] public Type IndexKeyType => typeof(int);

        [NotNull] public Type ValueType => typeof(IDatabaseObject);

        public virtual int Count => mIdMap.Count;

        [NotNull]
        public virtual IDictionary<Guid, IDatabaseObject> Clone
        {
            get
            {
                lock (mLock)
                {
                    return mIdMap.ToDictionary(pair => pair.Key, pair => pair.Value);
                }
            }
        }

        [NotNull] public virtual ICollection<KeyValuePair<Guid, IDatabaseObject>> Pairs => Clone;
        [NotNull] public virtual ICollection<Guid> Keys => mIdMap.Keys;
        [NotNull] public virtual ICollection<IDatabaseObject> Values => mIdMap.Values;

        public bool IsEmpty => Count < 1;

        public virtual IDatabaseObject Get(Guid id) => TryGetValue(id, out IDatabaseObject value)
            ? value
            : default(IDatabaseObject);

        public virtual TObject Get<TObject>(Guid id)
            where TObject : IDatabaseObject => TryGetValue(id, out TObject value) ? value : default(TObject);

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

            lock (mLock)
            {
                return mIdMap.TryGetValue(id, out value);
            }
        }

        public bool Add(IDatabaseObject value) => InternalSet(value, false);

        public IDatabaseObject AddNew(Type type, Guid id)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), @"No type specified.");

            var mixedConstructor = type.GetConstructor(new[] {KeyType, IndexKeyType});

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
            if (key != (value?.Id ?? Guid.Empty))
                throw new ArgumentException("Provided Guid does not match value.Guid.");
            return InternalSet(value, true);
        }

        public virtual bool Delete(IDatabaseObject value)
        {
            if (value == null) throw new ArgumentNullException();
            if (!IsIdValid(value.Id)) throw new ArgumentOutOfRangeException();

            lock (mLock)
            {
                return mIdMap.Remove(value.Id);
            }
        }

        public virtual bool DeleteAt(Guid guid)
        {
            if (guid == null) throw new ArgumentNullException();
            if (!IsIdValid(guid)) throw new ArgumentOutOfRangeException();

            IDatabaseObject obj;

            lock (mLock)
            {
                if (!mIdMap.TryGetValue(guid, out obj)) return false;
            }

            return Delete(obj);
        }

        public virtual void Clear()
        {
            lock (mLock)
            {
                mIdMap.Clear();
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
            if (!IsIdValid(value.Id)) throw new ArgumentOutOfRangeException(nameof(value.Id));

            lock (mLock)
            {
                mIdMap.TryGetValue(value.Id, out IDatabaseObject gameObject);

                if (!overwrite)
                {
                    if (mIdMap.ContainsKey(value.Id)) return false;
                }

                mIdMap[value.Id] = value;
                return true;
            }
        }

        private static string MessageNoConstructor(Type type, params string[] constructorMessage)
        {
            var joinedConstructorMessage = string.Join(",", constructorMessage ?? new string[] { });
            var builder = new StringBuilder();
            builder.AppendLine(
                $@"No ({joinedConstructorMessage}) constructor for type '{type?.Name}'.");
            builder.AppendLine(ReflectionUtils.StringifyConstructors(type));
            return builder.ToString();
        }
    }
}