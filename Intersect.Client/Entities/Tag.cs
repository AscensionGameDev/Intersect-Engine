using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.Client.Entities
{
    public struct Tag
    {
        [JsonProperty("Tag")] public string TagName;

        public TagPosition TagPos;

        public Tag(string tagName, TagPosition tagPos)
        {
            TagName = tagName;
            TagPos = tagPos;
        }
    }
}
