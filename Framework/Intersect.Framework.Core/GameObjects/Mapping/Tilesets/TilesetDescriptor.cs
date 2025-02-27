using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Mapping.Tilesets;

public partial class TilesetDescriptor : DatabaseObject<TilesetDescriptor>
{
    [JsonConstructor]
    public TilesetDescriptor(Guid id) : base(id)
    {
        Name = string.Empty;
    }

    //Ef Parameterless Constructor
    public TilesetDescriptor()
    {
        Name = string.Empty;
    }

    public new string Name
    {
        get => base.Name;
        set => base.Name = value?.Trim().ToLower();
    }
}
