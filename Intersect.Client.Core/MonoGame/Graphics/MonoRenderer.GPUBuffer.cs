using System.Collections.Concurrent;
using Intersect.Client.Framework.Graphics;
using Intersect.Framework;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer
{
    private static Microsoft.Xna.Framework.Graphics.BufferUsage BufferUsageToMonoGameBufferUsage(BufferUsage usage)
    {
        Microsoft.Xna.Framework.Graphics.BufferUsage platformUsage = usage switch
        {
            BufferUsage.None => Microsoft.Xna.Framework.Graphics.BufferUsage.None,
            BufferUsage.WriteOnly => Microsoft.Xna.Framework.Graphics.BufferUsage.WriteOnly,
            _ => throw Exceptions.UnreachableInvalidEnum(usage),
        };
        return platformUsage;
    }

    private abstract class GPUBuffer : IGPUBuffer
    {
        private static readonly ConcurrentDictionary<Type, uint> NextIdLookup = [];

        protected GPUBuffer()
        {
            Id = NextIdLookup.AddOrUpdate(GetType(), _ => 1, (_, nextId) => nextId + 1);
        }

        public uint Id { get; }

        public abstract int Count { get; }

        public abstract int SizeBytes { get; }

        protected virtual void ReleaseUnmanagedResources()
        {
        }


        protected virtual void ReleaseManagedResources()
        {
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                ReleaseManagedResources();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GPUBuffer()
        {
            Dispose(false);
        }
    }
}