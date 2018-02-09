using System.Collections.Generic;
using System.IO;
using System.Xml;
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
        public static bool GwenInitialized;

        public static InputBase GwenInput;
        public static Base GwenRenderer;
        private static Canvas sGameCanvas;
        private static Canvas sMenuCanvas;
        private static TexturedBase sGwenSkin;
        public static List<KeyValuePair<string, string>> MsgboxErrors = new List<KeyValuePair<string, string>>();
        public static bool SetupHandlers;
        public static GameGuiBase GameUi;
        public static MenuGuiBase MenuUi;
        public static ErrorMessageHandler ErrorMsgHandler;
        public static string ActiveFont = "arial";
        public static bool HideUi;

        //Input Handling
        public static List<IntersectClientExtras.Gwen.Control.Base> FocusElements;

        public static List<IntersectClientExtras.Gwen.Control.Base> InputBlockingElements;

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

            // Create a Canvas (it's root, on which all other GWEN controls are created)
            sMenuCanvas = new Canvas(sGwenSkin, "MainMenu")
            {
                Scale = 1f //(GameGraphics.Renderer.GetScreenWidth()/1920f);
            };
            sMenuCanvas.SetSize((int) (GameGraphics.Renderer.GetScreenWidth() / sMenuCanvas.Scale),
                (int) (GameGraphics.Renderer.GetScreenHeight() / sMenuCanvas.Scale));
            sMenuCanvas.ShouldDrawBackground = false;
            sMenuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            sMenuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            sGameCanvas = new Canvas(gameSkin, "InGame");
            //_gameCanvas.Scale = (GameGraphics.Renderer.GetScreenWidth() / 1920f);
            sGameCanvas.SetSize((int) (GameGraphics.Renderer.GetScreenWidth() / sGameCanvas.Scale),
                (int) (GameGraphics.Renderer.GetScreenHeight() / sGameCanvas.Scale));
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

            FocusElements = new List<IntersectClientExtras.Gwen.Control.Base>();
            InputBlockingElements = new List<IntersectClientExtras.Gwen.Control.Base>();
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

            if (GameUi == null) LoadRootUiData(sMenuCanvas, "MainMenu.xml");
            if (MenuUi == null)
            {
                LoadRootUiData(sGameCanvas, "InGame.xml");
            }

            GwenInitialized = true;
        }

        public static void SaveRootUiData(IntersectClientExtras.Gwen.Control.Base control, string xmlname,
            bool bounds = false)
        {
            //Create XML Doc with UI 
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (XmlWriter writer = XmlWriter.Create(Path.Combine("resources", "gui", xmlname), settings))
            {
                writer.WriteStartDocument();
                control.WriteBaseUiXml(writer, bounds);
                writer.WriteEndDocument();
            }
        }

        public static void LoadRootUiData(IntersectClientExtras.Gwen.Control.Base control, string xmlname)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreComments = true;
            if (!File.Exists(Path.Combine("resources", "gui", xmlname))) return;
            using (XmlReader reader = XmlReader.Create(Path.Combine("resources", "gui", xmlname), readerSettings))
            {
                while (reader.Read())
                {
                    if (reader.Name == control.Name)
                    {
                        control.LoadUiXml(reader);
                        control.ProcessAlignments();
                    }
                }
            }
        }

        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            if (sMenuCanvas == null)
            {
                return;
            }
            sMenuCanvas.Dispose();
            sGameCanvas.Dispose();
            sGwenSkin.Dispose();
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
            if (!GwenInitialized) InitGwen();
            ErrorMsgHandler.Update();
            sGameCanvas.RestrictToParent = false;
            if (Globals.GameState == GameStates.Menu)
            {
                MenuUi.Draw();
            }
            else if (Globals.GameState == GameStates.InGame && !HideUi)
            {
                GameUi.Draw();
            }
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

        public static bool MouseHitBase(IntersectClientExtras.Gwen.Control.Base obj)
        {
            if (obj.IsHidden == true)
            {
                return false;
            }
            else
            {
                FloatRect rect = new FloatRect(obj.LocalPosToCanvas(new Point(0, 0)).X,
                    obj.LocalPosToCanvas(new Point(0, 0)).Y, obj.Width, obj.Height);
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