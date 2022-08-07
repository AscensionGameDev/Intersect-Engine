using System.Globalization;
using System.Runtime.Serialization;

using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class LocalesPacket : IntersectPacket
{
    public LocalesPacket() : this(CultureInfo.CurrentCulture) { }

    public LocalesPacket(
        CultureInfo defaultCultureInfo,
        IEnumerable<CultureInfo>? supportedCultureInfos = default
    )
    {
        DefaultCultureInfo = defaultCultureInfo;
        SupportedCultureInfos = supportedCultureInfos?.ToList() ?? new() { DefaultCultureInfo };
    }

    public LocalesPacket(
        string defaultLocale,
        IEnumerable<string>? supportedLocales = default
    )
    {
        DefaultLocale = defaultLocale;
        SupportedLocales = supportedLocales?.ToList() ?? new() { DefaultLocale };
    }

    [IgnoreDataMember]
    public CultureInfo DefaultCultureInfo { get; set; }

    [Key(0)]
    public string DefaultLocale
    {
        get => DefaultCultureInfo.IetfLanguageTag;
        set => DefaultCultureInfo = CultureInfo.GetCultureInfo(value);
    }

    [IgnoreDataMember]
    public override bool IsValid => DefaultCultureInfo == default && base.IsValid;

    [IgnoreDataMember]
    public List<CultureInfo> SupportedCultureInfos { get; set; }

    [Key(1)]
    public List<string> SupportedLocales
    {
        get => SupportedCultureInfos.Select(cultureInfo => cultureInfo.IetfLanguageTag).ToList();
        set => SupportedCultureInfos = value.Select(ietfLanguageTag => CultureInfo.GetCultureInfo(ietfLanguageTag)).ToList();
    }
}
