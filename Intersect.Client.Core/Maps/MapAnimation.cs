using Intersect.Client.Entities;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Maps;
using Intersect.GameObjects;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Animations;

namespace Intersect.Client.Maps;


public partial class MapAnimation : Animation, IMapAnimation
{
    public Guid Id { get; } = Guid.NewGuid();

    private Direction mDir;

    private int mTileX;

    private int mTileY;

    public MapAnimation(
        AnimationBase animBase,
        int tileX,
        int tileY,
        Direction dir,
        IEntity? owner = null,
        AnimationSource source = default
    ) : base(
        animBase,
        false,
        false,
        -1,
        owner,
        source: source
    )
    {
        mTileX = tileX;
        mTileY = tileY;
        mDir = dir;
    }
}
