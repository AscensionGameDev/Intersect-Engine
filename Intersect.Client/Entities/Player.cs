using System;
using System.Linq;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Game.EntityPanel;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;

using Newtonsoft.Json;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Utilities;
using Intersect.Client.Items;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Config.Guilds;

namespace Intersect.Client.Entities
{

    public partial class Player : Entity
    {

        public delegate void InventoryUpdated();

        public Guid Class;

        public long Experience = 0;

        public long ExperienceToNextLevel = 0;

        public List<FriendInstance> Friends = new List<FriendInstance>();

        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];

        public InventoryUpdated InventoryUpdatedDelegate;

        public Dictionary<Guid, long> ItemCooldowns = new Dictionary<Guid, long>();

        private ItemDescWindow mItemTargetBox;

        private Entity mLastBumpedEvent = null;

        private List<PartyMember> mParty;

        public Dictionary<Guid, QuestProgress> QuestProgress = new Dictionary<Guid, QuestProgress>();

        public Guid[] HiddenQuests = new Guid[0];

        public Dictionary<Guid, long> SpellCooldowns = new Dictionary<Guid, long>();

        public int StatPoints = 0;

        public EntityBox TargetBox;

        public Guid TargetIndex;

        public int TargetType;

        public long CombatTimer { get; set; } = 0;

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
        public string Guild;

        /// <summary>
        /// Index of our rank where 0 is the leader
        /// </summary>
        public int Rank;

        /// <summary>
        /// Returns whether or not we are in a guild by checking to see if we are assigned a guild name
        /// </summary>
        public bool InGuild => !string.IsNullOrWhiteSpace(Guild);

