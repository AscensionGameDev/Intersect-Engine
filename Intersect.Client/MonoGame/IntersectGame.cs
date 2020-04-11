using System;
using System.Reflection;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Renderer;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Database;
using Intersect.Client.MonoGame.File_Management;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Client.MonoGame.Input;
using Intersect.Client.MonoGame.Network;
using Intersect.Client.MonoGame.System;
using Intersect.Configuration;
using Intersect.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MainMenu = Intersect.Client.Interface.Menu.MainMenu;

namespace Intersect.Client.MonoGame
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
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage =
                    RenderTargetUsage.PreserveContents;
            };

            Content.RootDirectory = "";
            IsMouseVisible = true;
            Globals.ContentManager = new MonoContentManager();
            Globals.Database = new MonoDatabase();

            /* Load configuration */
            ClientConfiguration.LoadAndSave(ClientConfiguration.DefaultPath);

            Globals.Database.LoadPreferences();

            Globals.InputManager = new MonoInput(this);

            var renderer = new MonoRenderer(graphics, Content, this);
            Core.Graphics.Renderer = renderer;
            if (renderer == null)
            {
                throw new NullReferenceException("No renderer.");
            }

            Globals.System = new MonoSystem();
            Interface.Interface.GwenRenderer = new IntersectRenderer(null, Core.Graphics.Renderer);
            Interface.Interface.GwenInput = new IntersectInput();
            Controls.Init();

            Window.Position = new Microsoft.Xna.Framework.Point(-20, -2000);
            Window.IsBorderless = false;
            Window.AllowAltF4 = false;
        }

        //Really basic error handler for debugging purposes
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            Log.Error((Exception) exception?.ExceptionObject);
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
            (Core.Graphics.Renderer as MonoRenderer)?.Init(GraphicsDevice);

            // TODO: Remove old netcode
            Networking.Network.Socket = new MonoSocket();
            Networking.Network.Socket.Connected += (sender, connectionEventArgs) => MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);
            Networking.Network.Socket.ConnectionFailed += (sender, connectionEventArgs, denied) => MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);
            Networking.Network.Socket.Disconnected += (sender, connectionEventArgs) => MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Main.Start();
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
                        Main.Update();
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
                    Core.Graphics.Render();
                }
            }
            else
            {
                Exit();
            }

            base.Draw(gameTime);
        }

        private void ExitToDesktop(object sender, EventArgs e)
        {
            if (Globals.Me != null)
            {
                Globals.Me.CombatTimer = 0;
            }

            Globals.IsRunning = false;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (Globals.Me != null && Globals.Me.CombatTimer > Globals.System?.GetTimeMs())
            {
                //Try to prevent SDL Window Close
                var exception = false;
                try
                {
                    var platform = GetType().GetField("Platform", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
                    var field = platform.GetType().GetField("_isExiting", BindingFlags.NonPublic | BindingFlags.Instance);
                    field.SetValue(platform, 0);
                    
                }
                catch
                {
                    //TODO: Should we log here? I really don't know if it's necessary.
                    exception = true;
                }

                if (!exception)
                {
                    //Show Message Getting Exit Confirmation From Player to Leave in Combat
                    var box = new InputBox(
                        Strings.Combat.warningtitle, Strings.Combat.warningcharacterselect, true, InputBox.InputType.YesNo,
                        ExitToDesktop, null, null
                    );

                    //Restart the MonoGame RunLoop
                    Run();
                    return;
                }
            }

            //Just close if we don't need to show a combat warning
            base.OnExiting(sender, args);
            Networking.Network.Close("quitting");
            base.Dispose();
        }

    }

}
