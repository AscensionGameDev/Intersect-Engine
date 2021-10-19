using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public class ItemDescriptionWindow : DescriptionWindowBase
    {
        protected ItemBase mItem;

        protected int mAmount;

        protected int[] mStatBuffs;

        protected string mTitleOverride;

        protected string mValueLabel;

        protected SpellDescriptionWindow mSpellDescWindow;

        public ItemDescriptionWindow(
            ItemBase item,
            int amount,
            int x,
            int y,
            int[] statBuffs,
            string titleOverride = "",
            string valueLabel = ""
        ) : base(Interface.GameUi.GameCanvas, "DescriptionWindow")
        {
            mItem = item;
            mAmount = amount;
            mStatBuffs = statBuffs;
            mTitleOverride = titleOverride;
            mValueLabel = valueLabel;

            GenerateComponents();
            SetupDescriptionWindow();
            SetPosition(x, y);

            // If a spell, also display the spell description!
            if (mItem.ItemType == ItemTypes.Spell)
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
                case ItemTypes.Equipment:
                    SetupEquipmentInfo();
                    break;

                case ItemTypes.Consumable:
                    SetupConsumableInfo();
                    break;

                case ItemTypes.Spell:
                    SetupSpellInfo();
                    break;

                case ItemTypes.Bag:
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
            Strings.ItemDescription.ItemTypes.TryGetValue((int) mItem.ItemType, out var typeDesc);
            if (mItem.ItemType == ItemTypes.Equipment)
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
            Strings.ItemDescription.Rarity.TryGetValue(mItem.Rarity, out var rarityDesc);
            header.SetDescription(rarityDesc, rarityColor ?? Color.White);

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
                description.SetText(Strings.ItemDescription.ItemLimits.ToString(string.Join(", ", limits)), Color.White);
            }
        }

        protected void SetupDescription()
        {
            // Add a divider.
            AddDivider();

            // Add the actual description.
            var description = AddDescription();
            description.SetText(Strings.ItemDescription.Description.ToString(mItem.Description), Color.White);
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
                    var speed = Globals.Me.Stat[(int)Stats.Speed];

                    // Remove currently equipped weapon stats.. We want to create a fair display!
                    var weaponSlot = Globals.Me.MyEquipment[Options.WeaponIndex];
                    if (weaponSlot != -1)
                    {
                        var statBuffs = Globals.Me.Inventory[weaponSlot].StatBuffs;
                        var weapon = ItemBase.Get(Globals.Me.Inventory[weaponSlot].ItemId);
                        if (weapon != null && statBuffs != null)
                        {
                            speed = (int) Math.Round(speed / ((100 + weapon.PercentageStatsGiven[(int)Stats.Speed]) / 100f));
                            speed -= weapon.StatsGiven[(int)Stats.Speed];
                            speed -= statBuffs[(int)Stats.Speed];
                        }
                    }

                    // Add current item's speed stats!
                    if (mStatBuffs != null)
                    {
                        speed += mItem.StatsGiven[(int) Stats.Speed];
                        speed += mStatBuffs[(int) Stats.Speed];
                        speed += (int) Math.Floor(speed * (mItem.PercentageStatsGiven[(int)Stats.Speed] / 100f));
                    }

                    // Display the actual speed this weapon would have based off of our calculated speed stat.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Seconds.ToString(Globals.Me.CalculateAttackTime(speed) / 1000f));
                }
                else if (mItem.AttackSpeedModifier == 1)
                {
                    // Static, so this weapon's attack speed.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Seconds.ToString(mItem.AttackSpeedValue / 1000f));
                }
                else if (mItem.AttackSpeedModifier == 2)
                {
                    // Percentage based.
                    rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Percentage.ToString(mItem.AttackSpeedValue));
                }
            }

            // Vitals
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                if (mItem.VitalsGiven[i] > 0 && mItem.PercentageVitalsGiven[i] > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.VitalsGiven[i], mItem.PercentageVitalsGiven[i]));
                }
                else if (mItem.VitalsGiven[i] > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], mItem.VitalsGiven[i].ToString());
                }
                else if (mItem.PercentageVitalsGiven[i] > 0)
                {
                    rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageVitalsGiven[i]));
                }
            }

            // Stats
            if (mStatBuffs != null)
            {
                for (var i = 0; i < (int)Stats.StatCount; i++)
                {
                    var flatStat = mItem.StatsGiven[i] + mStatBuffs[i];
                    if (flatStat > 0 && mItem.PercentageStatsGiven[i] > 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.RegularAndPercentage.ToString(flatStat, mItem.PercentageStatsGiven[i]));
                    }
                    else if (flatStat > 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], flatStat.ToString());
                    }
                    else if (mItem.PercentageStatsGiven[i] > 0)
                    {
                        rows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageStatsGiven[i]));
                    }
                }
            }

            // Bonus Effect
            if (mItem.Effect.Type != EffectType.None && mItem.Effect.Percentage > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.BonusEffects[(int) mItem.Effect.Type], Strings.ItemDescription.Percentage.ToString(mItem.Effect.Percentage));
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
                    rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int) mItem.Consumable.Type], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.Consumable.Value, mItem.Consumable.Percentage));
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
