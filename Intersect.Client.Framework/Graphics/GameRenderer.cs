using System.Collections.Concurrent;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Core;
using Intersect.Framework.Collections;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Graphics;

public abstract partial class GameRenderer : IGameRenderer, ITextHelper
{
    private readonly List<GameTexture> _allocatedTextures = [];
    private readonly SortedList<long, GameTexture> _textureExpiration = [];
    private readonly ConcurrentConditionalDequeue<ScreenshotRequest> _screenshotRequests = [];
    private readonly ConcurrentDictionary<string, ScreenshotRequest> _screenshotRequestLookup = [];

    public Resolution ActiveResolution => new(PreferredResolution, OverrideResolution);

    public bool HasOverrideResolution => OverrideResolution != Resolution.Empty;

    public bool HasScreenshotRequests => _screenshotRequests.Count > 0;

    public Resolution OverrideResolution { get; set; }

    public Resolution PreferredResolution { get; set; }

    protected float _scale = 1.0f;

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

    public abstract void Init();

    /// <summary>
    ///     Called before a frame is drawn, if the renderer must re-created or anything it does it here.
    /// </summary>
    /// <returns></returns>
    public abstract bool Begin();

    public abstract bool BeginScreenshot();

    protected abstract bool RecreateSpriteBatch();

    /// <summary>
    ///     Called when the frame is done being drawn, generally used to finally display the content to the screen.
    /// </summary>
    public abstract void End();

    public abstract void EndScreenshot();

    /// <summary>
    ///     Clears everything off the render target with a specified color.
    /// </summary>
    public abstract void Clear(Color color);

    public abstract void SetView(FloatRect view);

    public FloatRect CurrentView
    {
        get { return GetView(); }
        set { SetView(value); }
    }

    public abstract FloatRect GetView();

    public abstract GameFont LoadFont(string filename);

    public abstract void DrawTexture(
        GameTexture tex,
        float sx,
        float sy,
        float sw,
        float sh,
        float tx,
        float ty,
        float tw,
        float th,
        Color renderColor,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f,
        bool isUi = false,
        bool drawImmediate = false
    );

    public int Fps => GetFps();

    public abstract int GetFps();

    public int ScreenWidth => GetScreenWidth();

    public abstract int GetScreenWidth();

    public int ScreenHeight => GetScreenHeight();

    public abstract int GetScreenHeight();

    public string ResolutionAsString => GetResolutionString();

     void IGameRenderer.DrawTexture(
        GameTexture tex,
        float sx,
        float sy,
        float sw,
        float sh,
        float tx,
        float ty,
        float tw,
        float th,
        Color renderColor,
        GameRenderTexture? renderTarget = null,
        GameBlendModes blendMode = GameBlendModes.None,
        GameShader? shader = null,
        float rotationDegrees = 0.0f
    ) => DrawTexture(tex, sx, sy, sw, sh, tx, ty, tw, th, renderColor, renderTarget, blendMode, shader, rotationDegrees);

    public GameRenderTexture CreateWhiteTexture() => GetWhiteTexture() as GameRenderTexture;

    public abstract string GetResolutionString();

    public abstract bool DisplayModeChanged();

    public abstract GameRenderTexture CreateRenderTexture(int width, int height);

    public abstract GameTexture LoadTexture(string filename, string realFilename);

    public abstract GameTexture LoadTexture(
        string assetName,
        Func<Stream> createStream
    );

    public abstract GameTexture GetWhiteTexture();

    public abstract Pointf MeasureText(string text, GameFont? gameFont, float fontScale);

    public abstract void DrawString(
        string text,
        GameFont? gameFont,
        float x,
        float y,
        float fontScale,
        Color? fontColor,
        bool worldPos = true,
        GameRenderTexture? renderTexture = null,
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
        GameRenderTexture renderTexture,
        FloatRect clipRect,
        Color borderColor = null
    );

    //Buffers
    public abstract GameTileBuffer CreateTileBuffer();

    public abstract void DrawTileBuffer(GameTileBuffer buffer);

    public abstract void Close();

    public List<string> ValidVideoModes => GetValidVideoModes();
    public abstract List<string> GetValidVideoModes();

    public abstract GameShader LoadShader(string shaderName);

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
            StreamFactory: () => File.OpenWrite(pathToScreenshotFile),
            Hint: screenshotFileName
        );

        if (_screenshotRequestLookup.TryAdd(screenshotFileName, screenshotRequest))
        {
            _screenshotRequests.Enqueue(screenshotRequest);
        }
    }

    private bool ScreenshotOrRequestExistsFor(string pathToScreenshotFile) =>
        File.Exists(pathToScreenshotFile) || _screenshotRequestLookup.ContainsKey(pathToScreenshotFile);

    protected void ProcessScreenshots(WriteTextureToStreamDelegate writeTexture)
    {
        Func<ScreenshotRequest, bool> consumer = screenshotRequest => writeTexture(
            screenshotRequest.StreamFactory(),
            screenshotRequest.Hint
        );
        while (_screenshotRequests.TryDequeueIf(consumer)) { }
    }

}