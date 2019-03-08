using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Skin;
using Intersect.Client.General;
using Intersect.Client.UI.Game;
using Intersect.Client.UI.Menu;
using JetBrains.Annotations;
using Base = Intersect.Client.Framework.Gwen.Renderer.Base;

namespace Intersect.Client.UI
{
    public static class Gui
    {
        //GWEN GUI
        public static bool GwenInitialized;

        public static InputBase GwenInput;
        public static Base GwenRenderer;
        private static Canvas sGameCanvas;
        private static Canvas sMenuCanvas;
        private static TexturedBase sGwenSkin;

        [NotNull]
        public static readonly List<KeyValuePair<string, string>> MsgboxErrors = new List<KeyValuePair<string, string>>();

        public static bool SetupHandlers;

        public static GameGuiBase GameUi;
        public static MenuGuiBase MenuUi;

        public static ErrorMessageHandler ErrorMsgHandler;
        public static string ActiveFont = "arial";
        public static bool HideUi;

        //Input Handling
        public static List<Framework.Gwen.Control.Base> FocusElements;

        public static List<Framework.Gwen.Control.Base> InputBlockingElements;

        #region "Gwen Setup and Input"

        //Gwen Low Level Functions
        public static void InitGwen()
        {
            //TODO: Make it easier to modify skin.
            sGwenSkin = new TexturedBase(GwenRenderer,
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "defaultskin.png"))
            {
                DefaultFont = Globals.ContentManager.GetFont(ActiveFont, 10)
            };
            var gameSkin = new TexturedBase(GwenRenderer,
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "defaultskin.png"))
            {
                DefaultFont = Globals.ContentManager.GetFont(ActiveFont, 10)
            };

            if (MenuUi != null) MenuUi.Dispose();
            if (GameUi != null) GameUi.Dispose();

            // Create a Canvas (it's root, on which all other GWEN controls are created)
            sMenuCanvas = new Canvas(sGwenSkin, "MainMenu")
            {
                Scale = 1f //(GameGraphics.Renderer.GetScreenWidth()/1920f);
            };
            sMenuCanvas.SetSize((int) (GameGraphics.Renderer.GetScreenWidth() / sMenuCanvas.Scale),
                (int) (GameGraphics.Renderer.GetScreenHeight() / sMenuCanvas.Scale));
            sMenuCanvas.ShouldDrawBackground = false;
            sMenuCanvas.BackgroundColor = Framework.GenericClasses.Color.FromArgb(255, 150, 170, 170);
            sMenuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            sGameCanvas = new Canvas(gameSkin, "InGame");
            //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
            sGameCanvas.SetSize((int) (GameGraphics.Renderer.GetScreenWidth() / sGameCanvas.Scale),
                (int) (GameGraphics.Renderer.GetScreenHeight() / sGameCanvas.Scale));
            sGameCanvas.ShouldDrawBackground = false;
            sGameCanvas.BackgroundColor = Framework.GenericClasses.Color.FromArgb(255, 150, 170, 170);
            sGameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
            {
                GwenInput.Initialize(sMenuCanvas);
            }
            else
            {
                GwenInput.Initialize(sGameCanvas);
            }

            FocusElements = new List<Framework.Gwen.Control.Base>();
            InputBlockingElements = new List<Framework.Gwen.Control.Base>();
            ErrorMsgHandler = new ErrorMessageHandler(sMenuCanvas, sGameCanvas);

            if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
            {
                MenuUi = new MenuGuiBase(sMenuCanvas);
                GameUi = null;
            }
            else
            {
                GameUi = new GameGuiBase(sGameCanvas);
                MenuUi = null;
            }

            GwenInitialized = true;
        }

        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            sMenuCanvas?.Dispose();
            sGameCanvas?.Dispose();
            sGwenSkin?.Dispose();
            GwenInitialized = false;
        }

        public static bool HasInputFocus()
        {
            if (FocusElements == null || InputBlockingElements == null) return false;
            return FocusElements.Any(t => t?.HasFocus ?? false) ||
                InputBlockingElements.Any(t => t?.IsHidden == false);
        }

        #endregion

        #region "GUI Functions"

        //Actual Drawing Function
        public static void DrawGui()
        {
            if (!GwenInitialized) InitGwen();
            ErrorMsgHandler.Update();
            sGameCanvas.RestrictToParent = false;
            if (Globals.GameState == GameStates.Menu)
            {
                MenuUi.Draw();
            }
            else if (Globals.GameState == GameStates.InGame && ((!Gui.GameUi?.EscapeMenu?.IsHidden ?? true) || !HideUi))
            {
                GameUi.Draw();
            }
        }

        public static void ToggleInput(bool val)
        {
            GwenInput.HandleInput = val;
        }

        public static bool MouseHitGui()
        {
            for (int i = 0; i < sGameCanvas.Children.Count; i++)
            {
                if (MouseHitBase(sGameCanvas.Children[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool MouseHitBase(Framework.Gwen.Control.Base obj)
        {
            if (obj.IsHidden == true)
            {
                return false;
            }
            else
            {
                FloatRect rect = new FloatRect(obj.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X,
                    obj.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y, obj.Width, obj.Height);
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
            var lastOk = 0;
            var lastCut = 0;
            input = input.Replace("\r\n", "\n");
            float measured;
            string line;
            while (curPos + curLen < input.Length)
            {
                line = input.Substring(curPos, curLen);
                measured = GameGraphics.Renderer.MeasureText(line, font, 1).X;
                //Debug.WriteLine($"w:{width},m:{measured},p:{curPos},l:{curLen},s:{lastSpace},t:'{line}'");
                if (measured < width)
                {
                    lastOk = lastSpace;
                    switch (input[curPos + curLen])
                    {
                        case ' ':
                        case '-':
                            lastSpace = curLen;
                            break;

                        case '\n':
                            myOutput.Add(input.Substring(curPos, curLen).Trim());
                            lastSpace = 0;
                            curPos = curPos + curLen + 1;
                            curLen = 1;
                            break;
                    }
                }
                else
                {
                    if (lastOk == 0) lastOk = curLen - 1;
                    line = input.Substring(curPos, lastOk).Trim();
                    //Debug.WriteLine($"line={line}");
                    myOutput.Add(line);
                    curPos = curPos + lastOk;
                    lastOk = 0;
                    lastSpace = 0;
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