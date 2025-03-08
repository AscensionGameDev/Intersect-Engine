using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text;
using Htmx;
using Intersect.Framework.Core.AssetManagement;
using Intersect.Framework.Reflection;
using Intersect.Server.Web.Extensions;
using Intersect.Server.Web.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Intersect.Server.Web.Controllers.AssetManagement;

[Route("assets")]
[Authorize(Policy = "Developer")]
public sealed partial class AssetsController : IntersectController
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    private static readonly MediaTypeHeaderValue MediaTypeApplicationJson =
        MediaTypeHeaderValue.Parse("application/json");

    private static DateTime _clientUpdateManifestCacheExpiry;
    private static UpdateManifest? _clientUpdateManifest;
    private static DateTime _editorUpdateManifestCacheExpiry;
    private static UpdateManifest? _editorUpdateManifest;

    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<AssetsController> _logger;
    private readonly IOptionsMonitor<UpdateServerOptions> _updateServerOptionsMonitor;

    public AssetsController(
        IHostEnvironment hostEnvironment,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<UpdateServerOptions> updateServerOptionsMonitor
    )
    {
        _hostEnvironment = hostEnvironment;
        _logger = loggerFactory.CreateLogger<AssetsController>();
        _updateServerOptionsMonitor = updateServerOptionsMonitor;
    }

    private string AssetRootPath => Path.Combine(
        _hostEnvironment.ContentRootPath,
        _updateServerOptionsMonitor.CurrentValue.AssetRoot
    );

    private TimeSpan ManifestCacheExpiry => _updateServerOptionsMonitor.CurrentValue.ManifestCacheExpiry ??
                                            UpdateServerOptions.DefaultManifestCacheExpiry;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        if (context.Result != default)
        {
            return;
        }

        // ReSharper disable once InvertIf
        if (!_updateServerOptionsMonitor.CurrentValue.Enabled)
        {
            context.Result = NotFound();
            return;
        }
    }

    [HttpGet("{**path}")]
    [AllowAnonymous]
    public IActionResult AssetGet([FromRoute] string? path = default)
    {
        if (!(path?.StartsWith("client") ?? false))
        {
            if (!User.HasRole("Editor"))
            {
                return Forbidden();
            }
        }

        var assetRootPath = AssetRootPath;
        var pathToInspect = Path.Combine(assetRootPath, path?.Trim() ?? string.Empty);

        var assetFileSystemInfo = AssetFileSystemInfo.From(assetRootPath, pathToInspect);
        if (assetFileSystemInfo == default)
        {
            return Ok(Array.Empty<AssetFileSystemInfo>());
        }

        if (assetFileSystemInfo.Type == AssetFileSystemInfoType.File)
        {
            if (Request.GetTypedHeaders().Accept.Any(mt => mt.IsSubsetOf(MediaTypeApplicationJson)))
            {
                return Ok(new[] { assetFileSystemInfo });
            }

            if (!ContentTypeProvider.TryGetContentType(pathToInspect, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new PhysicalFileResult(assetFileSystemInfo.FileSystemInfo?.FullName ?? pathToInspect, contentType);
        }

        try
        {
            DirectoryInfo directoryInfo = new(pathToInspect);
            var assetFileSystemInfos = directoryInfo.EnumerateFileSystemInfos()
                .Select(fileSystemInfo => AssetFileSystemInfo.From(assetRootPath, fileSystemInfo))
                .ToArray();
            return Ok(assetFileSystemInfos);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate client update manifest");

            object? data = default;
#if DEBUG
            data = exception;
#endif

            return InternalServerError(data);
        }
    }

    private IActionResult AssetUploadSingleFile(string assetRootPath, string originalPath, string resolvedPath)
    {
        throw new NotImplementedException("Single file uploading");
    }

    private IActionResult AssetRename(
        string assetRootPath,
        string sourcePath,
        string resolvedSourcePath,
        string destinationPath,
        string resolvedDestinationPath
    )
    {
        if (Path.GetRelativePath(assetRootPath, resolvedDestinationPath).StartsWith(".."))
        {
            _logger.LogWarning(
                "{UserId} attempted to move '{SourcePath}' to '{DestinationPath}' which is outside of the sandbox",
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                sourcePath,
                destinationPath
            );
            return BadRequest("Invalid destination path");
        }

        AssetFileSystemInfo assetFileSystemInfo;
        try
        {
            assetFileSystemInfo = AssetFileSystemInfo.From(assetRootPath, resolvedSourcePath);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to get file system information for '{SourcePath}' ({ResolvedSourcePath})",
                sourcePath,
                resolvedSourcePath
            );
            return BadRequest("Invalid source path");
        }

        if (string.Equals(resolvedSourcePath, resolvedDestinationPath, StringComparison.Ordinal))
        {
            return Ok(assetFileSystemInfo);
        }

        (FileSystemInfo destinationInfo, DirectoryInfo? destinationParentInfo) =
            assetFileSystemInfo.FileSystemInfo switch
            {
                DirectoryInfo => (new DirectoryInfo(resolvedDestinationPath) as FileSystemInfo,
                    new DirectoryInfo(resolvedDestinationPath).Parent),
                FileInfo => (new FileInfo(resolvedDestinationPath) as FileSystemInfo,
                    new FileInfo(resolvedDestinationPath).Directory),
                _ => throw new NotSupportedException(assetFileSystemInfo.FileSystemInfo?.GetQualifiedTypeName()),
            };

        if (destinationParentInfo is { Exists: false })
        {
            destinationParentInfo.Create();
        }

        switch (assetFileSystemInfo.FileSystemInfo)
        {
            case DirectoryInfo directoryInfo:
                directoryInfo.MoveTo(destinationInfo.FullName);
                break;
            case FileInfo fileInfo:
                fileInfo.MoveTo(destinationInfo.FullName);
                break;
            default:
                throw new UnreachableException(
                    $"Should have already been excluded: {assetFileSystemInfo.FileSystemInfo.GetQualifiedTypeName()}"
                );
        }

        return Ok(AssetFileSystemInfo.From(assetRootPath, destinationInfo));
    }

    [HttpPost]
    public IActionResult AssetsUpload([FromForm] string folder, [FromForm] List<IFormFile> files)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            if (!Request.IsHtmx())
            {
                return BadRequest("No destination folder");
            }

            var partial = PartialView(
                "~/Web/Pages/Shared/_Toast.cshtml",
                new ToastModel
                {
                    Message = "No destination folder",
                    Type = ToastModel.TypeError,
                }
            );
            return partial;

        }

        var assetRootPath = AssetRootPath;
        var destinationFolder = Path.GetFullPath(Path.Combine(assetRootPath, folder));
        var relativeDestinationFolder = Path.GetRelativePath(assetRootPath, destinationFolder);
        if (relativeDestinationFolder.StartsWith(".."))
        {
            _logger.LogWarning(
                "{UserId} tried to upload to a folder outside of the sandbox: {DirectoryPath}",
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                destinationFolder
            );

            if (!Request.IsHtmx())
            {
                return BadRequest("Destination folder does not exist");
            }

            var partial = PartialView(
                "~/Web/Pages/Shared/_Toast.cshtml",
                new ToastModel
                {
                    Message = "Destination folder does not exist",
                    Type = ToastModel.TypeError,
                }
            );
            return partial;
        }

        DirectoryInfo directoryInfo = new(destinationFolder);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        Dictionary<string, (HttpStatusCode StatusCode, string? Message)> results = new();
        foreach (var file in files)
        {
            if (results.ContainsKey(file.FileName))
            {
                if (!Request.IsHtmx())
                {
                    return BadRequest($"Multiple files uploaded with the name '{file.FileName}'");
                }

                var partial = PartialView(
                    "~/Web/Pages/Shared/_Toast.cshtml",
                    new ToastModel
                    {
                        Message = $"Multiple files uploaded with the name '{file.FileName}'",
                        Type = ToastModel.TypeError,
                    }
                );
                return partial;
            }

            var resolvedFilePath = Path.GetFullPath(Path.Combine(directoryInfo.FullName, file.FileName));
            var relativeFilePath = Path.GetRelativePath(directoryInfo.FullName, resolvedFilePath);
            if (relativeFilePath.StartsWith(".."))
            {
                _logger.LogWarning(
                    "{UserId} tried to upload a file outside of its folder: {FilePath} {DirectoryPath}",
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    resolvedFilePath,
                    destinationFolder
                );
                results[file.FileName] = (HttpStatusCode.BadRequest, "Destination folder does not exist");
                continue;
            }

            FileInfo fileInfo = new(resolvedFilePath);
            if (fileInfo.Exists)
            {
                results[file.FileName] = (HttpStatusCode.Conflict, $"'{fileInfo.Name}' already exists");
                continue;
            }

            try
            {
                using var networkStream = file.OpenReadStream();
                using var targetStream = fileInfo.OpenWrite();
                networkStream.CopyTo(targetStream);
                results[file.FileName] = (HttpStatusCode.OK, $"Successfully uploaded {fileInfo.Name}");
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Upload of {FilePath} interrupted",
                    Path.GetRelativePath(assetRootPath, resolvedFilePath)
                );
                results[file.FileName] = (HttpStatusCode.InternalServerError,
                    $"Upload interrupted for '{fileInfo.Name}'");
            }
        }

        var statusCode = results.Aggregate(
            HttpStatusCode.OK,
            (aggregateStatusCode, result) =>
            {
                var (_, (statusCode, _)) = result;
                return statusCode == aggregateStatusCode ? statusCode : HttpStatusCode.MultiStatus;
            }
        );

        if (folder == "client")
        {
            _clientUpdateManifest = default;
        }
        else
        {
            _editorUpdateManifest = default;
        }

        if (!Request.IsHtmx())
        {
            return StatusCode(statusCode, results);
        }

        var aggregatePartial = PartialView(
            "~/Web/Pages/Shared/_Toasts.cshtml",
            results.Select(
                    pair =>
                    {
                        var (resultStatusCode, message) = pair.Value;
                        var type = resultStatusCode switch
                        {
                            >= HttpStatusCode.BadRequest => ToastModel.TypeError,
                            HttpStatusCode.OK => ToastModel.TypeSuccess,
                            _ => ToastModel.TypeWarning,
                        };
                        return new ToastModel
                        {
                            Message = message,
                            Type = type,
                        };
                    }
                )
                .ToArray()
        );
        aggregatePartial.StatusCode = (int)statusCode;
        return aggregatePartial;
    }

    [HttpPost("{**path}")]
    public IActionResult AssetPost(string? path = default, [FromHeader(Name = "Move-To")] string? destinationPath = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return BadRequest("No source path");
        }

        var assetRootPath = AssetRootPath;

        var sanitizedSourcePath = path.Trim();
        var resolvedSourcePath = Path.Combine(assetRootPath, sanitizedSourcePath);
        if (Path.GetRelativePath(assetRootPath, resolvedSourcePath).StartsWith(".."))
        {
            return BadRequest("Invalid source path");
        }

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            return AssetUploadSingleFile(assetRootPath, sanitizedSourcePath, resolvedSourcePath);
        }

        var sanitizedDestinationPath = destinationPath.Trim();
        var resolvedDestinationPath = Path.Combine(assetRootPath, sanitizedDestinationPath);

        return AssetRename(
            assetRootPath,
            sanitizedSourcePath,
            resolvedSourcePath,
            sanitizedDestinationPath,
            resolvedDestinationPath
        );
    }

    [HttpDelete("{**path}")]
    public IActionResult BrowseDelete(string? path = default)
    {
        var assetRootPath = AssetRootPath;
        var pathToInspect = Path.Combine(assetRootPath, path?.Trim() ?? string.Empty);

        var assetFileSystemInfo = AssetFileSystemInfo.From(assetRootPath, pathToInspect);

        try
        {
            var fileSystemInfo = assetFileSystemInfo.FileSystemInfo;
            if (fileSystemInfo is not { Exists: true })
            {
                return NotFound();
            }

            fileSystemInfo.Delete();
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Failed to delete asset: {Path}", string.IsNullOrWhiteSpace(path) ? "<empty>" : path);
            return InternalServerError();
        }
    }

    [HttpDelete("client")]
    public IActionResult ClearClientAssets() => DeleteAssets("client");

    [AllowAnonymous]
    [HttpGet("client/update.json")]
    public IActionResult ClientUpdateManifest()
    {
        try
        {
            Response.Headers.ContentType = "application/json";

            if (_clientUpdateManifest != default && DateTime.UtcNow < _clientUpdateManifestCacheExpiry)
            {
                return Ok(_clientUpdateManifest);
            }

            if (!TryGenerateUpdateManifestFrom("client", out var manifest))
            {
                return InternalServerError();
            }

            _clientUpdateManifestCacheExpiry = DateTime.UtcNow.Add(ManifestCacheExpiry);
            _clientUpdateManifest = manifest;
            return Ok(manifest);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate client update manifest");

            object? data = default;
#if DEBUG
            data = exception;
#endif

            return InternalServerError(data);
        }
    }

    [AllowAnonymous]
    [HttpPost("stream/client")]
    public IActionResult StreamClientAssets([FromBody] List<string> assetNames) => StreamAssets("client", assetNames);

    [HttpPost("stream/editor")]
    public IActionResult StreamEditorAssets([FromBody] List<string> assetNames) => StreamAssets("editor", assetNames);


    [HttpDelete("client/update.json")]
    public IActionResult ClearClientUpdateManifestCache()
    {
        _clientUpdateManifest = default;
        return Ok();
    }

    [HttpDelete("editor")]
    public IActionResult ClearEditorAssets() => DeleteAssets("editor");

    [HttpGet("editor/update.json")]
    public IActionResult EditorUpdateManifest()
    {
        try
        {
            Response.Headers.ContentType = "application/json";
            if (_editorUpdateManifest != default && DateTime.UtcNow < _editorUpdateManifestCacheExpiry)
            {
                return Ok(_editorUpdateManifest);
            }

            if (!TryGenerateUpdateManifestFrom("editor", out var manifest))
            {
                return InternalServerError();
            }

            _editorUpdateManifestCacheExpiry = DateTime.UtcNow.Add(ManifestCacheExpiry);
            _editorUpdateManifest = manifest;
            return Ok(manifest);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate editor update manifest");

            object? data = default;
#if DEBUG
            data = exception;
#endif

            return InternalServerError(data);
        }
    }

    [HttpDelete("editor/update.json")]
    public IActionResult ClearEditorUpdateManifestCache()
    {
        _editorUpdateManifest = default;
        return Ok();
    }

    private IActionResult DeleteAssets(string subdirectory)
    {
        try
        {
            var assetSubdirectoryPath = Path.Combine(AssetRootPath, subdirectory);
            DirectoryInfo assetSubdirectoryInfo = new(assetSubdirectoryPath);
            if (assetSubdirectoryInfo.Exists)
            {
                assetSubdirectoryInfo.Delete(true);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to delete {Subdirectory} assets", subdirectory);

            object? data = default;
#if DEBUG
            data = exception;
#endif

            return InternalServerError(data);
        }
    }

    private bool TryGenerateUpdateManifestFrom(string subdirectory, [NotNullWhen(true)] out UpdateManifest? updateManifest)
    {
        try
        {
            var assetSubdirectoryPath = Path.Combine(AssetRootPath, subdirectory);
            DirectoryInfo assetSubdirectoryInfo = new(assetSubdirectoryPath);
            if (!assetSubdirectoryInfo.Exists)
            {
                updateManifest = default;
                return false;
            }

            HashSet<FileSystemInfo> visited = new();
            Queue<FileSystemInfo> scanQueue = new();
            scanQueue.Enqueue(assetSubdirectoryInfo);

            updateManifest = new UpdateManifest
            {
                StreamingUrl = $"/assets/stream/{subdirectory}",
                TrustCache = true,
            };

            while (scanQueue.TryDequeue(out var currentFileSystemInfo))
            {
                if (visited.TryGetValue(currentFileSystemInfo, out var collidingFileSystemInfo))
                {
                    if (collidingFileSystemInfo == currentFileSystemInfo)
                    {
                        continue;
                    }
                }

                visited.Add(currentFileSystemInfo);

                switch (currentFileSystemInfo)
                {
                    case DirectoryInfo currentDirectoryInfo:
                    {
                        foreach (var childFileSystemInfo in currentDirectoryInfo.EnumerateFileSystemInfos())
                        {
                            scanQueue.Enqueue(childFileSystemInfo);
                        }

                        break;
                    }

                    case FileInfo currentFileInfo:
                    {
                        var updateManifestFile = UpdateManifestFile.From(currentFileInfo, assetSubdirectoryInfo.FullName);
                        updateManifest.Files.Add(updateManifestFile);
                        updateManifest.TotalSize += updateManifestFile.Size;
                        break;
                    }

                    default:
                        _logger.LogWarning(
                            "Unsupported {FileSystemInfo} type '{UnsupportedType}'",
                            typeof(FileSystemInfo).GetQualifiedName(),
                            currentFileSystemInfo.GetQualifiedTypeName()
                        );
                        break;
                }
            }

            updateManifest.Files.Sort();

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate update manifest for {Subdirectory} assets", subdirectory);

            updateManifest = default;
            return false;
        }
    }

    private IActionResult StreamAssets(string subdirectory, List<string> assetNames)
    {
        try
        {
            var assetSubdirectoryPath = Path.Combine(AssetRootPath, subdirectory);
            DirectoryInfo assetSubdirectoryInfo = new(assetSubdirectoryPath);
            if (!assetSubdirectoryInfo.Exists)
            {
                _logger.LogWarning(
                    "No assets in {Subdirectory} ({PathToSubdirectory})",
                    subdirectory,
                    assetSubdirectoryPath
                );
                return InternalServerError();
            }

            long totalSize = 0;
            List<(FileInfo Info, string Name)> assets = new();
            foreach (var assetName in assetNames)
            {
                var pathToAsset = Path.GetFullPath(assetName, assetSubdirectoryPath);
                var relativePathToAsset = Path.GetRelativePath(assetSubdirectoryPath, pathToAsset);
                if (relativePathToAsset.StartsWith(".."))
                {
                    _logger.LogWarning(
                        "{UserId} tried to download asset from outside the {Subdirectory} sandbox: {PathToAsset}",
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
                        subdirectory,
                        pathToAsset
                    );
                    continue;
                }

                FileInfo info = new(pathToAsset);
                assets.Add((info, assetName));
                totalSize += info.Length + sizeof(int) + sizeof(long) + Encoding.UTF8.GetBytes(assetName).Length;
            }

            return new UpdateStreamResult(assets, totalSize);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to stream one or more assets from {Subdirectory}", subdirectory);
            return InternalServerError();
        }
    }
}