        /// <summary>
        /// Obtains our rank and permissions from the game config
        /// </summary>
        public GuildRank GuildRank => InGuild ? Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(this.Rank, Options.Instance.Guild.Ranks.Length - 1))] : null;

        /// <summary>
        /// Contains a record of all members of this player's guild.
        /// </summary>
        public GuildMember[] GuildMembers = new GuildMember[0];

        public Player(Guid id, PlayerEntityPacket packet) : base(id, packet)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }

            mRenderPriority = 2;
        }


        public List<PartyMember> Party
        {
            get
            {
                if (mParty == null)
                {
                    mParty = new List<PartyMember>();
                }

                return mParty;
            }
        }

        public override Guid CurrentMap
        {
            get => base.CurrentMap;
            set
            {
                if (value != base.CurrentMap)
                {
                    var oldMap = MapInstance.Get(base.CurrentMap);
                    var newMap = MapInstance.Get(value);
                    base.CurrentMap = value;
                    if (Globals.Me == this)
                    {
                        if (MapInstance.Get(Globals.Me.CurrentMap) != null)
                        {
                            Audio.PlayMusic(MapInstance.Get(Globals.Me.CurrentMap).Music, 3, 3, true);
                        }

                        if (newMap != null && oldMap != null)
                        {
                            newMap.CompareEffects(oldMap);
                        }
                    }
                }
            }
        }

        public bool IsInParty()
        {
            return Party.Count > 0;
        }

        public bool IsInMyParty(Player player) => IsInMyParty(player.Id);

        public bool IsInMyParty(Guid id) => Party.Any(member => member.Id == id);

        public bool IsBusy()
        {
            return !(Globals.EventHolds.Count == 0 &&
                     !Globals.MoveRouteActive &&
                     Globals.GameShop == null &&
                     Globals.InBank == false &&
                     Globals.InCraft == false &&
                     Globals.InTrade == false &&
                     !Interface.Interface.HasInputFocus());
        }

        public override bool Update()
        {

            if (Globals.Me == this)
            {
                HandleInput();
            }


            if (!IsBusy())
            {
                if (this == Globals.Me && IsMoving == false)
                {
                    ProcessDirectionalInput();
                }

                if (Controls.KeyDown(Control.AttackInteract))
                {
                    if (!Globals.Me.TryAttack())
                    {
                        if (Globals.Me.AttackTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond)
                        {
                            Globals.Me.AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + Globals.Me.CalculateAttackTime();
                        }
                    }
                }
            }

            if (TargetBox != null)
            {
                TargetBox.Update();
            }
            else if (this == Globals.Me && TargetBox == null && Interface.Interface.GameUi != null)
            {
                // If for WHATEVER reason the box hasn't been created, create it.
                TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityTypes.Player, null);
                TargetBox.Hide();
            }


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
            var pkt = (PlayerEntityPacket) packet;
            Gender = pkt.Gender;
            Class = pkt.ClassId;
            Type = pkt.AccessLevel;
            CombatTimer = pkt.CombatTimeRemaining + Globals.System.GetTimeMs();
            Guild = pkt.Guild;
            Rank = pkt.GuildRank;

            var playerPacket = (PlayerEntityPacket) packet;

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

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Player;
        }

        //Item Processing
        public void SwapItems(int Label, int Color)
        {
            var tmpInstance = Inventory[Color].Clone();
            Inventory[Color] = Inventory[Label].Clone();
            Inventory[Label] = tmpInstance.Clone();
        }

        public void TryDropItem(int index)
        {
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                if (Inventory[index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Inventory.dropitem,
                        Strings.Inventory.dropitemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, DropItemInputBoxOkay, null, index, Inventory[index].Quantity
                    );
                }
                else
                {
                    var iBox = new InputBox(
                        Strings.Inventory.dropitem,
                        Strings.Inventory.dropprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.YesNo, DropInputBoxOkay, null, index
                    );
                }
            }
        }

        private void DropItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendDropItem((int) ((InputBox) sender).UserData, value);
            }
        }

        private void DropInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendDropItem((int) ((InputBox) sender).UserData, 1);
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

        public void TryUseItem(int index)
        {
            if (Globals.GameShop == null && Globals.InBank == false && Globals.InTrade == false && !ItemOnCd(index) &&
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

        public int FindHotbarItem(HotbarInstance hotbarInstance)
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

        public bool ItemOnCd(int slot)
        {
            if (Inventory[slot] != null)
            {
                var itm = Inventory[slot];
                if (itm.ItemId != Guid.Empty)
                {
                    if (ItemCooldowns.ContainsKey(itm.ItemId) && ItemCooldowns[itm.ItemId] > Globals.System.GetTimeMs())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public long ItemCdRemainder(int slot)
        {
            if (Inventory[slot] != null)
            {
                var itm = Inventory[slot];
                if (itm.ItemId != Guid.Empty)
                {
                    if (ItemCooldowns.ContainsKey(itm.ItemId) && ItemCooldowns[itm.ItemId] > Globals.System.GetTimeMs())
                    {
                        return ItemCooldowns[itm.ItemId] - Globals.System.GetTimeMs();
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
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                var foundItem = -1;
                for (var i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
                {
                    if (Globals.GameShop.BuyingItems[i].ItemId == Inventory[index].ItemId)
                    {
                        foundItem = i;

                        break;
                    }
                }

                if (foundItem > -1 && Globals.GameShop.BuyingWhitelist ||
                    foundItem == -1 && !Globals.GameShop.BuyingWhitelist)
                {
                    if (Inventory[index].Quantity > 1)
                    {
                        var iBox = new InputBox(
                            Strings.Shop.sellitem,
                            Strings.Shop.sellitemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                            InputBox.InputType.NumericInput, SellItemInputBoxOkay, null, index, Inventory[index].Quantity
                        );
                    }
                    else
                    {
                        var iBox = new InputBox(
                            Strings.Shop.sellitem,
                            Strings.Shop.sellprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                            InputBox.InputType.YesNo, SellInputBoxOkay, null, index
                        );
                    }
                }
                else
                {
                    var iBox = new InputBox(
                        Strings.Shop.sellitem, Strings.Shop.cannotsell, true, InputBox.InputType.OkayOnly, null, null,
                        -1
                    );
                }
            }
        }

        private void SellItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendSellItem((int) ((InputBox) sender).UserData, value);
            }
        }

        private void SellInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendSellItem((int) ((InputBox) sender).UserData, 1);
        }

        //bank
        public void TryDepositItem(int index)
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
                    var iBox = new InputBox(
                        Strings.Bank.deposititem,
                        Strings.Bank.deposititemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, DepositItemInputBoxOkay, null, index, Inventory[index].Quantity
                    );
                }
                else
                {
                    PacketSender.SendDepositItem(index, 1);
                }
            }
        }

        private void DepositItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendDepositItem((int) ((InputBox) sender).UserData, value);
            }
        }

        public void TryWithdrawItem(int index)
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
                    var iBox = new InputBox(
                        Strings.Bank.withdrawitem,
                        Strings.Bank.withdrawitemprompt.ToString(ItemBase.Get(Globals.Bank[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, WithdrawItemInputBoxOkay, null, index
                    );
                }
                else
                {
                    PacketSender.SendWithdrawItem(index, 1);
                }
            }
        }

        private void WithdrawItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendWithdrawItem((int) ((InputBox) sender).UserData, value);
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
                        InputBox.InputType.NumericInput, StoreBagItemInputBoxOkay, null, userData, Inventory[invSlot].Quantity
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
                        InputBox.InputType.NumericInput, RetreiveBagItemInputBoxOkay, null, userData
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
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                if (Inventory[index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Trading.offeritem,
                        Strings.Trading.offeritemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, TradeItemInputBoxOkay, null, index, Inventory[index].Quantity
                    );
                }
                else
                {
                    PacketSender.SendOfferTradeItem(index, 1);
                }
            }
        }

        private void TradeItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendOfferTradeItem((int) ((InputBox) sender).UserData, value);
            }
        }

        public void TryRevokeItem(int index)
        {
            if (Globals.Trade[0, index] != null && ItemBase.Get(Globals.Trade[0, index].ItemId) != null)
            {
                if (Globals.Trade[0, index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Trading.revokeitem,
                        Strings.Trading.revokeitemprompt.ToString(ItemBase.Get(Globals.Trade[0, index].ItemId).Name),
                        true, InputBox.InputType.NumericInput, RevokeItemInputBoxOkay, null, index
                    );
                }
                else
                {
                    PacketSender.SendRevokeTradeItem(index, 1);
                }
            }
        }

        private void RevokeItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendRevokeTradeItem((int) ((InputBox) sender).UserData, value);
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
            if (SpellBase.Get(Spells[index].SpellId) != null)
            {
                var iBox = new InputBox(
                    Strings.Spells.forgetspell,
                    Strings.Spells.forgetspellprompt.ToString(SpellBase.Get(Spells[index].SpellId).Name), true,
                    InputBox.InputType.YesNo, ForgetSpellInputBoxOkay, null, index
                );
            }
        }

        private void ForgetSpellInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendForgetSpell((int) ((InputBox) sender).UserData);
        }

        public void TryUseSpell(int index)
        {
            if (Spells[index].SpellId != Guid.Empty &&
                (!Globals.Me.SpellCooldowns.ContainsKey(Spells[index].SpellId) ||
                 Globals.Me.SpellCooldowns[Spells[index].SpellId] < Globals.System.GetTimeMs()))
            {
                var spellBase = SpellBase.Get(Spells[index].SpellId);

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
                if (Spells[i].SpellId == spellId)
                {
                    TryUseSpell(i);

                    return;
                }
            }
        }

        public int FindHotbarSpell(HotbarInstance hotbarInstance)
        {
            if (hotbarInstance.ItemOrSpellId != Guid.Empty && SpellBase.Get(hotbarInstance.ItemOrSpellId) != null)
            {
                for (var i = 0; i < Spells.Length; i++)
                {
                    if (Spells[i].SpellId == hotbarInstance.ItemOrSpellId)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        //Hotbar Processing
        public void AddToHotbar(byte hotbarSlot, sbyte itemType, int itemSlot)
        {
            Hotbar[hotbarSlot].ItemOrSpellId = Guid.Empty;
            Hotbar[hotbarSlot].PreferredStatBuffs = new int[(int) Stats.StatCount];
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
                    Hotbar[hotbarSlot].ItemOrSpellId = spell.SpellId;
                }
            }

            PacketSender.SendHotbarUpdate(hotbarSlot, itemType, itemSlot);
        }

        public void HotbarSwap(byte index, byte swapIndex)
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
                    if (MapInstance.Get(CurrentMap) != null && MapInstance.Get(CurrentMap).Attributes[X, Y] != null)
                    {
                        if (MapInstance.Get(CurrentMap).Attributes[X, Y].Type == MapAttributes.ZDimension)
                        {
                            if (((MapZDimensionAttribute) MapInstance.Get(CurrentMap).Attributes[X, Y]).GatewayTo > 0)
                            {
                                Z = (byte) (((MapZDimensionAttribute) MapInstance.Get(CurrentMap).Attributes[X, Y])
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
            for (var barSlot = 0; barSlot < Options.MaxHotbar; barSlot++)
            {
                if (!mLastHotbarUseTime.ContainsKey(barSlot))
                {
                    mLastHotbarUseTime.Add(barSlot, 0);
                }

                if (Controls.KeyDown((Control)barSlot + 9))
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

        protected int GetDistanceTo(Entity target)
        {
            if (target != null)
            {
                var myMap = MapInstance.Get(CurrentMap);
                var targetMap = MapInstance.Get(target.CurrentMap);
                if (myMap != null && targetMap != null)
                {
                    //Calculate World Tile of Me
                    var x1 = X + myMap.MapGridX * Options.MapWidth;
                    var y1 = Y + myMap.MapGridY * Options.MapHeight;

                    //Calculate world tile of target
                    var x2 = target.X + targetMap.MapGridX * Options.MapWidth;
                    var y2 = target.Y + targetMap.MapGridY * Options.MapHeight;

                    return (int) Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
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
            var canTargetPlayers = Globals.Me.MapInstance.ZoneType == MapZones.Safe ? false : true;

            // Build a list of Entities to select from with positions if our list is either old, we've moved or changed maps somehow.
            if (
                mlastTargetScanTime < Timing.Global.Milliseconds ||
                mlastTargetScanMap != Globals.Me.CurrentMap ||
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
                    if (en.Value.HideEntity || (en.Value.IsStealthed() && !Globals.Me.IsInMyParty(en.Value.Id)))
                    {
                        continue;
                    }

                    // Check if we are allowed to target players here, if we're not and this is a player then skip!
                    // If we are, check to see if they're our party or nation member, then exclude them. We're friendly happy people here.
                    if (!canTargetPlayers && en.Value.GetEntityType() == EntityTypes.Player)
                    {
                        continue;
                    }
                    else if (canTargetPlayers && en.Value.GetEntityType() == EntityTypes.Player)
                    {
                        var player = en.Value as Player;
                        if (IsInMyParty(player))
                        {
                            continue;
                        }
                    }

                    if (en.Value.GetEntityType() == EntityTypes.GlobalEntity || en.Value.GetEntityType() == EntityTypes.Player)
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
                foreach(var en in toRemove)
                {
                    mlastTargetList.Remove(en.Key);
                }

                // Skip scanning for another second or so.. And set up other values.
                mlastTargetScanTime = Timing.Global.Milliseconds + 300;
                mlastTargetScanMap = CurrentMap;
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
                            ((en.Key.CurrentMap == CurrentMap || en.Key.CurrentMap == MapInstance.Left || en.Key.CurrentMap == MapInstance.Right) && en.Key.Y < Y) || en.Key.CurrentMap == MapInstance.Down)
                            .ToArray();
                        break;

                    case (byte)Directions.Down:
                        validEntities = mlastTargetList.Where(en =>
                            ((en.Key.CurrentMap == CurrentMap || en.Key.CurrentMap == MapInstance.Left || en.Key.CurrentMap == MapInstance.Right) && en.Key.Y > Y) || en.Key.CurrentMap == MapInstance.Up)
                            .ToArray();
                        break;

                    case (byte)Directions.Left:
                        validEntities = mlastTargetList.Where(en =>
                            ((en.Key.CurrentMap == CurrentMap || en.Key.CurrentMap == MapInstance.Up || en.Key.CurrentMap == MapInstance.Down) && en.Key.X < X) || en.Key.CurrentMap == MapInstance.Left)
                            .ToArray();
                        break;

                    case (byte)Directions.Right:
                        validEntities = mlastTargetList.Where(en =>
                                    ((en.Key.CurrentMap == CurrentMap || en.Key.CurrentMap == MapInstance.Up || en.Key.CurrentMap == MapInstance.Down) && en.Key.X > X) || en.Key.CurrentMap == MapInstance.Right)
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
            foreach(var entity in validEntities)
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
                foreach(var entity in mlastTargetList)
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
            mLastEntitySelected = currentEntity;

            if (TargetIndex != currentEntity.Id)
            {
                SetTargetBox(currentEntity);
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
                    Blocking = true;

                    return true;
                }
            }

            return false;
        }

        public void StopBlocking()
        {
            if (Blocking)
            {
                Blocking = false;
                PacketSender.SendBlock(false);
                AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + CalculateAttackTime();
            }
        }

        public bool TryAttack()
        {
            if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond || Blocking || (IsMoving && !Options.Instance.PlayerOpts.AllowCombatMovement))
            {
                return false;
            }

            int x = Globals.Me.X;
            int y = Globals.Me.Y;
            var map = Globals.Me.CurrentMap;
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

            if (GetRealLocation(ref x, ref y, ref map))
            {
                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value != Globals.Me)
                    {
                        if (en.Value.CurrentMap == map &&
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

            foreach (MapInstance eventMap in MapInstance.Lookup.Values)
            {
                foreach (var en in eventMap.LocalEntities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value.CurrentMap == map && en.Value.X == x && en.Value.Y == y)
                    {
                        if (en.Value.GetType() == typeof(Event))
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

        public bool GetRealLocation(ref int x, ref int y, ref Guid mapId)
        {
            var tmpX = x;
            var tmpY = y;
            var tmpI = -1;
            if (MapInstance.Get(mapId) != null)
            {
                var gridX = MapInstance.Get(mapId).MapGridX;
                var gridY = MapInstance.Get(mapId).MapGridY;

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
                    if (MapInstance.Get(Globals.MapGrid[gridX, gridY]) != null)
                    {
                        x = (byte) tmpX;
                        y = (byte) tmpY;
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

            var x = (int) Math.Floor(Globals.InputManager.GetMousePosition().X + Graphics.CurrentView.Left);
            var y = (int) Math.Floor(Globals.InputManager.GetMousePosition().Y + Graphics.CurrentView.Top);
            var targetRect = new FloatRect(x - 8, y - 8, 16, 16); //Adjust to allow more/less error

            Entity bestMatch = null;
            var bestAreaMatch = 0f;


            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (x >= map.GetX() && x <= map.GetX() + Options.MapWidth * Options.TileWidth)
                {
                    if (y >= map.GetY() && y <= map.GetY() + Options.MapHeight * Options.TileHeight)
                    {
                        //Remove the offsets to just be dealing with pixels within the map selected
                        x -= (int) map.GetX();
                        y -= (int) map.GetY();

                        //transform pixel format to tile format
                        x /= Options.TileWidth;
                        y /= Options.TileHeight;
                        var mapId = map.Id;

                        if (GetRealLocation(ref x, ref y, ref mapId))
                        {
                            foreach (var en in Globals.Entities)
                            {
                                if (en.Value == null || en.Value.CurrentMap != mapId || en.Value is Projectile || en.Value is Resource || (en.Value.IsStealthed() && !Globals.Me.IsInMyParty(en.Value.Id)))
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

                            foreach (MapInstance eventMap in MapInstance.Lookup.Values)
                            {
                                foreach (var en in eventMap.LocalEntities)
                                {
                                    if (en.Value == null || en.Value.CurrentMap != mapId || ((Event)en.Value).DisablePreview)
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


                                SetTargetBox(bestMatch);

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

        public bool TryTarget(Entity entity, bool force = false)
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
            if (TargetBox != null && TargetBox.MyEntity == entity )
            {
                return true;
            }

            var targetType = entity is Event ? 1 : 0;

            if (entity.GetType() == typeof(Player))
            {
                //Select in admin window if open
                if (Interface.Interface.GameUi.AdminWindowOpen())
                {
                    Interface.Interface.GameUi.AdminWindowSelectName(entity.Name);
                }
            }

            if (TargetIndex != entity.Id)
            {
                SetTargetBox(entity);
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
            if (mItemTargetBox != null)
            {
                mItemTargetBox.Dispose();
                mItemTargetBox = null;
            }
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
            var map = MapInstance.Get(mapId);
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
                    if (uniqueId != Guid.Empty && item.UniqueId != uniqueId)
                    {
                        continue;
                    }

                    PacketSender.SendPickupItem(mapId, tileIndex, item.UniqueId);

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
                    attackTime = (int) (attackTime * (100f / weapon.AttackSpeedValue));
                }
            }

            return attackTime;
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
            if (Dashing != null || DashQueue.Count > 0 || DashTimer > Globals.System.GetTimeMs())
            {
                return;
            }

            if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond && !Options.Instance.PlayerOpts.AllowCombatMovement)
            {
                return;
            }

            var tmpX = (sbyte) X;
            var tmpY = (sbyte) Y;
            Entity blockedBy = null;

            if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
            {
                //Try to move if able and not casting spells.
                if (!IsMoving && MoveTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond && (Options.Combat.MovementCancelsCast || CastTime < Globals.System.GetTimeMs())) 
                {
                    if (Options.Combat.MovementCancelsCast)
                    {
                        CastTime = 0;
                    }

                    switch (MoveDir)
                    {
                        case 0: // Up
                            if (IsTileBlocked(X, Y - 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY--;
                                IsMoving = true;
                                Dir = 0;
                                OffsetY = Options.TileHeight;
                                OffsetX = 0;
                            }

                            break;
                        case 1: // Down
                            if (IsTileBlocked(X, Y + 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY++;
                                IsMoving = true;
                                Dir = 1;
                                OffsetY = -Options.TileHeight;
                                OffsetX = 0;
                            }

                            break;
                        case 2: // Left
                            if (IsTileBlocked(X - 1, Y, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpX--;
                                IsMoving = true;
                                Dir = 2;
                                OffsetY = 0;
                                OffsetX = Options.TileWidth;
                            }

                            break;
                        case 3: // Right
                            if (IsTileBlocked(X + 1, Y, Z, CurrentMap, ref blockedBy) == -1)
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
                            var gridX = MapInstance.Get(Globals.Me.CurrentMap).MapGridX;
                            var gridY = MapInstance.Get(Globals.Me.CurrentMap).MapGridY;
                            if (tmpX < 0)
                            {
                                gridX--;
                                X = (byte) (Options.MapWidth - 1);
                            }
                            else if (tmpX >= Options.MapWidth)
                            {
                                X = 0;
                                gridX++;
                            }
                            else
                            {
                                X = (byte) tmpX;
                            }

                            if (tmpY < 0)
                            {
                                gridY--;
                                Y = (byte) (Options.MapHeight - 1);
                            }
                            else if (tmpY >= Options.MapHeight)
                            {
                                Y = 0;
                                gridY++;
                            }
                            else
                            {
                                Y = (byte) tmpY;
                            }

                            if (CurrentMap != Globals.MapGrid[gridX, gridY])
                            {
                                CurrentMap = Globals.MapGrid[gridX, gridY];
                                FetchNewMaps();
                            }

                        }
                        else
                        {
                            X = (byte) tmpX;
                            Y = (byte) tmpY;
                        }

                        TryToChangeDimension();
                        PacketSender.SendMove();
                        MoveTimer = (Timing.Global.Ticks / TimeSpan.TicksPerMillisecond) + (long)GetMovementTime();
                    }
                    else
                    {
                        if (MoveDir != Dir)
                        {
                            Dir = (byte) MoveDir;
                            PacketSender.SendDirection(Dir);
                        }

                        if (blockedBy != null && mLastBumpedEvent != blockedBy && blockedBy.GetType() == typeof(Event))
                        {
                            PacketSender.SendBumpEvent(blockedBy.CurrentMap, blockedBy.Id);
                            mLastBumpedEvent = blockedBy;
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

            if (MapInstance.Get(Globals.Me.CurrentMap) != null)
            {
                var gridX = MapInstance.Get(Globals.Me.CurrentMap).MapGridX;
                var gridY = MapInstance.Get(Globals.Me.CurrentMap).MapGridY;
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
                            if (MapInstance.Get(Globals.MapGrid[x, y]) == null)
                            {
                                PacketSender.SendNeedMap(Globals.MapGrid[x, y]);
                            }
                        }
                    }
                }
            }
        }

        public override void DrawEquipment(string filename, int alpha)
        {
            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Transform)
                {
                    return;
                }
            }

            base.DrawEquipment(filename, alpha);
        }

        //Override of the original function, used for rendering the color of a player based on rank
        public override void DrawName(Color textColor, Color borderColor, Color backgroundColor)
        {
            if (textColor == null)
            {
                if (Type == 1) //Mod
                {
                    textColor = CustomColors.Names.Players["Moderator"].Name;
                    borderColor = CustomColors.Names.Players["Moderator"].Outline;
                    backgroundColor = CustomColors.Names.Players["Moderator"].Background;
                }
                else if (Type == 2) //Admin
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

            var x = (int)Math.Ceiling(GetCenterPos().X);
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

        public void DrawTargets()
        {
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                if (!en.Value.HideEntity && (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                {
                    if (en.Value.GetType() != typeof(Projectile) && en.Value.GetType() != typeof(Resource))
                    {
                        if (TargetType == 0 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int) TargetTypes.Selected);
                        }
                    }
                }
                else
                {
                    //TODO: Completely wipe the stealthed player from memory and have server re-send once stealth ends.
                    ClearTarget();
                }
            }

            foreach (MapInstance eventMap in MapInstance.Lookup.Values)
            {
                foreach (var en in eventMap.LocalEntities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value.CurrentMap == eventMap.Id &&
                        !((Event) en.Value).DisablePreview &&
                        !en.Value.HideEntity &&
                        (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                    {
                        if (TargetType == 1 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int) TargetTypes.Selected);
                        }
                    }
                }
            }

            var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
            foreach (MapInstance map in MapInstance.Lookup.Values)
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

                            if (en.Value.CurrentMap == mapId &&
                                !en.Value.HideName &&
                                (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)) &&
                                en.Value.WorldPos.Contains(mousePos.X, mousePos.Y))
                            {
                                if (en.Value.GetType() != typeof(Projectile) && en.Value.GetType() != typeof(Resource))
                                {
                                    if (TargetType != 0 || TargetIndex != en.Value.Id)
                                    {
                                        en.Value.DrawTarget((int) TargetTypes.Hover);
                                    }
                                }
                            }
                        }

                        foreach (MapInstance eventMap in MapInstance.Lookup.Values)
                        {
                            foreach (var en in eventMap.LocalEntities)
                            {
                                if (en.Value == null)
                                {
                                    continue;
                                }

                                if (en.Value.CurrentMap == mapId &&
                                    !((Event) en.Value).DisablePreview &&
                                    !en.Value.HideEntity &&
                                    (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)) &&
                                    en.Value.WorldPos.Contains(mousePos.X, mousePos.Y))
                                {
                                    if (TargetType != 1 || TargetIndex != en.Value.Id)
                                    {
                                        en.Value.DrawTarget((int) TargetTypes.Hover);
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

        private class TargetInfo
        {
            public long LastTimeSelected;

            public int DistanceTo;
        }

    }

    public class FriendInstance
    {

        public string Map;

        public string Name;

        public bool Online = false;

    }

    public class HotbarInstance
    {

        public Guid BagId = Guid.Empty;

        public Guid ItemOrSpellId = Guid.Empty;

        public int[] PreferredStatBuffs = new int[(int) Stats.StatCount];

        public void Load(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }

    }

}
