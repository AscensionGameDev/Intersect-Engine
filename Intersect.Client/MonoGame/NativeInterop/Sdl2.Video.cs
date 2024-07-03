using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop
{
    public partial class Sdl2
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SDL_GetNumVideoDisplays_d();

        public static SDL_GetNumVideoDisplays_d SDL_GetNumVideoDisplays =
            Loader.Functions.LoadFunction<SDL_GetNumVideoDisplays_d>(nameof(SDL_GetNumVideoDisplays));

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int SDL_GetDisplayBounds_d(int displayIndex, SDL_Rect * rect);
        
        private static SDL_GetDisplayBounds_d SDL_GetDisplayBounds_f =
            Loader.Functions.LoadFunction<SDL_GetDisplayBounds_d>(nameof(SDL_GetDisplayBounds));

        public static SDL_Rect SDL_GetDisplayBounds(int displayIndex)
        {
            if (TryGetDisplayBounds(displayIndex, out var bounds))
            {
                return bounds;
            }

            throw new Exception(SDL_GetError());
        }

        public static unsafe bool TryGetDisplayBounds(int displayIndex, out SDL_Rect bounds)
        {
            fixed (SDL_Rect* boundsPointer = &bounds)
            {
                return SDL_GetDisplayBounds_f(displayIndex, boundsPointer) == 0;
            }
        }
        
        public static SDL_Rect[] GetDisplayBounds()
        {
            var displayCount = SDL_GetNumVideoDisplays();
            if (displayCount < 1)
            {
                throw new Exception(SDL_GetError());
            }

            var displayBounds = new SDL_Rect[displayCount];
            for (var displayIndex = 0; displayIndex < displayCount; ++displayIndex)
            {
                if (TryGetDisplayBounds(displayIndex, out displayBounds[displayIndex]))
                {
                    continue;
                }

                throw new Exception(SDL_GetError());
            }

            return displayBounds;
        }
    }
}