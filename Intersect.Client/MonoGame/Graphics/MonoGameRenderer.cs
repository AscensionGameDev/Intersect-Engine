using Intersect.Client.Classes.MonoGame.Graphics;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Localization;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using XNAColor = Microsoft.Xna.Framework.Color;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace Intersect.Client.MonoGame.Graphics
{
    internal class MonoGameRenderer : GameRenderer
    {
        [NotNull] private readonly List<MonoGameTexture> mAllTextures = new List<MonoGameTexture>();

        private BasicEffect mBasicEffect;

        private BasicEffect BasicEffect =>
            mBasicEffect ??
            (mBasicEffect = new BasicEffect(GraphicsDevice)
            {
                LightingEnabled = false,
                TextureEnabled = true
            });

        private ContentManager mContentManager;

        private GameBlendModes mCurrentBlendmode = GameBlendModes.None;

        private IShader mCurrentShader;

        private FloatRect mCurrentSpriteView;

        private IRenderTexture mCurrentTarget;

        private FloatRect mCurrentView;

        private BlendState mCutoutState;

        private int mDisplayHeight;

        private bool mDisplayModeChanged = false;

        private int mDisplayWidth;

        private int mFpsCount;

        private long mFpsTimer;

        private long mFsChangedTimer = -1;

        private bool mInitialized;

        private bool mInitializing;

        private BlendState mMultiplyState;

        private BlendState mNormalState;

        private DisplayMode mOldDisplayMode;

        RasterizerState mRasterizerState = new RasterizerState() {ScissorTestEnable = true};

        private int mScreenHeight;

        private RenderTarget2D mScreenshotRenderTarget;

        private int mScreenWidth;

        private SpriteBatch mSpriteBatch;

        private SpriteBatch SpriteBatch =>
            mSpriteBatch ??
            (mSpriteBatch = new SpriteBatch(GraphicsDevice));

        private bool mSpriteBatchBegan;

        private List<string> mValidVideoModes;

        private IRenderTexture mWhiteTexture;

        protected new MonoGameContext GameContext => base.GameContext as MonoGameContext;

        public IntersectGame Game => GameContext.Game;

        public ContentManager Content => Game.Content;

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public GraphicsDeviceManager GraphicsDeviceManager => Game.GraphicsDeviceManager;

        public GameWindow Window => Game.Window;

        public MonoGameRenderer(MonoGameContext gameContext) : base(gameContext)
        {
            mNormalState = new BlendState()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };

            mMultiplyState = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero
            };

            mCutoutState = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
        }

        public IList<string> ValidVideoModes => GetValidVideoModes();

        public void UpdateGraphicsState(int width, int height, bool initial = false)
        {
            var currentDisplayMode = GraphicsDevice.Adapter.CurrentDisplayMode;

            if (GameContext.Storage.Preferences.Fullscreen)
            {
                var supported = false;
                foreach (var mode in GraphicsDevice.Adapter.SupportedDisplayModes)
                {
                    if (mode.Width == width && mode.Height == height)
                    {
                        supported = true;
                    }
                }

                if (!supported)
                {
                    GameContext.Storage.Preferences.Fullscreen = false;
                    GameContext.Storage.Preferences.Save();
                    Interface.Interface.MsgboxErrors.Add(
                        new KeyValuePair<string, string>(
                            Strings.Errors.displaynotsupported,
                            Strings.Errors.displaynotsupportederror.ToString(width + "x" + height)
                        )
                    );
                }
            }

            var fsChanged = GraphicsDeviceManager.IsFullScreen != GameContext.Storage.Preferences.Fullscreen && !GameContext.Storage.Preferences.Fullscreen;

            GraphicsDeviceManager.IsFullScreen = GameContext.Storage.Preferences.Fullscreen;
            if (fsChanged)
            {
                GraphicsDeviceManager.ApplyChanges();
            }

            mScreenWidth = width;
            mScreenHeight = height;
            GraphicsDeviceManager.PreferredBackBufferWidth = width;
            GraphicsDeviceManager.PreferredBackBufferHeight = height;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = GameContext.Storage.Preferences.Fps == 0;

            if (GameContext.Storage.Preferences.Fps == 1)
            {
                Game.TargetElapsedTime = new TimeSpan(333333);
            }
            else if (GameContext.Storage.Preferences.Fps == 2)
            {
                Game.TargetElapsedTime = new TimeSpan(333333 / 2);
            }
            else if (GameContext.Storage.Preferences.Fps == 3)
            {
                Game.TargetElapsedTime = new TimeSpan(333333 / 3);
            }
            else if (GameContext.Storage.Preferences.Fps == 4)
            {
                Game.TargetElapsedTime = new TimeSpan(333333 / 4);
            }

            Game.IsFixedTimeStep = GameContext.Storage.Preferences.Fps > 0;

            GraphicsDeviceManager.ApplyChanges();

            mDisplayWidth = GraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            mDisplayHeight = GraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            if (fsChanged || initial)
            {
                Window.Position = new Microsoft.Xna.Framework.Point(
                    (mDisplayWidth - mScreenWidth) / 2, (mDisplayHeight - mScreenHeight) / 2
                );
            }

            mOldDisplayMode = currentDisplayMode;
            if (fsChanged)
            {
                mFsChangedTimer = Globals.System.GetTimeMs() + 1000;
            }

            if (fsChanged)
            {
                mDisplayModeChanged = true;
            }
        }

        public void CreateWhiteTexture()
        {
            mWhiteTexture = CreateRenderTexture(1, 1);
            mWhiteTexture.BeginFrame();
            mWhiteTexture.Clear(Color.White);
            mWhiteTexture.EndFrame();
        }

        public override bool Begin()
        {
            //GraphicsDeviceManagerDevice.SetRenderTarget(null);
            if (mFsChangedTimer > -1 && mFsChangedTimer < Globals.System.GetTimeMs())
            {
                GraphicsDeviceManager.PreferredBackBufferWidth--;
                GraphicsDeviceManager.ApplyChanges();
                GraphicsDeviceManager.PreferredBackBufferWidth++;
                GraphicsDeviceManager.ApplyChanges();
                mFsChangedTimer = -1;
            }

            if (Window.ClientBounds.Width != 0 &&
                Window.ClientBounds.Height != 0 &&
                (Window.ClientBounds.Width != mScreenWidth || Window.ClientBounds.Height != mScreenHeight) &&
                !GraphicsDeviceManager.IsFullScreen)
            {
                if (mOldDisplayMode != GraphicsDeviceManager.GraphicsDevice.DisplayMode)
                {
                    mDisplayModeChanged = true;
                }

                UpdateGraphicsState(mScreenWidth, mScreenHeight);
            }

            StartSpritebatch(mCurrentView, GameBlendModes.None, null, null, true, null);

            return true;
        }

        public Pointf GetMouseOffset()
        {
            return new Pointf(
                GraphicsDeviceManager.PreferredBackBufferWidth / (float) Window.ClientBounds.Width,
                GraphicsDeviceManager.PreferredBackBufferHeight / (float) Window.ClientBounds.Height
            );
        }

        private void StartSpritebatch(
            FloatRect view,
            GameBlendModes mode = GameBlendModes.None,
            IShader shader = null,
            IRenderTexture target = null,
            bool forced = false,
            RasterizerState rs = null,
            bool drawImmediate = false
        )
        {
            var viewsDiff = view.X != mCurrentSpriteView.X ||
                            view.Y != mCurrentSpriteView.Y ||
                            view.Width != mCurrentSpriteView.Width ||
                            view.Height != mCurrentSpriteView.Height;

            if (mode != mCurrentBlendmode ||
                shader != mCurrentShader ||
                shader != null && shader.Dirty ||
                target != mCurrentTarget ||
                viewsDiff ||
                forced ||
                drawImmediate ||
                !mSpriteBatchBegan)
            {
                if (mSpriteBatchBegan)
                {
                    SpriteBatch.End();
                }

                if (target != null)
                {
                    SetRenderTexture(target);
                }
                else
                {
                    GraphicsDevice?.SetRenderTarget(mScreenshotRenderTarget);
                }

                var blend = mNormalState;
                Effect useEffect = null;

                switch (mode)
                {
                    case GameBlendModes.None:
                        blend = mNormalState;

                        break;

                    case GameBlendModes.Alpha:
                        blend = BlendState.AlphaBlend;

                        break;

                    case GameBlendModes.Multiply:
                        blend = mMultiplyState;

                        break;

                    case GameBlendModes.Add:
                        blend = BlendState.Additive;

                        break;

                    case GameBlendModes.Opaque:
                        blend = BlendState.Opaque;

                        break;

                    case GameBlendModes.Cutout:
                        blend = mCutoutState;

                        break;
                }

                if (shader != null)
                {
                    useEffect = shader.GetShader<Effect>();
                    shader.MarkClean();
                }

                SpriteBatch.Begin(
                    drawImmediate ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, blend, SamplerState.PointClamp,
                    null, rs, useEffect,
                    Matrix.CreateRotationZ(0f) *
                    Matrix.CreateScale(new Vector3(1, 1, 1)) *
                    Matrix.CreateTranslation(-view.X, -view.Y, 0)
                );

                mCurrentSpriteView = view;
                mCurrentBlendmode = mode;
                mCurrentShader = shader;
                mCurrentTarget = target;
                mSpriteBatchBegan = true;
            }
        }

        public override bool DisplayModeChanged()
        {
            var changed = mDisplayModeChanged;
            mDisplayModeChanged = false;

            return changed;
        }

        public void EndSpriteBatch()
        {
            if (mSpriteBatchBegan)
            {
                SpriteBatch.End();
            }

            mSpriteBatchBegan = false;
        }

        public static Microsoft.Xna.Framework.Color ConvertColor(Color clr)
        {
            return new Microsoft.Xna.Framework.Color(clr.R, clr.G, clr.B, clr.A);
        }

        public override void Clear(Color color)
        {
            GraphicsDevice.Clear(ConvertColor(color));
        }

        public override void DrawTileBuffer(GameTileBuffer buffer)
        {
            EndSpriteBatch();
            GraphicsDevice?.SetRenderTarget(mScreenshotRenderTarget);
            GraphicsDevice.BlendState = mNormalState;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            ((MonoTileBuffer) buffer).Draw(BasicEffect, mCurrentView);
        }

        public override void Close()
        {
        }

        public override ITexture GetWhiteTexture()
        {
            return mWhiteTexture;
        }

        public ContentManager GetContentManager()
        {
            return mContentManager;
        }

        public override IRenderTexture CreateRenderTexture(int width, int height)
        {
            var renderTarget = new RenderTarget2D(
                GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat,
                GraphicsDevice.PresentationParameters.DepthStencilFormat,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents
            );

            return new MonoRenderTexture(this, renderTarget);
        }

        public override void SetRenderTexture(IRenderTexture renderTexture) =>
            GraphicsDevice.SetRenderTarget(renderTexture?.AsPlatformTexture<RenderTarget2D>());

        public override void DrawString(
            string text,
            IFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos = true,
            IRenderTexture gameRenderTexture = null,
            Color borderColor = null
        )
        {
            if (gameFont == null)
            {
                return;
            }

            var font = gameFont.AsPlatformFont<SpriteFont>();
            if (font == null)
            {
                return;
            }

            StartSpritebatch(mCurrentView, GameBlendModes.None, null, gameRenderTexture, false, null);
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }

            if (borderColor != null && borderColor != Color.Transparent)
            {
                SpriteBatch.DrawString(
                    font, text, new Vector2(x, y - 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x - 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x + 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x, y + 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );
            }

            SpriteBatch.DrawString(font, text, new Vector2(x, y), ConvertColor(fontColor));
        }

        public override void DrawString(
            string text,
            IFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos,
            IRenderTexture gameRenderTexture,
            FloatRect clipRect,
            Color borderColor = null
        )
        {
            if (gameFont == null)
            {
                return;
            }

            x += mCurrentView.X;
            y += mCurrentView.Y;

            //clipRect.X += _currentView.X;
            //clipRect.Y += _currentView.Y;
            var font = gameFont.AsPlatformFont<SpriteFont>();
            if (font == null)
            {
                return;
            }

            var clr = ConvertColor(fontColor);

            //Copy the current scissor rect so we can restore it after
            var currentRect = SpriteBatch.GraphicsDevice.ScissorRectangle;
            StartSpritebatch(mCurrentView, GameBlendModes.None, null, gameRenderTexture, false, mRasterizerState, true);

            //Set the current scissor rectangle
            SpriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(
                (int) clipRect.X, (int) clipRect.Y, (int) clipRect.Width, (int) clipRect.Height
            );

            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }

            if (borderColor != null && borderColor != Color.Transparent)
            {
                SpriteBatch.DrawString(
                    font, text, new Vector2(x, y - 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x - 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x + 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                SpriteBatch.DrawString(
                    font, text, new Vector2(x, y + 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );
            }

            SpriteBatch.DrawString(
                font, text, new Vector2(x, y), clr, 0f, Vector2.Zero, new Vector2(fontScale, fontScale),
                SpriteEffects.None, 0
            );

            EndSpriteBatch();

            //Reset scissor rectangle to the saved value
            SpriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
        }

        public override GameTileBuffer CreateTileBuffer()
        {
            return new MonoTileBuffer(GraphicsDevice);
        }

        public override void DrawTexture(
            ITexture tex,
            float sx,
            float sy,
            float sw,
            float sh,
            float tx,
            float ty,
            float tw,
            float th,
            Color renderColor,
            IRenderTexture gameRenderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            IShader shader = null,
            float rotationDegrees = 0,
            bool isUi = false,
            bool drawImmediate = false
        )
        {
            var texture = tex?.AsPlatformTexture<Texture2D>();
            if (texture == null)
            {
                return;
            }

            var packRotated = false;

            var pack = tex.TexturePackFrame;
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
                rotationDegrees = (float) (Math.PI / 180 * rotationDegrees);
                origin = new Vector2(sw / 2f, sh / 2f);

                //TODO: Optimize in terms of memory AND performance.
                var pnt = new Pointf(0, 0);
                var pnt1 = new Pointf((float) tw, 0);
                var pnt2 = new Pointf(0, (float) th);
                var cntr = new Pointf((float) tw / 2, (float) th / 2);

                var pntMod = Rotate(pnt, cntr, rotationDegrees);
                var pntMod2 = Rotate(pnt1, cntr, rotationDegrees);
                var pntMod3 = Rotate(pnt2, cntr, rotationDegrees);

                var width = (int) Math.Round(GetDistance(pntMod.X, pntMod.Y, pntMod2.X, pntMod2.Y));
                var height = (int) Math.Round(GetDistance(pntMod.X, pntMod.Y, pntMod3.X, pntMod3.Y));

                if (packRotated)
                {
                    var z = width;
                    width = height;
                    height = z;
                }

                tx += width / 2f;
                ty += height / 2f;
            }

            if (gameRenderTarget == null)
            {
                if (isUi)
                {
                    tx += mCurrentView.X;
                    ty += mCurrentView.Y;
                }

                StartSpritebatch(mCurrentView, blendMode, shader, null, false, null, drawImmediate);

                SpriteBatch.Draw(
                    (Texture2D) texture, new Vector2(tx, ty), new XNARectangle((int) sx, (int) sy, (int) sw, (int) sh),
                    ConvertColor(renderColor), rotationDegrees, origin, new Vector2(tw / sw, th / sh),
                    SpriteEffects.None, 0
                );
            }
            else
            {
                StartSpritebatch(
                    new FloatRect(0, 0, gameRenderTarget.Width, gameRenderTarget.Height), blendMode, shader,
                    gameRenderTarget, false, null, drawImmediate
                );

                SpriteBatch.Draw(
                    (Texture2D) texture, new Vector2(tx, ty), new XNARectangle((int) sx, (int) sy, (int) sw, (int) sh),
                    ConvertColor(renderColor), rotationDegrees, origin, new Vector2(tw / sw, th / sh),
                    SpriteEffects.None, 0
                );
            }
        }

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            var a2 = Math.Pow(x2 - x1, 2);
            var b2 = Math.Pow(y2 - y1, 2);
            var root = Math.Sqrt(a2 + b2);

            return root;
        }

        private Pointf Rotate(Pointf pnt, Pointf ctr, float angle)
        {
            return new Pointf(
                (float) (pnt.X + ctr.X * Math.Cos(angle) - ctr.Y * Math.Sin(angle)),
                (float) (pnt.Y + ctr.X * Math.Sin(angle) + ctr.Y * Math.Cos(angle))
            );
        }

        public override int Fps { get; protected set; }

        /// <inheritdoc />
        public override int ScreenWidth => mScreenWidth;

        /// <inheritdoc />
        public override int ScreenHeight => mScreenHeight;

        public override List<string> GetValidVideoModes()
        {
            if (mValidVideoModes != null)
            {
                return mValidVideoModes;
            }

            mValidVideoModes = new List<string>();

            var allowedResolutions = new[]
            {
                new Resolution(800, 600),
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
                new Resolution(1920, 1080)
            };

            var displayWidth = GraphicsDevice?.DisplayMode?.Width;
            var displayHeight = GraphicsDevice?.DisplayMode?.Height;

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

                mValidVideoModes.Add(resolution.ToString());
            }

            return mValidVideoModes;
        }

        public override FloatRect GetView()
        {
            return mCurrentView;
        }

        public override void Init()
        {
            if (mInitializing)
            {
                return;
            }

            mInitializing = true;

            var preferences = GameContext.Storage.Preferences;
            var validVideoModes = GetValidVideoModes();
            var targetResolution = preferences.PreferredResolution;

            if (targetResolution < 0 || validVideoModes?.Count <= targetResolution)
            {
                Debug.Assert(preferences != null, "preferences != null");
                preferences.PreferredResolution = 0;
                preferences.Save();
            }

            var targetVideoMode = validVideoModes?[targetResolution];
            if (Resolution.TryParse(targetVideoMode, out var resolution))
            {
                PreferredResolution = resolution;
            }

            GraphicsDeviceManager.PreferredBackBufferWidth = PreferredResolution.X;
            GraphicsDeviceManager.PreferredBackBufferHeight = PreferredResolution.Y;

            UpdateGraphicsState(ActiveResolution.X, ActiveResolution.Y, true);

            if (mWhiteTexture == null)
            {
                CreateWhiteTexture();
            }

            mInitializing = false;
        }

        public override IFont LoadFont(string filename)
        {
            //Get font size from filename, format should be name_size.xnb or whatever
            var name = GameContentManager.RemoveExtension(filename)
                .Replace(Path.Combine("resources", "fonts"), "")
                .TrimStart(Path.DirectorySeparatorChar);

            var parts = name.Split('_');
            if (parts.Length >= 1)
            {
                if (int.TryParse(parts[parts.Length - 1], out var size))
                {
                    name = "";
                    for (var i = 0; i <= parts.Length - 2; i++)
                    {
                        name += parts[i];
                        if (i + 1 < parts.Length - 2)
                        {
                            name += "_";
                        }
                    }

                    return new MonoFont(name, filename, size, mContentManager);
                }
            }

            return null;
        }

        public override IShader LoadShader(string name) => new MonoShader(
            name, mContentManager.Load<Effect>(GameContentManager.RemoveExtension(name))
        );

        public override ITexture LoadTexture(TextureType textureType, string assetName)
        {
            var texturePackFrame = GameContext.ContentManager.FindTexturePackFrameFor(textureType, assetName);
            return LoadTexture(assetName, texturePackFrame);
        }

        public override ITexture LoadTexture(string filename, ITexturePackFrame texturePackFrame = null)
        {
            if (texturePackFrame != null)
            {
                var tx = new MonoGameTexture(GraphicsDevice, filename, texturePackFrame);
                mAllTextures.Add(tx);

                return tx;
            }

            var tex = new MonoGameTexture(GraphicsDevice, filename);
            mAllTextures.Add(tex);

            return tex;
        }

        /// <inheritdoc />
        public override ITexture LoadTexture(string assetName, Func<Stream> createStream) =>
            new MonoGameTexture(GraphicsDevice, assetName, createStream);

        public override Pointf MeasureText(string text, IFont gameFont, float fontScale)
        {
            if (gameFont == null)
            {
                return Pointf.Empty;
            }

            var font = gameFont.AsPlatformFont<SpriteFont>();
            if (font == null)
            {
                return Pointf.Empty;
            }

            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }

            var size = font.MeasureString(text);

            return new Pointf(size.X * fontScale, size.Y * fontScale);
        }

        public override void SetView(FloatRect view)
        {
            mCurrentView = view;

            Matrix projection;
            Matrix.CreateOrthographicOffCenter(0, view.Width, view.Height, 0, 0f, -1, out projection);
            projection.M41 += -0.5f * projection.M11;
            projection.M42 += -0.5f * projection.M22;
            BasicEffect.Projection = projection;
            BasicEffect.View = Matrix.CreateRotationZ(0f) *
                                Matrix.CreateScale(new Vector3(1, 1, 1)) *
                                Matrix.CreateTranslation(-view.X, -view.Y, 0);

            return;
        }

        public override bool BeginScreenshot()
        {
            if (GraphicsDevice == null)
            {
                return false;
            }

            mScreenshotRenderTarget = new RenderTarget2D(
                GraphicsDevice, mScreenWidth, mScreenHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                GraphicsDevice.PresentationParameters.DepthStencilFormat,
                GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents
            );

            return true;
        }

        /// <inheritdoc />
        public override void End(bool frame = true)
        {
            EndSpriteBatch();
            if (frame)
            {
                mFpsCount++;
                if (mFpsTimer < Globals.System.GetTimeMs())
                {
                    Fps = mFpsCount;
                    mFpsCount = 0;
                    mFpsTimer = Globals.System.GetTimeMs() + 1000;
                    Window.Title = Strings.Main.gamename;
                }

                foreach (var texture in mAllTextures)
                {
                    texture?.Update();
                }
            }
        }

        public override void EndScreenshot()
        {
            if (mScreenshotRenderTarget == null)
            {
                return;
            }

            ScreenshotRequests.ForEach(
                screenshotRequestStream =>
                {
                    if (screenshotRequestStream == null)
                    {
                        return;
                    }

                    mScreenshotRenderTarget.SaveAsPng(
                        screenshotRequestStream, mScreenshotRenderTarget.Width, mScreenshotRenderTarget.Height
                    );

                    screenshotRequestStream.Close();
                }
            );

            ScreenshotRequests.Clear();

            if (GraphicsDevice == null)
            {
                return;
            }

            var skippedFrame = mScreenshotRenderTarget;
            mScreenshotRenderTarget = null;
            GraphicsDevice.SetRenderTarget(null);

            if (!Begin())
            {
                return;
            }

            SpriteBatch?.Draw(skippedFrame, new XNARectangle(), XNAColor.White);
            End();
        }
    }
}
