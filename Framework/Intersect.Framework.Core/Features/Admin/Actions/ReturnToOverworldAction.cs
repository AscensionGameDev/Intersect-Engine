using MessagePack;

namespace Intersect.Admin.Actions;

[MessagePackObject]
public partial class ReturnToOverworldAction : AdminAction
{
    //Parameterless Constructor for MessagePack
    public ReturnToOverworldAction()
    {

    }

    public ReturnToOverworldAction(string name)
    {
        PlayerName = name;
    }

    [Key(1)]
    public override Enums.AdminAction Action { get; } = Enums.AdminAction.ReturnToOverworld;

    [Key(2)]
    public string PlayerName { get; set; }
}
