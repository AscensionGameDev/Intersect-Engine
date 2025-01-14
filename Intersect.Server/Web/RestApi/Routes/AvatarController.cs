using System.Collections.Concurrent;
using Intersect.Server.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace Intersect.Server.Web.Controllers;

[AllowAnonymous]
[Route("avatar")]
// Does not need API access do not extend IntersectController
public class AvatarController : Controller
{
    private static readonly ConcurrentDictionary<string, Task<IActionResult>> CachingTasks = new();

    private readonly DirectoryInfo _cacheDirectoryInfo;
    private readonly ILogger<AvatarController> _logger;

    public AvatarController(ILogger<AvatarController> logger)
    {
        _cacheDirectoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, ".cache", "avatars"));
        _logger = logger;
    }

    [HttpGet("player/{playerId:guid}")]
    public async Task<IActionResult> GetPlayerAvatarAsync(Guid playerId)
    {
        DirectoryInfo assetsDirectoryInfo = new("assets/editor/resources");
        if (!assetsDirectoryInfo.Exists)
        {
            return new NotFoundResult();
        }

        var player = Player.Find(playerId);
        if (player == default)
        {
            return new NotFoundResult();
        }

        return !player.TryLoadAvatarName(out var avatarName, out var isFace)
            ? new NotFoundResult()
            : await ResolveAvatarAsync(assetsDirectoryInfo, avatarName, isFace);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserAvatarAsync(Guid userId)
    {
        DirectoryInfo assetsDirectoryInfo = new("assets/editor/resources");
        if (!assetsDirectoryInfo.Exists)
        {
            return new NotFoundResult();
        }

        var user = Database.PlayerData.User.FindById(userId);
        if (user == default)
        {
            return new NotFoundResult();
        }

        return !user.TryLoadAvatarName(out var avatarName, out var isFace)
            ? new NotFoundResult()
            : await ResolveAvatarAsync(assetsDirectoryInfo, avatarName, isFace);
    }

    private async Task<IActionResult> ResolveAvatarAsync(
        DirectoryInfo assetsDirectoryInfo,
        string avatarName,
        bool isFace
    )
    {
        var directoryInfos = isFace
            ? assetsDirectoryInfo.EnumerateDirectories("faces")
            : assetsDirectoryInfo.EnumerateDirectories("entities");

        var fileInfos = directoryInfos.SelectMany(di => di.EnumerateFiles()).Where(
            f => string.Equals(f.Name, avatarName, StringComparison.OrdinalIgnoreCase)
        );

        var avatarFileInfo = fileInfos.FirstOrDefault();

        if (avatarFileInfo == default)
        {
            return new NotFoundResult();
        }

        if (isFace)
        {
            return new PhysicalFileResult(avatarFileInfo.FullName, "image/png");
        }

        var relativeName = Path.GetRelativePath(assetsDirectoryInfo.FullName, avatarFileInfo.FullName);
        FileInfo cacheFileInfo = new(Path.Combine(_cacheDirectoryInfo.FullName, relativeName));

        var taskKey = cacheFileInfo.FullName;
        Task<IActionResult> task;
        lock (CachingTasks)
        {
            task = CachingTasks.GetOrAdd(
                taskKey,
                async _ =>
                {
                    if (cacheFileInfo.Exists)
                    {
                        return new PhysicalFileResult(cacheFileInfo.FullName, "image/png");
                    }

                    var cacheParentDirectoryInfo = cacheFileInfo.Directory;

                    if (cacheParentDirectoryInfo is { Exists: false })
                    {
                        cacheParentDirectoryInfo.Create();
                    }

                    using var avatarImage = await Image.LoadAsync(new DecoderOptions(), avatarFileInfo.FullName);
                    var spritesOptions = Options.Instance.Sprites;
                    var horizontalFrames = spritesOptions.IdleFrames;
                    var verticalFrames = spritesOptions.Directions;

                    var minSize = Math.Min(avatarImage.Width / horizontalFrames, avatarImage.Height / verticalFrames);
                    avatarImage.Mutate(i => i.Crop(minSize, minSize));
                    await avatarImage.SaveAsync(cacheFileInfo.FullName);

                    return new PhysicalFileResult(cacheFileInfo.FullName, "image/png");
                }
            );
        }

        var result = await task;

        // ReSharper disable once InconsistentlySynchronizedField
        _ = CachingTasks.TryRemove(taskKey, out _);

        return result;
    }
}