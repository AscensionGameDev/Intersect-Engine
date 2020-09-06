Imports Intersect.Plugins
Imports Newtonsoft.Json

Namespace Intersect.Examples.ClientPlugin
    ''' <summary>
    ''' Example configuration class for a plugin.
    ''' </summary>
    Friend Class ExamplePluginConfiguration
        Inherits PluginConfiguration

        ''' <summary>
        ''' Link to discord invite that should open when discord button is clicked
        ''' </summary>
        <JsonProperty(NullValueHandling := NullValueHandling.Ignore)>
        Public Property DiscordInviteUrl As String = "https://discord.gg/fAwDR5v"
    End Class
End Namespace
