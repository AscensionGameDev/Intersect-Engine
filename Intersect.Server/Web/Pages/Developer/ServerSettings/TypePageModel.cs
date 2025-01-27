using System.Reflection;
using Intersect.Framework.Reflection;

namespace Intersect.Server.Web.Pages.Developer.ServerSettings;

public abstract class TypePageModel(
    ILogger logger,
    object target,
    Type type,
    bool isEditing = false,
    string? parentId = null
)
    : MemberInfoPageModel<Type>(
        logger: logger,
        target: target,
        info: type,
        isEditing: isEditing,
        parentId: parentId
    )
{

    public List<MemberInfo> Members =>
        ((MemberInfo[])
        [
            ..Type.GetProperties(BindingFlags.Instance | BindingFlags.Public),
            ..Type.GetFields(BindingFlags.Instance | BindingFlags.Public)
        ]).Where(memberInfo => !memberInfo.IsIgnored()).ToList();

    public override string OwnId => ParentId;

    public Type Type { get; } = type;
}