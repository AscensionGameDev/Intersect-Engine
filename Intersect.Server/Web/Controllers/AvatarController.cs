using System.Collections.Concurrent;
using System.Net;
using Intersect.Framework.IO;
using Intersect.Server.Collections.Indexing;
using Intersect.Server.Entities;
using Intersect.Server.Web.Http;
using Intersect.Server.Web.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace Intersect.Server.Web.Controllers;

[AllowAnonymous]
[Route("avatar")]
[ResponseCache(CacheProfileName = nameof(AvatarController))]
[OutputCache(PolicyName = nameof(AvatarController))]
public class AvatarController(ILogger<AvatarController> logger) : IntersectController
{
#if DEBUG
    private const int CacheSeconds = 30;
#else
    private const int CacheSeconds = 900;
#endif

    public static readonly CacheProfile ResponseCacheProfile = new()
    {
        Duration = CacheSeconds,
    };

    private record struct CachingResult(IActionResult? Result, string? Checksum, FileInfo? FileInfo);

    public static readonly Action<OutputCachePolicyBuilder> OutputCachePolicy =
        builder => builder.Expire(TimeSpan.FromSeconds(CacheSeconds)).Tag();

    private static readonly ConcurrentDictionary<string, Task<CachingResult>> CachingTasks = new();

    private readonly DirectoryInfo _cacheDirectoryInfo =
        new(Path.Combine(Environment.CurrentDirectory, ".cache", "avatars"));

    [HttpGet("player/{lookupKey:LookupKey}")]
    [EndpointSummary($"{nameof(AvatarController)}_{nameof(GetPlayerAvatarAsync)}_Summary")]
    [EndpointDescription($"{nameof(AvatarController)}_{nameof(GetPlayerAvatarAsync)}_Description")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, ContentTypes.Png)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    public async Task<IActionResult> GetPlayerAvatarAsync(LookupKey lookupKey)
    {
        DirectoryInfo assetsDirectoryInfo = new("assets/editor/resources");
        if (!assetsDirectoryInfo.Exists)
        {
            return InternalServerError("Error occurred while fetching the avatar");
        }

        if (lookupKey.IsInvalid)
        {
            return BadRequest($"Invalid lookup key: {lookupKey}");
        }

        if (!Player.TryFetch(lookupKey, out var player))
        {
            return NotFound($"Player not found for lookup key '{lookupKey}'");
        }

        if (!player.TryLoadAvatarName(out var avatarName, out var isFace))
        {
            return NotFound($"Avatar not found for player '{lookupKey}'");
        }

        var (result, checksum, fileInfo) = await ResolveAvatarAsync(assetsDirectoryInfo, avatarName, isFace);
        if (result == null)
        {
            if (fileInfo != null)
            {
                logger.LogWarning(
                    "Avatar '{AvatarName}' was found in {AssetsDirectory} but failed to be loaded and should be located at {AvatarFilePath}",
                    avatarName,
                    assetsDirectoryInfo.FullName,
                    fileInfo.FullName
                );
            }
            else
            {
                logger.LogWarning(
                    "Avatar '{AvatarName}' was not found in {AssetsDirectory}",
                    avatarName,
                    assetsDirectoryInfo.FullName
                );
            }
            return InternalServerError($"Error loading avatar for player '{lookupKey}'");
        }

        if (!string.IsNullOrWhiteSpace(checksum))
        {
            Response.Headers.ETag = checksum;
        }

        return result;
    }

