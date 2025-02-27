using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class ShopDescriptor : DatabaseObject<ShopDescriptor>, IFolderable
{
    [NotMapped]
    public List<ShopItemDescriptor> BuyingItems { get; set; } = [];

    [NotMapped]
    public List<ShopItemDescriptor> SellingItems { get; set; } = [];

    [JsonConstructor]
    public ShopDescriptor(Guid id) : base(id)
    {
        Name = "New Shop";
    }

    //EF is so damn picky about its parameters
    public ShopDescriptor()
    {
        Name = "New Shop";
    }

    public bool BuyingWhitelist { get; set; } = true;

    //Spawn Info
    [Column("DefaultCurrency")]
    [JsonProperty]
    public Guid DefaultCurrencyId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public ItemDescriptor DefaultCurrency
    {
        get => ItemDescriptor.Get(DefaultCurrencyId);
        set => DefaultCurrencyId = value?.Id ?? Guid.Empty;
    }

    [Column("BuyingItems")]
    [JsonIgnore]
    public string JsonBuyingItems
    {
        get => JsonConvert.SerializeObject(BuyingItems);
        set => BuyingItems = JsonConvert.DeserializeObject<List<ShopItemDescriptor>>(value);
    }

    [Column("SellingItems")]
    [JsonIgnore]
    public string JsonSellingItems
    {
        get => JsonConvert.SerializeObject(SellingItems);
        set => SellingItems = JsonConvert.DeserializeObject<List<ShopItemDescriptor>>(value);
    }

    public string BuySound { get; set; } = null;

    public string SellSound { get; set; } = null;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}