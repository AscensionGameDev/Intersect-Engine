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

namespace Intersect.Client.Entities
{

    public class Player : Entity
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

        public Dictionary<Guid, long> SpellCooldowns = new Dictionary<Guid, long>();

        public int StatPoints = 0;

        public EntityBox TargetBox;

        public Guid TargetIndex;

        public int TargetType;

        public bool TargetOnFocus;

        public Player(Guid id, PlayerEntityPacket packet) : base(id, packet)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }

            mRenderPriority = 2;
        }

        public long CombatTimer { get; set; } = 0;

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
                        if (Globals.Me.AttackTimer < Globals.System.GetTimeMs())
                        {
                            Globals.Me.AttackTimer = Globals.System.GetTimeMs() + Globals.Me.CalculateAttackTime();
                        }
                    }
                }
            }

            if (TargetBox != null)
            {
                TargetBox.Update();
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
            Type = pkt.AccessLevel;
            CombatTimer = pkt.CombatTimeRemaining + Globals.System.GetTimeMs();

            if (((PlayerEntityPacket)packet).Equipment != null)
            {
                if (this == Globals.Me && ((PlayerEntityPacket)packet).Equipment.InventorySlots != null)
                {
                    this.MyEquipment = ((PlayerEntityPacket)packet).Equipment.InventorySlots;
                }
                else if (((PlayerEntityPacket)packet).Equipment.ItemIds != null)
                {
                    this.Equipment = ((PlayerEntityPacket)packet).Equipment.ItemIds;
                }
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
                        InputBox.InputType.NumericInput, DropItemInputBoxOkay, null, index
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
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendDropItem((int)((InputBox)sender).UserData, value);
            }
        }

        private void DropInputBoxOkay(object sender, EventArgs e)
        {
            PacketSender.SendDropItem((int)((InputBox)sender).UserData, 1);
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
            if (Globals.GameShop == null && Globals.InBank == false && Globals.InTrade == false && !ItemOnCd(index))
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
                            InputBox.InputType.NumericInput, SellItemInputBoxOkay, null, index
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
        public void TryDepositItem(int index)
        {
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                if (Inventory[index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Bank.deposititem,
                        Strings.Bank.deposititemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, DepositItemInputBoxOkay, null, index
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
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendDepositItem((int)((InputBox)sender).UserData, value);
            }
        }

        public void TryWithdrawItem(int index)
        {
            if (Globals.Bank[index] != null && ItemBase.Get(Globals.Bank[index].ItemId) != null)
            {
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
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendWithdrawItem((int)((InputBox)sender).UserData, value);
            }
        }

        //Bag
        public void TryStoreBagItem(int index)
        {
            if (ItemBase.Get(Inventory[index].ItemId) != null)
            {
                if (Inventory[index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Bags.storeitem,
                        Strings.Bags.storeitemprompt.ToString(ItemBase.Get(Inventory[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, StoreBagItemInputBoxOkay, null, index
                    );
                }
                else
                {
                    PacketSender.SendStoreBagItem(index, 1);
                }
            }
        }

        private void StoreBagItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendStoreBagItem((int)((InputBox)sender).UserData, value);
            }
        }

        public void TryRetreiveBagItem(int index)
        {
            if (Globals.Bag[index] != null && ItemBase.Get(Globals.Bag[index].ItemId) != null)
            {
                if (Globals.Bag[index].Quantity > 1)
                {
                    var iBox = new InputBox(
                        Strings.Bags.retreiveitem,
                        Strings.Bags.retreiveitemprompt.ToString(ItemBase.Get(Globals.Bag[index].ItemId).Name), true,
                        InputBox.InputType.NumericInput, RetreiveBagItemInputBoxOkay, null, index
                    );
                }
                else
                {
                    PacketSender.SendRetrieveBagItem(index, 1);
                }
            }
        }

        private void RetreiveBagItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendRetrieveBagItem((int)((InputBox)sender).UserData, value);
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
                        InputBox.InputType.NumericInput, TradeItemInputBoxOkay, null, index
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
            var value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendOfferTradeItem((int)((InputBox)sender).UserData, value);
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
            PacketSender.SendForgetSpell((int)((InputBox)sender).UserData);
        }

        public void TryUseSpell(int index)
        {
            if (Spells[index].SpellId != Guid.Empty &&
                (!Globals.Me.SpellCooldowns.ContainsKey(Spells[index].SpellId) ||
                 Globals.Me.SpellCooldowns[Spells[index].SpellId] < Globals.System.GetTimeMs()))
            {
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
                            if (((MapZDimensionAttribute)MapInstance.Get(CurrentMap).Attributes[X, Y]).GatewayTo > 0)
                            {
                                Z = (byte)(((MapZDimensionAttribute)MapInstance.Get(CurrentMap).Attributes[X, Y])
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


            // Used this so I can do multiple switch case
            var move = movex / 10 + movey;

            Globals.Me.MoveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                switch (move)
                {
                    case 1.0f:
                        Globals.Me.MoveDir = 0; // Up

                        break;
                    case -1.0f:
                        Globals.Me.MoveDir = 1; // Down

                        break;
                    case -0.1f: // x = 0, y = -1
                        Globals.Me.MoveDir = 2; // Left

                        break;
                    case 0.1f:
                        Globals.Me.MoveDir = 3; // Right

                        break;
                    case 0.9f:
                        Globals.Me.MoveDir = 4; // NW

                        break;
                    case 1.1f:
                        Globals.Me.MoveDir = 5; // NE

                        break;
                    case -1.1f:
                        Globals.Me.MoveDir = 6; // SW

                        break;
                    case -0.9f:
                        Globals.Me.MoveDir = 7; // SE

                        break;
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

                    return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                }
            }

            //Something is null.. return a value that is out of range :) 
            return 9999;
        }

        public void AutoTarget()
        {
            Entity closestEntity = null;

            //Check for taunt status if so don't allow to change target
            for (var i = 0; i < Status.Count; i++)
            {
                if (Status[i].Type == StatusTypes.Taunt)
                {
                    return;
                }
            }

            foreach (var en in Globals.Entities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                if (Globals.GridMaps.Contains(en.Value.CurrentMap))
                {
                    if (en.Value.GetEntityType() == EntityTypes.GlobalEntity ||
                        en.Value.GetEntityType() == EntityTypes.Player)
                    {
                        if (en.Value != Globals.Me && !(en.Value is Player player && Globals.Me.IsInMyParty(player)))
                        {
                            if (GetDistanceTo(en.Value) < GetDistanceTo(closestEntity))
                            {
                                closestEntity = en.Value;
                            }
                        }
                    }
                }
            }

            if (TargetBox != null && closestEntity != TargetBox.MyEntity)
            {
                TargetBox.Dispose();
                TargetBox = null;
            }

            if (closestEntity == null)
            {
                return;
            }

            if (TargetBox == null)
            {
                if (closestEntity.GetType() == typeof(Player))
                {
                    TargetBox = new EntityBox(Interface.Interface.GameUi.GameCanvas, EntityTypes.Player, closestEntity);
                }
                else
                {
                    TargetBox = new EntityBox(
                        Interface.Interface.GameUi.GameCanvas, EntityTypes.GlobalEntity, closestEntity
                    );
                }
            }

            TargetIndex = closestEntity.Id;
            TargetType = 0;
        }

        public bool TryBlock()
        {
            if (AttackTimer > Globals.System.GetTimeMs())
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
                AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
            }
        }

        public bool TryAttack()
        {
            if (AttackTimer > Globals.System.GetTimeMs() || Blocking)
            {
                return false;
            }

            int x = Globals.Me.X;
            int y = Globals.Me.Y;
            var map = Globals.Me.CurrentMap;

            List<int[]> hitbox = new List<int[]>();

            // The latest moving direction of the player
            switch (Globals.Me.Dir)
            {

                // Tabulation used to have a sight of the hitbox.
                case 0: // Up
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y - 1 }, new int[] { x, y - 1 }, new int[] { x + 1, y - 1 },
                                new int[] { x - 1, y },                              new int[] { x + 1, y },
                            });
                    y--;

                    break;
                case 1: // Down
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y },                              new int[] { x + 1, y },
                                new int[] { x - 1, y + 1 }, new int[] { x, y + 1 }, new int[] { x + 1, y + 1 },
                            });
                    y++;

                    break;
                case 2: // Left
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y - 1 }, new int[] { x, y - 1 },
                                new int[] { x - 1, y },
                                new int[] { x - 1, y + 1 }, new int[] { x, y + 1 }
                            });
                    x--;

                    break;
                case 3: // Right
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x, y - 1 }, new int[] { x + 1, y - 1 },
                                new int[] { x + 1, y },
                                new int[] { x, y + 1 }, new int[] { x + 1, y + 1 }
                            });
                    x++;

                    break;

                case 4: // UpLeft
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y - 1 }, new int[] { x, y - 1 }, new int[] { x + 1, y - 1 },
                                new int[] { x - 1, y },
                                new int[] { x - 1, y + 1 }
                            });
                    y--;
                    x--;

                    break;
                case 5: //UpRight
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y - 1 }, new int[] { x, y - 1 }, new int[] { x + 1, y - 1 },
                                                                                        new int[] { x + 1, y },
                                                                                        new int[] { x + 1, y + 1 }
                            });
                    y--;
                    x++;

                    break;
                case 6: // DownLeft
                    hitbox.AddRange(new List<int[]>
                            {
                                new int[] { x - 1, y - 1 },
                                new int[] { x - 1, y },
                                new int[] { x - 1, y + 1 }, new int[] { x, y + 1 }, new int[] { x + 1, y + 1 }
                            });
                    y++;
                    x--;

                    break;
                case 7: // DownRight
                    hitbox.AddRange(new List<int[]>
                            {
                                                                                    new int[] { x + 1, y - 1 },
                                                                                    new int[] { x + 1, y },
                                new int[] { x - 1, y + 1 }, new int[] { x, y + 1 }, new int[] { x + 1, y + 1 }
                            });
                    y++;
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
                            en.Value.CanBeAttacked())
                        {
                            if (TargetIndex != null && TargetOnFocus)
                            {
                                bool canAttack = false;
                                foreach (int[] hitBx in hitbox)
                                {
                                    if (hitBx[0] == en.Value.X && hitBx[1] == en.Value.Y)
                                    {
                                        canAttack = true;
                                        break;
                                    }
                                }

                                if (canAttack)
                                {
                                    PacketSender.SendAttack(en.Key, TargetOnFocus);
                                    AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();

                                    return true;
                                }
                            }
                            else if (en.Value.X == x && en.Value.Y == y)
                            {
                                //ATTACKKKKK!!!
                                PacketSender.SendAttack(en.Key, TargetOnFocus);
                                AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();

                                return true;
                            }
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
                            AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();

                            return true;
                        }
                    }
                }
            }

            //Projectile/empty swing for animations
            PacketSender.SendAttack(Guid.Empty, TargetOnFocus);
            AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();

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

            foreach (MapInstance map in MapInstance.Lookup.Values)
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

                        if (GetRealLocation(ref x, ref y, ref mapId))
                        {
                            foreach (var en in Globals.Entities)
                            {
                                if (en.Value == null)
                                {
                                    continue;
                                }

                                if (en.Value.CurrentMap == mapId &&
                                    en.Value.X == x &&
                                    en.Value.Y == y &&
                                    (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                                {
                                    if (en.Value.GetType() != typeof(Projectile) &&
                                        en.Value.GetType() != typeof(Resource))
                                    {
                                        if (TargetBox != null)
                                        {
                                            TargetBox.Dispose();
                                            TargetBox = null;
                                        }

                                        if (en.Value != Globals.Me)
                                        {
                                            if (en.Value.GetType() == typeof(Player))
                                            {
                                                TargetBox = new EntityBox(
                                                    Interface.Interface.GameUi.GameCanvas, EntityTypes.Player, en.Value
                                                );
                                            }
                                            else
                                            {
                                                TargetBox = new EntityBox(
                                                    Interface.Interface.GameUi.GameCanvas, EntityTypes.GlobalEntity,
                                                    en.Value
                                                );
                                            }
                                        }

                                        if (TargetType == 0 && TargetIndex == en.Value.Id)
                                        {
                                            ClearTarget();

                                            return true;
                                        }

                                        if (en.Value.GetType() == typeof(Player))
                                        {
                                            //Select in admin window if open
                                            if (Interface.Interface.GameUi.AdminWindowOpen())
                                            {
                                                Interface.Interface.GameUi.AdminWindowSelectName(en.Value.Name);
                                            }
                                        }

                                        TargetType = 0;
                                        TargetIndex = en.Value.Id;

                                        return true;
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
                                        en.Value.X == x &&
                                        en.Value.Y == y &&
                                        !((Event)en.Value).DisablePreview &&
                                        (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                                    {
                                        if (TargetBox != null)
                                        {
                                            TargetBox.Dispose();
                                            TargetBox = null;
                                        }

                                        TargetBox = new EntityBox(
                                            Interface.Interface.GameUi.GameCanvas, EntityTypes.Event, en.Value
                                        );

                                        if (TargetType == 1 && TargetIndex == en.Value.Id)
                                        {
                                            ClearTarget();

                                            return true;
                                        }

                                        TargetType = 1;
                                        TargetIndex = en.Value.Id;

                                        return true;
                                    }
                                }
                            }
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        private void ClearTarget()
        {
            if (TargetBox != null)
            {
                TargetBox.Dispose();
                TargetBox = null;
            }

            TargetIndex = Guid.Empty;
            TargetType = -1;
            if (mItemTargetBox != null)
            {
                mItemTargetBox.Dispose();
                mItemTargetBox = null;
            }
        }

        public bool TryPickupItem()
        {
            var map = MapInstance.Get(CurrentMap);
            if (map == null)
            {
                return false;
            }

            foreach (var item in map.MapItems)
            {
                if (item.Value.X == X && item.Value.Y == Y)
                {
                    // Are we allowed to see and pick this item up?
                    if (!item.Value.VisibleToAll && item.Value.Owner != Globals.Me.Id && !Globals.Me.IsInMyParty(item.Value.Owner))
                    {
                        // This item does not apply to us!
                        return false;
                    }

                    PacketSender.SendPickupItem(item.Key);

                    return true;
                }
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

        //Movement Processing
        private void ProcessDirectionalInput()
        {
            var didMove = false;
            var tmpI = -1;

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

            var tmpX = (sbyte)X;
            var tmpY = (sbyte)Y;
            Entity blockedBy = null;

            if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
            {
                //Try to move if able and not casting spells.
                if (!IsMoving && MoveTimer < Globals.System.GetTimeMs() && CastTime < Globals.System.GetTimeMs())
                {
                    switch (MoveDir)
                    {
                        // Dir is the direction the player faces
                        // tmp the next position of the player
                        // DeplacementDir is used because I don't know how to set the sprite animation for the diagonal mouvement.

                        case 0: // Up
                            if (IsTileBlocked(X, Y - 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY--;
                                IsMoving = true;
                                Dir = 0; // Set the sprite direction
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
                        case 4: // NW
                            if (IsTileBlocked(X - 1, Y - 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY--;
                                tmpX--;
                                Dir = 4;
                                IsMoving = true;
                                OffsetY = Options.TileHeight;
                                OffsetX = Options.TileWidth;
                            }
                            break;
                        case 5: // NE
                            if (IsTileBlocked(X + 1, Y - 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY--;
                                tmpX++;
                                Dir = 5;
                                IsMoving = true;
                                OffsetY = Options.TileHeight;
                                OffsetX = -Options.TileWidth;
                            }
                            break;
                        case 6: // SW
                            if (IsTileBlocked(X - 1, Y + 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY++;
                                tmpX--;
                                Dir = 6;
                                IsMoving = true;
                                OffsetY = -Options.TileHeight;
                                OffsetX = Options.TileWidth;
                            }
                            break;
                        case 7: // SE
                            if (IsTileBlocked(X + 1, Y + 1, Z, CurrentMap, ref blockedBy) == -1)
                            {
                                tmpY++;
                                tmpX++;
                                Dir = 7;
                                IsMoving = true;
                                OffsetY = -Options.TileHeight;
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
                        MoveTimer = Globals.System.GetTimeMs() + GetMovementTime();
                        didMove = true;
                        if (tmpX < 0 || tmpY < 0 || tmpX > Options.MapWidth - 1 || tmpY > Options.MapHeight - 1)
                        {
                            var gridX = MapInstance.Get(Globals.Me.CurrentMap).MapGridX;
                            var gridY = MapInstance.Get(Globals.Me.CurrentMap).MapGridY;
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

                            if (CurrentMap != Globals.MapGrid[gridX, gridY])
                            {
                                CurrentMap = Globals.MapGrid[gridX, gridY];
                                FetchNewMaps();
                            }
                        }
                        else
                        {
                            X = (byte)tmpX;
                            Y = (byte)tmpY;
                        }

                        TryToChangeDimension();
                    }
                    else
                    {
                        if (MoveDir != Dir)
                        {
                            Dir = (byte)MoveDir;
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

            Globals.MyX = X;
            Globals.MyY = Y;
            if (didMove)
            {
                PacketSender.SendMove();
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

        /// <summary>
        ///     Returns -6 if the tile is blocked by a global (non-event) entity
        ///     Returns -5 if the tile is completely out of bounds.
        ///     Returns -4 if a tile is blocked because of a local event.
        ///     Returns -3 if a tile is blocked because of a Z dimension tile
        ///     Returns -2 if a tile does not exist or is blocked by a map attribute.
        ///     Returns -1 is a tile is passable.
        ///     Returns any value zero or greater matching the entity index that is in the way.
        /// </summary>
        /// <returns></returns>
        public int IsTileBlocked(
            int x,
            int y,
            int z,
            Guid mapId,
            ref Entity blockedBy,
            bool ignoreAliveResources = true,
            bool ignoreDeadResources = true
        )
        {
            var mapInstance = MapInstance.Get(mapId);
            if (mapInstance == null)
            {
                return -2;
            }

            var gridX = mapInstance.MapGridX;
            var gridY = mapInstance.MapGridY;
            try
            {
                var tmpX = x;
                var tmpY = y;
                var tmpMapId = Guid.Empty;
                if (x < 0)
                {
                    gridX--;
                    tmpX = Options.MapWidth - x * -1;
                }

                if (y < 0)
                {
                    gridY--;
                    tmpY = Options.MapHeight - y * -1;
                }

                if (x > Options.MapWidth - 1)
                {
                    gridX++;
                    tmpX = x - Options.MapWidth;
                }

                if (y > Options.MapHeight - 1)
                {
                    gridY++;
                    tmpY = y - Options.MapHeight;
                }

                if (gridX < 0 || gridY < 0 || gridX >= Globals.MapGridWidth || gridY >= Globals.MapGridHeight)
                {
                    return -2;
                }

                tmpMapId = Globals.MapGrid[gridX, gridY];

                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value == Globals.Me)
                    {
                        continue;
                    }
                    else
                    {
                        if (en.Value.CurrentMap == tmpMapId &&
                            en.Value.X == tmpX &&
                            en.Value.Y == tmpY &&
                            en.Value.Z == Z)
                        {
                            if (en.Value.GetType() != typeof(Projectile))
                            {
                                if (en.Value.GetType() == typeof(Resource))
                                {
                                    var resourceBase = ((Resource)en.Value).GetResourceBase();
                                    if (resourceBase != null)
                                    {
                                        if (!ignoreAliveResources && !((Resource)en.Value).IsDead)
                                        {
                                            blockedBy = en.Value;

                                            return -6;
                                        }

                                        if (!ignoreDeadResources && ((Resource)en.Value).IsDead)
                                        {
                                            blockedBy = en.Value;

                                            return -6;
                                        }

                                        if (resourceBase.WalkableAfter && ((Resource)en.Value).IsDead ||
                                            resourceBase.WalkableBefore && !((Resource)en.Value).IsDead)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else if (en.Value.GetType() == typeof(Player))
                                {
                                    //Return the entity key as this should block the player.  Only exception is if the MapZone this entity is on is passable.
                                    var entityMap = MapInstance.Get(en.Value.CurrentMap);
                                    if (Options.Instance.Passability.Passable[(int)entityMap.ZoneType])
                                    {
                                        continue;
                                    }
                                }

                                blockedBy = en.Value;

                                return -6;
                            }
                        }
                    }
                }

                if (MapInstance.Get(tmpMapId) != null)
                {
                    foreach (var en in MapInstance.Get(tmpMapId).LocalEntities)
                    {
                        if (en.Value == null)
                        {
                            continue;
                        }

                        if (en.Value.CurrentMap == tmpMapId &&
                            en.Value.X == tmpX &&
                            en.Value.Y == tmpY &&
                            en.Value.Z == Z &&
                            !en.Value.Passable)
                        {
                            blockedBy = en.Value;

                            return -4;
                        }
                    }
                }

                var gameMap = MapInstance.Get(Globals.MapGrid[gridX, gridY]);
                if (gameMap != null)
                {
                    if (gameMap.Attributes[tmpX, tmpY] != null)
                    {
                        if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.Blocked)
                        {
                            return -2;
                        }
                        else if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.ZDimension)
                        {
                            if (((MapZDimensionAttribute)gameMap.Attributes[tmpX, tmpY]).BlockedLevel - 1 == z)
                            {
                                return -3;
                            }
                        }
                    }
                }
                else
                {
                    return -5;
                }

                return -1;
            }
            catch
            {
                return -2;
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
        }

        public void DrawTargets()
        {
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                if (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player))
                {
                    if (en.Value.GetType() != typeof(Projectile) && en.Value.GetType() != typeof(Resource))
                    {
                        TargetOnFocus = false;
                        if (TargetType == 0 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int)TargetTypes.Selected);
                            TargetOnFocus = true;
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
                        !((Event)en.Value).DisablePreview &&
                        (!en.Value.IsStealthed() || en.Value is Player player && Globals.Me.IsInMyParty(player)))
                    {
                        if (TargetType == 1 && TargetIndex == en.Value.Id)
                        {
                            en.Value.DrawTarget((int)TargetTypes.Selected);
                        }
                    }
                }
            }

            var x = (int)Math.Floor(Globals.InputManager.GetMousePosition().X + Graphics.CurrentView.Left);
            var y = (int)Math.Floor(Globals.InputManager.GetMousePosition().Y + Graphics.CurrentView.Top);

            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (x >= map.GetX() && x <= map.GetX() + Options.MapWidth * Options.TileWidth)
                {
                    if (y >= map.GetY() && y <= map.GetY() + Options.MapHeight * Options.TileHeight)
                    {
                        var mapId = map.Id;

                        foreach (var en in Globals.Entities)
                        {
                            if (en.Value == null)
                            {
                                continue;
                            }

                            if (en.Value.CurrentMap == mapId &&
                                !en.Value.IsStealthed() &&
                                en.Value.WorldPos.Contains(x, y))
                            {
                                if (en.Value.GetType() != typeof(Projectile) && en.Value.GetType() != typeof(Resource))
                                {
                                    if (TargetType != 0 || TargetIndex != en.Value.Id)
                                    {
                                        en.Value.DrawTarget((int)TargetTypes.Hover);
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
                                    !((Event)en.Value).DisablePreview &&
                                    !en.Value.IsStealthed() &&
                                    en.Value.WorldPos.Contains(x, y))
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

        public int[] PreferredStatBuffs = new int[(int)Stats.StatCount];

        public void Load(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }

    }

}
