﻿namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ChangeNameCommand : EventCommand
{
    //For Json Deserialization
    public ChangeNameCommand()
    {
    }

    public ChangeNameCommand(Dictionary<Guid, List<EventCommand>> commandLists)
    {
        for (var i = 0; i < BranchIds.Length; i++)
        {
            BranchIds[i] = Guid.NewGuid();
            commandLists.Add(BranchIds[i], []);
        }
    }

    public override EventCommandType Type { get; } = EventCommandType.ChangeName;

    public Guid VariableId { get; set; }

    //Branch[0] is the event commands to execute when given/taken successfully, Branch[1] is for when they're not.
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