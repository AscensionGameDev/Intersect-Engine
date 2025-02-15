namespace Intersect.Client.MonoGame.NativeInterop.OpenGL;

public static partial class GL
{
    private static HashSet<string>? _cachedExtensions;

    public static HashSet<string> glGetExtensions()
    {
        // ReSharper disable once InvertIf
        if (_cachedExtensions is not { Count: > 0 })
        {
            var numExtensions = glGetIntegerv(GLenum.GL_NUM_EXTENSIONS);
            var extensions = glGetStrings(GLenum.GL_EXTENSIONS, numExtensions);
            _cachedExtensions = extensions.OfType<string>().ToHashSet();
        }

        return _cachedExtensions;
    }

    public static bool IsExtensionSupported(string extensionName) => glGetExtensions().Contains(extensionName);
}