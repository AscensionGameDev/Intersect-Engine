using System.Reflection;

namespace Intersect.Framework.Reflection;

public static class PropertyInfoExtensions
{
    public static bool IsPropertyDeclaredInBaseTypeOrInterface(
        this PropertyInfo propertyInfo,
        Type type,
        bool checkSetter = false
    ) => propertyInfo.DeclaringType != type || propertyInfo.IsPropertyDeclaredByInterface(type, checkSetter);

    public static bool IsPropertyDeclaredByInterface(this PropertyInfo propertyInfo, bool checkSetter = false) =>
        IsPropertyDeclaredByInterface(
            propertyInfo,
            propertyInfo.DeclaringType ?? throw new ArgumentException(
                $"{nameof(propertyInfo)} has a null {nameof(PropertyInfo.DeclaringType)}",
                nameof(propertyInfo)
            ),
            checkSetter
        );

    public static bool IsPropertyDeclaredByInterface<TSearchType>(this PropertyInfo propertyInfo,
        bool checkSetter = false) => IsPropertyDeclaredByInterface(propertyInfo, typeof(TSearchType), checkSetter);

    public static bool IsPropertyDeclaredByInterface(this PropertyInfo propertyInfo, Type searchType, bool checkSetter = false)
    {
        var declaringType = propertyInfo.DeclaringType;
        if (declaringType == default)
        {
            return false;
        }

        MethodInfo searchMethodInfo;
        if (checkSetter)
        {
            searchMethodInfo = propertyInfo.SetMethod ?? throw new InvalidOperationException(
                $"Tried to check setter of {declaringType.GetName(qualified: true)}.{propertyInfo.Name} which does not exist"
            );
        }
        else
        {
            searchMethodInfo = propertyInfo.GetMethod ?? throw new InvalidOperationException(
                $"Tried to check getter of {declaringType.GetName(qualified: true)}.{propertyInfo.Name} which does not exist"
            );
        }

        var interfaceTypes = searchType.GetInterfaces();
        if (searchType.IsInterface)
        {
            interfaceTypes =
            [
                searchType,
                ..interfaceTypes,
            ];
        }

        if (searchType.IsInterface)
        {
            return false;
        }

        foreach (var interfaceType in interfaceTypes)
        {
            var interfaceMap = declaringType.GetInterfaceMap(interfaceType);
            if (interfaceMap.TargetMethods.Contains(searchMethodInfo))
            {
                return true;
            }

            if (interfaceMap.TargetMethods.Any(
                    targetMethodInfo => targetMethodInfo.MethodHandle == searchMethodInfo.MethodHandle
                ))
            {
                return true;
            }
        }

        return false;
    }
}