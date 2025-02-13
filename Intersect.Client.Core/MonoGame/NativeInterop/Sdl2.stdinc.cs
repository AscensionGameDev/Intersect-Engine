using System.Runtime.InteropServices;
using System.Text;

namespace Intersect.Client.MonoGame.NativeInterop;

public partial class Sdl2
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate byte* SDL_getenv_d(byte* name);

    private static SDL_getenv_d SDL_getenv_f = Loader.Functions.LoadFunction<SDL_getenv_d>(nameof(SDL_getenv));

    public static unsafe string? SDL_getenv(string name)
    {
        fixed (byte* pName = Encoding.UTF8.GetBytes(name))
        {
            var textBytes = SDL_GetHint_f(pName);
            if (textBytes == default)
            {
                return null;
            }

            var endTextBytes = textBytes;
            while (*endTextBytes != default)
            {
                ++endTextBytes;
            }

            var hintText = Encoding.UTF8.GetString(textBytes, (int)(endTextBytes - textBytes));
            return hintText;
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate int SDL_setenv_d(byte* name, byte* value, int overwrite);

    private static SDL_setenv_d SDL_setenv_f = Loader.Functions.LoadFunction<SDL_setenv_d>(nameof(SDL_setenv));

    public static unsafe bool SDL_setenv(string name, string value, bool overwrite)
    {
        fixed (byte* pValue = Encoding.UTF8.GetBytes(value))
        {
            fixed (byte* pName = Encoding.UTF8.GetBytes(name))
            {
                return SDL_setenv_f(pName, pValue, overwrite ? 1 : 0) != 0;
            }
        }
    }
}