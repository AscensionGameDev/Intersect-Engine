using MessagePack;

namespace Intersect.Admin.Actions;

[MessagePackObject]
public partial class KillAction : AdminAction
{
    //Parameterless Constructor for MessagePack
    public KillAction()
    {

    }

    public KillAction(string name)
    {
        Name = name;
    }

    [Key(1)]
    public override Enums.AdminAction Action { get; } = Enums.AdminAction.Kill;

    [Key(2)]
    public string Name { get; set; }
}
