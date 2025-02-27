using System.Text;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps.MapList;
using Microsoft.Extensions.Logging;
using VariableMod = Intersect.GameObjects.Events.VariableMod;

namespace Intersect.Editor.Forms.Editors.Events;


public static partial class CommandPrinter
{

    /// <summary>
    ///     Takes a string and a length value. If the string is longer than the length it will cut the string and add a ...,
    ///     otherwise it will return the original string.
    /// </summary>
    /// <param name="value">String to process.</param>
    /// <param name="maxChars">Max length allowed for the string.</param>
    /// <returns></returns>
    private static string Truncate(string value, int maxChars)
    {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
    }

    /// <summary>
    ///     Recursively prints the referenced command list and all of it's children.
    /// </summary>
    /// <param name="commandList">The command list to print.</param>
    /// <param name="indent">The starting indent of commands in this list.</param>
    public static void PrintCommandList(
        EventPage page,
        List<EventCommand> commandList,
        string indent,
        ListBox lstEventCommands,
        List<CommandListProperties> mCommandProperties,
        MapInstance map
    ) //TODO: How we can simplify and clean this up?
    {
        CommandListProperties clp;
        if (commandList.Count > 0)
        {
            for (var i = 0; i < commandList.Count; i++)
            {
                switch (commandList[i].Type)
                {
                    case EventCommandType.ShowOptions:
                        var cmd = (ShowOptionsCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);
                        for (var x = 0; x < 4; x++)
                        {
                            if (cmd.Options[x].Trim().Length <= 0)
                            {
                                continue;
                            }

                            lstEventCommands.Items.Add(
                                indent +
                                "      : " +
                                Strings.EventCommandList.whenoption.ToString(Truncate(cmd.Options[x], 20))
                            );

                            clp = new CommandListProperties
                            {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList[i].Type,
                                Cmd = commandList[i]
                            };

                            mCommandProperties.Add(clp);
                            PrintCommandList(
                                page, page.CommandLists[cmd.BranchIds[x]], indent + "          ", lstEventCommands,
                                mCommandProperties, map
                            );
                        }

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.endoptions);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    case EventCommandType.InputVariable:
                        var cid = (InputVariableCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        PrintCommandList(
                            page, page.CommandLists[cid.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.conditionalelse);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        PrintCommandList(
                            page, page.CommandLists[cid.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.conditionalend);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    case EventCommandType.ConditionalBranch:
                        var cnd = (ConditionalBranchCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        if ((cnd.BranchIds?.Length ?? 0) < 2)
                        {
                            Intersect.Core.ApplicationContext.Context.Value?.Logger.LogError("Missing branch ids in conditional branch.");
                        }

                        if (!page.CommandLists.TryGetValue(cnd.BranchIds[0], out var branchCommandList))
                        {
                            Intersect.Core.ApplicationContext.Context.Value?.Logger.LogError($"Missing command list for branch {cnd.BranchIds[0]}");
                        }

                        PrintCommandList(
                            page, branchCommandList, indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        if (cnd.Condition.ElseEnabled)
                        {
                            lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.conditionalelse);
                            clp = new CommandListProperties {
                                Editable = false,
                                MyIndex = i,
                                MyList = commandList,
                                Type = commandList[i].Type,
                                Cmd = commandList[i]
                            };

                            mCommandProperties.Add(clp);

                            if (!page.CommandLists.TryGetValue(cnd.BranchIds[1], out branchCommandList))
                            {
                                Intersect.Core.ApplicationContext.Context.Value?.Logger.LogError($"Missing command list for branch {cnd.BranchIds[1]}");
                            }

                            PrintCommandList(
                                page, branchCommandList, indent + "          ", lstEventCommands,
                                mCommandProperties, map
                            );
                        }
                        
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.conditionalend);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    case EventCommandType.ChangeSpells:
                        var spl = (ChangeSpellsCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the spell was successfully taught:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.spellsucceeded);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[spl.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the spell failed to be taught:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.spellfailed);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[spl.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.endspell);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    case EventCommandType.ChangeItems:
                        var itm = (ChangeItemsCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the item(s) were successfully given/taken:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.itemschanged);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[itm.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the items failed to be given/taken:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.itemnotchanged);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[itm.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.enditemchange);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;

                    case EventCommandType.StartQuest:
                        var qst = (StartQuestCommand) commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the quest is accepted/started successfully:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.queststarted);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[qst.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the quest was declined or requirements not met:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.questnotstarted);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[qst.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.endstartquest);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    case EventCommandType.ChangeName:
                        var chna = (ChangeNameCommand)commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic)commandList[i], map)
                        );

                        clp = new CommandListProperties {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the name was successfully changed:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.namesucceeded);
                        clp = new CommandListProperties {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[chna.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the name failed to change:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.namefailed);
                        clp = new CommandListProperties {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[chna.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.endname);
                        clp = new CommandListProperties {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                    default:
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic) commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;

                    case EventCommandType.CreateGuild:
                        var gld = (CreateGuildCommand)commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic)commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the guild is created successfully:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.guildcreated);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[gld.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the guild was not created for any reason:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.guildfailed);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[gld.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.endcreateguild);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;

                    case EventCommandType.DisbandGuild:
                        var gldd = (DisbandGuildCommand)commandList[i];
                        lstEventCommands.Items.Add(
                            indent +
                            Strings.EventCommandList.linestart +
                            GetCommandText((dynamic)commandList[i], map)
                        );

                        clp = new CommandListProperties
                        {
                            Editable = true,
                            MyIndex = i,
                            MyList = commandList,
                            Cmd = commandList[i],
                            Type = commandList[i].Type
                        };

                        mCommandProperties.Add(clp);

                        //When the guild is disbanded successfully:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.guildisbanded);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[gldd.BranchIds[0]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        //When the guild was not disbanded for any reason:
                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.guilddisbandfailed);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);
                        PrintCommandList(
                            page, page.CommandLists[gldd.BranchIds[1]], indent + "          ", lstEventCommands,
                            mCommandProperties, map
                        );

