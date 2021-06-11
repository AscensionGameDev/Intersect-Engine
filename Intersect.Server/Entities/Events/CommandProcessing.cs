using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities.Events
{

    public static partial class CommandProcessing
    {

        public static void ProcessCommand(EventCommand command, Player player, Event instance)
        {
            var stackInfo = instance.CallStack.Peek();
            stackInfo.WaitingForResponse = CommandInstance.EventResponse.None;
            stackInfo.WaitingOnCommand = null;
            stackInfo.BranchIds = null;

            ProcessCommand((dynamic) command, player, instance, instance.CallStack.Peek(), instance.CallStack);

            stackInfo.CommandIndex++;
        }

        //Show Text Command
        private static void ProcessCommand(
            ShowTextCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendEventDialog(
                player, ParseEventText(command.Text, player, instance), command.Face, instance.PageInstance.Id
            );

            stackInfo.WaitingForResponse = CommandInstance.EventResponse.Dialogue;
        }

        //Show Options Command
        private static void ProcessCommand(
            ShowOptionsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
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

        //Input Variable Command
        private static void ProcessCommand(
            InputVariableCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var title = ParseEventText(command.Title, player, instance);
            var txt = ParseEventText(command.Text, player, instance);
            var type = VariableDataTypes.Integer;

            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                var variable = PlayerVariableBase.Get(command.VariableId);
                type = variable.Type;
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                var variable = ServerVariableBase.Get(command.VariableId);
                type = variable.Type;
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                var variable = GuildVariableBase.Get(command.VariableId);
                type = variable.Type;
            }

            PacketSender.SendInputVariableDialog(player, title, txt, type, instance.PageInstance.Id);
            stackInfo.WaitingForResponse = CommandInstance.EventResponse.Dialogue;
            stackInfo.WaitingOnCommand = command;
            stackInfo.BranchIds = command.BranchIds;
        }

        //Show Add Chatbox Text Command
        private static void ProcessCommand(
            AddChatboxTextCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var txt = ParseEventText(command.Text, player, instance);
            var color = Color.FromName(command.Color, Strings.Colors.presets);
            switch (command.Channel)
            {
                case ChatboxChannel.Player:
                    PacketSender.SendChatMsg(player, txt, command.MessageType, color);

                    break;
                case ChatboxChannel.Local:
                    PacketSender.SendProximityMsg(txt, command.MessageType, player.MapId, color);

                    break;
                case ChatboxChannel.Global:
                    PacketSender.SendGlobalMsg(txt, color, string.Empty, command.MessageType);

                    break;
            }
        }

        //Set Variable Commands
        private static void ProcessCommand(
            SetVariableCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            ProcessVariableModification(command, (dynamic) command.Modification, player, instance);
        }

        //Set Self Switch Command
        private static void ProcessCommand(
            SetSelfSwitchCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (instance.Global)
            {
                var evts = MapInstance.Get(instance.MapId).GlobalEventInstances.Values.ToList();
                for (var i = 0; i < evts.Count; i++)
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
        private static void ProcessCommand(
            ConditionalBranchCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var success = Conditions.MeetsCondition(command.Condition, player, instance, null);
            if (command.Condition.Negated)
            {
                success = !success;
            }

            List<EventCommand> newCommandList = null;
            if (success && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[0]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[0]];
            }

            if (!success && command.Condition.ElseEnabled && stackInfo.Page.CommandLists.ContainsKey(command.BranchIds[1]))
            {
                newCommandList = stackInfo.Page.CommandLists[command.BranchIds[1]];
            }

            if (newCommandList != null)
            {
                var tmpStack = new CommandInstance(stackInfo.Page) {
                    CommandList = newCommandList,
                    CommandIndex = 0,
                };

                callStack.Push(tmpStack);
            }
        }

        //Exit Event Process Command
        private static void ProcessCommand(
            ExitEventProcessingCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            callStack.Clear();
        }

        //Label Command
        private static void ProcessCommand(
            LabelCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            return; //Do Nothing.. just a label
        }

        //Go To Label Command
        private static void ProcessCommand(
            GoToLabelCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            //Recursively search through commands for the label, and create a brand new call stack based on where that label is located.
            var newCallStack = LoadLabelCallstack(command.Label, stackInfo.Page);
            if (newCallStack != null)
            {
                newCallStack.Reverse();
                while (callStack.Peek().CommandListId != Guid.Empty)
                {
                    callStack.Pop();
                }

                //also pop the current event
                callStack.Pop();
                foreach (var itm in newCallStack)
                {
                    callStack.Push(itm);
                }
            }
        }

        //Start Common Event Command
        private static void ProcessCommand(
            StartCommmonEventCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var commonEvent = EventBase.Get(command.EventId);
            if (commonEvent != null)
            {
                for (var i = 0; i < commonEvent.Pages.Count; i++)
                {
                    if (Conditions.CanSpawnPage(commonEvent.Pages[i], player, instance))
                    {
                        var commonEventStack = new CommandInstance(commonEvent.Pages[i]);
                        callStack.Push(commonEventStack);
                    }
                }
            }
        }

        //Restore Hp Command
        private static void ProcessCommand(
            RestoreHpCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (command.Amount > 0)
            {
                player.AddVital(Vitals.Health, command.Amount);
            }
            else if (command.Amount < 0)
            {
                player.SubVital(Vitals.Health, -command.Amount);
                player.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
                if (player.GetVital(Vitals.Health) <= 0)
                {
                    lock (player.EntityLock)
                    {
                        player.Die();
                    }
                }
            }
            else
            {
                player.RestoreVital(Vitals.Health);
            }
        }

        //Restore Mp Command
        private static void ProcessCommand(
            RestoreMpCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (command.Amount > 0)
            {
                player.AddVital(Vitals.Mana, command.Amount);
            }
            else if (command.Amount < 0)
            {
                player.SubVital(Vitals.Mana, -command.Amount);
                player.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
            }
            else
            {
                player.RestoreVital(Vitals.Mana);
            }
        }

        //Level Up Command
        private static void ProcessCommand(
            LevelUpCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.LevelUp();
        }

        //Give Experience Command
        private static void ProcessCommand(
            GiveExperienceCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var quantity = command.Exp;
            if (command.UseVariable)
            {
                switch (command.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        quantity = (int)player.GetVariableValue(command.VariableId).Integer;

                        break;
                    case VariableTypes.ServerVariable:
                        quantity = (int)ServerVariableBase.Get(command.VariableId)?.Value.Integer;

                        break;

                    case VariableTypes.GuildVariable:
                        quantity = (int)player.Guild?.GetVariableValue(command.VariableId)?.Integer;

                        break;
                }
            }

            player.GiveExperience(quantity);
        }

        //Change Level Command
        private static void ProcessCommand(
            ChangeLevelCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.SetLevel(command.Level, true);
        }

        //Change Spells Command
        private static void ProcessCommand(
            ChangeSpellsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            //0 is add, 1 is remove
            var success = false;
            if (command.Add) //Try to add a spell
            {
                success = player.TryTeachSpell(new Spell(command.SpellId));
            }
            else
            {
                if (player.FindSpell(command.SpellId) > -1 && command.SpellId != Guid.Empty)
                {
                    player.ForgetSpell(player.FindSpell(command.SpellId));
                    success = true;
                }
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

        //Change Items Command
        private static void ProcessCommand(
            ChangeItemsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var success = false;
            var skip = false;

            // Use the command quantity, unless we're using a variable for input!
            var quantity = command.Quantity;
            if (command.UseVariable)
            {
                switch (command.VariableType)
                {
                    case VariableTypes.PlayerVariable:
                        quantity = (int)player.GetVariableValue(command.VariableId).Integer;

                        break;
                    case VariableTypes.ServerVariable:
                        quantity = (int)ServerVariableBase.Get(command.VariableId)?.Value.Integer;

                        break;
                    case VariableTypes.GuildVariable:
                        quantity = (int)player.Guild?.GetVariableValue(command.VariableId)?.Integer;

                        break;
                }

                // The code further ahead converts 0 to quantity 1, due to some legacy junk where some editors would (maybe still do?) set quantity to 0 for non-stackable items.
                // but if we want to give a player no items through an event we should listen to that.
                if (quantity <= 0)
                {
                    skip = true;
                }
            }

            if (!skip)
            {
                if (command.Add)
                {
                    success = player.TryGiveItem(command.ItemId, quantity, command.ItemHandling);
                }
                else
                {
                    success = player.TryTakeItem(command.ItemId, quantity, command.ItemHandling);
                }
            }
            else
            {
                // If we're skipping, this always succeeds.
                success = true;
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

        //Equip Items Command
        private static void ProcessCommand(
            EquipItemCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var itm = ItemBase.Get(command.ItemId);

            if (itm == null)
            {
                return;
            }

            if (command.Unequip)
            {
                player.UnequipItem(command.ItemId);
            }
            else
            {
                player.EquipItem(ItemBase.Get(command.ItemId));
            }
        }

        //Change Sprite Command
        private static void ProcessCommand(
            ChangeSpriteCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.Sprite = command.Sprite;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Face Command
        private static void ProcessCommand(
            ChangeFaceCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.Face = command.Face;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Gender Command
        private static void ProcessCommand(
            ChangeGenderCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.Gender = command.Gender;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Name Color Command
        private static void ProcessCommand(
            ChangeNameColorCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (command.Remove)
            {
                player.NameColor = null;
                PacketSender.SendEntityDataToProximity(player);

                return;
            }

            //Don't set the name color if it doesn't override admin name colors.
            if (player.Power != UserRights.None && !command.Override)
            {
                return;
            }

            player.NameColor = command.Color;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Change Player Label Command
        private static void ProcessCommand(
            ChangePlayerLabelCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var label = ParseEventText(command.Value, player, instance);

            var color = command.Color;
            if (command.MatchNameColor)
            {
                color = Color.Transparent;
            }

            if (command.Position == 0) // Header
            {
                player.HeaderLabel = new Label(label, color);
            }
            else if (command.Position == 1) // Footer
            {
                player.FooterLabel = new Label(label, color);
            }

            PacketSender.SendEntityDataToProximity(player);
        }

        //Set Access Command (wtf why would we even allow this? lol)
        private static void ProcessCommand(
            SetAccessCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (player.Client == null)
            {
                return;
            }

            switch (command.Access)
            {
                case Access.Moderator:
                    player.Client.Power = UserRights.Moderation;

                    break;
                case Access.Admin:
                    player.Client.Power = UserRights.Admin;

                    break;
                default:
                    player.Client.Power = UserRights.None;

                    break;
            }

            PacketSender.SendEntityDataToProximity(player);
            PacketSender.SendChatMsg(player, Strings.Player.powerchanged, ChatMessageType.Notice ,Color.Red);
        }

        //Warp Player Command
        private static void ProcessCommand(
            WarpCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.Warp(
                command.MapId, command.X, command.Y,
                command.Direction == WarpDirection.Retain ? (byte) player.Dir : (byte) (command.Direction - 1)
            );
        }

        //Set Move Route Command
        private static void ProcessCommand(
            SetMoveRouteCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (command.Route.Target == Guid.Empty)
            {
                player.MoveRoute = new EventMoveRoute();
                player.MoveRouteSetter = instance.PageInstance;
                player.MoveRoute.CopyFrom(command.Route);
                PacketSender.SendMoveRouteToggle(player, true);
            }
            else
            {
                foreach (var evt in player.EventLookup)
                {
                    if (evt.Value.BaseEvent.Id == command.Route.Target)
                    {
                        if (evt.Value.PageInstance != null)
                        {
                            evt.Value.PageInstance.MoveRoute.CopyFrom(command.Route);
                            evt.Value.PageInstance.MovementType = EventMovementType.MoveRoute;
                            if (evt.Value.PageInstance.GlobalClone != null)
                            {
                                evt.Value.PageInstance.GlobalClone.MovementType = EventMovementType.MoveRoute;
                            }
                        }
                    }
                }
            }
        }

        //Wait for Route Completion Command
        private static void ProcessCommand(
            WaitForRouteCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (command.TargetId == Guid.Empty)
            {
                stackInfo.WaitingForRoute = player.Id;
                stackInfo.WaitingForRouteMap = player.MapId;
            }
            else
            {
                foreach (var evt in player.EventLookup)
                {
                    if (evt.Value.BaseEvent.Id == command.TargetId)
                    {
                        stackInfo.WaitingForRoute = evt.Value.BaseEvent.Id;
                        stackInfo.WaitingForRouteMap = evt.Value.MapId;

                        break;
                    }
                }
            }
        }

        //Spawn Npc Command
        private static void ProcessCommand(
            SpawnNpcCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var npcId = command.NpcId;
            var mapId = command.MapId;
            var tileX = 0;
            var tileY = 0;
            var direction = (byte) Directions.Up;
            var targetEntity = (Entity) player;
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
                    foreach (var evt in player.EventLookup)
                    {
                        if (evt.Value.MapId != instance.MapId)
                        {
                            continue;
                        }

                        if (evt.Value.BaseEvent.Id == command.EntityId)
                        {
                            targetEntity = evt.Value.PageInstance;

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
                        var tmp = 0;
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

                        direction = (byte) targetEntity.Dir;
                    }

                    mapId = targetEntity.MapId;
                    tileX = (byte) (targetEntity.X + xDiff);
                    tileY = (byte) (targetEntity.Y + yDiff);
                }
                else
                {
                    return;
                }
            }

            var tile = new TileHelper(mapId, tileX, tileY);
            if (tile.TryFix())
            {
                var npc = MapInstance.Get(mapId).SpawnNpc((byte) tileX, (byte) tileY, direction, npcId, true);
                player.SpawnedNpcs.Add((Npc) npc);
            }
        }

        //Despawn Npcs Command
        private static void ProcessCommand(
            DespawnNpcCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var entities = player.SpawnedNpcs.ToArray();
            for (var i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null && entities[i].GetType() == typeof(Npc))
                {
                    if (((Npc) entities[i]).Despawnable == true)
                    {
                        lock (player.EntityLock)
                        {
                            ((Npc)entities[i]).Die();
                        }
                    }
                }
            }

            player.SpawnedNpcs.Clear();
        }

        //Play Animation Command
        private static void ProcessCommand(
            PlayAnimationCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            //Playing an animations requires a target type/target or just a tile.
            //We need an animation number and whether or not it should rotate (and the direction I guess)
            var animId = command.AnimationId;
            var mapId = command.MapId;
            var tileX = 0;
            var tileY = 0;
            var direction = (byte) Directions.Up;
            var targetEntity = (Entity) player;
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
                    foreach (var evt in player.EventLookup)
                    {
                        if (evt.Value.MapId != instance.MapId)
                        {
                            continue;
                        }

                        if (evt.Value.BaseEvent.Id == command.EntityId)
                        {
                            targetEntity = evt.Value.PageInstance;

                            break;
                        }
                    }
                }

                if (targetEntity != null)
                {
                    if (command.X == 0 && command.Y == 0 && command.Dir == 0)
                    {
                        //Attach to entity instead of playing on tile
                        PacketSender.SendAnimationToProximity(
                            animId, targetEntity.GetEntityType() == EntityTypes.Event ? 2 : 1, targetEntity.Id,
                            targetEntity.MapId, 0, 0, 0
                        );

                        return;
                    }

                    int xDiff = command.X;
                    int yDiff = command.Y;
                    if (command.Dir == 1)
                    {
                        var tmp = 0;
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

                        direction = (byte) targetEntity.Dir;
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
                PacketSender.SendAnimationToProximity(
                    animId, -1, Guid.Empty, tile.GetMapId(), tile.GetX(), tile.GetY(), (sbyte) direction
                );
            }
        }

        //Hold Player Command
        private static void ProcessCommand(
            HoldPlayerCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            instance.HoldingPlayer = true;
            PacketSender.SendHoldPlayer(player, instance.BaseEvent.Id, instance.BaseEvent.MapId);
        }

        //Release Player Command
        private static void ProcessCommand(
            ReleasePlayerCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            instance.HoldingPlayer = false;
            PacketSender.SendReleasePlayer(player, instance.BaseEvent.Id);
        }

        //Hide Player Command
        private static void ProcessCommand(
            HidePlayerCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.HideEntity = true;
            player.HideName = true;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Show Player Command
        private static void ProcessCommand(
            ShowPlayerCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.HideEntity = false;
            player.HideName = false;
            PacketSender.SendEntityDataToProximity(player);
        }

        //Play Bgm Command
        private static void ProcessCommand(
            PlayBgmCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendPlayMusic(player, command.File);
        }

        //Fadeout Bgm Command
        private static void ProcessCommand(
            FadeoutBgmCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendFadeMusic(player);
        }

        //Play Sound Command
        private static void ProcessCommand(
            PlaySoundCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendPlaySound(player, command.File);
        }

        //Stop Sounds Command
        private static void ProcessCommand(
            StopSoundsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendStopSounds(player);
        }

        //Show Picture Command
        private static void ProcessCommand(
            ShowPictureCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var id = Guid.Empty;
            var shouldWait = command.WaitUntilClosed && (command.Clickable || command.HideTime > 0);
            if (shouldWait)
            {
                id = instance.PageInstance.Id;
                stackInfo.WaitingForResponse = CommandInstance.EventResponse.Picture;
            }
            
            PacketSender.SendShowPicture(player, command.File, command.Size, command.Clickable, command.HideTime, id);
        }

        //Hide Picture Command
        private static void ProcessCommand(
            HidePictureCommmand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            PacketSender.SendHidePicture(player);
        }

        //Wait Command
        private static void ProcessCommand(
            WaitCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            instance.WaitTimer = Globals.Timing.Milliseconds + command.Time;
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Timer;
        }

        //Open Bank Command
        private static void ProcessCommand(
            OpenBankCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.OpenBank();
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Bank;
        }

        //Open Shop Command
        private static void ProcessCommand(
            OpenShopCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.OpenShop(ShopBase.Get(command.ShopId));
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Shop;
        }

        //Open Crafting Table Command
        private static void ProcessCommand(
            OpenCraftingTableCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.OpenCraftingTable(CraftingTableBase.Get(command.CraftingTableId));
            callStack.Peek().WaitingForResponse = CommandInstance.EventResponse.Crafting;
        }

        //Set Class Command
        private static void ProcessCommand(
            SetClassCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            if (ClassBase.Get(command.ClassId) != null)
            {
                player.ClassId = command.ClassId;
                player.RecalculateStatsAndPoints();
                player.UnequipInvalidItems();
            }

            PacketSender.SendEntityDataToProximity(player);
        }

        //Start Quest Command
        private static void ProcessCommand(
            StartQuestCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
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

        //Complete Quest Task Command
        private static void ProcessCommand(
            CompleteQuestTaskCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.CompleteQuestTask(command.QuestId, command.TaskId);
        }

        //End Quest Command
        private static void ProcessCommand(
            EndQuestCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.CompleteQuest(command.QuestId, command.SkipCompletionEvent);
        }

        // Change Player Color Command
        private static void ProcessCommand(
            ChangePlayerColorCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            player.Color = command.Color;
            PacketSender.SendEntityDataToProximity(player);
        }

        private static void ProcessCommand(
            ChangeNameCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            var success = false;

            var variable = PlayerVariableBase.Get(command.VariableId);
            if (variable != null)
            {
                if (variable.Type == VariableDataTypes.String)
                {
                    var data = player.GetVariable(variable.Id)?.Value;
                    if (data != null)
                    {
                        success = player.TryChangeName(data.String);
                    }
                }
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
                if (gname != null && FieldChecking.IsValidGuildName(gname, Strings.Regex.guildname))
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
                    PacketSender.SendChatMsg(player, Strings.Guilds.VariableInvalid, ChatMessageType.Guild, CustomColors.Alerts.Error);
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
                Guild.DeleteGuild(player.Guild, player);

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
                case VariableTypes.GuildVariable:
                    quantity = (int)player.Guild?.GetVariableValue(command.VariableId)?.Integer;

                    break;
            }
            var guild = player.Guild;
            if (quantity > 0 && guild != null && guild.BankSlotsCount != quantity)
            {
                guild.ExpandBankSlots(quantity);
            }
        }

        //Reset Stat Point Allocations Command
        private static void ProcessCommand(
            ResetStatPointAllocationsCommand command,
            Player player,
            Event instance,
            CommandInstance stackInfo,
            Stack<CommandInstance> callStack
        )
        {
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                player.StatPointAllocations[i] = 0;
            }
            player.RecalculateStatsAndPoints();
            PacketSender.SendEntityDataToProximity(player);
        }

        private static Stack<CommandInstance> LoadLabelCallstack(string label, EventPage currentPage)
        {
            var newStack = new Stack<CommandInstance>();
            newStack.Push(new CommandInstance(currentPage)); //Start from the top
            if (FindLabelResursive(newStack, currentPage, newStack.Peek().CommandList, label))
            {
                return newStack;
            }

            return null;
        }

        private static bool FindLabelResursive(
            Stack<CommandInstance> stack,
            EventPage page,
            List<EventCommand> commandList,
            string label
        )
        {
            if (page.CommandLists.ContainsValue(commandList))
            {
                while (stack.Peek().CommandIndex < commandList.Count)
                {
                    var command = commandList[stack.Peek().CommandIndex];
                    var branchIds = new List<Guid>();
                    switch (command.Type)
                    {
                        case EventCommandType.ShowOptions:
                            branchIds.AddRange(((ShowOptionsCommand) command).BranchIds);

                            break;
                        case EventCommandType.InputVariable:
                            branchIds.AddRange(((InputVariableCommand) command).BranchIds);

                            break;
                        case EventCommandType.ConditionalBranch:
                            branchIds.AddRange(((ConditionalBranchCommand) command).BranchIds);

                            break;
                        case EventCommandType.ChangeSpells:
                            branchIds.AddRange(((ChangeSpellsCommand) command).BranchIds);

                            break;
                        case EventCommandType.ChangeItems:
                            branchIds.AddRange(((ChangeItemsCommand) command).BranchIds);

                            break;
                        case EventCommandType.StartQuest:
                            branchIds.AddRange(((StartQuestCommand) command).BranchIds);

                            break;
                        case EventCommandType.Label:
                            //See if we found the label!
                            if (((LabelCommand) command).Label == label)
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
                            if (FindLabelResursive(stack, page, tmpStack.CommandList, label))
                            {
                                return true;
                            }

                            stack.Peek().CommandIndex--;
                        }
                    }

                    stack.Peek().CommandIndex++;
                }

                stack.Pop(); //We made it through a list
            }

            return false;
        }

        public static string ParseEventText(string input, Player player, Event instance)
        {
            if (input == null)
            {
                input = "";
            }

            if (player != null && input.Contains("\\"))
            {
                var sb = new StringBuilder(input);
                var time = Time.GetTime();
                var replacements = new Dictionary<string, string>()
                {
                    { Strings.Events.playernamecommand, player.Name },
                    { Strings.Events.playerguildcommand, player.Guild?.Name ?? "" },
                    { Strings.Events.timehour, Time.Hour },
                    { Strings.Events.militaryhour, Time.MilitaryHour },
                    { Strings.Events.timeminute, Time.Minute },
                    { Strings.Events.timesecond, Time.Second },
                    { Strings.Events.timeperiod, time.Hour >= 12 ? Strings.Events.periodevening : Strings.Events.periodmorning },
                    { Strings.Events.onlinecountcommand, Player.OnlineCount.ToString() },
                    { Strings.Events.onlinelistcommand, input.Contains(Strings.Events.onlinelistcommand) ? string.Join(", ", Player.OnlineList.Select(p => p.Name).ToList()) : "" },
                    { Strings.Events.eventnamecommand, instance?.PageInstance?.Name ?? "" },
                    { Strings.Events.commandparameter, instance?.PageInstance?.Param ?? "" },
                    { Strings.Events.eventparams, (instance != null && input.Contains(Strings.Events.eventparams)) ? instance.FormatParameters(player) : "" },

                };

                foreach (var val in replacements)
                {
                    if (input.Contains(val.Key))
                        sb.Replace(val.Key, val.Value);
                }

                foreach (var val in DbInterface.ServerVariableEventTextLookup)
                {
                    if (input.Contains(val.Key))
                        sb.Replace(val.Key, (val.Value).Value.ToString((val.Value).Type));
                }

                foreach (var val in DbInterface.PlayerVariableEventTextLookup)
                {
                    if (input.Contains(val.Key))
                        sb.Replace(val.Key, player.GetVariableValue(val.Value.Id).ToString((val.Value).Type));
                }

                foreach (var val in DbInterface.GuildVariableEventTextLookup)
                {
                    if (input.Contains(val.Key))
                        sb.Replace(val.Key, (player.Guild?.GetVariableValue(val.Value.Id) ?? new VariableValue()).ToString((val.Value).Type));
                }

                if (instance != null)
                {
                    var parms = instance.GetParams(player);
                    foreach (var val in parms)
                    {
                        sb.Replace(Strings.Events.eventparam + "{" + val.Key + "}", val.Value);
                    }
                }

                return sb.ToString();
            }

            return input;
        }

        private static void ProcessVariableModification(
            SetVariableCommand command,
            VariableMod mod,
            Player player,
            Event instance
        )
        {
        }

        private static void ProcessVariableModification(
            SetVariableCommand command,
            BooleanVariableMod mod,
            Player player,
            Event instance
        )
        {
            VariableValue value = null;
            Guild guild = null;
            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                value = player.GetVariableValue(command.VariableId);
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                value = ServerVariableBase.Get(command.VariableId)?.Value;
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                guild = player.Guild;
                if (guild == null)
                {
                    return;
                }
                value = guild.GetVariableValue(command.VariableId) ?? new VariableValue();
            }

            if (value == null)
            {
                value = new VariableValue();
            }

            var originalValue = value.Boolean;

            if (mod.DuplicateVariableId != Guid.Empty)
            {
                if (mod.DupVariableType == VariableTypes.PlayerVariable)
                {
                    value.Boolean = player.GetVariableValue(mod.DuplicateVariableId).Boolean;
                }
                else if (mod.DupVariableType == VariableTypes.ServerVariable)
                {
                    var variable = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (variable != null)
                    {
                        value.Boolean = ServerVariableBase.Get(mod.DuplicateVariableId).Value.Boolean;
                    }
                }
                else if (mod.DupVariableType == VariableTypes.GuildVariable)
                {
                    if (player.Guild != null)
                    {
                        value.Boolean = player.Guild.GetVariableValue(mod.DuplicateVariableId).Boolean;
                    }
                }
            }
            else
            {
                value.Boolean = mod.Value;
            }

            var changed = value.Boolean != originalValue;

            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                if (changed)
                {

                }

                // Set the party member switches too if Sync Party enabled!
                if (command.SyncParty)
                {
                    if (changed)
                    {
                        player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", command.VariableId.ToString());
                    }

                    foreach (var partyMember in player.Party)
                    {
                        if (partyMember != player)
                        {
                            partyMember.SetSwitchValue(command.VariableId, mod.Value);
                        }
                    }
                }
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                if (changed)
                {
                    Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, "", command.VariableId.ToString());
                    DbInterface.UpdatedServerVariables.AddOrUpdate(command.VariableId, ServerVariableBase.Get(command.VariableId), (key, oldValue) => ServerVariableBase.Get(command.VariableId));
                }
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                if (changed)
                {
                    guild.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.GuildVariableChange, "", command.VariableId.ToString());
                    guild.UpdatedVariables.AddOrUpdate(command.VariableId, GuildVariableBase.Get(command.VariableId), (key, oldValue) => GuildVariableBase.Get(command.VariableId));
                }
            }
        }

        private static void ProcessVariableModification(
            SetVariableCommand command,
            IntegerVariableMod mod,
            Player player,
            Event instance
        )
        {
            VariableValue value = null;
            Guild guild = null;
            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                value = player.GetVariableValue(command.VariableId);
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                value = ServerVariableBase.Get(command.VariableId)?.Value;
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                guild = player.Guild;
                if (guild == null)
                {
                    return;
                }
                value = guild.GetVariableValue(command.VariableId);
            }

            if (value == null)
            {
                value = new VariableValue();
            }

            var originalValue = value.Integer;

            switch (mod.ModType)
            {
                case Enums.VariableMods.Set:
                    value.Integer = mod.Value;

                    break;
                case Enums.VariableMods.Add:
                    value.Integer += mod.Value;

                    break;
                case Enums.VariableMods.Subtract:
                    value.Integer -= mod.Value;

                    break;
                case Enums.VariableMods.Multiply:
                    value.Integer *= mod.Value;

                    break;
                case Enums.VariableMods.Divide:
                    if (mod.Value != 0)  //Idiot proofing divide by 0 LOL
                    {
                        value.Integer /= mod.Value;
                    }

                    break;
                case Enums.VariableMods.LeftShift:
                    value.Integer = value.Integer << (int)mod.Value;

                    break;
                case Enums.VariableMods.RightShift:
                    value.Integer = value.Integer >> (int)mod.Value;

                    break;
                case Enums.VariableMods.Random:
                    //TODO: Fix - Random doesnt work with longs lolz
                    value.Integer = Randomization.Next((int) mod.Value, (int) mod.HighValue + 1);

                    break;
                case Enums.VariableMods.SystemTime:
                    var ms = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                        .TotalMilliseconds;

                    value.Integer = ms;

                    break;
                case Enums.VariableMods.DupPlayerVar:
                    value.Integer = player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.DupGlobalVar:
                    var dupServerVariable = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (dupServerVariable != null)
                    {
                        value.Integer = dupServerVariable.Value.Integer;
                    }

                    break;
                case Enums.VariableMods.AddPlayerVar:
                    value.Integer += player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.AddGlobalVar:
                    var asv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (asv != null)
                    {
                        value.Integer += asv.Value.Integer;
                    }

                    break;
                case Enums.VariableMods.SubtractPlayerVar:
                    value.Integer -= player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.SubtractGlobalVar:
                    var ssv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (ssv != null)
                    {
                        value.Integer -= ssv.Value.Integer;
                    }

                    break;
                case Enums.VariableMods.MultiplyPlayerVar:
                    value.Integer *= player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.MultiplyGlobalVar:
                    var msv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (msv != null)
                    {
                        value.Integer *= msv.Value.Integer;
                    }

                    break;
                case Enums.VariableMods.DividePlayerVar:
                    if (player.GetVariableValue(mod.DuplicateVariableId).Integer != 0) //Idiot proofing divide by 0 LOL
                    {
                        value.Integer /= player.GetVariableValue(mod.DuplicateVariableId).Integer;
                    }

                    break;
                case Enums.VariableMods.DivideGlobalVar:
                    var dsv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (dsv != null)
                    {
                        if (dsv.Value != 0) //Idiot proofing divide by 0 LOL
                        {
                            value.Integer /= dsv.Value.Integer;
                        }
                    }

                    break;
                case Enums.VariableMods.LeftShiftPlayerVar:
                    value.Integer = value.Integer << (int)player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.LeftShiftGlobalVar:
                    var lhsv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (lhsv != null)
                    {
                        value.Integer = value.Integer << (int)lhsv.Value.Integer;
                    }

                    break;
                case Enums.VariableMods.RightShiftPlayerVar:
                    value.Integer = value.Integer >> (int)player.GetVariableValue(mod.DuplicateVariableId).Integer;

                    break;
                case Enums.VariableMods.RightShiftGlobalVar:
                    var rhsv = ServerVariableBase.Get(mod.DuplicateVariableId);
                    if (rhsv != null)
                    {
                        value.Integer = value.Integer >> (int)rhsv.Value.Integer;
                    }

                    break;
            }

            var changed = value.Integer != originalValue;

            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                if (changed)
                {
                    player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", command.VariableId.ToString());
                }

                // Set the party member switches too if Sync Party enabled!
                if (command.SyncParty)
                {
                    foreach (var partyMember in player.Party)
                    {
                        if (partyMember != player)
                        {
                            partyMember.SetVariableValue(command.VariableId, value.Integer);
                        }
                    }
                }
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                if (changed)
                {
                    Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, "", command.VariableId.ToString());
                    DbInterface.UpdatedServerVariables.AddOrUpdate(command.VariableId, ServerVariableBase.Get(command.VariableId), (key, oldValue) => ServerVariableBase.Get(command.VariableId));
                }
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                if (changed)
                {
                    guild.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.GuildVariableChange, "", command.VariableId.ToString());
                    guild.UpdatedVariables.AddOrUpdate(command.VariableId, GuildVariableBase.Get(command.VariableId), (key, oldValue) => GuildVariableBase.Get(command.VariableId));
                }
            }
        }

        private static void ProcessVariableModification(
            SetVariableCommand command,
            StringVariableMod mod,
            Player player,
            Event instance
        )
        {
            VariableValue value = null;
            Guild guild = null;
            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                value = player.GetVariableValue(command.VariableId);
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                value = ServerVariableBase.Get(command.VariableId)?.Value;
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                guild = player.Guild;
                if (guild == null)
                {
                    return;
                }
                value = guild.GetVariableValue(command.VariableId);
            }

            if (value == null)
            {
                value = new VariableValue();
            }

            var originalValue = value.String;

            switch (mod.ModType)
            {
                case Enums.VariableMods.Set:
                    value.String = ParseEventText(mod.Value, player, instance);

                    break;
                case Enums.VariableMods.Replace:
                    var find = ParseEventText(mod.Value, player, instance);
                    var replace = ParseEventText(mod.Replace, player, instance);
                    value.String = value.String.Replace(find, replace);

                    break;
            }

            var changed = value.String != originalValue;

            if (command.VariableType == VariableTypes.PlayerVariable)
            {
                if (changed)
                {
                    player.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerVariableChange, "", command.VariableId.ToString());
                }

                // Set the party member switches too if Sync Party enabled!
                if (command.SyncParty)
                {
                    foreach (var partyMember in player.Party)
                    {
                        if (partyMember != player)
                        {
                            partyMember.SetVariableValue(command.VariableId, value.String);
                        }
                    }
                }
            }
            else if (command.VariableType == VariableTypes.ServerVariable)
            {
                if (changed)
                {
                    Player.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.ServerVariableChange, "", command.VariableId.ToString());
                    DbInterface.UpdatedServerVariables.AddOrUpdate(command.VariableId, ServerVariableBase.Get(command.VariableId), (key, oldValue) => ServerVariableBase.Get(command.VariableId));
                }
            }
            else if (command.VariableType == VariableTypes.GuildVariable)
            {
                if (changed)
                {
                    guild.StartCommonEventsWithTriggerForAll(Enums.CommonEventTrigger.GuildVariableChange, "", command.VariableId.ToString());
                    guild.UpdatedVariables.AddOrUpdate(command.VariableId, GuildVariableBase.Get(command.VariableId), (key, oldValue) => GuildVariableBase.Get(command.VariableId));
                }
            }
        }

    }

}
