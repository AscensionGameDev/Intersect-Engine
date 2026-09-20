using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Titles;

/// <summary>
/// Describes a title that can be unlocked and equipped by a player. When equipped, the title's
/// <see cref="DatabaseObject{TObject}.Name"/> and <see cref="Color"/> are shown above the player's head
/// (using the same Header Label mechanism as the "Change Player Label" event command).
/// Titles are regular game objects: they live in the game database and are managed from the editor.
/// </summary>
public partial class TitleDescriptor : DatabaseObject<TitleDescriptor>, IFolderable
{
    [JsonConstructor]
    public TitleDescriptor(Guid id) : base(id)
    {
        Name = "New Title";
    }

    //EF wants NO PARAMETERS!!!!!
    public TitleDescriptor()
    {
        Name = "New Title";
    }

    /// <summary>
    /// Color used to render the title above the player's head.
    /// </summary>
    [NotMapped]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// The database compatible version of <see cref="Color"/>
    /// </summary>
    [Column("Color")]
    [JsonIgnore]
    public string JsonColor
    {
        get => JsonConvert.SerializeObject(Color);
        set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
