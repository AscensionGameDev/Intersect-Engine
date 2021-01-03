
using System;
using System.Linq;

namespace Intersect.Client.Framework.Content
{

    public static class ContentTypesExtensions
    {

        public static Type GetAssetType(this ContentTypes contentType)
        {
            var memberName = contentType.ToString();
            var memberInfo = typeof(ContentTypes).GetMember(memberName).FirstOrDefault();
            if (memberInfo == null)
            {
                throw new InvalidOperationException($@"{nameof(ContentTypes)} missing expected member: {memberName}");
            }

            var attribute = memberInfo.GetCustomAttributes(typeof(AssetTypeAttribute), true).FirstOrDefault();
            if (attribute is AssetTypeAttribute assetTypeAttribute)
            {
                return assetTypeAttribute.Type;
            }

            throw new InvalidOperationException(
                $@"{nameof(ContentTypes)} missing {nameof(AssetTypeAttribute)} on member: {memberName}"
            );
        }

    }

}
