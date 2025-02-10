namespace Intersect.Client.Entities.Events;

public partial class Dialog
{
    public Guid EventId;

    public string? Face;

    public string[] Options = [];

    public string? Prompt;

    public bool ResponseSent;
}
