namespace Intersect.Client.Entities.Events;

public partial class Dialog
{
    public Guid EventId;

    public string Face = string.Empty;

    public string Opt1 = string.Empty;

    public string Opt2 = string.Empty;

    public string Opt3 = string.Empty;

    public string Opt4 = string.Empty;

    public string Prompt = string.Empty;

    public int ResponseSent;

    public int Type;
}
