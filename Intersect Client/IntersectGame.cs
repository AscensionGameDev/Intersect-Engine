using System;
using System.IO;
using System.Windows.Forms;
using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.MonoGame.Network;
using Intersect.Config;
using Intersect.Client.Classes.Localization;
using Intersect.Client.Classes.MonoGame.Database;
using Intersect.Logging;
using Intersect.Utilities;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Gwen.Renderer;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.File_Management;
using Intersect_Client.Classes.Bridges_and_Interfaces.SFML.System;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client_MonoGame.Classes.SFML.Graphics;
using Intersect_Client_MonoGame.Classes.SFML.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MainMenu = Intersect_Client.Classes.UI.Menu.MainMenu;

namespace Intersect.Client
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class IntersectGame : Game
    {
        public IntersectGame()
        {
            //Setup an error handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "";
            IsMouseVisible = true;
            Globals.ContentManager = new MonoContentManager();
            Globals.Database = new MonoDatabase();
            
            //Load ClientOptions
            if (File.Exists("resources/config.json"))
            {
                ClientOptions.LoadFrom(File.ReadAllText("resources/config.json"));
            }
            else
            {
                ClientOptions.LoadFrom(null);
            }
            File.WriteAllText("resources/config.json", ClientOptions.ToJson());

            Globals.Database.LoadPreferences();
            Strings.Load(ClientOptions.Language);
            
            Gui.ActiveFont = TextUtils.StripToLower(ClientOptions.Font);
            Globals.InputManager = new MonoInput(this);

            var renderer = new MonoRenderer(graphics, Content, this);
            GameGraphics.Renderer = renderer;
            if (renderer == null) throw new NullReferenceException("No renderer.");

            Globals.System = new MonoSystem();
            Gui.GwenRenderer = new IntersectRenderer(null, GameGraphics.Renderer);
            Gui.GwenInput = new IntersectInput();
            GameControls.Init();

            Window.Position = new Microsoft.Xna.Framework.Point(-20, -2000);
            Window.IsBorderless = false;
        }

        //Really basic error handler for debugging purposes
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            Log.Error((Exception) exception?.ExceptionObject);
            MessageBox.Show(
                @"The Intersect Client has encountered an error and must close. Error information can be found in logs/errors.log");
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
            (GameGraphics.Renderer as MonoRenderer)?.Init(GraphicsDevice);

            // TODO: Remove old netcode
            GameNetwork.MySocket = new IntersectNetworkSocket();
            GameNetwork.MySocket.Connected += MainMenu.OnNetworkConnected;
            GameNetwork.MySocket.ConnectionFailed += MainMenu.OnNetworkFailed;
            GameNetwork.MySocket.Disconnected += MainMenu.OnNetworkDisconnected;

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

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            GameNetwork.Close("quitting");
            base.Dispose();
        }
    }
}