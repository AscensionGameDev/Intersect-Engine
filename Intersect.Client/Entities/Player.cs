using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.EntityPanel;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Config.Guilds;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

namespace Intersect.Client.Entities
{

    public partial class Player : Entity, IPlayer
    {

        public delegate void InventoryUpdated();

        public Guid Class { get; set; }

        public long Experience { get; set; } = 0;

        public long ExperienceToNextLevel { get; set; } = 0;

        IReadOnlyList<IFriendInstance> IPlayer.Friends => Friends;

        public List<IFriendInstance> Friends { get; set; } = new List<IFriendInstance>();

        IReadOnlyList<IHotbarInstance> IPlayer.HotbarSlots => Hotbar.ToList();

        public HotbarInstance[] Hotbar { get; set; } = new HotbarInstance[Options.Instance.PlayerOpts.HotbarSlotCount];

        public InventoryUpdated InventoryUpdatedDelegate { get; set; }

        IReadOnlyDictionary<Guid, long> IPlayer.ItemCooldowns => ItemCooldowns;

        public Dictionary<Guid, long> ItemCooldowns { get; set; } = new Dictionary<Guid, long>();

        private Entity mLastBumpedEvent = null;

        private List<IPartyMember> mParty;

        IReadOnlyDictionary<Guid, QuestProgress> IPlayer.QuestProgress => QuestProgress;

        public Dictionary<Guid, QuestProgress> QuestProgress { get; set; } = new Dictionary<Guid, QuestProgress>();

        public Guid[] HiddenQuests { get; set; } = new Guid[0];

        IReadOnlyDictionary<Guid, long> IPlayer.SpellCooldowns => SpellCooldowns;

        public Dictionary<Guid, long> SpellCooldowns { get; set; } = new Dictionary<Guid, long>();

        public int StatPoints { get; set; } = 0;

        public EntityBox TargetBox { get; set; }

        public Guid TargetIndex { get; set; }

        TargetTypes IPlayer.TargetType => (TargetTypes) TargetType;

        public int TargetType { get; set; }

        public long CombatTimer { get; set; } = 0;

        public long IsCastingCheckTimer { get; set; }

        // Target data
        private long mlastTargetScanTime = 0;

        Guid mlastTargetScanMap = Guid.Empty;

        Point mlastTargetScanLocation = new Point(-1, -1);

        Dictionary<Entity, TargetInfo> mlastTargetList = new Dictionary<Entity, TargetInfo>(); // Entity, Last Time Selected

        Entity mLastEntitySelected = null;

        private Dictionary<int, long> mLastHotbarUseTime = new Dictionary<int, long>();
        private int mHotbarUseDelay = 150;

        /// <summary>
        /// Name of our guild if we are in one.
        /// </summary>
        public string Guild { get; set; }

