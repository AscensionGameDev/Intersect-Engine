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
using System.Windows.Forms;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Gwen.Renderer;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Database;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.File_Management;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Input;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Network;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.System;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

namespace Intersect_Client.Classes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Load Content/Options
            Globals.Database = new SfmlDatabase();
            Globals.Database.LoadConfig();
            Globals.Database.LoadPreferences();

            //Setup SFML Classes
            Globals.InputManager = new SfmlInput();
            GameGraphics.Renderer = new SfmlRenderer();
            Globals.ContentManager = new SfmlContentManager();
            Globals.System = new SfmlSystem();
            Gui.GwenTexture = GameGraphics.Renderer.CreateRenderTexture(GameGraphics.Renderer.GetScreenWidth(),
                GameGraphics.Renderer.GetScreenHeight());
            Gui.GwenRenderer = new IntersectRenderer(Gui.GwenTexture,GameGraphics.WhiteTex,GameGraphics.Renderer);
            Gui.GwenInput = new IntersectInput();

            GameNetwork.MySocket = new SfmlSocket();

            GameMain.Start();

            while (Globals.IsRunning)
            {
                GameMain.Update();
                GameGraphics.Render();
                Application.DoEvents();
            }
        }
    }
}
