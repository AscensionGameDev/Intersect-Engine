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
using Gwen.Control;
using Gwen.Input;
using Gwen.Skin;
using SFML.Graphics;
using SFML.Window;
using Font = Gwen.Font;
using Intersect_Client.Classes.UI.Menu;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;
using System.IO;
using SFML.System;
using SFML = Gwen.Input.SFML;

namespace Intersect_Client.Classes
{
    public static class Gui
    {

        //GWEN GUI
        private static Gwen.Input.SFML _gwenInput;
        public static RenderTexture GwenTexture;
        private static Canvas _gameCanvas;
        private static Canvas _menuCanvas;
        public static Gwen.Renderer.SFML _gwenRenderer;
        private static TexturedBase _gwenSkin;
        private static Font _gwenFont;
        public static List<string> MsgboxErrors = new List<string>();
        public static bool SetupHandlers;
        public static GameGuiBase _GameGui;
        public static MenuGuiBase _MenuGui;
        public static ErrorMessageHandler ErrorMsgHandler;

        //Input Handling
        public static List<Gwen.Control.Base> FocusElements;

        #region "Gwen Setup and Input"
        //Gwen Low Level Functions
        public static void InitGwen()
        {
            GwenTexture = new RenderTexture((uint)Graphics.ScreenWidth,(uint)Graphics.ScreenHeight);
            _gwenRenderer = new Gwen.Renderer.SFML(GwenTexture);
            //TODO: Make it easier to modify skin.
            _gwenSkin = new TexturedBase(_gwenRenderer, "DefaultSkin.png");



            //TODO Move font system over from Orion
            // try to load, fallback if failed
            _gwenFont = new Font(_gwenRenderer) { Size = 10, FaceName = "Arvo-Regular.ttf" };
            if (_gwenRenderer.LoadFont(_gwenFont))
            {
                _gwenRenderer.FreeFont(_gwenFont);
            }
            else // try another
            {
                _gwenFont.FaceName = "Arial";
                if (_gwenRenderer.LoadFont(_gwenFont))
                {
                    _gwenRenderer.FreeFont(_gwenFont);
                }
                else // try default
                {
                    _gwenFont.FaceName = "OpenSans.ttf";
                }
            }

            _gwenSkin.SetDefaultFont(_gwenFont.FaceName);
            _gwenFont.Dispose(); // skin has its own


            // Create a Canvas (it's root, on which all other GWEN controls are created)
            _menuCanvas = new Canvas(_gwenSkin);
            _menuCanvas.SetSize(Graphics.ScreenWidth, Graphics.ScreenHeight);
            _menuCanvas.ShouldDrawBackground = false;
            _menuCanvas.BackgroundColor = System.Drawing.Color.FromArgb(255, 150, 170, 170);
            _menuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            _gameCanvas = new Canvas(_gwenSkin);
            _gameCanvas.SetSize(Graphics.ScreenWidth, Graphics.ScreenHeight);
            _gameCanvas.ShouldDrawBackground = false;
            _gameCanvas.BackgroundColor = System.Drawing.Color.FromArgb(255, 150, 170, 170);
            _gameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            _gwenInput = new Gwen.Input.SFML();
            if (Globals.GameState == (int)Enums.GameStates.Intro || Globals.GameState == (int)Enums.GameStates.Menu)
            {
                _gwenInput.Initialize(_menuCanvas, Graphics.RenderWindow);
            }
            else
            {
                _gwenInput.Initialize(_gameCanvas, Graphics.RenderWindow);
            }

            // Setup event handlers
            if (SetupHandlers == false)
            {
                Graphics.RenderWindow.Closed += OnClosed;
                Graphics.RenderWindow.KeyPressed += OnKeyPressed;
                Graphics.RenderWindow.KeyReleased += window_KeyReleased;
                Graphics.RenderWindow.MouseButtonPressed += window_MouseButtonPressed;
                Graphics.RenderWindow.MouseButtonReleased += window_MouseButtonReleased;
                Graphics.RenderWindow.MouseWheelMoved += window_MouseWheelMoved;
                Graphics.RenderWindow.MouseMoved += window_MouseMoved;
                Graphics.RenderWindow.TextEntered += window_TextEntered;
                SetupHandlers = true;
            }

            FocusElements = new List<Gwen.Control.Base>();
            ErrorMsgHandler = new ErrorMessageHandler(_menuCanvas, _gameCanvas);
            if (Globals.GameState == (int)Enums.GameStates.Intro || Globals.GameState == (int)Enums.GameStates.Menu)
            {
                _MenuGui = new MenuGuiBase(_menuCanvas);
            }
            else
            {
                _GameGui = new GameGuiBase(_gameCanvas);
            }
        }
        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            if (_menuCanvas == null)
            {
                return;}
            _menuCanvas.Dispose();
            _gameCanvas.Dispose();
            _gwenRenderer.Dispose();
            _gwenSkin.Dispose();
            _gwenFont.Dispose();
        }
        static void window_TextEntered(object sender, TextEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        static void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            e.X -= (int)Graphics.CurrentView.Left;
            e.Y -= (int)Graphics.CurrentView.Top;
            _gwenInput.ProcessMessage(e);
        }

