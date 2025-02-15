namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    public static bool IsNVX_gpu_memory_infoSupported => IsExtensionSupported("GL_NVX_gpu_memory_info");

    private static long glGetNVXGPUMemoryInfo(GLenum name)
    {
        return glGetIntegerv(name);
    }
}