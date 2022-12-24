using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Intersect.Client.MonoGame.NativeInterop
{
    public static class Sdl2
    {
        private static readonly FunctionLoader FunctionLoader = new FunctionLoader(KnownLibrary.SDL2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void SDL_free_d(void* ptr);

        private static SDL_free_d SDL_free = FunctionLoader.LoadFunction<SDL_free_d>(nameof(SDL_free));
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate byte* SDL_GetClipboardText_d();

        private static SDL_GetClipboardText_d SDL_GetClipboardText_f =
            FunctionLoader.LoadFunction<SDL_GetClipboardText_d>(nameof(SDL_GetClipboardText));

        public static unsafe string SDL_GetClipboardText()
        {
            if (SDL_GetClipboardText_f == default || SDL_free == default)
            {
                throw new PlatformNotSupportedException();
            }
            
            var textBytes = SDL_GetClipboardText_f();
            var endTextBytes = textBytes;
            while (*endTextBytes != default)
            {
                ++endTextBytes;
            }

            var text = Encoding.UTF8.GetString(textBytes, (int)(endTextBytes - textBytes));
            SDL_free(textBytes);
            return text;
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int SDL_SetClipboardText_d(byte* text);

        private static SDL_SetClipboardText_d SDL_SetClipboardText_f =
            FunctionLoader.LoadFunction<SDL_SetClipboardText_d>(nameof(SDL_SetClipboardText));

        public static unsafe bool SDL_SetClipboardText(string text)
        {
            if (SDL_SetClipboardText_f == default)
            {
                throw new PlatformNotSupportedException();
            }
            
            fixed (byte* textBytes = Encoding.UTF8.GetBytes(text))
            {
                return SDL_SetClipboardText_f(textBytes) == 0;
            }
        }

        public static bool IsClipboardSupported => SDL_GetClipboardText_f != default &&
                                                   SDL_SetClipboardText_f != default && SDL_free != default;
    }
}
