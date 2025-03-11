using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text;
using Htmx;
using Intersect.Framework.Core.AssetManagement;
using Intersect.Framework.Reflection;
using Intersect.Server.Web.Extensions;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Pages.Shared;
using Intersect.Server.Web.Types;
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

    private static ConcurrentDictionary<string, CachedManifest> _clientUpdateManifests = [];
    private static ConcurrentDictionary<string, CachedManifest> _editorUpdateManifests = [];

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
        }
    }

    [HttpGet("{**path}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AssetFileSystemInfo[]), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, ContentTypes.OctetStream)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
    [ProducesResponseType(
        typeof(StatusMessageResponseBody),
        (int)HttpStatusCode.InternalServerError,
        ContentTypes.Json
    )]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    public IActionResult AssetGet([FromRoute] string? path = default)
    {
        if (!(path?.StartsWith("client") ?? false))
        {
            if (!User.HasRole("Editor"))
            {
                return Forbidden($"No access to {path}");
            }
        }

        var assetRootPath = AssetRootPath;
        var partialPath = path?.Trim() ?? string.Empty;
        var pathToInspect = Path.Combine(assetRootPath, partialPath);

        var assetFileSystemInfo = AssetFileSystemInfo.From(assetRootPath, pathToInspect);
        if (assetFileSystemInfo == default)
        {
            // ReSharper disable once InvertIf
            if (RuntimeIdentifier is { } runtimeIdentifier && !string.IsNullOrWhiteSpace(runtimeIdentifier))
            {
                var segments = partialPath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
                var initialSegment = segments.FirstOrDefault();
                // ReSharper disable once InvertIf
                if (!string.IsNullOrWhiteSpace(initialSegment))
                {
                    var remainingSegments = segments.Skip(1).ToArray();
                    var combinedRemainingSegments = Path.Combine(remainingSegments);
                    var pathToBinary = Path.Combine(
                        assetRootPath,
                        initialSegment,
                        "binaries",
                        RuntimeIdentifier,
                        combinedRemainingSegments
                    );
                    FileInfo binaryFileInfo = new(pathToBinary);
                    if (binaryFileInfo.Exists)
                    {
                        return new PhysicalFileResult(binaryFileInfo.FullName, ContentTypes.OctetStream);
                    }
                }
            }

            return NotFound($"Path not found: {path}");
        }

        if (assetFileSystemInfo.Type == AssetFileSystemInfoType.File)
        {
            if (Request.GetTypedHeaders().Accept.Any(mt => mt.IsSubsetOf(MediaTypeApplicationJson)))
            {
                return Ok(
                    new[]
                    {
                        assetFileSystemInfo,
                    }
                );
            }

            if (!ContentTypeProvider.TryGetContentType(pathToInspect, out var contentType))
            {
                contentType = ContentTypes.OctetStream;
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
            _logger.LogError(exception, "Failed to retrieve results for {Path}", path);

#if DEBUG
            return InternalServerError(exception);
#else
            return InternalServerError($"An error occurred when retrieving results for {path}");
#endif
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
            if (assetFileSystemInfo == null)
            {
                _logger.LogWarning(
                    "No file system information found for '{SourcePath}' ({ResolvedSourcePath})",
                    sourcePath,
                    resolvedSourcePath
                );
                return BadRequest("Invalid source path");
            }
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

        var (destinationInfo, destinationParentInfo) =
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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Html)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.MultiStatus, ContentTypes.Html)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
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
                    Message = "No destination folder", Type = ToastModel.TypeError,
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
                    Message = "Destination folder does not exist", Type = ToastModel.TypeError,
                }
            );
            return partial;
        }

        DirectoryInfo directoryInfo = new(destinationFolder);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        Dictionary<string, (HttpStatusCode StatusCode, string? Message)> results = [];
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

        if (folder.StartsWith("client"))
        {
            _clientUpdateManifests.Clear();
        }
        else
        {
            _editorUpdateManifests.Clear();
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
                            Message = message, Type = type,
                        };
                    }
                )
                .ToArray()
        );
        aggregatePartial.StatusCode = (int)statusCode;
        return aggregatePartial;
    }

    [HttpPost("{**path}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    public IActionResult AssetPost(
        string? path = default,
        [FromHeader(Name = "Move-To")] string? destinationPath = default
    )
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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(
        typeof(StatusMessageResponseBody),
        (int)HttpStatusCode.InternalServerError,
        ContentTypes.Json
    )]
    public IActionResult BrowseDelete(string? path = default)
    {
        var assetRootPath = AssetRootPath;
        var pathToInspect = Path.Combine(assetRootPath, path?.Trim() ?? string.Empty);

        var assetFileSystemInfo = AssetFileSystemInfo.From(assetRootPath, pathToInspect);

        try
        {
            var fileSystemInfo = assetFileSystemInfo?.FileSystemInfo;
            if (fileSystemInfo is not { Exists: true })
            {
                return NotFound($"Unable to delete missing resource: {path}");
            }

            fileSystemInfo.Delete();
            return Ok(message: $"Deleted {path}");
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to delete asset: {Path}",
                string.IsNullOrWhiteSpace(path) ? "<empty>" : path
            );

#if DEBUG
            return InternalServerError(exception);
#else
            return InternalServerError(message: $"Failed to delete {path}");
#endif
        }
    }

    [HttpDelete("client")]
    public IActionResult ClearClientAssets()
    {
        return DeleteAssets("client");
    }

    [AllowAnonymous]
    [HttpGet("client/update.json")]
    [ProducesResponseType(typeof(UpdateManifest), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(
        typeof(StatusMessageResponseBody),
        (int)HttpStatusCode.InternalServerError,
        ContentTypes.Json
    )]
    public IActionResult GetClientUpdateManifest([FromQuery(Name = "rid")] string? runtimeIdentifier) =>
        GetUpdateManifest(_clientUpdateManifests, "client", runtimeIdentifier);

    private IActionResult GetUpdateManifest(ConcurrentDictionary<string, CachedManifest> cachedManifests, string manifestType, string? runtimeIdentifier)
    {
        try
        {
            Response.Headers.ContentType = "application/json";

            var resolvedRuntimeIdentifier = runtimeIdentifier ?? string.Empty;
            if (cachedManifests.TryGetValue(resolvedRuntimeIdentifier, out var cachedManifest))
            {
                if (!cachedManifest.IsExpired)
                {
                    return Ok(cachedManifest.Manifest);
                }
            }

            if (!TryGenerateUpdateManifestFrom(manifestType, runtimeIdentifier, out var manifest))
            {
                return InternalServerError($"Failed to generate {manifestType} manifest");
            }

            cachedManifest = new CachedManifest(manifest, DateTime.UtcNow.Add(ManifestCacheExpiry));
            cachedManifests[resolvedRuntimeIdentifier] = cachedManifest;
            return Ok(manifest);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to generate {ManifestType} manifest due to an exception", manifestType);

#if DEBUG
            return InternalServerError(exception);
#else
            return InternalServerError($"Failed to generate {manifestType} manifest");
#endif
        }
    }

    [AllowAnonymous]
    [HttpPost("stream/client")]
    public IActionResult StreamClientAssets([FromBody] List<string> assetNames)
    {
        return StreamAssets("client", assetNames);
    }

    [HttpPost("stream/editor")]
    public IActionResult StreamEditorAssets([FromBody] List<string> assetNames)
    {
        return StreamAssets("editor", assetNames);
    }


    [HttpDelete("client/update.json")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ClearClientUpdateManifestCache()
    {
        _clientUpdateManifests.Clear();
        return Ok("Cache cleared");
    }

    [HttpDelete("editor")]
    public IActionResult ClearEditorAssets()
    {
        return DeleteAssets("editor");
    }

    [HttpGet("editor/update.json")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    [ProducesResponseType(
        typeof(StatusMessageResponseBody),
        (int)HttpStatusCode.InternalServerError,
        ContentTypes.Json
    )]
    public IActionResult GetEditorUpdateManifest([FromQuery(Name = "rid")] string? runtimeIdentifier) =>
        GetUpdateManifest(_editorUpdateManifests, "editor", runtimeIdentifier);

    [HttpDelete("editor/update.json")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK, ContentTypes.Json)]
    public IActionResult ClearEditorUpdateManifestCache()
    {
        _editorUpdateManifests.Clear();
        return Ok("Cache cleared");
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

            return Ok(message: $"Deleted {subdirectory}");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to delete {Subdirectory} assets", subdirectory);

#if DEBUG
            return InternalServerError(exception);
#else
            return InternalServerError(message: $"Failed to delete {subdirectory}");
#endif
        }
    }

    private record struct RemappedFileSystemInfo(FileSystemInfo Info, FileSystemInfo? MappedTo = null);

    private bool TryGenerateUpdateManifestFrom(
        string subdirectory,
        string? runtimeIdentifier,
        [NotNullWhen(true)] out UpdateManifest? updateManifest
    )
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

            Dictionary<string, RemappedFileSystemInfo> visited = [];
            Queue<RemappedFileSystemInfo> scanQueue = [];
            scanQueue.Enqueue(new RemappedFileSystemInfo(assetSubdirectoryInfo));

            var pathToBinaries = Path.GetFullPath("binaries", assetSubdirectoryInfo.FullName);
            DirectoryInfo binariesSubdirectoryInfo = new(pathToBinaries);
            visited.Add(binariesSubdirectoryInfo.FullName, new RemappedFileSystemInfo(binariesSubdirectoryInfo));

            if (!string.IsNullOrWhiteSpace(runtimeIdentifier))
            {
                var pathToRuntime = Path.GetFullPath(runtimeIdentifier, pathToBinaries);
                DirectoryInfo runtimeBinariesSubdirectoryInfo = new(pathToRuntime);
                if (runtimeBinariesSubdirectoryInfo.Exists)
                {
                    scanQueue.Enqueue(
                        new RemappedFileSystemInfo(runtimeBinariesSubdirectoryInfo, assetSubdirectoryInfo)
                    );
                }
            }

            updateManifest = new UpdateManifest
            {
                StreamingUrl = $"/assets/stream/{subdirectory}",
                TrustCache = true,
            };

            while (scanQueue.TryDequeue(out var currentRemappedFileSystemInfo))
            {
                if (visited.TryGetValue(currentRemappedFileSystemInfo.Info.FullName, out var collidingFileSystemInfo))
                {
                    if (string.Equals(
                            collidingFileSystemInfo.Info.FullName,
                            currentRemappedFileSystemInfo.Info.FullName
                        ))
                    {
                        continue;
                    }
                }

                var (currentFileSystemInfo, currentMappedTo) = currentRemappedFileSystemInfo;

                visited.Add(currentFileSystemInfo.FullName, currentRemappedFileSystemInfo);

                switch (currentFileSystemInfo)
                {
                    case DirectoryInfo currentDirectoryInfo:
                    {
                        foreach (var childFileSystemInfo in currentDirectoryInfo.EnumerateFileSystemInfos())
                        {
                            FileSystemInfo? childMappedTo = null;
                            if (currentMappedTo != null)
                            {
                                var relativePathToInfo = Path.GetRelativePath(
                                    currentDirectoryInfo.FullName,
                                    childFileSystemInfo.FullName
                                );
                                var remappedRelativePathToInfo = Path.Combine(
                                    currentMappedTo.FullName,
                                    relativePathToInfo
                                );
                                var resolvedRelativePathToInfo = Path.GetFullPath(remappedRelativePathToInfo);
                                childMappedTo = childFileSystemInfo switch
                                {
                                    DirectoryInfo _ => new DirectoryInfo(resolvedRelativePathToInfo),
                                    FileInfo _ => new FileInfo(resolvedRelativePathToInfo),
                                    _ => throw new NotImplementedException(
                                        $"Handling for type {childFileSystemInfo.GetType().GetName(qualified: true)} not implemented"
                                    ),
                                };
                            }
                            scanQueue.Enqueue(new RemappedFileSystemInfo(childFileSystemInfo, childMappedTo));
                        }

                        break;
                    }

                    case FileInfo currentFileInfo:
                    {
                        var currentMappedToFileInfo = currentMappedTo as FileInfo;
                        if (currentMappedTo is not null and not FileInfo)
                        {
                            var expectedTypeName = typeof(FileInfo).GetName(qualified: true);
                            var actualTypeName = currentMappedTo.GetType().GetName(qualified: true);
                            var originalPath = currentFileInfo.FullName;
                            var mappedPath = currentMappedTo.FullName;
                            throw new InvalidOperationException(
                                $"{expectedTypeName} was mapped to {actualTypeName} ('{originalPath}' => '{mappedPath}')"
                            );
                        }

                        var updateManifestFile = UpdateManifestFile.From(
                            currentFileInfo,
                            assetSubdirectoryInfo.FullName,
                            currentMappedToFileInfo
                        );
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
            List<(FileInfo Info, string Name)> assets = [];
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