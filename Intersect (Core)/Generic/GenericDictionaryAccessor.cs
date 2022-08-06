using System.Reflection;

using Intersect.Framework.Reflection;

namespace Intersect.Generic;

public struct GenericDictionaryAccessor
{
    private static readonly Dictionary<Type, GenericDictionaryAccessor> _accessorCache = new();

    private static readonly MethodInfo CreateWeaklyTypedDelegatesInfo =
        typeof(GenericDictionaryAccessor)
            .GetMethod(
                nameof(CreateWeaklyTypedDelegates),
                BindingFlags.NonPublic | BindingFlags.Static
            );

    private delegate TValue DictionaryGet<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key);

    private delegate void DictionarySet<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value);

    private delegate object GenericDictionaryGet(object dictionary, object key);

    private delegate void GenericDictionarySet(object dictionary, object key, object value);

    private static GenericDictionaryAccessor CreateWeaklyTypedDelegates<TKey, TValue>()
    {
        return new GenericDictionaryAccessor(
            (dictionary, key) => (dictionary as IDictionary<TKey, TValue>)[(TKey)key],
            (dictionary, key, value) => (dictionary as IDictionary<TKey, TValue>)[(TKey)key] = (TValue)value
        );
    }

    private static GenericDictionaryAccessor CreateOrGetAccessor(Type dictionaryType)
    {
        if (!_accessorCache.TryGetValue(dictionaryType, out GenericDictionaryAccessor accessor))
        {
            var dictionaryTypeParameters = dictionaryType.FindGenericTypeParameters(typeof(IDictionary<,>));
            var delegatesFactory = CreateWeaklyTypedDelegatesInfo.MakeGenericMethod(dictionaryTypeParameters);
            accessor = (GenericDictionaryAccessor)delegatesFactory.Invoke(null, null);
            _accessorCache[dictionaryType] = accessor;
        }

        return accessor;
    }

    private readonly GenericDictionaryGet InternalGet;

    private readonly GenericDictionarySet InternalSet;

    private GenericDictionaryAccessor(GenericDictionaryGet getter, GenericDictionarySet setter)
    {
        InternalGet = getter;
        InternalSet = setter;
    }

    public static object Get(object dictionary, object key) =>
        CreateOrGetAccessor(dictionary.GetType()).InternalGet(dictionary, key);

    public static object Get<TKey>(object dictionary, TKey key) =>
        Get(dictionary, key as object);

    public static TValue Get<TValue>(object dictionary, object key) =>
        Get(dictionary, key) is TValue value ? value : default;

    public static void Set(object dictionary, object key, object value) =>
        CreateOrGetAccessor(dictionary.GetType()).InternalSet(dictionary, key, value);

    public static void Set<TKey>(object dictionary, TKey key, object value) =>
        Set(dictionary, key as object, value);

    public static void Set<TValue>(object dictionary, object key, TValue value) =>
        Set(dictionary, key, value as object);
}
