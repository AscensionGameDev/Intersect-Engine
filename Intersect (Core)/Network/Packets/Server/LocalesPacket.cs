using System.Globalization;
using System.Runtime.Serialization;

using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class LocalesPacket : IntersectPacket
{
    public LocalesPacket()
    {
        DefaultCultureInfo = CultureInfo.CurrentCulture;
        SupportedCultureInfos = new() { DefaultCultureInfo };
    }

    public LocalesPacket(IEnumerable<CultureInfo> supportedCultureInfos)
    {

    }

    public LocalesPacket(IEnumerable<string> supportedLocales)
    {

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
