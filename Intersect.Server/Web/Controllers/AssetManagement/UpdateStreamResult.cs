using System.Buffers.Binary;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.AssetManagement;

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