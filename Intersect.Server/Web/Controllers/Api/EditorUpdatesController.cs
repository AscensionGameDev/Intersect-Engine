using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using Intersect.Server.Web.Controllers.AssetManagement;
using Intersect.Server.Web.Extensions;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Intersect.Server.Web.Controllers.Api;

/// <summary>
/// API endpoints for uploading game updates from the editor to the server.
/// These endpoints integrate with the existing asset management system.
/// </summary>
[Route("api/v1/editor/updates")]
[ApiController]
[Authorize(Policy = "Developer")]
public sealed class EditorUpdatesController : ControllerBase
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<EditorUpdatesController> _logger;
    private readonly IOptionsMonitor<UpdateServerOptions> _updateServerOptionsMonitor;

    // Reference to AssetsController's manifest caches for invalidation
    // These are in AssetsController as static fields, we'll need to access them via reflection
    // or implement cache clearing differently
    private static readonly object _cacheLock = new();

    public EditorUpdatesController(
        IHostEnvironment hostEnvironment,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<UpdateServerOptions> updateServerOptionsMonitor
    )
    {
        _hostEnvironment = hostEnvironment;
        _logger = loggerFactory.CreateLogger<EditorUpdatesController>();
        _updateServerOptionsMonitor = updateServerOptionsMonitor;
    }

    private string AssetRootPath => Path.Combine(
        _hostEnvironment.ContentRootPath,
        _updateServerOptionsMonitor.CurrentValue.AssetRoot
    );

    /// <summary>
    /// Upload client update files from the editor.
    /// Files are saved to the assets/client directory and will be automatically
    /// included in the next client update manifest.
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="subfolder">Optional subfolder within assets/client (e.g., "resources", "resources/images")</param>
    /// <returns>Upload results for each file</returns>
    [HttpPost("client")]
    [ProducesResponseType(typeof(UploadResponse), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(UploadResponse), (int)HttpStatusCode.MultiStatus, ContentTypes.Json)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
    public async Task<IActionResult> UploadClientFiles(
        [FromForm] IFormFileCollection files,
        [FromForm] string? subfolder = null
    )
    {
        return await UploadFilesInternal("client", files, subfolder);
    }

    /// <summary>
    /// Upload editor update files from the editor.
    /// Files are saved to the assets/editor directory and will be automatically
    /// included in the next editor update manifest.
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <param name="subfolder">Optional subfolder within assets/editor</param>
    /// <returns>Upload results for each file</returns>
    [HttpPost("editor")]
    [ProducesResponseType(typeof(UploadResponse), (int)HttpStatusCode.OK, ContentTypes.Json)]
    [ProducesResponseType(typeof(UploadResponse), (int)HttpStatusCode.MultiStatus, ContentTypes.Json)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden, ContentTypes.Json)]
    public async Task<IActionResult> UploadEditorFiles(
        [FromForm] IFormFileCollection files,
        [FromForm] string? subfolder = null
    )
    {
        return await UploadFilesInternal("editor", files, subfolder);
    }

    private async Task<IActionResult> UploadFilesInternal(
        string targetType,
        IFormFileCollection files,
        string? subfolder
    )
    {
        if (!_updateServerOptionsMonitor.CurrentValue.Enabled)
        {
            return NotFound(new ErrorResponse { Error = "Update server is not enabled" });
        }

        if (files == null || files.Count == 0)
        {
            return BadRequest(new ErrorResponse { Error = "No files were uploaded" });
        }

        // Construct the destination folder (e.g., assets/client or assets/client/resources)
        var assetRootPath = AssetRootPath;
        var relativePath = string.IsNullOrWhiteSpace(subfolder)
            ? targetType
            : Path.Combine(targetType, subfolder.Trim().Trim('/').Trim('\\'));

        var destinationFolder = Path.GetFullPath(Path.Combine(assetRootPath, relativePath));
        var relativeDestinationFolder = Path.GetRelativePath(assetRootPath, destinationFolder);

        // Security: Ensure uploads stay within the asset root
        if (relativeDestinationFolder.StartsWith(".."))
        {
            _logger.LogWarning(
                "{UserId} tried to upload to a folder outside of the sandbox: {DirectoryPath}",
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                destinationFolder
            );
            return StatusCode(
                (int)HttpStatusCode.Forbidden,
                new ErrorResponse { Error = "Invalid destination path" }
            );
        }

        // Ensure target type matches (prevent uploading to wrong directory)
        if (!relativeDestinationFolder.StartsWith(targetType))
        {
            return BadRequest(new ErrorResponse
            {
                Error = $"Invalid subfolder path - must be within {targetType} directory"
            });
        }

        // Create destination directory if it doesn't exist
        var directoryInfo = new DirectoryInfo(destinationFolder);
        if (!directoryInfo.Exists)
        {
            _logger.LogInformation(
                "Creating directory for updates: {DirectoryPath}",
                relativeDestinationFolder
            );
            directoryInfo.Create();
        }

        // Process each file
        var results = new Dictionary<string, FileUploadResult>();
        long totalBytesUploaded = 0;

        foreach (var file in files)
        {
            if (results.ContainsKey(file.FileName))
            {
                results[file.FileName] = new FileUploadResult
                {
                    Success = false,
                    Message = "Duplicate file name in upload",
                    StatusCode = HttpStatusCode.BadRequest
                };
                continue;
            }

            var resolvedFilePath = Path.GetFullPath(Path.Combine(directoryInfo.FullName, file.FileName));
            var relativeFilePath = Path.GetRelativePath(directoryInfo.FullName, resolvedFilePath);

            // Security: Ensure file stays within destination folder
            if (relativeFilePath.StartsWith(".."))
            {
                _logger.LogWarning(
                    "{UserId} tried to upload a file outside of its folder: {FilePath}",
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    resolvedFilePath
                );
                results[file.FileName] = new FileUploadResult
                {
                    Success = false,
                    Message = "Invalid file path",
                    StatusCode = HttpStatusCode.Forbidden
                };
                continue;
            }

            var fileInfo = new FileInfo(resolvedFilePath);

            try
            {
                // Write the file (overwrites if exists - this is intentional for updates)
                using var networkStream = file.OpenReadStream();
                using var targetStream = fileInfo.Open(
                    fileInfo.Exists ? FileMode.Truncate : FileMode.Create,
                    FileAccess.Write
                );
                await networkStream.CopyToAsync(targetStream);

                totalBytesUploaded += file.Length;

                var relativeToAssetRoot = Path.GetRelativePath(assetRootPath, resolvedFilePath);
                results[file.FileName] = new FileUploadResult
                {
                    Success = true,
                    Message = $"Successfully uploaded to {relativeToAssetRoot}",
                    StatusCode = HttpStatusCode.OK,
                    Size = file.Length,
                    Path = relativeToAssetRoot
                };

                _logger.LogInformation(
                    "Uploaded {FileName} ({Size:N0} bytes) to {Path}",
                    file.FileName,
                    file.Length,
                    relativeToAssetRoot
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Upload of {FilePath} failed",
                    Path.GetRelativePath(assetRootPath, resolvedFilePath)
                );
                results[file.FileName] = new FileUploadResult
                {
                    Success = false,
                    Message = $"Upload failed: {exception.Message}",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // Invalidate manifest caches so new manifests are generated
        // We need to clear the caches in AssetsController
        ClearManifestCaches(targetType);

        // Determine overall status code
        var hasFailures = results.Values.Any(r => !r.Success);
        var hasSuccesses = results.Values.Any(r => r.Success);
        var overallStatusCode = hasFailures && hasSuccesses
            ? HttpStatusCode.MultiStatus
            : hasFailures
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

        var response = new UploadResponse
        {
            Success = !hasFailures,
            TotalFiles = files.Count,
            SuccessfulFiles = results.Values.Count(r => r.Success),
            FailedFiles = results.Values.Count(r => !r.Success),
            TotalBytes = totalBytesUploaded,
            TargetDirectory = relativeDestinationFolder,
            Files = results
        };

        return StatusCode((int)overallStatusCode, response);
    }

    /// <summary>
    /// Clear manifest caches in AssetsController by using reflection.
    /// This is necessary because the caches are static private fields in AssetsController.
    /// </summary>
    private void ClearManifestCaches(string targetType)
    {
        try
        {
            var assetsControllerType = typeof(AssetManagement.AssetsController);

            // Clear the appropriate cache based on target type
            var cacheFieldName = targetType.StartsWith("client")
                ? "_clientUpdateManifests"
                : "_editorUpdateManifests";

            var cacheField = assetsControllerType.GetField(
                cacheFieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            if (cacheField?.GetValue(null) is IDictionary cache)
            {
                lock (_cacheLock)
                {
                    cache.Clear();
                    _logger.LogInformation("Cleared {CacheType} manifest cache", targetType);
                }
            }
        }
        catch (Exception ex)
        {
            // Non-critical - cache will eventually expire anyway
            _logger.LogWarning(ex, "Failed to clear manifest cache, cache will expire naturally");
        }
    }
}

#region Response Models

public class UploadResponse
{
    public bool Success { get; set; }
    public int TotalFiles { get; set; }
    public int SuccessfulFiles { get; set; }
    public int FailedFiles { get; set; }
    public long TotalBytes { get; set; }
    public string TargetDirectory { get; set; } = string.Empty;
    public Dictionary<string, FileUploadResult> Files { get; set; } = new();
}

public class FileUploadResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public long Size { get; set; }
    public string? Path { get; set; }
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
}

#endregion
