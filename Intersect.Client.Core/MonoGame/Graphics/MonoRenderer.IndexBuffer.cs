using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Intersect.Client.Framework.Graphics;
using Intersect.Core;
using Intersect.Framework;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using BufferUsage = Intersect.Client.Framework.Graphics.BufferUsage;
using PrimitiveType = Intersect.Client.Framework.Graphics.PrimitiveType;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer
{
    private readonly ConcurrentDictionary<Microsoft.Xna.Framework.Graphics.IndexBuffer, IIndexBuffer> _allocatedIndexBuffers = [];
    private long _allocatedIndexBuffersSize;

    private bool AddAllocatedIndexBuffer(Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer, IIndexBuffer buffer)
    {
        if (!_allocatedIndexBuffers.TryAdd(platformBuffer, buffer))
        {
            return false;
        }

        MarkAllocated(buffer);

        _allocatedIndexBuffersSize += MeasureDataSize(platformBuffer);
        return true;
    }

    private bool RemoveAllocatedIndexBuffer(Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer, [NotNullWhen(true)] out IIndexBuffer? buffer)
    {
        if (!_allocatedIndexBuffers.TryRemove(platformBuffer, out buffer))
        {
            return false;
        }

        _allocatedIndexBuffersSize -= MeasureDataSize(platformBuffer);
        return true;
    }

    private static long MeasureDataSize(Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer)
    {
        return platformBuffer.IndexCount *
               platformBuffer.IndexElementSize switch
               {
                   IndexElementSize.SixteenBits => 2,
                   IndexElementSize.ThirtyTwoBits => 4,
                   _ => throw Exceptions.UnreachableInvalidEnum(platformBuffer.IndexElementSize),
               };
    }

    public override IIndexBuffer CreateIndexBuffer<TIndex>(int count, BufferUsage usage = BufferUsage.None, bool dynamic = false) where TIndex : struct
    {
        Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer;
        var platformUsage = BufferUsageToMonoGameBufferUsage(usage);

        if (dynamic)
        {
            platformBuffer = new DynamicIndexBuffer(_graphicsDevice, typeof(TIndex), count, platformUsage);
        }
        else
        {
            platformBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(
                _graphicsDevice,
                typeof(TIndex),
                count,
                platformUsage
            );
        }

        IndexBuffer buffer = new(platformBuffer, typeof(TIndex));
        platformBuffer.Disposing += IndexBufferOnDisposing;
        if (AddAllocatedIndexBuffer(platformBuffer, buffer))
        {
            return buffer;
        }

        buffer.Dispose();
        throw new InvalidOperationException("Unable to track index buffer");
    }

    private void IndexBufferOnDisposing(object? sender, EventArgs args)
    {
        if (sender is Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer)
        {
            OnPlatformIndexBufferDisposal(platformBuffer);
            return;
        }

        ApplicationContext.CurrentContext.Logger.LogError(
            "Received disposal event but it was not from an instance of {ExpectedType} ({ActualType})",
            typeof(Microsoft.Xna.Framework.Graphics.IndexBuffer).GetName(qualified: true),
            sender?.GetType().GetName(qualified: true) ?? "null"
        );
    }

    private void OnPlatformIndexBufferDisposal(Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer)
    {
        if (RemoveAllocatedIndexBuffer(platformBuffer, out var buffer))
        {
            MarkFreed(buffer);
        }
        else
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Failed to remove platform buffer from allocations, is it not tracked? '{BufferId}'",
                platformBuffer.ToString()
            );
        }
    }

    private sealed class IndexBuffer(Microsoft.Xna.Framework.Graphics.IndexBuffer platformBuffer, Type indexType) : GPUBuffer, IIndexBuffer
    {
        internal readonly Microsoft.Xna.Framework.Graphics.IndexBuffer PlatformBuffer = platformBuffer;

        public Type IndexType { get; } = indexType;

        public override int Count => PlatformBuffer.IndexCount;

        public override int SizeBytes  => (int)MeasureDataSize(PlatformBuffer);

        protected override void ReleaseManagedResources()
        {
            base.ReleaseManagedResources();

            if (!PlatformBuffer.IsDisposed)
            {
                PlatformBuffer.Dispose();
            }
        }

        public bool GetData<TIndex>(TIndex[] destination) where TIndex : struct =>
            GetData(bufferOffset: 0, destination: destination, destinationOffset: 0, length: destination.Length);

        public bool GetData<TIndex>(TIndex[] destination, int destinationOffset, int length) where TIndex : struct =>
            GetData(bufferOffset: 0, destination: destination, destinationOffset: destinationOffset, length: length);

        public bool GetData<TIndex>(int bufferOffset, TIndex[] destination, int destinationOffset, int length)
            where TIndex : struct
        {
            try
            {
                PlatformBuffer.GetData(
                    offsetInBytes: bufferOffset,
                    data: destination,
                    startIndex: destinationOffset,
                    elementCount: length
                );
                return true;
            }
            catch (Exception exception)
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    exception,
                    "Failed to get data for {BufferType} {Id}",
                    nameof(IIndexBuffer),
                    Id
                );
                return false;
            }
        }

        public bool SetData<TIndex>(TIndex[] data) where TIndex : struct => SetData(
            destinationOffset: 0,
            data: data,
            sourceOffset: 0,
            length: data.Length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TIndex>(TIndex[] data, int sourceOffset, int length)
            where TIndex : struct => SetData(
            destinationOffset: 0,
            data: data,
            sourceOffset: sourceOffset,
            length: length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TIndex>(int destinationOffset, TIndex[] data, int sourceOffset, int length)
            where TIndex : struct => SetData(
            destinationOffset: destinationOffset,
            data: data,
            sourceOffset: sourceOffset,
            length: length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TIndex>(
            int destinationOffset,
            TIndex[] data,
            int sourceOffset,
            int length,
            BufferWriteMode bufferWriteMode
        ) where TIndex : struct
        {
            if (bufferWriteMode != BufferWriteMode.Overwrite)
            {
                if (PlatformBuffer is not DynamicIndexBuffer dynamicBuffer)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        "{BufferWriteMode} is only supported on dynamic buffers",
                        bufferWriteMode
                    );
                    return false;
                }

                SetDataOptions setDataOptions = bufferWriteMode switch
                {
                    // ReSharper disable once UnreachableSwitchArmDueToIntegerAnalysis
                    BufferWriteMode.Overwrite => SetDataOptions.None,
                    BufferWriteMode.Discard => SetDataOptions.Discard,
                    BufferWriteMode.NoOverwrite => SetDataOptions.None,
                    _ => throw Exceptions.UnreachableInvalidEnum(value: bufferWriteMode),
                };
                dynamicBuffer.SetData(
                    offsetInBytes: destinationOffset,
                    data: data,
                    startIndex: sourceOffset,
                    elementCount: length,
                    options: setDataOptions
                );
            }
            else
            {
                PlatformBuffer.SetData(
                    offsetInBytes: destinationOffset,
                    data: data,
                    startIndex: sourceOffset,
                    elementCount: length
                );
            }

            return true;
        }
    }
}