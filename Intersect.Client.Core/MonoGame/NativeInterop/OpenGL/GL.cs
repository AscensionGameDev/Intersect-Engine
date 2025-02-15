using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    private static TDelegate LoadFunction<TDelegate>(string function)
    {
        return LoadFunction<TDelegate>(function, false)!;
    }

    private static T? LoadFunction<T>(string function, bool throwIfNotFound)
    {
        var ret = Sdl2.SDL_GL_GetProcAddress(function);

        if (ret != IntPtr.Zero)
        {
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
        }

        if (throwIfNotFound)
        {
            throw new EntryPointNotFoundException(function);
        }

        return default;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate GLenum glGetError_d();

    private static readonly glGetError_d glGetError_f = LoadFunction<glGetError_d>(nameof(glGetError));

    public static GLenum glGetError()
    {
        return glGetError_f();
    }
}