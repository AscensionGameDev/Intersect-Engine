using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Intersect.Client.Interface;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoRenderer : GameRenderer
    {
        private ContentManager _contentManager;
        private GameBlendModes _currentBlendmode = GameBlendModes.Alpha;
        private GameShader _currentShader;
        private FloatRect _currentSpriteView;
        private GameRenderTexture _currentTarget;
        private FloatRect mCurrentView;
        private int _fps;
        private int _fpsCount;
        private long _fpsTimer;
        private Game _game;
        private GameWindow _gameWindow;
        private GraphicsDeviceManager mGraphics;
        private GraphicsDevice mGraphicsDevice;
        private bool _initialized;
        private BlendState _multiplyState;
        private BlendState _cutoutState;
        RasterizerState _rasterizerState = new RasterizerState() {ScissorTestEnable = true};
        private int mScreenHeight;
        private int mScreenWidth;
        private int mDisplayWidth;
        private int mDisplayHeight;

        private SpriteBatch mSpriteBatch;
        private bool _spriteBatchBegan;
        private List<string> mValidVideoModes;
        private GameRenderTexture mWhiteTexture;
        private List<MonoTexture> AllTextures = new List<MonoTexture>();
        
        private bool mInitializing;

        public MonoRenderer(GraphicsDeviceManager graphics, ContentManager contentManager, Game monoGame)
        {
            _game = monoGame;
            mGraphics = graphics;
            _contentManager = contentManager;

            _multiplyState = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero
            };

            _cutoutState = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.Zero,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaBlendFunction = BlendFunction.Add,
                AlphaSourceBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };

            _gameWindow = monoGame.Window;
        }

        public void UpdateGraphicsState(int width, int height)
        {
            var currentDisplayMode = mGraphics.GraphicsDevice.Adapter.CurrentDisplayMode;
            mDisplayWidth = currentDisplayMode.Width;
            mDisplayHeight = currentDisplayMode.Height;
            mScreenWidth = width;
            mScreenHeight = height;
            mGraphics.PreferredBackBufferWidth = width;
            mGraphics.PreferredBackBufferHeight = height;
            mGraphics.IsFullScreen = Globals.Database.FullScreen;
            mGraphics.SynchronizeWithVerticalRetrace = (Globals.Database.TargetFps == 0);
            mGraphics.ApplyChanges();

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
            else
            {
                _game.IsFixedTimeStep = false;
            }

            _gameWindow.Position = new Microsoft.Xna.Framework.Point((mDisplayWidth - mScreenWidth) / 2, (mDisplayHeight - mScreenHeight) / 2);
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
            if (_gameWindow.ClientBounds.Width != 0 && _gameWindow.ClientBounds.Height != 0 &&
                (_gameWindow.ClientBounds.Width != mScreenWidth || _gameWindow.ClientBounds.Height != mScreenHeight) &&
                !mGraphics.IsFullScreen)
            {
                UpdateGraphicsState(mScreenWidth, mScreenHeight);
            }
            StartSpritebatch(mCurrentView, GameBlendModes.Alpha, null, null, true, null);
            return true;
        }

        public Pointf GetMouseOffset()
        {
            return new Pointf(mGraphics.PreferredBackBufferWidth / (float) _gameWindow.ClientBounds.Width,
                mGraphics.PreferredBackBufferHeight / (float) _gameWindow.ClientBounds.Height);
        }

        private void StartSpritebatch(FloatRect view, GameBlendModes mode = GameBlendModes.None,
            GameShader shader = null, GameRenderTexture target = null, bool forced = false, RasterizerState rs = null)
        {
            bool viewsDiff = view.X != _currentSpriteView.X || view.Y != _currentSpriteView.Y ||
                             view.Width != _currentSpriteView.Width || view.Height != _currentSpriteView.Height;
            if (mode != _currentBlendmode || shader != _currentShader || target != _currentTarget || viewsDiff || forced ||
                !_spriteBatchBegan)
            {
                if (_spriteBatchBegan) mSpriteBatch.End();
                if (target == null)
                {
                    mGraphicsDevice.SetRenderTarget(null);
                }
                else
                {
                    mGraphicsDevice.SetRenderTarget((RenderTarget2D) target.GetTexture());
                }
                BlendState blend = BlendState.AlphaBlend;
                Effect useEffect = null;

                switch (mode)
                {
                    case GameBlendModes.None:
                        blend = BlendState.NonPremultiplied;
                        break;
                    case GameBlendModes.Alpha:
                        blend = BlendState.NonPremultiplied;
                        break;
                    case (GameBlendModes.Multiply):
                        blend = _multiplyState;
                        break;
                    case (GameBlendModes.Add):
                        blend = BlendState.Additive;
                        break;
                    case (GameBlendModes.Opaque):
                        blend = BlendState.Opaque;
                        break;
                    case GameBlendModes.Cutout:
                        blend = _cutoutState;
                        break;
                }

                if (shader != null)
                {
                    useEffect = (Effect) shader.GetShader();
                }
                mSpriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp, null, rs, useEffect,
                    Matrix.CreateRotationZ(0f) * Matrix.CreateScale(new Vector3(1, 1, 1)) *
                    Matrix.CreateTranslation(-view.X, -view.Y, 0));
                _currentSpriteView = view;
                _currentBlendmode = mode;
                _currentShader = shader;
                _currentTarget = target;
                _spriteBatchBegan = true;
            }
        }

        private void EndSpriteBatch()
        {
            if (_spriteBatchBegan)
            {
                mSpriteBatch.End();
            }

            _spriteBatchBegan = false;
        }

        public static Microsoft.Xna.Framework.Color ConvertColor(Color clr)
        {
            return new Microsoft.Xna.Framework.Color(new Vector4(clr.R / 255f, clr.G / 255f, clr.B / 255f, clr.A / 255f));
        }

        public override void Clear(Color color)
        {
            mGraphicsDevice.Clear(ConvertColor(color));
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
            return _contentManager;
        }

        public override GameRenderTexture CreateRenderTexture(int width, int height)
        {
            return new MonoRenderTexture(mGraphicsDevice, width, height);
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, float fontScale,
            Color fontColor, bool worldPos = true, GameRenderTexture renderTexture = null, bool outline = true)
        {
            if (gameFont == null) return;
            SpriteFont font = (SpriteFont) gameFont.GetFont();
            if (font == null) return;
            StartSpritebatch(mCurrentView, GameBlendModes.None, null, renderTexture, false, null);
            Color backColor = Color.Black;
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }
            if (outline)
            {
                mSpriteBatch.DrawString(font, text, new Vector2(x, y - 1), ConvertColor(backColor) * .8f, 0f,
                    Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                mSpriteBatch.DrawString(font, text, new Vector2(x - 1, y), ConvertColor(backColor) * .8f, 0f,
                    Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                mSpriteBatch.DrawString(font, text, new Vector2(x + 1, y), ConvertColor(backColor) * .8f, 0f,
                    Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                mSpriteBatch.DrawString(font, text, new Vector2(x, y + 1), ConvertColor(backColor) * .8f, 0f,
                    Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
            }
            mSpriteBatch.DrawString(font, text, new Vector2(x, y), ConvertColor(fontColor));
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, float fontScale,
            Color fontColor, bool worldPos, GameRenderTexture renderTexture, FloatRect clipRect)
        {
            if (gameFont == null) return;
            x += mCurrentView.X;
            y += mCurrentView.Y;
            //clipRect.X += _currentView.X;
            //clipRect.Y += _currentView.Y;
            SpriteFont font = (SpriteFont) gameFont.GetFont();
            if (font == null) return;
            Microsoft.Xna.Framework.Color clr = ConvertColor(fontColor);

            //Copy the current scissor rect so we can restore it after
            Microsoft.Xna.Framework.Rectangle currentRect = mSpriteBatch.GraphicsDevice.ScissorRectangle;
            EndSpriteBatch();
            //Set the current scissor rectangle
            mSpriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int) clipRect.X,
                (int) clipRect.Y, (int) clipRect.Width, (int) clipRect.Height);
            StartSpritebatch(mCurrentView, GameBlendModes.None, null, renderTexture, false, _rasterizerState);
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }
            mSpriteBatch.DrawString(font, text, new Vector2(x, y), clr, 0f, Vector2.Zero,
                new Vector2(fontScale, fontScale), SpriteEffects.None, 0);

            EndSpriteBatch();

            //Reset scissor rectangle to the saved value
            mSpriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
        }

        public override void DrawTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect,
            Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha,
            GameShader shader = null, float rotationDegrees = 0, bool isUi = false)
        {
            if (tex == null || tex.GetTexture() == null) return;
            Vector2 origin = Vector2.Zero;
            if (rotationDegrees != 0f)
            {
                rotationDegrees = (float) ((Math.PI / 180) * rotationDegrees);
                origin = new Vector2(srcRectangle.Width / 2, srcRectangle.Height / 2);
                targetRect.X += srcRectangle.Width / 2;
                targetRect.Y += srcRectangle.Height / 2;
            }
            if (renderTarget == null)
            {
                if (isUi)
                {
                    targetRect.X += mCurrentView.X;
                    targetRect.Y += mCurrentView.Y;
                }
                StartSpritebatch(mCurrentView, blendMode, shader, null, false, null);
                mSpriteBatch.Draw((Texture2D) tex.GetTexture(), null,
                    new Microsoft.Xna.Framework.Rectangle((int) targetRect.X, (int) targetRect.Y, (int) targetRect.Width,
                        (int) targetRect.Height),
                    new Microsoft.Xna.Framework.Rectangle((int) srcRectangle.X, (int) srcRectangle.Y,
                        (int) srcRectangle.Width, (int) srcRectangle.Height),
                    origin, rotationDegrees, null, ConvertColor(renderColor), SpriteEffects.None, 0);
            }
            else
            {
                StartSpritebatch(new FloatRect(0, 0, renderTarget.GetWidth(), renderTarget.GetHeight()), blendMode,
                    shader, renderTarget, false, null);
                mSpriteBatch.Draw((Texture2D) tex.GetTexture(), null,
                    new Microsoft.Xna.Framework.Rectangle((int) targetRect.X, (int) targetRect.Y, (int) targetRect.Width,
                        (int) targetRect.Height),
                    new Microsoft.Xna.Framework.Rectangle((int) srcRectangle.X, (int) srcRectangle.Y,
                        (int) srcRectangle.Width, (int) srcRectangle.Height),
                    origin, rotationDegrees, null, ConvertColor(renderColor), SpriteEffects.None, 0);
            }
        }

        public override void End()
        {
            EndSpriteBatch();
            _fpsCount++;
            if (_fpsTimer < Globals.System.GetTimeMS())
            {
                _fps = _fpsCount;
                _fpsCount = 0;
                _fpsTimer = Globals.System.GetTimeMS() + 1000;
                _gameWindow.Title = Strings.Get("main", "gamename");
            }
            for (int i = 0; i < AllTextures.Count; i++)
            {
                AllTextures[i].Update();
            }
        }

        public override int GetFps()
        {
            return _fps;
        }

        public override int GetScreenHeight()
        {
            return mScreenHeight;
        }

        public override int GetScreenWidth()
        {
            return mScreenWidth;
        }

        public override List<string> GetValidVideoModes()
        {
            if (mValidVideoModes != null) return mValidVideoModes;
            mValidVideoModes = new List<string>();

            var allowedResolutions = new[] {
                new Resolution(800, 600),
                new Resolution(1024, 768),
                new Resolution(1024, 720),
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
                if (resolution.X > displayWidth) continue;
                if (resolution.Y > displayHeight) continue;
                mValidVideoModes.Add(resolution.ToString());
            }

            return mValidVideoModes;
        }

        public IList<string> ValidVideoModes => GetValidVideoModes();

        public override FloatRect GetView()
        {
            return mCurrentView;
        }

        public override void Init()
        {
            if (mInitializing) return;
            mInitializing = true;

            var database = Globals.Database;
            var validVideoModes = GetValidVideoModes();
            var targetResolution = database?.TargetResolution ?? 0;

            if (targetResolution < 0 || validVideoModes?.Count <= targetResolution)
            {
                Debug.Assert(database != null, "database != null");
                database.TargetResolution = 0;
                database.SavePreference("Resolution", database.TargetResolution.ToString());
            }

            var targetVideoMode = validVideoModes?[targetResolution];
            var resolution = Resolution.Parse(targetVideoMode);
            mGraphics.PreferredBackBufferWidth = resolution.X;
            mGraphics.PreferredBackBufferHeight = resolution.Y;

            UpdateGraphicsState(mGraphics?.PreferredBackBufferWidth ?? 800, mGraphics?.PreferredBackBufferHeight ?? 600);

            if (mWhiteTexture == null) CreateWhiteTexture();

            mInitializing = false;
        }

        public void Init(GraphicsDevice graphicsDevice)
        {
            mGraphicsDevice = graphicsDevice;
            mSpriteBatch = new SpriteBatch(mGraphicsDevice);
        }

        public override GameFont LoadFont(string filename)
        {
            //Get font size from filename, format should be name_size.xnb or whatever
            string name =
                GameContentManager.RemoveExtension(filename)
                    .Replace(Path.Combine("resources", "fonts"), "")
                    .TrimStart(Path.DirectorySeparatorChar);
            string[] parts = name.Split('_');
            if (parts.Length >= 1)
            {
                if (int.TryParse(parts[parts.Length - 1], out int size))
                {
                    name = "";
                    for (int i = 0; i <= parts.Length - 2; i++)
                    {
                        name += parts[i];
                        if (i + 1 < parts.Length - 2) name += "_";
                    }
                    return new MonoFont(name, filename, size, _contentManager);
                }
            }
            return null;
        }

        public override GameShader LoadShader(string shaderName)
        {
            return new MonoShader(shaderName, _contentManager);
        }

        public override GameTexture LoadTexture(string filename)
        {
            MonoTexture tex = new MonoTexture(mGraphicsDevice, filename);
            AllTextures.Add(tex);
            return tex;
        }

        public override Pointf MeasureText(string text, GameFont gameFont, float fontScale)
        {
            if (gameFont == null) return Pointf.Empty;
            SpriteFont font = (SpriteFont) gameFont.GetFont();
            if (font == null) return Pointf.Empty;
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }
            Vector2 size = font.MeasureString(text);
            return new Pointf(size.X * fontScale, size.Y * fontScale);
        }

        public override void SetView(FloatRect view)
        {
            mCurrentView = view;
            return;
        }
    }
}