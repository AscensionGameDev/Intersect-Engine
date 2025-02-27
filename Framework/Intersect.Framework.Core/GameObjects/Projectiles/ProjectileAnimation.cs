namespace Intersect.GameObjects;

public partial class ProjectileAnimation
{
    public Guid AnimationId;

    public bool AutoRotate;

    public int SpawnRange = 1;

    public ProjectileAnimation(Guid animationId, int spawnRange, bool autoRotate)
    {
        AnimationId = animationId;
        SpawnRange = spawnRange;
        AutoRotate = autoRotate;
    }
}