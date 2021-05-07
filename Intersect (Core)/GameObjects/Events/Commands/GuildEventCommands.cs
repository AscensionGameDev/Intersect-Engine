using Intersect.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.GameObjects.Events.Commands
{
    public class CreateGuildCommand : EventCommand
    {

        //For Json Deserialization
        public CreateGuildCommand()
        {
        }

        public CreateGuildCommand(Dictionary<Guid, List<EventCommand>> commandLists)
        {
            for (var i = 0; i < BranchIds.Length; i++)
            {
                BranchIds[i] = Guid.NewGuid();
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.CreateGuild;

        public Guid VariableId { get; set; }

        public Guid[] BranchIds { get; set; } =
            new Guid[2]; //Branch[0] is the event commands to execute when quest is started successfully, Branch[1] is for when it's not.

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
    }

    public class DisbandGuildCommand : EventCommand
    {

        //For Json Deserialization
        public DisbandGuildCommand()
        {
        }

        public DisbandGuildCommand(Dictionary<Guid, List<EventCommand>> commandLists)
        {
            for (var i = 0; i < BranchIds.Length; i++)
            {
                BranchIds[i] = Guid.NewGuid();
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.DisbandGuild;

        public Guid[] BranchIds { get; set; } =
            new Guid[2]; //Branch[0] is the event commands to execute when quest is started successfully, Branch[1] is for when it's not.

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

    public class OpenGuildBankCommand : EventCommand
    {

        public override EventCommandType Type { get; } = EventCommandType.OpenGuildBank;

    }


    public class SetGuildBankSlotsCommand : EventCommand
    {

        public override EventCommandType Type { get; } = EventCommandType.SetGuildBankSlots;

        public VariableTypes VariableType { get; set; }

        public Guid VariableId { get; set; }

    }
}
