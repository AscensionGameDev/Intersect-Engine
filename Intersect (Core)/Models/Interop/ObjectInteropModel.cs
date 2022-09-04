using System.Linq.Expressions;
using System.Reflection;
using Intersect.Localization;
using Intersect.Localization.Common;
using Intersect.Logging;
using Intersect.Models.Annotations;

namespace Intersect.Models.Interop;

public record PropertyInteropInfo
{
    private GroupAttribute? _groupAttribute;
    private InputAttribute? _inputAttribute;
    private LabelAttribute? _labelAttribute;
    private TooltipAttribute? _tooltipAttribute;

    public ObjectModelAttribute[] Attributes { get; init; }

    public Delegate DelegateGet { get; init; }

    public Delegate? DelegateSet { get; init; }

    public GroupAttribute? Group => _groupAttribute ??= Attributes.OfType<GroupAttribute>().FirstOrDefault();

    public InputAttribute? Input => _inputAttribute ??= Attributes.OfType<InputAttribute>().FirstOrDefault();

    public bool IsReadOnly => DelegateSet == default || (Input?.ReadOnly ?? true);

    public LabelAttribute? Label => _labelAttribute ??= Attributes.OfType<LabelAttribute>().FirstOrDefault();

    public string Name { get; init; }

    public PropertyInfo PropertyInfo { get; init; }

    public int Score { get; init; }

    public TooltipAttribute? Tooltip => _tooltipAttribute ??= Attributes.OfType<TooltipAttribute>().FirstOrDefault();
}

public record GroupInteropInfo
{
    public GroupAttribute? Attribute { get; init; }

    public LocalizedStringReference? Name => Attribute?.Name;

    public IReadOnlyList<PropertyInteropInfo> Properties { get; init; }
}

public partial class ObjectInteropModel
{
    private ObjectInteropModel(IReadOnlyList<GroupInteropInfo> groups)
    {
        Groups = groups;
        Properties = groups.SelectMany(group => group.Properties).ToList().AsReadOnly();
    }

    public IReadOnlyList<GroupInteropInfo> Groups { get; }

    public IReadOnlyList<PropertyInteropInfo> Properties { get; }
}

public partial class ObjectInteropModel
{
    private const BindingFlags TargetPublicBindingFlags = BindingFlags.Public | BindingFlags.Instance;

    public class Builder
    {
        private readonly Type _type;

        private BindingFlags _bindingFlagsProperties;
        private bool _includeProperties;
        private bool _includeReadOnly;

