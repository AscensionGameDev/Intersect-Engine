using System;
using System.IO;
using System.Windows.Forms;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Renderer;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Database;
using Intersect.Client.MonoGame.File_Management;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Client.MonoGame.Input;
using Intersect.Client.MonoGame.Network;
using Intersect.Client.MonoGame.System;
using Intersect.Client.Networking;
using Intersect.Client.UI;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MainMenu = Intersect.Client.UI.Menu.MainMenu;

namespace Intersect.Client
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class IntersectGame : Game
    {
        private double mLastUpdateTime = 0;
        public IntersectGame()
        {
            //Setup an error handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Strings.Load();

            var graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };

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
            
            Gui.ActiveFont = TextUtils.StripToLower(ClientOptions.Instance.Font);
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
            MessageBox.Show(Strings.Errors.errorencountered);
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
            GameNetwork.Socket = new IntersectNetworkSocket();
            GameNetwork.Socket.Connected += MainMenu.OnNetworkConnected;
            GameNetwork.Socket.ConnectionFailed += MainMenu.OnNetworkFailed;
            GameNetwork.Socket.Disconnected += MainMenu.OnNetworkDisconnected;

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
                if (mLastUpdateTime < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    lock (Globals.GameLock)
                    {
                        GameMain.Update();
                    }

                    ///mLastUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + (1000/60f);
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