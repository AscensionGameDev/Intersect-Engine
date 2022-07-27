using System.Collections.ObjectModel;
using System.Reflection;

namespace Intersect.Localization.Common;

public abstract partial class AbstractRootNamespace : LocaleNamespace
{
    private readonly Dictionary<FieldInfo, Localized> _localized;

    private readonly Dictionary<FieldInfo, LocaleNamespace> _namespaces;

    protected AbstractRootNamespace()
    {
        _localized = new();
        _namespaces = new();

        CrawlNamespace(this, this);

        Localized = new ReadOnlyDictionary<FieldInfo, Localized>(_localized);
        Namespaces = new ReadOnlyDictionary<FieldInfo, LocaleNamespace>(_namespaces);
    }

    public IReadOnlyDictionary<FieldInfo, Localized> Localized { get; }

    public IReadOnlyDictionary<FieldInfo, LocaleNamespace> Namespaces { get; }

    private static void CrawlNamespace(AbstractRootNamespace rootNamespace, LocaleNamespace currentNamespace)
    {
        var currentNamespaceType = currentNamespace.GetType();
        var fieldInfos = currentNamespaceType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var fieldInfo in fieldInfos)
        {
            var fieldValue = fieldInfo.GetValue(currentNamespace);
            switch (fieldValue)
            {
                case LocaleNamespace localeNamespace:
                    rootNamespace._namespaces[fieldInfo] = localeNamespace;
                    CrawlNamespace(rootNamespace, localeNamespace);
                    break;

                case Localized localized:
                    rootNamespace._localized[fieldInfo] = localized;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
