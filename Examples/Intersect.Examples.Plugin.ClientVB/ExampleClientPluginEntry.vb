Imports System.Diagnostics.CodeAnalysis
Imports System.Threading
Imports Intersect.Client.Framework.Content
Imports Intersect.Client.Framework.Graphics
Imports Intersect.Client.Framework.Gwen
Imports Intersect.Client.Framework.Gwen.Control
Imports Intersect.Client.General
Imports Intersect.Client.Interface
Imports Intersect.Client.Plugins
Imports Intersect.Client.Plugins.Interfaces
Imports Intersect.Plugins
Imports Intersect.Utilities
Imports Microsoft.Extensions.Logging


''' <summary>
'''     Demonstrates basic plugin functionality for the client.
''' </summary>
<SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")>
Public Class ExampleClientPluginEntry
    Inherits ClientPluginEntry

    Private _disposed As Boolean
    Private _mutex As Mutex

    Private _buttonTexture As IGameTexture

    ''' <inheritdoc />
    Public Overrides Sub OnBootstrap(context As IPluginBootstrapContext)
        MyBase.OnBootstrap(context)

        context.Logging.Application.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnBootstrap)} writing to the application log!"
        )
        context.Logging.Plugin.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnBootstrap)} writing to the plugin log!"
        )

        Dim createdNew As Boolean = Nothing
        _mutex = New Mutex(True, "testplugin", createdNew)

        If Not createdNew Then
            Environment.[Exit](- 1)
        End If

        Dim exampleCommandLineOptions = context.CommandLine.ParseArguments (Of ExampleCommandLineOptions)()

        If Not exampleCommandLineOptions.ExampleFlag Then
            context.Logging.Plugin.LogWarning("Client wasn't started with the start-up flag!")
        End If

        context.Logging.Plugin.LogInformation(
            $"{NameOf(exampleCommandLineOptions.ExampleVariable)} = {exampleCommandLineOptions.ExampleVariable}")
    End Sub

    ''' <inheritdoc />
    Public Overrides Sub OnStart(context As IClientPluginContext)
        context.Logging.Application.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStart)} writing to the application log!")
        context.Logging.Plugin.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStart)} writing to the plugin log!")
        _buttonTexture = context.ContentManager.LoadEmbedded (Of IGameTexture)(context, ContentType.[Interface],
                                                                              "join-our-discord.png")
        AddHandler context.Lifecycle.LifecycleChangeState, AddressOf HandleLifecycleChangeState
    End Sub

    ''' <inheritdoc />
    Public Overrides Sub OnStop(context As IClientPluginContext)
        context.Logging.Application.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStop)} writing to the application log!"
        )
        context.Logging.Plugin.LogInformation(
            $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStop)} writing to the plugin log!"
        )
    End Sub

    Private Sub HandleLifecycleChangeState(
                                           context As IClientPluginContext,
                                           lifecycleChangeStateArgs As LifecycleChangeStateArgs
    )
        Debug.Assert(_buttonTexture IsNot Nothing, NameOf(_buttonTexture) & " != null")
        Dim activeInterface = context.Lifecycle.[Interface]

        If activeInterface Is Nothing Then
            Return
        End If

        Select Case lifecycleChangeStateArgs.State
            Case GameStates.Menu
                AddButtonToMainMenu(context, activeInterface)
        End Select
    End Sub

    Private Sub AddButtonToMainMenu(
                                    context As IClientPluginContext,
                                    activeInterface As IMutableInterface
    )
        Dim button = activeInterface.Create (Of Button)("DiscordButton")
        Debug.Assert(button IsNot Nothing, NameOf(button) & " != null")
        Dim discordInviteUrl = context.GetTypedConfiguration (Of ExamplePluginConfiguration)()?.DiscordInviteUrl
        AddHandler button.Clicked,
            Sub(sender, args)
                If String.IsNullOrWhiteSpace(discordInviteUrl) Then
                    context.Logging.Plugin.[LogError](
                        $"DiscordInviteUrl configuration property is null/empty/whitespace.")
                    Return
                End If

                BrowserUtils.Open(discordInviteUrl)
            End Sub

        Dim buttonTexture = _buttonTexture
        If buttonTexture Is Nothing Then
            Return
        End If

        button.SetStateTexture(buttonTexture, buttonTexture.Name, ComponentState.Normal)
        button.SetSize(buttonTexture.Width, buttonTexture.Height)
        button.CurAlignments?.Add(Alignments.Bottom)
        button.CurAlignments?.Add(Alignments.Right)
        button.ProcessAlignments()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(True)

        If _disposed Then
            Return
        End If

        If disposing Then
            _mutex?.Dispose()
        End If

        _disposed = True
    End Sub
End Class