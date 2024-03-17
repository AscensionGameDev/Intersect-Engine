using System;
using System.Collections.Generic;

using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Events.Commands
{
    public abstract partial class EventCommand
    {
        public abstract EventCommandType Type { get; }

        public virtual string GetCopyData(
            Dictionary<Guid, List<EventCommand>> commandLists,
            Dictionary<Guid, List<EventCommand>> copyLists
        )
        {
            return JsonConvert.SerializeObject(
                this,
                typeof(EventCommand),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        public virtual void FixBranchIds(Dictionary<Guid, Guid> idDict)
        {
        }

    }

    public partial class ShowTextCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ShowText;

        public string Text { get; set; } = "";

        public string Face { get; set; } = "";
    }

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
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.ShowOptions;

        public string Text { get; set; } = "";

        public string[] Options { get; set; } = new string[4];

        //Id of the command list(s) you follow when a particular option is selected
        public Guid[] BranchIds { get; set; } = new Guid[4];

        public string Face { get; set; } = "";

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
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.InputVariable;

        public string Title { get; set; }

        public string Text { get; set; } = "";

        public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

        public Guid VariableId { get; set; } = new Guid();

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

    public partial class AddChatboxTextCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.AddChatboxText;

        public string Text { get; set; } = "";

        // TODO: Expose this option to the user?
        public ChatMessageType MessageType { get; set; } = ChatMessageType.Notice;

        public string Color { get; set; } = "";

        public ChatboxChannel Channel { get; set; } = ChatboxChannel.Player;

        public bool ShowChatBubble { get; set; }
    }

    public partial class SetVariableCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetVariable;

        public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

        public Guid VariableId { get; set; }

        public bool SyncParty { get; set; }

        public VariableMod Modification { get; set; }
    }

    public partial class SetSelfSwitchCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetSelfSwitch;

        public int SwitchId { get; set; } //0 through 3

        public bool Value { get; set; }
    }

    public partial class ConditionalBranchCommand : EventCommand
    {
        //For Json Deserialization
        public ConditionalBranchCommand()
        {
        }

        public ConditionalBranchCommand(Dictionary<Guid, List<EventCommand>> commandLists)
        {
            for (var i = 0; i < BranchIds.Length; i++)
            {
                BranchIds[i] = Guid.NewGuid();
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.ConditionalBranch;

        public Condition Condition { get; set; }

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

    public partial class ExitEventProcessingCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ExitEventProcess;
    }

    public partial class LabelCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.Label;

        public string Label { get; set; }
    }

    public partial class GoToLabelCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.GoToLabel;

        public string Label { get; set; }
    }

    public partial class StartCommmonEventCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.StartCommonEvent;

        public bool AllInInstance { get; set;  }

        public bool AllowInOverworld { get; set; }

        public Guid EventId { get; set; }
    }

    public partial class RestoreHpCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.RestoreHp;

        public int Amount { get; set; }
    }

    public partial class RestoreMpCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.RestoreMp;

        public int Amount { get; set; }
    }

    public partial class LevelUpCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.LevelUp;
    }

    public partial class GiveExperienceCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.GiveExperience;

        public long Exp { get; set; }

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
    }

    public partial class ChangeLevelCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ChangeLevel;

        public int Level { get; set; }
    }

    public partial class ChangeSpellsCommand : EventCommand
    {
        //For Json Deserialization
        public ChangeSpellsCommand()
        {
        }

        public ChangeSpellsCommand(Dictionary<Guid, List<EventCommand>> commandLists)
        {
            for (var i = 0; i < BranchIds.Length; i++)
            {
                BranchIds[i] = Guid.NewGuid();
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.ChangeSpells;

        public Guid SpellId { get; set; }

        public bool Add { get; set; } //If !Add then Remove

        public bool RemoveBoundSpell { get; set; }

        //Branch[0] is the event commands to execute when taught/removed successfully, Branch[1] is for when it's not.
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
                commandLists.Add(BranchIds[i], new List<EventCommand>());
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

    public partial class EquipItemCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.EquipItem;

        public Guid ItemId { get; set; }

        public bool Unequip { get; set; }

        public bool TriggerCooldown { get; set; }

    }

    public partial class ChangeSpriteCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ChangeSprite;

        public string Sprite { get; set; } = "";
    }

    public partial class ChangeNameColorCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ChangeNameColor;

        public Color Color { get; set; }

        public bool Override { get; set; }

        public bool Remove { get; set; }
    }

    public partial class ChangePlayerLabelCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.PlayerLabel;

        public string Value { get; set; }

        public int Position { get; set; } //0 = Above Player Name, 1 = Below Player Name

        public Color Color { get; set; }

        public bool MatchNameColor { get; set; }
    }

    public partial class ChangeFaceCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ChangeFace;

        public string Face { get; set; } = "";
    }

    public partial class ChangeGenderCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ChangeGender;

        public Gender Gender { get; set; } = Gender.Male;
    }

    public partial class SetAccessCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetAccess;

        public Access Access { get; set; }
    }

    public partial class WarpCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.WarpPlayer;

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

        public WarpDirection Direction { get; set; } = WarpDirection.Retain;

        /// <summary>
        /// Whether or not the warp event will change a player's map instance settings
        /// </summary>
        public bool ChangeInstance { get; set; } = false;

        /// <summary>
        /// The <see cref="MapInstanceType"/> we are going to be warping to
        /// </summary>
        public MapInstanceType InstanceType { get; set; } = MapInstanceType.Overworld;
    }

    public partial class SetMoveRouteCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetMoveRoute;

        public EventMoveRoute Route { get; set; } = new EventMoveRoute();
    }

    public partial class WaitForRouteCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.WaitForRouteCompletion;

        public Guid TargetId { get; set; }
    }

    public partial class SpawnNpcCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SpawnNpc;

        public Guid NpcId { get; set; }

        public Direction Dir { get; set; }

        //Tile Spawn Variables  (Will spawn on map tile if mapid is not empty)
        public Guid MapId { get; set; }

        //Entity Spawn Variables (Will spawn on/around entity if entityId is not empty)
        public Guid EntityId { get; set; }

        //Map Coords or Coords Centered around player to spawn at
        public sbyte X { get; set; }

        public sbyte Y { get; set; }
    }

    public partial class DespawnNpcCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.DespawnNpc;

        //No parameters, only despawns npcs that have been spawned via events for the player
    }

    public partial class PlayAnimationCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.PlayAnimation;

        public Guid AnimationId { get; set; }

        public byte Dir { get; set; }

        //Tile Spawn Variables  (Will spawn on map tile if mapid is not empty)
        public Guid MapId { get; set; }

        //Entity Spawn Variables (Will spawn on/around entity if entityId is not empty)
        public Guid EntityId { get; set; }

        //Map Coords or Coords Centered around player to spawn at
        public sbyte X { get; set; }

        public sbyte Y { get; set; }

        public bool InstanceToPlayer { get; set; }
    }

    public partial class HoldPlayerCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.HoldPlayer;
    }

    public partial class ReleasePlayerCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ReleasePlayer;
    }

    public partial class HidePlayerCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.HidePlayer;
    }

    public partial class ShowPlayerCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ShowPlayer;
    }

    public partial class PlayBgmCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.PlayBgm;

        public string File { get; set; } = "";
    }

    public partial class FadeoutBgmCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.FadeoutBgm;
    }

    public partial class PlaySoundCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.PlaySound;

        public string File { get; set; } = "";
    }

    public partial class StopSoundsCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.StopSounds;
    }

    public partial class ShowPictureCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ShowPicture;

        /// <summary>
        /// Picture filename to show.
        /// </summary>
        public string File { get; set; } = "";

        /// <summary>
        /// How the picture is rendered on the screen.
        /// </summary>
        public int Size { get; set; } //Original = 0, Full Screen, Half Screen, Stretch To Fit  //TODO Enum this?

        /// <summary>
        /// If true the picture will close upon being clicked
        /// </summary>
        public bool Clickable { get; set; }

        /// <summary>
        /// If not 0 the picture will go away after shown for the time below
        /// </summary>
        public int HideTime { get; set; } = 0;

        /// <summary>
        /// If true this event won't continue with commands until this picture is closed.
        /// </summary>
        public bool WaitUntilClosed { get; set; }
    }

    public partial class HidePictureCommmand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.HidePicture;
    }

    public partial class WaitCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.Wait;

        public int Time { get; set; }
    }

    public partial class OpenBankCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.OpenBank;
    }

    public partial class OpenShopCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.OpenShop;

        public Guid ShopId { get; set; }
    }

    public partial class OpenCraftingTableCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.OpenCraftingTable;

        public Guid CraftingTableId { get; set; }

        /// <summary>
        /// Does not allow crafting, but displays crafts and their requirements.
        /// </summary>
        public bool JournalMode { get; set; }
    }

    public partial class SetClassCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetClass;

        public Guid ClassId { get; set; }
    }

    public partial class StartQuestCommand : EventCommand
    {
        //For Json Deserialization
        public StartQuestCommand()
        {
        }

        public StartQuestCommand(Dictionary<Guid, List<EventCommand>> commandLists)
        {
            for (var i = 0; i < BranchIds.Length; i++)
            {
                BranchIds[i] = Guid.NewGuid();
                commandLists.Add(BranchIds[i], new List<EventCommand>());
            }
        }

        public override EventCommandType Type { get; } = EventCommandType.StartQuest;

        public Guid QuestId { get; set; }

        public bool Offer { get; set; } //Show the offer screen and give the player a chance to decline the quest

        //Branch[0] is the event commands to execute when quest is started successfully, Branch[1] is for when it's not.
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

    public partial class CompleteQuestTaskCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.CompleteQuestTask;

        public Guid QuestId { get; set; }

        public Guid TaskId { get; set; }
    }

    public partial class EndQuestCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.EndQuest;

        public Guid QuestId { get; set; }

        public bool SkipCompletionEvent { get; set; }
    }

    /// <summary>
    /// Defines the Event command partial class for the Change Player Color command.
    /// </summary>
    public partial class ChangePlayerColorCommand : EventCommand
    {
        /// <summary>
        /// The <see cref="EventCommandType"/> of this command.
        /// </summary>
        public override EventCommandType Type { get; } = EventCommandType.ChangePlayerColor;

        /// <summary>
        /// The <see cref="Color"/> to apply to the player.
        /// </summary>
        public Color Color { get; set; } = new Color(255, 255, 255, 255);
    }

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
                commandLists.Add(BranchIds[i], new List<EventCommand>());
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

    public partial class CreateGuildCommand : EventCommand
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

        //Branch[0] is the event commands to execute when quest is started successfully, Branch[1] is for when it's not.
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
    }

    public partial class DisbandGuildCommand : EventCommand
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

        //Branch[0] is the event commands to execute when quest is started successfully, Branch[1] is for when it's not.
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

    public partial class OpenGuildBankCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.OpenGuildBank;
    }

    public partial class SetGuildBankSlotsCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.SetGuildBankSlots;

        public VariableType VariableType { get; set; }

        public Guid VariableId { get; set; }
    }

    public partial class ResetStatPointAllocationsCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.ResetStatPointAllocations;
    }

    public partial class CastSpellOn : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.CastSpellOn;

        public Guid SpellId { get; set; }

        public bool Self { get; set; }

        public bool PartyMembers { get; set; }

        public bool GuildMembers { get; set; }
    }

    public partial class ScreenFadeCommand : EventCommand
    {
        public override EventCommandType Type { get; } = EventCommandType.Fade;

        public FadeType FadeType { get; set; }

        public bool WaitForCompletion { get; set; }

        public int DurationMs { get; set; }
    }
}
