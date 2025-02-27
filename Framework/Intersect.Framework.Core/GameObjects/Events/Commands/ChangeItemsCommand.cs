using Intersect.Enums;

namespace Intersect.GameObjects.Events.Commands;

public partial class ChangeItemsCommand : EventCommand
{
    //For Json Deserialization
    public ChangeItemsCommand()
    {
    }

    public ChangeItemsCommand(Dictionary<Guid, List<EventCommand>> commandLists)
    {
        for (var i = 0; i < BranchIds.Length; i++)
        {
            BranchIds[i] = Guid.NewGuid();
            commandLists.Add(BranchIds[i], []);
        }
    }

    public override EventCommandType Type { get; } = EventCommandType.ChangeItems;

    public Guid ItemId { get; set; }

    public bool Add { get; set; } //If !Add then Remove

    /// <summary>
    /// Defines how the server is supposed to handle changing the items of this request.
    /// </summary>
    public ItemHandling ItemHandling { get; set; } = ItemHandling.Normal;

    /// <summary>
    /// Defines whether this event command will use a variable for processing or not.
    /// </summary>
    public bool UseVariable { get; set; } = false;

    /// <summary>
    /// Defines whether the variable used is a Player or Global variable.
    /// </summary>
    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    /// <summary>
    /// The Variable Id to use.
    /// </summary>
    public Guid VariableId { get; set; }

    public int Quantity { get; set; }

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