                        lstEventCommands.Items.Add(indent + "      : " + Strings.EventCommandList.enddisbandguild);
                        clp = new CommandListProperties
                        {
                            Editable = false,
                            MyIndex = i,
                            MyList = commandList,
                            Type = commandList[i].Type,
                            Cmd = commandList[i]
                        };

                        mCommandProperties.Add(clp);

                        break;
                }
            }
        }

        lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart);
        clp = new CommandListProperties {Editable = true, MyIndex = -1, MyList = commandList};
        mCommandProperties.Add(clp);
    }

    //TODO: Bring this in and clean up the above later (AFTER we get everything else working and we can actually test our changes.)
    //public static void PrintCommand(List<EventCommand> commandList, EventCommand command, string indent, ListBox lstEventCommands, List<CommandListProperties> mCommandProperties)
    //{
    //    lstEventCommands.Items.Add(indent + Strings.EventCommandList.linestart + GetCommandText(command));
    //    var clp = new CommandListProperties
    //    {
    //        Editable = true,
    //        MyIndex = commandList.IndexOf(command),
    //        MyList = commandList,
    //        Type = command.Type,
    //        Cmd = command
    //    };
    //    mCommandProperties.Add(clp);
    //}

    private static string GetCommandText(ShowTextCommand command, MapInstance map)
    {
        return Strings.EventCommandList.showtext.ToString(Truncate(command.Text, 30));
    }

    private static string GetCommandText(ShowOptionsCommand command, MapInstance map)
    {
        return Strings.EventCommandList.showoptions.ToString(Truncate(command.Text, 30));
    }

    private static string GetCommandText(InputVariableCommand command, MapInstance map)
    {
        return Strings.EventCommandList.variableinput.ToString(Truncate(command.Text, 30));
    }

    private static string GetCommandText(AddChatboxTextCommand command, MapInstance map)
    {
        var channel = string.Empty;
        switch (command.Channel)
        {
            case ChatboxChannel.Player:
                channel += Strings.EventCommandList.chatplayer;

                break;
            case ChatboxChannel.Local:
                channel += Strings.EventCommandList.chatlocal;

                break;
            case ChatboxChannel.Global:
                channel += Strings.EventCommandList.chatglobal;

                break;
        }

        return Strings.EventCommandList.chatboxtext.ToString(channel, command.Color, Truncate(command.Text, 20));
    }

    private static string GetCommandText(SetVariableCommand command, MapInstance map)
    {
        return GetVariableModText(command, (dynamic) command.Modification);
    }

    private static string GetCommandText(SetSelfSwitchCommand command, MapInstance map)
    {
        var selfvalue = string.Empty;
        selfvalue = Strings.EventCommandList.False;
        if (command.Value)
        {
            selfvalue = Strings.EventCommandList.True;
        }

        return Strings.EventCommandList.selfswitch.ToString(
            Strings.EventCommandList.selfswitches[command.SwitchId], selfvalue
        );
    }

    private static string GetCommandText(ConditionalBranchCommand command, MapInstance map)
    {
        if (command.Condition.Negated)
        {
            return Strings.EventCommandList.conditionalbranch.ToString(
                Strings.EventConditionDesc.negated.ToString(
                    Strings.GetEventConditionalDesc((dynamic) command.Condition)
                )
            );
        }
        else
        {
            return Strings.EventCommandList.conditionalbranch.ToString(
                Strings.GetEventConditionalDesc((dynamic) command.Condition)
            );
        }
    }

    private static string GetCommandText(ExitEventProcessingCommand command, MapInstance map)
    {
        return Strings.EventCommandList.exitevent;
    }

    private static string GetCommandText(LabelCommand command, MapInstance map)
    {
        return Strings.EventCommandList.label.ToString(command.Label);
    }

    private static string GetCommandText(GoToLabelCommand command, MapInstance map)
    {
        return Strings.EventCommandList.gotolabel.ToString(command.Label);
    }

    private static string GetCommandText(StartCommmonEventCommand command, MapInstance map)
    {
        if (command.AllInInstance)
        {
            return Strings.EventCommandList.CommonEventInstanced.ToString(EventBase.GetName(command.EventId), command.AllowInOverworld);
        }

        return Strings.EventCommandList.commonevent.ToString(EventBase.GetName(command.EventId));
    }

    private static string GetCommandText(RestoreHpCommand command, MapInstance map)
    {
        if (command.Amount == 0)
        {
            return Strings.EventCommandList.restorehp.ToString();
        }
        else
        {
            return Strings.EventCommandList.restorehpby.ToString(command.Amount);
        }
    }

    private static string GetCommandText(RestoreMpCommand command, MapInstance map)
    {
        if (command.Amount == 0)
        {
            return Strings.EventCommandList.restoremp.ToString();
        }
        else
        {
            return Strings.EventCommandList.restorempby.ToString(command.Amount);
        }
    }

    private static string GetCommandText(LevelUpCommand command, MapInstance map)
    {
        return Strings.EventCommandList.levelup;
    }

    private static string GetCommandText(GiveExperienceCommand command, MapInstance map)
    {
        if (command.UseVariable)
        {
            var exp = string.Empty;
            switch (command.VariableType)
            {
                case VariableType.PlayerVariable:
                    exp = string.Format(@"({0}: {1})", Strings.EventGiveExperience.PlayerVariable, PlayerVariableDescriptor.GetName(command.VariableId));
                    break;
                case VariableType.ServerVariable:
                    exp = string.Format(@"({0}: {1})", Strings.EventGiveExperience.ServerVariable, ServerVariableDescriptor.GetName(command.VariableId));
                    break;
                case VariableType.GuildVariable:
                    exp = string.Format(@"({0}: {1})", Strings.EventGiveExperience.GuildVariable, GuildVariableDescriptor.GetName(command.VariableId));
                    break;
            }

            return Strings.EventCommandList.giveexp.ToString(exp);
        }
        else
        {
            return Strings.EventCommandList.giveexp.ToString(command.Exp);
        }
    }

    private static string GetCommandText(ChangeLevelCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setlevel.ToString(command.Level);
    }

    private static string GetCommandText(ChangeSpellsCommand command, MapInstance map)
    {
        if (command.Add)
        {
            return Strings.EventCommandList.changespells.ToString(
                Strings.EventCommandList.teach.ToString(SpellBase.GetName(command.SpellId))
            );
        }

        return Strings.EventCommandList.changespells.ToString(
            Strings.EventCommandList.forget.ToString(SpellBase.GetName(command.SpellId))
        );
    }

    private static string GetCommandText(ChangeItemsCommand command, MapInstance map)
    {
        if (command.Add)
        {
            return Strings.EventCommandList.changeitems.ToString(
                Strings.EventCommandList.give.ToString(ItemBase.GetName(command.ItemId))
            );
        }

        return Strings.EventCommandList.changeitems.ToString(
            Strings.EventCommandList.take.ToString(ItemBase.GetName(command.ItemId))
        );
    }

    private static string GetCommandText(EquipItemCommand command, MapInstance map)
    {
        var commandText = string.Empty;

        if(!command.Unequip)
        {
            commandText = Strings.EventCommandList.equipitem.ToString(ItemBase.GetName(command.ItemId));
        }
        else
        {
            if (command.IsItem)
            {
                 commandText = Strings.EventCommandList.unequipitem.ToString(ItemBase.GetName(command.ItemId));
            }
            else
            {
                commandText = Strings.EventCommandList.unequipslot.ToString(Options.Instance.Equipment.Slots[command.Slot]);
            }
        }

        return commandText;
    }

    private static string GetCommandText(ChangeSpriteCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setsprite.ToString(command.Sprite);
    }

    private static string GetCommandText(ChangeFaceCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setface.ToString(command.Face);
    }

    private static string GetCommandText(ChangeNameColorCommand command, MapInstance map)
    {
        if (command.Remove)
        {
            return Strings.EventCommandList.removenamecolor.ToString();
        }
        else
        {
            return Strings.EventCommandList.setnamecolor.ToString();
        }
    }

    private static string GetCommandText(ChangePlayerLabelCommand command, MapInstance map)
    {
        return Strings.EventCommandList.changeplayerlabel.ToString(command.Value);
    }

    private static string GetCommandText(ChangeGenderCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setgender.ToString(command.Gender == 0
            ? Strings.EventCommandList.male
            : Strings.EventCommandList.female);
    }

    private static string GetCommandText(SetAccessCommand command, MapInstance map)
    {
        switch (command.Access)
        {
            case Access.None:
                return Strings.EventCommandList.setaccess.ToString(Strings.EventCommandList.regularuser);
            case Access.Moderator:
                return Strings.EventCommandList.setaccess.ToString(Strings.EventCommandList.moderator);
            case Access.Admin:
                return Strings.EventCommandList.setaccess.ToString(Strings.EventCommandList.admin);
        }

        return Strings.EventCommandList.setaccess.ToString(Strings.EventCommandList.unknownrole);
    }

    private static string GetCommandText(WarpCommand command, MapInstance map)
    {
        var mapName = Strings.EventCommandList.mapnotfound;
        for (var i = 0; i < MapList.OrderedMaps.Count; i++)
        {
            if (MapList.OrderedMaps[i].MapId == command.MapId)
            {
                mapName = MapList.OrderedMaps[i].Name;
            }
        }

        if (command.ChangeInstance)
        {
            return Strings.EventCommandList.InstancedWarp.ToString(
                mapName, command.X, command.Y, Strings.Direction.dir[(Direction)command.Direction - 1], command.InstanceType.ToString()
            );
        } else
        {
            return Strings.EventCommandList.warp.ToString(
                mapName, command.X, command.Y, Strings.Direction.dir[(Direction)command.Direction - 1]
            );
        }
    }

    private static string GetCommandText(SetMoveRouteCommand command, MapInstance map)
    {
        if (command.Route.Target == Guid.Empty)
        {
            return Strings.EventCommandList.moveroute.ToString(Strings.EventCommandList.moverouteplayer);
        }
        else
        {
            if (map.LocalEvents.ContainsKey(command.Route.Target))
            {
                return Strings.EventCommandList.moveroute.ToString(
                    Strings.EventCommandList.moverouteevent.ToString(map.LocalEvents[command.Route.Target].Name)
                );
            }
            else
            {
                return Strings.EventCommandList.moveroute.ToString(Strings.EventCommandList.deletedevent);
            }
        }
    }

    private static string GetCommandText(WaitForRouteCommand command, MapInstance map)
    {
        if (command.TargetId == Guid.Empty)
        {
            return Strings.EventCommandList.waitforroute.ToString(Strings.EventCommandList.moverouteplayer);
        }
        else if (map.LocalEvents.ContainsKey(command.TargetId))
        {
            return Strings.EventCommandList.waitforroute.ToString(
                Strings.EventCommandList.moverouteevent.ToString(map.LocalEvents[command.TargetId].Name)
            );
        }
        else
        {
            return Strings.EventCommandList.waitforroute.ToString(Strings.EventCommandList.deletedevent);
        }
    }

    private static string GetCommandText(HoldPlayerCommand command, MapInstance map)
    {
        return Strings.EventCommandList.holdplayer;
    }

    private static string GetCommandText(ReleasePlayerCommand command, MapInstance map)
    {
        return Strings.EventCommandList.releaseplayer;
    }

    private static string GetCommandText(HidePlayerCommand command, MapInstance map)
    {
        return Strings.EventCommandList.hideplayer;
    }

    private static string GetCommandText(ShowPlayerCommand command, MapInstance map)
    {
        return Strings.EventCommandList.showplayer;
    }

    private static string GetCommandText(SpawnNpcCommand command, MapInstance map)
    {
        if (command == null)
        {
            return null;
        }

        if (command.MapId != Guid.Empty)
        {
            foreach (var orderedMap in MapList.OrderedMaps)
            {
                if (orderedMap == null)
                {
                    continue;
                }

                if (orderedMap.MapId == command.MapId)
                {
                    return Strings.EventCommandList.spawnnpc.ToString(
                        NpcBase.GetName(command.NpcId),
                        Strings.EventCommandList.spawnonmap.ToString(
                            orderedMap.Name, command.X, command.Y, Strings.Direction.dir?[command.Dir]
                        )
                    );
                }
            }

            return Strings.EventCommandList.spawnnpc.ToString(
                NpcBase.GetName(command.NpcId),
                Strings.EventCommandList.spawnonmap.ToString(
                    Strings.EventCommandList.mapnotfound, command.X, command.Y, Strings.Direction.dir[command.Dir]
                )
            );
        }

        var retain = Strings.EventCommandList.False;

        //TODO: Possibly bugged -- test this.
        if (Convert.ToBoolean(command.Dir))
        {
            retain = Strings.EventCommandList.True;
        }

        if (command.EntityId == Guid.Empty)
        {
            return Strings.EventCommandList.spawnnpc.ToString(
                NpcBase.GetName(command.NpcId),
                Strings.EventCommandList.spawnonplayer.ToString(command.X, command.Y, retain)
            );
        }

        if (map.LocalEvents.TryGetValue(command.EntityId, out var localEvent))
        {
            return Strings.EventCommandList.spawnnpc.ToString(
                NpcBase.GetName(command.NpcId),
                Strings.EventCommandList.spawnonevent.ToString(localEvent.Name, command.X, command.Y, retain)
            );
        }

        return Strings.EventCommandList.spawnnpc.ToString(
            NpcBase.GetName(command.NpcId),
            Strings.EventCommandList.spawnonevent.ToString(
                Strings.EventCommandList.deletedevent, command.X, command.Y, retain
            )
        );
    }

    private static string GetCommandText(DespawnNpcCommand command, MapInstance map)
    {
        return Strings.EventCommandList.despawnnpcs;
    }

    private static string GetCommandText(PlayAnimationCommand command, MapInstance map)
    {
        StringBuilder commandTextBuilder = new StringBuilder();
        if (command.MapId != Guid.Empty)
        {
            for (var i = 0; i < MapList.OrderedMaps.Count; i++)
            {
                if (MapList.OrderedMaps[i].MapId == command.MapId)
                {
                    commandTextBuilder.Append(Strings.EventCommandList.playanimation.ToString(
                        AnimationDescriptor.GetName(command.AnimationId),
                        Strings.EventCommandList.animationonmap.ToString(
                            MapList.OrderedMaps[i].Name, command.X, command.Y,
                            Strings.Direction.dir[(Direction) command.Dir]
                        )
                    ));
                }
            }

            commandTextBuilder.Append(Strings.EventCommandList.playanimation.ToString(
                AnimationDescriptor.GetName(command.AnimationId),
                Strings.EventCommandList.animationonmap.ToString(
                    Strings.EventCommandList.mapnotfound, command.X, command.Y, Strings.Direction.dir[(Direction)command.Dir]
                )
            ));
        }
        else
        {
            var spawnOpt = string.Empty;
            switch (command.Dir)
            {
                //0 does not adhere to direction, 1 is Spawning Relative to Direction, 2 is Rotating Relative to Direction, and 3 is both.
                case 1:
                    spawnOpt = Strings.EventCommandList.animationrelativedir;

                    break;
                case 2:
                    spawnOpt = Strings.EventCommandList.animationrotatedir;

                    break;
                case 3:
                    spawnOpt = Strings.EventCommandList.animationrelativerotate;

                    break;
            }

            if (command.EntityId == Guid.Empty)
            {
                commandTextBuilder.Append(Strings.EventCommandList.playanimation.ToString(
                    AnimationDescriptor.GetName(command.AnimationId),
                    Strings.EventCommandList.animationonplayer.ToString(command.X, command.Y, spawnOpt)
                ));
            }
            else
            {
                if (map.LocalEvents.ContainsKey(command.EntityId))
                {
                    commandTextBuilder.Append(Strings.EventCommandList.playanimation.ToString(
                        AnimationDescriptor.GetName(command.AnimationId),
                        Strings.EventCommandList.animationonevent.ToString(
                            map.LocalEvents[command.EntityId].Name, command.X, command.Y, spawnOpt
                        )
                    ));
                }
                else
                {
                    commandTextBuilder.Append(Strings.EventCommandList.playanimation.ToString(
                        AnimationDescriptor.GetName(command.AnimationId),
                        Strings.EventCommandList.animationonevent.ToString(
                            Strings.EventCommandList.deletedevent, command.X, command.Y, spawnOpt
                        )
                    ));
                }
            }
        }

        commandTextBuilder.Append(Strings.EventCommandList.PlayAnimationInstanced.ToString(command.InstanceToPlayer));
        return commandTextBuilder.ToString();
    }

    private static string GetCommandText(PlayBgmCommand command, MapInstance map)
    {
        return Strings.EventCommandList.playbgm.ToString(command.File);
    }

    private static string GetCommandText(FadeoutBgmCommand command, MapInstance map)
    {
        return Strings.EventCommandList.fadeoutbgm;
    }

    private static string GetCommandText(PlaySoundCommand command, MapInstance map)
    {
        return Strings.EventCommandList.playsound.ToString(command.File);
    }

    private static string GetCommandText(StopSoundsCommand command, MapInstance map)
    {
        return Strings.EventCommandList.stopsounds;
    }

    private static string GetCommandText(ShowPictureCommand command, MapInstance map)
    {
        return Strings.EventCommandList.showpicture;
    }

    private static string GetCommandText(ChangeNameCommand command, MapInstance map)
    {
        return Strings.EventCommandList.changename.ToString(PlayerVariableDescriptor.GetName(command.VariableId));
    }

    private static string GetCommandText(HidePictureCommmand command, MapInstance map)
    {
        return Strings.EventCommandList.hidepicture;
    }

    private static string GetCommandText(WaitCommand command, MapInstance map)
    {
        return Strings.EventCommandList.wait.ToString(command.Time);
    }

    private static string GetCommandText(OpenBankCommand command, MapInstance map)
    {
        return Strings.EventCommandList.openbank;
    }

    private static string GetCommandText(OpenShopCommand command, MapInstance map)
    {
        return Strings.EventCommandList.openshop.ToString(ShopBase.GetName(command.ShopId));
    }

    private static string GetCommandText(OpenCraftingTableCommand command, MapInstance map)
    {
        return command.JournalMode ?
            Strings.EventCommandList.OpenCraftingJournal.ToString(CraftingTableBase.GetName(command.CraftingTableId)) :
            Strings.EventCommandList.opencrafting.ToString(CraftingTableBase.GetName(command.CraftingTableId));
    }

    private static string GetCommandText(SetClassCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setclass.ToString(ClassDescriptor.GetName(command.ClassId));
    }

    private static string GetCommandText(StartQuestCommand command, MapInstance map)
    {
        if (!command.Offer)
        {
            return Strings.EventCommandList.startquest.ToString(
                QuestBase.GetName(command.QuestId), Strings.EventCommandList.forcedstart
            );
        }
        else
        {
            return Strings.EventCommandList.startquest.ToString(
                QuestBase.GetName(command.QuestId), Strings.EventCommandList.showoffer
            );
        }
    }

    private static string GetCommandText(CompleteQuestTaskCommand command, MapInstance map)
    {
        var quest = QuestBase.Get(command.QuestId);
        if (quest != null)
        {
            //Try to find task
            foreach (var task in quest.Tasks)
            {
                if (task.Id == command.TaskId)
                {
                    return Strings.EventCommandList.completetask.ToString(
                        QuestBase.GetName(command.QuestId), task.GetTaskString(Strings.TaskEditor.descriptions)
                    );
                }
            }
        }

        return Strings.EventCommandList.completetask.ToString(
            QuestBase.GetName(command.QuestId), Strings.EventCommandList.taskundefined
        );
    }

    private static string GetCommandText(EndQuestCommand command, MapInstance map)
    {
        if (!command.SkipCompletionEvent)
        {
            return Strings.EventCommandList.endquest.ToString(
                QuestBase.GetName(command.QuestId), Strings.EventCommandList.runcompletionevent
            );
        }

        return Strings.EventCommandList.endquest.ToString(
            QuestBase.GetName(command.QuestId), Strings.EventCommandList.skipcompletionevent
        );
    }

    private static string GetCommandText(ChangePlayerColorCommand command, MapInstance map)
    {
        return Strings.EventCommandList.ChangePlayerColor.ToString(command.Color.R, command.Color.G, command.Color.B, command.Color.A);
    }

    private static string GetCommandText(CreateGuildCommand command, MapInstance map)
    {
        return Strings.EventCommandList.createguild.ToString(PlayerVariableDescriptor.GetName(command.VariableId));
    }

    private static string GetCommandText(DisbandGuildCommand command, MapInstance map)
    {
        return Strings.EventCommandList.disbandguild;
    }

    private static string GetCommandText(OpenGuildBankCommand command, MapInstance map)
    {
        return Strings.EventCommandList.openguildbank;
    }

    private static string GetCommandText(SetGuildBankSlotsCommand command, MapInstance map)
    {
        return Strings.EventCommandList.setguildbankslots;
    }

    private static string GetCommandText(ResetStatPointAllocationsCommand command, MapInstance map)
    {
        return Strings.EventCommandList.resetstatpointallocations;
    }

    private static string GetCommandText(CastSpellOn command, MapInstance map)
    {
        return Strings.EventCommandList.CastSpellOn.ToString(SpellBase.GetName(command.SpellId), command.Self, command.PartyMembers, command.GuildMembers);
    }

    private static string GetCommandText(ScreenFadeCommand command, MapInstance map)
    {
        Strings.EventScreenFade.FadeTypes.TryGetValue((int)command.FadeType, out var commandType);

        if (command.FadeType == FadeType.None)
        {
            return Strings.EventCommandList.FadeCancel.ToString(commandType, command.WaitForCompletion);
        }
        
        return Strings.EventCommandList.Fade.ToString(commandType, command.WaitForCompletion, command.DurationMs);
    }

    //Set Variable Modification Texts
    private static string GetVariableModText(SetVariableCommand command, VariableMod mod)
    {
        return Strings.EventCommandList.invalid;
    }

    private static string GetVariableModText(SetVariableCommand command, BooleanVariableMod mod)
    {
        var varvalue = string.Empty;
        if (mod.DuplicateVariableId != Guid.Empty)
        {
            if (mod.DupVariableType == VariableType.PlayerVariable)
            {
                varvalue = Strings.EventCommandList.dupplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );
            }
            else if (mod.DupVariableType == VariableType.ServerVariable)
            {
                varvalue = Strings.EventCommandList.dupglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );
            }
            else if (mod.DupVariableType == VariableType.GuildVariable)
            {
                varvalue = Strings.EventCommandList.dupguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );
            }
            else if (mod.DupVariableType == VariableType.UserVariable)
            {
                varvalue = Strings.EventCommandList.DupUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );
            }
        }
        else
        {
            if (mod.Value == true)
            {
                varvalue = Strings.EventCommandList.setvariable.ToString(Strings.EventCommandList.True);
            }
            else
            {
                varvalue = Strings.EventCommandList.setvariable.ToString(Strings.EventCommandList.False);
            }
        }

        if (command.VariableType == VariableType.PlayerVariable)
        {
            return Strings.EventCommandList.playervariable.ToString(
                PlayerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.ServerVariable)
        {
            return Strings.EventCommandList.globalvariable.ToString(
                ServerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.GuildVariable)
        {
            return Strings.EventCommandList.guildvariable.ToString(
                GuildVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.UserVariable)
        {
            return Strings.EventCommandList.UserVariable.ToString(
                Strings.GameObjectStrings.UserVariable,
                UserVariableDescriptor.GetName(command.VariableId),
                varvalue
            );
        }

        return Strings.EventCommandList.invalid;
    }

    private static string GetVariableModText(SetVariableCommand command, IntegerVariableMod mod)
    {
        var varvalue = string.Empty;
        switch (mod.ModType)
        {
            case VariableModType.Set:
                varvalue = Strings.EventCommandList.setvariable.ToString(mod.Value);

                break;
            case VariableModType.Add:
                varvalue = Strings.EventCommandList.addvariable.ToString(mod.Value);

                break;
            case VariableModType.Subtract:
                varvalue = Strings.EventCommandList.subtractvariable.ToString(mod.Value);

                break;
            case VariableModType.Multiply:
                varvalue = Strings.EventCommandList.multiplyvariable.ToString(mod.Value);

                break;
            case VariableModType.Divide:
                varvalue = Strings.EventCommandList.dividevariable.ToString(mod.Value);

                break;
            case VariableModType.LeftShift:
                varvalue = Strings.EventCommandList.leftshiftvariable.ToString(mod.Value);

                break;
            case VariableModType.RightShift:
                varvalue = Strings.EventCommandList.rightshiftvariable.ToString(mod.Value);

                break;
            case VariableModType.Random:
                varvalue = Strings.EventCommandList.randvariable.ToString(mod.Value, mod.HighValue);

                break;
            case VariableModType.SystemTime:
                varvalue = Strings.EventCommandList.systemtimevariable;

                break;


            //Player Variable
            case VariableModType.DupPlayerVar:
                varvalue = Strings.EventCommandList.dupplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.AddPlayerVar:
                varvalue = Strings.EventCommandList.addplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.SubtractPlayerVar:
                varvalue = Strings.EventCommandList.subtractplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.MultiplyPlayerVar:
                varvalue = Strings.EventCommandList.multiplyplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.DividePlayerVar:
                varvalue = Strings.EventCommandList.divideplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.LeftShiftPlayerVar:
                varvalue = Strings.EventCommandList.leftshiftplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.RightShiftPlayerVar:
                varvalue = Strings.EventCommandList.rightshiftplayervariable.ToString(
                    PlayerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;


            //Global Variable
            case VariableModType.DupGlobalVar:
                varvalue = Strings.EventCommandList.dupglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.AddGlobalVar:
                varvalue = Strings.EventCommandList.addglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.SubtractGlobalVar:
                varvalue = Strings.EventCommandList.subtractglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.MultiplyGlobalVar:
                varvalue = Strings.EventCommandList.multiplyglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.DivideGlobalVar:
                varvalue = Strings.EventCommandList.divideglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.LeftShiftGlobalVar:
                varvalue = Strings.EventCommandList.leftshiftglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.RightShiftGlobalVar:
                varvalue = Strings.EventCommandList.rightshiftglobalvariable.ToString(
                    ServerVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;


            //Guilds Variable
            case VariableModType.DupGuildVar:
                varvalue = Strings.EventCommandList.dupguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.AddGuildVar:
                varvalue = Strings.EventCommandList.addguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.SubtractGuildVar:
                varvalue = Strings.EventCommandList.subtractguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.MultiplyGuildVar:
                varvalue = Strings.EventCommandList.multiplyguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.DivideGuildVar:
                varvalue = Strings.EventCommandList.divideguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.LeftShiftGuildVar:
                varvalue = Strings.EventCommandList.leftshiftguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.RightShiftGuildVar:
                varvalue = Strings.EventCommandList.rightshiftguildvariable.ToString(
                    GuildVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;


            //User Variable
            case VariableModType.DuplicateUserVariable:
                varvalue = Strings.EventCommandList.DupUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.AddUserVariable:
                varvalue = Strings.EventCommandList.AddUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.SubtractUserVariable:
                varvalue = Strings.EventCommandList.SubtractUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.MultiplyUserVariable:
                varvalue = Strings.EventCommandList.MultiplyUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.DivideUserVariable:
                varvalue = Strings.EventCommandList.DivideUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.LeftShiftUserVariable:
                varvalue = Strings.EventCommandList.LeftShiftUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
            case VariableModType.RightShiftUserVariable:
                varvalue = Strings.EventCommandList.RightShiftUserVariable.ToString(
                    Strings.GameObjectStrings.UserVariable,
                    UserVariableDescriptor.GetName(mod.DuplicateVariableId)
                );

                break;
        }

        if (command.VariableType == VariableType.PlayerVariable)
        {
            return Strings.EventCommandList.playervariable.ToString(
                PlayerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.ServerVariable)
        {
            return Strings.EventCommandList.globalvariable.ToString(
                ServerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.GuildVariable)
        {
            return Strings.EventCommandList.guildvariable.ToString(
                GuildVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.UserVariable)
        {
            return Strings.EventCommandList.UserVariable.ToString(
                Strings.GameObjectStrings.UserVariable,
                UserVariableDescriptor.GetName(command.VariableId),
                varvalue
            );
        }

        return Strings.EventCommandList.invalid;
    }

    private static string GetVariableModText(SetVariableCommand command, StringVariableMod mod)
    {
        var varvalue = string.Empty;
        switch (mod.ModType)
        {
            case VariableModType.Set:
                varvalue = Strings.EventCommandList.setvariable.ToString(mod.Value);

                break;
            case VariableModType.Replace:
                varvalue = Strings.EventCommandList.replace.ToString(mod.Value, mod.Replace);

                break;
        }

        if (command.VariableType == VariableType.PlayerVariable)
        {
            return Strings.EventCommandList.playervariable.ToString(
                PlayerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.ServerVariable)
        {
            return Strings.EventCommandList.globalvariable.ToString(
                ServerVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.GuildVariable)
        {
            return Strings.EventCommandList.guildvariable.ToString(
                GuildVariableDescriptor.GetName(command.VariableId), varvalue
            );
        }

        if (command.VariableType == VariableType.UserVariable)
        {
            return Strings.EventCommandList.UserVariable.ToString(
                Strings.GameObjectStrings.UserVariable,
                UserVariableDescriptor.GetName(command.VariableId),
                varvalue
            );
        }

        return Strings.EventCommandList.invalid;
    }

}
