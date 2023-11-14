using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public partial class ItemDescriptionWindow : DescriptionWindowBase
    {
        protected ItemBase mItem;

        protected int mAmount;

        protected ItemProperties mItemProperties;

        protected string mTitleOverride;

        protected string mValueLabel;

        protected SpellDescriptionWindow mSpellDescWindow;

        public ItemDescriptionWindow(
            ItemBase item,
            int amount,
            int x,
            int y,
            ItemProperties itemProperties,
            string titleOverride = "",
            string valueLabel = "",
            bool centerOnPosition = false
        ) : base(Interface.GameUi.GameCanvas, "DescriptionWindow", centerOnPosition)
        {
            mItem = item;
            mAmount = amount;
            mItemProperties = itemProperties;
            mTitleOverride = titleOverride;
            mValueLabel = valueLabel;

            GenerateComponents();
            SetupDescriptionWindow();
            SetPosition(x, y);

            // If a spell, also display the spell description!
            if (mItem.ItemType == ItemType.Spell)
            {
                mSpellDescWindow = new SpellDescriptionWindow(mItem.SpellId, x, mContainer.Bottom);
            }
        }

        protected void SetupDescriptionWindow()
        {
            if (mItem == null)
            {
                return;
            }

            // Set up our header information.
            SetupHeader();

            // Set up our item limit information.
            SetupItemLimits();

            // if we have a description, set that up.
            if (!string.IsNullOrWhiteSpace(mItem.Description))
            {
                SetupDescription();
            }

            // Set up information depending on the item type.
            switch (mItem.ItemType)
            {
                case ItemType.Equipment:
                    SetupEquipmentInfo();
                    break;

                case ItemType.Consumable:
                    SetupConsumableInfo();
                    break;

                case ItemType.Spell:
                    SetupSpellInfo();
                    break;

                case ItemType.Bag:
                    SetupBagInfo();
                    break;
            }

            // Set up additional information such as amounts and shop values.
            SetupExtraInfo();

            // Resize the container, correct the display and position our window.
            FinalizeWindow();
        }

        protected void SetupHeader()
        {
            // Create our header, but do not load our layout yet since we're adding components manually.
            var header = AddHeader();

            // Set up the icon, if we can load it.
            var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, mItem.Icon);
            if (tex != null)
            {
                header.SetIcon(tex, mItem.Color);
            }

            // Set up the header as the item name.
            CustomColors.Items.Rarities.TryGetValue(mItem.Rarity, out var rarityColor);
            var name = !string.IsNullOrWhiteSpace(mTitleOverride) ? mTitleOverride : mItem.Name;
            header.SetTitle(name, rarityColor ?? Color.White);

            // Set up the description telling us what type of item this is.
            // if equipment, also list what kind.
            Strings.ItemDescription.ItemTypes.TryGetValue((int)mItem.ItemType, out var typeDesc);
            if (mItem.ItemType == ItemType.Equipment)
            {
                var equipSlot = Options.Equipment.Slots[mItem.EquipmentSlot];
                var extraInfo = equipSlot;
                if (mItem.EquipmentSlot == Options.WeaponIndex && mItem.TwoHanded)
                {
                    extraInfo = $"{Strings.ItemDescription.TwoHand} {equipSlot}";
                }
                header.SetSubtitle($"{typeDesc} - {extraInfo}", Color.White);
            }
            else
            {
                header.SetSubtitle(typeDesc, Color.White);
            }

            // Set up the item rarity label.
            try
            {
                var rarityName = Options.Instance.Items.RarityTiers[mItem.Rarity];
                _ = Strings.ItemDescription.Rarity.TryGetValue(rarityName, out var rarityLabel);
                header.SetDescription(rarityLabel, rarityColor ?? Color.White);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }

            header.SizeToChildren(true, false);
        }

        protected void SetupItemLimits()
        {
            // Gather up what limitations apply to this item.
            var limits = new List<string>();
            if (!mItem.CanBank)
            {
                limits.Add(Strings.ItemDescription.Banked);
            }
            if (!mItem.CanGuildBank)
            {
                limits.Add(Strings.ItemDescription.GuildBanked);
            }
            if (!mItem.CanBag)
            {
                limits.Add(Strings.ItemDescription.Bagged);
            }
            if (!mItem.CanTrade)
            {
                limits.Add(Strings.ItemDescription.Traded);
            }
            if (!mItem.CanDrop)
            {
                limits.Add(Strings.ItemDescription.Dropped);
            }
            if (!mItem.CanSell)
            {
                limits.Add(Strings.ItemDescription.Sold);
            }

            // Do we have any limitations? If so, generate a display for it.
            if (limits.Count > 0)
            {
                // Add a divider.
                AddDivider();

                // Add the actual description.
                var description = AddDescription();

                // Commbine our lovely limitations to a single line and display them.
                description.AddText(Strings.ItemDescription.ItemLimits.ToString(string.Join(", ", limits)), Color.White);
            }
        }

        protected void SetupDescription()
        {
            // Add a divider.
            AddDivider();

            // Add the actual description.
            var description = AddDescription();
            description.AddText(Strings.ItemDescription.Description.ToString(mItem.Description), Color.White);
        }

        protected void SetupEquipmentInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Is this a weapon?
            if (mItem.EquipmentSlot == Options.WeaponIndex)
            {
                // Base Damage:
                rows.AddKeyValueRow(Strings.ItemDescription.BaseDamage, mItem.Damage.ToString());

                // Damage Type:
                Strings.ItemDescription.DamageTypes.TryGetValue(mItem.DamageType, out var damageType);
                rows.AddKeyValueRow(Strings.ItemDescription.BaseDamageType, damageType);

                if (mItem.Scaling > 0)
                {
                    Strings.ItemDescription.Stats.TryGetValue(mItem.ScalingStat, out var stat);
                    rows.AddKeyValueRow(Strings.ItemDescription.ScalingStat, stat);
                    rows.AddKeyValueRow(Strings.ItemDescription.ScalingPercentage, Strings.ItemDescription.Percentage.ToString(mItem.Scaling));
                }

                // Crit Chance
                if (mItem.CritChance > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.CritChance, Strings.ItemDescription.Percentage.ToString(mItem.CritChance));
                    rows.AddKeyValueRow(Strings.ItemDescription.CritMultiplier, Strings.ItemDescription.Multiplier.ToString(mItem.CritMultiplier));
                }

                // Attack Speed
                // Are we supposed to change our attack time based on a modifier?
                if (mItem.AttackSpeedModifier == 0)
                {
                    // No modifier, assuming base attack rate? We have to calculate the speed stat manually here though..!
                    var speed = Globals.Me.Stat[(int)Stat.Speed];

                    // Remove currently equipped weapon stats.. We want to create a fair display!
                    var weaponSlot = Globals.Me.MyEquipment[Options.WeaponIndex];
                    if (weaponSlot != -1)
                    {
                        var randomStats = Globals.Me.Inventory[weaponSlot].ItemProperties.StatModifiers;
                        var weapon = ItemBase.Get(Globals.Me.Inventory[weaponSlot].ItemId);
                        if (weapon != null && randomStats != null)
                        {
                            speed = (int)Math.Round(speed / ((100 + weapon.PercentageStatsGiven[(int)Stat.Speed]) / 100f));
                            speed -= weapon.StatsGiven[(int)Stat.Speed];
                            speed -= randomStats[(int)Stat.Speed];
                        }
                    }

                    // Add current item's speed stats!
                    if (mItemProperties?.StatModifiers != default)
                    {
                        speed += mItem.StatsGiven[(int)Stat.Speed];
                        speed += mItemProperties.StatModifiers[(int)Stat.Speed];
                        speed += (int)Math.Floor(speed * (mItem.PercentageStatsGiven[(int)Stat.Speed] / 100f));
                    }

                    // Display the actual speed this weapon would have based off of our calculated speed stat.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, TimeSpan.FromMilliseconds(Globals.Me.CalculateAttackTime(speed)).WithSuffix());
                }
                else if (mItem.AttackSpeedModifier == 1)
                {
                    // Static, so this weapon's attack speed.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, TimeSpan.FromMilliseconds(mItem.AttackSpeedValue).WithSuffix());
                }
                else if (mItem.AttackSpeedModifier == 2)
                {
                    // Percentage based.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Percentage.ToString(mItem.AttackSpeedValue));
                }
            }

            //Blocking options
            if (mItem.EquipmentSlot == Options.ShieldIndex)
            {
                if (mItem.BlockChance > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.BlockChance, Strings.ItemDescription.Percentage.ToString(mItem.BlockChance));
                }

                if (mItem.BlockAmount > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.BlockAmount, Strings.ItemDescription.Percentage.ToString(mItem.BlockAmount));
                }

                if (mItem.BlockAbsorption > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.BlockAbsorption, Strings.ItemDescription.Percentage.ToString(mItem.BlockAbsorption));
                }
            }

            // Vitals
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                if (mItem.VitalsGiven[i] != 0 && mItem.PercentageVitalsGiven[i] != 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.VitalsGiven[i], mItem.PercentageVitalsGiven[i]));
                }
                else if (mItem.VitalsGiven[i] != 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], mItem.VitalsGiven[i].ToString());
                }
                else if (mItem.PercentageVitalsGiven[i] != 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageVitalsGiven[i]));
                }
            }

            // Vitals Regen
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                if (mItem.VitalsRegen[i] != 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.VitalsRegen[i], Strings.ItemDescription.Percentage.ToString(mItem.VitalsRegen[i]));
                }
            }

            // Stats
            var statModifiers = mItemProperties?.StatModifiers;
            for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
            {
                // Do we have item properties, if so this is a finished item. Otherwise does this item not have growing stats?
                if (statModifiers != default || mItem.StatRanges?.Length == 0)
                {
                    var flatStat = mItem.StatsGiven[i];
                    if (statModifiers != default)
                    {
                        flatStat += statModifiers[i];
                    }

                    if (flatStat != 0 && mItem.PercentageStatsGiven[i] != 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.RegularAndPercentage.ToString(flatStat, mItem.PercentageStatsGiven[i]));
                    }
                    else if (flatStat != 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], flatStat.ToString());
                    }
                    else if (mItem.PercentageStatsGiven[i] != 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageStatsGiven[i]));
                    }
                }
                // We do not have item properties and have growing stats! So don't display a finished stat but a range instead.
                else
                {
                    _ = mItem.TryGetRangeFor((Stat)i, out var range);
                    var statLow = mItem.StatsGiven[i] + range.LowRange;
                    var statHigh = mItem.StatsGiven[i] + range.HighRange;

                    if (mItem.PercentageStatsGiven[i] != 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.RegularAndPercentage.ToString(Strings.ItemDescription.StatGrowthRange.ToString(statLow, statHigh), mItem.PercentageStatsGiven[i]));
                    }
                    else
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.StatGrowthRange.ToString(statLow, statHigh));
                    }
                }

            }

            // Bonus Effect
            foreach (var effect in mItem.Effects)
            {
                if (effect.Type != ItemEffect.None && effect.Percentage != 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.BonusEffects[(int)effect.Type], Strings.ItemDescription.Percentage.ToString(effect.Percentage));
                }
            }

            // Resize the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupConsumableInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Consumable data.
            if (mItem.Consumable != null)
            {
                if (mItem.Consumable.Value > 0 && mItem.Consumable.Percentage > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)mItem.Consumable.Type], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.Consumable.Value, mItem.Consumable.Percentage));
                }
                else if (mItem.Consumable.Value > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)mItem.Consumable.Type], mItem.Consumable.Value.ToString());
                }
                else if (mItem.Consumable.Percentage > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)mItem.Consumable.Type], Strings.ItemDescription.Percentage.ToString(mItem.Consumable.Percentage));
                }
            }

            // Resize and position the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupSpellInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Spell data.
            if (mItem.Spell != null)
            {
                if (mItem.QuickCast)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.CastSpell.ToString(mItem.Spell.Name), string.Empty);
                }
                else
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.TeachSpell.ToString(mItem.Spell.Name), string.Empty);
                }

                if (mItem.SingleUse)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.SingleUse, string.Empty);
                }
            }

            // Resize and position the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupBagInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Bag data.
            rows.AddKeyValueRow(Strings.ItemDescription.BagSlots, mItem.SlotCount.ToString());

            // Resize and position the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupExtraInfo()
        {
            // Our list of data to add, should we need to.
            var data = new List<Tuple<string, string>>();

            // Display our amount, but only if we are stackable and have more than one.
            if (mItem.IsStackable && mAmount > 1)
            {
                data.Add(new Tuple<string, string>(Strings.ItemDescription.Amount, mAmount.ToString("N0").Replace(",", Strings.Numbers.comma)));
            }

            // Display item drop chance if configured.
            if (mItem.DropChanceOnDeath > 0)
            {
                data.Add(new Tuple<string, string>(Strings.ItemDescription.DropOnDeath, Strings.ItemDescription.Percentage.ToString(mItem.DropChanceOnDeath)));
            }

            // Display shop value if we have one.
            if (!string.IsNullOrWhiteSpace(mValueLabel))
            {
                data.Add(new Tuple<string, string>(mValueLabel, string.Empty));
            }

            // Do we have any data to display? If so, generate the element and add the data to it.
            if (data.Count > 0)
            {
                // Add a divider.
                AddDivider();

                // Add a row component.
                var rows = AddRowContainer();

                foreach (var item in data)
                {
                    rows.AddKeyValueRow(item.Item1, item.Item2);
                }

                // Resize and position the container.
                rows.SizeToChildren(true, true);
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
            mSpellDescWindow?.Dispose();
        }
    }
}
