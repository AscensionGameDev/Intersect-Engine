namespace Intersect.Server.Web.Controllers.AssetManagement;

public sealed record AssetFileInfo(string Path, string Name, long Size) : AssetFileSystemInfo(Path, Name, AssetFileSystemInfoType.File);