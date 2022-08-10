using System.Collections;
using System.Text;

using Intersect.Logging;
using Intersect.Models;
using Intersect.Utilities;

namespace Intersect.Collections;

public partial class GuidLookup<TValue> : ILookup<Guid, TValue>
    where TValue : IWeaklyIdentifiedObject
{
    private readonly object _lock;
    private readonly SortedDictionary<Guid, TValue> _lookup;

    private Dictionary<Guid, TValue>? _cloneCache;
    private bool _dirty = true;

    public GuidLookup()
    {
        _lock = new object();
        _lookup = new SortedDictionary<Guid, TValue>();
    }

    public virtual Type StoredType => typeof(TValue);

    public virtual TValue this[Guid id]
    {
        get => Get(id);
        set => Set(id, value);
    }

    public List<Guid> KeyList
    {
        get
        {
            lock (_lock)
            {
                return _lookup.Keys.ToList();
            }
        }
    }

    public virtual List<TValue> ValueList
    {
        get
        {
            lock (_lock)
            {
                try
                {
                    return _lookup.Values.ToList();
                }
                catch (Exception exception)
                {
                    Log.Warn(
                        exception,
                        $@"{StoredType.Name}[Count={_lookup.Count},NullCount={_lookup.Count(pair => pair.Value == null)}]"
                    );

                    throw;
                }
            }
        }
    }

    public bool IsEmpty => Count < 1;

    public Type KeyType => typeof(Guid);

    public Type ValueType => typeof(TValue);

    public virtual int Count
    {
        get
        {
            lock (_lock)
            {
                return _lookup.Count;
            }
        }
    }

    public virtual IDictionary<Guid, TValue> Clone
    {
        get
        {
            if (_dirty || _cloneCache == null)
            {
                lock (_lock)
                {
                    _cloneCache = _lookup.ToDictionary(pair => pair.Key, pair => pair.Value);
                    _dirty = false;
                }
            }
            return _cloneCache;
        }
    }

    public virtual ICollection<KeyValuePair<Guid, TValue>> Pairs => Clone;

    public virtual ICollection<Guid> Keys
    {
        get
        {
            lock (_lock)
            {
                return _lookup.Keys;
            }
        }
    }

    public virtual ICollection<TValue> Values => ValueList;

    public virtual TValue Get(Guid id)
    {
        return TryGetValue(id, out var value) ? value : default;
    }

    public virtual TObject Get<TObject>(Guid id) where TObject : TValue
    {
        return TryGetValue(id, out TObject value) ? value : default;
    }

    public virtual bool TryGetValue<TObject>(Guid id, out TObject value) where TObject : TValue
    {
        if (TryGetValue(id, out var baseObject))
        {
            value = (TObject) baseObject;

            return true;
        }

        value = default;

        return false;
    }

    public virtual bool TryGetValue(Guid id, out TValue value)
    {
        if (!IsIdValid(id))
        {
            value = default;
            return false;
        }

        lock (_lock)
        {
            return _lookup.TryGetValue(id, out value);
        }
    }

    public bool Add(TValue value) =>
        InternalSet(value, false);

    public TValue AddNew(Type type, Guid id)
    {
        var idConstructor = type.GetConstructor(new[] {KeyType});
        if (idConstructor == null)
        {
            throw new ArgumentNullException(nameof(type), MessageNoConstructor(type, KeyType.Name));
        }

        var value = (TValue) idConstructor.Invoke(new object[] {id});
        if (value == null)
        {
            throw new ArgumentNullException(
                nameof(type),
                $"Failed to create instance of '{ValueType.Name}' with the ({KeyType.Name}) constructor."
            );
        }

        return InternalSet(value, false) ? value : default;
    }

    public virtual bool Set(Guid key, TValue value)
    {
        if (key != (value?.Id ?? Guid.Empty))
        {
            throw new ArgumentException(@"Key does not match the Guid of the value.", nameof(key));
        }

        return InternalSet(value, true);
    }

    public virtual bool Delete(TValue value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!IsIdValid(value.Id))
        {
            throw new ArgumentException("Invalid Id.", nameof(value));
        }

        lock (_lock)
        {
            _dirty = true;
            return _lookup.Remove(value.Id);
        }
    }

    public virtual bool DeleteAt(Guid guid)
    {
        if (!IsIdValid(guid))
        {
            throw new ArgumentOutOfRangeException(nameof(guid));
        }

        TValue obj;

        lock (_lock)
        {
            if (!_lookup.TryGetValue(guid, out obj))
            {
                return false;
            }
        }

        return Delete(obj);
    }

    public virtual void Clear()
    {
        lock (_lock)
        {
            _dirty = true;
            _lookup.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public virtual IEnumerator<KeyValuePair<Guid, TValue>> GetEnumerator() =>
        Clone.GetEnumerator();

    protected virtual bool IsIdValid(Guid id) => id != Guid.Empty;

    internal virtual bool InternalSet(TValue value, bool overwrite)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!IsIdValid(value.Id))
        {
            throw new ArgumentException("Invalid Id.", nameof(value));
        }

        lock (_lock)
        {
            if (!overwrite && _lookup.ContainsKey(value.Id))
            {
                return false;
            }

            _dirty = true;
            _lookup[value.Id] = value;

            return true;
        }
    }

    private static string MessageNoConstructor(Type type, params string[] constructorMessage)
    {
        var joinedConstructorMessage = string.Join(",", constructorMessage ?? Array.Empty<string>());
        var builder = new StringBuilder()
            .AppendLine($@"No ({joinedConstructorMessage}) constructor for type '{type?.Name}'.")
            .AppendLine(ReflectionUtils.StringifyConstructors(type));
        return builder.ToString();
    }
}

