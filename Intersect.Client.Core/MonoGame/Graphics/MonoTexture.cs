using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Compression;
using Intersect.Core;
using Intersect.IO.Files;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;

public partial class MonoTexture : GameTexture
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly string _name;

    private readonly string _path;

    private readonly string _realPath;

    private readonly Func<Stream>? _createStream;

    private readonly GameTexturePackFrame? _packFrame;

    private readonly bool _doNotFree;

    private int _width = -1;

    private Texture2D? _texture;

    private int _height = -1;

    private long _lastAccessTime;

    private bool _loadError;

    private MonoTexture(Texture2D texture2D, string assetName) : base(assetName)
    {
        _graphicsDevice = texture2D.GraphicsDevice;
        _path = assetName;
        _realPath = string.Empty;
        _name = assetName;
        _texture = texture2D;
        _doNotFree = true;
    }

    public MonoTexture(GraphicsDevice graphicsDevice, string filename, string realPath) : base(
        Path.GetFileName(filename)
    )
    {
        _graphicsDevice = graphicsDevice;
        _path = filename;
        _realPath = realPath;
        _name = Path.GetFileName(filename);
    }

    public MonoTexture(GraphicsDevice graphicsDevice, string assetName, Func<Stream> createStream) : base(assetName)
    {
        _graphicsDevice = graphicsDevice;
        _path = assetName;
        _realPath = string.Empty;
        _name = assetName;
        _createStream = createStream;
    }

    public MonoTexture(GraphicsDevice graphicsDevice, string filename, GameTexturePackFrame packFrame) : base(
        Path.GetFileName(filename)
    )
    {
        _graphicsDevice = graphicsDevice;
        _path = filename;
        _realPath = string.Empty;
        _name = Path.GetFileName(filename);
        _packFrame = packFrame;
        _width = packFrame.SourceRect.Width;
        _height = packFrame.SourceRect.Height;
    }

    public override bool IsLoaded => base.IsLoaded && _texture != null;

    private void Load(Stream stream)
    {
        _texture = Texture2D.FromStream(_graphicsDevice, stream);
        if (_texture == null)
        {
            throw new InvalidDataException("Failed to load texture, received no data.");
        }

        _width = _texture.Width;
        _height = _texture.Height;
        _loadError = false;

        EmitLoaded();
    }

    public void LoadTexture()
    {
        if (_texture != null)
        {
            return;
        }

        if (_createStream != null)
        {
            using var stream = _createStream();
            Load(stream);
            return;
        }

        if (_packFrame != null)
        {
            ((MonoTexture) _packFrame.PackTexture)?.LoadTexture();

            return;
        }

        _loadError = true;
        if (string.IsNullOrWhiteSpace(_realPath))
        {
            ApplicationContext.Context.Value?.Logger.LogError("Invalid texture path (empty/null).");

            return;
        }

        var relativePath = FileSystemHelper.RelativePath(Directory.GetCurrentDirectory(), _path);

        if (!File.Exists(_realPath))
        {
            ApplicationContext.Context.Value?.Logger.LogError($"Texture does not exist: {relativePath}");

            return;
        }

        using var fileStream = File.Open(_realPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            if (Path.GetExtension(_path) == ".asset")
            {
                using var gzip = GzipCompression.CreateDecompressedFileStream(fileStream);
                Load(gzip);
            }
            else
            {
                Load(fileStream);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                $"Failed to load texture ({FileSystemHelper.FormatSize(fileStream.Length)}): {relativePath}"
            );

            ChatboxMsg.AddMessage(
                new ChatboxMsg(
                    Strings.Errors.LoadFile.ToString(Strings.Words.LcaseSprite) + " [" + _name + "]",
                    new Color(0xBF, 0x0, 0x0), Enums.ChatMessageType.Error
                )
            );
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetAccessTime()
    {
        _lastAccessTime = Timing.Global.MillisecondsUtc + 15000;
    }

    public override int Width
    {
        get
        {
            ResetAccessTime();
            if (_width != -1)
            {
                return _width;
            }

            if (_texture == null)
            {
                LoadTexture();
            }

            if (_loadError)
            {
                _width = 0;
            }

            return _width;
        }
    }

    public override int Height
    {
        get
        {
            ResetAccessTime();
            if (_height != -1)
            {
                return _height;
            }

            if (_texture == null)
            {
                LoadTexture();
            }

            if (_loadError)
            {
                _height = 0;
            }

            return _height;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object? GetTexture()
    {
        if (_packFrame != null)
        {
            return _packFrame.PackTexture.GetTexture();
        }

        ResetAccessTime();

        if (_texture == null)
        {
            LoadTexture();
        }

        return _texture;
    }

    public override Color GetPixel(int x1, int y1)
    {
        if (_texture == null)
        {
            LoadTexture();
        }

        if (_loadError)
        {
            return Color.White;
        }

        var tex = _texture;

        var pack = GetTexturePackFrame();
        if (pack != null)
        {
            tex = (Texture2D) _packFrame.PackTexture.GetTexture();
            if (pack.Rotated)
            {
                var z = x1;
                x1 = pack.Rect.Right - y1 - pack.Rect.Height;
                y1 = pack.Rect.Top + z;
            }
            else
            {
                x1 += pack.Rect.X;
                y1 += pack.Rect.Y;
            }
        }

        var pixel = new Microsoft.Xna.Framework.Color[1];
        tex?.GetData(0, new Rectangle(x1, y1, 1, 1), pixel, 0, 1);

        return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override GameTexturePackFrame? GetTexturePackFrame() => _packFrame;

    public void Update()
    {
        if (_doNotFree)
        {
            return;
        }

        if (_texture == null)
        {
            return;
        }

        if (_lastAccessTime >= Timing.Global.MillisecondsUtc)
        {
            return;
        }

        EmitUnloaded();

        _texture.Dispose();
        _texture = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MonoTexture CreateFromTexture2D(Texture2D texture2D, string assetName)
    {
        return new MonoTexture(texture2D, assetName);
    }

    public override void Dispose()
    {
        base.Dispose();

        _texture?.Dispose();
        _texture = null;
    }
}
