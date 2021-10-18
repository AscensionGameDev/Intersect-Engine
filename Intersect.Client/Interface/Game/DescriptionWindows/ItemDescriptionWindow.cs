using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Core;
using Intersect.Enums;
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

        protected bool mCenterHorizontally;

        protected HeaderComponent mHeader;

        protected DividerComponent mDivider1;

        protected DividerComponent mDivider2;

        protected DividerComponent mDivider3;

        protected DescriptionComponent mDescription;

        protected RowContainerComponent mDetailRows;

        protected RowContainerComponent mValueInfo;

        protected SpellDescWindow mSpellDescWindow;

        public ItemDescriptionWindow(
            ItemBase item,
            int amount,
            int x,
            int y,
            int[] statBuffs,
            string titleOverride = "",
            string valueLabel = ""
        ) : base(Interface.GameUi.GameCanvas, "ItemDescriptionWindow")
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

            SetupHeader();
            if (!string.IsNullOrWhiteSpace(mItem.Description))
            {
                SetupDescription();
            }

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
                mSpellDescWindow = new SpellDescWindow(mItem.SpellId, mX, mContainer.Bottom);
            }
        }

        protected void SetupHeader()
        {
            // Create our header, but do not load our layout yet since we're adding components manually.
            mHeader = AddHeader("ItemDescriptionWindowHeader", false);

            // Generate additional components as required, then load in our layout file and set our position.
            var rarity = new Label(mHeader.Container, "Rarity");
            mHeader.LoadLayout();

            // Set up the header as the item name.
            CustomColors.Items.Rarities.TryGetValue(mItem.Rarity, out var headerColor);
            var name = !string.IsNullOrWhiteSpace(mTitleOverride) ? mTitleOverride : mItem.Name;
            mHeader.SetHeaderText(name, headerColor ?? Color.White);

            // Set up the item rarity label.
            Strings.ItemDescription.Rarity.TryGetValue(mItem.Rarity, out var rarityDesc);
            rarity.SetText(rarityDesc);

            // Set up the icon, if we can load it.
            var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, mItem.Icon);
            if (tex != null)
            {
                mHeader.SetIcon(tex, mItem.Color);
            }

            // Set up the description telling us what type of item this is.
            // if equipment, also list what kind.
            Strings.ItemDescription.ItemTypes.TryGetValue((int) mItem.ItemType, out var typeDesc);
            if (mItem.ItemType == ItemTypes.Equipment)
            {
                var equipSlot = Options.Equipment.Slots[mItem.EquipmentSlot];
                var extraInfo = equipSlot;
                if (equipSlot == "Weapon" && mItem.TwoHanded)
                {
                    extraInfo = $"{Strings.ItemDescription.TwoHand} {equipSlot}";
                }
                mHeader.SetDescriptionText($"{typeDesc} - {extraInfo}", Color.White);
            }
            else
            {
                mHeader.SetDescriptionText(typeDesc, Color.White);
            }

            mHeader.SizeToChildren(true, false);
        }

        protected void SetupDescription()
        {
            // Add a divider.
            mDivider1 = AddDivider("DescriptionWindowDivider");

            // Add the actual description.
            mDescription = AddDescription("DescriptionWindowDescription");
            mDescription.SetText(Strings.ItemDescription.Description.ToString(mItem.Description), Color.White);
        }

        protected void SetupEquipmentInfo()
        {
            // Add a divider.
            mDivider2 = AddDivider("DescriptionWindowDivider");

            // Add a row component.
            mDetailRows = AddRowContainer("DescriptionWindowStatRows");

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
            mDivider2 = AddDivider("DescriptionWindowDivider");

            // Add a row component.
            mDetailRows = AddRowContainer("DescriptionWindowStatRows");

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
            mDivider2 = AddDivider("DescriptionWindowDivider");

            // Add a row component.
            mDetailRows = AddRowContainer("DescriptionWindowStatRows");

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
            mDivider2 = AddDivider("DescriptionWindowDivider");

            // Add a row component.
            mDetailRows = AddRowContainer("DescriptionWindowStatRows");

            // Bag data.
            mDetailRows.AddKeyValueRow(Strings.ItemDescription.BagSlots, mItem.SlotCount.ToString());

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupValueInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider("DescriptionWindowDivider");

            // Add a row component.
            mValueInfo = AddRowContainer("DescriptionWindowStatRows");

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
            mDescription?.CorrectWidth();
            mDivider2?.CorrectWidth();
            mDetailRows?.CorrectWidth();
            mDivider3?.CorrectWidth();
            mValueInfo?.CorrectWidth();

            // Position all components.
            PositionComponent(mHeader);
            PositionComponent(mDivider1);
            PositionComponent(mDescription);
            PositionComponent(mDivider2);
            PositionComponent(mDetailRows);
            PositionComponent(mDivider3);
            PositionComponent(mValueInfo);

            // Set the correct height.
            mContainer.SizeToChildren(false, true);
        }

        protected void PositionWindow()
        {
            if (mCenterHorizontally)
            {
                mContainer.MoveTo(mX - mContainer.Width / 2, mY + mContainer.Padding.Top);
            }
            else
            {
                mContainer.MoveTo(mX - mContainer.Width - mContainer.Padding.Right, mY + mContainer.Padding.Top);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            mSpellDescWindow?.Dispose();
        }
    }
}
