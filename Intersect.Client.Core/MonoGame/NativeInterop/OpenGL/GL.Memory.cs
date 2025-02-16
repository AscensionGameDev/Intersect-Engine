namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    /// <summary>
    /// Available amount of memory for render buffers in bytes
    /// </summary>
    public static long AvailableRenderBufferMemory
    {
        get
        {
            if (IsATI_meminfoSupported)
            {
                var info = glGetATIMemInfo(GLenum.RENDERBUFFER_FREE_MEMORY_ATI);
                return info.FreeInPool * 1000L;
            }

            if (IsNVX_gpu_memory_infoSupported)
            {
                return glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX) * 1000L;
            }

            return -1;
        }
    }

    /// <summary>
    /// Available amount of memory for textures in bytes
    /// </summary>
    public static long AvailableTextureMemory
    {
        get
        {
            if (IsATI_meminfoSupported)
            {
                var info = glGetATIMemInfo(GLenum.TEXTURE_FREE_MEMORY_ATI);
                return info.FreeInPool * 1000L;
            }

            if (IsNVX_gpu_memory_infoSupported)
            {
                return glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX) * 1000L;
            }

            return -1;
        }
    }

    /// <summary>
    /// Available amount of memory for VBOs in bytes
    /// </summary>
    public static long AvailableVBOMemory
    {
        get
        {
            if (IsATI_meminfoSupported)
            {
                var info = glGetATIMemInfo(GLenum.VBO_FREE_MEMORY_ATI);
                return info.FreeInPool * 1000L;
            }

            if (IsNVX_gpu_memory_infoSupported)
            {
                return glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX) * 1000L;
            }

            return -1;
        }
    }

    public static long TotalMemory
    {
        get
        {
            if (IsNVX_gpu_memory_infoSupported)
            {
                return glGetNVXGPUMemoryInfo(GLenum.GPU_MEMORY_INFO_TOTAL_AVAILABLE_MEMORY_NVX) * 1000L;
            }

            return -1;
        }
    }
}