    [HttpGet("user/{lookupKey:LookupKey}")]
    [EndpointSummary($"{nameof(AvatarController)}_{nameof(GetUserAvatarAsync)}_Summary")]
    [EndpointDescription($"{nameof(AvatarController)}_{nameof(GetUserAvatarAsync)}_Description")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, ContentTypes.Png)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.InternalServerError, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.BadRequest, ContentTypes.Json)]
    [ProducesResponseType(typeof(StatusMessageResponseBody), (int)HttpStatusCode.NotFound, ContentTypes.Json)]
    public async Task<IActionResult> GetUserAvatarAsync(LookupKey lookupKey)
    {
        DirectoryInfo assetsDirectoryInfo = new("assets/editor/resources");
        if (!assetsDirectoryInfo.Exists)
        {
            return InternalServerError("Error occurred while fetching the avatar");
        }

        if (lookupKey.IsInvalid)
        {
            return BadRequest($"Invalid lookup key: {lookupKey}");
        }

        if (!Database.PlayerData.User.TryFetch(lookupKey, out var user))
        {
            return NotFound($"User not found for lookup key '{lookupKey}'");
        }

        if (!user.TryLoadAvatarName(out _, out var avatarName, out var isFace))
        {
            return NotFound($"Avatar not found for user '{lookupKey}'");
        }

        var (result, checksum, fileInfo) = await ResolveAvatarAsync(assetsDirectoryInfo, avatarName, isFace);
        if (result == null)
        {
            if (fileInfo != null)
            {
                logger.LogWarning(
                    "Avatar '{AvatarName}' was found in {AssetsDirectory} but failed to be loaded and should be located at {AvatarFilePath}",
                    avatarName,
                    assetsDirectoryInfo.FullName,
                    fileInfo.FullName
                );
            }
            else
            {
                logger.LogWarning(
                    "Avatar '{AvatarName}' was not found in {AssetsDirectory}",
                    avatarName,
                    assetsDirectoryInfo.FullName
                );
            }
            return InternalServerError($"Error loading avatar for user '{lookupKey}'");
        }

        if (!string.IsNullOrWhiteSpace(checksum))
        {
            Response.Headers.ETag = checksum;
        }

        return result;
    }

    private async Task<CachingResult> ResolveAvatarAsync(
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
            return default;
        }

        if (isFace)
        {
            _ = avatarFileInfo.TryComputeChecksum(out var checksum);
            return new CachingResult(
                new PhysicalFileResult(avatarFileInfo.FullName, "image/png"),
                checksum,
                avatarFileInfo
            );
        }

        var relativeName = Path.GetRelativePath(assetsDirectoryInfo.FullName, avatarFileInfo.FullName);
        FileInfo cacheFileInfo = new(Path.Combine(_cacheDirectoryInfo.FullName, relativeName));

        var taskKey = cacheFileInfo.FullName;
        Task<CachingResult> task;
        lock (CachingTasks)
        {
            task = CachingTasks.GetOrAdd(
                taskKey,
                async (_, args) =>
                {
                    string? checksum;

                    var result = new PhysicalFileResult(args.cacheFileInfo.FullName, "image/png");
                    if (args.cacheFileInfo.Exists)
                    {
                        args.cacheFileInfo.TryComputeChecksum(out checksum);
                        return new CachingResult(result, checksum, args.cacheFileInfo);
                    }

                    var cacheParentDirectoryInfo = args.cacheFileInfo.Directory;

                    if (cacheParentDirectoryInfo is { Exists: false })
                    {
                        cacheParentDirectoryInfo.Create();
                    }

                    using var avatarImage = await Image.LoadAsync(new DecoderOptions(), args.avatarFileInfo.FullName);
                    var spritesOptions = Options.Instance.Sprites;
                    var horizontalFrames = spritesOptions.IdleFrames;
                    var verticalFrames = spritesOptions.Directions;

                    var minSize = Math.Min(avatarImage.Width / horizontalFrames, avatarImage.Height / verticalFrames);
                    avatarImage.Mutate(i => i.Crop(minSize, minSize));
                    await avatarImage.SaveAsync(args.cacheFileInfo.FullName);

                    args.cacheFileInfo.TryComputeChecksum(out checksum);
                    return new CachingResult(result, checksum, args.cacheFileInfo);
                },
                (avatarFileInfo, cacheFileInfo)
            );
        }

        var result = await task;

        // ReSharper disable once InconsistentlySynchronizedField
        _ = CachingTasks.TryRemove(taskKey, out _);

        return result;
    }
}