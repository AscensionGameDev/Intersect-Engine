namespace Intersect.Client.Framework.Graphics;

/// <summary>
/// A usage hint for optimizing memory placement of graphics buffers.
/// </summary>
public enum BufferUsage
{
    /// <summary>
    /// No special usage.
    /// </summary>
    None,

    /// <summary>
    /// The buffer will not be readable and will be optimized for rendering and writing.
    /// </summary>
    WriteOnly
}
