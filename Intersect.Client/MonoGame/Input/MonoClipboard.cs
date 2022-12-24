using Intersect.Client.Framework.Input;
using Intersect.Client.MonoGame.NativeInterop;

namespace Intersect.Client.MonoGame.Input
{
    public partial class MonoClipboard : GameClipboard
    {
        /// <inheritdoc />
        public override void SetText(string data) => Sdl2.SDL_SetClipboardText(data);

        /// <inheritdoc />
        public override string GetText() => Sdl2.SDL_GetClipboardText();

        /// <inheritdoc />
        public override bool IsEmpty => string.IsNullOrEmpty(GetText());

        /// <inheritdoc />
        public override bool IsEnabled => Sdl2.IsClipboardSupported;
    }
}
