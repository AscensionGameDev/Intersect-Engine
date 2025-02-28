namespace Intersect.Client.Framework.Graphics;

public interface IGPUBuffer : IDisposable
{
    uint Id { get; }

    int Count { get; }

    int SizeBytes { get; }
}