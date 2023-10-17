using System.Diagnostics;
using System.Reflection;

namespace Intersect.Reflection;

public static partial class TypeExtensions
{
    public static string QualifiedGenericName(this Type type) =>
        $"{type.Name}<{string.Join(", ", type.GenericTypeArguments.Select(parameterType => parameterType.Name))}>";

    public static IEnumerable<ConstructorInfo> FindConstructors(
        this Type type,
        params object[] parameters
    ) => type.GetConstructors()
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

    public static bool ExtendedBy<TChildType>(this Type type) => type.ExtendedBy(typeof(TChildType));

    public static bool ExtendedBy(this Type type, Type childType) => childType.Extends(type);

    public static Type? FindConcreteType(this Type abstractType, Func<Type, bool> predicate, bool allLoadedAssemblies = false)
    {
        if (!abstractType.IsAbstract && !abstractType.IsInterface)
        {
            throw new ArgumentException($"Expected abstract/interface type, received {abstractType.FullName}", nameof(abstractType));
        }

        var assembliesToCheck = allLoadedAssemblies ? AppDomain.CurrentDomain.GetAssemblies() : new[] { abstractType.Assembly };
        var validAssembliesToCheck = assembliesToCheck.Where(assembly => !assembly.IsDynamic);
        var allTypes = validAssembliesToCheck.SelectMany(assembly =>
        {
            try
            {
                return assembly.ExportedTypes;
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        });

        var allConcreteTypes = allTypes.Where(type => !type.IsAbstract && !type.IsInterface && !type.IsGenericType);
        var allDescendantTypes = allConcreteTypes.Where(type => type.Extends(abstractType));
        var firstPredicateMatch = allDescendantTypes.FirstOrDefault(predicate);
        return firstPredicateMatch;
    }

    public static Type FindGenericType(this Type type) =>
        type.FindGenericType(true);

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
                    var genericInterfaceType = currentType
                        .GetInterfaces()
                        .FirstOrDefault(interfaceType =>
                            interfaceType.IsGenericType &&
                            interfaceType.GetGenericTypeDefinition() == genericTypeDefinition);

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

        throw new ArgumentException($"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type));
    }

    public static Type FindGenericTypeDefinition(this Type type) =>
        type.FindGenericTypeDefinition(true);

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

        throw new ArgumentException($"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type));
    }

    public static Type[] FindGenericTypeParameters(this Type type) =>
        type.FindGenericTypeParameters(true);

    public static Type[] FindGenericTypeParameters(this Type type, bool throwOnNonGeneric) =>
        type.FindGenericTypeParameters(default, throwOnNonGeneric);

    public static Type[] FindGenericTypeParameters(this Type type, Type genericTypeDefinition,
        bool throwOnNonGeneric = true)
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
                    var genericInterfaceType = currentType
                        .GetInterfaces()
                        .FirstOrDefault(interfaceType =>
                            interfaceType.IsGenericType &&
                            interfaceType.GetGenericTypeDefinition() == genericTypeDefinition);

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

        throw new ArgumentException($"{type.FullName} is not a generic type and does not extend from a generic type.",
            nameof(type));
    }
}
