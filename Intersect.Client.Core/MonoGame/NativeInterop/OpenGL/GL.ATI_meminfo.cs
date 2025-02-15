using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class GL
{
    private static bool IsATI_meminfoSupported => IsExtensionSupported("GL_ATI_meminfo");

    [StructLayout(LayoutKind.Sequential)]
    private struct ATI_meminfo_Tuple
    {
        public int FreeInPool;
        public int LargestFreeBlockInPool;
        public int FreeInAuxiliaryPool;
        public int LargestFreeBlockInAuxiliaryPool;
    }

    private static unsafe ATI_meminfo_Tuple glGetATIMemInfo(GLenum name)
    {
        ATI_meminfo_Tuple data = default;
        int* p_data = &data.FreeInPool;
        glGetIntegerv_f(name, p_data);
        return data;
    }
}

/*
        VBO_FREE_MEMORY_ATI                     0x87FB
   TEXTURE_FREE_MEMORY_ATI                 0x87FC
   RENDERBUFFER_FREE_MEMORY_ATI            0x87FD*/