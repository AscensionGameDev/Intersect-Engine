namespace Intersect.Server.Web.Controllers.AssetManagement;

public sealed record AssetDirectoryInfo(string Path, string Name) : AssetFileSystemInfo(Path, Name, AssetFileSystemInfoType.Directory);