using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Reflection;

public static partial class TypeExtensions
{
    public static string[] GetMappedColumnNames(this Type type)
    {
        return type.GetProperties()
            .Where(propertyInfo => propertyInfo.GetCustomAttribute<NotMappedAttribute>() == default)
            .GroupBy(
                propertyInfo =>
                {
                    var foreignKeyAttribute = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
                    return foreignKeyAttribute == default ? propertyInfo.Name : foreignKeyAttribute.Name;
                }
            )
            .SelectMany(
                group =>
                {
                    var items = group.ToArray();
                    if (items.Length > 1)
                    {
                        return items.Where(propertyInfo => propertyInfo.PropertyType.IsValueType)
                            .Select(propertyInfo => propertyInfo.Name);
                    }

                    return items.SelectMany(
                        propertyInfo =>
                        {
                            if (propertyInfo.PropertyType.IsValueType)
                            {
                                return new[] { propertyInfo.Name };
                            }

                            if (!propertyInfo.PropertyType.IsClass ||
                                propertyInfo.PropertyType.IsAbstract ||
                                propertyInfo.PropertyType.GetCustomAttribute<OwnedAttribute>() == default)
                            {
                                return Array.Empty<string>();
                            }

                            return propertyInfo.PropertyType.GetMappedColumnNames()
                                .Select(name => string.Join('_', propertyInfo.Name, name));
                        }
                    );
                }
            )
            .ToArray();
    }

    public static string QualifiedGenericName(this Type type) =>
        $"{type.Name}<{string.Join(", ", type.GenericTypeArguments.Select(parameterType => parameterType.Name))}>";

    public static IEnumerable<ConstructorInfo> FindConstructors(this Type type, params object[] parameters) => type
        .GetConstructors()
        .Where(
            constructor =>
            {
                var constructorParameters = constructor.GetParameters();
                if (constructorParameters.Length < parameters.Length)
                {
                    return false;
                }

                for (var index = 0; index < constructorParameters.Length; ++index)
                {
                    var constructorParameter = constructorParameters[index];
                    Debug.Assert(constructorParameter != null, $"{nameof(constructorParameter)} != null");

                    if (index >= parameters.Length)
                    {
                        return constructorParameter.IsOptional;
                    }

                    var parameter = parameters[index];

                    if (parameter == null)
                    {
                        if (constructorParameter.ParameterType.IsValueType)
                        {
                            return false;
                        }

                        continue;
                    }

                    var parameterType = parameter.GetType();
                    if (!constructorParameter.ParameterType.IsAssignableFrom(parameterType))
                    {
                        return false;
                    }
                }

                return true;
            }
        );

    public static bool Extends(this Type childType, Type baseType)
    {
        if (!baseType.IsGenericTypeDefinition)
        {
            return baseType.IsAssignableFrom(childType);
        }

        var currentType = childType;
        while (currentType != default)
        {
            if (currentType.IsGenericType)
            {
                if (currentType.GetGenericTypeDefinition() == baseType)
                {
                    return true;
                }
            }

            currentType = currentType.BaseType;
        }

        return false;
    }

    public static bool Extends<TBaseType>(this Type type) => type.Extends(typeof(TBaseType));

    public static bool ExtendedBy<TChildType>(this Type type) => typeof(TChildType).Extends(type);

