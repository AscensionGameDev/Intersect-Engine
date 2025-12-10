using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Server.Updates;
using Intersect.Server.Web.RestApi.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        private readonly UpdateService _updateService;

        public UpdatesController(UpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task UploadUpdate([FromForm] string version, [FromForm] IFormFileCollection files)
        {
            try
            {
                if (string.IsNullOrEmpty(version))
                {
                    return BadRequest(new { error = "Version is required" });
                }

                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { error = "No files uploaded" });
                }

                var fileData = new Dictionary();
                foreach (var file in files)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileData[file.FileName] = ms.ToArray();
                    }
                }

                var success = await _updateService.ProcessUpdateUpload(version, fileData);

                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        version = version,
                        fileCount = fileData.Count,
                        message = "Update uploaded successfully"
                    });
                }
                else
                {
                    return StatusCode(500, new { error = "Failed to process update" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("/updates/manifest.json")]
        [AllowAnonymous]
        public async Task GetManifest()
        {
            var manifest = await _updateService.GetManifest();
            return Ok(manifest);
        }

        [HttpGet("/updates/files/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetFile(string fileName)
        {
            fileName = System.IO.Path.GetFileName(fileName);

            var fileData = _updateService.GetUpdateFile(fileName);
            if (fileData == null)
            {
                return NotFound(new { error = "File not found" });
            }

            return File(fileData, "application/octet-stream", fileName);
        }
    }
}
