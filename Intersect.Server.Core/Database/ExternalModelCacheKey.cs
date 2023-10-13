namespace Intersect.Server.Database;

internal sealed record ExternalModelCacheKey(Type ContextType, bool DesignTime);