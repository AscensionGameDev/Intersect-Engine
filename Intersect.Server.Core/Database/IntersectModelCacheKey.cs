namespace Intersect.Server.Database;

internal sealed record IntersectModelCacheKey(Type ContextType, bool DesignTime, IntersectModelOptions ModelOptions);