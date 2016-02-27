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
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Tao.OpenGl;
using Color = IntersectClientExtras.GenericClasses.Color;
using FloatRect = IntersectClientExtras.GenericClasses.FloatRect;
using IntersectClientExtras.Graphics;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics
{
    public class SfmlRenderer : GameRenderer
    {
        private int _fpsCount;
        private int _fps;
        private long _fpsTimer;
        private RenderWindow _renderWindow;
        private int _screenWidth = 0;
        private int _screenHeight = 0;

        //Rendering Variables
        private static Vertex[] _vertexCache = new Vertex[1024];
        private static int _vertexCount = 0;
        public static int DrawCalls = 0;
        private static RenderStates _renderState = new RenderStates(BlendMode.Alpha);

        //Current variables
        private static GameTexture _curTexture = null;
        private static GameRenderTexture _curTarget = null;
        private static GameBlendModes _curBlendMode = GameBlendModes.Alpha;
        private static GameShader _curShader = null;

        private static bool _initialized = false;

        public SfmlRenderer() : base()
        {
            Init();
        }


        public override void Init()
        {
            if (_initialized) return;
            if (Globals.Database.TargetResolution < 0 || Globals.Database.TargetResolution > GetValidVideoModes().Count)
            {
                Globals.Database.TargetResolution = 0;
                Globals.Database.SavePreference("Resolution", Globals.Database.TargetResolution.ToString());
            }
            uint resX = Convert.ToUInt32(GetValidVideoModes()[Globals.Database.TargetResolution].Split("x".ToCharArray())[0]);
            uint resY = Convert.ToUInt32(GetValidVideoModes()[Globals.Database.TargetResolution].Split("x".ToCharArray())[1]);
            if (Globals.Database.FullScreen)
            {
                _renderWindow = new RenderWindow(new VideoMode(resX, resY), "Intersect Client", Styles.Fullscreen);
            }
            else
            {
                _renderWindow = new RenderWindow(new VideoMode(resX, resY), "Intersect Client", Styles.Titlebar);
            }
            _screenWidth = (int)_renderWindow.Size.X;
            _screenHeight = (int)_renderWindow.Size.Y;
            _renderWindow.SetVerticalSyncEnabled(false);
            if (Globals.Database.TargetFps == 0)
            {
                _renderWindow.SetVerticalSyncEnabled(true);
            }
            else if (Globals.Database.TargetFps == 1)
            {
                _renderWindow.SetFramerateLimit(30);
            }
            else if (Globals.Database.TargetFps == 2)
            {
                _renderWindow.SetFramerateLimit(60);
            }
            else if (Globals.Database.TargetFps == 3)
            {
                _renderWindow.SetFramerateLimit(90);
            }
            else if (Globals.Database.TargetFps == 4)
            {
                _renderWindow.SetFramerateLimit(120);
            }
            else
            {
                _renderWindow.SetFramerateLimit(0);
            }

            _renderWindow.KeyPressed += ((SfmlInput)Globals.InputManager).RenderWindow_KeyPressed;
            _renderWindow.KeyReleased += ((SfmlInput)Globals.InputManager).RenderWindow_KeyReleased;
            _renderWindow.MouseButtonPressed += ((SfmlInput)Globals.InputManager).RenderWindow_MouseButtonPressed;
            _renderWindow.MouseButtonReleased += ((SfmlInput)Globals.InputManager).RenderWindow_MouseButtonReleased;
            _renderWindow.MouseMoved += ((SfmlInput)Globals.InputManager).RenderWindow_MouseMoved;
            _renderWindow.TextEntered += ((SfmlInput)Globals.InputManager).RenderWindow_TextEntered;
            _initialized = true;
        }

        public override bool Begin()
        {
            return true;
        }

        public override void End()
        {
            _fpsCount++;
            DrawCalls++;
            if (_fpsTimer < Environment.TickCount)
            {
                _fps = _fpsCount;
                _renderWindow.SetTitle("Intersect Engine - FPS: " + _fps + " Draw Calls: " + DrawCalls);
                _fpsCount = 0;
                _fpsTimer = Environment.TickCount + 1000;
            }
            RenderCurrentBatch();
            _renderWindow.Display();
            _renderWindow.DispatchEvents();
            DrawCalls = 0;
        }

        public override void Clear(Color color)
        {
            _renderWindow.Clear(new global::SFML.Graphics.Color(color.R, color.G, color.B, color.A));
        }

        public override void SetView(FloatRect view)
        {
            global::SFML.Graphics.FloatRect rect = new global::SFML.Graphics.FloatRect(view.X, view.Y, view.Width,
                view.Height);
            _renderWindow.SetView(new View(rect));
        }

        public override FloatRect GetView()
        {
            return new FloatRect(_renderWindow.GetView().Viewport.Left, _renderWindow.GetView().Viewport.Top,
                _renderWindow.GetView().Viewport.Width, _renderWindow.GetView().Viewport.Height);
        }

        public override GameFont LoadFont(string filename)
        {
            return new SfmlFont(filename);
        }

        public override void DrawTexture(GameTexture tex, FloatRect srcRectangle, FloatRect targetRect, Color renderColor,
            GameRenderTexture renderTarget = null, GameBlendModes blendMode = GameBlendModes.Alpha, GameShader shader = null, float rotationDegrees = 0.0f)
        {
            if (tex == null) return;
            Texture drawTex = (Texture)tex.GetTexture();
            var u1 = (float)srcRectangle.X / drawTex.Size.X;
            var v1 = (float)srcRectangle.Y / drawTex.Size.Y;
            var u2 = (float)srcRectangle.Right / drawTex.Size.X;
            var v2 = (float)srcRectangle.Bottom / drawTex.Size.Y;


            global::SFML.Graphics.Color drawColor = global::SFML.Graphics.Color.White;

            if (renderColor != null) drawColor = new global::SFML.Graphics.Color(renderColor.R, renderColor.G,
                renderColor.B, renderColor.A);


            u1 *= drawTex.Size.X;
            v1 *= drawTex.Size.Y;
            u2 *= drawTex.Size.X;
            v2 *= drawTex.Size.Y;

            //Check for different blend mode, different texture, different shader and different target.. if any of those have changed then render it all out now.
            BlendMode sfmlBlendMode = BlendMode.Alpha;
            switch (blendMode)
            {
                case GameBlendModes.Add:
                    sfmlBlendMode = BlendMode.Add;
                    break;

                case GameBlendModes.Alpha:
                    sfmlBlendMode = BlendMode.Alpha;
                    break;

                case GameBlendModes.Multiply:
                    sfmlBlendMode = BlendMode.Multiply;
                    break;

                case GameBlendModes.None:
                    sfmlBlendMode = BlendMode.None;
                    break;
            }

            Transform transform = Transform.Identity;
            transform.Rotate(rotationDegrees, targetRect.X + targetRect.Width/2, targetRect.Y + targetRect.Height/2);
            if (_curTexture != tex || _curBlendMode != blendMode || _curShader != _curShader ||
                _curTarget != renderTarget)
            {
                RenderCurrentBatch();
                if (shader != null)
                {
                    _renderState = new RenderStates(sfmlBlendMode, transform, null, (Shader)shader.GetShader());
                }
                else
                {
                    _renderState = new RenderStates(sfmlBlendMode, transform,null,null );
                    _renderState.Texture = drawTex;
                }
            }




            if (renderTarget == null)
            {
                if (
                !targetRect.IntersectsWith(new FloatRect(GameGraphics.CurrentView.Left, GameGraphics.CurrentView.Top, GameGraphics.CurrentView.Width,
                    GameGraphics.CurrentView.Height)))
                {
                    return;
                }


                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;

                _vertexCache[_vertexCount] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                _vertexCache[_vertexCount++].Color = drawColor;
                _vertexCache[_vertexCount] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                _vertexCache[_vertexCount++].Color = drawColor;
                _vertexCache[_vertexCount] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                _vertexCache[_vertexCount++].Color = drawColor;
                _vertexCache[_vertexCount] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
                _vertexCache[_vertexCount++].Color = drawColor;
            }
            else
            {
                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;
                var vertexCache = new Vertex[4];
                vertexCache[0] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                vertexCache[1] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                vertexCache[2] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                vertexCache[3] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
                vertexCache[0].Color = drawColor;
                vertexCache[1].Color = drawColor;
                vertexCache[2].Color = drawColor;
                vertexCache[3].Color = drawColor;
                DrawCalls++;
                ((SfmlRenderTexture)renderTarget).GetRenderTexture().Draw(vertexCache, 0, 4, PrimitiveType.Quads, _renderState);
                ((SfmlRenderTexture)renderTarget).GetRenderTexture().ResetGLStates();
            }
        }

        private void RenderCurrentBatch()
        {
            if (_vertexCount > 0)
            {
                _renderWindow.Draw(_vertexCache, 0, (uint)_vertexCount, PrimitiveType.Quads, _renderState);
                DrawCalls++;
                _renderWindow.ResetGLStates();
                _vertexCount = 0;
            }
        }

        public override int GetFps()
        {
            return _fps;
        }

        public override int GetScreenWidth()
        {
            return _screenWidth;
        }

        public override int GetScreenHeight()
        {
            return _screenHeight;
        }

        public override GameRenderTexture CreateRenderTexture(int width, int height)
        {
            return new SfmlRenderTexture(width, height);
        }

        public override GameTexture LoadTexture(string filename)
        {
            return new SfmlTexture(filename);
        }

        public override Pointf MeasureText(string text, GameFont gameFont, int fontSize)
        {
            Font sfFont = (Font) gameFont.GetFont();
            // todo: this is workaround for SFML.Net bug under mono
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                if (text[text.Length - 1] != '\0')
                    text += '\0';
            }

            Point extents = new Point(0, (int)sfFont.GetLineSpacing((uint)fontSize));
            char prev = '\0';

            for (int i = 0; i < text.Length; i++)
            {
                char cur = text[i];
                sfFont.GetKerning(prev, cur, (uint)fontSize);
                prev = cur;
                if (cur == '\n' || cur == '\v')
                    continue;
                extents.X += (int)sfFont.GetGlyph(cur, (uint)fontSize, false).Advance;
            }

            return new Pointf(extents.X,extents.Y);
            var textObject = new Text(text, (Font)gameFont.GetFont(), (uint)fontSize);
            global::SFML.Graphics.FloatRect bounds = textObject.GetLocalBounds();
            return new Pointf(bounds.Width, bounds.Height);
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, int fontSize, Color fontColor, bool worldPos = true, GameRenderTexture renderTexture = null)
        {
            var textObject = new Text(text, (Font)gameFont.GetFont(), (uint)fontSize);
            textObject.Position = new Vector2f(x, y);
            textObject.Color = new global::SFML.Graphics.Color(fontColor.R, fontColor.G, fontColor.B, fontColor.A);
            //RenderCurrentBatch();
            if (renderTexture == null)
            {
                _renderWindow.Draw(textObject);
            }
            else
            {
                ((SfmlRenderTexture)renderTexture).GetRenderTexture().Draw(textObject);
            }
        }

        public override void DrawString(string text, GameFont gameFont, float x, float y, int fontSize, Color fontColor, bool worldPos, GameRenderTexture renderTexture, FloatRect clipRect)
        {
            var textObject = new Text(text, (Font)gameFont.GetFont(), (uint)fontSize);
            textObject.Position = new Vector2f(x, y);
            textObject.Color = new global::SFML.Graphics.Color(fontColor.R, fontColor.G, fontColor.B, fontColor.A);
            //RenderCurrentBatch();
            RenderTarget target;
            if (renderTexture == null)
            {
                target = _renderWindow;
            }
            else
            {
                target = ((SfmlRenderTexture)renderTexture).GetRenderTexture();
            }


            Rectangle clip = new Rectangle((int)clipRect.X, (int)clipRect.Y, (int)clipRect.Width, (int)clipRect.Height);
            var view = target.GetView();
            var v = target.GetViewport(view);
            view.Dispose();
            clip.Y = v.Height - (clip.Y + clip.Height);
            Gl.glScissor(clip.X, clip.Y, clip.Width, clip.Height);
            Gl.glEnable(Gl.GL_SCISSOR_TEST);
            target.Draw(textObject);
            Gl.glDisable(Gl.GL_SCISSOR_TEST);

        }

        public override void Close()
        {
            _renderWindow.Close();
            _initialized = false;
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

        public override GameShader LoadShader(string shaderName)
        {
            return new SfmlShader(shaderName);
        }
    }
}
