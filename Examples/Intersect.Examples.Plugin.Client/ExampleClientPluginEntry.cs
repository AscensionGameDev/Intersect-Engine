using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Plugins;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins;
using Microsoft;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Examples.Plugin.Packets.Server;
using Intersect.Examples.Plugin.Client.PacketHandlers;
using Intersect.Examples.Plugin.Packets.Client;

namespace Intersect.Examples.Plugin.Client
{
    /// <summary>
    /// Demonstrates basic plugin functionality for the client.
    /// </summary>
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
    public class ExampleClientPluginEntry : ClientPluginEntry
    {
        private bool mDisposed;
        private Mutex mMutex;

        private GameTexture mButtonTexture;

        /// <inheritdoc />
        public override void OnBootstrap([ValidatedNotNull] IPluginBootstrapContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the application log!");

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the plugin log!");

            mMutex = new Mutex(true, "testplugin", out var createdNew);
            if (!createdNew)
            {
                Environment.Exit(-2);
            }

            var exampleCommandLineOptions = context.CommandLine.ParseArguments<ExampleCommandLineOptions>();
            if (!exampleCommandLineOptions.ExampleFlag)
            {
                context.Logging.Plugin.Warn("Client wasn't started with the start-up flag!");
            }

            context.Logging.Plugin.Info("Registering packets...");
            if (!context.Network.TryRegisterPacketType<ExamplePluginClientPacket>())
            {
                context.Logging.Plugin.Error($"Failed to register {nameof(ExamplePluginClientPacket)} packet.");
                Environment.Exit(-3);
            }

            if (!context.Network.TryRegisterPacketType<ExamplePluginServerPacket>())
            {
                context.Logging.Plugin.Error($"Failed to register {nameof(ExamplePluginServerPacket)} packet.");
                Environment.Exit(-3);
            }

            context.Logging.Plugin.Info("Registering packet handlers...");
            if (!context.Network.TryRegisterPacketHandler<ExamplePluginServerPacketHandler, ExamplePluginServerPacket>(out _))
            {
                context.Logging.Plugin.Error($"Failed to register {nameof(ExamplePluginServerPacketHandler)} packet handler.");
                Environment.Exit(-4);
            }

            context.Logging.Plugin.Info(
                $@"{nameof(exampleCommandLineOptions.ExampleVariable)} = {exampleCommandLineOptions.ExampleVariable}");
        }

        /// <inheritdoc />
        public override void OnStart([ValidatedNotNull] IClientPluginContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the application log!");

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the plugin log!");

            mButtonTexture = context.ContentManager.LoadEmbedded<GameTexture>(
                context, ContentTypes.Interface, "Assets/join-our-discord.png");

            context.Lifecycle.LifecycleChangeState += HandleLifecycleChangeState;
        }

        /// <inheritdoc />
        public override void OnStop([ValidatedNotNull] IClientPluginContext context)
        {
            context.Logging.Application.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the application log!");

            context.Logging.Plugin.Info(
                $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the plugin log!");
        }

        private void HandleLifecycleChangeState([ValidatedNotNull] IClientPluginContext context,
            [ValidatedNotNull] LifecycleChangeStateArgs lifecycleChangeStateArgs)
        {
            Debug.Assert(mButtonTexture != null, nameof(mButtonTexture) + " != null");

            var activeInterface = context.Lifecycle.Interface;
            if (activeInterface == null)
            {
                return;
            }

            switch (lifecycleChangeStateArgs.State)
            {
                case GameStates.Menu:
                    AddButtonToMainMenu(context, activeInterface);
                    break;
            }
        }

        private void AddButtonToMainMenu([ValidatedNotNull] IClientPluginContext context,
            [ValidatedNotNull] IMutableInterface activeInterface)
        {
            var button = activeInterface.Create<Button>("DiscordButton");
            Debug.Assert(button != null, nameof(button) + " != null");

            var discordInviteUrl = context.GetTypedConfiguration<ExamplePluginConfiguration>()?.DiscordInviteUrl;
            button.Clicked += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(discordInviteUrl))
                {
                    context.Logging.Plugin.Error($@"DiscordInviteUrl configuration property is null/empty/whitespace.");
                    return;
                }

                Process.Start(discordInviteUrl);
            };

            button.SetImage(mButtonTexture, mButtonTexture.Name, Button.ControlState.Normal);
            button.SetSize(mButtonTexture.GetWidth(), mButtonTexture.GetHeight());
            button.CurAlignments?.Add(Alignments.Bottom);
            button.CurAlignments?.Add(Alignments.Right);
            button.ProcessAlignments();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);

            if (mDisposed)
            {
                return;
            }

            if (disposing)
            {
                mMutex.Dispose();
            }

            mDisposed = true;
        }
    }
}