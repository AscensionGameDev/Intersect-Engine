using System.Globalization;
using System.Runtime.Serialization;

using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class ChangeLocalePacket : IntersectPacket
{
    public ChangeLocalePacket() { }

    public ChangeLocalePacket(CultureInfo cultureInfo) => CultureInfo = cultureInfo;

    public ChangeLocalePacket(string locale) => Locale = locale;

    [IgnoreDataMember]
    public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;

    [IgnoreDataMember]
    public override bool IsValid => CultureInfo == default && base.IsValid;

    [Key(0)]
    public string Locale
    {
        get => CultureInfo.IetfLanguageTag;
        set => CultureInfo = CultureInfo.GetCultureInfo(value);
    }
}
