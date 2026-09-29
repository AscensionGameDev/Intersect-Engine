using MessagePack;

namespace Intersect.Network.Packets.Client;

/// <summary>
/// Sent by the client right after connecting to tell the server which language its UI is set to (see
/// <see cref="Intersect.Configuration.ClientConfiguration.Language"/>), so the server can pick English
/// translations of event dialogue/quest text when the map/quest author has filled them in.
/// </summary>
[MessagePackObject]
public partial class SetLanguagePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public SetLanguagePacket()
    {
    }

    public SetLanguagePacket(string language)
    {
        Language = language;
    }

    [Key(0)]
    public string Language { get; set; } = string.Empty;
}
