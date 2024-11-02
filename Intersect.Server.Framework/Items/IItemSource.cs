namespace Intersect.Server.Framework.Items;

public interface IItemSource
{
    Guid Id { get; }
    ItemSourceType SourceType { get; }
}