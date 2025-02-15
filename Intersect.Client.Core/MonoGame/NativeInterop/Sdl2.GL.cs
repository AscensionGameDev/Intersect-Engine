using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop;

public partial class Sdl2
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SDL_GL_GetProcAddress_d(string proc);
    private static SDL_GL_GetProcAddress_d SDL_GL_GetProcAddress_f = Loader.Functions.LoadFunction<SDL_GL_GetProcAddress_d>(nameof(SDL_GL_GetProcAddress));
    public static nint SDL_GL_GetProcAddress(string proc) => SDL_GL_GetProcAddress_f(proc);
}