using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Intersect.Client.Classes.MonoGame.Graphics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Localization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using XNARectangle = Microsoft.Xna.Framework.Rectangle;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace Intersect.Client.MonoGame.Graphics
{

    public class MonoRenderer : GameRenderer
    {

        private readonly List<MonoTexture> mAllTextures = new List<MonoTexture>();

        private BasicEffect mBasicEffect;

        private ContentManager mContentManager;

        private GameBlendModes mCurrentBlendmode = GameBlendModes.None;

        private GameShader mCurrentShader;

        private FloatRect mCurrentSpriteView;

        private GameRenderTexture mCurrentTarget;

        private FloatRect mCurrentView;

        private BlendState mCutoutState;

        private int mDisplayHeight;

        private bool mDisplayModeChanged = false;

        private int mDisplayWidth;

        private int mFps;

        private int mFpsCount;

        private long mFpsTimer;

        private long mFsChangedTimer = -1;

        private Game mGame;

        private GameWindow mGameWindow;

        private GraphicsDeviceManager mGraphics;

        private GraphicsDevice mGraphicsDevice;

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

        private bool mSpriteBatchBegan;

        private List<string> mValidVideoModes;

        private GameRenderTexture mWhiteTexture;

        public MonoRenderer(GraphicsDeviceManager graphics, ContentManager contentManager, Game monoGame)
        {
            mGame = monoGame;
            mGraphics = graphics;
            mContentManager = contentManager;

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

            mGameWindow = monoGame.Window;
        }

        public IList<string> ValidVideoModes => GetValidVideoModes();

        public void UpdateGraphicsState(int width, int height, bool initial = false)
        {
            var currentDisplayMode = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode;

            if (Globals.Database.FullScreen)
            {
                var supported = false;
                foreach (var mode in mGraphics.GraphicsDevice.Adapter.SupportedDisplayModes)
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
                    Interface.Interface.MsgboxErrors.Add(
                        new KeyValuePair<string, string>(
                            Strings.Errors.displaynotsupported,
                            Strings.Errors.displaynotsupportederror.ToString(width + "x" + height)
                        )
                    );
                }
            }

            var fsChanged = mGraphics.IsFullScreen != Globals.Database.FullScreen && !Globals.Database.FullScreen;

            mGraphics.IsFullScreen = Globals.Database.FullScreen;
            if (fsChanged)
            {
                mGraphics.ApplyChanges();
            }

            mScreenWidth = width;
            mScreenHeight = height;
            mGraphics.PreferredBackBufferWidth = width;
            mGraphics.PreferredBackBufferHeight = height;
            mGraphics.SynchronizeWithVerticalRetrace = Globals.Database.TargetFps == 0;

            if (Globals.Database.TargetFps == 1)
            {
                mGame.TargetElapsedTime = new TimeSpan(333333);
            }
            else if (Globals.Database.TargetFps == 2)
            {
                mGame.TargetElapsedTime = new TimeSpan(333333 / 2);
            }
            else if (Globals.Database.TargetFps == 3)
            {
                mGame.TargetElapsedTime = new TimeSpan(333333 / 3);
            }
            else if (Globals.Database.TargetFps == 4)
            {
                mGame.TargetElapsedTime = new TimeSpan(333333 / 4);
            }

            mGame.IsFixedTimeStep = Globals.Database.TargetFps > 0;

            mGraphics.ApplyChanges();

            mDisplayWidth = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            mDisplayHeight = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            if (fsChanged || initial)
            {
                mGameWindow.Position = new Microsoft.Xna.Framework.Point(
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
            mWhiteTexture.Begin();
            mWhiteTexture.Clear(Color.White);
            mWhiteTexture.End();
        }

        public override bool Begin()
        {
            //mGraphicsDevice.SetRenderTarget(null);
            if (mFsChangedTimer > -1 && mFsChangedTimer < Globals.System.GetTimeMs())
            {
                mGraphics.PreferredBackBufferWidth--;
                mGraphics.ApplyChanges();
                mGraphics.PreferredBackBufferWidth++;
                mGraphics.ApplyChanges();
                mFsChangedTimer = -1;
            }

            if (mGameWindow.ClientBounds.Width != 0 &&
                mGameWindow.ClientBounds.Height != 0 &&
                (mGameWindow.ClientBounds.Width != mScreenWidth || mGameWindow.ClientBounds.Height != mScreenHeight) &&
                !mGraphics.IsFullScreen)
            {
                if (mOldDisplayMode != mGraphics.GraphicsDevice.DisplayMode)
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
                mGraphics.PreferredBackBufferWidth / (float) mGameWindow.ClientBounds.Width,
                mGraphics.PreferredBackBufferHeight / (float) mGameWindow.ClientBounds.Height
            );
        }

        private void StartSpritebatch(
            FloatRect view,
            GameBlendModes mode = GameBlendModes.None,
            GameShader shader = null,
            GameRenderTexture target = null,
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
                shader != null && shader.ValuesChanged() ||
                target != mCurrentTarget ||
                viewsDiff ||
                forced ||
                drawImmediate ||
                !mSpriteBatchBegan)
            {
                if (mSpriteBatchBegan)
                {
                    mSpriteBatch.End();
                }

                if (target != null)
                {
                    mGraphicsDevice?.SetRenderTarget((RenderTarget2D) target.GetTexture());
                }
                else
                {
                    mGraphicsDevice?.SetRenderTarget(mScreenshotRenderTarget);
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
                    useEffect = (Effect) shader.GetShader();
                    shader.ResetChanged();
                }

                mSpriteBatch.Begin(
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
                mSpriteBatch.End();
            }

            mSpriteBatchBegan = false;
        }

        public static Microsoft.Xna.Framework.Color ConvertColor(Color clr)
        {
            return new Microsoft.Xna.Framework.Color(clr.R, clr.G, clr.B, clr.A);
        }

        public override void Clear(Color color)
        {
            mGraphicsDevice.Clear(ConvertColor(color));
        }

        public override void DrawTileBuffer(GameTileBuffer buffer)
        {
            EndSpriteBatch();
            mGraphicsDevice?.SetRenderTarget(mScreenshotRenderTarget);
            mGraphicsDevice.BlendState = mNormalState;
            mGraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            mGraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            mGraphicsDevice.DepthStencilState = DepthStencilState.None;

            ((MonoTileBuffer) buffer).Draw(mBasicEffect, mCurrentView);
        }

        public override void Close()
        {
        }

        public override GameTexture GetWhiteTexture()
        {
            return mWhiteTexture;
        }

        public ContentManager GetContentManager()
        {
            return mContentManager;
        }

        public override GameRenderTexture CreateRenderTexture(int width, int height)
        {
            return new MonoRenderTexture(mGraphicsDevice, width, height);
        }

        public override void DrawString(
            string text,
            GameFont gameFont,
            float x,
            float y,
            float fontScale,
            Color fontColor,
            bool worldPos = true,
            GameRenderTexture renderTexture = null,
            Color borderColor = null
        )
        {
            if (gameFont == null)
            {
                return;
            }

            var font = (SpriteFont) gameFont.GetFont();
            if (font == null)
            {
                return;
            }

            StartSpritebatch(mCurrentView, GameBlendModes.None, null, renderTexture, false, null);
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }

            if (borderColor != null && borderColor != Color.Transparent)
            {
                mSpriteBatch.DrawString(
                    font, text, new Vector2(x, y - 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x - 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x + 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x, y + 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );
            }

            mSpriteBatch.DrawString(font, text, new Vector2(x, y), ConvertColor(fontColor));
        }

        public override void DrawString(
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
            var font = (SpriteFont) gameFont.GetFont();
            if (font == null)
            {
                return;
            }

            var clr = ConvertColor(fontColor);

            //Copy the current scissor rect so we can restore it after
            var currentRect = mSpriteBatch.GraphicsDevice.ScissorRectangle;
            StartSpritebatch(mCurrentView, GameBlendModes.None, null, renderTexture, false, mRasterizerState, true);

            //Set the current scissor rectangle
            mSpriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(
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
                mSpriteBatch.DrawString(
                    font, text, new Vector2(x, y - 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x - 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x + 1, y), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );

                mSpriteBatch.DrawString(
                    font, text, new Vector2(x, y + 1), ConvertColor(borderColor), 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0
                );
            }

            mSpriteBatch.DrawString(
                font, text, new Vector2(x, y), clr, 0f, Vector2.Zero, new Vector2(fontScale, fontScale),
                SpriteEffects.None, 0
            );

            EndSpriteBatch();

            //Reset scissor rectangle to the saved value
            mSpriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
        }

        public override GameTileBuffer CreateTileBuffer()
        {
            return new MonoTileBuffer(mGraphicsDevice);
        }

        public override void DrawTexture(
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
            GameRenderTexture renderTarget = null,
            GameBlendModes blendMode = GameBlendModes.None,
            GameShader shader = null,
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

            var pack = tex.GetTexturePackFrame();
            if (pack != null)
            {
                if (pack.Rotated)
                {
                    rotationDegrees -= 90;
                    var z = tw;
                    tw = th;
                    th = z;

                    z = sx;
                    sx = pack.Rect.Right - sy - sh;
                    sy = pack.Rect.Top + z;

                    z = sw;
                    sw = sh;
                    sh = z;
                    packRotated = true;
                }
                else
                {
                    sx += pack.Rect.X;
                    sy += pack.Rect.Y;
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

            if (renderTarget == null)
            {
                if (isUi)
                {
                    tx += mCurrentView.X;
                    ty += mCurrentView.Y;
                }

                StartSpritebatch(mCurrentView, blendMode, shader, null, false, null, drawImmediate);

                mSpriteBatch.Draw(
                    (Texture2D) texture, new Vector2(tx, ty), new XNARectangle((int) sx, (int) sy, (int) sw, (int) sh),
                    ConvertColor(renderColor), rotationDegrees, origin, new Vector2(tw / sw, th / sh),
                    SpriteEffects.None, 0
                );
            }
            else
            {
                StartSpritebatch(
                    new FloatRect(0, 0, renderTarget.GetWidth(), renderTarget.GetHeight()), blendMode, shader,
                    renderTarget, false, null, drawImmediate
                );

                mSpriteBatch.Draw(
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

        public override void End()
        {
            EndSpriteBatch();
            mFpsCount++;
            if (mFpsTimer < Globals.System.GetTimeMs())
            {
                mFps = mFpsCount;
                mFpsCount = 0;
                mFpsTimer = Globals.System.GetTimeMs() + 1000;
                mGameWindow.Title = Strings.Main.gamename;
            }

            foreach (var texture in mAllTextures)
            {
                texture?.Update();
            }
        }

        public override int GetFps()
        {
            return mFps;
        }

        public override int GetScreenHeight()
        {
            return mScreenHeight;
        }

        public override int GetScreenWidth()
        {
            return mScreenWidth;
        }

        public override string GetResolutionString()
        {
            return mScreenWidth + "x" + mScreenHeight;
        }

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

            var displayWidth = mGraphicsDevice?.DisplayMode?.Width;
            var displayHeight = mGraphicsDevice?.DisplayMode?.Height;

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

            var database = Globals.Database;
            var validVideoModes = GetValidVideoModes();
            var targetResolution = database.TargetResolution;

            if (targetResolution < 0 || validVideoModes?.Count <= targetResolution)
            {
                Debug.Assert(database != null, "database != null");
                database.TargetResolution = 0;
                database.SavePreference("Resolution", database.TargetResolution.ToString());
            }

            var targetVideoMode = validVideoModes?[targetResolution];
            if (Resolution.TryParse(targetVideoMode, out var resolution))
            {
                PreferredResolution = resolution;
            }

            mGraphics.PreferredBackBufferWidth = PreferredResolution.X;
            mGraphics.PreferredBackBufferHeight = PreferredResolution.Y;

            UpdateGraphicsState(ActiveResolution.X, ActiveResolution.Y, true);

            if (mWhiteTexture == null)
            {
                CreateWhiteTexture();
            }

            mInitializing = false;
        }

        public void Init(GraphicsDevice graphicsDevice)
        {
            mGraphicsDevice = graphicsDevice;
            mBasicEffect = new BasicEffect(mGraphicsDevice);
            mBasicEffect.LightingEnabled = false;
            mBasicEffect.TextureEnabled = true;
            mSpriteBatch = new SpriteBatch(mGraphicsDevice);
        }

        public override GameFont LoadFont(string filename)
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

        public override GameShader LoadShader(string shaderName)
        {
            return new MonoShader(shaderName, mContentManager);
        }

        public override GameTexture LoadTexture(string filename, string realFilename)
        {
            var packFrame = GameTexturePacks.GetFrame(filename);
            if (packFrame != null)
            {
                var tx = new MonoTexture(mGraphicsDevice, filename, packFrame);
                mAllTextures.Add(tx);

                return tx;
            }

            var tex = new MonoTexture(mGraphicsDevice, filename, realFilename);
            mAllTextures.Add(tex);

            return tex;
        }

        /// <inheritdoc />
        public override GameTexture LoadTexture(string assetName, Func<Stream> createStream) =>
            new MonoTexture(mGraphicsDevice, assetName, createStream);

        public override Pointf MeasureText(string text, GameFont gameFont, float fontScale)
        {
            if (gameFont == null)
            {
                return Pointf.Empty;
            }

            var font = (SpriteFont) gameFont.GetFont();
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

            Matrix.CreateOrthographicOffCenter(0, view.Width, view.Height, 0, 0f, -1, out var projection);
            projection.M41 += -0.5f * projection.M11;
            projection.M42 += -0.5f * projection.M22;
            mBasicEffect.Projection = projection;
            mBasicEffect.View = Matrix.CreateRotationZ(0f) *
                                Matrix.CreateScale(new Vector3(1, 1, 1)) *
                                Matrix.CreateTranslation(-view.X, -view.Y, 0);

            return;
        }

        public override bool BeginScreenshot()
        {
            if (mGraphicsDevice == null)
            {
                return false;
            }

            mScreenshotRenderTarget = new RenderTarget2D(
                mGraphicsDevice, mScreenWidth, mScreenHeight, false,
                mGraphicsDevice.PresentationParameters.BackBufferFormat,
                mGraphicsDevice.PresentationParameters.DepthStencilFormat,
                mGraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents
            );

            return true;
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

            if (mGraphicsDevice == null)
            {
                return;
            }

            var skippedFrame = mScreenshotRenderTarget;
            mScreenshotRenderTarget = null;
            mGraphicsDevice.SetRenderTarget(null);

            if (!Begin())
            {
                return;
            }

            mSpriteBatch?.Draw(skippedFrame, new XNARectangle(), XNAColor.White);
            End();
        }

    }

}