        static void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            e.X -= (int)Graphics.CurrentView.Left;
            e.Y -= (int)Graphics.CurrentView.Top;
            _gwenInput.ProcessMessage(e);
        }

        static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            e.X -= (int)Graphics.CurrentView.Left;
            e.Y -= (int)Graphics.CurrentView.Top;
            _gwenInput.ProcessMessage(new SFMLMouseButtonEventArgs(e, true));
        }

        static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            e.X -= (int)Graphics.CurrentView.Left;
            e.Y -= (int)Graphics.CurrentView.Top;
            _gwenInput.ProcessMessage(new SFMLMouseButtonEventArgs(e, false));
        }
        static void window_KeyReleased(object sender, KeyEventArgs e) { _gwenInput.ProcessMessage(new SFMLKeyEventArgs(e, false)); }
        static void OnClosed(object sender, EventArgs e) { GameMain.IsRunning = false; }
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                GameMain.IsRunning = false;
            }
            else if (e.Code == Keyboard.Key.Insert)
            {
                //Try to open admin panel!
                if (Globals.GameState == (int)Enums.GameStates.InGame)
                {
                    PacketSender.SendOpenAdminWindow();
                }
            }
            else if (e.Code == Keyboard.Key.F1)
            {
                _GameGui.ShowHideDebug();
            }
            else
            {
                _gwenInput.ProcessMessage(new SFMLKeyEventArgs(e, true));

            }
        }
        public static bool HasInputFocus()
        {
            for (var i = 0; i < FocusElements.Count; i++)
            {
                if (FocusElements[i].HasFocus)
                {
                    return true;
                }
            }
            return false;
        }

        public static void OpenAdminWindow()
        {
            AdminWindow adminWindow = new AdminWindow(_gameCanvas);
        }
        #endregion

        #region "GUI Functions"
        //Actual Drawing Function
        public static void DrawGui()
        {
            ErrorMsgHandler.Update();
            GwenTexture.Clear(Color.Transparent);
            if (Globals.GameState == (int)Enums.GameStates.Menu)
            {
                _MenuGui.Draw();
            }
            else if (Globals.GameState == (int)Enums.GameStates.InGame)
            {
                _GameGui.Draw();
            }
            GwenTexture.Display();
        }
        public static Gwen.Texture SFMLToGwenTexture(Texture sftex)
        {
            Gwen.Texture tex = new Gwen.Texture(_gwenRenderer);
            _gwenRenderer.LoadSFMLTexture(tex, sftex);
            return tex;
        }
        public static RenderTexture CreateTextureFromSprite(string spritename, int w, int h)
        {
            RenderTexture rt = new RenderTexture((uint)w, (uint)h);
            if (Graphics.EntityFileNames.Contains(spritename))
            {
                Sprite enSprite = new Sprite(Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(spritename)]);
                enSprite.TextureRect = new IntRect(0, 0,
                    (int) Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(spritename)].Size.X/4,
                    (int) Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(spritename)].Size.Y/4);
                enSprite.Scale =
                    new Vector2f(w/(Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(spritename)].Size.X/4f),
                        h/(Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(spritename)].Size.Y/4f));
                rt.Draw((enSprite));
            }
            rt.Display();
            return rt;
        }

        public static Texture CreateItemTex(int itemnum, int xOffset = 0, int yOffset = 0, int width = 32, int height = 32, bool isEquipped = false, Texture bg = null)
        {
            RenderTexture rt = new RenderTexture((uint)width, (uint)height);
            Sprite sprite;
            if (bg != null)
            {
                sprite = new Sprite(bg);
                rt.Draw(sprite);
            }
            if (itemnum > -1)
            {
                if (Graphics.ItemFileNames.Contains(Globals.GameItems[itemnum].Pic))
                {
                    sprite =
                        new Sprite(Graphics.ItemTextures[Graphics.ItemFileNames.IndexOf(Globals.GameItems[itemnum].Pic)]);
                    sprite.Position = new Vector2f(xOffset, yOffset);
                    rt.Draw(sprite);
                }
            }
            if (isEquipped)
            {
                CircleShape equipDot = new CircleShape();
                equipDot.FillColor = Color.Red;
                equipDot.Position = new Vector2f(26 + xOffset, 0 + yOffset);
                equipDot.Radius = 5;
                rt.Draw(equipDot);
            }
            rt.Display();
            return rt.Texture;
        }
        public static Texture CreateSpellTex(int spellnum, int xOffset = 0, int yOffset = 0, int width = 32, int height = 32, bool onCD = false, Texture bg = null)
        {
            RenderTexture rt = new RenderTexture((uint)width, (uint)height);
            Sprite sprite;
            if (bg != null)
            {
                sprite = new Sprite(bg);
                rt.Draw(sprite);
            }
            if (spellnum > -1)
            {
                if (Graphics.SpellFileNames.Contains(Globals.GameSpells[spellnum].Pic))
                {
                    sprite =
                        new Sprite(Graphics.SpellTextures[Graphics.SpellFileNames.IndexOf(Globals.GameSpells[spellnum].Pic)]);
                    sprite.Position = new Vector2f(xOffset, yOffset);
                    
                    if (onCD)
                    {
                        sprite.Color = new Color(255,255,255,100);
                    }
                    rt.Draw(sprite);
                }
            }
            rt.Display();
            return rt.Texture;
        }
        //Code courtousy of http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
        public static System.Drawing.Bitmap MakeGrayscale3(System.Drawing.Bitmap original)
        {
            //create a blank bitmap the same size as original
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
               new float[][] 
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });

            //create some image attributes
            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, System.Drawing.GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
        public static bool MouseHitGUI()
        {
            for (int i = 0; i < _gameCanvas.Children.Count; i++)
            {
                if (MouseHitBase(_gameCanvas.Children[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool MouseHitBase(Gwen.Control.Base obj)
        {
            if (obj.IsHidden == true)
            {
                return false;
            }
            else
            {
                System.Drawing.RectangleF rect = new System.Drawing.RectangleF(obj.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X, obj.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y, obj.Width, obj.Height);
                if (rect.Contains(Gwen.Input.InputHandler.MousePosition.X, Gwen.Input.InputHandler.MousePosition.Y))
                {
                    return true;
                }

            }
            return false;
        }
        public static string[] WrapText(string input, int width)
        {
            var myOutput = new List<string>();
            var lastSpace = 0;
            var curPos = 0;
            var curLen = 1;
            var myText = new Text(input.Substring(curPos, curLen), Graphics.GameFont);
            myText.CharacterSize = 10;
            while (curPos + curLen < input.Length)
            {
                if (myText.GetLocalBounds().Width < width)
                {
                    if (input[curPos + curLen] == ' ' || input[curPos + curLen] == '-')
                    {
                        lastSpace = curLen;
                    }
                    else if (input[curPos + curLen] == '\n')
                    {
                        myOutput.Add(input.Substring(curPos, curLen));
                        curPos = curPos + curLen;
                        curLen = 1;
                    }
                }
                else
                {
                    myOutput.Add(input.Substring(curPos, lastSpace));
                    curPos = curPos + lastSpace;
                    curLen = 1;
                }
                curLen++;
                myText = new Text(input.Substring(curPos, curLen), Graphics.GameFont);
                myText.CharacterSize = 10;
            }
            myOutput.Add(input.Substring(curPos, input.Length - curPos));
            return myOutput.ToArray();
        }
        #endregion

    }
}
