using System.Text;

namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    private static unsafe string? PointerToUTF8(byte* ptr)
    {
        if (ptr == default)
        {
            return null;
        }

        var end = ptr;
        while (*end != 0)
        {
            ++end;
        }

        var length = (int)(end - ptr);
        var str = Encoding.UTF8.GetString(ptr, length);
        return str;
    }
}