using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities.Events
{
    public static partial class CommandProcessing
    {
        //Create Guild Command
        private static void ProcessCommand(
            CreateGuildCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var success = false;
            var playerVariable = PlayerVariableBase.Get(command.VariableId);

            // We only accept Strings as our Guild Names!
            if (playerVariable.Type == VariableDataTypes.String)
            {
                // Get our intended guild name
                var gname = player.GetVariable(playerVariable.Id)?.Value.String?.Trim();

                // Can we use this name according to our configuration?
                if (gname != null && gname.Length >= Options.Instance.Guild.MinimumGuildNameSize && gname.Length <= Options.Instance.Guild.MaximumGuildNameSize)
                {
                    // Is the name already in use?
                    if (Guild.GetGuild(gname) == null)
                    {
                        // Is the player already in a guild?
                        if (player.Guild == null)
                        {
                            // Finally, we can actually MAKE this guild happen!
                            var guild = Guild.CreateGuild(player, gname);
                            if (guild != null)
                            {
                                // Send them a welcome message!
                                PacketSender.SendChatMsg(player, Strings.Guilds.Welcome.ToString(gname), ChatMessageType.Guild, CustomColors.Alerts.Success);

                                // Denote that we were successful.
                                success = true;
                            }
                        }
                        else
                        {
                            // This cheeky bugger is already in a guild, tell him so!
                            PacketSender.SendChatMsg(player, Strings.Guilds.AlreadyInGuild, ChatMessageType.Guild, CustomColors.Alerts.Error);
                        }
                    }
                    else
                    {
                        // This name already exists, oh dear!
                        PacketSender.SendChatMsg(player, Strings.Guilds.GuildNameInUse, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                }
                else
                {
                    // Let our player know they need to adjust their name.
                    PacketSender.SendChatMsg(player, Strings.Guilds.VariableNotMatchLength.ToString(Options.Instance.Guild.MinimumGuildNameSize, Options.Instance.Guild.MaximumGuildNameSize), ChatMessageType.Guild, CustomColors.Alerts.Error);
                }
            }
            else
            {
                // Notify the user that something went wrong, the user really shouldn't see this.. Assuming the creator set up his events properly.
                PacketSender.SendChatMsg(player, Strings.Guilds.VariableNotString, ChatMessageType.Guild, CustomColors.Alerts.Error);
            }

            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            }

            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];
            }

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };

            callStack.Push(tmpStack);
        }

        private static void ProcessCommand(
            DisbandGuildCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var success = false;

            // Is this player in a guild?
            if (player.Guild != null)
            {
                // Send the members a notification, then start wiping the guild from existence through sheer willpower!
                PacketSender.SendGuildMsg(player, Strings.Guilds.DisbandGuild.ToString(player.Guild.Name), CustomColors.Alerts.Info);
                Guild.DeleteGuild(player.Guild);

                // :(
                success = true;
            }
            else
            {
                // They're not in a guild.. tell them?
                PacketSender.SendChatMsg(player, Strings.Guilds.NotInGuild, ChatMessageType.Guild, CustomColors.Alerts.Error);
            }

            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            }

            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];
            }

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };

            callStack.Push(tmpStack);
        }

        //Open Guild Bank Command
        private static void ProcessCommand(
            OpenGuildBankCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.OpenBank(true);
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Bank;
        }


        //Open Guild Bank Slots Count Command
        private static void ProcessCommand(
            SetGuildBankSlotsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var quantity = 0;
            switch (command.VariableType)
            {
                case VariableTypes.PlayerVariable:
                    quantity = (int)player.GetVariableValue(command.VariableId).Integer;

                    break;
                case VariableTypes.ServerVariable:
                    quantity = (int)ServerVariableBase.Get(command.VariableId)?.Value.Integer;
                    break;
            }
            var guild = player.Guild;
            if (quantity > 0 && guild != null && guild.BankSlotsCount != quantity)
            {
                guild.ExpandBankSlots(quantity);
            }
        }
    }
}
