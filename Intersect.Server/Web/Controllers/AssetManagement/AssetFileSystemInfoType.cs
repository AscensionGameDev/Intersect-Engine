namespace Intersect.Server.Web.Controllers.AssetManagement;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum AssetFileSystemInfoType
{
    File,
    Directory,
}