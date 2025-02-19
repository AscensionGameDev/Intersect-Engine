using System.Diagnostics;
using System.Runtime.CompilerServices;
using Hardware.Info;
using Intersect.Framework.SystemInformation;

namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    private sealed class AMDStatisticsProvider : IGPUStatisticsProvider
    {
        public long? AvailableMemory
        {
            get
            {
                var info = glGetATIMemInfo(GLenum.TEXTURE_FREE_MEMORY_ATI);
                return info.FreeInPool * 1000L;
            }
        }

        public long? TotalMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => -1;
        }
    }

    private sealed class NvidiaStatisticsProvider : IGPUStatisticsProvider
    {
        public long? AvailableMemory =>
            glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX) * 1000L;

        public long? TotalMemory => glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_TOTAL_AVAILABLE_MEMORY_NVX) * 1000L;
    }

    private static bool _gpuStatisticsProviderCreated;
    private static IGPUStatisticsProvider? _gpuStatisticsProvider;

    public static IGPUStatisticsProvider? CreateGPUStatisticsProvider()
    {
        if (_gpuStatisticsProviderCreated)
        {
            return _gpuStatisticsProvider;
        }

        _gpuStatisticsProviderCreated = true;

        if (IsATI_meminfoSupported)
        {
            _gpuStatisticsProvider = new AMDStatisticsProvider();
        }
        else if (IsNVX_gpu_memory_infoSupported)
        {
            _gpuStatisticsProvider = new NvidiaStatisticsProvider();
        }

        return _gpuStatisticsProvider;
    }
}