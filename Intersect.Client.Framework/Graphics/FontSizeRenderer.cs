namespace Intersect.Client.Framework.Graphics;

public abstract class FontSizeRenderer<TPlatformObject>(TPlatformObject platformObject)
{
    public TPlatformObject PlatformObject { get; } = platformObject;
}