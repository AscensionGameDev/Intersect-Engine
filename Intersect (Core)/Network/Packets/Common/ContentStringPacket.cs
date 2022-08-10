using System.Globalization;

using Intersect.Framework;
using Intersect.Models;

using MessagePack;

namespace Intersect.Network.Packets.Common;

[MessagePackObject]
public class ContentStringPacket : IntersectPacket
{
    [IgnoreMember]
    private ContentString _contentString = default!;

    [IgnoreMember]
    private CultureInfo? _cultureInfo;

    public ContentStringPacket() { }

    public ContentStringPacket(ContentString contentString)
        : this(contentString, default) { }

    public ContentStringPacket(ContentString contentString, CultureInfo? cultureInfo)
    {
        _cultureInfo = cultureInfo;
        ContentString = contentString;
    }

    [IgnoreMember]
    public ContentString? ContentString
    {
        get => _contentString;
        set
        {
            _contentString = value ?? throw new ArgumentNullException(nameof(value));
            Id = _contentString.Id;
            Comment = _contentString.Comment;
            Localizations = _contentString.Localizations
                .Where(lcs => _cultureInfo == default || lcs.MatchesCulture(_cultureInfo))
                .Select(lcs => new LocaleContentStringPacket(lcs))
                .ToArray();
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
    public string Comment { get; set; }

    [Key(2)]
    public LocaleContentStringPacket[] Localizations { get; set; }

    public static implicit operator ContentString?(ContentStringPacket? packet) =>
        packet == default ? default : new ContentString
        {
            Id = packet.Id,
            Comment = packet.Comment,
            Localizations = packet.Localizations.Select(lcsp => (LocaleContentString)lcsp).ToList(),
        };

    public static implicit operator ContentStringPacket?(ContentString? @string) =>
        @string == default ? default : new ContentStringPacket(@string);
}