        public Builder(Type type)
        {
            _type = type;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns>this <see cref="ObjectInteropModel.Builder"/></returns>
        public Builder IncludeProperties(bool includeProperties = true)
        {
            _includeProperties = includeProperties;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="includeReadOnly"></param>
        /// <returns>this <see cref="ObjectInteropModel.Builder"/></returns>
        public Builder IncludeReadOnly(bool includeReadOnly = true)
        {
            _includeReadOnly = includeReadOnly;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="BindingFlags"/> to filter properties by.
        /// </summary>
        /// <param name="bindingFlags">the flags to filter properties by; <see cref="BindingFlags.Instance"/> is implicit</param>
        /// <returns>this <see cref="ObjectInteropModel.Builder"/></returns>
        public Builder SetPropertyBindingFlags(BindingFlags bindingFlags)
        {
            _bindingFlagsProperties = bindingFlags;
            return this;
        }

        public ObjectInteropModel Build()
        {
            var groupedProperties = _includeProperties
                ? ScanProperties(_type, _bindingFlagsProperties, _includeReadOnly)
                : Enumerable.Empty<IGrouping<GroupAttribute, PropertyInteropInfo>>();

            var groups = groupedProperties
                .Select(grouping => new GroupInteropInfo
                {
                    Attribute = grouping.Key, Properties = grouping.ToList().AsReadOnly()
                })
                .ToList();

            return new(groups);
        }

        public static Delegate? PropertyInfoToGetDelegate(PropertyInfo propertyInfo, bool includeNonPublic = false)
        {
            if (propertyInfo.GetMethod == default)
            {
                return default;
            }

            if (!includeNonPublic && !propertyInfo.GetMethod.IsPublic)
            {
                return default;
            }

            var instanceParameter = Expression.Parameter(
                propertyInfo.DeclaringType ?? throw new InvalidOperationException(),
                "instance"
            );
            var parameters = propertyInfo.GetMethod.GetParameters()
                .Select(
                    parameterInfo => Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name)
                )
                .ToArray();
            var call = Expression.Call(
                instanceParameter,
                propertyInfo.GetMethod,
                parameters.OfType<Expression>()
            );
            var lambda = Expression.Lambda(call, parameters.Prepend(instanceParameter));
            return lambda.Compile();
        }

        public static Delegate? PropertyInfoToSetDelegate(PropertyInfo propertyInfo, bool includeNonPublic = false)
        {
            if (propertyInfo.SetMethod == default)
            {
                return default;
            }

            if (!includeNonPublic && !propertyInfo.SetMethod.IsPublic)
            {
                return default;
            }

            var instanceParameter = Expression.Parameter(
                propertyInfo.DeclaringType ?? throw new InvalidOperationException(),
                "instance"
            );
            var parameters = propertyInfo.SetMethod.GetParameters()
                .Select(
                    parameterInfo => Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name)
                )
                .ToArray();
            var call = Expression.Call(
                instanceParameter,
                propertyInfo.SetMethod,
                parameters.OfType<Expression>()
            );
            var lambda = Expression.Lambda(call, parameters.Prepend(instanceParameter));
            return lambda.Compile();
        }

        internal static void ValidateInputAttribute(
            Type type,
            PropertyInfo propertyInfo,
            InputAttribute inputAttribute
        )
        {
            switch (inputAttribute)
            {
                case InputLookupAttribute inputLookupAttribute:
                    ValidateInputLookupAttribute(type, propertyInfo, inputLookupAttribute);
                    break;
            }
        }

        internal static void ValidateInputLookupAttribute(
            Type type,
            PropertyInfo propertyInfo,
            InputLookupAttribute inputLookupAttribute
        )
        {
            var foreignKeyPropertyOnThisType = type.GetProperty(
                inputLookupAttribute.ForeignKeyName,
                TargetPublicBindingFlags
            );

            if (foreignKeyPropertyOnThisType == default)
            {
                throw new ArgumentException(
                    $"{type.FullName} does not have a public instance property called \"{inputLookupAttribute.ForeignKeyName}\""
                );
            }

            var foreignKeyPropertyOnOtherType = type.GetProperty(
                inputLookupAttribute.ForeignObjectKeyName,
                TargetPublicBindingFlags
            );

            if (foreignKeyPropertyOnOtherType == default)
            {
                throw new ArgumentException(
                    $"{propertyInfo.PropertyType.FullName} does not have a public instance property called \"{inputLookupAttribute.ForeignObjectKeyName}\""
                );
            }

            // TODO: Validate lookup type
        }

        internal static bool IsValidInputLookupAttribute(Type type, PropertyInfo propertyInfo, InputLookupAttribute inputLookupAttribute)
        {
            try
            {
                ValidateInputLookupAttribute(type, propertyInfo, inputLookupAttribute);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static IEnumerable<IGrouping<GroupAttribute?, PropertyInteropInfo>> ScanProperties(
            Type type,
            BindingFlags bindingFlags,
            bool includeReadOnly
        )
        {
            var hierarchy = ScanHierarchy(type);
            var includedProperties = hierarchy.Select(hierarchyType =>
                    hierarchyType
                        .GetProperties(bindingFlags | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .OrderBy(propertyInfo => propertyInfo.Name)
                        .Where(propertyInfo => propertyInfo.GetCustomAttribute<IgnoredAttribute>() == default)
                        .Where(propertyInfo =>
                            includeReadOnly || propertyInfo.SetMethod != default && propertyInfo.SetMethod.IsPublic)
                        .ToList()
                )
                .ToList();
            var hierarchyCount = includedProperties.Aggregate(
                0,
                (count, properties) => Math.Max(count, properties.Count)
            );
            var orderedProperties = includedProperties.SelectMany((properties, index) =>
                    properties
                        .Select(propertyInfo =>
                        {
                            var attributes = propertyInfo.GetCustomAttributes<ObjectModelAttribute>().ToArray();
                            var delegateGet = PropertyInfoToGetDelegate(propertyInfo);
                            var delegateSet = PropertyInfoToSetDelegate(propertyInfo);
                            var inputAttribute = propertyInfo.GetCustomAttribute<InputAttribute>();

                            try
                            {
                                ValidateInputAttribute(propertyInfo.DeclaringType, propertyInfo, inputAttribute);
                            }
                            catch (Exception exception)
                            {
                                Log.Error(exception);
                                throw;
                            }

                            var orderAttribute = propertyInfo.GetCustomAttribute<OrderAttribute>();
                            var order = (orderAttribute?.Order ?? hierarchyCount - 1);
                            var propertyInteropInfo = new PropertyInteropInfo
                            {
                                Attributes = attributes,
                                DelegateGet = delegateGet,
                                DelegateSet = delegateSet,
                                Name = propertyInfo.Name,
                                PropertyInfo = propertyInfo,
                                Score = index * hierarchyCount + order
                            };
                            return propertyInteropInfo;
                        })
                        .OrderBy(propertyInteropInfo => propertyInteropInfo.Score)
                )
                .ToList();
            var groupedProperties = orderedProperties
                .GroupBy(propertyInteropInfo => propertyInteropInfo.Group)
                .OrderBy(group => group.Key, new GroupAttributeComparer());
            return groupedProperties;
        }

        internal static List<Type> ScanHierarchy(Type type)
        {
            var hierarchy = new List<Type>();

            var currentType = type;
            do
            {
                hierarchy.Add(currentType);
                currentType = currentType.BaseType;
            } while (currentType != default && currentType != typeof(object));

            hierarchy.Reverse();
            return hierarchy;
        }
    }
}
