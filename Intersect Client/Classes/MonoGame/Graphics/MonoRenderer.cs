/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoRenderer : GameRenderer
    {
        private Game _game;
        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private FloatRect _currentView;
        private ContentManager _contentManager;
        private bool _initialized;
        private int _fps;
        private int _fpsCount;
        private long _fpsTimer;
        private BlendState _multiplyState;
        private GameWindow _gameWindow;
        private int _screenWidth;
        private int _screenHeight;
        private GameRenderTexture _whiteTex;

        private SpriteBatch _spriteBatch;
        private bool _spriteBatchBegan;
        private GameBlendModes _currentBlendmode = GameBlendModes.Alpha;
        private GameShader _currentShader = null;
        private GameRenderTexture _currentTarget = null;
        private FloatRect _currentSpriteView;
        RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
        private List<MonoTexture> AllTextures = new List<MonoTexture>();

        private int centerScreenX = 0;
        private int centerScreenY = 0;


        public MonoRenderer(GraphicsDeviceManager graphics, ContentManager contentManager, Game monoGame)
        {
            _game = monoGame;
            _graphics = graphics;
            _contentManager = contentManager;

            _multiplyState = new BlendState();
            _multiplyState.ColorBlendFunction = BlendFunction.Add;
            _multiplyState.ColorSourceBlend = Blend.DestinationColor;
            _multiplyState.ColorDestinationBlend = Blend.Zero;

            _gameWindow = monoGame.Window;
            centerScreenX = _gameWindow.ClientBounds.Center.X;
            centerScreenY = _gameWindow.ClientBounds.Center.Y;
        }

        public void UpdateGraphicsState(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.IsFullScreen = Globals.Database.FullScreen;
            _graphics.SynchronizeWithVerticalRetrace = (Globals.Database.TargetFps == 0);
            _graphics.ApplyChanges();

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

            _gameWindow.Position = new Microsoft.Xna.Framework.Point(centerScreenX - _screenWidth / 2, centerScreenY - _screenHeight / 2);
        }

        public void CreateWhiteTexture()
        {
            _whiteTex = CreateRenderTexture(1, 1);
            _whiteTex.Begin();
            _whiteTex.Clear(Color.White);
            _whiteTex.End();
        }

        public override bool Begin()
        {
            if (_gameWindow.ClientBounds.Width != 0 && _gameWindow.ClientBounds.Height != 0 && (_gameWindow.ClientBounds.Width != _screenWidth || _gameWindow.ClientBounds.Height != _screenHeight) && !_graphics.IsFullScreen)
            {
                UpdateGraphicsState(_screenWidth, _screenHeight);
            }
            StartSpritebatch(_currentView, GameBlendModes.Alpha, null, null, true, null);
            return true;
        }

        public Pointf GetMouseOffset()
        {
            return new Pointf(_graphics.PreferredBackBufferWidth / (float)_gameWindow.ClientBounds.Width, _graphics.PreferredBackBufferHeight / (float)_gameWindow.ClientBounds.Height);
        }

        private void StartSpritebatch(FloatRect view, GameBlendModes mode = GameBlendModes.None, GameShader shader = null, GameRenderTexture target = null, bool forced = false, RasterizerState rs = null)
        {
            bool viewsDiff = false;
            if (view.X != _currentSpriteView.X || view.Y != _currentSpriteView.Y ||
                view.Width != _currentSpriteView.Width || view.Height != _currentSpriteView.Height) viewsDiff = true;
            if (mode != _currentBlendmode || shader != _currentShader || target != _currentTarget || viewsDiff || forced || !_spriteBatchBegan)
            {
                if (_spriteBatchBegan) _spriteBatch.End();
                if (target == null)
                {
                    _graphicsDevice.SetRenderTarget(null);
                }
                else
                {
                    _graphicsDevice.SetRenderTarget((RenderTarget2D)target.GetTexture());
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
                }

                if (shader != null)
                {
                    useEffect = (Effect)shader.GetShader();
                }
                _spriteBatch.Begin(SpriteSortMode.Immediate, blend, null, null, rs, useEffect, Matrix.CreateRotationZ(0f) * Matrix.CreateScale(new Vector3(1, 1, 1)) * Matrix.CreateTranslation(-view.X, -view.Y, 0));
                _currentSpriteView = view;
                _currentBlendmode = mode;
                _currentShader = shader;
                _currentTarget = target;
                _spriteBatchBegan = true;
            }
        }

        private void EndSpriteBatch()
        {
            _spriteBatch.End();
            _spriteBatchBegan = false;
        }

        public static Microsoft.Xna.Framework.Color ConvertColor(Color clr)
        {
            return new Microsoft.Xna.Framework.Color(new Vector4(clr.R / 255f, clr.G / 255f, clr.B / 255f, clr.A / 255f));
        }

        public override void Clear(Color color)
        {
            _graphicsDevice.Clear(ConvertColor(color));
        }

        public override void Close()
        {

        }

        public override GameTexture GetWhiteTexture()
        {
            return _whiteTex;
        }

        public ContentManager GetContentManager()
        {
            return _contentManager;
        }

        public override GameRenderTexture CreateRenderTexture(int width, int height)
        {
            return new MonoRenderTexture(_graphicsDevice, width, height);
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos = true, GameRenderTexture renderTexture = null, bool outline = true)
        {
            if (gameFont == null) return;
            SpriteFont font = (SpriteFont)gameFont.GetFont();
            if (font == null) return;
            StartSpritebatch(_currentView, GameBlendModes.None, null, renderTexture, false, null);
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
                _spriteBatch.DrawString(font, text, new Vector2(x, y - 1), ConvertColor(backColor)*.8f, 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, text, new Vector2(x - 1, y), ConvertColor(backColor)*.8f, 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, text, new Vector2(x + 1, y), ConvertColor(backColor)*.8f, 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
                _spriteBatch.DrawString(font, text, new Vector2(x, y + 1), ConvertColor(backColor)*.8f, 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);
            }
            _spriteBatch.DrawString(font, text, new Vector2(x, y), ConvertColor(fontColor));
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, float fontScale, Color fontColor, bool worldPos, GameRenderTexture renderTexture, FloatRect clipRect)
        {
            if (gameFont == null) return;
            x += _currentView.X;
            y += _currentView.Y;
            //clipRect.X += _currentView.X;
            //clipRect.Y += _currentView.Y;
            SpriteFont font = (SpriteFont)gameFont.GetFont();
            if (font == null) return;
            Microsoft.Xna.Framework.Color clr = ConvertColor(fontColor);

            //Copy the current scissor rect so we can restore it after
            Microsoft.Xna.Framework.Rectangle currentRect = _spriteBatch.GraphicsDevice.ScissorRectangle;
            EndSpriteBatch();
            //Set the current scissor rectangle
            _spriteBatch.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Width, (int)clipRect.Height);
            StartSpritebatch(_currentView, GameBlendModes.None, null, renderTexture, false, _rasterizerState);
            foreach (var chr in text)
            {
                if (!font.Characters.Contains(chr))
                {
                    text = text.Replace(chr, ' ');
                }
            }
            _spriteBatch.DrawString(font, text, new Vector2(x, y), clr, 0f, Vector2.Zero,
                    new Vector2(fontScale, fontScale), SpriteEffects.None, 0);

            EndSpriteBatch();

            //Reset scissor rectangle to the saved value
            _spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
        }

        public override void DrawTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect, Color renderColor, GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0, bool isUi = false)
        {
            if (tex == null || tex.GetTexture() == null) return;
            Vector2 origin = Vector2.Zero;
            if (rotationDegrees != 0f)
            {
                rotationDegrees = (float)((Math.PI / 180) * rotationDegrees);
                origin = new Vector2(srcRectangle.Width / 2, srcRectangle.Height / 2);
                targetRect.X += srcRectangle.Width / 2;
                targetRect.Y += srcRectangle.Height / 2;
            }
            if (renderTarget == null)
            {
                if (isUi)
                {
                    targetRect.X += _currentView.X;
                    targetRect.Y += _currentView.Y;
                }
                StartSpritebatch(_currentView, blendMode, shader, null, false, null);
                _spriteBatch.Draw((Texture2D)tex.GetTexture(), null,
                    new Microsoft.Xna.Framework.Rectangle((int)targetRect.X, (int)targetRect.Y, (int)targetRect.Width,
                        (int)targetRect.Height),
                    new Microsoft.Xna.Framework.Rectangle((int)srcRectangle.X, (int)srcRectangle.Y,
                        (int)srcRectangle.Width, (int)srcRectangle.Height),
                    origin, rotationDegrees, null, ConvertColor(renderColor), SpriteEffects.None, 0);
            }
            else
            {
                StartSpritebatch(new FloatRect(0, 0, renderTarget.GetWidth(), renderTarget.GetHeight()), blendMode, shader, renderTarget, false, null);
                _spriteBatch.Draw((Texture2D)tex.GetTexture(), null,
                        new Microsoft.Xna.Framework.Rectangle((int)targetRect.X, (int)targetRect.Y, (int)targetRect.Width,
                            (int)targetRect.Height),
                        new Microsoft.Xna.Framework.Rectangle((int)srcRectangle.X, (int)srcRectangle.Y,
                            (int)srcRectangle.Width, (int)srcRectangle.Height),
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
                _gameWindow.Title = "Intersect Client";
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
            return _screenHeight;
        }

        public override int GetScreenWidth()
        {
            return _screenWidth;
        }

        public override List<string> GetValidVideoModes()
        {
            var myList = new List<string>();
            myList.Add("800x600");
            myList.Add("1024x768");
            myList.Add("1280x720");
            myList.Add("1280x768");
            myList.Add("1280x1024");
            myList.Add("1360x768");
            myList.Add("1366x768");
            myList.Add("1400x1050");
            myList.Add("1440x900");
            myList.Add("1600x900");
            myList.Add("1680x1050");
            myList.Add("1920x1080");
            return myList;
        }

        public override FloatRect GetView()
        {
            return _currentView;
        }

        public override void Init()
        {
            if (Globals.Database.TargetResolution < 0 || Globals.Database.TargetResolution > GetValidVideoModes().Count)
            {
                Globals.Database.TargetResolution = 0;
                Globals.Database.SavePreference("Resolution", Globals.Database.TargetResolution.ToString());
            }
            int resX = Convert.ToInt32(GetValidVideoModes()[Globals.Database.TargetResolution].Split("x".ToCharArray())[0]);
            int resY = Convert.ToInt32(GetValidVideoModes()[Globals.Database.TargetResolution].Split("x".ToCharArray())[1]);
            UpdateGraphicsState(resX, resY);
            if (_whiteTex == null) CreateWhiteTexture();
        }

        public void Init(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public override GameFont LoadFont(string filename)
        {
            //Get font size from filename, format should be name_size.xnb or whatever
            string name = GameContentManager.RemoveExtension(filename).Replace(Path.Combine("resources","fonts"), "").TrimStart(Path.DirectorySeparatorChar);
            string[] parts = name.Split('_');
            if (parts.Length >= 1)
            {
                int size = 0;
                if (Int32.TryParse(parts[parts.Length - 1], out size))
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
            MonoTexture tex = new MonoTexture(_graphicsDevice, filename);
            AllTextures.Add(tex);
            return tex;
        }

        public override Pointf MeasureText(string text, GameFont gameFont, float fontScale)
        {
            if (gameFont == null) return Pointf.Empty;
            SpriteFont font = (SpriteFont)gameFont.GetFont();
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
            _currentView = view;
            return;
        }
    }
}
