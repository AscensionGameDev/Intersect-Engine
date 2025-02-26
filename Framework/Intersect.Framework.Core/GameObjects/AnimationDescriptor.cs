using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class AnimationDescriptor : DatabaseObject<AnimationDescriptor>, IFolderable
{
    [JsonConstructor]
    public AnimationDescriptor(Guid id) : base(id)
    {
        // TODO: localize this
        Name = "New Animation";
        Lower = new AnimationLayer();
        Upper = new AnimationLayer();
    }

    //EF Parameterless Constructor
    public AnimationDescriptor()
    {
        // TODO: localize this
        Name = "New Animation";
        Lower = new AnimationLayer();
        Upper = new AnimationLayer();
    }

    public AnimationLayer Lower { get; set; }

    public AnimationLayer Upper { get; set; }

    //Misc
    public string Sound { get; set; }

    public bool CompleteSound { get; set; }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
