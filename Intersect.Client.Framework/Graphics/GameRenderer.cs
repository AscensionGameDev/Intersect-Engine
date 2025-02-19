using System.Collections.Concurrent;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Core;
using Intersect.Framework.Collections;
using Intersect.Framework.Core;
using Intersect.Framework.SystemInformation;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Graphics;

public abstract partial class GameRenderer : IGameRenderer, ITextHelper
{
    // To test the unloading system, lower InactiveTimeCutoff to 5000 (5 seconds) and raise
    // MinimumAvailableVRAM to above your GPU RAM (if using Nvidia) or system RAM (if not using Nvidia)

    /// <summary>
    /// 30 seconds in milliseconds
    /// </summary>
    private const long InactiveTimeCutoff = 30_000;

    /// <summary>
    ///     128MiB, which is 2x 4096px² atlases, half an 8192px² atlas
    /// </summary>
    private const long MinimumAvailableVRAM = 134217728;

    /// <summary>
    /// 30 minutes in milliseconds
    /// </summary>
    private const long StaleTimeCutoff = 30 * 60 * 1000;

    /// <summary>
    /// 1GiB, which is 16x 4096px² atlases, 4x 8192px² atlases, and 1x 16384px² atlas
    /// </summary>
    private const long StaleVRAMCutoff = MinimumAvailableVRAM * 8;

    /// <summary>
    /// 6 hours in milliseconds
    /// </summary>
    private const long UnusedTimeCutoff = 6 * 60 * 60 * 1000;

    /// <summary>
    /// 8GiB, which is 128x 4096px² atlases, 32x 8192px² atlases, and 8x 16384px² atlases
    /// </summary>
    private const long UnusedVRAMCutoff = StaleVRAMCutoff * 8;

    private readonly ConcurrentDictionary<string, ScreenshotRequest> _screenshotRequestLookup = [];
    private readonly ConcurrentConditionalDequeue<ScreenshotRequest> _screenshotRequests = [];

    private readonly HashSet<IGameTexture> _textures = [];
    private readonly List<IGameTexture> _texturesSortedByAccessTime = [];

    protected float _scale = 1.0f;

    private IGameTexture? _whitePixel;

    public IGameTexture[] Textures
    {
        get
        {
            try
            {
                var buffer = new IGameTexture[_textures.Count];
                _textures.CopyTo(buffer);
                return buffer;
            }
            catch
            {
                return _textures.ToArray();
            }
        }
    }

    public ulong TextureCount => (ulong)Math.Max(0, _textures.Count);

    public ulong RenderTargetAllocations { get; private set; }

    public ulong TextureAllocations { get; private set; }

    public bool HasOverrideResolution => OverrideResolution != Resolution.Empty;

    public bool HasScreenshotRequests => _screenshotRequests.Count > 0;

    public float Scale
    {
        get => _scale;
        set
        {
            if (Math.Abs(_scale - value) < 0.001)
            {
                return;
            }

            _scale = value;
            RecreateSpriteBatch();
        }
    }

    public Resolution ActiveResolution => new(PreferredResolution, OverrideResolution);

    public Resolution OverrideResolution { get; set; }

    public Resolution PreferredResolution { get; set; }

    /// <summary>
    ///     Clears everything off the render target with a specified color.
    /// </summary>
    public abstract void Clear(Color color);

    public FloatRect CurrentView
    {
        get => GetView();
        set => SetView(value);
    }

    public int Fps => GetFps();

    public int ScreenWidth => GetScreenWidth();

    public int ScreenHeight => GetScreenHeight();

    public string ResolutionAsString => GetResolutionString();

    public event EventHandler<TextureEventArgs>? TextureAllocated;

    public event EventHandler<TextureEventArgs>? TextureCreated;

    public event EventHandler<TextureEventArgs>? TextureDisposed;

    public event EventHandler<TextureEventArgs>? TextureFreed;

    void IGameRenderer.DrawTexture(
        IGameTexture tex,
        float sx,
        float sy,
        float sw,
        float sh,
        float tx,
        float ty,
        float tw,
        float th,
        Color renderColor,
        IGameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f
    )
    {
        DrawTexture(
            tex,
            sx,
            sy,
            sw,
            sh,
            tx,
            ty,
            tw,
            th,
            renderColor,
            renderTarget,
            blendMode,
            shader,
            rotationDegrees
        );
    }

    public IGameRenderTexture CreateWhiteTexture()
    {
        return WhitePixel as IGameRenderTexture;
    }

    public abstract IGameRenderTexture CreateRenderTexture(int width, int height);

    public Pointf MeasureText(string text, GameFont? font) => MeasureText(text, font, 1);

    public abstract Pointf MeasureText(string text, GameFont? gameFont, float fontScale);

    public abstract void DrawString(
        string text,
        GameFont? gameFont,
        float x,
        float y,
        float fontScale,
        Color? fontColor,
        bool worldPos = true,
        IGameRenderTexture? renderTexture = null,
        Color? borderColor = null
    );

