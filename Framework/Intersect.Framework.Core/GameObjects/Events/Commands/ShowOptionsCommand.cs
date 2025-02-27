namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ShowOptionsCommand : EventCommand
{
    //For Json Deserialization
    public ShowOptionsCommand()
    {
    }

    public ShowOptionsCommand(Dictionary<Guid, List<EventCommand>> commandLists)
    {
        for (var i = 0; i < BranchIds.Length; i++)
        {
            BranchIds[i] = Guid.NewGuid();
            commandLists.Add(BranchIds[i], []);
        }
    }

    public override EventCommandType Type { get; } = EventCommandType.ShowOptions;

    public string Text { get; set; } = string.Empty;

    public string[] Options { get; set; } = new string[4];

    //Id of the command list(s) you follow when a particular option is selected
    public Guid[] BranchIds { get; set; } = new Guid[4];

    public string Face { get; set; } = string.Empty;

    public override string GetCopyData(
        Dictionary<Guid, List<EventCommand>> commandLists,
        Dictionary<Guid, List<EventCommand>> copyLists
    )
    {
        foreach (var branch in BranchIds)
        {
            if (branch != Guid.Empty && commandLists.ContainsKey(branch))
            {
                copyLists.Add(branch, commandLists[branch]);
                foreach (var cmd in commandLists[branch])
                {
                    cmd.GetCopyData(commandLists, copyLists);
                }
            }
        }

        return base.GetCopyData(commandLists, copyLists);
    }

    public override void FixBranchIds(Dictionary<Guid, Guid> idDict)
    {
        for (var i = 0; i < BranchIds.Length; i++)
        {
            if (idDict.ContainsKey(BranchIds[i]))
            {
                BranchIds[i] = idDict[BranchIds[i]];
            }
        }
    }
}