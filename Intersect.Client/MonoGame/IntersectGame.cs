using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Gwen.Renderer;
using Intersect.Client.Framework.Input;
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
using Intersect.Updater;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using MainMenu = Intersect.Client.Interface.Menu.MainMenu;

namespace Intersect.Client.MonoGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    internal class IntersectGame : Game
    {
        private bool mInitialized;

        private double mLastUpdateTime = 0;

        private GraphicsDeviceManager mGraphics;

        #region "Autoupdate Variables"

        private Updater.Updater mUpdater;

        private Texture2D updaterBackground;

        private SpriteFont updaterFont;

        private SpriteFont updaterFontSmall;

        private Texture2D updaterProgressBar;

        private SpriteBatch updateBatch;

        private bool updaterGraphicsReset;

        #endregion

        private IClientContext Context { get; }

        private Action PostStartupAction { get; }

        private IntersectGame(IClientContext context, Action postStartupAction)
        {
            Context = context;
            PostStartupAction = postStartupAction;

            Strings.Load();

            mGraphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480,
                PreferHalfPixelOffset = true
            };

            mGraphics.PreparingDeviceSettings += (s, args) =>
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

            Window.IsBorderless = Context.StartupOptions.BorderlessWindow;

            var renderer = new MonoRenderer(mGraphics, Content, this)
            {
                OverrideResolution = Context.StartupOptions.ScreenResolution
            };

            Globals.InputManager = new MonoInput(this);
            GameClipboard.Instance = new MonoClipboard();

            Core.Graphics.Renderer = renderer;

            Globals.System = new MonoSystem();
            Interface.Interface.GwenRenderer = new IntersectRenderer(null, Core.Graphics.Renderer);
            Interface.Interface.GwenInput = new IntersectInput();
            Controls.Init();

            Window.Position = new Microsoft.Xna.Framework.Point(-20, -2000);
            Window.AllowAltF4 = false;

            // If we're going to be rendering a custom mouse cursor, hide the default one!
            if (!string.IsNullOrWhiteSpace(ClientConfiguration.Instance.MouseCursor))
            {
                IsMouseVisible = false;
            }
            
            if (!string.IsNullOrWhiteSpace(ClientConfiguration.Instance.UpdateUrl))
            {
                mUpdater = new Updater.Updater(
                    ClientConfiguration.Instance.UpdateUrl, Path.Combine("version.json"), true, 5
                );
            }
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (mUpdater != null)
            {
                LoadUpdaterContent();

                //Set the size of the updater screen before applying graphic changes.
                //We need to do this here instead of in the constructor for the size change to apply to Linux
                mGraphics.PreferredBackBufferWidth = 800;
                mGraphics.PreferredBackBufferHeight = 480;
            }

            mGraphics.ApplyChanges();
        }

        private void IntersectInit()
        {
            (Core.Graphics.Renderer as MonoRenderer)?.Init(GraphicsDevice);

            // TODO: Remove old netcode
            Networking.Network.Socket = new MonoSocket(Context);
            Networking.Network.Socket.Connected += (sender, connectionEventArgs) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Networking.Network.Socket.ConnectionFailed += (sender, connectionEventArgs, denied) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Networking.Network.Socket.Disconnected += (sender, connectionEventArgs) =>
                MainMenu.SetNetworkStatus(connectionEventArgs.NetworkStatus);

            Main.Start(Context);

            mInitialized = true;

            PostStartupAction();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (mUpdater != null)
            {
                if (mUpdater.CheckUpdaterContentLoaded())
                {
                    LoadUpdaterContent();
                }

                if (mUpdater.Status == UpdateStatus.Done || mUpdater.Status == UpdateStatus.None)
                {
                    if (updaterGraphicsReset == true)
                    {
                        //Drew a frame, now let's initialize the engine
                        IntersectInit();
                        mUpdater = null;
                    }
                }
                else if (mUpdater.Status == UpdateStatus.Restart)
                {
                    //Auto relaunch on Windows
                    switch (Environment.OSVersion.Platform)
                    {
                        case PlatformID.Win32NT:
                        case PlatformID.Win32S:
                        case PlatformID.Win32Windows:
                        case PlatformID.WinCE:
                            Process.Start(
                                Environment.GetCommandLineArgs()[0],
                                Environment.GetCommandLineArgs().Length > 1
                                    ? string.Join(" ", Environment.GetCommandLineArgs().Skip(1))
                                    : null
                            );

                            Exit();
                            break;
                    }
                }
            }

            if (mUpdater == null)
            {
                if (!mInitialized)
                {
                    IntersectInit();
                }

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
                else
                {
                    Exit();
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

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            if (mUpdater != null)
            {
                if (mUpdater.Status == UpdateStatus.Done || mUpdater.Status == UpdateStatus.None)
                {
                    if (updaterGraphicsReset == false)
                    {
                        (Core.Graphics.Renderer as MonoRenderer)?.Init(GraphicsDevice);
                        (Core.Graphics.Renderer as MonoRenderer)?.Init();
                        (Core.Graphics.Renderer as MonoRenderer)?.Begin();
                        (Core.Graphics.Renderer as MonoRenderer)?.End();
                        updaterGraphicsReset = true;
                    }
                }
                else
                {
                    DrawUpdater();
                }
            }
            else
            {
                if (Globals.IsRunning && mInitialized)
                {
                    lock (Globals.GameLock)
                    {
                        Core.Graphics.Render();
                    }
                }
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
                    var platform = GetType()
                        .GetField("Platform", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);

                    var field = platform.GetType()
                        .GetField("_isExiting", BindingFlags.NonPublic | BindingFlags.Instance);

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
                        Strings.Combat.warningtitle, Strings.Combat.warningcharacterselect, true,
                        InputBox.InputType.YesNo, ExitToDesktop, null, null
                    );

                    //Restart the MonoGame RunLoop
                    Run();
                    return;
                }
            }

            try
            {
                mUpdater?.Stop();
            }
            catch
            {
            }

            //Just close if we don't need to show a combat warning
            base.OnExiting(sender, args);
            Networking.Network.Close("quitting");
            Dispose();
        }

        private void DrawUpdater()
        {
            //Draw updating text and show progress bar...

            if (updateBatch == null)
            {
                updateBatch = new SpriteBatch(GraphicsDevice);
            }

            updateBatch.Begin(SpriteSortMode.Immediate);

            //Default Window Size is 800x480
            if (updaterBackground != null)
            {
                updateBatch.Draw(
                    updaterBackground, new Rectangle(0, 0, 800, 480),
                    new Rectangle?(new Rectangle(0, 0, updaterBackground.Width, updaterBackground.Height)),
                    Microsoft.Xna.Framework.Color.White
                );
            }

            var status = "";
            var progressPercent = 0f;
            var progress = "";
            var filesRemaining = "";
            var sizeRemaining = "";

            switch (mUpdater.Status)
            {
                case UpdateStatus.Checking:
                    status = Strings.Update.Checking;
                    break;

                case UpdateStatus.Updating:
                    status = Strings.Update.Updating;
                    progressPercent = mUpdater.Progress / 100f;
                    progress = Strings.Update.Percent.ToString((int) mUpdater.Progress);
                    filesRemaining = mUpdater.FilesRemaining + " Files Remaining";
                    sizeRemaining = mUpdater.GetHumanReadableFileSize(mUpdater.SizeRemaining) + " Left";
                    break;

                case UpdateStatus.Restart:
                    status = Strings.Update.Restart.ToString(Strings.Main.gamename);
                    progressPercent = 100;
                    progress = Strings.Update.Percent.ToString(100);
                    break;

                case UpdateStatus.Done:
                    status = Strings.Update.Done;
                    progressPercent = 100;
                    progress = Strings.Update.Percent.ToString(100);
                    break;

                case UpdateStatus.Error:
                    status = Strings.Update.Error;
                    progress = mUpdater.Exception?.Message ?? "";
                    progressPercent = 100;
                    break;

                case UpdateStatus.None:
                    //Nothing here!
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (updaterFont != null)
            {
                var size = updaterFont.MeasureString(status);
                updateBatch.DrawString(
                    updaterFont, status, new Vector2(800 / 2 - size.X / 2, 360), Microsoft.Xna.Framework.Color.White
                );
            }

            //Bar will exist at 400 to 432
            if (updaterProgressBar != null)
            {
                updateBatch.Draw(
                    updaterProgressBar, new Rectangle(100, 400, (int) (600 * progressPercent), 32),
                    new Rectangle?(
                        new Rectangle(
                            0, 0, (int) (updaterProgressBar.Width * progressPercent), updaterProgressBar.Height
                        )
                    ), Microsoft.Xna.Framework.Color.White
                );
            }

            //Bar will be 600 pixels wide
            if (updaterFontSmall != null)
            {
                //Draw % in center of bar
                var size = updaterFontSmall.MeasureString(progress);
                updateBatch.DrawString(
                    updaterFontSmall, progress, new Vector2(800 / 2 - size.X / 2, 405),
                    Microsoft.Xna.Framework.Color.White
                );

                //Draw files remaining on bottom left
                updateBatch.DrawString(
                    updaterFontSmall, filesRemaining, new Vector2(100, 440), Microsoft.Xna.Framework.Color.White
                );

                //Draw total remaining on bottom right
                size = updaterFontSmall.MeasureString(sizeRemaining);
                updateBatch.DrawString(
                    updaterFontSmall, sizeRemaining, new Vector2(700 - size.X, 440), Microsoft.Xna.Framework.Color.White
                );
            }

            updateBatch.End();
        }

        private void LoadUpdaterContent()
        {
            if (File.Exists(Path.Combine("resources", "updater", "background.png")))
            {
                updaterBackground = Texture2D.FromFile(
                    GraphicsDevice, Path.Combine("resources", "updater", "background.png")
                );
            }

            if (File.Exists(Path.Combine("resources", "updater", "progressbar.png")))
            {
                updaterProgressBar = Texture2D.FromFile(
                    GraphicsDevice, Path.Combine("resources", "updater", "progressbar.png")
                );
            }

            if (File.Exists(Path.Combine("resources", "updater", "font.xnb")))
            {
                updaterFont = Content.Load<SpriteFont>(Path.Combine("resources", "updater", "font"));
            }

            if (File.Exists(Path.Combine("resources", "updater", "fontsmall.xnb")))
            {
                updaterFontSmall = Content.Load<SpriteFont>(Path.Combine("resources", "updater", "fontsmall"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
            {
                return;
            }

            if (!Context.IsDisposed)
            {
                Context.Dispose();
            }
        }

        /// <summary>
        /// Implements <see cref="IPlatformRunner"/> for MonoGame.
        /// </summary>
        internal class MonoGameRunner : IPlatformRunner
        {
            /// <inheritdoc />
            public void Start(IClientContext context, Action postStartupAction)
            {
                using (var game = new IntersectGame(context, postStartupAction))
                {
                    game.Run();
                }
            }
        }
    }
}
