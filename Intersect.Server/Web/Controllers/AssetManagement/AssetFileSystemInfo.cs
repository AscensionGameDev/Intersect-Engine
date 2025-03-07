namespace Intersect.Server.Web.Controllers.AssetManagement;

public abstract record AssetFileSystemInfo(string Path, string Name, AssetFileSystemInfoType Type)
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public FileSystemInfo? FileSystemInfo { get; init; }

    public static AssetFileSystemInfo? From(string rootPath, string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            return default;
        }

        return File.GetAttributes(path).HasFlag(FileAttributes.Directory)
            ? From(rootPath, new DirectoryInfo(path))
            : From(rootPath, new FileInfo(path));
    }

    public static AssetFileSystemInfo? From(string rootPath, FileSystemInfo fileSystemInfo)
    {
        var relativePath = System.IO.Path.GetRelativePath(rootPath, fileSystemInfo.FullName);
        return fileSystemInfo switch
        {
            DirectoryInfo => new AssetDirectoryInfo(relativePath, fileSystemInfo.Name)
            {
                FileSystemInfo = fileSystemInfo,
            },
            FileInfo fileInfo => new AssetFileInfo(relativePath, fileSystemInfo.Name, fileInfo.Length)
            {
                FileSystemInfo = fileSystemInfo,
            },
            _ => default,
        };
    }
}