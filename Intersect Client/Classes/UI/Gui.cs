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

using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Gwen.Skin;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Game;
using Intersect_Client.Classes.UI.Menu;
using Base = IntersectClientExtras.Gwen.Renderer.Base;
using Font = IntersectClientExtras.Gwen.Font;

namespace Intersect_Client.Classes.UI
{
    public static class Gui
    {

        //GWEN GUI
        public static bool GwenInitialized = false;
        public static InputBase GwenInput;
        public static Base GwenRenderer;
        public static GameRenderTexture GwenTexture;
        private static Canvas _gameCanvas;
        private static Canvas _menuCanvas;
        private static TexturedBase _gwenSkin;
        public static List<string> MsgboxErrors = new List<string>();
        public static bool SetupHandlers;
        public static GameGuiBase GameUI;
        public static MenuGuiBase MenuUI;
        public static ErrorMessageHandler ErrorMsgHandler;

        //Input Handling
        public static List<IntersectClientExtras.Gwen.Control.Base> FocusElements;

        #region "Gwen Setup and Input"
        //Gwen Low Level Functions
        public static void InitGwen()
        {
            //TODO: Make it easier to modify skin.
            _gwenSkin = new TexturedBase(GwenRenderer, "Resources/GUI/DefaultSkin.png");

            _gwenSkin.DefaultFont = new Font(GwenRenderer, "Resources/Fonts/Arvo-Regular.ttf", 10);


            // Create a Canvas (it's root, on which all other GWEN controls are created)
            _menuCanvas = new Canvas(_gwenSkin);
            _menuCanvas.SetSize(GameGraphics.Renderer.GetScreenWidth(), GameGraphics.Renderer.GetScreenHeight());
            _menuCanvas.ShouldDrawBackground = false;
            _menuCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _menuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            _gameCanvas = new Canvas(_gwenSkin);
            _gameCanvas.SetSize(GameGraphics.Renderer.GetScreenWidth(), GameGraphics.Renderer.GetScreenHeight());
            _gameCanvas.ShouldDrawBackground = false;
            _gameCanvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            _gameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            if (Globals.GameState == Enums.GameStates.Intro || Globals.GameState == Enums.GameStates.Menu)
            {
                GwenInput.Initialize(_menuCanvas);
            }
            else
            {
                GwenInput.Initialize(_gameCanvas);
            }

            FocusElements = new List<IntersectClientExtras.Gwen.Control.Base>();
            ErrorMsgHandler = new ErrorMessageHandler(_menuCanvas, _gameCanvas);
            if (Globals.GameState == Enums.GameStates.Intro || Globals.GameState == Enums.GameStates.Menu)
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
                return;}
            _menuCanvas.Dispose();
            _gameCanvas.Dispose();
            _gwenSkin.Dispose();
            GwenInitialized = false;
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
            if (!Gui.GwenInitialized) Gui.InitGwen();
            ErrorMsgHandler.Update();
            if (GwenTexture != null)
            {
                GwenTexture.Begin();
                GwenTexture.Clear(Color.Transparent);
            }
            _gameCanvas.RestrictToParent = false;
            _gameCanvas.SetPosition((int) GameGraphics.CurrentView.X, (int) GameGraphics.CurrentView.Y);
            if (Globals.GameState == Enums.GameStates.Menu)
            {
                MenuUI.Draw();
            }
            else if (Globals.GameState == Enums.GameStates.InGame)
            {
                GameUI.Draw();
            }
            if (GwenTexture != null)
            {
                GwenTexture.End();

                GameGraphics.DrawGameTexture(GwenTexture, GameGraphics.CurrentView.Left, GameGraphics.CurrentView.Top);
                    //SFML will need this part....
            }
        }
        public static Texture ToGwenTexture(GameTexture gameTex)
        {
            Texture tex = new Texture(GwenRenderer);
            GwenRenderer.LoadGameTexture(tex, gameTex);
            return tex;
        }
        public static void DrawSpriteToTexture(GameRenderTexture rt, string spritename, int w, int h)
        {
            rt.Begin();
            if (GameGraphics.EntityFileNames.Contains(spritename))
            {
                GameGraphics.DrawGameTexture(
                    GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)], new FloatRect(0, 0,
                        GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetWidth()/4f,
                        GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetHeight()/4f),
                    new FloatRect(
                        w/2 - (GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetWidth()/4f)/2,
                        h/2 - (GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetHeight()/4f)/2,
                        GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetWidth()/4f,
                        GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(spritename)].GetHeight()/4f),
                    Color.White, rt);
            }
            rt.End();
        }

        public static void DrawItemToTexture(GameRenderTexture rt, int itemnum, int xOffset = 0, int yOffset = 0, int width = 32, int height = 32, bool isEquipped = false, GameTexture bg = null)
        {
            rt.Begin();
            if (bg != null)
            {
                GameGraphics.DrawGameTexture(bg, 0, 0, rt);
            }
            if (itemnum > -1)
            {
                if (GameGraphics.ItemFileNames.Contains(Globals.GameItems[itemnum].Pic))
                {
                    GameGraphics.DrawGameTexture(
                            GameGraphics.ItemTextures[GameGraphics.ItemFileNames.IndexOf(Globals.GameItems[itemnum].Pic)], xOffset, yOffset,
                            Color.White, rt);
                }
            }
            if (isEquipped)
            {
                GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1),
                    new FloatRect(26 + xOffset, 0 + yOffset, 2, 2), Color.Red, rt);
            }
            rt.End();
        }
        public static void DrawSpellIconToTexture(GameRenderTexture rt, int spellnum, int xOffset = 0, int yOffset = 0, int width = 32, int height = 32, bool onCD = false, GameTexture bg = null)
        {
            rt.Begin();
            if (bg != null)
            {
                GameGraphics.DrawGameTexture(bg, 0, 0,rt);
            }
            if (spellnum > -1)
            {
                if (GameGraphics.SpellFileNames.Contains(Globals.GameSpells[spellnum].Pic))
                {
                    if (onCD)
                    {
                        GameGraphics.DrawGameTexture(
                            GameGraphics.SpellTextures[
                                GameGraphics.SpellFileNames.IndexOf(Globals.GameSpells[spellnum].Pic)], xOffset, yOffset,
                            new Color(255, 255, 255, 100), rt);
                    }
                    else
                    {
                        GameGraphics.DrawGameTexture(
                            GameGraphics.SpellTextures[
                                GameGraphics.SpellFileNames.IndexOf(Globals.GameSpells[spellnum].Pic)], xOffset, yOffset,
                            Color.White, rt);
                    }
                    
                }
            }
            rt.End();
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
        public static string[] WrapText(string input, int width)
        {
            var myOutput = new List<string>();
            var lastSpace = 0;
            var curPos = 0;
            var curLen = 1;
            input = input.Replace("\r\n", "\n");
            while (curPos + curLen < input.Length)
            {
                if (GameGraphics.Renderer.MeasureText(input.Substring(curPos, curLen),GameGraphics.GameFont,10).X < width)
                {
                    if (input[curPos + curLen] == ' ' || input[curPos + curLen] == '-')
                    {
                        lastSpace = curLen;
                    }
                    else if (input[curPos + curLen] == '\n')
                    {
                        myOutput.Add(input.Substring(curPos, curLen));
                        curPos = curPos + curLen + 1;
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
            }
            myOutput.Add(input.Substring(curPos, input.Length - curPos));
            return myOutput.ToArray();
        }
        #endregion

    }
}
