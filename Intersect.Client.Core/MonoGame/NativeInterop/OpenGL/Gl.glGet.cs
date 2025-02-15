using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class GL
{
    private static readonly glGetIntegerv_d glGetIntegerv_f =
        LoadFunction<glGetIntegerv_d>(nameof(glGetIntegerv));

    private static readonly glGetString_d glGetString_f =
        LoadFunction<glGetString_d>(nameof(glGetString));

    private static readonly glGetStringi_d glGetStringi_f =
        LoadFunction<glGetStringi_d>(nameof(glGetStringi));

    public static unsafe int glGetIntegerv(GLenum property)
    {
        int value;
        glGetIntegerv_f(property, &value);
        return value;
    }

    public static unsafe int[] glGetIntegerv(GLenum property, uint count)
    {
        var values = new int[count];
        fixed (int* p_values = values)
        {
            glGetIntegerv_f(property, p_values);
        }
        return values;
    }

    public static unsafe string? glGetString(GLenum name)
    {
        var ptr = glGetString_f(name);
        if (ptr != default)
        {
            return PointerToUTF8(ptr);
        }

        var error = glGetError();
        if (error == GLenum.GL_INVALID_ENUM)
        {
            throw new ArgumentException($"Invalid glGetString() name '{name}'", nameof(name));
        }

        throw new InvalidOperationException($"Unexpected error {error} when executing glGetString({name})");
    }

    public static unsafe string? glGetStringi(GLenum name, uint index)
    {
        var ptr = glGetStringi_f(name, index);
        if (ptr != default)
        {
            return PointerToUTF8(ptr);
        }

        var error = glGetError();
        if (error == GLenum.GL_INVALID_ENUM)
        {
            throw new ArgumentException($"Invalid glGetString() name '{name}'", nameof(name));
        }

        if (error == GLenum.GL_INVALID_VALUE)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                $"Index out of range for glGetString enum {name}"
            );
        }

        throw new InvalidOperationException($"Unexpected error {error} when executing glGetStringi({name}, {index})");
    }

    public static string?[] glGetStrings(GLenum name, int count)
    {
        var buffer = new string?[count];
        for (uint index = 0; index < count; ++index)
        {
            buffer[index] = glGetStringi(name, index);
        }

        return buffer;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void glGetIntegerv_d(GLenum property, int* value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate byte* glGetString_d(GLenum name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate byte* glGetStringi_d(GLenum name, uint index);
}