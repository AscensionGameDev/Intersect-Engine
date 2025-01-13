namespace Intersect.Server.Web.RestApi.Types;

public partial struct IdPayload(Guid id)
{
    public Guid Id { get; set; } = id;
}