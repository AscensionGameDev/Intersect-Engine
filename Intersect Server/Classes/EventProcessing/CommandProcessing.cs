using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.Events;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Localization;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.EventProcessing
{
    public static class CommandProcessing
    {
        public static void ProcessCommand(EventCommand command, Player player, EventInstance instance)
        {
            var stackInfo = instance.CallStack.Peek();
            stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
            stackInfo.WaitingOnCommand = null;
            stackInfo.BranchIds = null;
            
            ProcessCommand((dynamic)command, player, instance, instance.CallStack.Peek(), instance.CallStack);

            stackInfo.CommandIndex++;
        }

        //Show Text Command
        private static void ProcessCommand(ShowTextCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendEventDialog(player, ParseEventText(command.Text, player, instance), command.Face, instance.PageInstance.Id);
            stackInfo.WaitingForResponse = CommandInstance.EventResponse.Dialogue;
        }

        //Show Options Command
        private static void ProcessCommand(ShowOptionsCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var txt = ParseEventText(command.Text, player, instance);
            var opt1 = ParseEventText(command.Options[0], player, instance);
            var opt2 = ParseEventText(command.Options[1], player, instance);
            var opt3 = ParseEventText(command.Options[2], player, instance);
            var opt4 = ParseEventText(command.Options[3], player, instance);
            PacketSender.SendEventDialog(player, txt, opt1, opt2, opt3, opt4, command.Face, instance.PageInstance.Id);
            stackInfo.WaitingForResponse = CommandInstance.EventResponse.Dialogue;
            stackInfo.WaitingOnCommand = command;
            stackInfo.BranchIds = command.BranchIds;
        }

        //Show Add Chatbox Text Command
        private static void ProcessCommand(AddChatboxTextCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var txt = ParseEventText(command.Text, player, instance);
            var color = Color.FromName(command.Color, Strings.Colors.presets);
            switch (command.Channel)
            {
                case ChatboxChannel.Player:
                    PacketSender.SendPlayerMsg(player.MyClient, txt, color);
                    break;
                case ChatboxChannel.Local:
                    PacketSender.SendProximityMsg(txt, player.MapId, color);
                    break;
                case ChatboxChannel.Global:
                    PacketSender.SendGlobalMsg(txt, color);
                    break;
            }
        }

        //Set Switch Command
        private static void ProcessCommand(SetSwitchCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (command.SwitchType == SwitchTypes.PlayerSwitch)
            {
                player.SetSwitchValue(command.SwitchId, command.Value);
            }
            else if (command.SwitchType == SwitchTypes.ServerSwitch)
            {
                var serverSwitch = ServerSwitchBase.Get(command.SwitchId);
                if (serverSwitch != null)
                {
                    serverSwitch.Value = command.Value;
                    LegacyDatabase.SaveGameDatabase();
                }
            }
        }

        //Set Variable Commands
        private static void ProcessCommand(SetVariableCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                switch (command.ModType)
                {
                    case VariableMods.Set:
                        player.SetVariableValue(command.VariableId, command.Value);
                        break;
                    case VariableMods.Add:
                        player.SetVariableValue(command.VariableId, player.GetVariableValue(command.VariableId) + command.Value);
                        break;
                    case VariableMods.Subtract:
                        player.SetVariableValue(command.VariableId, player.GetVariableValue(command.VariableId) - command.Value);
                        break;
                    case VariableMods.Random:
                        player.SetVariableValue(command.VariableId, Globals.Rand.Next(command.Value, command.HighValue + 1));
                        break;
                }
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                var serverVarible = ServerVariableBase.Get(command.VariableId);
                if (serverVarible != null)
                {
                    switch (command.ModType)
                    {
                        case VariableMods.Set:
                            serverVarible.Value = command.Value;
                            break;
                        case VariableMods.Add:
                            serverVarible.Value += command.Value;
                            break;
                        case VariableMods.Subtract:
                            serverVarible.Value -= command.Value;
                            break;
                        case VariableMods.Random:
                            serverVarible.Value = Globals.Rand.Next(command.Value, command.HighValue + 1);
                            break;
                    }
                }
                LegacyDatabase.SaveGameDatabase();
            }
        }

        //Set Self Switch Command
        private static void ProcessCommand(SetSelfSwitchCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (instance.Global)
            {
                var evts = MapInstance.Get(instance.MapId).GlobalEventInstances.Values.ToList();
                for (int i = 0; i < evts.Count; i++)
                {
                    if (evts[i] != null && evts[i].BaseEvent == instance.BaseEvent)
                    {
                        evts[i].SelfSwitch[command.SwitchId] = command.Value;
                    }
                }
            }
            else
            {
                instance.SelfSwitch[command.SwitchId] = command.Value;
            }
        }

        //Conditional Branch Command
        private static void ProcessCommand(ConditionalBranchCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var success = Conditions.MeetsCondition((dynamic)command.Condition, player, instance, null);
            if (command.Condition.Negated) success = !success;
            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };
            callStack.Push(tmpStack);
        }

        //Exit Event Process Command
        private static void ProcessCommand(ExitEventProcessingCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            callStack.Clear();
        }

        //Label Command
        private static void ProcessCommand(LabelCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            return; //Do Nothing.. just a label
        }

        //Go To Label Command
        private static void ProcessCommand(GoToLabelCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            //Recursively search through commands for the label, and create a brand new call stack based on where that label is located.
            Stack<CommandInstance> newCallStack = LoadLabelCallstack(command.Label, stackInfo.Page);
            if (newCallStack != null)
            {
                callStack = newCallStack;
            }
        }

        //Start Common Event Command
        private static void ProcessCommand(StartCommmonEventCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var commonEvent = EventBase.Get(command.EventId);
            if (commonEvent != null)
            {
                for (int i = 0; i < commonEvent.Pages.Count; i++)
                {
                    if (Conditions.CanSpawnPage(commonEvent.Pages[i],player,instance))
                    {
                        var commonEventStack = new CommandInstance(commonEvent.Pages[i]);
                        callStack.Push(commonEventStack);
                    }
                }
            }
        }

        //Restore Hp Command
        private static void ProcessCommand(RestoreHpCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.RestoreVital(Vitals.Health);
        }

        //Restore Mp Command
        private static void ProcessCommand(RestoreMpCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.RestoreVital(Vitals.Mana);
        }

        //Level Up Command
        private static void ProcessCommand(LevelUpCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.LevelUp();
        }

        //Give Experience Command
        private static void ProcessCommand(GiveExperienceCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.GiveExperience(command.Exp);
        }

        //Change Level Command
        private static void ProcessCommand(ChangeLevelCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.SetLevel(command.Level, true);
        }

        //Change Spells Command
        private static void ProcessCommand(ChangeSpellsCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            //0 is add, 1 is remove
            var success = false;
            if (command.Add) //Try to add a spell
            {
                success = player.TryTeachSpell(new Spell(command.SpellId));
            }
            else
            {
                if (player.FindSpell(command.SpellId) > -1)
                {
                    player.ForgetSpell(player.FindSpell(command.SpellId));
                    success = true;
                }
            }
            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };
            callStack.Push(tmpStack);
        }

        //Change Items Command
        private static void ProcessCommand(ChangeItemsCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var success = false;
            if (command.Add) //Try to give item
            {
                success = player.TryGiveItem(new Item(command.ItemId, command.Quantity));
            }
            else
            {
                success = player.TakeItemsById(command.ItemId, command.Quantity);
            }
            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };
            callStack.Push(tmpStack);
        }

        //Change Sprite Command
        private static void ProcessCommand(ChangeSpriteCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.Sprite = command.Sprite;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Face Command
        private static void ProcessCommand(ChangeFaceCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.Face = command.Face;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Gender Command
        private static void ProcessCommand(ChangeGenderCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.Gender = command.Gender;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Set Access Command (wtf why would we even allow this? lol)
        private static void ProcessCommand(SetAccessCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.MyClient.Access = command.Access;
            PacketSender.SendEntityDataToProximity(player);
            PacketSender.SendPlayerMsg(player.MyClient, Strings.Player.powerchanged, Color.Red);
        }

        //Warp Player Command
        private static void ProcessCommand(WarpCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.Warp(command.MapId,command.X,command.Y,command.Direction == WarpDirection.Retain ? player.Dir : (int)command.Direction - 1);
        }

        //Set Move Route Command
        private static void ProcessCommand(SetMoveRouteCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (command.Route.Target == Guid.Empty)
            {
                player.MoveRoute = new EventMoveRoute();
                player.MoveRouteSetter = instance.PageInstance;
                player.MoveRoute.CopyFrom(command.Route);
                PacketSender.SendMoveRouteToggle(player.MyClient, true);
            }
            else
            {
                foreach (var evt in player.EventLookup.Values)
                {
                    if (evt.BaseEvent.Id == command.Route.Target)
                    {
                        if (evt.PageInstance != null)
                        {
                            evt.PageInstance.MoveRoute.CopyFrom(command.Route);
                            evt.PageInstance.MovementType = EventMovementType.MoveRoute;
                            if (evt.PageInstance.GlobalClone != null)
                                evt.PageInstance.GlobalClone.MovementType = EventMovementType.MoveRoute;
                        }
                    }
                }
            }
        }

        //Wait for Route Completion Command
        private static void ProcessCommand(WaitForRouteCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (command.TargetId == Guid.Empty)
            {
                stackInfo.WaitingForRoute = player.Id;
                stackInfo.WaitingForRouteMap = player.MapId;
            }
            else
            {
                foreach (var evt in player.EventLookup.Values)
                {
                    if (evt.BaseEvent.Id == command.TargetId)
                    {
                        stackInfo.WaitingForRoute = evt.BaseEvent.Id;
                        stackInfo.WaitingForRouteMap = evt.MapId;
                        break;
                    }
                }
            }
        }

        //Spawn Npc Command
        private static void ProcessCommand(SpawnNpcCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var npcId = command.NpcId;
            var mapId = command.MapId;
            var tileX = 0;
            var tileY = 0;
            var direction = (int)Directions.Up;
            var targetEntity = (EntityInstance) player;
            if (mapId != Guid.Empty)
            {
                tileX = command.X;
                tileY = command.Y;
                direction = command.Dir;
            }
            else
            {
                if (command.EntityId != Guid.Empty)
                {
                    foreach (var evt in player.EventLookup.Values)
                    {
                        if (evt.MapId != instance.MapId) continue;
                        if (evt.BaseEvent.Id == command.EntityId)
                        {
                            targetEntity = evt.PageInstance;
                            break;
                        }
                    }
                }
                if (targetEntity != null)
                {
                    int xDiff = command.X;
                    int yDiff = command.Y;
                    if (command.Dir == 1)
                    {
                        int tmp = 0;
                        switch (targetEntity.Dir)
                        {
                            case (int) Directions.Down:
                                yDiff *= -1;
                                xDiff *= -1;
                                break;
                            case (int) Directions.Left:
                                tmp = yDiff;
                                yDiff = xDiff;
                                xDiff = tmp;
                                break;
                            case (int) Directions.Right:
                                tmp = yDiff;
                                yDiff = xDiff;
                                xDiff = -tmp;
                                break;
                        }
                        direction = targetEntity.Dir;
                    }
                    mapId = targetEntity.MapId;
                    tileX = targetEntity.X + xDiff;
                    tileY = targetEntity.Y + yDiff;
                }
                else
                {
                    return;
                }
            }
            var tile = new TileHelper(mapId, tileX, tileY);
            if (tile.TryFix())
            {
                var npc = MapInstance.Get(mapId).SpawnNpc(tileX, tileY, direction, npcId, true);
                player.SpawnedNpcs.Add((Npc)npc);
            }
        }

        //Despawn Npcs Command
        private static void ProcessCommand(DespawnNpcCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var entities = player.SpawnedNpcs.ToArray();
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null && entities[i].GetType() == typeof(Npc))
                {
                    if (((Npc)entities[i]).Despawnable == true)
                    {
                        ((Npc)entities[i]).Die(100);
                    }
                }
            }
            player.SpawnedNpcs.Clear();
        }

        //Play Animation Command
        private static void ProcessCommand(PlayAnimationCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            //Playing an animations requires a target type/target or just a tile.
            //We need an animation number and whether or not it should rotate (and the direction I guess)
            var animId = command.AnimationId;
            var mapId = command.MapId;
            var tileX = 0;
            var tileY = 0;
            var direction = (int)Directions.Up;
            var targetEntity = (EntityInstance)player;
            if (mapId != Guid.Empty)
            {
                tileX = command.X;
                tileY = command.Y;
                direction = command.Dir;
            }
            else
            {
                if (command.EntityId != Guid.Empty)
                {
                    foreach (var evt in player.EventLookup.Values)
                    {
                        if (evt.MapId != instance.MapId) continue;
                        if (evt.BaseEvent.Id == command.EntityId)
                        {
                            targetEntity = evt.PageInstance;
                            break;
                        }
                    }
                }
                if (targetEntity != null)
                {
                    int xDiff = command.X;
                    int yDiff = command.Y;
                    if (command.Dir == 1)
                    {
                        int tmp = 0;
                        switch (targetEntity.Dir)
                        {
                            case (int)Directions.Down:
                                yDiff *= -1;
                                xDiff *= -1;
                                break;
                            case (int)Directions.Left:
                                tmp = yDiff;
                                yDiff = xDiff;
                                xDiff = tmp;
                                break;
                            case (int)Directions.Right:
                                tmp = yDiff;
                                yDiff = xDiff;
                                xDiff = -tmp;
                                break;
                        }
                        direction = targetEntity.Dir;
                    }
                    mapId = targetEntity.MapId;
                    tileX = targetEntity.X + xDiff;
                    tileY = targetEntity.Y + yDiff;
                }
                else
                {
                    return;
                }
            }
            var tile = new TileHelper(mapId, tileX, tileY);
            if (tile.TryFix())
            {
                PacketSender.SendAnimationToProximity(animId, -1, Guid.Empty, tile.GetMapId(), tile.GetX(), tile.GetY(), direction);
            }
        }

        //Hold Player Command
        private static void ProcessCommand(HoldPlayerCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            instance.HoldingPlayer = true;
            PacketSender.SendHoldPlayer(player.MyClient, instance.BaseEvent.Id, instance.BaseEvent.MapId);
        }

        //Release Player Command
        private static void ProcessCommand(ReleasePlayerCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            instance.HoldingPlayer = false;
            PacketSender.SendReleasePlayer(player.MyClient, instance.BaseEvent.Id);
        }

        //Play Bgm Command
        private static void ProcessCommand(PlayBgmCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendPlayMusic(player.MyClient, command.File);
        }

        //Fadeout Bgm Command
        private static void ProcessCommand(FadeoutBgmCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendFadeMusic(player.MyClient);
        }

        //Play Sound Command
        private static void ProcessCommand(PlaySoundCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendPlaySound(player.MyClient, command.File);
        }

        //Stop Sounds Command
        private static void ProcessCommand(StopSoundsCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendStopSounds(player.MyClient);
        }

        //Show Picture Command
        private static void ProcessCommand(ShowPictureCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendShowPicture(player.MyClient, command.File, command.Size, command.Clickable);
        }

        //Hide Picture Command
        private static void ProcessCommand(HidePictureCommmand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            PacketSender.SendHidePicture(player.MyClient);
        }

        //Wait Command
        private static void ProcessCommand(WaitCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            instance.WaitTimer = Globals.System.GetTimeMs() + command.Time;
        }

        //Open Bank Command
        private static void ProcessCommand(OpenBankCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.OpenBank();
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Bank;
        }

        //Open Shop Command
        private static void ProcessCommand(OpenShopCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.OpenShop(ShopBase.Get(command.ShopId));
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Shop;
        }

        //Open Crafting Table Command
        private static void ProcessCommand(OpenCraftingTableCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.OpenCraftingTable(CraftingTableBase.Get(command.CraftingTableId));
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Crafting;
        }

        //Set Class Command
        private static void ProcessCommand(SetClassCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            if (ClassBase.Get(command.ClassId) != null)
            {
                player.ClassId = command.ClassId;
            }
            PacketSender.SendEntityDataToProximity(player);
        }

        //Start Quest Command
        private static void ProcessCommand(StartQuestCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            var success = false;
            var quest = QuestBase.Get(command.QuestId);
            if (quest != null)
            {
                if (player.CanStartQuest(quest))
                {
                    if (command.Offer)
                    {
                        player.OfferQuest(quest);
                        stackInfo.WaitingForResponse = CommandInstance.EventResponse.Quest;
                        stackInfo.BranchIds = command.BranchIds;
                        stackInfo.WaitingOnCommand = command;
                        return;
                    }
                    else
                    {
                        player.StartQuest(quest);
                        success = true;
                    }
                }
            }
            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            if (!success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1])) newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];

            var tmpStack = new CommandInstance(stackInfo.Page)
            {
                CommandList = newCommandList,
                CommandIndex = 0,
            };
            callStack.Push(tmpStack);
        }

        //Complete Quest Task Command
        private static void ProcessCommand(CompleteQuestTaskCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            player.CompleteQuestTask(command.QuestId, command.TaskId);
        }

        //End Quest Command
        private static void ProcessCommand(EndQuestCommand command, Player player, EventInstance instance, CommandInstance stackInfo, Stack<CommandInstance> callStack)
        {
            //TODO :(
        }

        private static Stack<CommandInstance> LoadLabelCallstack(string label, EventPage currentPage)
        {
            Stack<CommandInstance> newStack = new Stack<CommandInstance>();
            newStack.Push(new CommandInstance(currentPage)); //Start from the top
            if (FindLabelResursive(newStack, currentPage, newStack.Peek().CommandList, label))
            {
                return newStack;
            }
            return null;
        }

        private static bool FindLabelResursive(Stack<CommandInstance> stack, EventPage page, List<EventCommand> commandList, string label)
        {
            if (page.CommandLists.ContainsValue(commandList))
            {
                while (stack.Peek().CommandIndex < commandList.Count)
                {
                    EventCommand command = commandList[stack.Peek().CommandIndex];
                    var branchIds = new List<Guid>();
                    switch (command.Type)
                    {
                        case EventCommandType.ShowOptions:
                            branchIds.AddRange(((ShowOptionsCommand)command).BranchIds);
                            break;
                        case EventCommandType.ConditionalBranch:
                            branchIds.AddRange(((ConditionalBranchCommand)command).BranchIds);
                            break;
                        case EventCommandType.ChangeSpells:
                            branchIds.AddRange(((ChangeSpellsCommand)command).BranchIds);
                            break;
                        case EventCommandType.ChangeItems:
                            branchIds.AddRange(((ChangeItemsCommand)command).BranchIds);
                            break;
                        case EventCommandType.StartQuest:
                            branchIds.AddRange(((StartQuestCommand)command).BranchIds);
                            break;
                        case EventCommandType.Label:
                            //See if we found the label!
                            if (((LabelCommand)command).Label == label)
                            {
                                return true;
                            }
                            break;
                    }
                    //Test each branch
                    foreach (var branch in branchIds)
                    {
                        if (page.CommandLists.ContainsKey(branch))
                        {
                            var tmpStack = new CommandInstance(page)
                            {
                                CommandList = page.CommandLists[branch],
                                CommandIndex = 0,
                            };
                            stack.Peek().CommandIndex++;
                            stack.Push(tmpStack);
                            if (FindLabelResursive(stack,page,tmpStack.CommandList, label)) return true;
                            stack.Peek().CommandIndex--;
                        }
                    }
                    stack.Peek().CommandIndex++;
                }
                stack.Pop(); //We made it through a list
            }
            return false;
        }


        private static string ParseEventText(string input, Player player, EventInstance instance)
        {
            if (player != null)
            {
                input = input.Replace(Strings.Events.playernamecommand, player.Name);
                input = input.Replace(Strings.Events.eventnamecommand, instance.PageInstance.Name);
                input = input.Replace(Strings.Events.commandparameter, instance.PageInstance.Param);
                if (input.Contains(Strings.Events.onlinelistcommand) ||
                    input.Contains(Strings.Events.onlinecountcommand))
                {
                    var onlineList = Globals.OnlineList;
                    input = input.Replace(Strings.Events.onlinecountcommand, onlineList.Count.ToString());
                    var sb = new StringBuilder();
                    for (int i = 0; i < onlineList.Count; i++)
                    {
                        sb.Append(onlineList[i].Name + (i != onlineList.Count - 1 ? ", " : ""));
                    }
                    input = input.Replace(Strings.Events.onlinelistcommand, sb.ToString());
                }

                //Time Stuff
                input = input.Replace(Strings.Events.timehour, ServerTime.GetTime().ToString("%h"));
                input = input.Replace(Strings.Events.militaryhour, ServerTime.GetTime().ToString("HH"));
                input = input.Replace(Strings.Events.timeminute, ServerTime.GetTime().ToString("mm"));
                input = input.Replace(Strings.Events.timesecond, ServerTime.GetTime().ToString("ss"));
                if (ServerTime.GetTime().Hour >= 12)
                {
                    input = input.Replace(Strings.Events.timeperiod, Strings.Events.periodevening);
                }
                else
                {
                    input = input.Replace(Strings.Events.timeperiod, Strings.Events.periodmorning);
                }

                //Have to accept a numeric parameter after each of the following (player switch/var and server switch/var)
                MatchCollection matches = Regex.Matches(input, Regex.Escape(Strings.Events.playervar) + @"{([^}]*)}");
                foreach (Match m in matches)
                {
                    if (m.Success)
                    {
                        var id = m.Groups[1].Value;
                        foreach (var var in PlayerVariableBase.Lookup.Values)
                        {
                            if (id == ((PlayerVariableBase) var).TextId)
                            {
                                input = input.Replace(Strings.Events.playervar + "{" + m.Groups[1].Value + "}", player.GetVariableValue(var.Id).ToString());
                            }
                        }
                    }
                }
                matches = Regex.Matches(input, Regex.Escape(Strings.Events.playerswitch) + @"{([^}]*)}");
                foreach (Match m in matches)
                {
                    if (m.Success)
                    {
                        var id = m.Groups[1].Value;
                        foreach (var var in PlayerSwitchBase.Lookup.Values)
                        {
                            if (id == ((PlayerSwitchBase)var).TextId)
                            {
                                input = input.Replace(Strings.Events.playerswitch + "{" + m.Groups[1].Value + "}", player.GetSwitchValue(var.Id).ToString());
                            }
                        }
                    }
                }
                matches = Regex.Matches(input, Regex.Escape(Strings.Events.globalvar) + @"{([^}]*)}");
                foreach (Match m in matches)
                {
                    if (m.Success)
                    {
                        var id = m.Groups[1].Value;
                        foreach (var var in ServerVariableBase.Lookup.Values)
                        {
                            if (id == ((ServerVariableBase)var).TextId)
                            {
                                input = input.Replace(Strings.Events.globalvar + "{" + m.Groups[1].Value + "}", ((ServerVariableBase)var).Value.ToString());
                            }
                        }
                    }
                }
                matches = Regex.Matches(input, Regex.Escape(Strings.Events.globalswitch) + @"{([^}]*)}");
                foreach (Match m in matches)
                {
                    if (m.Success)
                    {
                        var id = m.Groups[1].Value;
                        foreach (var var in ServerSwitchBase.Lookup.Values)
                        {
                            if (id == ((ServerSwitchBase)var).TextId)
                            {
                                input = input.Replace(Strings.Events.globalswitch + "{" + m.Groups[1].Value + "}", ((ServerSwitchBase)var).Value.ToString());
                            }
                        }
                    }
                }
            }
            return input;
        }
    }
}
