
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Microsoft;

namespace Intersect.Reflection
{
    public static class TypeExtensions
    {
        public static string QualifiedGenericName([ValidatedNotNull] this Type type) =>
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
                        Debug.Assert(constructorParameter != null, nameof(constructorParameter) + " != null");

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
    }
}