    public abstract void DrawString(
        string text,
        GameFont gameFont,
        float x,
        float y,
        float fontScale,
        Color fontColor,
        bool worldPos,
        IGameRenderTexture renderTexture,
        FloatRect clipRect,
        Color borderColor = null
    );

    public List<string> ValidVideoModes => GetValidVideoModes();

    public void RequestScreenshot(string? pathToScreenshots = default)
    {
        if (string.IsNullOrWhiteSpace(pathToScreenshots))
        {
            var pathToPictures = Environment.GetFolderPath(
                Environment.SpecialFolder.MyPictures,
                Environment.SpecialFolderOption.Create
            );
            var pathToApplicationPictures = Path.Combine(pathToPictures, ApplicationContext.CurrentContext.Name);
            pathToScreenshots = Path.Combine(pathToApplicationPictures, "screenshots");
        }

        if (!Directory.Exists(pathToScreenshots))
        {
            Directory.CreateDirectory(pathToScreenshots);
        }

        var screenshotNumber = 1;
        var screenshotFileName = $"{DateTime.Now:yyyyMMdd-HHmmssfff}.png";
        var pathToScreenshotFile = Path.Combine(pathToScreenshots, screenshotFileName);
        while (ScreenshotOrRequestExistsFor(pathToScreenshotFile) && screenshotNumber < 100)
        {
            screenshotFileName = $"{DateTime.Now:yyyyMMdd-HHmmssfff}.{screenshotNumber++:000}.png";
            pathToScreenshotFile = Path.Combine(pathToScreenshots ?? string.Empty, screenshotFileName);
        }

        if (ScreenshotOrRequestExistsFor(pathToScreenshotFile))
        {
            ApplicationContext.CurrentContext.Logger.LogWarning("Failed to request screenshot");
            return;
        }

        ScreenshotRequest screenshotRequest = new(
            () => File.OpenWrite(pathToScreenshotFile),
            screenshotFileName
        );

        if (_screenshotRequestLookup.TryAdd(screenshotFileName, screenshotRequest))
        {
            _screenshotRequests.Enqueue(screenshotRequest);
        }
    }

    protected internal void MarkConstructed(IGameTexture texture, bool markAllocated = false)
    {
        _textures.Add(texture);

        TextureCreated?.Invoke(this, new TextureEventArgs(texture));

        if (markAllocated)
        {
            MarkAllocated(texture);
        }
    }

    protected internal virtual void MarkDisposed(IGameTexture texture)
    {
        _textures.Remove(texture);
        TextureDisposed?.Invoke(this, new TextureEventArgs(texture));
    }

    protected internal void MarkAllocated(IGameTexture texture)
    {
        if (!_textures.Contains(texture))
        {
            throw new InvalidOperationException("Texture must be added first");
        }

        if (!texture.IsPinned)
        {
            _texturesSortedByAccessTime.AddSorted(texture);
        }

        if (texture is IGameRenderTexture)
        {
            ++RenderTargetAllocations;
        }

        ++TextureAllocations;

        TextureAllocated?.Invoke(this, new TextureEventArgs(texture));
    }

    protected internal void MarkFreed(IGameTexture texture)
    {
        if (!texture.IsPinned)
        {
            _texturesSortedByAccessTime.Remove(texture);
        }

        if (texture is IGameRenderTexture)
        {
            if (RenderTargetAllocations > 0)
            {
                --RenderTargetAllocations;
            }
            else
            {
                ApplicationContext.CurrentContext.Logger.LogCritical("RenderTextureAllocations out of sync");
            }
        }

        if (TextureAllocations > 0)
        {
            --TextureAllocations;
        }
        else
        {
            ApplicationContext.CurrentContext.Logger.LogCritical("TextureAllocations out of sync");
        }

        TextureFreed?.Invoke(this, new TextureEventArgs(texture));
    }

    internal void UpdateAccessTime(IGameTexture gameTexture)
    {
        _texturesSortedByAccessTime.Resort(gameTexture);
    }

    public abstract void Init();

    /// <summary>
    ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
    /// </summary>
    /// <returns></returns>
    public abstract bool Begin();

    public abstract bool BeginScreenshot();

    protected abstract bool RecreateSpriteBatch();

    public abstract long UsedMemory { get; }