        string IPlayer.GuildName => Guild;

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
        public GuildRank GuildRank => IsInGuild ? Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(this.Rank, Options.Instance.Guild.Ranks.Length - 1))] : null;

        /// <summary>
        /// Contains a record of all members of this player's guild.
        /// </summary>
        public GuildMember[] GuildMembers = new GuildMember[0];

        public Player(Guid id, PlayerEntityPacket packet) : base(id, packet, EntityTypes.Player)
        {
            for (var i = 0; i < Options.Instance.PlayerOpts.HotbarSlotCount; i++)
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
                if (mParty == null)
                {
                    mParty = new List<IPartyMember>();
                }

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
            // TODO: Friend List is updating only when opening it's GUI Window? It Should be updated upon login.
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

        public bool IsInMyParty(IPlayer player) => IsInMyParty(player.Id);

        public bool IsInMyParty(Guid id) => Party.Any(member => member.Id == id);

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

                if (Controls.KeyDown(Control.AttackInteract))
                {
                    if (IsCasting)
                    {
                        if (IsCastingCheckTimer < Timing.Global.Milliseconds &&
                            Options.Combat.EnableCombatChatMessages)
                        {
                            ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Combat.AttackWhileCastingDeny,
                                CustomColors.Alerts.Declined, ChatMessageType.Combat));
                            IsCastingCheckTimer = Timing.Global.Milliseconds + 350;
                        }
                    }
                    else if (!Globals.Me.TryAttack())
                    {
                        if (Globals.Me.AttackTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond)
                        {
                            Globals.Me.AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + Globals.Me.CalculateAttackTime();
                        }
                    }
                }

                //Holding block button for "auto blocking"
                if(Controls.KeyDown(Control.Block))
                {
                    TryBlock();
                }
            }

            if (TargetBox == default && this == Globals.Me && Interface.Interface.GameUi != default)
            {
                // If for WHATEVER reason the box hasn't been created, create it.
                TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityTypes.Player, null);
                TargetBox.Hide();
            }
            else if (TargetIndex != default)
            {
                if (!Globals.Entities.TryGetValue(TargetIndex, out var foundEntity))
                {
                    foundEntity = TargetBox.MyEntity?.MapInstance?.Entities.FirstOrDefault(entity => entity.Id == TargetIndex) as Entity;
                }

                if (foundEntity == default || foundEntity.IsHidden || foundEntity.IsStealthed)
                {
                    ClearTarget();
                }
            }

            TargetBox?.Update();

            // Hide our Guild window if we're not in a guild!
            if (this == Globals.Me && string.IsNullOrEmpty(Guild) && Interface.Interface.GameUi != null)
            {
                Interface.Interface.GameUi.HideGuildWindow();
            }

            var returnval = base.Update();

            return returnval;
        }

        //Loading
        public override void Load(EntityPacket packet)
        {
            base.Load(packet);
            var pkt = (PlayerEntityPacket)packet;
            Gender = pkt.Gender;
            Class = pkt.ClassId;
            Aggression = pkt.AccessLevel;
            CombatTimer = pkt.CombatTimeRemaining + Timing.Global.Milliseconds;
            Guild = pkt.Guild;
            Rank = pkt.GuildRank;

            var playerPacket = (PlayerEntityPacket)packet;

            if (playerPacket.Equipment != null)
            {
                if (this == Globals.Me && playerPacket.Equipment.InventorySlots != null)
                {
                    this.MyEquipment = playerPacket.Equipment.InventorySlots;
                }
                else if (playerPacket.Equipment.ItemIds != null)
                {
                    this.Equipment = playerPacket.Equipment.ItemIds;
                }
            }

            if (this == Globals.Me && TargetBox == null && Interface.Interface.GameUi != null)
            {
                TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityTypes.Player, null);
                TargetBox.Hide();
            }
        }

        //Item Processing
        public void SwapItems(int item1, int item2)
        {
            var tmpInstance = Inventory[item2].Clone();
            Inventory[item2] = Inventory[item1].Clone();
            Inventory[item1] = tmpInstance.Clone();
        }

        public void TryDropItem(int index)
        {
            var slot = Inventory[index];
            var descriptor = ItemBase.Get(slot.ItemId);
            if (descriptor == default)
            {
                return;
            }

            var quantity = slot.Quantity;
            var canDropMultiple = quantity > 1;
            var inputType = canDropMultiple ? InputBox.InputType.NumericSliderInput : InputBox.InputType.YesNo;
            var prompt = canDropMultiple ? Strings.Inventory.DropItemPrompt : Strings.Inventory.DropPrompt;
            InputBox.Open(
                title: Strings.Inventory.DropItemTitle,
                prompt: prompt.ToString(descriptor.Name),
                modal: true,
                inputType: inputType,
                onSuccess: DropInputBoxOkay,
                onCancel: default,
                userData: index,
                quantity: quantity,
                maxQuantity: quantity
            );
        }

        private void DropInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendDropItem((int)((InputBox)sender).UserData, value);
            }
        }

        public int FindItem(Guid itemId, int itemVal = 1)
        {
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                if (Inventory[i].ItemId == itemId && Inventory[i].Quantity >= itemVal)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetItemQuantity(Guid itemId)
        {
            long count = 0;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                if (Inventory[i].ItemId == itemId)
                {
                    count += Inventory[i].Quantity;
                }
            }

            return count > Int32.MaxValue ? Int32.MaxValue : (int)count;
        }

        public void TryUseItem(int index)
        {
            if (!IsItemOnCooldown(index) &&
                index >= 0 && index < Globals.Me.Inventory.Length && Globals.Me.Inventory[index]?.Quantity > 0)
            {
                PacketSender.SendUseItem(index, TargetIndex);
            }
        }

        public long GetItemCooldown(Guid id)
        {
            if (ItemCooldowns.ContainsKey(id))
            {
                return ItemCooldowns[id];
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
                            if (itemBase.ItemType == ItemTypes.Bag)
                            {
                                if (hotbarInstance.BagId == itm.BagId)
                                {
                                    break;
                                }
                            }
                            else if (itemBase.ItemType == ItemTypes.Equipment)
                            {
                                if (hotbarInstance.PreferredStatBuffs != null)
                                {
                                    var statMatch = true;
                                    for (var s = 0; s < hotbarInstance.PreferredStatBuffs.Length; s++)
                                    {
                                        if (itm.StatBuffs[s] != hotbarInstance.PreferredStatBuffs[s])
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
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
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
                    if (ItemCooldowns.ContainsKey(itm.ItemId) && ItemCooldowns[itm.ItemId] > Timing.Global.Milliseconds)
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
                    if (ItemCooldowns.ContainsKey(itm.ItemId) && ItemCooldowns[itm.ItemId] > Timing.Global.Milliseconds)
                    {
                        return ItemCooldowns[itm.ItemId] - Timing.Global.Milliseconds;
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
                    if (SpellCooldowns.ContainsKey(spl.Id) && SpellCooldowns[spl.Id] > Timing.Global.Milliseconds)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public long GetSpellRemainingCooldown(int slot)
        {
            if (Spells[slot] != null)
            {
                var spl = Spells[slot];
                if (spl.Id != Guid.Empty)
                {
                    if (SpellCooldowns.ContainsKey(spl.Id) && SpellCooldowns[spl.Id] > Timing.Global.Milliseconds)
                    {
                        return ItemCooldowns[spl.Id] - Timing.Global.Milliseconds;
                    }
                }
            }

            return 0;
        }

        public decimal GetCooldownReduction()
        {
            var cooldown = 0;

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (MyEquipment[i] > -1)
                {
                    if (Inventory[MyEquipment[i]].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Inventory[MyEquipment[i]].ItemId);
                        if (item != null)
                        {
                            //Check for cooldown reduction
                            if (item.Effect.Type == EffectType.CooldownReduction)
                            {
                                cooldown += item.Effect.Percentage;
                            }
                        }
                    }
                }
            }

            return cooldown;
        }

        public void TrySellItem(int index)
        {
            var slot = Inventory[index];
            var sellingItem = ItemBase.Get(slot.ItemId);
            if (sellingItem == null)
            {
                return;
            }

            var foundItem = -1;
            for (var i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
            {
                if (Globals.GameShop.BuyingItems[i].ItemId != slot.ItemId)
                {
                    continue;
                }

                foundItem = i;

                break;
            }

            var hasFoundItem = foundItem > -1 && Globals.GameShop.BuyingWhitelist ||
                               foundItem == -1 && !Globals.GameShop.BuyingWhitelist;
            var prompt = Strings.Shop.sellprompt;
            var inputType = InputBox.InputType.YesNo;
            EventHandler onSuccess = SellInputBoxOkay;
            var userData = index;
            var quantity = slot.Quantity;

            if (quantity > 1)
            {
                prompt = Strings.Shop.sellitemprompt;
                inputType = InputBox.InputType.NumericSliderInput;
                onSuccess = SellItemInputBoxOkay;
                userData = index;
            }

            if (!hasFoundItem)
            {
                prompt = Strings.Shop.cannotsell;
                inputType = InputBox.InputType.OkayOnly;
                onSuccess = null;
                userData = -1;
            }

            InputBox.Open(
                title: Strings.Shop.sellitem,
                prompt: prompt.ToString(sellingItem.Name),
                modal: true,
                inputType: inputType,
                onSuccess: onSuccess,
                onCancel: null,
                userData: userData,
                quantity: quantity,
                maxQuantity: quantity
            );
        }

        public void TryBuyItem(int slot)
        {
            //Confirm the purchase
            var item = ItemBase.Get(Globals.GameShop.SellingItems[slot].ItemId);
            if (item != null)
            {
                // Determine how many items we can purchase.
                var currencyCount = GetItemQuantity(Globals.GameShop.SellingItems[slot].CostItemId);
                var maxBuyAmount = (int)Math.Floor(currencyCount / (float)Globals.GameShop.SellingItems[slot].CostItemQuantity);

                // Is the item stackable, and can we make a purchase at all?
                if (item.IsStackable && maxBuyAmount != 0)
                {
                    var iBox = new InputBox(
                        Strings.Shop.buyitem, Strings.Shop.buyitemprompt.ToString(item.Name), true,
                        InputBox.InputType.NumericSliderInput, BuyItemInputBoxOkay, null, slot, maxBuyAmount, maxBuyAmount
                    );
                }
                // In any other case, attempt to purchase one and let the server handle the error and double checking.
                else
                {
                    PacketSender.SendBuyItem(slot, 1);
                }
            }
        }

        private void BuyItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendBuyItem((int)((InputBox)sender).UserData, value);
            }
        }

        private void SellItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendSellItem((int)((InputBox)sender).UserData, value);
            }
        }

        private void SellInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendSellItem((int)((InputBox)sender).UserData, 1);
        }

        //bank
        public void TryDepositItem(int index, int bankSlot = -1)
        {
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                //Permission Check
                if (Globals.GuildBank)
                {
                    var rank = Globals.Me.GuildRank;
                    if (string.IsNullOrWhiteSpace(Globals.Me.Guild) || (!rank.Permissions.BankDeposit && Globals.Me.Rank != 0))
                    {
                        ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Guilds.NotAllowedDeposit.ToString(Globals.Me.Guild), CustomColors.Alerts.Error, ChatMessageType.Bank));
                        return;
                    }
                }

                if (Inventory[index].Quantity > 1)
                {
                    int[] userData = new int[2] { index, bankSlot };

                    InputBox.Open(
                        title: Strings.Bank.deposititem,
                        prompt: Strings.Bank.deposititemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name),
                        modal: true,
                        inputType: InputBox.InputType.NumericSliderInput,
                        onSuccess: DepositItemInputBoxOkay,
                        onCancel: null,
                        userData: userData,
                        quantity: Inventory[index].Quantity,
                        maxQuantity: Inventory[index].Quantity
                    );
                }
                else
                {
                    PacketSender.SendDepositItem(index, 1, bankSlot);
                }
            }
        }

        private void DepositItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                int[] userData = (int[])((InputBox)sender).UserData;

                PacketSender.SendDepositItem(userData[0], value, userData[1]);
            }
        }

        public void TryWithdrawItem(int index, int invSlot = -1)
        {
            if (Globals.Bank[index] != null && ItemBase.Get(Globals.Bank[index].ItemId) != null)
            {
                //Permission Check
                if (Globals.GuildBank)
                {
                    var rank = Globals.Me.GuildRank;
                    if (string.IsNullOrWhiteSpace(Globals.Me.Guild) || (!rank.Permissions.BankRetrieve && Globals.Me.Rank != 0))
                    {
                        ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Guilds.NotAllowedWithdraw.ToString(Globals.Me.Guild), CustomColors.Alerts.Error, ChatMessageType.Bank));
                        return;
                    }
                }

                if (Globals.Bank[index].Quantity > 1)
                {
                    int[] userData = new int[2] { index, invSlot };

                    InputBox.Open(
                        title: Strings.Bank.withdrawitem,
                        prompt: Strings.Bank.withdrawitemprompt.ToString(ItemBase.Get(Globals.Bank[index].ItemId).Name),
                        modal: true,
                        inputType: InputBox.InputType.NumericSliderInput,
                        onSuccess: WithdrawItemInputBoxOkay,
                        onCancel: null,
                        userData: userData,
                        quantity: Globals.Bank[index].Quantity,
                        maxQuantity: Globals.Bank[index].Quantity
                    );
                }
                else
                {
                    PacketSender.SendWithdrawItem(index, 1, invSlot);
                }
            }
        }

        private void WithdrawItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                int[] userData = (int[])((InputBox)sender).UserData;
                PacketSender.SendWithdrawItem(userData[0], value, userData[1]);
            }
        }

        //Bag
        public void TryStoreBagItem(int invSlot, int bagSlot)
        {
            if (ItemBase.Get(Inventory[invSlot].ItemId) != null)
            {
                if (Inventory[invSlot].Quantity > 1)
                {
                    int[] userData = new int[2] { invSlot, bagSlot };

                    var iBox = new InputBox(
                        Strings.Bags.storeitem,
                        Strings.Bags.storeitemprompt.ToString(ItemBase.Get(Inventory[invSlot].ItemId).Name), true,
                        InputBox.InputType.NumericSliderInput, StoreBagItemInputBoxOkay, null, userData, Inventory[invSlot].Quantity,
                        Inventory[invSlot].Quantity
                    );
                }
                else
                {
                    PacketSender.SendStoreBagItem(invSlot, 1, bagSlot);
                }
            }
        }

        private void StoreBagItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                int[] userData = (int[])((InputBox)sender).UserData;
                PacketSender.SendStoreBagItem(userData[0], value, userData[1]);
            }
        }

        public void TryRetreiveBagItem(int bagSlot, int invSlot)
        {
            if (Globals.Bag[bagSlot] != null && ItemBase.Get(Globals.Bag[bagSlot].ItemId) != null)
            {
                int[] userData = new int[2] { bagSlot, invSlot };

                if (Globals.Bag[bagSlot].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Bags.retreiveitem,
                        Strings.Bags.retreiveitemprompt.ToString(ItemBase.Get(Globals.Bag[bagSlot].ItemId).Name), true,
                        InputBox.InputType.NumericSliderInput, RetreiveBagItemInputBoxOkay, null, userData, Globals.Bag[bagSlot].Quantity,
                        Globals.Bag[bagSlot].Quantity
                    );
                }
                else
                {
                    PacketSender.SendRetrieveBagItem(bagSlot, 1, invSlot);
                }
            }
        }

        private void RetreiveBagItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                int[] userData = (int[])((InputBox)sender).UserData;
                PacketSender.SendRetrieveBagItem(userData[0], value, userData[1]);
            }
        }

        //Trade
        public void TryTradeItem(int index)
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

            InputBox.Open(
                title: Strings.Trading.offeritem,
                prompt: Strings.Trading.offeritemprompt.ToString(tradingItem.Name),
                modal: true,
                inputType: InputBox.InputType.NumericSliderInput,
                onSuccess: TradeItemInputBoxOkay,
                onCancel: null,
                userData: index,
                quantity: quantity,
                maxQuantity: quantity
            );
        }

        private void TradeItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendOfferTradeItem((int)((InputBox)sender).UserData, value);
            }
        }

        public void TryRevokeItem(int index)
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

            InputBox.Open(
                title: Strings.Trading.revokeitem,
                prompt: Strings.Trading.revokeitemprompt.ToString(revokedItem.Name),
                modal: true,
                inputType: InputBox.InputType.NumericSliderInput,
                onSuccess: RevokeItemInputBoxOkay,
                onCancel: null,
                userData: index,
                quantity: quantity,
                maxQuantity: quantity
            );
        }

        private void RevokeItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendRevokeTradeItem((int)((InputBox)sender).UserData, value);
            }
        }

        //Spell Processing
        public void SwapSpells(int spell1, int spell2)
        {
            var tmpInstance = Spells[spell2].Clone();
            Spells[spell2] = Spells[spell1].Clone();
            Spells[spell1] = tmpInstance.Clone();
        }

        public void TryForgetSpell(int index)
        {
            if (SpellBase.Get(Spells[index].Id) != null)
            {
                var iBox = new InputBox(
                    Strings.Spells.forgetspell,
                    Strings.Spells.forgetspellprompt.ToString(SpellBase.Get(Spells[index].Id).Name), true,
                    InputBox.InputType.YesNo, ForgetSpellInputBoxOkay, null, index
                );
            }
        }

        private void ForgetSpellInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendForgetSpell((int)((InputBox)sender).UserData);
        }

        public void TryUseSpell(int index)
        {
            if (Spells[index].Id != Guid.Empty &&
                (!Globals.Me.SpellCooldowns.ContainsKey(Spells[index].Id) ||
                 Globals.Me.SpellCooldowns[Spells[index].Id] < Timing.Global.Milliseconds))
            {
                var spellBase = SpellBase.Get(Spells[index].Id);

                if (spellBase.CastDuration > 0 && (Options.Instance.CombatOpts.MovementCancelsCast && Globals.Me.IsMoving))
                {
                    return;
                }

                PacketSender.SendUseSpell(index, TargetIndex);
            }
        }

        public long GetSpellCooldown(Guid id)
        {
            if (SpellCooldowns.ContainsKey(id))
            {
                return SpellCooldowns[id];
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
            Hotbar[hotbarSlot].PreferredStatBuffs = new int[(int)Stats.StatCount];
            if (itemType == 0)
            {
                var item = Inventory[itemSlot];
                if (item != null)
                {
                    Hotbar[hotbarSlot].ItemOrSpellId = item.ItemId;
                    Hotbar[hotbarSlot].PreferredStatBuffs = item.StatBuffs;
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
            if (X < Options.MapWidth && X >= 0)
            {
                if (Y < Options.MapHeight && Y >= 0)
                {
                    if (Maps.MapInstance.Get(MapId) != null && Maps.MapInstance.Get(MapId).Attributes[X, Y] != null)
                    {
                        if (Maps.MapInstance.Get(MapId).Attributes[X, Y].Type == MapAttributes.ZDimension)
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
            var movex = 0f;
            var movey = 0f;
            if (Interface.Interface.HasInputFocus())
            {
                return;
            }

            if (Controls.KeyDown(Control.MoveUp))
            {
                movey = 1;
            }

            if (Controls.KeyDown(Control.MoveDown))
            {
                movey = -1;
            }

            if (Controls.KeyDown(Control.MoveLeft))
            {
                movex = -1;
            }

            if (Controls.KeyDown(Control.MoveRight))
            {
                movex = 1;
            }


            Globals.Me.MoveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                if (movey < 0)
                {
                    Globals.Me.MoveDir = 1;
                }

                if (movey > 0)
                {
                    Globals.Me.MoveDir = 0;
                }

                if (movex < 0)
                {
                    Globals.Me.MoveDir = 2;
                }

                if (movex > 0)
                {
                    Globals.Me.MoveDir = 3;
                }
            }

            var castInput = -1;
            for (var barSlot = 0; barSlot < Options.Instance.PlayerOpts.HotbarSlotCount; barSlot++)
            {
                if (!mLastHotbarUseTime.ContainsKey(barSlot))
                {
                    mLastHotbarUseTime.Add(barSlot, 0);
                }

                if (Controls.KeyDown((Control)barSlot + (int)Control.Hotkey1))
                {
                    castInput = barSlot;
                }
            }

            if (castInput != -1)
            {
                if (0 <= castInput && castInput < Interface.Interface.GameUi?.Hotbar?.Items?.Count && mLastHotbarUseTime[castInput] < Timing.Global.Milliseconds)
                {
                    Interface.Interface.GameUi?.Hotbar?.Items?[castInput]?.Activate();
                    mLastHotbarUseTime[castInput] = Timing.Global.Milliseconds + mHotbarUseDelay;
                }
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
                    var x1 = X + myMap.GridX * Options.MapWidth;
                    var y1 = Y + myMap.GridY * Options.MapHeight;

                    //Calculate world tile of target
                    var x2 = target.X + targetMap.GridX * Options.MapWidth;
                    var y2 = target.Y + targetMap.GridY * Options.MapHeight;

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
                if (Status[i].Type == StatusTypes.Taunt)
                {
                    return;
                }
            }

            // Do we need to account for players?
            // Depends on what type of map we're currently on.
            if (Globals.Me.MapInstance == null)
            {
                return;
            }
            var currentMap = Globals.Me.MapInstance as MapInstance;
            var canTargetPlayers = currentMap.ZoneType != MapZones.Safe;

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
                    if (!canTargetPlayers && en.Value.Type == EntityTypes.Player)
                    {
                        continue;
                    }
                    else if (canTargetPlayers && en.Value.Type == EntityTypes.Player)
                    {
                        var player = en.Value as Player;
                        if (IsInMyParty(player))
                        {
                            continue;
                        }
                    }

                    if (en.Value.Type == EntityTypes.GlobalEntity || en.Value.Type == EntityTypes.Player)
                    {
                        // Already in our list?
                        if (mlastTargetList.ContainsKey(en.Value))
                        {
                            mlastTargetList[en.Value].DistanceTo = GetDistanceTo(en.Value);
                        }
                        else
                        {
                            // Add entity with blank time. Never been selected.
                            mlastTargetList.Add(en.Value, new TargetInfo() { DistanceTo = GetDistanceTo(en.Value), LastTimeSelected = 0 });
                        }
                    }
                }

                // Remove old items.
                var toRemove = mlastTargetList.Where(en => !Globals.Entities.ContainsValue(en.Key)).ToArray();
                foreach (var en in toRemove)
                {
                    mlastTargetList.Remove(en.Key);
                }

                // Skip scanning for another second or so.. And set up other values.
                mlastTargetScanTime = Timing.Global.Milliseconds + 300;
                mlastTargetScanMap = MapId;
                mlastTargetScanLocation = new Point(X, Y);
            }

            // Find all valid entities in the direction we are facing.
            var validEntities = Array.Empty<KeyValuePair<Entity, TargetInfo>>();

            // TODO: Expose option to users
            if (Globals.Database.TargetAccountDirection)
            {
                switch (Dir)
                {
                    case (byte)Directions.Up:
                        validEntities = mlastTargetList.Where(en =>
                            ((en.Key.MapId == MapId || en.Key.MapId == currentMap.Left || en.Key.MapId == currentMap.Right) && en.Key.Y < Y) || en.Key.MapId == currentMap.Down)
                            .ToArray();
                        break;

                    case (byte)Directions.Down:
                        validEntities = mlastTargetList.Where(en =>
                            ((en.Key.MapId == MapId || en.Key.MapId == currentMap.Left || en.Key.MapId == currentMap.Right) && en.Key.Y > Y) || en.Key.MapId == currentMap.Up)
                            .ToArray();
                        break;

                    case (byte)Directions.Left:
                        validEntities = mlastTargetList.Where(en =>
                            ((en.Key.MapId == MapId || en.Key.MapId == currentMap.Up || en.Key.MapId == currentMap.Down) && en.Key.X < X) || en.Key.MapId == currentMap.Left)
                            .ToArray();
                        break;

                    case (byte)Directions.Right:
                        validEntities = mlastTargetList.Where(en =>
                                    ((en.Key.MapId == MapId || en.Key.MapId == currentMap.Up || en.Key.MapId == currentMap.Down) && en.Key.X > X) || en.Key.MapId == currentMap.Right)
                                    .ToArray();
                        break;
                }
            }
            else
            {
                validEntities = mlastTargetList.ToArray();
            }

            // Reduce the number of targets down to what is in our allowed range.
            validEntities = validEntities.Where(en => en.Value.DistanceTo <= Options.Combat.MaxPlayerAutoTargetRadius).ToArray();

            int currentDistance = 9999;
            long currentTime = Timing.Global.Milliseconds;
            Entity currentEntity = mLastEntitySelected;
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

            if (mlastTargetList.ContainsKey(currentEntity))
            {
                mlastTargetList[currentEntity].LastTimeSelected = Timing.Global.Milliseconds;
            }
            mLastEntitySelected = currentEntity as Entity;

            if (TargetIndex != currentEntity.Id)
            {
                SetTargetBox(currentEntity as Entity);
                TargetIndex = currentEntity.Id;
                TargetType = 0;
            }
        }

        private void SetTargetBox(Entity en)
        {
            if (en == null)
            {
                TargetBox?.SetEntity(null);
                TargetBox?.Hide();
                return;
            }

            if (en is Player)
            {
                TargetBox?.SetEntity(en, EntityTypes.Player);
            }
            else if (en is Event)
            {
                TargetBox?.SetEntity(en, EntityTypes.Event);
            }
            else
            {
                TargetBox?.SetEntity(en, EntityTypes.GlobalEntity);
            }

            TargetBox?.Show();
        }

        public bool TryBlock()
        {
            if (IsBlocking)
            {
                return false;
            }

            if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond)
            {
                return false;
            }

            if (Options.ShieldIndex > -1 && Globals.Me.MyEquipment[Options.ShieldIndex] > -1)
            {
                var item = ItemBase.Get(Globals.Me.Inventory[Globals.Me.MyEquipment[Options.ShieldIndex]].ItemId);
                if (item != null)
                {
                    PacketSender.SendBlock(true);
                    return true;
                }
            }

            return false;
        }

        public bool TryAttack()
        {
            if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond || IsBlocking || (IsMoving && !Options.Instance.PlayerOpts.AllowCombatMovement))
            {
                return false;
            }

            int x = Globals.Me.X;
            int y = Globals.Me.Y;
            var map = Globals.Me.MapId;
            switch (Globals.Me.Dir)
            {
                case 0:
                    y--;

                    break;
                case 1:
                    y++;

                    break;
                case 2:
                    x--;

                    break;
                case 3:
                    x++;

                    break;
            }

            if (TryGetRealLocation(ref x, ref y, ref map))
            {
                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value != Globals.Me)
                    {
                        if (en.Value.MapId == map &&
                            en.Value.X == x &&
                            en.Value.Y == y &&
                            en.Value.CanBeAttacked())
                        {
                            //ATTACKKKKK!!!
                            PacketSender.SendAttack(en.Key);
                            AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + CalculateAttackTime();

                            return true;
                        }
                    }
                }
            }

            foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values)
            {
                foreach (var en in eventMap.LocalEntities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value.MapId == map && en.Value.X == x && en.Value.Y == y)
                    {
                        if (en.Value is Event)
                        {
                            //Talk to Event
                            PacketSender.SendActivateEvent(en.Key);
                            AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + CalculateAttackTime();

                            return true;
                        }
                    }
                }
            }

            //Projectile/empty swing for animations
            PacketSender.SendAttack(Guid.Empty);
            AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + CalculateAttackTime();

            return true;
        }

        public bool TryGetRealLocation(ref int x, ref int y, ref Guid mapId)
        {
            var tmpX = x;
            var tmpY = y;
            var tmpI = -1;
            if (Maps.MapInstance.Get(mapId) != null)
            {
                var gridX = Maps.MapInstance.Get(mapId).GridX;
                var gridY = Maps.MapInstance.Get(mapId).GridY;

                if (x < 0)
                {
                    tmpX = Options.MapWidth - x * -1;
                    gridX--;
                }

                if (y < 0)
                {
                    tmpY = Options.MapHeight - y * -1;
                    gridY--;
                }

                if (y > Options.MapHeight - 1)
                {
                    tmpY = y - Options.MapHeight;
                    gridY++;
                }

                if (x > Options.MapWidth - 1)
                {
                    tmpX = x - Options.MapWidth;
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
                if (Status[i].Type == StatusTypes.Taunt)
                {
                    return false;
                }
            }

            var x = (int)Math.Floor(Globals.InputManager.GetMousePosition().X + Graphics.CurrentView.Left);
            var y = (int)Math.Floor(Globals.InputManager.GetMousePosition().Y + Graphics.CurrentView.Top);
            var targetRect = new FloatRect(x - 8, y - 8, 16, 16); //Adjust to allow more/less error

            IEntity bestMatch = null;
            var bestAreaMatch = 0f;


            foreach (MapInstance map in Maps.MapInstance.Lookup.Values)
            {
                if (x >= map.GetX() && x <= map.GetX() + Options.MapWidth * Options.TileWidth)
                {
                    if (y >= map.GetY() && y <= map.GetY() + Options.MapHeight * Options.TileHeight)
                    {
                        //Remove the offsets to just be dealing with pixels within the map selected
                        x -= (int)map.GetX();
                        y -= (int)map.GetY();

                        //transform pixel format to tile format
                        x /= Options.TileWidth;
                        y /= Options.TileHeight;
                        var mapId = map.Id;

                        if (TryGetRealLocation(ref x, ref y, ref mapId))
                        {
                            foreach (var en in Globals.Entities)
                            {
                                if (en.Value == null || en.Value.MapId != mapId || en.Value is Projectile || en.Value is Resource || (en.Value.IsStealthed && !Globals.Me.IsInMyParty(en.Value.Id)))
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

                            foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values)
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

                            if (bestMatch != null && bestMatch.Id != TargetIndex)
                            {
                                var targetType = bestMatch is Event ? 1 : 0;


                                SetTargetBox(bestMatch as Entity);

                                if (bestMatch is Player)
                                {
                                    //Select in admin window if open
                                    if (Interface.Interface.GameUi.AdminWindowOpen())
                                    {
                                        Interface.Interface.GameUi.AdminWindowSelectName(bestMatch.Name);
                                    }
                                }

                                TargetType = targetType;
                                TargetIndex = bestMatch.Id;

                                return true;
                            }
                            else if (!Globals.Database.StickyTarget)
                            {
                                // We've clicked off of our target and are allowed to clear it!
                                ClearTarget();
                                return true;
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
                if (Status[i].Type == StatusTypes.Taunt && !force)
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
                if (Interface.Interface.GameUi.AdminWindowOpen())
                {
                    Interface.Interface.GameUi.AdminWindowSelectName(entity.Name);
                }
            }

            if (TargetIndex != entity.Id)
            {
                SetTargetBox(entity as Entity);
                TargetType = targetType;
                TargetIndex = entity.Id;
            }

            return true;

        }

        public void ClearTarget()
        {
            SetTargetBox(null);

            TargetIndex = Guid.Empty;
            TargetType = -1;
        }

        /// <summary>
        /// Attempts to pick up an item at the specified location.
        /// </summary>
        /// <param name="mapId">The Id of the map we are trying to loot from.</param>
        /// <param name="x">The X location on the current map.</param>
        /// <param name="y">The Y location on the current map.</param>
        /// <param name="uniqueId">The Unique Id of the specific item we want to pick up, leave <see cref="Guid.Empty"/> to not specificy an item and pick up the first thing we can find.</param>
        /// <param name="firstOnly">Defines whether we only want to pick up the first item we can find when true, or all items when false.</param>
        /// <returns></returns>
        public bool TryPickupItem(Guid mapId, int tileIndex, Guid uniqueId = new Guid(), bool firstOnly = false)
        {
            var map = Maps.MapInstance.Get(mapId);
            if (map == null || tileIndex < 0 || tileIndex >= Options.MapWidth * Options.MapHeight)
            {
                return false;
            }

            // Are we trying to pick up anything in particular, or everything?
            if (uniqueId != Guid.Empty || firstOnly)
            {
                if (!map.MapItems.ContainsKey(tileIndex) || map.MapItems[tileIndex].Count < 1)
                {
                    return false;
                }

                foreach (var item in map.MapItems[tileIndex])
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
            ItemBase weapon = null;
            var attackTime = base.CalculateAttackTime();

            var cls = ClassBase.Get(Class);
            if (cls != null && cls.AttackSpeedModifier == 1) //Static
            {
                attackTime = cls.AttackSpeedValue;
            }

            if (this == Globals.Me)
            {
                if (Options.WeaponIndex > -1 &&
                    Options.WeaponIndex < Equipment.Length &&
                    MyEquipment[Options.WeaponIndex] >= 0)
                {
                    weapon = ItemBase.Get(Inventory[MyEquipment[Options.WeaponIndex]].ItemId);
                }
            }
            else
            {
                if (Options.WeaponIndex > -1 &&
                    Options.WeaponIndex < Equipment.Length &&
                    Equipment[Options.WeaponIndex] != Guid.Empty)
                {
                    weapon = ItemBase.Get(Equipment[Options.WeaponIndex]);
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
            return (int)(Options.MaxAttackRate +
                          (float)((Options.MinAttackRate - Options.MaxAttackRate) *
                                   (((float)Options.MaxStatValue - speed) /
                                    (float)Options.MaxStatValue)));
        }

        //Movement Processing
        private void ProcessDirectionalInput()
        {
            //Check if player is crafting
            if (Globals.InCraft == true)
            {
                return;
            }

            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Stun ||
                    Status[n].Type == StatusTypes.Snare ||
                    Status[n].Type == StatusTypes.Sleep)
                {
                    return;
                }
            }

            //Check if the player is dashing, if so don't let them move.
            if (Dashing != null || DashQueue.Count > 0 || DashTimer > Timing.Global.Milliseconds)
            {
                return;
            }

            if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond && !Options.Instance.PlayerOpts.AllowCombatMovement)
            {
                return;
            }

            var tmpX = (sbyte)X;
            var tmpY = (sbyte)Y;
            IEntity blockedBy = null;

            if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
            {
                //Try to move if able and not casting spells.
                if (!IsMoving && MoveTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond && (Options.Combat.MovementCancelsCast || !IsCasting))
                {
                    if (Options.Combat.MovementCancelsCast)
                    {
                        CastTime = 0;
                    }

                    switch (MoveDir)
                    {
                        case 0: // Up
                            if (IsTileBlocked(X, Y - 1, Z, MapId, ref blockedBy) == -1)
                            {
                                tmpY--;
                                IsMoving = true;
                                Dir = 0;
                                OffsetY = Options.TileHeight;
                                OffsetX = 0;
                            }

                            break;
                        case 1: // Down
                            if (IsTileBlocked(X, Y + 1, Z, MapId, ref blockedBy) == -1)
                            {
                                tmpY++;
                                IsMoving = true;
                                Dir = 1;
                                OffsetY = -Options.TileHeight;
                                OffsetX = 0;
                            }

                            break;
                        case 2: // Left
                            if (IsTileBlocked(X - 1, Y, Z, MapId, ref blockedBy) == -1)
                            {
                                tmpX--;
                                IsMoving = true;
                                Dir = 2;
                                OffsetY = 0;
                                OffsetX = Options.TileWidth;
                            }

                            break;
                        case 3: // Right
                            if (IsTileBlocked(X + 1, Y, Z, MapId, ref blockedBy) == -1)
                            {
                                tmpX++;
                                IsMoving = true;
                                Dir = 3;
                                OffsetY = 0;
                                OffsetX = -Options.TileWidth;
                            }

                            break;
                    }

                    if (blockedBy != mLastBumpedEvent)
                    {
                        mLastBumpedEvent = null;
                    }

                    if (IsMoving)
                    {
                        if (tmpX < 0 || tmpY < 0 || tmpX > Options.MapWidth - 1 || tmpY > Options.MapHeight - 1)
                        {
                            var gridX = Maps.MapInstance.Get(Globals.Me.MapId).GridX;
                            var gridY = Maps.MapInstance.Get(Globals.Me.MapId).GridY;
                            if (tmpX < 0)
                            {
                                gridX--;
                                X = (byte)(Options.MapWidth - 1);
                            }
                            else if (tmpX >= Options.MapWidth)
                            {
                                X = 0;
                                gridX++;
                            }
                            else
                            {
                                X = (byte)tmpX;
                            }

                            if (tmpY < 0)
                            {
                                gridY--;
                                Y = (byte)(Options.MapHeight - 1);
                            }
                            else if (tmpY >= Options.MapHeight)
                            {
                                Y = 0;
                                gridY++;
                            }
                            else
                            {
                                Y = (byte)tmpY;
                            }

                            if (MapId != Globals.MapGrid[gridX, gridY])
                            {
                                MapId = Globals.MapGrid[gridX, gridY];
                                FetchNewMaps();
                            }

                        }
                        else
                        {
                            X = (byte)tmpX;
                            Y = (byte)tmpY;
                        }

                        TryToChangeDimension();
                        PacketSender.SendMove();
                        MoveTimer = (Timing.Global.Ticks / TimeSpan.TicksPerMillisecond) + (long)GetMovementTime();
                    }
                    else
                    {
                        if (MoveDir != Dir)
                        {
                            Dir = (byte)MoveDir;
                            PacketSender.SendDirection(Dir);
                        }

                        if (blockedBy != null && mLastBumpedEvent != blockedBy && blockedBy is Event)
                        {
                            PacketSender.SendBumpEvent(blockedBy.MapId, blockedBy.Id);
                            mLastBumpedEvent = blockedBy as Entity;
                        }
                    }
                }
            }
        }

        public void FetchNewMaps()
        {
            if (Globals.MapGridWidth == 0 || Globals.MapGridHeight == 0)
            {
                return;
            }

            if (Maps.MapInstance.Get(Globals.Me.MapId) != null)
            {
                var gridX = Maps.MapInstance.Get(Globals.Me.MapId).GridX;
                var gridY = Maps.MapInstance.Get(Globals.Me.MapId).GridY;
                for (var x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (var y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 &&
                            x < Globals.MapGridWidth &&
                            y >= 0 &&
                            y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != Guid.Empty)
                        {
                            PacketSender.SendNeedMap(Globals.MapGrid[x, y]);
                        }
                    }
                }
            }
        }

        public override void DrawEquipment(string filename, Color renderColor, FloatRect entityRect)
        {
            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Transform)
                {
                    return;
                }
            }

            base.DrawEquipment(filename, renderColor, entityRect);
        }

        //Override of the original function, used for rendering the color of a player based on rank
        public override void DrawName(Color textColor, Color borderColor, Color backgroundColor)
        {
            if (textColor == null)
            {
                if (Aggression == 1) //Mod
                {
                    textColor = CustomColors.Names.Players["Moderator"].Name;
                    borderColor = CustomColors.Names.Players["Moderator"].Outline;
                    backgroundColor = CustomColors.Names.Players["Moderator"].Background;
                }
                else if (Aggression == 2) //Admin
                {
                    textColor = CustomColors.Names.Players["Admin"].Name;
                    borderColor = CustomColors.Names.Players["Admin"].Outline;
                    backgroundColor = CustomColors.Names.Players["Admin"].Background;
                }
                else //No Power
                {
                    textColor = CustomColors.Names.Players["Normal"].Name;
                    borderColor = CustomColors.Names.Players["Normal"].Outline;
                    backgroundColor = CustomColors.Names.Players["Normal"].Background;
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

            DrawNameAndLabels(textColor, borderColor, backgroundColor);
        }

        private void DrawNameAndLabels(Color textColor, Color borderColor, Color backgroundColor)
        {
            base.DrawName(textColor, borderColor, backgroundColor);
            DrawLabels(HeaderLabel.Text, 0, HeaderLabel.Color, textColor, borderColor, backgroundColor);
            DrawLabels(FooterLabel.Text, 1, FooterLabel.Color, textColor, borderColor, backgroundColor);
            DrawGuildName(textColor, borderColor, backgroundColor);
        }

        public virtual void DrawGuildName(Color textColor, Color borderColor = null, Color backgroundColor = null)
        {
            if (HideName || Guild == null || Guild.Trim().Length == 0 || !Options.Instance.Guild.ShowGuildNameTagsOverMembers)
            {
                return;
            }

            if (borderColor == null)
            {
                borderColor = Color.Transparent;
            }

            if (backgroundColor == null)
            {
                backgroundColor = Color.Transparent;
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
                {
                    if (this != Globals.Me && !(this is Player player && Globals.Me.IsInMyParty(player)))
                    {
                        return;
                    }
                }
            }

            var map = MapInstance;
            if (map == null)
            {
                return;
            }

            var textSize = Graphics.Renderer.MeasureText(Guild, Graphics.EntityNameFont, 1);

            var x = (int)Math.Ceiling(Origin.X);
            var y = GetLabelLocation(LabelType.Guild);

            if (backgroundColor != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(), new Framework.GenericClasses.FloatRect(0, 0, 1, 1),
                    new Framework.GenericClasses.FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y), backgroundColor
                );
            }

            Graphics.Renderer.DrawString(
                Guild, Graphics.EntityNameFont, (int)(x - (int)Math.Ceiling(textSize.X / 2f)), (int)y, 1,
                Color.FromArgb(textColor.ToArgb()), true, null, Color.FromArgb(borderColor.ToArgb())
            );
        }

        // Draw Entities Overhead Information when hovering the cursor by them
        // (when they are hidden by the game settings preferences).
        public void DrawOverheadInfoOnHover()
        {
            if (Interface.Interface.MouseHitGui())
            {
                return;
            }

            var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
            foreach (MapInstance map in Maps.MapInstance.Lookup.Values)
            {
                if (mousePos.X >= map.GetX() && mousePos.X <= map.GetX() + Options.MapWidth * Options.TileWidth)
                {
                    if (mousePos.Y >= map.GetY() && mousePos.Y <= map.GetY() + Options.MapHeight * Options.TileHeight)
                    {
                        var mapId = map.Id;
                        foreach (var en in Globals.Entities)
                        {
                            if (en.Value == null)
                            {
                                continue;
                            }

                            var isPlayer = en.Value is Player;
                            var isNpc = !isPlayer;
                            var player = en.Value as Player;

                            if (en.Value.MapId == mapId &&
                                !en.Value.HideName &&
                                (!en.Value.IsStealthed || isPlayer && Globals.Me.IsInMyParty(player)) &&
                                en.Value.WorldPos.Contains(mousePos.X, mousePos.Y))
                            {
                                // We don't want to deal with these entities when hovering the cursor by them.
                                var ignoreEntities = !(en.Value is Projectile || en.Value is Resource || en.Value is Event);

                                if (ignoreEntities)
                                {
                                    // Who's who.
                                    var isMe = isPlayer && player.Id == Globals.Me.Id;
                                    var isOtherPlayer = isPlayer && !isMe;
                                    var isFriend = isOtherPlayer && Globals.Me.IsFriend(player);
                                    var isGuildMate = isOtherPlayer && Globals.Me.IsGuildMate(player);
                                    var isPartyMate = isOtherPlayer && Globals.Me.IsInMyParty(player);

                                    // If MyOverheadInfo is toggled off, draw the local Players
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.MyOverheadInfo && isMe)
                                    {
                                        en.Value.DrawName(null);
                                    }

                                    // If NpcOverheadInfo is toggled off, draw NPCs
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.NpcOverheadInfo && isNpc)
                                    {
                                        en.Value.DrawName(null);
                                    }

                                    // If PlayerOverheadInfo is toggled off, draw Players
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.PlayerOverheadInfo && isOtherPlayer &&
                                        !isFriend && !isGuildMate && !isPartyMate)
                                    {
                                        en.Value.DrawName(null);
                                    }

                                    // If PartyMemberOverheadInfo is toggled off, draw Party Members
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.PartyMemberOverheadInfo && isPartyMate)
                                    {
                                        en.Value.DrawName(null);
                                    }

                                    // If FriendOverheadInfo & GuildMemberOverheadInfo are off,
                                    // let's prevent double draw / overlapping.
                                    if (!Globals.Database.FriendOverheadInfo && isFriend &&
                                        !Globals.Database.GuildMemberOverheadInfo && isGuildMate && !isPartyMate)
                                    {
                                        en.Value.DrawName(null);

                                        continue;
                                    }

                                    // If FriendOverheadInfo is toggled off, draw Friends
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.FriendOverheadInfo && isFriend && !isPartyMate)
                                    {
                                        // Skip if Friend is GuildMate.
                                        if (Globals.Database.GuildMemberOverheadInfo && isGuildMate)
                                        {
                                            continue;
                                        }

                                        en.Value.DrawName(null);
                                    }

                                    // If GuildMemberOverheadInfo is toggled off, draw Guild Members
                                    // overhead information only when hovered by the cursor.
                                    if (!Globals.Database.GuildMemberOverheadInfo && isGuildMate && !isPartyMate)
                                    {
                                        // Skip if GuildMate is Friend.
                                        if (Globals.Database.FriendOverheadInfo && isFriend)
                                        {
                                            continue;
                                        }

                                        en.Value.DrawName(null);
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
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

                if (!en.Value.IsHidden && (!en.Value.IsStealthed || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                {
                    if (!(en.Value is Projectile || en.Value is Resource))
                    {
                        if (TargetType == 0 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int)TargetTypes.Selected);
                        }
                    }
                }
            }

            foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values)
            {
                foreach (var en in eventMap.LocalEntities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value.MapId == eventMap.Id &&
                        !((Event)en.Value).DisablePreview &&
                        !en.Value.IsHidden &&
                        (!en.Value.IsStealthed || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                    {
                        if (TargetType == 1 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int)TargetTypes.Selected);
                        }
                    }
                }
            }

            if (!Interface.Interface.MouseHitGui())
            {
                var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
                foreach (MapInstance map in Maps.MapInstance.Lookup.Values)
                {
                    if (mousePos.X >= map.GetX() && mousePos.X <= map.GetX() + Options.MapWidth * Options.TileWidth)
                    {
                        if (mousePos.Y >= map.GetY() &&
                            mousePos.Y <= map.GetY() + Options.MapHeight * Options.TileHeight)
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
                                     en.Value is Player player && Globals.Me.IsInMyParty(player)) &&
                                    en.Value.WorldPos.Contains(mousePos.X, mousePos.Y))
                                {
                                    if (!(en.Value is Projectile || en.Value is Resource))
                                    {
                                        if (TargetType != 0 || TargetIndex != en.Value.Id)
                                        {
                                            en.Value.DrawTarget((int)TargetTypes.Hover);
                                        }
                                    }
                                }
                            }

                            foreach (MapInstance eventMap in Maps.MapInstance.Lookup.Values)
                            {
                                foreach (var en in eventMap.LocalEntities)
                                {
                                    if (en.Value == null)
                                    {
                                        continue;
                                    }

                                    if (en.Value.MapId == mapId &&
                                        !(en.Value as Event).DisablePreview &&
                                        !en.Value.IsHidden &&
                                        (!en.Value.IsStealthed ||
                                         en.Value is Player player && Globals.Me.IsInMyParty(player)) &&
                                        en.Value.WorldPos.Contains(mousePos.X, mousePos.Y))
                                    {
                                        if (TargetType != 1 || TargetIndex != en.Value.Id)
                                        {
                                            en.Value.DrawTarget((int)TargetTypes.Hover);
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

        private partial class TargetInfo
        {
            public long LastTimeSelected;

            public int DistanceTo;
        }

    }

}
