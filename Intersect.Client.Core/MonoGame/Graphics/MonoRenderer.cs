using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Intersect.Client.Classes.MonoGame.Graphics;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.NativeInterop;
using Intersect.Client.ThirdParty;
using Intersect.Core;
using Intersect.Extensions;
using Intersect.Framework.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MathHelper = Intersect.Utilities.MathHelper;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer : GameRenderer
{
    private readonly ContentManager _contentManager;

    private readonly Game _game;

    private readonly GameWindow _gameWindow;

    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly BlendState _cutoutState;

    private readonly BlendState _multiplyState;

    private readonly BlendState _normalState;

    private readonly RasterizerState _rasterizerState = new()
    {
        ScissorTestEnable = true,
    };

    public override GameShader BasicShader => _basicShader;

    protected override void OnSetActiveShader(GameShader? shader)
    {
    }

    protected override void OnSetActiveIndexBuffer(IIndexBuffer? indexBuffer)
    {
        var graphicsDevice = GraphicsDevice;

        if (indexBuffer is null)
        {
            graphicsDevice.Indices = null;
            return;
        }

        if (indexBuffer is not IndexBuffer { PlatformBuffer: { } platformBuffer })
        {
            throw new ArgumentException(
                $"Expected a {typeof(IndexBuffer).GetName(qualified: true)} but received a {indexBuffer.GetType().GetName(qualified: true)}",
                nameof(indexBuffer)
            );
        }

        graphicsDevice.Indices = platformBuffer;
    }

    public override long UsedMemory => _allocatedTexturesSize + _allocatedIndexBuffersSize + _allocatedVertexBuffersSize;

    private GraphicsDevice? _graphicsDevice;

    private List<string>? _validVideoModes;

    private BasicEffect? _basicEffect;

    private GameBlendModes _currentBlendmode = GameBlendModes.None;

    private GameShader? _currentShader;

    private FloatRect _currentSpriteView;

    private IGameRenderTexture? _currentRenderTarget;

    private FloatRect _currentView;

    private int _displayHeight;

    private int _displayWidth;

    private bool _displayModeChanged;

    private int _fps;

    private int _frames;

    private long _nextFpsTime;

    private long _fullscreenChangedTimer = -1;

    private bool _initialized;

    private bool _initializing;

    private DisplayMode? _previousDisplayMode;

    private int _screenHeight;

    private int _screenWidth;

    private RenderTarget2D? _screenshotRenderTarget;

    private SpriteBatch? _spriteBatch;

    private bool _spriteBatchBegan;

    public MonoRenderer(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, Game monoGame)
    {
        _game = monoGame;
        _graphicsDeviceManager = graphicsDeviceManager;
        _contentManager = contentManager;

        _normalState = new BlendState
        {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Add,
            AlphaBlendFunction = BlendFunction.Add,
        };

        _multiplyState = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
        };

        _cutoutState = new BlendState
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.Zero,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.InverseSourceAlpha,
        };

        _gameWindow = monoGame.Window;
    }

    public void UpdateGraphicsState(int width, int height, bool initial = false)
    {
        var currentDisplayMode = _graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode;

        if (Globals.Database.FullScreen)
        {
            var supported = false;
            foreach (var mode in _graphicsDeviceManager.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if (mode.Width == width && mode.Height == height)
                {
                    supported = true;
                }
            }

            if (!supported)
            {
                Globals.Database.FullScreen = false;
                Globals.Database.SavePreferences();
                Interface.Interface.ShowAlert(
                    Strings.Errors.DisplayNotSupportedError.ToString(
                        Strings.Internals.ResolutionXByY.ToString(width, height)
                    ),
                    Strings.Errors.DisplayNotSupported
                );
            }
        }

        var isFullscreen = Globals.Database.FullScreen;
        var updateFullscreen = initial || _graphicsDeviceManager.IsFullScreen != isFullscreen && !isFullscreen;

        if (updateFullscreen)
        {
            _graphicsDeviceManager.IsFullScreen = isFullscreen;
            _graphicsDeviceManager.HardwareModeSwitch = !isFullscreen;
        }

        var displayBounds = Sdl2.GetDisplayBounds();
        var currentDisplayBounds = displayBounds[0];
        var currentDisplayWidth = currentDisplayBounds.w;
        var currentDisplayHeight = currentDisplayBounds.h;
        if (isFullscreen)
        {
            width = currentDisplayWidth;
            height = currentDisplayHeight;
        }

        _screenWidth = width;
        _screenHeight = height;
        _graphicsDeviceManager.PreferredBackBufferWidth = width;
        _graphicsDeviceManager.PreferredBackBufferHeight = height;
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = Globals.Database.TargetFps == 0;

        if (Globals.Database.TargetFps == 1)
        {
            _game.TargetElapsedTime = new TimeSpan(333333);
        }
        else if (Globals.Database.TargetFps == 2)
        {
            _game.TargetElapsedTime = new TimeSpan(333333 / 2);
        }
        else if (Globals.Database.TargetFps == 3)
        {
            _game.TargetElapsedTime = new TimeSpan(333333 / 3);
        }
        else if (Globals.Database.TargetFps == 4)
        {
            _game.TargetElapsedTime = new TimeSpan(333333 / 4);
        }

        _game.IsFixedTimeStep = Globals.Database.TargetFps > 0;

        _graphicsDeviceManager.ApplyChanges();

        _displayWidth = _graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        _displayHeight = _graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        if (updateFullscreen)
        {
            _gameWindow.Position = new Microsoft.Xna.Framework.Point(
                currentDisplayBounds.x + ((currentDisplayWidth - _screenWidth) / 2),
                currentDisplayBounds.y + ((currentDisplayHeight - _screenHeight) / 2)
            );
        }

        _previousDisplayMode = currentDisplayMode;
        if (updateFullscreen)
        {
            _fullscreenChangedTimer = Timing.Global.MillisecondsUtc + 1000;
        }

        if (updateFullscreen)
        {
            _displayModeChanged = true;
        }
    }

    public override bool Begin()
    {
        if (_fullscreenChangedTimer > -1 && _fullscreenChangedTimer < Timing.Global.MillisecondsUtc)
        {
            _graphicsDeviceManager.PreferredBackBufferWidth--;
            _graphicsDeviceManager.ApplyChanges();
            _graphicsDeviceManager.PreferredBackBufferWidth++;
            _graphicsDeviceManager.ApplyChanges();
            _fullscreenChangedTimer = -1;
        }

        if (_gameWindow.ClientBounds.Width != 0 &&
            _gameWindow.ClientBounds.Height != 0 &&
            (_gameWindow.ClientBounds.Width != _screenWidth || _gameWindow.ClientBounds.Height != _screenHeight) &&
            !_graphicsDeviceManager.IsFullScreen)
        {
            if (_previousDisplayMode != _graphicsDeviceManager.GraphicsDevice.DisplayMode)
            {
                _displayModeChanged = true;
            }

            UpdateGraphicsState(_screenWidth, _screenHeight);
        }

        return RecreateSpriteBatch();
    }

    protected override bool RecreateSpriteBatch()
    {
        if (_spriteBatchBegan)
        {
            EndSpriteBatch();
        }

        StartSpritebatch(
            _currentView,
            GameBlendModes.None,
            null,
            null,
            true
        );
        return true;
    }

    public Vector2 GetMouseOffset()
    {
        return new Vector2(
            _graphicsDeviceManager.PreferredBackBufferWidth / (float)_gameWindow.ClientBounds.Width,
            _graphicsDeviceManager.PreferredBackBufferHeight / (float)_gameWindow.ClientBounds.Height
        );
    }

    private void StartSpritebatch(
        FloatRect view,
        GameBlendModes mode = GameBlendModes.None,
        GameShader? shader = null,
        IGameRenderTexture? target = null,
        bool forced = false,
        RasterizerState? rs = null,
        bool drawImmediate = false
    )
    {
        var viewsDiff = view.X != _currentSpriteView.X ||
                        view.Y != _currentSpriteView.Y ||
                        view.Width != _currentSpriteView.Width ||
                        view.Height != _currentSpriteView.Height;

        if (mode != _currentBlendmode ||
            shader != _currentShader ||
            (shader != null && shader.ValuesChanged()) ||
            target != _currentRenderTarget ||
            viewsDiff ||
            forced ||
            drawImmediate ||
            !_spriteBatchBegan)
        {
            if (_spriteBatchBegan)
            {
                _spriteBatch.End();
            }

            if (target != null)
            {
                _graphicsDevice?.SetRenderTarget((RenderTarget2D)target.GetTexture());
            }
            else
            {
                _graphicsDevice?.SetRenderTarget(_screenshotRenderTarget);
            }

            var blend = _normalState;
            Effect useEffect = null;

            switch (mode)
            {
                case GameBlendModes.None:
                    blend = _normalState;

                    break;

                case GameBlendModes.Alpha:
                    blend = BlendState.AlphaBlend;

                    break;

                case GameBlendModes.Multiply:
                    blend = _multiplyState;

                    break;

                case GameBlendModes.Add:
                    blend = BlendState.Additive;

                    break;

                case GameBlendModes.Opaque:
                    blend = BlendState.Opaque;

                    break;

                case GameBlendModes.Cutout:
                    blend = _cutoutState;

                    break;
            }

            if (shader != null)
            {
                useEffect = (Effect)shader.GetShader();
                shader.ResetChanged();
            }

            _spriteBatch.Begin(
                drawImmediate ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
                blend,
                SamplerState.PointClamp,
                null,
                rs,
                useEffect,
                CreateViewMatrix(view)
            );

            _currentSpriteView = view;
            _currentBlendmode = mode;
            _currentShader = shader;
            _currentRenderTarget = target;
            _spriteBatchBegan = true;
        }
    }

    public override bool DisplayModeChanged()
    {
        var changed = _displayModeChanged;
        _displayModeChanged = false;

        return changed;
    }

    public void EndSpriteBatch()
    {
        if (_spriteBatchBegan)
        {
            _spriteBatch.End();
        }

        _spriteBatchBegan = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XNAColor ConvertColor(Color color)
    {
        return new XNAColor(color.R, color.G, color.B, color.A);
    }

    public override void Clear(Color color)
    {
        _graphicsDevice.Clear(ConvertColor(color));
    }

    protected override void PreDrawTileBuffer(GameTileBuffer buffer)
    {
        base.PreDrawTileBuffer(buffer);

        EndSpriteBatch();

        var graphicsDevice = GraphicsDevice;
        graphicsDevice.SetRenderTarget(_screenshotRenderTarget);
        graphicsDevice.BlendState = _normalState;
        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        graphicsDevice.DepthStencilState = DepthStencilState.None;
    }

    public override void Close()
    {
    }

    protected override IGameTexture CreateWhitePixel()
    {
        Texture2D platformTexture = new(_graphicsDevice, 1, 1);
        platformTexture.SetData([XNAColor.White]);
        var gameTexture = CreateGameTextureFromPlatformTexture("system:white_1px_1px", platformTexture);
        return gameTexture;
    }

    public ContentManager GetContentManager()
    {
        return _contentManager;
    }

    public override void DrawString(
        string text,
        IFont? font,
        int size,
        float x,
        float y,
        float fontScale,
        Color? fontColor,
        bool worldPos = true,
        IGameRenderTexture? renderTexture = null,
        Color? borderColor = null
    )
    {
        if (font is not Font<SpriteFont> platformFont)
        {
            return;
        }

        var renderer = platformFont.GetRendererFor(size);

        var spriteFont = renderer.PlatformObject;

        text = SanitizeText(text, spriteFont);

        StartSpritebatch(
            _currentView,
            GameBlendModes.None,
            null,
            renderTexture
        );

        if (borderColor != null && borderColor != Color.Transparent)
        {
            var platformBorderColor = ConvertColor(borderColor);
            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x, y - 1),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x - 1, y),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x + 1, y),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x, y + 1),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );
        }

        _spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), ConvertColor(fontColor));
    }

    public override void DrawString(
        string text,
        IFont? font,
        int size,
        float x,
        float y,
        float fontScale,
        Color fontColor,
        bool worldPos,
        IGameRenderTexture renderTexture,
        FloatRect clipRect,
        Color? borderColor = null
    )
    {
        if (font is not Font<SpriteFont> platformFont)
        {
            return;
        }

        var renderer = platformFont.GetRendererFor(size);

        var spriteFont = renderer.PlatformObject;

        text = SanitizeText(text, spriteFont);

        x += _currentView.X;
        y += _currentView.Y;

        var clr = ConvertColor(fontColor);

        //Copy the current scissor rect so we can restore it after
        var currentRect = _spriteBatch.GraphicsDevice.ScissorRectangle;
        StartSpritebatch(
            _currentView,
            GameBlendModes.None,
            null,
            renderTexture,
            false,
            _rasterizerState,
            true
        );

        //Set the current scissor rectangle
        _spriteBatch.GraphicsDevice.ScissorRectangle = new XNARectangle(
            (int)clipRect.X,
            (int)clipRect.Y,
            (int)clipRect.Width,
            (int)clipRect.Height
        );

        if (borderColor != null && borderColor != Color.Transparent)
        {
            var platformBorderColor = ConvertColor(borderColor);
            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x, y - 1),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x - 1, y),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x + 1, y),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );

            _spriteBatch.DrawString(
                spriteFont,
                text,
                new Vector2(x, y + 1),
                platformBorderColor,
                0f,
                Vector2.Zero,
                new Vector2(fontScale, fontScale),
                SpriteEffects.None,
                0
            );
        }

        _spriteBatch.DrawString(
            spriteFont,
            text,
            new Vector2(x, y),
            clr,
            0f,
            Vector2.Zero,
            new Vector2(fontScale, fontScale),
            SpriteEffects.None,
            0
        );

        EndSpriteBatch();

        //Reset scissor rectangle to the saved value
        _spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
    }

    public override GameTileBuffer CreateTileBuffer()
    {
        return new MonoTileBuffer(this, _graphicsDevice);
    }

    public override void DrawTexture(
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
        float rotationDegrees = 0,
        bool isUi = false,
        bool drawImmediate = false
    )
    {
        var texture = tex?.GetTexture();
        if (texture == null)
        {
            return;
        }

        var packRotated = false;

        var pack = tex.AtlasReference;
        if (pack != null)
        {
            if (pack.IsRotated)
            {
                rotationDegrees -= 90;
                var z = tw;
                tw = th;
                th = z;

                z = sx;
                sx = pack.Bounds.Right - sy - sh;
                sy = pack.Bounds.Top + z;

                z = sw;
                sw = sh;
                sh = z;
                packRotated = true;
            }
            else
            {
                sx += pack.Bounds.X;
                sy += pack.Bounds.Y;
            }
        }

        var origin = Vector2.Zero;
        if (Math.Abs(rotationDegrees) > 0.01)
        {
            rotationDegrees = (float)(Math.PI / 180 * rotationDegrees);
            origin = new Vector2(sw / 2f, sh / 2f);

            //TODO: Optimize in terms of memory AND performance.
            var pnt = new Vector2(0, 0);
            var pnt1 = new Vector2(tw, 0);
            var pnt2 = new Vector2(0, th);
            var cntr = new Vector2(tw / 2, th / 2);

            var pntMod = Rotate(pnt, cntr, rotationDegrees);
            var pntMod2 = Rotate(pnt1, cntr, rotationDegrees);
            var pntMod3 = Rotate(pnt2, cntr, rotationDegrees);

            var width = (int)Math.Round(GetDistance(pntMod.X, pntMod.Y, pntMod2.X, pntMod2.Y));
            var height = (int)Math.Round(GetDistance(pntMod.X, pntMod.Y, pntMod3.X, pntMod3.Y));

            if (packRotated)
            {
                (width, height) = (height, width);
            }

            tx += width / 2f;
            ty += height / 2f;
        }

        // Cache the result of ConvertColor(renderColor) in a temporary variable.
        var color = ConvertColor(renderColor);

        // Use a single Draw method to avoid duplicating code.
        void Draw(FloatRect currentView, IGameRenderTexture targetObject)
        {
            StartSpritebatch(
                currentView,
                blendMode,
                shader,
                targetObject,
                false,
                null,
                drawImmediate
            );

            _spriteBatch.Draw(
                (Texture2D)texture,
                new Vector2(tx, ty),
                new XNARectangle((int)sx, (int)sy, (int)sw, (int)sh),
                color,
                rotationDegrees,
                origin,
                new Vector2(tw / sw, th / sh),
                SpriteEffects.None,
                0
            );
        }

        if (renderTarget == null)
        {
            if (isUi)
            {
                tx += _currentView.X;
                ty += _currentView.Y;
            }

            Draw(_currentView, null);
        }
        else
        {
            Draw(new FloatRect(0, 0, renderTarget.Width, renderTarget.Height), renderTarget);
        }
    }

    private static double GetDistance(double x1, double y1, double x2, double y2)
    {
        var a2 = Math.Pow(x2 - x1, 2);
        var b2 = Math.Pow(y2 - y1, 2);
        var root = Math.Sqrt(a2 + b2);

        return root;
    }

    private Vector2 Rotate(Vector2 pnt, Vector2 ctr, float angle)
    {
        return new Vector2(
            (float)(pnt.X + (ctr.X * Math.Cos(angle)) - (ctr.Y * Math.Sin(angle))),
            (float)(pnt.Y + (ctr.X * Math.Sin(angle)) + (ctr.Y * Math.Cos(angle)))
        );
    }

    protected override void DoEnd()
    {
        EndSpriteBatch();

        ++_frames;
        var now = Timing.Global.MillisecondsUtc;
        if (_nextFpsTime > now)
        {
            return;
        }

        var delta = now - _nextFpsTime;
        var scale = delta / 1000f;

        var frames = _frames;
        var scaledFrames = (int)(frames * scale);
        _fps = frames;//scaledFrames;
        _frames = 0;//Math.Max(0, frames - scaledFrames);
        _nextFpsTime = now + 1000;
        _gameWindow.Title = Strings.Main.GameName;
    }

    public override int FPS => _fps;

    public override int ScreenHeight => _screenHeight;

    public override int ScreenWidth => _screenWidth;

    public override string GetResolutionString()
    {
        return _screenWidth + "x" + _screenHeight;
    }

    public Resolution[] GetAllowedResolutions()
    {
        var allowedResolutions = new[]
            {
                new Resolution(800),
                new Resolution(1024, 768),
                new Resolution(1024, 720),
                new Resolution(1280, 720),
                new Resolution(1280, 768),
                new Resolution(1280, 1024),
                new Resolution(1360, 768),
                new Resolution(1366, 768),
                new Resolution(1440, 1050),
                new Resolution(1440, 900),
                new Resolution(1600, 900),
                new Resolution(1680, 1050),
                new Resolution(1920, 1080),
            }.Concat(
                _graphicsDevice.Adapter.SupportedDisplayModes
                    .Select(displayMode => new Resolution(displayMode.Width, displayMode.Height))
                    .Where(resolution => resolution is { X: >= 800, Y: >= 480 })
            )
            .Distinct()
            .Order()
            .ToArray();

        if (Steam.SteamDeck)
        {
            allowedResolutions = new[]
            {
                new Resolution(_graphicsDevice.DisplayMode.Width, _graphicsDevice.DisplayMode.Height),
            };
        }

        return allowedResolutions;
    }

    public override List<string> ValidVideoModes
    {
        get
        {
            if (_validVideoModes != null)
            {
                return _validVideoModes;
            }

            _validVideoModes = new List<string>();

            var allowedResolutions = GetAllowedResolutions();

            var displayWidth = _graphicsDevice.DisplayMode?.Width;
            var displayHeight = _graphicsDevice.DisplayMode?.Height;

            foreach (var resolution in allowedResolutions)
            {
                if (resolution.X > displayWidth)
                {
                    continue;
                }

                if (resolution.Y > displayHeight)
                {
                    continue;
                }

                _validVideoModes.Add(resolution.ToString());
            }

            return _validVideoModes;
        }
    }

    public override FloatRect GetView()
    {
        return _currentView;
    }

    public override void Init()
    {
        if (_initializing)
        {
            return;
        }

        _initializing = true;

        var database = Globals.Database;
        var validVideoModes = ValidVideoModes;

        if (database.TargetResolution < 0)
        {
            var allowedResolutions = GetAllowedResolutions();
            var currentDisplayMode = _graphicsDevice.Adapter.CurrentDisplayMode;
            var defaultResolution = allowedResolutions.LastOrDefault(
                allowedResolution => currentDisplayMode.Width != allowedResolution.X &&
                                     currentDisplayMode.Width ==
                                     allowedResolution.X * currentDisplayMode.Height / allowedResolution.Y
            );

            if (defaultResolution == default)
            {
                defaultResolution = allowedResolutions.LastOrDefault(
                    allowedResolution => allowedResolution.X * 9 / 16 == allowedResolution.Y &&
                                         allowedResolution.X < currentDisplayMode.Width &&
                                         allowedResolution.Y < currentDisplayMode.Height
                );
            }

            if (defaultResolution != default)
            {
                database.TargetResolution = allowedResolutions.IndexOf(defaultResolution);
            }
        }

        var targetResolution = MathHelper.Clamp(database.TargetResolution, 0, validVideoModes.Count - 1);

        if (targetResolution != database.TargetResolution)
        {
            Debug.Assert(database != default, "database != null");
            database.TargetResolution = targetResolution;
            database.SavePreference("Resolution", database.TargetResolution.ToString(CultureInfo.InvariantCulture));
        }

        var targetVideoMode = validVideoModes[targetResolution];
        if (!string.IsNullOrWhiteSpace(targetVideoMode) && Resolution.TryParse(targetVideoMode, out var resolution))
        {
            PreferredResolution = resolution;
        }

        _graphicsDeviceManager.PreferredBackBufferWidth = PreferredResolution.X;
        _graphicsDeviceManager.PreferredBackBufferHeight = PreferredResolution.Y;

        UpdateGraphicsState(ActiveResolution.X, ActiveResolution.Y, true);

        _initializing = false;
    }

    public void Init(GraphicsDevice graphicsDevice)
    {
        ArgumentNullException.ThrowIfNull(graphicsDevice);

        _graphicsDevice = graphicsDevice;
        _basicEffect = new BasicEffect(graphicsDevice);
        _basicEffect.LightingEnabled = false;
        _basicEffect.TextureEnabled = true;
        _spriteBatch = new SpriteBatch(graphicsDevice);

        _basicShader = new Shader(this, _basicEffect);
    }

    public override IFont LoadFont(string fontName, IDictionary<int, FileInfo> fontSourcesBySize)
    {
        // Return a new MonoFont with the extracted name and size
        return new MonoFont(fontName, fontSourcesBySize, _contentManager);
    }

    public override GameShader LoadShader(string shaderName)
    {
        return new Shader(this, shaderName, _contentManager);
    }

    public override System.Numerics.Vector2 MeasureText(string? text, IFont? font, int size, float fontScale)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default;
        }

        if (font is not Font<SpriteFont> platformFont)
        {
            return default;
        }

        var renderer = platformFont.GetRendererFor(size);
        var spriteFont = renderer.PlatformObject;
        text = SanitizeText(text, spriteFont);

        var textMeasurement = spriteFont.MeasureString(text);

        return new System.Numerics.Vector2(textMeasurement.X * fontScale, textMeasurement.Y * fontScale);
    }

    private readonly Dictionary<SpriteFont, char> _defaultCharacterForSpriteFont = [];
    private GameShader _basicShader;

    private string SanitizeText(string text, SpriteFont spriteFont)
    {
        if (!_defaultCharacterForSpriteFont.TryGetValue(spriteFont, out var defaultCharacter))
        {
            defaultCharacter = spriteFont.DefaultCharacter ??
                               (spriteFont.Characters.Contains(' ') ? ' ' : spriteFont.Characters.FirstOrDefault());
            _defaultCharacterForSpriteFont[spriteFont] = defaultCharacter;
        }

        var invalidCharacters = text.Where(@char => !spriteFont.Characters.Contains(@char)).Distinct().ToArray();
        text = invalidCharacters.Aggregate(
            text,
            (current, invalidCharacter) => current.Replace(invalidCharacter, defaultCharacter)
        );
        return text;
    }

    private Matrix CreateViewMatrix(FloatRect view)
    {
        return Matrix.CreateRotationZ(0f) *
               Matrix.CreateTranslation(-view.X, -view.Y, 0) *
               Matrix.CreateScale(new Vector3(Scale));
    }

    public override void SetView(FloatRect view)
    {
        _currentView = view;

        Matrix.CreateOrthographicOffCenter(
            0,
            view.Width,
            view.Height,
            0,
            0f,
            -1,
            out var projection
        );
        projection.M41 += -0.5f * projection.M11;
        projection.M42 += -0.5f * projection.M22;
        _basicEffect.Projection = projection;
        _basicEffect.View = CreateViewMatrix(view);
    }

    public override bool BeginScreenshot()
    {
        var graphicsDevice = GraphicsDevice;

        if (_screenWidth < 1 || _screenHeight < 1)
        {
            return false;
        }

        _screenshotRenderTarget = new RenderTarget2D(
            graphicsDevice,
            _screenWidth,
            _screenHeight,
            false,
            graphicsDevice.PresentationParameters.BackBufferFormat,
            graphicsDevice.PresentationParameters.DepthStencilFormat,
            /* For whatever reason if this isn't zero everything breaks in .NET 7 on MacOS and most Windows devices */
            0, // mGraphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.PreserveContents
        );

        return true;
    }

    private bool WriteScreenshotRenderTargetAsPngTo(Stream stream, string? hint)
    {
        if (_screenshotRenderTarget.IsContentLost)
        {
            return false;
        }

        try
        {
            _screenshotRenderTarget.SaveAsPng(stream, _screenshotRenderTarget.Width, _screenshotRenderTarget.Height);
            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                exception,
                "Exception when writing screenshot to {Target}",
                string.IsNullOrWhiteSpace(hint) ? "stream" : hint
            );
            return false;
        }
    }

    public override void EndScreenshot()
    {
        if (_screenshotRenderTarget == null)
        {
            return;
        }

        ProcessScreenshots(WriteScreenshotRenderTargetAsPngTo);

        var graphicsDevice = GraphicsDevice;

        var skippedFrame = _screenshotRenderTarget;
        _screenshotRenderTarget = null;
        graphicsDevice.SetRenderTarget(null);

        if (!Begin())
        {
            return;
        }

        _spriteBatch?.Draw(skippedFrame, new XNARectangle(), XNAColor.White);
        End();
    }
}