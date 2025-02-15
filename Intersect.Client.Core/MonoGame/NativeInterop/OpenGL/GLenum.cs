namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public enum GLenum
{
    #region Utility

    GL_VENDOR = 0x1F00,
    GL_RENDERER = 0x1F01,
    GL_VERSION = 0x1F02,
    GL_EXTENSIONS = 0x1F03,

    #endregion Utility

    #region Errors

    GL_NO_ERROR = 0,
    GL_INVALID_ENUM = 0x0500,
    GL_INVALID_VALUE = 0x0501,
    GL_INVALID_OPERATION = 0x0502,
    GL_STACK_OVERFLOW = 0x0503,
    GL_STACK_UNDERFLOW = 0x0504,
    GL_OUT_OF_MEMORY = 0x0505,

    #endregion Errors

    GL_NUM_EXTENSIONS = 0x821D,

    #region ATI_meminfo

    VBO_FREE_MEMORY_ATI = 0x87FB,
    TEXTURE_FREE_MEMORY_ATI = 0x87FC,
    RENDERBUFFER_FREE_MEMORY_ATI = 0x87FD,

    #endregion ATI_meminfo

    #region NVX_gpu_memory_info

    GPU_MEMORY_INFO_DEDICATED_VIDMEM_NVX = 0x9047,
    GPU_MEMORY_INFO_TOTAL_AVAILABLE_MEMORY_NVX = 0x9048,
    GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX = 0x9049,
    GPU_MEMORY_INFO_EVICTION_COUNT_NVX = 0x904A,
    GPU_MEMORY_INFO_EVICTED_MEMORY_NVX = 0x904B,

    #endregion NVX_gpu_memory_info
}