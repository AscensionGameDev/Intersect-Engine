using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

using Newtonsoft.Json;

namespace Intersect.Editor.Interface.Windows;

internal partial class LocalizationWindow
{
    private readonly Dictionary<(Type, Guid, string), object> _edited = new();

    private readonly Dictionary<KeyValuePair<Type, PropertyInfo>, object> _setterCache = new();

    protected virtual bool DetectChanges<TObject, TProperty>(
        (Type, Guid, string) compositeId,
        TObject? originalObject,
        Expression<Func<TObject, TProperty>> propertyExpression,
        TProperty? newValue,
        out TObject? editedObject
    )
    {
        if (_edited.TryGetValue(compositeId, out var intermediateEditedObject))
        {
            editedObject = (TObject)intermediateEditedObject;
        }
        else
        {
            editedObject = default;
        }

        if (originalObject == null)
        {
            if (newValue == null)
            {
                if (editedObject != null)
                {
                    _ = _edited.Remove(compositeId);
                }

                editedObject = default;
                return false;
            }

            if (editedObject == null)
            {
                if (Activator.CreateInstance(typeof(TObject), true) is not TObject createdObject)
                {
                    throw new InvalidOperationException();
                }

                editedObject = createdObject;
                _edited[compositeId] = editedObject;
            }
        }
        else
        {
            var getterDelegate = propertyExpression.Compile();
            var originalValue = getterDelegate(originalObject);

            if (originalValue?.Equals(newValue) ?? false)
            {
                if (editedObject != null)
                {
                    _ = _edited.Remove(compositeId);
                }
                return false;
            }

            if (editedObject == null)
            {
                editedObject = JsonClone(originalObject);
                _edited[compositeId] = editedObject!;
            }
        }

        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException("Expected a member expression.");
        }

        if (memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new InvalidOperationException("Expected a property.");
        }

        if (!_setterCache.TryGetValue(new (typeof(TObject), propertyInfo), out var setterDelegate))
        {
            var setMethod = propertyInfo.GetSetMethod();
            if (setMethod == default)
            {
                throw new InvalidOperationException($"{propertyInfo.DeclaringType?.FullName ?? "?"}.{propertyInfo.Name} does not have a setter.");
            }

            var parameterTObject = Expression.Parameter(typeof(TObject), "target");
            var parameterTProperty = Expression.Parameter(typeof(TProperty?), "propertyValue");

            var setterExpression = Expression.Lambda<Action<TObject, TProperty?>>(
                Expression.Call(parameterTObject, setMethod, parameterTProperty),
                parameterTObject,
                parameterTProperty
            );

            setterDelegate = setterExpression.Compile();
        }

        if (setterDelegate is not Action<TObject, TProperty?> setAction) {
            throw new InvalidOperationException();
        }

        Debug.Assert(editedObject != null);
        setAction(editedObject, newValue);

        return true;
    }

    protected virtual TValue? JsonClone<TValue>(TValue? value)
    {
        if (value == null)
        {
            return default;
        }

        var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        });

        var clone = JsonConvert.DeserializeObject<TValue>(json, new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        });

        return clone;
    }
}
