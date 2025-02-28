using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    private readonly ConcurrentDictionary<Microsoft.Xna.Framework.Graphics.VertexBuffer, IVertexBuffer> _allocatedVertexBuffers = [];
    private long _allocatedVertexBuffersSize;

    private bool AddAllocatedVertexBuffer(Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer, IVertexBuffer buffer)
    {
        if (!_allocatedVertexBuffers.TryAdd(platformBuffer, buffer))
        {
            return false;
        }

        MarkAllocated(buffer);

        _allocatedVertexBuffersSize += MeasureDataSize(platformBuffer);
        return true;
    }

    private bool RemoveAllocatedVertexBuffer(Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer, [NotNullWhen(true)] out IVertexBuffer? buffer)
    {
        if (!_allocatedVertexBuffers.TryRemove(platformBuffer, out buffer))
        {
            return false;
        }

        _allocatedVertexBuffersSize -= MeasureDataSize(platformBuffer);
        return true;
    }

    private static long MeasureDataSize(Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer) =>
        platformBuffer.VertexCount * platformBuffer.VertexDeclaration.VertexStride;

    private static readonly ConcurrentDictionary<Type, VertexDeclaration> CachedVertexDeclarations = [];

    private static VertexDeclaration CreateVertexDeclaration<TVertex>()
    {
        var vertexType = typeof(TVertex);
        if (CachedVertexDeclarations.TryGetValue(vertexType, out var vertexDeclaration))
        {
            return vertexDeclaration;
        }

        if (typeof(TVertex).Extends<IVertexType>())
        {
            var dummyInstance = Activator.CreateInstance<TVertex>();
            if (dummyInstance is null)
            {
                throw new InvalidOperationException(
                    $"Failed to create an instance of {vertexType.GetName(qualified: true)}"
                );
            }

            if (dummyInstance is not IVertexType monoGameVertexType)
            {
                throw new InvalidOperationException(
                    $"{vertexType.GetName(qualified: true)} supposedly extends from {typeof(IVertexType).GetName(qualified: true)} but the cast failed?"
                );
            }

            vertexDeclaration = monoGameVertexType.VertexDeclaration;
            if (!CachedVertexDeclarations.TryAdd(vertexType, vertexDeclaration))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Failed to cache vertex declaration for {VertexType}",
                    vertexType.GetName(qualified: true)
                );
            }

            return vertexDeclaration;
        }

        VertexComponentAttribute[] vertexComponentAttributes =
            vertexType.GetCustomAttributes<VertexComponentAttribute>().ToArray();
        var vertexElements = vertexComponentAttributes
            .Select(
                componentAttribute =>
                {
                    var usage = GetVertexElementUsageForComponent(componentAttribute.Component);
                    VertexElementFormat format = GetVertexElementFormatForComponentFormat(componentAttribute.Format);
                    return new VertexElement(
                        offset: componentAttribute.Offset,
                        elementFormat: format,
                        elementUsage: usage,
                        usageIndex: componentAttribute.UsageIndex
                    );
                })
            .ToArray();

        vertexDeclaration = new VertexDeclaration(vertexElements);
        if (!CachedVertexDeclarations.TryAdd(vertexType, vertexDeclaration))
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Failed to cache vertex declaration for {VertexType}",
                vertexType.GetName(qualified: true)
            );
        }

        return vertexDeclaration;
    }

    private static VertexElementFormat GetVertexElementFormatForComponentFormat(VertexComponentFormat componentAttributeFormat)
    {
        return componentAttributeFormat switch
        {
            VertexComponentFormat.Float32 => VertexElementFormat.Single,
            VertexComponentFormat.Vector2f32 => VertexElementFormat.Vector2,
            VertexComponentFormat.Vector3f32 => VertexElementFormat.Vector3,
            VertexComponentFormat.Vector4f32 => VertexElementFormat.Vector4,
            VertexComponentFormat.PackedARGBu32 => VertexElementFormat.Color,
            VertexComponentFormat.Vector4u8 => VertexElementFormat.Byte4,
            VertexComponentFormat.Vector2u16 => VertexElementFormat.Short2,
            VertexComponentFormat.Vector4u16 => VertexElementFormat.Short4,
            VertexComponentFormat.Normalized2u16 => VertexElementFormat.NormalizedShort2,
            VertexComponentFormat.Normalized4u16 => VertexElementFormat.NormalizedShort4,
            VertexComponentFormat.Vector2f16 => VertexElementFormat.HalfVector2,
            VertexComponentFormat.Vector4f16 => VertexElementFormat.HalfVector4,
            _ => throw Exceptions.UnreachableInvalidEnum(componentAttributeFormat),
        };
    }

    private static VertexElementUsage GetVertexElementUsageForComponent(VertexComponent component)
    {
        return component switch
        {
            VertexComponent.Position => VertexElementUsage.Position,
            VertexComponent.Color => VertexElementUsage.Color,
            VertexComponent.TextureUV => VertexElementUsage.TextureCoordinate,
            VertexComponent.Normal => VertexElementUsage.Normal,
            VertexComponent.Binormal => VertexElementUsage.Binormal,
            VertexComponent.Tangent => VertexElementUsage.Tangent,
            VertexComponent.BlendIndices => VertexElementUsage.BlendIndices,
            VertexComponent.BlendWeight => VertexElementUsage.BlendWeight,
            VertexComponent.Depth => VertexElementUsage.Depth,
            VertexComponent.Fog => VertexElementUsage.Fog,
            VertexComponent.PointSize => VertexElementUsage.PointSize,
            _ => throw Exceptions.UnreachableInvalidEnum(component),
        };
    }

    public override void SetActiveVertexBuffer(IVertexBuffer vertexBuffer, int bufferIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bufferIndex);

        if (vertexBuffer is not VertexBuffer { PlatformBuffer: { } platformVertexBuffer })
        {
            throw new ArgumentException(
                $"Expected a {typeof(VertexBuffer).GetName(qualified: true)} but received a {vertexBuffer.GetType().GetName(qualified: true)}",
                nameof(vertexBuffer)
            );
        }

        GraphicsDevice.SetVertexBuffer(platformVertexBuffer, bufferIndex);
    }

    private GraphicsDevice GraphicsDevice =>
        _graphicsDevice ?? throw new InvalidOperationException("Graphics device not initialized");

    public override IVertexBuffer CreateVertexBuffer<TVertex>(int count, BufferUsage usage = BufferUsage.None, bool dynamic = false) where TVertex : struct
    {
        var vertexDeclaration = CreateVertexDeclaration<TVertex>();
        var platformUsage = BufferUsageToMonoGameBufferUsage(usage);

        Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer;
        if (dynamic)
        {
            platformBuffer = new DynamicVertexBuffer(_graphicsDevice, vertexDeclaration, count, platformUsage);
        }
        else
        {
            platformBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(
                _graphicsDevice,
                vertexDeclaration,
                count,
                platformUsage
            );
        }

        VertexBuffer buffer = new(platformBuffer, typeof(TVertex));
        platformBuffer.Disposing += VertexBufferOnDisposing;
        if (AddAllocatedVertexBuffer(platformBuffer, buffer))
        {
            return buffer;
        }

        buffer.Dispose();
        throw new InvalidOperationException("Unable to track vertex buffer");
    }

    private void VertexBufferOnDisposing(object? sender, EventArgs args)
    {
        if (sender is Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer)
        {
            OnPlatformVertexBufferDisposal(platformBuffer);
            return;
        }

        ApplicationContext.CurrentContext.Logger.LogError(
            "Received disposal event but it was not from an instance of {ExpectedType} ({ActualType})",
            typeof(Microsoft.Xna.Framework.Graphics.VertexBuffer).GetName(qualified: true),
            sender?.GetType().GetName(qualified: true) ?? "null"
        );
    }

    private void OnPlatformVertexBufferDisposal(Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer)
    {
        if (RemoveAllocatedVertexBuffer(platformBuffer, out var buffer))
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

    public override void DrawBuffer(IVertexBuffer vertexBuffer, IIndexBuffer? indexBuffer = null)
    {
        ArgumentNullException.ThrowIfNull(vertexBuffer);

        if (ActiveShader is not Shader { PlatformShader: { CurrentTechnique.Passes.Count: >0 } platformShader })
        {
            throw new InvalidOperationException("Rendering buffers requires active shader with at least 1 pass");
        }

        if (vertexBuffer is not VertexBuffer
            {
                PlatformBuffer: { } platformVertexBuffer,
                PrimitiveType: var primitiveType,
                PlatformPrimitiveType: var platformPrimitiveType
            })
        {
            throw new ArgumentException(
                $"Invalid vertex buffer type {vertexBuffer.GetType().GetName(qualified: true)}",
                nameof(vertexBuffer)
            );
        }

        int primitiveCount;
        if (indexBuffer is not IndexBuffer { PlatformBuffer: { } platformIndexBuffer })
        {
            if (indexBuffer is not null)
            {
                throw new ArgumentException(
                    $"Invalid index buffer type {indexBuffer.GetType().GetName(qualified: true)}",
                    nameof(vertexBuffer)
                );
            }

            platformIndexBuffer = null;
            primitiveCount = primitiveType switch
            {
                PrimitiveType.LineList => platformVertexBuffer.VertexCount / 2,
                PrimitiveType.TriangleList => platformVertexBuffer.VertexCount / 3,
                PrimitiveType.TriangleStrip => platformVertexBuffer.VertexCount - 2,
                _ => throw Exceptions.UnreachableInvalidEnum(platformPrimitiveType),
            };
        }
        else
        {
            primitiveCount = primitiveType switch
            {
                PrimitiveType.LineList => platformIndexBuffer.IndexCount / 2,
                PrimitiveType.TriangleList => platformIndexBuffer.IndexCount / 3,
                PrimitiveType.TriangleStrip => platformIndexBuffer.IndexCount - 2,
                _ => throw Exceptions.UnreachableInvalidEnum(platformPrimitiveType),
            };
        }

        var graphicsDevice = GraphicsDevice;

        graphicsDevice.Indices = platformIndexBuffer;
        graphicsDevice.SetVertexBuffer(vertexBuffer: platformVertexBuffer, vertexOffset: 0);

        foreach (var pass in platformShader.CurrentTechnique.Passes)
        {
            pass.Apply();
            if (platformIndexBuffer is null)
            {
                graphicsDevice.DrawPrimitives(
                    primitiveType: platformPrimitiveType,
                    vertexStart: 0,
                    primitiveCount: primitiveCount
                );
            }
            else
            {
                graphicsDevice.DrawIndexedPrimitives(
                    primitiveType: platformPrimitiveType,
                    baseVertex: 0,
                    startIndex: 0,
                    primitiveCount: primitiveCount
                );
            }
        }

        graphicsDevice.SetVertexBuffer(null, 0);
        graphicsDevice.Indices = null;
    }

    private class VertexBuffer(Microsoft.Xna.Framework.Graphics.VertexBuffer platformBuffer, Type vertexType) : GPUBuffer, IVertexBuffer
    {
        private PrimitiveType _primitiveType;

        internal readonly Microsoft.Xna.Framework.Graphics.VertexBuffer PlatformBuffer = platformBuffer;

        internal Microsoft.Xna.Framework.Graphics.PrimitiveType PlatformPrimitiveType { get; private set; }

        public Type VertexType { get; } = vertexType;

        public override int Count => PlatformBuffer.VertexCount;

        public override int SizeBytes => (int)MeasureDataSize(PlatformBuffer);

        protected override void ReleaseManagedResources()
        {
            base.ReleaseManagedResources();

            if (!PlatformBuffer.IsDisposed)
            {
                PlatformBuffer.Dispose();
            }
        }

        public PrimitiveType PrimitiveType
        {
            get => _primitiveType;
            set
            {
                if (value == _primitiveType)
                {
                    return;
                }

                _primitiveType = value;
                PlatformPrimitiveType = value switch
                {
                    PrimitiveType.LineList => Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList,
                    PrimitiveType.TriangleList => Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                    PrimitiveType.TriangleStrip => Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip,
                    _ => throw Exceptions.UnreachableInvalidEnum(value),
                };
            }
        }

        public bool GetData<TVertex>(TVertex[] destination) where TVertex : struct =>
            GetData(bufferOffset: 0, destination: destination, destinationOffset: 0, length: destination.Length);

        public bool GetData<TVertex>(TVertex[] destination, int destinationOffset, int length) where TVertex : struct =>
            GetData(bufferOffset: 0, destination: destination, destinationOffset: destinationOffset, length: length);

        public bool GetData<TVertex>(int bufferOffset, TVertex[] destination, int destinationOffset, int length)
            where TVertex : struct
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
                    nameof(IVertexBuffer),
                    Id
                );
                return false;
            }
        }

        public bool SetData<TVertex>(TVertex[] data) where TVertex : struct => SetData(
            destinationOffset: 0,
            data: data,
            sourceOffset: 0,
            length: data.Length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TVertex>(TVertex[] data, int sourceOffset, int length)
            where TVertex : struct => SetData(
            destinationOffset: 0,
            data: data,
            sourceOffset: sourceOffset,
            length: length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TVertex>(int destinationOffset, TVertex[] data, int sourceOffset, int length)
            where TVertex : struct => SetData(
            destinationOffset: destinationOffset,
            data: data,
            sourceOffset: sourceOffset,
            length: length,
            bufferWriteMode: BufferWriteMode.Overwrite
        );

        public bool SetData<TVertex>(
            int destinationOffset,
            TVertex[] data,
            int sourceOffset,
            int length,
            BufferWriteMode bufferWriteMode
        ) where TVertex : struct
        {
            if (bufferWriteMode != BufferWriteMode.Overwrite)
            {
                if (PlatformBuffer is not DynamicVertexBuffer dynamicBuffer)
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
                    vertexStride: Marshal.SizeOf<TVertex>(),
                    options: setDataOptions
                );
            }
            else
            {
                PlatformBuffer.SetData(
                    offsetInBytes: destinationOffset,
                    data: data,
                    startIndex: sourceOffset,
                    elementCount: length,
                    vertexStride: Marshal.SizeOf<TVertex>()
                );
            }

            return true;
        }
    }
}