public class UpdateStreamResult : IActionResult
{
    private readonly List<(FileInfo Info, string Name)> _assets;
    private readonly long _totalSize;

    public UpdateStreamResult(List<(FileInfo Info, string Name)> assets, long totalSize)
    {
        _assets = assets;
        _totalSize = totalSize;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<UpdateStreamResult>>();

        logger?.LogDebug("Beginning streaming {AssetCount} assets {TotalSize}...", _assets.Count, _totalSize);

        context.HttpContext.Response.ContentLength = _totalSize;
        context.HttpContext.Response.ContentType = "application/octet-stream";

        var stream = context.HttpContext.Response.Body;

        foreach (var (fileInfo, assetName) in _assets)
        {
            logger?.LogDebug("Beginning streaming {AssetName}...", assetName);

            var assetNameBytes = Encoding.UTF8.GetBytes(assetName);
            var assetNameLengthBuffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(assetNameLengthBuffer, assetNameBytes.Length);
            await stream.WriteAsync(assetNameLengthBuffer);
            await stream.WriteAsync(assetNameBytes);

            var fileInfoLengthBuffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(fileInfoLengthBuffer, fileInfo.Length);
            await stream.WriteAsync(fileInfoLengthBuffer);

            await using var fileStream = fileInfo.OpenRead();
            await fileStream.CopyToAsync(stream);
        }

        logger?.LogDebug("Completed streaming assets");
    }
}