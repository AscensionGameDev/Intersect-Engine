using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.EventArguments.InputSubmissionEvent;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Items;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.EntityPanel;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Config.Guilds;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Extensions;
using Intersect.Framework.Core;
using Intersect.Framework.Reflection;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Player : Entity, IPlayer
{
    public delegate void InventoryUpdatedEventHandler(Player player, int slotIndex);

    private Guid _class;

    public Guid Class
    {
        get => _class;
        set
        {
            if (_class == value)
            {
                return;
            }

            _class = value;
            LoadAnimationTexture(string.IsNullOrWhiteSpace(TransformedSprite) ? Sprite : TransformedSprite, SpriteAnimations.Attack);
        }
    }

    public Access AccessLevel { get; set; }

    public long Experience { get; set; } = 0;

    public long ExperienceToNextLevel { get; set; } = 0;

    IReadOnlyList<IFriendInstance> IPlayer.Friends => Friends;

    public List<IFriendInstance> Friends { get; set; } = [];

    IReadOnlyList<IHotbarInstance> IPlayer.HotbarSlots => Hotbar.ToList();

    public HotbarInstance[] Hotbar { get; set; } = new HotbarInstance[Options.Instance.Player.HotbarSlotCount];

    public event InventoryUpdatedEventHandler? InventoryUpdated;

    public void UpdateInventory(
        int slotIndex,
        Guid descriptorId,
        int quantity,
        Guid? bagId,
        ItemProperties itemProperties
    )
    {
        Inventory[slotIndex].Load(id: descriptorId, quantity: quantity, bagId: bagId, itemProperties: itemProperties);
        InventoryUpdated?.Invoke(this, slotIndex);
    }

    IReadOnlyDictionary<Guid, long> IPlayer.ItemCooldowns => ItemCooldowns;

    public Dictionary<Guid, long> ItemCooldowns { get; set; } = [];

    private Entity? mLastBumpedEvent;

    private List<IPartyMember>? mParty;

    public override Direction MoveDir
    {
        get => base.MoveDir;
        set
        {
            base.MoveDir = value;
        }
    }

    IReadOnlyDictionary<Guid, QuestProgress> IPlayer.QuestProgress => QuestProgress;

    public Dictionary<Guid, QuestProgress> QuestProgress { get; set; } = [];

    public Guid[] HiddenQuests { get; set; } = [];

    IReadOnlyDictionary<Guid, long> IPlayer.SpellCooldowns => SpellCooldowns;

    public Dictionary<Guid, long> SpellCooldowns { get; set; } = [];

    public int StatPoints { get; set; } = 0;

    public EntityBox? TargetBox { get; set; }

    public PlayerStatusWindow? StatusWindow { get; set; }

    public Guid TargetId { get; set; }

    TargetType IPlayer.TargetType => (TargetType)TargetType;

    public int TargetType { get; set; }

    public long CombatTimer { get; set; }

    public long IsCastingCheckTimer { get; set; }

    public long GlobalCooldown { get; set; }

    // Target data
    private long mlastTargetScanTime;

    Guid mlastTargetScanMap = Guid.Empty;

    Point mlastTargetScanLocation = new(-1, -1);

    readonly Dictionary<Entity, TargetInfo> mlastTargetList = []; // Entity, Last Time Selected

    Entity? mLastEntitySelected;

    private readonly Dictionary<int, long> mLastHotbarUseTime = [];
    private readonly int mHotbarUseDelay = 150;

    /// <summary>
    /// Name of our guild if we are in one.
    /// </summary>
    public string? Guild { get; set; }

    string IPlayer.GuildName => Guild ?? string.Empty;

    /// <summary>
    /// Index of our rank where 0 is the leader
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Returns whether or not we are in a guild by checking to see if we are assigned a guild name
    /// </summary>
    public bool IsInGuild => !string.IsNullOrWhiteSpace(Guild);

    /// <summary>
    /// Obtains our rank and permissions from the game config
    /// </summary>
    public GuildRank? GuildRank => IsInGuild
        ? Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Rank, Options.Instance.Guild.Ranks.Length - 1))]
        : null;

    /// <summary>
    /// Contains a record of all members of this player's guild.
    /// </summary>
    public GuildMember[] GuildMembers = [];

    public Player(Guid id, PlayerEntityPacket packet) : base(id, packet, EntityType.Player)
    {
        for (var i = 0; i < Options.Instance.Player.HotbarSlotCount; i++)
        {
            Hotbar[i] = new HotbarInstance();
        }

        mRenderPriority = 2;
    }

    IReadOnlyList<IPartyMember> IPlayer.PartyMembers => Party;

    public List<IPartyMember> Party
    {
        get
        {
            mParty ??= [];
            return mParty;
        }
    }

    public override Guid MapId
    {
        get => base.MapId;
        set
        {
            if (value != base.MapId)
            {
                var oldMap = Maps.MapInstance.Get(base.MapId);
                var newMap = Maps.MapInstance.Get(value);
                base.MapId = value;
                if (Globals.Me == this)
                {
                    if (Maps.MapInstance.Get(Globals.Me.MapId) != null)
                    {
                        Audio.PlayMusic(Maps.MapInstance.Get(Globals.Me.MapId).Music, ClientConfiguration.Instance.MusicFadeTimer, ClientConfiguration.Instance.MusicFadeTimer, true);
                    }

                    if (newMap != null && oldMap != null)
                    {
                        newMap.CompareEffects(oldMap);
                    }
                }
            }
        }
    }

    public bool IsFriend(IPlayer player)
    {
        return Friends.Any(
            friend => player != null &&
                      string.Equals(player.Name, friend.Name, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool IsGuildMate(IPlayer player)
    {
        return GuildMembers.Any(
            guildMate =>
                player != null &&
                string.Equals(player.Name, guildMate.Name, StringComparison.CurrentCultureIgnoreCase));
    }

    bool IPlayer.IsInParty => IsInParty();

    public bool IsInParty()
    {
        return Party.Count > 0;
    }

    public bool IsInMyParty(IPlayer player) => player != null && IsInMyParty(player.Id);

    public bool IsInMyParty(Guid id) => Party.Any(member => member.Id == id);

    public bool IsInMyGuild(IPlayer player) => IsInGuild && player != null && player.GuildName == Guild;

    public bool IsBusy => !(Globals.EventHolds.Count == 0 &&
                 !Globals.MoveRouteActive &&
                 Globals.GameShop == null &&
                 Globals.InBank == false &&
                 Globals.InCraft == false &&
                 Globals.InTrade == false &&
                 !Interface.Interface.HasInputFocus());

    public override bool Update()
    {

        if (Globals.Me == this)
        {
            HandleInput();
        }

        if (!IsBusy)
        {
            if (this == Globals.Me && IsMoving == false)
            {
                ProcessDirectionalInput();
            }

            if (Controls.IsControlPressed(Control.AttackInteract))
            {
                if (IsCasting)
                {
                    if (IsCastingCheckTimer < Timing.Global.Milliseconds &&
                        Options.Instance.Combat.EnableCombatChatMessages)
                    {
                        ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Combat.AttackWhileCastingDeny,
                            CustomColors.Alerts.Declined, ChatMessageType.Combat));
                        IsCastingCheckTimer = Timing.Global.Milliseconds + 350;
                    }
                }
                else if (Globals.Me?.TryAttack() == false)
                {
                    if (!Globals.Me.IsAttacking && (!IsMoving || Options.Instance.Player.AllowCombatMovement))
                    {
                        Globals.Me.AttackTimer = Timing.Global.Milliseconds + Globals.Me.CalculateAttackTime();
                    }
                }
            }

            //Holding block button for "auto blocking"
            if (Controls.IsControlPressed(Control.Block))
            {
                _ = TryBlock();
            }
        }

        if (TargetBox == default && this == Globals.Me && Interface.Interface.GameUi != default)
        {
            // If for WHATEVER reason the box hasn't been created, create it.
            TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityType.Player, null);
            TargetBox.Hide();
        }
        else if (TargetId != default)
        {
            if (!Globals.Entities.TryGetValue(TargetId, out var foundEntity))
            {
                foundEntity = TargetBox?.MyEntity?.MapInstance?.Entities.FirstOrDefault(entity => entity.Id == TargetId) as Entity;
            }

            if (foundEntity == default || foundEntity.IsHidden || foundEntity.IsStealthed)
            {
                _ = ClearTarget();
            }
        }

        TargetBox?.Update();
        StatusWindow?.Update();

        // Hide our Guild window if we're not in a guild!
        if (this == Globals.Me && string.IsNullOrEmpty(Guild) && Interface.Interface.GameUi != null)
        {
            Interface.Interface.GameUi.HideGuildWindow();
        }

        if (IsBlocking && !IsAttacking && !IsMoving)
        {
            IsBlocking = false;
        }

        var returnval = base.Update();

        return returnval;
    }

    //Loading
    public override void Load(EntityPacket? packet)
    {
        base.Load(packet);
        var playerPacket = packet as PlayerEntityPacket;

        if (playerPacket == default)
        {
            return;
        }

        Gender = playerPacket.Gender;
        Class = playerPacket.ClassId;
        AccessLevel = playerPacket.AccessLevel;
        CombatTimer = playerPacket.CombatTimeRemaining + Timing.Global.Milliseconds;
        Guild = playerPacket.Guild;
        Rank = playerPacket.GuildRank;

        if (playerPacket.Equipment != null)
        {
            if (this == Globals.Me && playerPacket.Equipment.InventorySlots != null)
            {
                MyEquipment = playerPacket.Equipment.InventorySlots;
            }
            else if (playerPacket.Equipment.ItemIds != null)
            {
                Equipment = playerPacket.Equipment.ItemIds;
            }
        }

        if (this == Globals.Me && TargetBox == null && Interface.Interface.GameUi != null)
        {
            TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityType.Player, null);
            TargetBox.Hide();
        }
    }

    //Item Processing
    public void SwapItems(int fromSlotIndex, int toSlotIndex)
    {
        PacketSender.SendSwapInvItems(fromSlotIndex, toSlotIndex);
        var fromSlot = Inventory[fromSlotIndex];
        var toSlot = Inventory[toSlotIndex];

        if (
            fromSlot.ItemId == toSlot.ItemId
            && ItemBase.TryGet(toSlot.ItemId, out var itemInSlot)
            && itemInSlot.IsStackable
            && fromSlot.Quantity < itemInSlot.MaxInventoryStack
            && toSlot.Quantity < itemInSlot.MaxInventoryStack
        )
        {
            var combinedQuantity = fromSlot.Quantity + toSlot.Quantity;
            var toQuantity = Math.Min(itemInSlot.MaxInventoryStack, combinedQuantity);
            var fromQuantity = combinedQuantity - toQuantity;
            toSlot.Quantity = toQuantity;
            fromSlot.Quantity = fromQuantity;
            if (fromQuantity < 1)
            {
                Inventory[fromSlotIndex].ItemId = default;
            }
        }
        else
        {
            Inventory[fromSlotIndex] = toSlot;
            Inventory[toSlotIndex] = fromSlot;
        }
    }

    public void TryDropItem(int inventorySlotIndex)
    {
        var inventorySlot = Inventory[inventorySlotIndex];
        if (!ItemBase.TryGet(inventorySlot.ItemId, out var itemDescriptor))
        {
            return;
        }

        var quantity = inventorySlot.Quantity;
        var canDropMultiple = quantity > 1;
        var inputType = canDropMultiple ? InputType.NumericSliderInput : InputType.YesNo;
        var prompt = canDropMultiple ? Strings.Inventory.DropItemPrompt : Strings.Inventory.DropPrompt;
        _ = new InputBox(
            title: Strings.Inventory.DropItemTitle,
            prompt: prompt.ToString(itemDescriptor.Name),
            inputType: inputType,
            quantity: quantity,
            maximumQuantity: GetQuantityOfItemInInventory(itemDescriptor.Id),
            userData: inventorySlotIndex,
            onSubmit: (sender, args) =>
            {
                if (sender is not InputBox { UserData: int slotIndex })
                {
                    return;
                }

                var promptQuantity = 0;
                switch (args.Value)
                {
                    case BooleanSubmissionValue booleanSubmission:
                        promptQuantity = booleanSubmission.Value ? 1 : 0;
                        break;

                    case NumericalSubmissionValue numericalSubmission:
                        promptQuantity = (int)Math.Round(numericalSubmission.Value);
                        break;
                }

                if (promptQuantity < 1)
                {
                    return;
                }

                // Check if the item can be dropped in multiple quantities or if value is less than or equal to the quantity in the initial slot
                if (!canDropMultiple || promptQuantity <= quantity)
                {
                    PacketSender.SendDropItem(slotIndex, !canDropMultiple ? 1 : promptQuantity);
                    return;
                }

                // Find all slots containing the item.
                var itemSlots = Inventory.Where(s => s.ItemId == itemDescriptor.Id).ToList();

                // Send the drop item packet for the initial slot.
                PacketSender.SendDropItem(slotIndex, quantity);
                promptQuantity -= quantity;
                _ = itemSlots.Remove(inventorySlot); // Remove the initial slot from the list of item slots

                // Iterate through the remaining slots containing the item
                foreach (var slot in itemSlots)
                {
                    var dropAmount = Math.Min(promptQuantity, slot.Quantity);

                    if (dropAmount <= 0)
                    {
                        break;
                    }

                    PacketSender.SendDropItem(Inventory.IndexOf(slot), dropAmount);
                    promptQuantity -= dropAmount;
                }
            }
        );
    }

    public int FindItem(Guid itemId, int itemVal = 1)
    {
        for (var i = 0; i < Options.Instance.Player.MaxInventory; i++)
        {
            if (Inventory[i].ItemId == itemId && Inventory[i].Quantity >= itemVal)
            {
                return i;
            }
        }

        return -1;
    }

    private static int GetQuantityOfItemIn(IEnumerable<IItem> items, Guid itemId)
    {
        long count = 0;

        foreach (var slot in items ?? [])
        {
            if (slot?.ItemId == itemId)
            {
                count += slot.Quantity;
            }
        }

        return (int)Math.Min(count, int.MaxValue);
    }

    public static int GetQuantityOfItemInBank(Guid itemId) => GetQuantityOfItemIn(Globals.Bank, itemId);

    public int GetQuantityOfItemInInventory(Guid itemId) => GetQuantityOfItemIn(Inventory, itemId);

    public void TryUseItem(int index)
    {
        if (!IsItemOnCooldown(index) &&
            index >= 0 && index < Globals.Me?.Inventory.Length && Globals.Me.Inventory[index]?.Quantity > 0)
        {
            PacketSender.SendUseItem(index, TargetId);
        }
    }

    public long GetItemCooldown(Guid id)
    {
        if (ItemCooldowns.TryGetValue(id, out var value))
        {
            return value;
        }

        return 0;
    }

    public int FindHotbarItem(IHotbarInstance hotbarInstance)
    {
        var bestMatch = -1;

        if (hotbarInstance.ItemOrSpellId != Guid.Empty)
        {
            for (var i = 0; i < Inventory.Length; i++)
            {
                var itm = Inventory[i];
                if (itm != null && itm.ItemId == hotbarInstance.ItemOrSpellId)
                {
                    bestMatch = i;
                    var itemBase = ItemBase.Get(itm.ItemId);
                    if (itemBase != null)
                    {
                        if (itemBase.ItemType == ItemType.Bag)
                        {
                            if (hotbarInstance.BagId == itm.BagId)
                            {
                                break;
                            }
                        }
                        else if (itemBase.ItemType == ItemType.Equipment)
                        {
                            if (hotbarInstance.PreferredStatBuffs != null)
                            {
                                var statMatch = true;
                                for (var s = 0; s < hotbarInstance.PreferredStatBuffs.Length; s++)
                                {
                                    if (itm.ItemProperties.StatModifiers[s] != hotbarInstance.PreferredStatBuffs[s])
                                    {
                                        statMatch = false;
                                    }
                                }

                                if (statMatch)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        return bestMatch;
    }

    public bool IsEquipped(int slot)
    {
        for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
        {
            if (MyEquipment[i] == slot)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsItemOnCooldown(int slot)
    {
        if (Inventory[slot] != null)
        {
            var itm = Inventory[slot];
            if (itm.ItemId != Guid.Empty)
            {
                if (ItemCooldowns.TryGetValue(itm.ItemId, out var value) && value > Timing.Global.Milliseconds)
                {
                    return true;
                }

                if ((ItemBase.TryGet(itm.ItemId, out var itemBase) && !itemBase.IgnoreGlobalCooldown) && Globals.Me?.GlobalCooldown > Timing.Global.Milliseconds)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public long GetItemRemainingCooldown(int slot)
    {
        if (Inventory[slot] != null)
        {
            var itm = Inventory[slot];
            if (itm.ItemId != Guid.Empty)
            {
                if (ItemCooldowns.TryGetValue(itm.ItemId, out var value) && value > Timing.Global.Milliseconds)
                {
                    return value - Timing.Global.Milliseconds;
                }

                if ((ItemBase.TryGet(itm.ItemId, out var itemBase) && !itemBase.IgnoreGlobalCooldown) && Globals.Me?.GlobalCooldown > Timing.Global.Milliseconds)
                {
                    return Globals.Me.GlobalCooldown - Timing.Global.Milliseconds;
                }
            }
        }

        return 0;
    }

    public bool IsSpellOnCooldown(int slot)
    {
        if (Spells[slot] != null)
        {
            var spl = Spells[slot];
            if (spl.Id != Guid.Empty)
            {
                if (SpellCooldowns.TryGetValue(spl.Id, out var value) && value > Timing.Global.Milliseconds)
                {
                    return true;
                }

                if ((SpellBase.TryGet(spl.Id, out var spellBase) && !spellBase.IgnoreGlobalCooldown) && Globals.Me?.GlobalCooldown > Timing.Global.Milliseconds)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public long GetSpellRemainingCooldown(int slot)
    {
        if (slot < 0 || Spells.Length <= slot)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                new ArgumentOutOfRangeException(nameof(slot), slot, $@"Slot was out of the range [0,{Spells.Length}"),
                "Tried to get remaining cooldown for spell in invalid slot {SlotIndex}",
                slot
            );
            return 0;
        }

        var spell = Spells[slot];
        // These two can't be combined into a nullish expression because Guid is a struct
        if (spell == default || spell.Id == Guid.Empty)
        {
            return 0;
        }

        var now = Timing.Global.Milliseconds;

        if (SpellCooldowns.TryGetValue(spell.Id, out var cd) && cd > now)
        {
            return cd - now;
        }

        if (SpellBase.TryGet(spell.Id, out var spellBase) && !spellBase.IgnoreGlobalCooldown && Globals.Me?.GlobalCooldown > now)
        {
            return Globals.Me.GlobalCooldown - now;
        }

        return 0;
    }

    public void TrySellItem(int inventorySlotIndex)
    {
        var inventorySlot = Inventory[inventorySlotIndex];
        if (!ItemBase.TryGet(inventorySlot.ItemId, out var itemDescriptor))
        {
            return;
        }

        var shop = Globals.GameShop;
        var shopCanBuyItem =
            shop?.BuyingWhitelist == true && shop.BuyingItems.Any(buyingItem => buyingItem.ItemId == itemDescriptor.Id)
            || shop?.BuyingWhitelist == false && !shop.BuyingItems.Any(buyingItem => buyingItem.ItemId == itemDescriptor.Id);

        var prompt = Strings.Shop.SellPrompt;
        var inputType = InputType.YesNo;
        Base.GwenEventHandler<InputSubmissionEventArgs>? onSuccess = (sender, args) =>
        {
            if (sender is not InputBox { UserData: int slotIndex })
            {
                return;
            }

            PacketSender.SendSellItem(slotIndex, 1);
        };
        var userData = inventorySlotIndex;
        var slotQuantity = inventorySlot.Quantity;
        var maxQuantity = slotQuantity;

        if (!shopCanBuyItem)
        {
            prompt = Strings.Shop.CannotSell;
            inputType = InputType.Okay;
            onSuccess = null;
            userData = -1;
        }
        else if (itemDescriptor.IsStackable || slotQuantity > 1)
        {
            var inventoryQuantity = GetQuantityOfItemInInventory(itemDescriptor.Id);
            if (inventoryQuantity > 1)
            {
                maxQuantity = inventoryQuantity;
                prompt = Strings.Shop.SellItemPrompt;
                inputType = InputType.NumericSliderInput;
                onSuccess = TrySellItemOnSubmit;
                userData = inventorySlotIndex;
            }
        }

        _ = new InputBox(
            title: Strings.Shop.SellItem,
            prompt: prompt.ToString(itemDescriptor.Name),
            inputType: inputType,
            quantity: slotQuantity,
            maximumQuantity: maxQuantity,
            userData: userData,
            onSubmit: onSuccess
        );
    }

    private static void TrySellItemOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: int slotIndex })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendSellItem(slotIndex, value);
        }
    }

    public void TryBuyItem(int shopSlotIndex)
    {
        //Confirm the purchase
        var shopSlot = Globals.GameShop?.SellingItems[shopSlotIndex];
        if (shopSlot == default || !ItemBase.TryGet(shopSlot.ItemId, out var itemDescriptor))
        {
            return;
        }

        var maxBuyAmount = 0;
        if (shopSlot.CostItemQuantity < 1)
        {
            var emptySlots = Inventory.Count(inventorySlot => inventorySlot.ItemId == default);
            var slotsWithItem = Inventory.Count(inventorySlot => inventorySlot.ItemId == itemDescriptor.Id);
            var currentItemQuantity = GetQuantityOfItemInInventory(shopSlot.ItemId);
            var stackSize = itemDescriptor.IsStackable ? itemDescriptor.MaxInventoryStack : 1;
            var itemQuantityLimitInCurrentStacks = slotsWithItem * stackSize;
            var partialStackEmptyQuantity = itemQuantityLimitInCurrentStacks - currentItemQuantity;
            maxBuyAmount = emptySlots * stackSize + partialStackEmptyQuantity;
        }
        else
        {
            var currencyCount = GetQuantityOfItemInInventory(shopSlot.CostItemId);
            maxBuyAmount = (int)Math.Floor(currencyCount / (float)shopSlot.CostItemQuantity);
        }

        // If our max buy amount is 0 or the item isn't stackable, let the server handle it
        if (!itemDescriptor.IsStackable || maxBuyAmount == 0)
        {
            PacketSender.SendBuyItem(shopSlotIndex, 1);
            return;
        }

        _ = new InputBox(
            title: Strings.Shop.BuyItem,
            prompt: Strings.Shop.BuyItemPrompt.ToString(itemDescriptor.Name),
            inputType: InputType.NumericSliderInput,
            quantity: maxBuyAmount,
            maximumQuantity: maxBuyAmount,
            userData: shopSlotIndex,
            onSubmit: TryBuyItemOnSubmit
        );
    }

    private static void TryBuyItemOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: int slotIndex })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendBuyItem(slotIndex, value);
        }
    }

    /// <summary>
    /// Attempts to deposit the item from the inventory into the bank.
    /// </summary>
    /// <param name="inventorySlotIndex"></param>
    /// <param name="slot"></param>
    /// <param name="bankSlotIndex"></param>
    /// <param name="quantityHint"></param>
    /// <param name="skipPrompt"></param>
    /// <returns></returns>
    public bool TryStoreItemInBank(
        int inventorySlotIndex,
        IItem? slot = null,
        int bankSlotIndex = -1,
        int quantityHint = -1,
        bool skipPrompt = false
    )
    {
        // Permission Check for Guild Bank
        if (Globals.GuildBank && !IsGuildBankDepositAllowed())
        {
            ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Guilds.NotAllowedDeposit.ToString(Globals.Me?.Guild), CustomColors.Alerts.Error, ChatMessageType.Bank));
            return false;
        }

        slot ??= Inventory[inventorySlotIndex];
        if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"Tried to move item that does not exist from slot {inventorySlotIndex}: {itemDescriptor.Id}");
            return false;
        }

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (quantityHint == 0)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"Tried to move 0 of '{itemDescriptor.Name}' ({itemDescriptor.Id})");
            return false;
        }

        var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxBankStack : 1;
        var sourceQuantity = GetQuantityOfItemInInventory(itemDescriptor.Id);
        var quantity = quantityHint < 0 ? sourceQuantity : quantityHint;

        var targetSlots = Globals.Bank.ToArray();

        var movableQuantity = Item.FindSpaceForItem(
            itemDescriptor.Id,
            itemDescriptor.ItemType,
            maximumStack,
            bankSlotIndex,
            quantityHint < 0 ? sourceQuantity : quantityHint,
            targetSlots
        );

        if (movableQuantity < 1)
        {
            ChatboxMsg.AddMessage(new ChatboxMsg(
                Strings.Bank.NoSpace,
                CustomColors.Alerts.Error,
                ChatMessageType.Bank
            ));
            return false;
        }

        if (bankSlotIndex < 0 && itemDescriptor.IsStackable)
        {
            bankSlotIndex = Item.FindFirstPartialSlot(itemDescriptor.Id, targetSlots, maximumStack);
        }

        if (skipPrompt)
        {
            PacketSender.SendDepositItem(inventorySlotIndex, movableQuantity, bankSlotIndex);
            return true;
        }

        var maximumQuantity = movableQuantity < quantity ? movableQuantity : Item.FindSpaceForItem(
            itemDescriptor.Id,
            itemDescriptor.ItemType,
            maximumStack,
            bankSlotIndex,
            sourceQuantity,
            targetSlots
        );

        if (maximumQuantity == 1)
        {
            PacketSender.SendDepositItem(inventorySlotIndex, movableQuantity, bankSlotIndex);
            return true;
        }

        _ = new InputBox(
            title: Strings.Bank.DepositItem,
            prompt: Strings.Bank.DepositItemPrompt.ToString(itemDescriptor.Name),
            inputType: InputType.NumericSliderInput,
            quantity: movableQuantity,
            maximumQuantity: maximumQuantity,
            userData: new Tuple<int, int>(inventorySlotIndex, bankSlotIndex),
            onSubmit: TryDepositItemOnSubmitted
        );

        return true;
    }

    private static void TryDepositItemOnSubmitted(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: (int inventorySlotIndex, int bankSlotIndex) })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendDepositItem(inventorySlotIndex, value, bankSlotIndex);
        }
    }

    private static bool IsGuildBankDepositAllowed()
    {
        return !string.IsNullOrWhiteSpace(Globals.Me?.Guild) &&
               (Globals.Me?.GuildRank?.Permissions.BankDeposit == true || Globals.Me?.Rank == 0);
    }

    /// <summary>
    /// Attempts to withdraw items from the bank into the inventory.
    /// </summary>
    /// <param name="bankSlotIndex"></param>
    /// <param name="slot"></param>
    /// <param name="inventorySlotIndex"></param>
    /// <param name="quantityHint"></param>
    /// <param name="skipPrompt"></param>
    /// <returns></returns>
    public bool TryRetrieveItemFromBank(
        int bankSlotIndex,
        IItem? slot = null,
        int inventorySlotIndex = -1,
        int quantityHint = -1,
        bool skipPrompt = false
    )
    {
        // Permission Check for Guild Bank
        if (Globals.GuildBank && !IsGuildBankWithdrawAllowed())
        {
            ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Guilds.NotAllowedWithdraw.ToString(Globals.Me?.Guild), CustomColors.Alerts.Error, ChatMessageType.Bank));
            return false;
        }

        slot ??= Globals.Bank[bankSlotIndex];
        if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"Tried to move item that does not exist from slot {bankSlotIndex}: {itemDescriptor.Id}");
            return false;
        }

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (quantityHint == 0)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"Tried to move 0 of '{itemDescriptor.Name}' ({itemDescriptor.Id})");
            return false;
        }

        var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxInventoryStack : 1;
        var sourceQuantity = GetQuantityOfItemInBank(itemDescriptor.Id);
        var quantity = quantityHint < 0 ? sourceQuantity : quantityHint;
        var targetSlots = Inventory.ToArray();

        var movableQuantity = Item.FindSpaceForItem(
            itemDescriptor.Id,
            itemDescriptor.ItemType,
            maximumStack,
            bankSlotIndex,
            quantityHint < 0 ? sourceQuantity : quantityHint,
            targetSlots
        );

        if (movableQuantity < 1)
        {
            ChatboxMsg.AddMessage(new ChatboxMsg(
                Strings.Bank.WithdrawItemNoSpace,
                CustomColors.Alerts.Error,
                ChatMessageType.Bank
            ));
            return false;
        }

        if (inventorySlotIndex < 0 && itemDescriptor.IsStackable)
        {
            inventorySlotIndex = Item.FindFirstPartialSlot(itemDescriptor.Id, targetSlots, maximumStack);
        }

        if (skipPrompt)
        {
            PacketSender.SendWithdrawItem(bankSlotIndex, movableQuantity, inventorySlotIndex);
            return true;
        }

        var maximumQuantity = movableQuantity < quantity ? movableQuantity : Item.FindSpaceForItem(
            itemDescriptor.Id,
            itemDescriptor.ItemType,
            maximumStack,
            inventorySlotIndex,
            sourceQuantity,
            targetSlots
        );

        if (maximumQuantity == 1)
        {
            PacketSender.SendWithdrawItem(bankSlotIndex, movableQuantity, inventorySlotIndex);
            return true;
        }

        _ = new InputBox(
            title: Strings.Bank.WithdrawItem,
            prompt: Strings.Bank.WithdrawItemPrompt.ToString(itemDescriptor.Name),
            inputType: InputType.NumericSliderInput,
            quantity: movableQuantity,
            maximumQuantity: maximumQuantity,
            userData: new Tuple<int, int>(bankSlotIndex, inventorySlotIndex),
            onSubmit: TryWithdrawItemOnSubmitted
        );

        return true;
    }

    private static void TryWithdrawItemOnSubmitted(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: (int bankSlotIndex, int inventorySlotIndex) })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendWithdrawItem(bankSlotIndex, value, inventorySlotIndex);
        }
    }

    private static bool IsGuildBankWithdrawAllowed()
    {
        return !string.IsNullOrWhiteSpace(Globals.Me?.Guild) &&
               (Globals.Me?.GuildRank?.Permissions.BankRetrieve == true || Globals.Me?.Rank == 0);
    }

    //Bag
    public void TryStoreItemInBag(int inventorySlotIndex, int bagSlotIndex)
    {
        var inventorySlot = Inventory[inventorySlotIndex];
        if (!ItemBase.TryGet(inventorySlot.ItemId, out var itemDescriptor))
        {
            return;
        }

        var quantity = inventorySlot.Quantity;
        var maxQuantity = quantity;

        if (maxQuantity < 2)
        {
            PacketSender.SendStoreBagItem(inventorySlotIndex, 1, bagSlotIndex);
            return;
        }

        _ = new InputBox(
            title: Strings.Bags.StoreItem,
            prompt: Strings.Bags.StoreItemPrompt.ToString(itemDescriptor.Name),
            inputType: InputType.NumericSliderInput,
            quantity: quantity,
            maximumQuantity: maxQuantity,
            userData: new Tuple<int, int>(inventorySlotIndex, bagSlotIndex),
            onSubmit: TryStoreItemInBagOnSubmit
        );
    }

    private static void TryStoreItemInBagOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: (int inventorySlotIndex, int bagSlotIndex) })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendStoreBagItem(inventorySlotIndex, value, bagSlotIndex);
        }
    }

    public void TryRetrieveItemFromBag(int bagSlotIndex, int inventorySlotIndex)
    {
        var bagSlot = Globals.Bag[bagSlotIndex];
        if (bagSlot == default)
        {
            return;
        }

        if (!ItemBase.TryGet(bagSlot.ItemId, out var itemDescriptor))
        {
            return;
        }

        var quantity = bagSlot.Quantity;
        var maxQuantity = quantity;

        if (maxQuantity < 2)
        {
            PacketSender.SendRetrieveBagItem(bagSlotIndex, 1, inventorySlotIndex);
            return;
        }

        _ = new InputBox(
            title: Strings.Bags.RetrieveItem,
            prompt: Strings.Bags.RetrieveItemPrompt.ToString(itemDescriptor.Name),
            inputType: InputType.NumericSliderInput,
            quantity: quantity,
            maximumQuantity: maxQuantity,
            userData: new Tuple<int, int>(bagSlotIndex, inventorySlotIndex),
            onSubmit: TryRetrieveItemFromBagOnSubmit
        );
    }

    private static void TryRetrieveItemFromBagOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: (int bagSlotIndex, int inventorySlotIndex) })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendRetrieveBagItem(bagSlotIndex, value, inventorySlotIndex);
        }
    }

    //Trade
    public void TryOfferItemToTrade(int index)
    {
        var slot = Inventory[index];
        var quantity = slot.Quantity;
        var tradingItem = ItemBase.Get(slot.ItemId);
        if (tradingItem == null)
        {
            return;
        }

        if (quantity == 1)
        {
            PacketSender.SendOfferTradeItem(index, 1);
            return;
        }

        _ = new InputBox(
            title: Strings.Trading.OfferItem,
            prompt: Strings.Trading.OfferItemPrompt.ToString(tradingItem.Name),
            inputType: InputType.NumericSliderInput,
            quantity: quantity,
            maximumQuantity: quantity,
            userData: index,
            onSubmit: TryOfferItemToTradeOnSubmit
        );
    }

    private static void TryOfferItemToTradeOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: int slotIndex })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendOfferTradeItem(slotIndex, value);
        }
    }

    public void TryCancelOfferToTradeItem(int index)
    {
        var slot = Globals.Trade[0, index];
        var quantity = slot.Quantity;
        var revokedItem = ItemBase.Get(slot.ItemId);
        if (revokedItem == null)
        {
            return;
        }

        if (quantity == 1)
        {
            PacketSender.SendRevokeTradeItem(index, 1);
            return;
        }

        _ = new InputBox(
            title: Strings.Trading.RevokeItem,
            prompt: Strings.Trading.RevokeItemPrompt.ToString(revokedItem.Name),
            inputType: InputType.NumericSliderInput,
            quantity: quantity,
            maximumQuantity: quantity,
            userData: index,
            onSubmit: TryCancelOfferToTradeItemOnSubmit
        );
    }

    private static void TryCancelOfferToTradeItemOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: int slotIndex })
        {
            return;
        }

        if (args.Value is not NumericalSubmissionValue submissionValue)
        {
            return;
        }

        var value = (int)Math.Round(submissionValue.Value);
        if (value > 0)
        {
            PacketSender.SendRevokeTradeItem(slotIndex, value);
        }
    }

    //Spell Processing
    public void SwapSpells(int fromSpellIndex, int toSpellIndex)
    {
        PacketSender.SendSwapSpells(fromSpellIndex, toSpellIndex);
        (Spells[fromSpellIndex], Spells[toSpellIndex]) = (Spells[toSpellIndex], Spells[fromSpellIndex]);
    }

    public void TryForgetSpell(int spellIndex)
    {
        var spellSlot = Spells[spellIndex];
        if (SpellBase.TryGet(spellSlot.Id, out var spellDescriptor))
        {
            _ = new InputBox(
                title: Strings.Spells.ForgetSpell,
                prompt: Strings.Spells.ForgetSpellPrompt.ToString(spellDescriptor.Name),
                inputType: InputType.YesNo,
                userData: spellIndex,
                onSubmit: TryForgetSpellOnSubmit
            );
        }
    }

    private static void TryForgetSpellOnSubmit(Base sender, InputSubmissionEventArgs args)
    {
        if (sender is not InputBox { UserData: int slotIndex })
        {
            return;
        }

        PacketSender.SendForgetSpell(slotIndex);
    }

    public void TryUseSpell(int index)
    {
        if (index < 0 || Spells.Length <= index)
        {
            return;
        }

        var spell = Spells[index];
        if (spell.Id == default)
        {
            return;
        }

        if (GetSpellRemainingCooldown(index) > Timing.Global.Milliseconds)
        {
            return;
        }

        if (!SpellBase.TryGet(spell.Id, out var spellDescriptor))
        {
            return;
        }

        if (spellDescriptor.CastDuration > 0)
        {
            if (Options.Instance.Combat.MovementCancelsCast && Globals.Me?.IsMoving == true)
            {
                return;
            }
        }

        PacketSender.SendUseSpell(index, TargetId);
    }

    public long GetSpellCooldown(Guid id)
    {
        if (SpellCooldowns.TryGetValue(id, out var cd) && cd > Timing.Global.Milliseconds)
        {
            return cd;
        }

        if ((SpellBase.TryGet(id, out var spellBase) && !spellBase.IgnoreGlobalCooldown) && Globals.Me?.GlobalCooldown > Timing.Global.Milliseconds)
        {
            return Globals.Me.GlobalCooldown;
        }

        return 0;
    }

    public void TryUseSpell(Guid spellId)
    {
        if (spellId == Guid.Empty)
        {
            return;
        }

        for (var i = 0; i < Spells.Length; i++)
        {
            if (Spells[i].Id == spellId)
            {
                TryUseSpell(i);

                return;
            }
        }
    }

    public int FindHotbarSpell(IHotbarInstance hotbarInstance)
    {
        if (hotbarInstance.ItemOrSpellId != Guid.Empty && SpellBase.Get(hotbarInstance.ItemOrSpellId) != null)
        {
            for (var i = 0; i < Spells.Length; i++)
            {
                if (Spells[i].Id == hotbarInstance.ItemOrSpellId)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    //Hotbar Processing
    public void AddToHotbar(int hotbarSlot, sbyte itemType, int itemSlot)
    {
        Hotbar[hotbarSlot].ItemOrSpellId = Guid.Empty;
        Hotbar[hotbarSlot].PreferredStatBuffs = new int[Enum.GetValues<Stat>().Length];
        if (itemType == 0)
        {
            var item = Inventory[itemSlot];
            if (item != null)
            {
                Hotbar[hotbarSlot].ItemOrSpellId = item.ItemId;
                Hotbar[hotbarSlot].PreferredStatBuffs = item.ItemProperties.StatModifiers;
            }
        }
        else if (itemType == 1)
        {
            var spell = Spells[itemSlot];
            if (spell != null)
            {
                Hotbar[hotbarSlot].ItemOrSpellId = spell.Id;
            }
        }

        PacketSender.SendHotbarUpdate(hotbarSlot, itemType, itemSlot);
    }

    public void HotbarSwap(int index, int swapIndex)
    {
        var itemId = Hotbar[index].ItemOrSpellId;
        var bagId = Hotbar[index].BagId;
        var stats = Hotbar[index].PreferredStatBuffs;

        Hotbar[index].ItemOrSpellId = Hotbar[swapIndex].ItemOrSpellId;
        Hotbar[index].BagId = Hotbar[swapIndex].BagId;
        Hotbar[index].PreferredStatBuffs = Hotbar[swapIndex].PreferredStatBuffs;

        Hotbar[swapIndex].ItemOrSpellId = itemId;
        Hotbar[swapIndex].BagId = bagId;
        Hotbar[swapIndex].PreferredStatBuffs = stats;

        PacketSender.SendHotbarSwap(index, swapIndex);
    }

    // Change the dimension if the player is on a gateway
    private void TryToChangeDimension()
    {
        if (X < Options.Instance.Map.MapWidth && X >= 0)
        {
            if (Y < Options.Instance.Map.MapHeight && Y >= 0)
            {
                if (Maps.MapInstance.Get(MapId) != null && Maps.MapInstance.Get(MapId).Attributes[X, Y] != null)
                {
                    if (Maps.MapInstance.Get(MapId).Attributes[X, Y].Type == MapAttributeType.ZDimension)
                    {
                        if (((MapZDimensionAttribute)Maps.MapInstance.Get(MapId).Attributes[X, Y]).GatewayTo > 0)
                        {
                            Z = (byte)(((MapZDimensionAttribute)Maps.MapInstance.Get(MapId).Attributes[X, Y])
                                        .GatewayTo -
                                        1);
                        }
                    }
                }
            }
        }
    }

    //Input Handling
    private void HandleInput()
    {
        var inputX = 0;
        var inputY = 0;

        if (Interface.Interface.HasInputFocus())
        {
            return;
        }

        if (Controls.IsControlPressed(Control.MoveUp))
        {
            inputY += 1;
        }

        if (Controls.IsControlPressed(Control.MoveDown))
        {
            inputY -= 1;
        }

        if (Controls.IsControlPressed(Control.MoveLeft))
        {
            inputX -= 1;
        }

        if (Controls.IsControlPressed(Control.MoveRight))
        {
            inputX += 1;
        }

        Direction inputDirection;
        if (inputX == 0 && inputY == 0)
        {
            inputDirection = Direction.None;
        }
        else
        {
            var diagonalMovement = inputX != 0 && Options.Instance.Map.EnableDiagonalMovement;
            var inputXDirection = Math.Sign(inputX) switch
            {
                < 0 => Direction.Left,
                > 0 => Direction.Right,
                _ => Direction.None,
            };
            var inputYDirection = Math.Sign(inputY) switch
            {
                < 0 => Direction.Down,
                > 0 => Direction.Up,
                _ => Direction.None,
            };

            if (diagonalMovement)
            {
                inputDirection = inputYDirection switch
                {
                    Direction.Down => inputXDirection switch
                    {
                        Direction.Left => Direction.DownLeft,
                        Direction.Right => Direction.DownRight,
                        _ => inputYDirection,
                    },
                    Direction.Up => inputXDirection switch
                    {
                        Direction.Left => Direction.UpLeft,
                        Direction.Right => Direction.UpRight,
                        _ => inputYDirection,
                    },
                    _ => inputXDirection,
                };
            }
            else if (inputYDirection != Direction.None)
            {
                inputDirection = inputYDirection;
            }
            else
            {
                inputDirection = inputXDirection;
            }
        }

        if (Globals.Me != default)
        {
            Globals.Me.MoveDir = inputDirection;
        }

        TurnAround();

        var castInput = -1;

        // Yes we unfortunately need to do all of this extra logic because multiple hotbar slots could get triggered
        // because one has no modifier (e.g. 1) but another has the same key but a different modifier (e.g. Alt + 1)
        // - Select -> finds out if a hotbar slot is pressed or not
        // - Where -> filters out hotbar slots that are not pressed
        // - OrderByDescending -> prioritizes hotbar slots with modifiers over those without (e.g. Alt + 1 over 1)
        // - GroupBy -> groups hotbar slots that have a colliding key, maintaining the order (e.g. Alt + 1 and 1 will be grouped)
        var activeHotbarSlotIndicesGroupedByKey = Enumerable.Range(0, Options.Instance.Player.HotbarSlotCount)
            .Select(
                slotIndex => Controls.IsControlPressed(
                    Control.HotkeyOffset + slotIndex + 1,
                    out _,
                    out var activeBinding
                )
                    ? (slotIndex, activeBinding)
                    : default
            )
            .Where(controlHit => controlHit != default)
            .OrderByDescending(controlHit => controlHit.activeBinding.Modifier)
            .GroupBy(controlHit => controlHit.activeBinding.Key)
            .ToArray();

        // This grabs the first active slot per key group, so if slot 11 is mapped to Alt + 1 and slot 1 is mapped to 1,
        // then slot 11 will be returned here instead of both slot 1 and 11
        var activeHotbarSlotIndices =
            activeHotbarSlotIndicesGroupedByKey.Select(group => group.FirstOrDefault().slotIndex).ToArray();

        foreach (var slotIndex in activeHotbarSlotIndices)
        {
            mLastHotbarUseTime.TryAdd(slotIndex, 0);
            castInput = slotIndex;
        }

        // ReSharper disable once InvertIf
        if (0 <= castInput && castInput < Interface.Interface.GameUi?.Hotbar?.Items?.Count && mLastHotbarUseTime[castInput] < Timing.Global.Milliseconds)
        {
            Interface.Interface.GameUi?.Hotbar?.Items?[castInput]?.Activate();
            mLastHotbarUseTime[castInput] = Timing.Global.Milliseconds + mHotbarUseDelay;
        }
    }

    protected int GetDistanceTo(IEntity target)
    {
        if (target != null)
        {
            var myMap = Maps.MapInstance.Get(MapId);
            var targetMap = Maps.MapInstance.Get(target.MapId);
            if (myMap != null && targetMap != null)
            {
                //Calculate World Tile of Me
                var x1 = X + myMap.GridX * Options.Instance.Map.MapWidth;
                var y1 = Y + myMap.GridY * Options.Instance.Map.MapHeight;

                //Calculate world tile of target
                var x2 = target.X + targetMap.GridX * Options.Instance.Map.MapWidth;
                var y2 = target.Y + targetMap.GridY * Options.Instance.Map.MapHeight;

                return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }
        }

        //Something is null.. return a value that is out of range :)
        return 9999;
    }

    public void AutoTarget()
    {
        //Check for taunt status if so don't allow to change target
        for (var i = 0; i < Status.Count; i++)
        {
            if (Status[i].Type == SpellEffect.Taunt)
            {
                return;
            }
        }

        // Do we need to account for players?
        // Depends on what type of map we're currently on.
        if (Globals.Me?.MapInstance == null)
        {
            return;
        }

        var currentMap = Globals.Me.MapInstance as MapInstance;
        var canTargetPlayers = currentMap?.ZoneType != MapZone.Safe;

        // Build a list of Entities to select from with positions if our list is either old, we've moved or changed maps somehow.
        if (
            mlastTargetScanTime < Timing.Global.Milliseconds ||
            mlastTargetScanMap != Globals.Me.MapId ||
            mlastTargetScanLocation != new Point(X, Y)
            )
        {
            // Add new items to our list!
            foreach (var en in Globals.Entities)
            {
                // Check if this is a valid entity.
                if (en.Value == null)
                {
                    continue;
                }

                // Don't allow us to auto target ourselves.
                if (en.Value == Globals.Me)
                {
                    continue;
                }

                // Check if the entity has stealth status
                if (en.Value.IsHidden || (en.Value.IsStealthed && !Globals.Me.IsInMyParty(en.Value.Id)))
                {
                    continue;
                }

                // Check if we are allowed to target players here, if we're not and this is a player then skip!
                // If we are, check to see if they're our party or nation member, then exclude them. We're friendly happy people here.
                if (!canTargetPlayers && en.Value.Type == EntityType.Player)
                {
                    continue;
                }

                if (canTargetPlayers && en.Value.Type == EntityType.Player)
                {
                    var player = en.Value as Player;
                    if (player != default && IsInMyParty(player))
                    {
                        continue;
                    }
                }

                if (en.Value.Type is EntityType.GlobalEntity or EntityType.Player)
                {
                    // Already in our list?
                    if (mlastTargetList.TryGetValue(en.Value, out var value))
                    {
                        value.DistanceTo = GetDistanceTo(en.Value);
                    }
                    else
                    {
                        // Add entity with blank time. Never been selected.
                        mlastTargetList.Add(en.Value, new TargetInfo { DistanceTo = GetDistanceTo(en.Value), LastTimeSelected = 0 });
                    }
                }
            }

            // Remove old items.
            var toRemove = mlastTargetList.Where(en => !Globals.Entities.ContainsValue(en.Key)).ToArray();
            foreach (var en in toRemove)
            {
                _ = mlastTargetList.Remove(en.Key);
            }

            // Skip scanning for another second or so.. And set up other values.
            mlastTargetScanTime = Timing.Global.Milliseconds + 300;
            mlastTargetScanMap = MapId;
            mlastTargetScanLocation = new Point(X, Y);
        }

        // Find valid entities.
        var validEntities = mlastTargetList.ToArray();

        // Reduce the number of targets down to what is in our allowed range.
        validEntities = validEntities.Where(en => en.Value.DistanceTo <= Options.Instance.Combat.MaxPlayerAutoTargetRadius).ToArray();

        int currentDistance = 9999;
        long currentTime = Timing.Global.Milliseconds;
        Entity? currentEntity = mLastEntitySelected;
        foreach (var entity in validEntities)
        {
            if (currentEntity == entity.Key)
            {
                continue;
            }

            // if distance is the same
            if (entity.Value.DistanceTo == currentDistance)
            {
                if (entity.Value.LastTimeSelected < currentTime)
                {
                    currentTime = entity.Value.LastTimeSelected;
                    currentDistance = entity.Value.DistanceTo;
                    currentEntity = entity.Key;
                }
            }
            else if (entity.Value.DistanceTo < currentDistance)
            {
                if (entity.Value.LastTimeSelected < currentTime || entity.Value.LastTimeSelected == currentTime)
                {
                    currentTime = entity.Value.LastTimeSelected;
                    currentDistance = entity.Value.DistanceTo;
                    currentEntity = entity.Key;
                }
            }
        }

        // We didn't target anything? Can we default to closest?
        if (currentEntity == null)
        {
            currentEntity = validEntities.Where(x => x.Value.DistanceTo == validEntities.Min(y => y.Value.DistanceTo)).FirstOrDefault().Key;

            // Also reset our target times so we can start auto targetting again.
            foreach (var entity in mlastTargetList)
            {
                entity.Value.LastTimeSelected = 0;
            }
        }

        if (currentEntity == null)
        {
            mLastEntitySelected = null;
            return;
        }

        if(!Globals.Entities.TryGetValue(currentEntity.Id, out var targetedEntity))
        {
            return;
        }

        if (mlastTargetList.TryGetValue(targetedEntity, out var lastTarget))
        {
            lastTarget.LastTimeSelected = Timing.Global.Milliseconds;
        }

        mLastEntitySelected = targetedEntity;

        if (TargetId != targetedEntity.Id)
        {
            Target = targetedEntity;
        }
    }

    private void SetTargetBox(IEntity? targetEntity)
    {
        switch (targetEntity)
        {
            case null:
            {
                // ReSharper disable once InvertIf
                if (TargetBox is { } targetBox)
                {
                    TargetBox.SetEntity(null);
                    if (targetBox.IsVisible)
                    {
                        TargetBox.Hide();
                    }
                }
                return;
            }

            case Player:
                TargetBox?.SetEntity(targetEntity, EntityType.Player);
                break;
            case Event:
                TargetBox?.SetEntity(targetEntity, EntityType.Event);
                break;
            default:
                TargetBox?.SetEntity(targetEntity, EntityType.GlobalEntity);
                break;
        }

        TargetBox?.Show();
        PacketSender.SendTarget(targetEntity.Id);
    }

    private void AutoTurnToTarget(Entity en)
    {
        if (en == this)
        {
            return;
        }

        if (!Globals.Database.AutoTurnToTarget)
        {
            return;
        }

        if (!Options.Instance.Player.EnableAutoTurnToTarget)
        {
            return;
        }

        if (Controls.IsControlPressed(Control.TurnAround))
        {
            return;
        }

        if (IsTurnAroundWhileCastingDisabled)
        {
            return;
        }

        var directionToTarget = DirectionToTarget(en);

        if (IsMoving || Dir == MoveDir || Dir == directionToTarget)
        {
            AutoTurnToTargetTimer = Timing.Global.Milliseconds + Options.Instance.Player.AutoTurnToTargetDelay;
            return;
        }

        if (AutoTurnToTargetTimer > Timing.Global.Milliseconds)
        {
            return;
        }

        if (Options.Instance.Player.AutoTurnToTargetIgnoresEntitiesBehind &&
            IsTargetAtOppositeDirection(Dir, directionToTarget))
        {
            return;
        }

        MoveDir = Direction.None;
        Dir = directionToTarget;
        PacketSender.SendDirection(Dir);
        PickLastDirection(Dir);
    }

    private static void ToggleTargetContextMenu(Entity en)
    {
        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Right))
        {
            Interface.Interface.GameUi.TargetContextMenu.ToggleHidden(en);
        }
    }

    public bool TryBlock()
    {
        var shieldIndex = Options.Instance.Equipment.ShieldSlot;
        var myShieldIndex = MyEquipment[shieldIndex];

        // Return false if character is attacking, or blocking or if they don't have a shield equipped.
        if (IsAttacking || IsBlocking || shieldIndex < 0 || myShieldIndex < 0)
        {
            return false;
        }

        // Return false if the shield item descriptor could not be retrieved.
        if (!ItemBase.TryGet(Inventory[myShieldIndex].ItemId, out _))
        {
            return false;
        }

        IsBlocking = true;
        PacketSender.SendBlock(IsBlocking);
        return IsBlocking;
    }

    public bool TryAttack()
    {
        if (IsAttacking || IsBlocking || (IsMoving && !Options.Instance.Player.AllowCombatMovement) || Globals.Me == default)
        {
            return false;
        }

        int x = Globals.Me.X;
        int y = Globals.Me.Y;
        var map = Globals.Me.MapId;

        switch (Globals.Me.Dir)
        {
            case Direction.Up:
                y--;
                break;

            case Direction.Down:
                y++;
                break;

            case Direction.Left:
                x--;
                break;

            case Direction.Right:
                x++;
                break;

            case Direction.UpLeft:
                y--;
                x--;
                break;

            case Direction.UpRight:
                y--;
                x++;
                break;

            case Direction.DownRight:
                y++;
                x++;
                break;

            case Direction.DownLeft:
                y++;
                x--;
                break;
        }

        if (TryGetRealLocation(ref x, ref y, ref map))
        {
            // Iterate through all entities
            foreach (var en in Globals.Entities)
            {
                // Skip if the entity is null or is not within the player's map.
                if (en.Value?.MapId != map)
                {
                    continue;
                }

                // Skip if the entity is the current player.
                if (en.Value == Globals.Me)
                {
                    continue;
                }

                // Skip if the entity can't be attacked.
                if (!en.Value.CanBeAttacked)
                {
                    continue;
                }

                if (en.Value.X != x || en.Value.Y != y)
                {
                    continue;
                }

                // Attack the entity.
                PacketSender.SendAttack(en.Key);
                AttackTimer = Timing.Global.Milliseconds + CalculateAttackTime();

                return true;
            }
        }

        foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
        {
            foreach (var en in eventMap.LocalEntities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                if (en.Value.MapId == map && en.Value.X == x && en.Value.Y == y)
                {
                    if (en.Value is Event evt && evt.Trigger == EventTrigger.ActionButton)
                    {
                        //Talk to Event
                        PacketSender.SendActivateEvent(en.Key);
                        AttackTimer = Timing.Global.Milliseconds + CalculateAttackTime();

                        return true;
                    }
                }
            }
        }

        //Projectile/empty swing for animations
        PacketSender.SendAttack(Guid.Empty);
        AttackTimer = Timing.Global.Milliseconds + CalculateAttackTime();

        return true;
    }

    public bool TryGetRealLocation(ref int x, ref int y, ref Guid mapId)
    {
        if (Globals.MapGrid == default)
        {
            return false;
        }

        var tmpX = x;
        var tmpY = y;
        if (Maps.MapInstance.Get(mapId) != null)
        {
            var gridX = Maps.MapInstance.Get(mapId).GridX;
            var gridY = Maps.MapInstance.Get(mapId).GridY;

            if (x < 0)
            {
                tmpX = Options.Instance.Map.MapWidth - x * -1;
                gridX--;
            }

            if (y < 0)
            {
                tmpY = Options.Instance.Map.MapHeight - y * -1;
                gridY--;
            }

            if (y > Options.Instance.Map.MapHeight - 1)
            {
                tmpY = y - Options.Instance.Map.MapHeight;
                gridY++;
            }

            if (x > Options.Instance.Map.MapWidth - 1)
            {
                tmpX = x - Options.Instance.Map.MapWidth;
                gridX++;
            }

            if (gridX >= 0 && gridX < Globals.MapGridWidth && gridY >= 0 && gridY < Globals.MapGridHeight)
            {
                if (Maps.MapInstance.Get(Globals.MapGrid[gridX, gridY]) != null)
                {
                    x = (byte)tmpX;
                    y = (byte)tmpY;
                    mapId = Globals.MapGrid[gridX, gridY];

                    return true;
                }
            }
        }

        return false;
    }

    public bool TryTarget()
    {
        //Check for taunt status if so don't allow to change target
        for (var i = 0; i < Status.Count; i++)
        {
            if (Status[i].Type == SpellEffect.Taunt)
            {
                return false;
            }
        }

        var mouseInWorld = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
        var x = (int)mouseInWorld.X;
        var y = (int)mouseInWorld.Y;
        var targetRect = new FloatRect(x - 8, y - 8, 16, 16); //Adjust to allow more/less error

        IEntity? bestMatch = null;
        var bestAreaMatch = 0f;

        foreach (MapInstance map in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
        {
            if (x >= map.X && x <= map.X + Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth)
            {
                if (y >= map.Y && y <= map.Y + Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight)
                {
                    //Remove the offsets to just be dealing with pixels within the map selected
                    x -= (int)map.X;
                    y -= (int)map.Y;

                    //transform pixel format to tile format
                    x /= Options.Instance.Map.TileWidth;
                    y /= Options.Instance.Map.TileHeight;
                    var mapId = map.Id;

                    if (TryGetRealLocation(ref x, ref y, ref mapId))
                    {
                        foreach (var en in Globals.Entities)
                        {
                            if (en.Value == null ||
                                en.Value.MapId != mapId ||
                                (en.Value is Projectile or Resource) ||
                                (en.Value.IsStealthed && Globals.Me?.IsInMyParty(en.Value.Id) == false)
                            )
                            {
                                continue;
                            }

                            var intersectRect = FloatRect.Intersect(en.Value.WorldPos, targetRect);
                            if (intersectRect.Width * intersectRect.Height > bestAreaMatch)
                            {
                                bestAreaMatch = intersectRect.Width * intersectRect.Height;
                                bestMatch = en.Value;
                            }
                        }

                        foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
                        {
                            foreach (var en in eventMap.LocalEntities)
                            {
                                if (en.Value == null || en.Value.MapId != mapId || ((Event)en.Value).DisablePreview)
                                {
                                    continue;
                                }

                                var intersectRect = FloatRect.Intersect(en.Value.WorldPos, targetRect);
                                if (intersectRect.Width * intersectRect.Height > bestAreaMatch)
                                {
                                    bestAreaMatch = intersectRect.Width * intersectRect.Height;
                                    bestMatch = en.Value;
                                }
                            }
                        }

                        if (bestMatch != null && bestMatch.Id != TargetId)
                        {
                            Target = bestMatch;

                            if (bestMatch is Player && Interface.Interface.GameUi.IsAdminWindowOpen)
                            {
                                // Select in admin window if open
                                Interface.Interface.GameUi.AdminWindowSelectName(bestMatch.Name);
                            }

                            return true;
                        }

                        if (!Globals.Database.StickyTarget)
                        {
                            // We've clicked off of our target and are allowed to clear it!
                            return ClearTarget();
                        }
                    }

                    return false;
                }
            }
        }

        return false;
    }

    public bool TryTarget(IEntity entity, bool force = false)
    {
        //Check for taunt status if so don't allow to change target
        for (var i = 0; i < Status.Count; i++)
        {
            if (Status[i].Type == SpellEffect.Taunt && !force)
            {
                return false;
            }
        }

        if (entity == null)
        {
            return false;
        }

        // Are we already targetting this?
        if (TargetBox != null && TargetBox.MyEntity == entity)
        {
            return true;
        }

        var targetType = entity is Event ? 1 : 0;

        if (entity is Player)
        {
            //Select in admin window if open
            if (Interface.Interface.GameUi.IsAdminWindowOpen)
            {
                Interface.Interface.GameUi.AdminWindowSelectName(entity.Name);
            }
        }

        if (TargetId != entity.Id)
        {
            Target = entity;
        }

        return true;

    }

    private IEntity? _target;

    public IEntity? Target
    {
        get => _target;
        set
        {
            if (value == _target)
            {
                return;
            }

            _target = value;

            if (value == null)
            {
                TargetId = default;
                TargetType = 0;
            }
            else
            {
                TargetId = value.Id;
                TargetType = value is Event ? 1 : 0;
            }

            SetTargetBox(value as Entity);
        }
    }

    public bool ClearTarget()
    {
        _target = null;

        if (TargetId == default && TargetType == -1)
        {
            return false;
        }

        if (TargetId != default)
        {
            PacketSender.SendTarget(default);
            SetTargetBox(null);
        }

        TargetId = default;
        TargetType = -1;
        return true;
    }

    /// <summary>
    /// Attempts to pick up an item at the specified location.
    /// </summary>
    /// <param name="mapId">The Id of the map we are trying to loot from.</param>
    /// <param name="tileIndex"> The index of the tile we are trying to loot from.</param>
    /// <param name="uniqueId">The Unique Id of the specific item we want to pick up, leave <see cref="Guid.Empty"/> to not specificy an item and pick up the first thing we can find.</param>
    /// <param name="firstOnly">Defines whether we only want to pick up the first item we can find when true, or all items when false.</param>
    /// <returns> Whether the action was successful or not </returns>
    public static bool TryPickupItem(Guid mapId, int tileIndex, Guid uniqueId = new(), bool firstOnly = false)
    {
        var map = Maps.MapInstance.Get(mapId);
        if (map == null || tileIndex < 0 || tileIndex >= Options.Instance.Map.MapWidth * Options.Instance.Map.MapHeight)
        {
            return false;
        }

        // Are we trying to pick up anything in particular, or everything?
        if (uniqueId != Guid.Empty || firstOnly)
        {
            if (!map.MapItems.TryGetValue(tileIndex, out var value) || value.Count < 1)
            {
                return false;
            }

            foreach (var item in value)
            {
                // Check if we are trying to pick up a specific item, and if this is the one.
                if (uniqueId != Guid.Empty && item.Id != uniqueId)
                {
                    continue;
                }

                PacketSender.SendPickupItem(mapId, tileIndex, item.Id);
                return true;
            }
        }
        else
        {
            // Let the server worry about what we can and can not pick up.
            PacketSender.SendPickupItem(mapId, tileIndex, uniqueId);
            return true;
        }

        return false;
    }

    //Forumlas
    public long GetNextLevelExperience()
    {
        return ExperienceToNextLevel;
    }

    public override int CalculateAttackTime()
    {
        ItemBase? weapon = null;
        var attackTime = base.CalculateAttackTime();

        var cls = ClassBase.Get(Class);
        if (cls != null && cls.AttackSpeedModifier == 1) //Static
        {
            attackTime = cls.AttackSpeedValue;
        }

        if (this == Globals.Me)
        {
            if (Options.Instance.Equipment.WeaponSlot > -1 &&
                Options.Instance.Equipment.WeaponSlot < Equipment.Length &&
                MyEquipment[Options.Instance.Equipment.WeaponSlot] >= 0)
            {
                weapon = ItemBase.Get(Inventory[MyEquipment[Options.Instance.Equipment.WeaponSlot]].ItemId);
            }
        }
        else
        {
            if (Options.Instance.Equipment.WeaponSlot > -1 &&
                Options.Instance.Equipment.WeaponSlot < Equipment.Length &&
                Equipment[Options.Instance.Equipment.WeaponSlot] != Guid.Empty)
            {
                weapon = ItemBase.Get(Equipment[Options.Instance.Equipment.WeaponSlot]);
            }
        }

        if (weapon != null)
        {
            if (weapon.AttackSpeedModifier == 1) // Static
            {
                attackTime = weapon.AttackSpeedValue;
            }
            else if (weapon.AttackSpeedModifier == 2) //Percentage
            {
                attackTime = (int)(attackTime * (100f / weapon.AttackSpeedValue));
            }
        }

        return attackTime;
    }

    /// <summary>
    /// Calculate the attack time for the player as if they have a specified speed stat.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public virtual int CalculateAttackTime(int speed)
    {
        return (int)(Options.Instance.Combat.MaxAttackRate +
                      (Options.Instance.Combat.MinAttackRate - Options.Instance.Combat.MaxAttackRate) *
                      (((float)Options.Instance.Player.MaxStat - speed) /
                       Options.Instance.Player.MaxStat));
    }

    //Movement Processing
    private void ProcessDirectionalInput()
    {
        if (Globals.Me == default || Globals.MapGrid == default)
        {
            return;
        }

        //Check if player is crafting
        if (Globals.InCraft)
        {
            return;
        }

        //check if player is stunned or snared, if so don't let them move.
        for (var n = 0; n < Status.Count; n++)
        {
            if (Status[n].Type is SpellEffect.Stun or SpellEffect.Snare or SpellEffect.Sleep)
            {
                return;
            }
        }

        //Check if the player is dashing, if so don't let them move.
        if (Dashing != null || DashQueue.Count > 0 || DashTimer > Timing.Global.Milliseconds)
        {
            return;
        }

        if (IsAttacking && !Options.Instance.Player.AllowCombatMovement)
        {
            return;
        }

        Point position = new(X, Y);
        IEntity? blockedBy = null;

        if (MoveDir <= Direction.None || Globals.EventDialogs.Count != 0)
        {
            return;
        }

        //Try to move if able and not casting spells.
        if (IsMoving || MoveTimer >= Timing.Global.Milliseconds ||
            (!Options.Instance.Combat.MovementCancelsCast && IsCasting))
        {
            return;
        }

        if (Options.Instance.Combat.MovementCancelsCast)
        {
            CastTime = 0;
        }

        var dir = Dir;
        var moveDir = MoveDir;

        var enableCrossingDiagonalBlocks = Options.Instance.Map.EnableCrossingDiagonalBlocks;

        if (moveDir != Direction.None)
        {
            List<Direction> possibleDirections = new(4) { moveDir };
            if (dir.IsAdjacent(moveDir))
            {
                System.Console.WriteLine($"{dir} is adjacent to {moveDir}");
                possibleDirections.Add(dir);
            }

            if (moveDir.IsDiagonal())
            {
                possibleDirections.AddRange(moveDir.GetComponentDirections());
            }

            foreach (var possibleDirection in possibleDirections)
            {
                var delta = possibleDirection.GetDeltaPoint();
                var target = position + delta;
                if (IsTileBlocked(target, Z, MapId, ref blockedBy) != -1)
                {
                    continue;
                }

                if (!enableCrossingDiagonalBlocks && possibleDirection.IsDiagonal())
                {
                    if (possibleDirection.GetComponentDirections()
                        .Select(componentDirection => componentDirection.GetDeltaPoint() + position)
                        .All(componentTarget => IsTileBlocked(componentTarget, Z, MapId, ref blockedBy) != -1))
                    {
                        continue;
                    }
                }

                position.X += delta.X;
                position.Y += delta.Y;
                IsMoving = true;
                Dir = possibleDirection;

                if (delta.X == 0)
                {
                    OffsetX = 0;
                }
                else
                {
                    OffsetX = delta.X > 0 ? -Options.Instance.Map.TileWidth : Options.Instance.Map.TileWidth;
                }

                if (delta.Y == 0)
                {
                    OffsetY = 0;
                }
                else
                {
                    OffsetY = delta.Y > 0 ? -Options.Instance.Map.TileHeight : Options.Instance.Map.TileHeight;
                }

                break;
            }
        }

        if (blockedBy != mLastBumpedEvent)
        {
            mLastBumpedEvent = null;
        }

        if (IsMoving)
        {
            if (position.X < 0 || position.Y < 0 || position.X > Options.Instance.Map.MapWidth - 1 || position.Y > Options.Instance.Map.MapHeight - 1)
            {
                var gridX = Maps.MapInstance.Get(Globals.Me.MapId).GridX;
                var gridY = Maps.MapInstance.Get(Globals.Me.MapId).GridY;
                if (position.X < 0)
                {
                    gridX--;
                    X = (byte)(Options.Instance.Map.MapWidth - 1);
                }
                else if (position.X >= Options.Instance.Map.MapWidth)
                {
                    X = 0;
                    gridX++;
                }
                else
                {
                    X = (byte)position.X;
                }

                if (position.Y < 0)
                {
                    gridY--;
                    Y = (byte)(Options.Instance.Map.MapHeight - 1);
                }
                else if (position.Y >= Options.Instance.Map.MapHeight)
                {
                    Y = 0;
                    gridY++;
                }
                else
                {
                    Y = (byte)position.Y;
                }

                if (MapId != Globals.MapGrid[gridX, gridY])
                {
                    MapId = Globals.MapGrid[gridX, gridY];
                    FetchNewMaps();
                }
            }
            else
            {
                X = (byte)position.X;
                Y = (byte)position.Y;
            }

            TryToChangeDimension();
            PacketSender.SendMove();
            MoveTimer = (Timing.Global.Milliseconds) + (long)GetMovementTime();
        }
        else
        {
            if (MoveDir != Dir)
            {
                Dir = MoveDir;
                PacketSender.SendDirection(Dir);
                PickLastDirection(Dir);
            }

            if (blockedBy != null && mLastBumpedEvent != blockedBy && blockedBy is Event)
            {
                PacketSender.SendBumpEvent(blockedBy.MapId, blockedBy.Id);
                mLastBumpedEvent = (Entity)blockedBy;
            }
        }
    }

    public static void FetchNewMaps()
    {
        if (Globals.MapGridWidth == 0 || Globals.MapGridHeight == 0)
        {
            return;
        }

        PacketSender.SendNeedMapForGrid();
    }

    public override void DrawEquipment(string filename, Color renderColor)
    {
        //check if player is stunned or snared, if so don't let them move.
        for (var n = 0; n < Status.Count; n++)
        {
            if (Status[n].Type == SpellEffect.Transform)
            {
                return;
            }
        }

        base.DrawEquipment(filename, renderColor);
    }

    //Override of the original function, used for rendering the color of a player based on rank
    public override void DrawName(Color? textColor, Color? borderColor, Color? backgroundColor)
    {
        if (textColor == null)
        {
            switch (AccessLevel)
            {
                case Access.Moderator:
                    textColor = CustomColors.Names.Players["Moderator"].Name;
                    borderColor = CustomColors.Names.Players["Moderator"].Outline;
                    backgroundColor = CustomColors.Names.Players["Moderator"].Background;
                    break;

                case Access.Admin:
                    textColor = CustomColors.Names.Players["Admin"].Name;
                    borderColor = CustomColors.Names.Players["Admin"].Outline;
                    backgroundColor = CustomColors.Names.Players["Admin"].Background;
                    break;

                case Access.None:
                default:
                    textColor = CustomColors.Names.Players["Normal"].Name;
                    borderColor = CustomColors.Names.Players["Normal"].Outline;
                    backgroundColor = CustomColors.Names.Players["Normal"].Background;
                    break;
            }
        }

        var customColorOverride = NameColor;
        if (customColorOverride != null)
        {
            //We don't want to override the default colors if the color is transparent!
            if (customColorOverride.A != 0)
            {
                textColor = customColorOverride;
            }
        }

        if (Globals.Me != default && Globals.Me.Id != Id)
        {
            // Party member names
            if (Globals.Me.IsInMyParty(this) && CustomColors.Names.Players.TryGetValue(nameof(Party), out var partyColors))
            {
                textColor = partyColors.Name;
                borderColor = partyColors.Outline;
                backgroundColor = partyColors.Background;
            }

            // Guildies
            else if (Globals.Me.IsInGuild && Guild == Globals.Me.Guild && CustomColors.Names.Players.TryGetValue(nameof(Guild), out var guildColors))
            {
                textColor = guildColors.Name;
                borderColor = guildColors.Outline;
                backgroundColor = guildColors.Background;
            }

            // Enemies in PvP
            if (Globals.Me.IsAllyOf(this) && Globals.Me.MapInstance?.ZoneType != MapZone.Safe && CustomColors.Names.Players.TryGetValue("Hostile", out var hostileColors))
            {
                textColor = hostileColors.Name;
                borderColor = hostileColors.Outline;
                backgroundColor = hostileColors.Background;
            }
        }

        DrawNameAndLabels(textColor, borderColor, backgroundColor);
    }

    public override bool IsAllyOf(Player en)
    {
        if (base.IsAllyOf(en))
        {
            return true;
        }

        return IsInMyParty(en) || IsInMyGuild(en) || en.MapInstance?.ZoneType == MapZone.Safe;
    }

    private void DrawNameAndLabels(Color textColor, Color? borderColor, Color? backgroundColor)
    {
        base.DrawName(textColor, borderColor, backgroundColor);
        DrawLabels(HeaderLabel.Text, 0, HeaderLabel.Color, textColor, borderColor, backgroundColor);
        DrawLabels(FooterLabel.Text, 1, FooterLabel.Color, textColor, borderColor, backgroundColor);
        DrawGuildName(textColor, borderColor, backgroundColor);
    }

    public virtual void DrawGuildName(Color textColor, Color? borderColor = default, Color? backgroundColor = default)
    {
        if (Graphics.Renderer == default || Globals.Me == default)
        {
            return;
        }

        var guildLabel = Guild?.Trim();
        if (!ShouldDrawName || string.IsNullOrWhiteSpace(guildLabel) || !Options.Instance.Guild.ShowGuildNameTagsOverMembers)
        {
            return;
        }

        if (IsStealthed && !IsInMyParty(Globals.Me))
        {
            // Do not render if the party is stealthed and not in the local player's party
            return;
        }

        if (MapInstance == default)
        {
            return;
        }

        var textSize = Graphics.Renderer.MeasureText(guildLabel, Graphics.EntityNameFont, 1);

        var x = (int)Math.Ceiling(Origin.X);
        var y = GetLabelLocation(LabelType.Guild);

        backgroundColor ??= Color.Transparent;
        if (backgroundColor != Color.Transparent)
        {
            Graphics.DrawGameTexture(
                Graphics.Renderer.WhitePixel,
                new FloatRect(0, 0, 1, 1),
                new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y),
                backgroundColor
            );
        }

        borderColor ??= Color.Transparent;
        Graphics.Renderer.DrawString(
            guildLabel,
            Graphics.EntityNameFont,
            x - (int)Math.Ceiling(textSize.X / 2f),
            (int)y,
            1,
            Color.FromArgb(textColor.ToArgb()),
            true,
            default,
            Color.FromArgb(borderColor.ToArgb())
        );
    }

    protected override bool ShouldDrawHpBar
    {
        get
        {
            if (ShouldNotDrawHpBar)
            {
                return false;
            }

            if (IsHovered)
            {
                return true;
            }

            var me = Globals.Me;

            if (me == default)
            {
                return false;
            }

            if (Globals.Database.MyOverheadHpBar && Id == me.Id)
            {
                return true;
            }

            if (Globals.Database.PlayerOverheadHpBar && Id != me.Id)
            {
                return true;
            }

            if (Globals.Database.PartyMemberOverheadHpBar && me.IsInMyParty(this))
            {
                return true;
            }

            if (Globals.Database.FriendOverheadHpBar && me.IsFriend(this))
            {
                return true;
            }

            return Globals.Database.GuildMemberOverheadHpBar && me.IsGuildMate(this);
        }
    }

    public void DrawTargets()
    {
        foreach (var en in Globals.Entities)
        {
            if (en.Value == null)
            {
                continue;
            }

            if (en.Value.IsHidden)
            {
                continue;
            }

            if (en.Value.IsStealthed && (en.Value is not Player player || Globals.Me?.IsInMyParty(player) == false))
            {
                continue;
            }

            if (en.Value is Projectile or Resource)
            {
                continue;
            }

            if (TargetType != 0 || TargetId != en.Value.Id)
            {
                continue;
            }

            en.Value.DrawTarget((int)Enums.TargetType.Selected);
            AutoTurnToTarget(en.Value);
            ToggleTargetContextMenu(en.Value);
        }

        foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
        {
            foreach (var en in eventMap.LocalEntities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                if (en.Value.MapId != eventMap.Id)
                {
                    continue;
                }

                if (en.Value is Event eventEntity && eventEntity.DisablePreview)
                {
                    continue;
                }

                if (en.Value.IsHidden)
                {
                    continue;
                }

                if (en.Value.IsStealthed && (en.Value is not Player player || Globals.Me?.IsInMyParty(player) == false))
                {
                    continue;
                }

                if (TargetType != 1 || TargetId != en.Value.Id)
                {
                    continue;
                }

                en.Value.DrawTarget((int)Enums.TargetType.Selected);
                AutoTurnToTarget(en.Value);
                ToggleTargetContextMenu(en.Value);
            }
        }

        if (!Interface.Interface.DoesMouseHitInterface())
        {
            var mouseInWorld = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
            foreach (MapInstance map in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
            {
                if (mouseInWorld.X >= map.X && mouseInWorld.X <= map.X + Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth)
                {
                    if (mouseInWorld.Y >= map.Y &&
                        mouseInWorld.Y <= map.Y + Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight)
                    {
                        var mapId = map.Id;

                        foreach (var en in Globals.Entities)
                        {
                            if (en.Value == null)
                            {
                                continue;
                            }

                            if (en.Value.MapId == mapId &&
                                !en.Value.HideName &&
                                (!en.Value.IsStealthed ||
                                 en.Value is Player player && Globals.Me?.IsInMyParty(player) == true) &&
                                en.Value.WorldPos.Contains(mouseInWorld.X, mouseInWorld.Y))
                            {
                                if (en.Value is not (Projectile or Resource))
                                {
                                    if (TargetType != 0 || TargetId != en.Value.Id)
                                    {
                                        en.Value.DrawTarget((int)Enums.TargetType.Hover);
                                        ToggleTargetContextMenu(en.Value);
                                    }
                                }
                            }
                        }

                        foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values.Cast<MapInstance>())
                        {
                            foreach (var en in eventMap.LocalEntities)
                            {
                                if (en.Value == null)
                                {
                                    continue;
                                }

                                if (en.Value.MapId == mapId &&
                                    !((Event)en.Value).DisablePreview &&
                                    !en.Value.IsHidden &&
                                    (!en.Value.IsStealthed ||
                                     en.Value is Player player && Globals.Me?.IsInMyParty(player) == true) &&
                                    en.Value.WorldPos.Contains(mouseInWorld.X, mouseInWorld.Y))
                                {
                                    if (TargetType != 1 || TargetId != en.Value.Id)
                                    {
                                        en.Value.DrawTarget((int)Enums.TargetType.Hover);
                                        ToggleTargetContextMenu(en.Value);
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    private class TargetInfo
    {
        public long LastTimeSelected;

        public int DistanceTo;
    }

    private void TurnAround()
    {
        if (Globals.Me == default)
        {
            return;
        }

        // If players hold the 'TurnAround' Control Key and tap to any direction, they will turn on their own axis.
        for (var direction = 0; direction < Options.Instance.Map.MovementDirections; direction++)
        {
            if (!Controls.IsControlPressed(Control.TurnAround) || direction != (int)Globals.Me.MoveDir || IsTurnAroundWhileCastingDisabled)
            {
                continue;
            }

            // Turn around and hold the player in place if the requested direction is different from the current one.
            if (!Globals.Me.IsMoving && Dir != Globals.Me.MoveDir)
            {
                Dir = Globals.Me.MoveDir;
                PacketSender.SendDirection(Dir);
                Globals.Me.MoveDir = Direction.None;
                PickLastDirection(Dir);
            }

            // Hold the player in place if the requested direction is the same as the current one.
            if (!Globals.Me.IsMoving && Dir == Globals.Me.MoveDir)
            {
                Globals.Me.MoveDir = Direction.None;
            }
        }
    }

    // Checks if the target is at the opposite direction of the current player's direction.
    // The comparison also takes into account whether diagonal movement is enabled or not.
    private static bool IsTargetAtOppositeDirection(Direction currentDir, Direction targetDir)
    {
        if (Options.Instance.Map.EnableDiagonalMovement)
        {
            // If diagonal movement is disabled, check opposite directions on 4 directions.
            switch (currentDir)
            {
                case Direction.Up:
                    return targetDir is Direction.Down or Direction.DownLeft or Direction.DownRight;

                case Direction.Down:
                    return targetDir is Direction.Up or Direction.UpLeft or Direction.UpRight;

                case Direction.Left:
                    return targetDir is Direction.Right or Direction.UpRight or Direction.DownRight;

                case Direction.Right:
                    return targetDir is Direction.Left or Direction.UpLeft or Direction.DownLeft;

                default:
                    if (!Options.Instance.Map.EnableDiagonalMovement)
                    {
                        return false;
                    }

                    break;
            }
        }

        // If diagonal movement is enabled, check opposite directions on 8 directions.
        return currentDir switch
        {
            Direction.UpLeft => targetDir is Direction.DownRight or Direction.Right or Direction.Down,
            Direction.UpRight => targetDir is Direction.DownLeft or Direction.Left or Direction.Down,
            Direction.DownLeft => targetDir is Direction.UpRight or Direction.Right or Direction.Up,
            Direction.DownRight => targetDir is Direction.UpLeft or Direction.Left or Direction.Up,
            _ => false,
        };
    }
}
