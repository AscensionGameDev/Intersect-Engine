using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Gwen.Skin;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Game;
using Intersect_Client.Classes.UI.Menu;
using Base = IntersectClientExtras.Gwen.Renderer.Base;

namespace Intersect_Client.Classes.UI
{
    public static class Gui
    {

        //GWEN GUI
        public static bool GwenInitialized = false;
        public static InputBase GwenInput;
        public static Base GwenRenderer;
        private static Canvas _gameCanvas;
        private static Canvas _menuCanvas;
        private static TexturedBase _gwenSkin;
        public static List<string> MsgboxErrors = new List<string>();
        public static bool SetupHandlers;
        public static GameGuiBase GameUI;
        public static MenuGuiBase MenuUI;
        public static ErrorMessageHandler ErrorMsgHandler;
        public static string DefaultFont = "arial";

        //Input Handling
        public static List<IntersectClientExtras.Gwen.Control.Base> FocusElements;
        public static List<IntersectClientExtras.Gwen.Control.Base> InputBlockingElements;

        #region "Gwen Setup and Input"
        //Gwen Low Level Functions
        public static void InitGwen()
        {
            //TODO: Make it easier to modify skin.
            _gwenSkin = new TexturedBase(GwenRenderer, Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "defaultskin.png"))
            {
                DefaultFont = Globals.ContentManager.GetFont(Gui.DefaultFont, 10)
            };
            var _gameSkin = new TexturedBase(GwenRenderer, Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "defaultskin.png"))
            {
                DefaultFont = Globals.ContentManager.GetFont(Gui.DefaultFont, 10)
            };


            // Create a Canvas (it's root, on which all other GWEN controls are created)
            _menuCanvas = new Canvas(_gwenSkin)
            {
                Scale = 1f//(GameGraphics.Renderer.GetScreenWidth()/1920f);
            };
            _menuCanvas.SetSize((int)(GameGraphics.Renderer.GetScreenWidth() / _menuCanvas.Scale), (int)(GameGraphics.Renderer.GetScreenHeight() / _menuCanvas.Scale));
            _menuCanvas.ShouldDrawBackground = false;
            _menuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _menuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            _gameCanvas = new Canvas(_gameSkin);
            //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
            _gameCanvas.SetSize((int)(GameGraphics.Renderer.GetScreenWidth() / _gameCanvas.Scale), (int)(GameGraphics.Renderer.GetScreenHeight() / _gameCanvas.Scale));
            _gameCanvas.ShouldDrawBackground = false;
            _gameCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _gameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
            {
                GwenInput.Initialize(_menuCanvas);
            }
            else
            {
                GwenInput.Initialize(_gameCanvas);
            }

            FocusElements = new List<IntersectClientExtras.Gwen.Control.Base>();
            InputBlockingElements = new List<IntersectClientExtras.Gwen.Control.Base>();
            ErrorMsgHandler = new ErrorMessageHandler(_menuCanvas, _gameCanvas);
            if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
            {
                MenuUI = new MenuGuiBase(_menuCanvas);
            }
            else
            {
                GameUI = new GameGuiBase(_gameCanvas);
            }

            GwenInitialized = true;
        }
        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            if (_menuCanvas == null)
            {
                return;
            }
            _menuCanvas.Dispose();
            _gameCanvas.Dispose();
            _gwenSkin.Dispose();
            GwenInitialized = false;
        }

        public static bool HasInputFocus()
        {
            if (FocusElements == null || InputBlockingElements == null) return false;
            for (var i = 0; i < FocusElements.Count; i++)
            {
                if (FocusElements[i].HasFocus)
                {
                    return true;
                }
            }
            for (var i = 0; i < InputBlockingElements.Count; i++)
            {
                if (InputBlockingElements[i].IsHidden == false)
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
            if (!Gui.GwenInitialized) Gui.InitGwen();
            ErrorMsgHandler.Update();
            _gameCanvas.RestrictToParent = false;
            if (Globals.GameState == GameStates.Menu)
            {
                MenuUI.Draw();
            }
            else if (Globals.GameState == GameStates.InGame)
            {
                GameUI.Draw();
            }
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
        public static bool MouseHitBase(IntersectClientExtras.Gwen.Control.Base obj)
        {
            if (obj.IsHidden == true)
            {
                return false;
            }
            else
            {
                FloatRect rect = new FloatRect(obj.LocalPosToCanvas(new Point(0, 0)).X, obj.LocalPosToCanvas(new Point(0, 0)).Y, obj.Width, obj.Height);
                if (rect.Contains(InputHandler.MousePosition.X, InputHandler.MousePosition.Y))
                {
                    return true;
                }

            }
            return false;
        }
        public static string[] WrapText(string input, int width, GameFont font)
        {
            var myOutput = new List<string>();
            var lastSpace = 0;
            var curPos = 0;
            var curLen = 1;
            input = input.Replace("\r\n", "\n");
            while (curPos + curLen < input.Length)
            {
                if (GameGraphics.Renderer.MeasureText(input.Substring(curPos, curLen), font, 1).X < width)
                {
                    if (input[curPos + curLen] == ' ' || input[curPos + curLen] == '-')
                    {
                        lastSpace = curLen;
                    }
                    else if (input[curPos + curLen] == '\n')
                    {
                        myOutput.Add(input.Substring(curPos, curLen).Trim());
                        curPos = curPos + curLen + 1;
                        curLen = 1;
                    }
                }
                else
                {
                    myOutput.Add(input.Substring(curPos, lastSpace).Trim());
                    if (lastSpace == 0) lastSpace = curLen-1;
                    curPos = curPos + lastSpace;
                    curLen = 1;
                }
                curLen++;
            }
            myOutput.Add(input.Substring(curPos, input.Length - curPos).Trim());
            return myOutput.ToArray();
        }
        #endregion

    }
}
