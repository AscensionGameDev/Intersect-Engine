using Intersect.Framework;
using Intersect.Models;

using MessagePack;

namespace Intersect.Network.Packets.Common;

[MessagePackObject]
public class LocaleContentStringPacket : IntersectPacket
{
    [IgnoreMember]
    private LocaleContentString _localeContentString = default!;

    public LocaleContentStringPacket() { }

    public LocaleContentStringPacket(LocaleContentString localeContentString) =>
        LocaleContentString = localeContentString;

    [IgnoreMember]
    public LocaleContentString? LocaleContentString
    {
        get => _localeContentString;
        set
        {
            _localeContentString = value ?? throw new ArgumentNullException(nameof(value));
            Id = _localeContentString.Id;
            Locale = _localeContentString.Locale;
            Value = _localeContentString.Value;
        }
    }

    [IgnoreMember]
    public Id<ContentString> Id
    {
        get => new(IdGuid);
        set => IdGuid = value.Guid;
    }

    [Key(0)]
    private Guid IdGuid { get; set; }

    [Key(1)]
    public string Locale { get; set; }

    [Key(2)]
    public string? Value { get; set; }

    public static implicit operator LocaleContentString?(LocaleContentStringPacket? packet) =>
        packet == default ? default : new LocaleContentString(packet.Id, packet.Locale, packet.Value);

    public static implicit operator LocaleContentStringPacket?(LocaleContentString? @string) =>
        @string == default ? default : new LocaleContentStringPacket(@string);
}
