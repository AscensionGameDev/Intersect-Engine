using System;
using System.IO;
using System.Windows.Forms;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Gwen.Renderer;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Database;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.File_Management;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Network;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.System;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client_MonoGame.Classes.SFML.Graphics;
using Intersect_Client_MonoGame.Classes.SFML.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Client_MonoGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Intersect : Game
    {
        GraphicsDeviceManager graphics;

        public Intersect()
        {
            //Setup an error handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "";
            IsMouseVisible = true;
            Globals.ContentManager = new MonoContentManager();
            Globals.Database = new MonoDatabase();
            Globals.Database.LoadConfig();
            Globals.Database.LoadPreferences();
            Globals.InputManager = new MonoInput(this);
            GameGraphics.Renderer = new MonoRenderer(graphics, Content, this);
            Globals.System = new MonoSystem();
            Gui.GwenRenderer = new IntersectRenderer(null, GameGraphics.Renderer);
            Gui.GwenInput = new IntersectInput();

            //We remove the border and add it back in draw to the window size will be forced to update.
            //This is to bypass a MonoGame issue where the client viewport was not updating until the window was dragged.
            Window.IsBorderless = true;
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            using (StreamWriter writer = new StreamWriter(Path.Combine("resources", "errors.log"), true))
            {
                writer.WriteLine("Message :" + ((Exception)e.ExceptionObject).Message + "<br/>" + Environment.NewLine +
                                 "StackTrace :" + ((Exception)e.ExceptionObject).StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine +
                                 "-----------------------------------------------------------------------------" +
                                 Environment.NewLine);
            }
            MessageBox.Show(
                "The Intersect client has encountered an error and must close. Error information can be found in resources/errors.log");
            Environment.Exit(-1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Setup SFML Classes
            ((MonoRenderer)GameGraphics.Renderer).Init(GraphicsDevice);
            GameNetwork.MySocket = new MonoSocket();

            GameMain.Start();
            base.Initialize();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Globals.IsRunning)
            {
                lock (Globals.GameLock)
                {
                    GameMain.Update();
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //We remove the border and add it back in draw to the window size will be forced to update.
            //This is to bypass a MonoGame issue where the client viewport was not updating until the window was dragged.
            if (Window.IsBorderless) Window.IsBorderless = false;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied; ;
            if (Globals.IsRunning)
            {
                lock (Globals.GameLock)
                {
                    GameGraphics.Render();
                }
            }
            else
            {
                base.Exit();
            }
            base.Draw(gameTime);
        }
    }
}
