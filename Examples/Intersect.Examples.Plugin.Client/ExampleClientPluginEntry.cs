using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Plugins;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Examples.Plugin.Client.PacketHandlers;
using Intersect.Examples.Plugin.Packets.Client;
using Intersect.Examples.Plugin.Packets.Server;
using Intersect.Plugins;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Examples.Plugin.Client;

/// <summary>
///     Demonstrates basic plugin functionality for the client.
/// </summary>
[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
public class ExampleClientPluginEntry : ClientPluginEntry
{
    private IGameTexture? _buttonTexture;
    private bool _disposed;
    private Mutex? _mutex;

    private bool wasConnected;

    /// <inheritdoc />
    public override void OnBootstrap(IPluginBootstrapContext context)
    {
        context.Logging.Application.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the application log!"
        );

        context.Logging.Plugin.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnBootstrap)} writing to the plugin log!"
        );

        _mutex = new Mutex(true, "testplugin", out var createdNew);
        if (!createdNew)
        {
            Environment.Exit(-2);
        }

        var exampleCommandLineOptions = context.CommandLine.ParseArguments<ExampleCommandLineOptions>();
        if (!exampleCommandLineOptions.ExampleFlag)
        {
            context.Logging.Plugin.LogWarning("Client wasn't started with the start-up flag!");
        }

        context.Logging.Plugin.LogInformation("Registering packets...");
        if (!context.Packet.TryRegisterPacketType<ExamplePluginClientPacket>())
        {
            context.Logging.Plugin.LogError($"Failed to register {nameof(ExamplePluginClientPacket)} packet.");
            Environment.Exit(-3);
        }

        if (!context.Packet.TryRegisterPacketType<ExamplePluginServerPacket>())
        {
            context.Logging.Plugin.LogError($"Failed to register {nameof(ExamplePluginServerPacket)} packet.");
            Environment.Exit(-3);
        }

        context.Logging.Plugin.LogInformation("Registering packet handlers...");
        if (!context.Packet
                .TryRegisterPacketHandler<ExamplePluginServerPacketHandler, ExamplePluginServerPacket>(out _))
        {
            context.Logging.Plugin.LogError(
                $"Failed to register {nameof(ExamplePluginServerPacketHandler)} packet handler."
            );
            Environment.Exit(-4);
        }

        context.Logging.Plugin.LogInformation(
            $@"{nameof(exampleCommandLineOptions.ExampleVariable)} = {exampleCommandLineOptions.ExampleVariable}"
        );
    }

    /// <inheritdoc />
    public override void OnStart(IClientPluginContext context)
    {
        context.Logging.Application.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the application log!"
        );

        context.Logging.Plugin.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStart)} writing to the plugin log!"
        );

        _buttonTexture = context.ContentManager.LoadEmbedded<IGameTexture>(
            context,
            ContentType.Interface,
            "Assets/join-our-discord.png"
        );

        context.Lifecycle.LifecycleChangeState += HandleLifecycleChangeState;

        context.Lifecycle.GameUpdate += HandleGameUpdate;
    }

    /// <inheritdoc />
    public override void OnStop(IClientPluginContext context)
    {
        context.Logging.Application.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the application log!"
        );

        context.Logging.Plugin.LogInformation(
            $@"{nameof(ExampleClientPluginEntry)}.{nameof(OnStop)} writing to the plugin log!"
        );
    }

    private void HandleGameUpdate(
        IClientPluginContext context,
        GameUpdateArgs gameUpdateArgs
    )
    {
        if (!wasConnected && context.Network.IsConnected)
        {
            wasConnected = true;

            context.Network.PacketSender.Send(new ExamplePluginClientPacket("This is an unprompted client packet."));
        }
    }

    private void HandleLifecycleChangeState(
        IClientPluginContext context,
        LifecycleChangeStateArgs lifecycleChangeStateArgs
    )
    {
        Debug.Assert(_buttonTexture != null, nameof(_buttonTexture) + " != null");

        var activeInterface = context.Lifecycle.Interface;

        switch (lifecycleChangeStateArgs.State)
        {
            case GameStates.Menu:
                AddButtonToMainMenu(context, activeInterface);
                break;
        }
    }

    private void AddButtonToMainMenu(
        IClientPluginContext context,
        IMutableInterface activeInterface
    )
    {
        var button = activeInterface.Create<Button>("DiscordButton");
        Debug.Assert(button != null, nameof(button) + " != null");

        var discordInviteUrl = context.GetTypedConfiguration<ExamplePluginConfiguration>()?.DiscordInviteUrl;
        button.Clicked += (sender, args) =>
        {
            if (string.IsNullOrWhiteSpace(discordInviteUrl))
            {
                context.Logging.Plugin.LogError(@"DiscordInviteUrl configuration property is null/empty/whitespace.");
                return;
            }

            BrowserUtils.Open(discordInviteUrl);
        };

        if (_buttonTexture is not { } buttonTexture)
        {
            return;
        }

        button.SetStateTexture(buttonTexture, buttonTexture.Name, ComponentState.Normal);
        button.SetSize(buttonTexture.Width, buttonTexture.Height);
        button.CurAlignments?.Add(Alignments.Bottom);
        button.CurAlignments?.Add(Alignments.Right);
        button.ProcessAlignments();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(true);

        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _mutex.Dispose();
        }

        _disposed = true;
    }
}