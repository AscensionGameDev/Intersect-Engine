﻿namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class InputVariableCommand : EventCommand
{
    //For Json Deserialization
    public InputVariableCommand()
    {
    }

    public InputVariableCommand(Dictionary<Guid, List<EventCommand>> commandLists)
    {
        for (var i = 0; i < BranchIds.Length; i++)
        {
            BranchIds[i] = Guid.NewGuid();
            commandLists.Add(BranchIds[i], []);
        }
    }

    public override EventCommandType Type { get; } = EventCommandType.InputVariable;

    public string Title { get; set; }

    public string Text { get; set; } = string.Empty;

    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    public Guid VariableId { get; set; } = new();

    public long Minimum { get; set; } = 0;

    public long Maximum { get; set; } = 0;

    //Branch[0] is the event commands to execute when the condition is met, Branch[1] is for when it's not.
    public Guid[] BranchIds { get; set; } = new Guid[2];

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