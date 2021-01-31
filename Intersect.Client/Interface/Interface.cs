using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Skin;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Interface.Shared.Errors;

using Base = Intersect.Client.Framework.Gwen.Renderer.Base;

namespace Intersect.Client.Interface
{

    public static class Interface
    {

        public static readonly List<KeyValuePair<string, string>> MsgboxErrors =
            new List<KeyValuePair<string, string>>();

        public static ErrorHandler ErrorMsgHandler;

        //GWEN GUI
        public static bool GwenInitialized;

        public static InputBase GwenInput;

        public static Base GwenRenderer;

        public static bool HideUi;

        private static Canvas sGameCanvas;

        private static Canvas sMenuCanvas;

        public static bool SetupHandlers { get; set; }

        public static GameInterface GameUi { get; private set; }

        public static MenuGuiBase MenuUi { get; private set; }

        public static TexturedBase Skin { get; set; }

        //Input Handling
        public static List<Framework.Gwen.Control.Base> FocusElements { get; set; }

        public static List<Framework.Gwen.Control.Base> InputBlockingElements { get; set; }

        #region "Gwen Setup and Input"

        //Gwen Low Level Functions
        public static void InitGwen()
        {
            //TODO: Make it easier to modify skin.
            if (Skin == null)
            {
                Skin = new TexturedBase(
                    GwenRenderer,
                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "defaultskin.png")
                )
                {
                    DefaultFont = Graphics.UIFont
                };
            }

            MenuUi?.Dispose();

            GameUi?.Dispose();

            // Create a Canvas (it's root, on which all other GWEN controls are created)
            sMenuCanvas = new Canvas(Skin, "MainMenu")
            {
                Scale = 1f //(GameGraphics.Renderer.GetScreenWidth()/1920f);
            };

            sMenuCanvas.SetSize(
                (int) (Graphics.Renderer.GetScreenWidth() / sMenuCanvas.Scale),
                (int) (Graphics.Renderer.GetScreenHeight() / sMenuCanvas.Scale)
            );

            sMenuCanvas.ShouldDrawBackground = false;
            sMenuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            sMenuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            sGameCanvas = new Canvas(Skin, "InGame");

            //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
            sGameCanvas.SetSize(
                (int) (Graphics.Renderer.GetScreenWidth() / sGameCanvas.Scale),
                (int) (Graphics.Renderer.GetScreenHeight() / sGameCanvas.Scale)
            );

            sGameCanvas.ShouldDrawBackground = false;
            sGameCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
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
            ErrorMsgHandler = new ErrorHandler(sMenuCanvas, sGameCanvas);

            if (Globals.GameState == GameStates.Intro || Globals.GameState == GameStates.Menu)
            {
                MenuUi = new MenuGuiBase(sMenuCanvas);
                GameUi = null;
            }
            else
            {
                GameUi = new GameInterface(sGameCanvas);
                MenuUi = null;
            }

            Globals.OnLifecycleChangeState();

            GwenInitialized = true;
        }

        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            sMenuCanvas?.Dispose();
            sGameCanvas?.Dispose();
            GameUi?.Dispose();

            // Destroy our target UI as well! Above code does NOT appear to clear this properly.
            if (Globals.Me != null)
            {
                Globals.Me.ClearTarget();
                Globals.Me.TargetBox?.Dispose();
                Globals.Me.TargetBox = null;
            }
            
            GwenInitialized = false;
        }

        public static bool HasInputFocus()
        {
            if (FocusElements == null || InputBlockingElements == null)
            {
                return false;
            }

            return FocusElements.Any(t => t.MouseInputEnabled && (t?.HasFocus ?? false)) || InputBlockingElements.Any(t => t?.IsHidden == false);
        }

        #endregion

        #region "GUI Functions"

        //Actual Drawing Function
        public static void DrawGui()
        {
            if (!GwenInitialized)
            {
                InitGwen();
            }

            ErrorMsgHandler.Update();
            sGameCanvas.RestrictToParent = false;
            if (Globals.GameState == GameStates.Menu)
            {
                MenuUi.Draw();
            }
            else if (Globals.GameState == GameStates.InGame &&
                     ((!Interface.GameUi?.EscapeMenu?.IsHidden ?? true) || !HideUi))
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
            for (var i = 0; i < sGameCanvas.Children.Count; i++)
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
            else if (!obj.MouseInputEnabled)
            {
                // Check if we're hitting a child element.
                for (var i = 0; i < obj.Children.Count; i++)
                {
                    if (MouseHitBase(obj.Children[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                var rect = new FloatRect(
                    obj.LocalPosToCanvas(new Point(0, 0)).X, obj.LocalPosToCanvas(new Point(0, 0)).Y, obj.Width,
                    obj.Height
                );

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
            if (input == null)
            {
                myOutput.Add("");
            }
            else
            {
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
                    measured = Graphics.Renderer.MeasureText(line, font, 1).X;
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
                        if (lastOk == 0)
                        {
                            lastOk = curLen - 1;
                        }

                        line = input.Substring(curPos, lastOk).Trim();
                        myOutput.Add(line);
                        curPos = curPos + lastOk;
                        lastOk = 0;
                        lastSpace = 0;
                        curLen = 1;
                    }

                    curLen++;
                }

                myOutput.Add(input.Substring(curPos, input.Length - curPos).Trim());
            }

            return myOutput.ToArray();
        }

        #endregion

    }

}
