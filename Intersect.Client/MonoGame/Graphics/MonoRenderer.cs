using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Intersect.Client.Classes.MonoGame.Graphics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.NativeInterop;
using Intersect.Client.ThirdParty;
using Intersect.Configuration;
using Intersect.Extensions;
using Intersect.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MathHelper = Intersect.Utilities.MathHelper;
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

        private bool mDisplayModeChanged;

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

        RasterizerState mRasterizerState = new RasterizerState { ScissorTestEnable = true };

        private int mScreenHeight;

        private RenderTarget2D mScreenshotRenderTarget;

        private int mScreenWidth;

        private SpriteBatch mSpriteBatch;

        private bool mSpriteBatchBegan;

        private List<string>? _validVideoModes;

        private GameTexture? mWhiteTexture;

        public MonoRenderer(GraphicsDeviceManager graphics, ContentManager contentManager, Game monoGame)
        {
            mGame = monoGame;
            mGraphics = graphics;
            mContentManager = contentManager;

            mNormalState = new BlendState
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.One,
                ColorBlendFunction = BlendFunction.Add,
                AlphaBlendFunction = BlendFunction.Add,
            };

            mMultiplyState = new BlendState
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero
            };

            mCutoutState = new BlendState
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
                    Interface.Interface.ShowError(
                        Strings.Errors.displaynotsupportederror.ToString(
                            Strings.Internals.ResolutionXByY.ToString(width, height)
                        ),
                        Strings.Errors.displaynotsupported
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
                var displayBounds = Sdl2.GetDisplayBounds();
                var currentDisplayBounds = displayBounds[0];
                mGameWindow.Position = new Microsoft.Xna.Framework.Point(
                    currentDisplayBounds.x + (currentDisplayBounds.w - mScreenWidth) / 2,
                    currentDisplayBounds.y + (currentDisplayBounds.h - mScreenHeight) / 2
                );
            }

            mOldDisplayMode = currentDisplayMode;
            if (fsChanged)
            {
                mFsChangedTimer = Timing.Global.MillisecondsUtc + 1000;
            }

            if (fsChanged)
            {
                mDisplayModeChanged = true;
            }
        }

        public void CreateWhiteTexture()
        {
            var texture = new Texture2D(mGraphicsDevice, 1, 1);
            texture.SetData(new[] { XNAColor.White });
            mWhiteTexture = MonoTexture.CreateFromTexture2D(texture, "white_1px_1px");
        }

        public override bool Begin()
        {
            if (mFsChangedTimer > -1 && mFsChangedTimer < Timing.Global.MillisecondsUtc)
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

            return RecreateSpriteBatch();
        }

        protected override bool RecreateSpriteBatch()
        {
            if (mSpriteBatchBegan)
            {
                EndSpriteBatch();
            }

            StartSpritebatch(mCurrentView, GameBlendModes.None, null, null, true, null);
            return true;
        }

        public Pointf GetMouseOffset()
        {
            return new Pointf(
                mGraphics.PreferredBackBufferWidth / (float)mGameWindow.ClientBounds.Width,
                mGraphics.PreferredBackBufferHeight / (float)mGameWindow.ClientBounds.Height
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
                    mGraphicsDevice?.SetRenderTarget((RenderTarget2D)target.GetTexture());
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
                    useEffect = (Effect)shader.GetShader();
                    shader.ResetChanged();
                }

                mSpriteBatch.Begin(
                    drawImmediate ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
                    blend,
                    SamplerState.PointClamp,
                    null,
                    rs,
                    useEffect,
                    CreateViewMatrix(view)
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

        public static XNAColor ConvertColor(Color clr)
        {
            return new XNAColor(clr.R, clr.G, clr.B, clr.A);
        }

        public override void Clear(Color color)
        {
            mGraphicsDevice.Clear(ConvertColor(color));
        }

        public override void DrawTileBuffer(GameTileBuffer buffer)
        {
            EndSpriteBatch();
            if (mGraphicsDevice == null || buffer == null)
            {
                return;
            }

            mGraphicsDevice.SetRenderTarget(mScreenshotRenderTarget);
            mGraphicsDevice.BlendState = mNormalState;
            mGraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            mGraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            mGraphicsDevice.DepthStencilState = DepthStencilState.None;
            ((MonoTileBuffer)buffer).Draw(mBasicEffect, mCurrentView);
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
            var font = (SpriteFont)gameFont?.GetFont();
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

            var font = (SpriteFont)gameFont.GetFont();
            if (font == null)
            {
                return;
            }

            var clr = ConvertColor(fontColor);

            //Copy the current scissor rect so we can restore it after
            var currentRect = mSpriteBatch.GraphicsDevice.ScissorRectangle;
            StartSpritebatch(mCurrentView, GameBlendModes.None, null, renderTexture, false, mRasterizerState, true);

            //Set the current scissor rectangle
            mSpriteBatch.GraphicsDevice.ScissorRectangle = new XNARectangle(
                (int)clipRect.X, (int)clipRect.Y, (int)clipRect.Width, (int)clipRect.Height
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
                rotationDegrees = (float)(Math.PI / 180 * rotationDegrees);
                origin = new Vector2(sw / 2f, sh / 2f);

                //TODO: Optimize in terms of memory AND performance.
                var pnt = new Pointf(0, 0);
                var pnt1 = new Pointf(tw, 0);
                var pnt2 = new Pointf(0, th);
                var cntr = new Pointf(tw / 2, th / 2);

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
            void Draw(FloatRect currentView, GameRenderTexture targetObject)
            {
                StartSpritebatch(currentView, blendMode, shader, targetObject, false, null, drawImmediate);

                mSpriteBatch.Draw((Texture2D)texture, new Vector2(tx, ty),
                    new XNARectangle((int)sx, (int)sy, (int)sw, (int)sh), color, rotationDegrees, origin,
                    new Vector2(tw / sw, th / sh), SpriteEffects.None, 0);
            }

            if (renderTarget == null)
            {
                if (isUi)
                {
                    tx += mCurrentView.X;
                    ty += mCurrentView.Y;
                }

                Draw(mCurrentView, null);
            }
            else
            {
                Draw(new FloatRect(0, 0, renderTarget.GetWidth(), renderTarget.GetHeight()), renderTarget);
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
                (float)(pnt.X + ctr.X * Math.Cos(angle) - ctr.Y * Math.Sin(angle)),
                (float)(pnt.Y + ctr.X * Math.Sin(angle) + ctr.Y * Math.Cos(angle))
            );
        }

        public override void End()
        {
            EndSpriteBatch();
            mFpsCount++;
            if (mFpsTimer < Timing.Global.MillisecondsUtc)
            {
                mFps = mFpsCount;
                mFpsCount = 0;
                mFpsTimer = Timing.Global.MillisecondsUtc + 1000;
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

        public Resolution[] GetAllowedResolutions()
        {
            var allowedResolutions = new[]
                {
                    new Resolution(800, 600), new Resolution(1024, 768), new Resolution(1024, 720),
                    new Resolution(1280, 720), new Resolution(1280, 768), new Resolution(1280, 1024),
                    new Resolution(1360, 768), new Resolution(1366, 768), new Resolution(1440, 1050),
                    new Resolution(1440, 900), new Resolution(1600, 900), new Resolution(1680, 1050),
                    new Resolution(1920, 1080),
                }.Concat(
                    mGraphicsDevice.Adapter.SupportedDisplayModes
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
                    new Resolution(mGraphicsDevice.DisplayMode.Width, mGraphicsDevice.DisplayMode.Height),
                };
            }

            return allowedResolutions;
        }

        public override List<string> GetValidVideoModes()
        {
            if (_validVideoModes != null)
            {
                return _validVideoModes;
            }

            _validVideoModes = new List<string>();

            var allowedResolutions = GetAllowedResolutions();

            var displayWidth = mGraphicsDevice.DisplayMode?.Width;
            var displayHeight = mGraphicsDevice.DisplayMode?.Height;

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

            if (database.TargetResolution < 0)
            {
                var allowedResolutions = GetAllowedResolutions();
                var currentDisplayMode = mGraphicsDevice.Adapter.CurrentDisplayMode;
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
            // Get font size from filename, format should be name_size.xnb or whatever
            var name = GameContentManager.RemoveExtension(filename)
                .Replace(Path.Combine(ClientConfiguration.ResourcesDirectory, "fonts"), "")
                .TrimStart(Path.DirectorySeparatorChar);

            // Split the name into parts
            var parts = name.Split('_');

            // Check if the font size can be extracted
            if (parts.Length < 1 || !int.TryParse(parts[parts.Length - 1], out var size))
            {
                return null;
            }

            // Concatenate the parts of the name except the last one to get the full name
            name = string.Join("_", parts.Take(parts.Length - 1));

            // Return a new MonoFont with the extracted name and size
            return new MonoFont(name, filename, size, mContentManager);
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
            var font = (SpriteFont)gameFont?.GetFont();
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

        private Matrix CreateViewMatrix(FloatRect view)
        {
            return Matrix.CreateRotationZ(0f) *
                   Matrix.CreateTranslation(-view.X, -view.Y, 0) *
                   Matrix.CreateScale(new Vector3(_scale));
        }

        public override void SetView(FloatRect view)
        {
            mCurrentView = view;

            Matrix.CreateOrthographicOffCenter(0, view.Width, view.Height, 0, 0f, -1, out var projection);
            projection.M41 += -0.5f * projection.M11;
            projection.M42 += -0.5f * projection.M22;
            mBasicEffect.Projection = projection;
            mBasicEffect.View = CreateViewMatrix(view);
        }

        public override bool BeginScreenshot()
        {
            if (mGraphicsDevice == null)
            {
                return false;
            }

            mScreenshotRenderTarget = new RenderTarget2D(
                mGraphicsDevice,
                mScreenWidth,
                mScreenHeight,
                mipMap: false,
                preferredFormat: mGraphicsDevice.PresentationParameters.BackBufferFormat,
                preferredDepthFormat: mGraphicsDevice.PresentationParameters.DepthStencilFormat,
                /* For whatever reason if this isn't zero everything breaks in .NET 7 on MacOS and most Windows devices */
                preferredMultiSampleCount: 0, // mGraphicsDevice.PresentationParameters.MultiSampleCount,
                usage: RenderTargetUsage.PreserveContents
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