    /// <summary>
    ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
    /// </summary>
    public void End()
    {
        DoEnd();

        var usedVRAM = UsedMemory;
        var availableVRAM = PlatformStatistics.AvailableGPUMemory;
        var totalVRAM = PlatformStatistics.TotalGPUMemory;

        if (availableVRAM < 0)
        {
            if (totalVRAM < 0)
            {
                availableVRAM = PlatformStatistics.AvailableSystemMemory;
            }
            else
            {
                availableVRAM = totalVRAM - usedVRAM;
            }
        }

        if (totalVRAM < 0)
        {
            totalVRAM = availableVRAM + usedVRAM;
        }

        var now = Timing.Global.MillisecondsUtc;

        var unusedCutoffTime = now - UnusedTimeCutoff;
        var staleCutoffTime = now - StaleTimeCutoff;
        var inactiveCutoffTime = now - InactiveTimeCutoff;

        var unusedCutoffSize = UnusedVRAMCutoff < totalVRAM ? UnusedVRAMCutoff : totalVRAM - MinimumAvailableVRAM;
        var staleCutoffSize = StaleVRAMCutoff < totalVRAM ? StaleVRAMCutoff : totalVRAM - MinimumAvailableVRAM;

        while (_texturesSortedByAccessTime.FirstOrDefault() is { } texture)
        {
            var accessTime = texture.AccessTime;
            if (availableVRAM > MinimumAvailableVRAM)
            {
                if (availableVRAM > staleCutoffSize)
                {
                    if (availableVRAM > unusedCutoffSize)
                    {
                        // Don't unload anything if we've still got (at the time of writing) 8GiB of VRAM
                        break;
                    }

                    if (texture.AccessTime > unusedCutoffTime)
                    {
                        // It has been used in the last 6 hours, don't unload
                        break;
                    }

                    // We're below our unused VRAM threshold and the texture is considered unused, unload it
                    ApplicationContext.CurrentContext.Logger.LogTrace(
                        "Unloading {TextureName} because it's unused and we have {AvailableVRAM} bytes of VRAM available (cutoff is {CutoffVRAM})",
                        texture.Name,
                        availableVRAM,
                        unusedCutoffSize
                    );
                }
                else if (texture.AccessTime > staleCutoffTime)
                {
                    // We are below our stale VRAM threshold, but the texture is not yet considered stale, don't unload
                }
                else
                {
                    // We're below our stale VRAM threshold and the texture is considered stale, unload it
                    ApplicationContext.CurrentContext.Logger.LogTrace(
                        "Unloading {TextureName} because it's stale and we have {AvailableVRAM} bytes of VRAM available (cutoff is {CutoffVRAM})",
                        texture.Name,
                        availableVRAM,
                        staleCutoffSize
                    );
                }
            }
            else if (accessTime > inactiveCutoffTime)
            {
                // We are below our minimum VRAM threshold, but the texture is still active, don't unload
                break;
            }
            else
            {
                ApplicationContext.CurrentContext.Logger.LogTrace(
                    "Unloading {TextureName} because it's inactive and we have {AvailableVRAM} bytes of VRAM available (cutoff is {CutoffVRAM})",
                    texture.Name,
                    availableVRAM,
                    MinimumAvailableVRAM
                );
            }

            // If we've reached this point in the code one of the following is true:
            // - We are below the unused memory threshold, and the texture is considered to be unused
            // - We are below the stale memory threshold, and the texture is considered to be stale
            // - We are below the minimum memory threshold, and the texture is considered to be inactive
            // In any of these three cases we want to unload the texture

            if (texture.Unload())
            {
                _texturesSortedByAccessTime.Remove(texture);
            }
        }
    }

    protected abstract void DoEnd();

    public abstract void EndScreenshot();

    public abstract void SetView(FloatRect view);

    public abstract FloatRect GetView();

    public abstract GameFont LoadFont(string filename);

    public abstract void DrawTexture(
        IGameTexture tex,
        float sx,
        float sy,
        float sw,
        float sh,
        float tx,
        float ty,
        float tw,
        float th,
        Color renderColor,
        IGameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool isUi = false,
        bool drawImmediate = false
    );

    public abstract int GetFps();

    public abstract int GetScreenWidth();

    public abstract int GetScreenHeight();

    public abstract string GetResolutionString();

    public abstract bool DisplayModeChanged();

    public IGameTexture LoadTexture(string assetName, string filePath)
    {
        return AtlasReference.TryGet(assetName, out var atlasReference)
            ? CreateGameTextureFromAtlasReference(assetName, atlasReference)
            : CreateTextureFromStreamFactory(
                assetName,
                () =>
                {
                    try
                    {
                        return File.OpenRead(filePath);
                    }
                    catch
                    {
                        assetName.ToLowerInvariant();
                        throw;
                    }
                }
            );
    }

    protected abstract IGameTexture CreateGameTextureFromAtlasReference(string assetName, AtlasReference atlasReference);

    public abstract IGameTexture CreateTextureFromStreamFactory(string assetName, Func<Stream> streamFactory);

    public IGameTexture WhitePixel => _whitePixel ??= CreateWhitePixel();

    protected abstract IGameTexture CreateWhitePixel();

    //Buffers
    public abstract GameTileBuffer CreateTileBuffer();

    public abstract void DrawTileBuffer(GameTileBuffer buffer);

    public abstract void Close();
    public abstract List<string> GetValidVideoModes();

    public abstract GameShader LoadShader(string shaderName);

    private bool ScreenshotOrRequestExistsFor(string pathToScreenshotFile)
    {
        return File.Exists(pathToScreenshotFile) || _screenshotRequestLookup.ContainsKey(pathToScreenshotFile);
    }

    protected void ProcessScreenshots(WriteTextureToStreamDelegate writeTexture)
    {
        Func<ScreenshotRequest, bool> consumer = screenshotRequest => writeTexture(
            screenshotRequest.StreamFactory(),
            screenshotRequest.Hint
        );
        while (_screenshotRequests.TryDequeueIf(consumer)) { }
    }
}