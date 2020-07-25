using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Intersect.Reflection
{

    public static class TypeExtensions
    {

        [NotNull]
        public static IEnumerable<ConstructorInfo> FindConstructors(
            [NotNull] this Type type,
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
