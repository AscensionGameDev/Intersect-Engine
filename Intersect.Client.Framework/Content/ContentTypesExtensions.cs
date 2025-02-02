namespace Intersect.Client.Framework.Content;


public static partial class ContentTypesExtensions
{

    public static Type GetAssetType(this ContentType contentType)
    {
        var memberName = contentType.ToString();
        var memberInfo = typeof(ContentType).GetMember(memberName).FirstOrDefault();
        if (memberInfo == null)
        {
            throw new InvalidOperationException($@"{nameof(ContentType)} missing expected member: {memberName}");
        }

        var attribute = memberInfo.GetCustomAttributes(typeof(AssetTypeAttribute), true).FirstOrDefault();
        if (attribute is AssetTypeAttribute assetTypeAttribute)
        {
            return assetTypeAttribute.Type;
        }

        throw new InvalidOperationException(
            $@"{nameof(ContentType)} missing {nameof(AssetTypeAttribute)} on member: {memberName}"
        );
    }

}
