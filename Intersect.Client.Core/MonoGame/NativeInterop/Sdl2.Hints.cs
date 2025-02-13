using System.Runtime.InteropServices;
using System.Text;

namespace Intersect.Client.MonoGame.NativeInterop;

public partial class Sdl2
{
    public const string SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR = "SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate byte* SDL_GetHint_d(byte* name);

    private static SDL_GetHint_d SDL_GetHint_f = Loader.Functions.LoadFunction<SDL_GetHint_d>(nameof(SDL_GetHint));

    public static unsafe string? SDL_GetHint(string name)
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
    private unsafe delegate int SDL_SetHint_d(byte* name, byte* value);

    private static SDL_SetHint_d SDL_SetHint_f = Loader.Functions.LoadFunction<SDL_SetHint_d>(nameof(SDL_SetHint));

    public static unsafe bool SDL_SetHint(string name, string value)
    {
        fixed (byte* pValue = Encoding.UTF8.GetBytes(value))
        {
            fixed (byte* pName = Encoding.UTF8.GetBytes(name))
            {
                return SDL_SetHint_f(pName, pValue) != 0;
            }
        }
    }

    public static bool SDL_SetHint(string name, bool value) => SDL_SetHint(name, value ? "1" : "0");
}