    public static Type? FindConcreteType(
        this Type abstractType,
        Func<Type, bool> predicate,
        bool allLoadedAssemblies = false
    )
    {
        if (!abstractType.IsAbstract && !abstractType.IsInterface)
        {
            throw new ArgumentException(
                $"Expected abstract/interface type, received {abstractType.FullName}",
                nameof(abstractType)
            );
        }

        var assembliesToCheck = allLoadedAssemblies ? AppDomain.CurrentDomain.GetAssemblies()
            : new[] { abstractType.Assembly };
        var validAssembliesToCheck = assembliesToCheck.Where(assembly => !assembly.IsDynamic);
        var allTypes = validAssembliesToCheck.SelectMany(
            assembly =>
            {
                try
                {
                    return assembly.ExportedTypes;
                }
                catch
                {
                    return Enumerable.Empty<Type>();
                }
            }
        );

        var allConcreteTypes = allTypes.Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType);
        var allDescendantTypes = allConcreteTypes.Where(type => type.Extends(abstractType));
        var firstPredicateMatch = allDescendantTypes.FirstOrDefault(predicate);
        return firstPredicateMatch;
    }

    public static Type[] FindDerivedTypes(this Type type, params Assembly[] assemblies)
    {
        var targetAssemblies = assemblies;
        if (targetAssemblies.Length < 1)
        {
            targetAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        var derivedTypes = targetAssemblies.Where(assembly => !assembly.IsDynamic)
            .SelectMany(
                assembly =>
                {
                    try
                    {
                        return assembly.ExportedTypes;
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                }
            )
            .SelectMany(type => type.GetProperties())
            .Select(propertyInfo => propertyInfo.PropertyType)
            .Where(propertyType => propertyType.Extends(type))
            .Distinct()
            .ToArray();
        return derivedTypes;
    }

    public static Type FindGenericType(this Type type) => type.FindGenericType(true);

    public static Type? FindGenericType(this Type type, bool throwOnNonGeneric) =>
        type.FindGenericType(default, throwOnNonGeneric);

    public static Type? FindGenericType(this Type type, Type genericTypeDefinition, bool throwOnNonGeneric)
    {
        if (genericTypeDefinition != null && !genericTypeDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"Not a valid generic type definition: {genericTypeDefinition.FullName}",
                nameof(genericTypeDefinition)
            );
        }

        var currentType = type;
        while (currentType != default)
        {
            if (genericTypeDefinition != default)
            {
                if (genericTypeDefinition.IsInterface)
                {
                    var genericInterfaceType = currentType.GetInterfaces()
                        .FirstOrDefault(
                            interfaceType => interfaceType.IsGenericType &&
                                             interfaceType.GetGenericTypeDefinition() == genericTypeDefinition
                        );

                    if (genericInterfaceType != null)
                    {
                        return genericInterfaceType;
                    }
                }
                else if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return currentType;
                }
            }
            else if (currentType.IsGenericType)
            {
                return currentType;
            }

            currentType = currentType.BaseType;
        }

        if (!throwOnNonGeneric)
        {
            return default;
        }

        throw new ArgumentException(
            $"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type)
        );
    }

    public static Type? FindGenericInterfaceForDefinition(this Type type, Type genericTypeDefinition)
    {
        return type.GetInterfaces().FirstOrDefault(i => i.Extends(genericTypeDefinition));
    }

    public static Type? FindGenericTypeDefinition(this Type type) => type.FindGenericTypeDefinition(true);

    public static Type? FindGenericTypeDefinition(this Type type, bool throwOnNonGeneric)
    {
        if (type.IsGenericTypeDefinition)
        {
            return type;
        }

        var currentType = type;
        while (currentType != default)
        {
            if (currentType.IsGenericType)
            {
                return currentType.GetGenericTypeDefinition();
            }

            currentType = currentType.BaseType;
        }

        if (!throwOnNonGeneric)
        {
            return default;
        }

        throw new ArgumentException(
            $"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type)
        );
    }

    public static Type[] FindGenericTypeParameters(this Type type) => type.FindGenericTypeParameters(true);

    public static Type[] FindGenericTypeParameters(this Type type, bool throwOnNonGeneric) =>
        type.FindGenericTypeParameters(default, throwOnNonGeneric);

    public static Type[] FindGenericTypeParameters(
        this Type type,
        Type genericTypeDefinition,
        bool throwOnNonGeneric = true
    )
    {
        if (genericTypeDefinition != null && !genericTypeDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"Not a valid generic type definition: {genericTypeDefinition.FullName}",
                nameof(genericTypeDefinition)
            );
        }

        var currentType = type;
        while (currentType != default)
        {
            if (genericTypeDefinition != default)
            {
                if (genericTypeDefinition.IsInterface)
                {
                    var genericInterfaceType = currentType.GetInterfaces()
                        .FirstOrDefault(
                            interfaceType => interfaceType.IsGenericType &&
                                             interfaceType.GetGenericTypeDefinition() == genericTypeDefinition
                        );

                    if (genericInterfaceType != null)
                    {
                        return genericInterfaceType.GetGenericArguments();
                    }
                }
                else if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return currentType.GetGenericArguments();
                }
            }
            else if (currentType.IsGenericType)
            {
                return currentType.GetGenericArguments();
            }

            currentType = currentType.BaseType;
        }

        if (!throwOnNonGeneric)
        {
            return Array.Empty<Type>();
        }

        throw new ArgumentException(
            $"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type)
        );
    }

    public static Type[] FindImplementationsIn(this Type incompleteType, IEnumerable<Type> types)
    {
        if (!incompleteType.IsAbstract && !incompleteType.IsInterface)
        {
            throw new ArgumentException(
                string.Format(ReflectionStrings.ExpectedAnIncompleteTypeButReceivedX, incompleteType.FullName),
                nameof(incompleteType)
            );
        }

        return types.Where(
                type =>
                    type is { IsAbstract: false, IsClass: true, IsGenericTypeDefinition: false, IsInterface: false } &&
                    type.Extends(incompleteType)
            )
            .ToArray();
    }

    [return: NotNullIfNotNull(nameof(fallbackIfNull))]
    public static string? GetAssemblyName(this Type type, string? fallbackIfNull = default)
    {
        Debug.Assert(type != default);
        return type.Assembly.GetName().Name ?? fallbackIfNull;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetQualifiedName(this Type type) => type.FullName ?? type.Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsConcrete(this Type type) => type is { IsInterface: false, IsAbstract: false, IsClass: true };
}
