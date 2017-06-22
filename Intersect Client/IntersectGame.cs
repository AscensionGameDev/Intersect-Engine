using System;
using System.Windows.Forms;
using Intersect.Client.Network;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Network;
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
using Intersect.Client.Classes.Core;

namespace Intersect_Client_MonoGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class IntersectGame : Game
    {
        GraphicsDeviceManager graphics;

        public IntersectGame()
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
            Strings.Init(Strings.IntersectComponent.Client, Globals.Database.Language);
            Globals.InputManager = new MonoInput(this);
            GameGraphics.Renderer = new MonoRenderer(graphics, Content, this);
            Globals.System = new MonoSystem();
            Gui.GwenRenderer = new IntersectRenderer(null, GameGraphics.Renderer);
            Gui.GwenInput = new IntersectInput();
            GameControls.Init();
            Window.IsBorderless = false;
        }

        //Really basic error handler for debugging purposes
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error((Exception) e.ExceptionObject);
            MessageBox.Show(
                "The Intersect Client has encountered an error and must close. Error information can be found in resources/logs/errors.log");
            Environment.Exit(-1);
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Setup SFML Classes
            ((MonoRenderer) GameGraphics.Renderer).Init(GraphicsDevice);
            // TODO: Remove old netcode
            //GameNetwork.MySocket = new MonoSocket();

            GameMain.Start();
            base.Initialize();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
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
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            if (Globals.IsRunning)
            {
                lock (Globals.GameLock)
                {
                    GameGraphics.Render();
                }
            }
            else
            {
                Exit();
            }
            base.Draw(gameTime);
        }
    }
}