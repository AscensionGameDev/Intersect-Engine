namespace Intersect.Client.Framework.Graphics;

public record struct ScreenshotRequest(Func<Stream> StreamFactory, string? Hint = default);