using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Server.Core.Updates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api
{
    /// 
    /// API endpoints for game update distribution
    /// 
    [Route("api/v1/updates")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        private readonly UpdateService _updateService;

        public UpdatesController(UpdateService updateService)
        {
            _updateService = updateService;
        }

        /// 
        /// Upload update files (requires admin authorization)
        /// 
        /// 
        /// POST /api/v1/updates/upload
        /// 
        /// Upload game update files that clients will download.
        /// Requires administrator API access.
        /// Files should be sent as multipart/form-data.
        /// 
        /// Example using curl:
        /// ```
        /// curl -X POST http://localhost:5400/api/v1/updates/upload \
        ///   -H "Authorization: Bearer YOUR_TOKEN" \
        ///   -F "version=1.0.1" \
        ///   -F "files=@update.zip"
        /// ```
        /// 
        /// Version string for this update (e.g., "1.0.1")
        /// Collection of update files to upload
        /// Result of the upload operation
        [HttpPost("upload")]
        [Authorize] // Requires authentication
        [ProducesResponseType(typeof(UpdateUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task UploadUpdate([FromForm] string version, [FromForm] IFormFileCollection files)
        {
            try
            {
                // TODO: Verify admin access - check how other controllers do this
                // Look at existing controllers for the pattern, might be something like:
                // if (!User.IsInRole("Admin")) { return Forbid(); }
                // or
                // if (!User.HasClaim("Power", "Admin")) { return Forbid(); }

                if (string.IsNullOrWhiteSpace(version))
                {
                    return BadRequest(new ErrorResponse { Error = "Version parameter is required" });
                }

                if (files == null || files.Count == 0)
                {
                    return BadRequest(new ErrorResponse { Error = "No files were uploaded" });
                }

                // Convert IFormFiles to dictionary
                var fileData = new Dictionary();
                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        continue; // Skip empty files
                    }

                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileData[file.FileName] = ms.ToArray();
                    }
                }

                if (fileData.Count == 0)
                {
                    return BadRequest(new ErrorResponse { Error = "No valid files to upload" });
                }

                // Process the upload
                var result = await Task.Run(() => _updateService.ProcessUpdateUpload(version, fileData));

                if (result.Success)
                {
                    return Ok(new UpdateUploadResponse
                    {
                        Success = true,
                        Version = result.Version,
                        FileCount = result.FileCount,
                        TotalSize = result.TotalSize,
                        Message = result.Message
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        new ErrorResponse { Error = result.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Internal server error: {ex.Message}" });
            }
        }

        /// 
        /// Get the current update manifest
        /// 
        /// 
        /// GET /api/v1/updates/manifest
        /// 
        /// Public endpoint that returns the current update manifest.
        /// Clients use this to check for available updates.
        /// No authentication required.
        /// 
        /// The current update manifest with version and file list
        [HttpGet("manifest")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UpdateManifest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetManifest()
        {
            try
            {
                var manifest = _updateService.GetManifest();
                return Ok(manifest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Failed to get manifest: {ex.Message}" });
            }
        }

        /// 
        /// Download a specific update file
        /// 
        /// 
        /// GET /api/v1/updates/files/{filename}
        /// 
        /// Public endpoint for downloading update files.
        /// File integrity should be verified using the hash from the manifest.
        /// No authentication required.
        /// 
        /// Name of the file to download
        /// The requested file as a binary download
        [HttpGet("files/{fileName}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                var fileData = _updateService.GetUpdateFile(fileName);
                
                if (fileData == null)
                {
                    return NotFound(new ErrorResponse { Error = $"File '{fileName}' not found" });
                }

                // Determine content type based on extension
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".zip" => "application/zip",
                    ".exe" => "application/x-msdownload",
                    ".dll" => "application/x-msdownload",
                    ".json" => "application/json",
                    _ => "application/octet-stream"
                };

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Failed to get file: {ex.Message}" });
            }
        }

        /// 
        /// Check if an update is available
        /// 
        /// 
        /// GET /api/v1/updates/check?currentVersion={version}
        /// 
        /// Convenience endpoint to check if there's a newer version available.
        /// No authentication required.
        /// 
        /// The client's current version
        /// Information about available updates
        [HttpGet("check")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UpdateCheckResponse), StatusCodes.Status200OK)]
        public IActionResult CheckForUpdates([FromQuery] string currentVersion)
        {
            try
            {
                var manifest = _updateService.GetManifest();
                var updateAvailable = !string.Equals(currentVersion, manifest.Version, 
                    StringComparison.OrdinalIgnoreCase);

                return Ok(new UpdateCheckResponse
                {
                    UpdateAvailable = updateAvailable,
                    CurrentVersion = currentVersion,
                    LatestVersion = manifest.Version,
                    FileCount = manifest.Files?.Count ?? 0,
                    TotalSize = manifest.Files?.Sum(f => f.Size) ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ErrorResponse { Error = $"Failed to check for updates: {ex.Message}" });
            }
        }
    }

    #region Response Models

    public class UpdateUploadResponse
    {
        public bool Success { get; set; }
        public string Version { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
        public string Message { get; set; }
    }

    public class UpdateCheckResponse
    {
        public bool UpdateAvailable { get; set; }
        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }

    #endregion
}
