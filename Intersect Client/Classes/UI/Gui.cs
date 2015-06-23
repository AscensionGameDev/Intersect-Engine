using System;
using System.Collections.Generic;
using Gwen.Control;
using Gwen.Input;
using Gwen.Skin;
using SFML.Graphics;
using SFML.Window;
using Color = System.Drawing.Color;
using Font = Gwen.Font;
using Intersect_Client.Classes.UI.Menu;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;
using System.IO;

namespace Intersect_Client.Classes
{
    public static class Gui
    {

        //GWEN GUI
        private static Gwen.Input.SFML _gwenInput;
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

            _gwenRenderer = new Gwen.Renderer.SFML(Graphics.RenderWindow);
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
            _menuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _menuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            _gameCanvas = new Canvas(_gwenSkin);
            _gameCanvas.SetSize(Graphics.ScreenWidth, Graphics.ScreenHeight);
            _gameCanvas.ShouldDrawBackground = false;
            _gameCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _gameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            _gwenInput = new Gwen.Input.SFML();
            if (Globals.GameState == 0)
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
            if (Globals.GameState == 0)
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
        static void window_MouseMoved(object sender, MouseMoveEventArgs e) { _gwenInput.ProcessMessage(e); }
        static void window_MouseWheelMoved(object sender, MouseWheelEventArgs e) { _gwenInput.ProcessMessage(e); }
        static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e) { _gwenInput.ProcessMessage(new SFMLMouseButtonEventArgs(e, true)); }
        static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e) { _gwenInput.ProcessMessage(new SFMLMouseButtonEventArgs(e, false)); }
        static void window_KeyReleased(object sender, KeyEventArgs e) { _gwenInput.ProcessMessage(new SFMLKeyEventArgs(e, false)); }
        static void OnClosed(object sender, EventArgs e) { GameMain.IsRunning = false; }
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                GameMain.IsRunning = false;
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
        #endregion

        #region "GUI Functions"
        //Actual Drawing Function
        public static void DrawGui()
        {
            ErrorMsgHandler.Update();
            if (Globals.GameState == 0)
            {
                _MenuGui.Draw();
            }
            else
            {
                _GameGui.Draw();
            }
        }
        public static Gwen.Texture BitmapToGwenTexture(System.Drawing.Bitmap bmp)
        {
            Stream s = new MemoryStream();
            Gwen.Texture tex;
            bmp.Save(s, System.Drawing.Imaging.ImageFormat.Png);
            tex = new Gwen.Texture(_gwenRenderer);
            Gui._gwenRenderer.LoadTextureStream(tex, s);
            s.Dispose();
            return tex;
        }
        public static Gwen.Texture CreateTextureFromSprite(string spritename, int w, int h)
        {
            System.Drawing.Bitmap sprite = new System.Drawing.Bitmap("Resources/Entities/" + spritename + ".png");
            System.Drawing.Bitmap spriteImg = new System.Drawing.Bitmap(w, h);
            spriteImg = new System.Drawing.Bitmap(sprite.Width / 4, sprite.Height / 4);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(spriteImg);
            g.DrawImage(sprite, new System.Drawing.Rectangle(0, 0, sprite.Width / 4, sprite.Height / 4), new System.Drawing.Rectangle(0, 0, sprite.Width / 4, sprite.Height / 4), System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();
            sprite.Dispose();
            return BitmapToGwenTexture(spriteImg);
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
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(obj.LocalPosToCanvas(new System.Drawing.Point(0, 0)).X, obj.LocalPosToCanvas(new System.Drawing.Point(0, 0)).Y, obj.Width, obj.Height);
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
