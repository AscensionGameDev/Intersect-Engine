using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public class ItemDescriptionWindow : DescriptionWindowBase
    {
        protected ItemBase mItem;

        protected int mAmount;

        protected int mX;

        protected int mY;

        protected int[] mStatBuffs;

        protected string mTitleOverride;

        protected string mValueLabel;

        protected HeaderComponent mHeader;

        protected DividerComponent mDivider1;

        protected DividerComponent mDivider2;

        protected DividerComponent mDivider3;

        protected DividerComponent mDivider4;

        protected DescriptionComponent mItemLimits;

        protected DescriptionComponent mDescription;

        protected RowContainerComponent mDetailRows;

        protected RowContainerComponent mValueInfo;

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
            mX = x;
            mY = y;
            mStatBuffs = statBuffs;
            mTitleOverride = titleOverride;
            mValueLabel = valueLabel;

            GenerateComponents();
            SetupDescriptionWindow();
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

            // if we've been passed a value label, set that up.
            if (!string.IsNullOrWhiteSpace(mValueLabel))
            {
                SetupValueInfo();
            }

            // Resize the container, correct the display and position our window.
            CorrectDisplay();
            PositionWindow();

            // If a spell, also display the spell description!
            if (mItem.ItemType == ItemTypes.Spell)
            {
                mSpellDescWindow = new SpellDescriptionWindow(mItem.SpellId, mX, mContainer.Bottom);
            }
        }

        protected void SetupHeader()
        {
            // Create our header, but do not load our layout yet since we're adding components manually.
            mHeader = AddHeader();

            // Set up the icon, if we can load it.
            var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, mItem.Icon);
            if (tex != null)
            {
                mHeader.SetIcon(tex, mItem.Color);
            }

            // Set up the header as the item name.
            CustomColors.Items.Rarities.TryGetValue(mItem.Rarity, out var rarityColor);
            var name = !string.IsNullOrWhiteSpace(mTitleOverride) ? mTitleOverride : mItem.Name;
            mHeader.SetTitle(name, rarityColor ?? Color.White);

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
                mHeader.SetSubtitle($"{typeDesc} - {extraInfo}", Color.White);
            }
            else
            {
                mHeader.SetSubtitle(typeDesc, Color.White);
            }

            // Set up the item rarity label.
            Strings.ItemDescription.Rarity.TryGetValue(mItem.Rarity, out var rarityDesc);
            mHeader.SetDescription(rarityDesc, rarityColor ?? Color.White);

            mHeader.SizeToChildren(true, false);
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
                mDivider1 = AddDivider();

                // Add the actual description.
                mItemLimits = AddDescription();

                // Commbine our lovely limitations to a single line and display them.
                mItemLimits.SetText(Strings.ItemDescription.ItemLimits.ToString(string.Join(", ", limits)), Color.White);
            }
        }

        protected void SetupDescription()
        {
            // Add a divider.
            mDivider2 = AddDivider();

            // Add the actual description.
            mDescription = AddDescription();
            mDescription.SetText(Strings.ItemDescription.Description.ToString(mItem.Description), Color.White);
        }

        protected void SetupEquipmentInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();

            // Is this a weapon?
            if (mItem.EquipmentSlot == Options.WeaponIndex)
            {
                // Base Damage:
                mDetailRows.AddKeyValueRow(Strings.ItemDescription.BaseDamage, mItem.Damage.ToString());

                // Damage Type:
                Strings.ItemDescription.DamageTypes.TryGetValue(mItem.DamageType, out var damageType);
                mDetailRows.AddKeyValueRow(Strings.ItemDescription.BaseDamageType, damageType);

                if (mItem.Scaling > 0)
                {
                    Strings.ItemDescription.Stats.TryGetValue(mItem.ScalingStat, out var stat);
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.ScalingStat, stat);
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.ScalingPercentage, Strings.ItemDescription.Percentage.ToString(mItem.Scaling));
                }

                // Crit Chance
                if (mItem.CritChance > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.CritChance, Strings.ItemDescription.Percentage.ToString(mItem.CritChance));
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.CritMultiplier, Strings.ItemDescription.Multiplier.ToString(mItem.CritMultiplier));
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
                        var weapon = ItemBase.Get(Globals.Me.Inventory[Globals.Me.MyEquipment[Options.WeaponIndex]].ItemId);
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
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Seconds.ToString(Globals.Me.CalculateAttackTime(speed) / 1000f));
                }
                else if (mItem.AttackSpeedModifier == 1)
                {
                    // Static, so this weapon's attack speed.
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Seconds.ToString(mItem.AttackSpeedValue / 1000f));
                }
                else if (mItem.AttackSpeedModifier == 2)
                {
                    // Percentage based.
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Percentage.ToString(mItem.AttackSpeedValue));
                }
                

            }

            // Vitals
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                if (mItem.VitalsGiven[i] > 0 && mItem.PercentageVitalsGiven[i] > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.VitalsGiven[i], mItem.PercentageVitalsGiven[i]));
                }
                else if (mItem.VitalsGiven[i] > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], mItem.VitalsGiven[i].ToString());
                }
                else if (mItem.PercentageVitalsGiven[i] > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageVitalsGiven[i]));
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
                        mDetailRows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.RegularAndPercentage.ToString(flatStat, mItem.PercentageStatsGiven[i]));
                    }
                    else if (flatStat > 0)
                    {
                        mDetailRows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], flatStat.ToString());
                    }
                    else if (mItem.PercentageStatsGiven[i] > 0)
                    {
                        mDetailRows.AddKeyValueRow(Strings.ItemDescription.StatCounts[i], Strings.ItemDescription.Percentage.ToString(mItem.PercentageStatsGiven[i]));
                    }
                }
            }

            // Bonus Effect
            if (mItem.Effect.Type != EffectType.None && mItem.Effect.Percentage > 0)
            {
                mDetailRows.AddKeyValueRow(Strings.ItemDescription.BonusEffects[(int) mItem.Effect.Type], Strings.ItemDescription.Percentage.ToString(mItem.Effect.Percentage));
            }

            // Resize the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupConsumableInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();

            // Consumable data.
            if (mItem.Consumable != null)
            {
                if (mItem.Consumable.Value > 0 && mItem.Consumable.Percentage > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int) mItem.Consumable.Type], Strings.ItemDescription.RegularAndPercentage.ToString(mItem.Consumable.Value, mItem.Consumable.Percentage));
                }
                else if (mItem.Consumable.Value > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)mItem.Consumable.Type], mItem.Consumable.Value.ToString());
                }
                else if (mItem.Consumable.Percentage > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)mItem.Consumable.Type], Strings.ItemDescription.Percentage.ToString(mItem.Consumable.Percentage));
                }
            }

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupSpellInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();

            // Spell data.
            if (mItem.Spell != null)
            {
                if (mItem.QuickCast)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.CastSpell.ToString(mItem.Spell.Name), string.Empty);
                }
                else
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.TeachSpell.ToString(mItem.Spell.Name), string.Empty);
                }

                if (mItem.SingleUse)
                {
                    mDetailRows.AddKeyValueRow(Strings.ItemDescription.SingleUse, string.Empty);
                }
            }

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupBagInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();

            // Bag data.
            mDetailRows.AddKeyValueRow(Strings.ItemDescription.BagSlots, mItem.SlotCount.ToString());

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupValueInfo()
        {
            // Add a divider.
            mDivider4 = AddDivider();

            // Add a row component.
            mValueInfo = AddRowContainer();

            // Display shop value.
            mValueInfo.AddKeyValueRow(mValueLabel, string.Empty);

            // Resize and position the container.
            mValueInfo.SizeToChildren(true, true);
        }

        protected void CorrectDisplay()
        {
            // Set the correct width.
            mContainer.SizeToChildren(true, false);

            // Adjust all components.
            mHeader?.CorrectWidth();
            mDivider1?.CorrectWidth();
            mItemLimits?.CorrectWidth();
            mDivider2?.CorrectWidth();
            mDescription?.CorrectWidth();
            mDivider3?.CorrectWidth();
            mDetailRows?.CorrectWidth();
            mDivider4?.CorrectWidth();
            mValueInfo?.CorrectWidth();

            // Position all components.
            PositionComponent(mHeader);
            PositionComponent(mDivider1);
            PositionComponent(mItemLimits);
            PositionComponent(mDivider2);
            PositionComponent(mDescription);
            PositionComponent(mDivider3);
            PositionComponent(mDetailRows);
            PositionComponent(mDivider4);
            PositionComponent(mValueInfo);

            // Set the correct height.
            mContainer.SizeToChildren(false, true);
        }

        protected void PositionWindow()
        {
            mContainer.MoveTo(mX - mContainer.Width - mContainer.Padding.Right, mY + mContainer.Padding.Top);
        }

        public override void Dispose()
        {
            base.Dispose();
            mSpellDescWindow?.Dispose();
        }
    }
}
