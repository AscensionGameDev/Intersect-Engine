Imports System.Diagnostics.CodeAnalysis
Imports System.Threading
Imports Intersect.Client.Framework.Content
Imports Intersect.Client.Framework.Graphics
Imports Intersect.Client.Framework.Gwen
Imports Intersect.Client.Framework.Gwen.Control
Imports Intersect.Client.General
Imports Intersect.Client.[Interface]
Imports Intersect.Client.Plugins
Imports Intersect.Client.Plugins.Interfaces
Imports Intersect.Plugins
Imports Microsoft

Namespace Intersect.Examples.ClientPlugin
    ''' <summary>
    ''' Demonstrates basic plugin functionality for the client.
    ''' </summary>
    <SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")>
    Public Class ExampleClientPluginEntry
        Inherits ClientPluginEntry

        Private mDisposed As Boolean
        Private mMutex As Mutex

        Private mButtonTexture As GameTexture

        ''' <inheritdoc />
        Public Overrides Sub OnBootstrap(
                                         <ValidatedNotNull> ByVal context As IPluginBootstrapContext)
            MyBase.OnBootstrap(context)

            context.Logging.Application.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnBootstrap)} writing to the application log!")
            context.Logging.Plugin.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnBootstrap)} writing to the plugin log!")
            Dim createdNew As Boolean = Nothing
            mMutex = New Mutex(True, "testplugin", createdNew)

            If Not createdNew Then
                Environment.[Exit](-1)
            End If

            Dim exampleCommandLineOptions = context.CommandLine.ParseArguments(Of ExampleCommandLineOptions)()

            If Not exampleCommandLineOptions.ExampleFlag Then
                context.Logging.Plugin.Warn("Client wasn't started with the start-up flag!")
            End If

            context.Logging.Plugin.Info(
                $"{NameOf(exampleCommandLineOptions.ExampleVariable)} = {exampleCommandLineOptions.ExampleVariable}")
        End Sub

        ''' <inheritdoc />
        Public Overrides Sub OnStart(
                                     <ValidatedNotNull> ByVal context As IClientPluginContext)
            context.Logging.Application.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStart)} writing to the application log!")
            context.Logging.Plugin.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStart)} writing to the plugin log!")
            mButtonTexture = context.ContentManager.LoadEmbedded(Of GameTexture)(context, ContentTypes.[Interface],
                                                                                  "join-our-discord.png")
            AddHandler context.Lifecycle.LifecycleChangeState, AddressOf HandleLifecycleChangeState
        End Sub

        ''' <inheritdoc />
        Public Overrides Sub OnStop(
                                    <ValidatedNotNull> ByVal context As IClientPluginContext)
            context.Logging.Application.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStop)} writing to the application log!")
            context.Logging.Plugin.Info(
                $"{NameOf(ExampleClientPluginEntry)}.{NameOf(OnStop)} writing to the plugin log!")
        End Sub

        Private Sub HandleLifecycleChangeState(
                                               <ValidatedNotNull> ByVal context As IClientPluginContext,
                                               <ValidatedNotNull> ByVal lifecycleChangeStateArgs As _
                                                  LifecycleChangeStateArgs)
            Debug.Assert(mButtonTexture IsNot Nothing, NameOf(mButtonTexture) & " != null")
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
                                        <ValidatedNotNull> ByVal context As IClientPluginContext,
                                        <ValidatedNotNull> ByVal activeInterface As IMutableInterface)
            Dim button = activeInterface.Create(Of Button)("DiscordButton")
            Debug.Assert(button IsNot Nothing, NameOf(button) & " != null")
            Dim discordInviteUrl = context.GetTypedConfiguration(Of ExamplePluginConfiguration)()?.DiscordInviteUrl
            AddHandler button.Clicked,
                Sub(sender, args)
                    If String.IsNullOrWhiteSpace(discordInviteUrl) Then
                        context.Logging.Plugin.[Error](
                            $"DiscordInviteUrl configuration property is null/empty/whitespace.")
                        Return
                    End If

                    Process.Start(discordInviteUrl)
                End Sub

            button.SetImage(mButtonTexture, mButtonTexture.Name, Button.ControlState.Normal)
            button.SetSize(mButtonTexture.GetWidth(), mButtonTexture.GetHeight())
            button.CurAlignments?.Add(Alignments.Bottom)
            button.CurAlignments?.Add(Alignments.Right)
            button.ProcessAlignments()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(True)

            If mDisposed Then
                Return
            End If

            If disposing Then
                mMutex.Dispose()
            End If

            mDisposed = True
        End Sub
    End Class
End Namespace

