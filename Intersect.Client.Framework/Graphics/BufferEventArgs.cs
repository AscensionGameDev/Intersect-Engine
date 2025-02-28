namespace Intersect.Client.Framework.Graphics;

public class BufferEventArgs(IGPUBuffer gpuBuffer) : EventArgs
{
    public IGPUBuffer Buffer { get; } = gpuBuffer;
}