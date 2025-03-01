using MessagePack;

namespace Intersect.Admin.Actions;

[MessagePackObject]
public partial class WarpMeToAction : AdminAction
{
    //Parameterless Constructor for MessagePack
    public WarpMeToAction()
    {

    }

    public WarpMeToAction(string name)
    {
        Name = name;
    }

    [Key(1)]
    public override Enums.AdminAction Action { get; } = Enums.AdminAction.WarpMeTo;

    [Key(2)]
    public string Name { get; set; }
}
