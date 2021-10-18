using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Core;
using Intersect.Enums;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;
using System.Collections.Generic;
using System;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public class SpellDescriptionWindow : DescriptionWindowBase
    {
        protected SpellBase mSpell;

        protected int mAmount;

        protected int mX;

        protected int mY;

        protected HeaderComponent mHeader;

        protected DividerComponent mDivider1;

        protected DividerComponent mDivider2;

        protected DividerComponent mDivider3;

        protected DividerComponent mDivider4;

        protected RowContainerComponent mSpellInfo;

        protected DescriptionComponent mDescription;

        protected RowContainerComponent mDetailRows;

        protected RowContainerComponent mExtraInfo;

        public SpellDescriptionWindow(Guid spellId, int x, int y) : base(Interface.GameUi.GameCanvas, "DescriptionWindow")
        {
            mSpell = SpellBase.Get(spellId);
            mX = x;
            mY = y;

            GenerateComponents();
            SetupDescriptionWindow();
        }

        protected void SetupDescriptionWindow()
        {
            if (mSpell == null)
            {
                return;
            }

            // Set up our header information.
            SetupHeader();

            // Set up our basic spell info.
            SetupSpellInfo();

            // if we have a description, set that up.
            if (!string.IsNullOrWhiteSpace(mSpell.Description))
            {
                SetupDescription();
            }

            // Set up information depending on the item type.
            switch (mSpell.SpellType)
            {
                case SpellTypes.CombatSpell:
                case SpellTypes.WarpTo:
                    SetupCombatInfo();
                    break;
                case SpellTypes.Dash:
                    SetupDashInfo();
                    break;
            }

            if (mSpell.Bound)
            {
                SetupExtraInfo();
            }

            // Resize the container, correct the display and position our window.
            CorrectDisplay();
            PositionWindow();

        }

        protected void SetupHeader()
        {
            // Create our header, but do not load our layout yet since we're adding components manually.
            mHeader = AddHeader();

            // Set up the icon, if we can load it.
            var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Spell, mSpell.Icon);
            if (tex != null)
            {
                mHeader.SetIcon(tex, Color.White);
            }

            // Set up the header as the item name.
            mHeader.SetTitle(mSpell.Name, Color.White);

            // Set up the spell type description.
            Strings.SpellDesc.SpellTypes.TryGetValue((int) mSpell.SpellType, out var spellType);
            mHeader.SetSubtitle(spellType, Color.White);

            // Set up the spelldescription based on what kind of spell it is.
            if (mSpell.SpellType == (int)SpellTypes.CombatSpell)
            {
                if (mSpell.Combat.TargetType == SpellTargetTypes.Projectile)
                {
                    var proj = ProjectileBase.Get(mSpell.Combat.ProjectileId);
                    mHeader.SetDescription(Strings.SpellDesc.TargetTypes[(int)mSpell.Combat.TargetType].ToString(proj?.Range ?? 0, mSpell.Combat.HitRadius), Color.White);
                }
                else
                {
                    mHeader.SetDescription(Strings.SpellDesc.TargetTypes[(int)mSpell.Combat.TargetType].ToString(mSpell.Combat.CastRange, mSpell.Combat.HitRadius), Color.White);
                }
            }

            mHeader.SizeToChildren(true, false);
        }

        protected void SetupSpellInfo()
        {
            // Add a divider.
            mDivider1 = AddDivider();
            
            // Add a new row control to add our details into.
            mSpellInfo = AddRowContainer();

            // Friendly / Non Friendly for combat spells.
            if (mSpell.SpellType == SpellTypes.CombatSpell || mSpell.SpellType == SpellTypes.WarpTo)
            {
                if (mSpell.Combat.Friendly)
                {
                    mSpellInfo.AddKeyValueRow(Strings.SpellDesc.Friendly, string.Empty);
                }
                else
                {
                    mSpellInfo.AddKeyValueRow(Strings.SpellDesc.Unfriendly, string.Empty);
                }
            }

            // Add cast time
            var castTime = Strings.SpellDesc.Instant;
            if (mSpell.CastDuration > 0)
            {
                castTime = Strings.SpellDesc.Seconds.ToString(mSpell.CastDuration / 1000f);
            }
            mSpellInfo.AddKeyValueRow(Strings.SpellDesc.CastTime, castTime);

            // Add Vital Costs
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                if (mSpell.VitalCost[i] > 0)
                {
                    mSpellInfo.AddKeyValueRow(Strings.SpellDesc.VitalCosts[i], mSpell.VitalCost[i].ToString());
                }
            }

            // Add Cooldown time
            if (mSpell.CooldownDuration > 0)
            {
                mSpellInfo.AddKeyValueRow(Strings.SpellDesc.Cooldown, Strings.SpellDesc.Seconds.ToString(mSpell.CooldownDuration / 1000f));
            }

            // Add Cooldown Group
            if (!string.IsNullOrWhiteSpace(mSpell.CooldownGroup))
            {
                mSpellInfo.AddKeyValueRow(Strings.SpellDesc.CooldownGroup, mSpell.CooldownGroup);
            }

            // Ignores global cooldown if enabled?
            if (Options.Instance.CombatOpts.EnableGlobalCooldowns && mSpell.IgnoreGlobalCooldown)
            {
                mSpellInfo.AddKeyValueRow(Strings.SpellDesc.IgnoreGlobalCooldown, string.Empty);
            }

            // Ignore cooldown reduction stat?
            if (mSpell.IgnoreCooldownReduction)
            {
                mSpellInfo.AddKeyValueRow(Strings.SpellDesc.IgnoreCooldownReduction, string.Empty);
            }

            // Resize the container.
            mSpellInfo.SizeToChildren(true, true);
        }

        protected void SetupDescription()
        {
            // Add a divider.
            mDivider2 = AddDivider();

            // Add the actual description.
            mDescription = AddDescription();
            mDescription.SetText(Strings.ItemDescription.Description.ToString(mSpell.Description), Color.White);
        }

        protected void SetupCombatInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();



            // Vital Damage, if 0 don't display!
            // This bit is a bit iffy.. since in 
            var isHeal = false;
            var isDamage = false;
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                if (mSpell.Combat.VitalDiff[i] < 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.SpellDesc.VitalRecovery[i], Math.Abs(mSpell.Combat.VitalDiff[i]).ToString());
                    isHeal = true;
                }
                else if (mSpell.Combat.VitalDiff[i] > 0)
                {
                    mDetailRows.AddKeyValueRow(Strings.SpellDesc.VitalDamage[i], mSpell.Combat.VitalDiff[i].ToString());
                    isDamage = true;
                }
            }

            // Damage Type:
            Strings.SpellDesc.DamageTypes.TryGetValue(mSpell.Combat.DamageType, out var damageType);
            mDetailRows.AddKeyValueRow(Strings.SpellDesc.DamageType, damageType);

            if (mSpell.Combat.Scaling > 0)
            {
                Strings.SpellDesc.Stats.TryGetValue(mSpell.Combat.ScalingStat, out var stat);
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.ScalingStat, stat);
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.ScalingPercentage, Strings.SpellDesc.Percentage.ToString(mSpell.Combat.Scaling));
            }

            // Crit Chance
            if (mSpell.Combat.CritChance > 0)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.CritChance, Strings.SpellDesc.Percentage.ToString(mSpell.Combat.CritChance));
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.CritMultiplier, Strings.SpellDesc.Multiplier.ToString(mSpell.Combat.CritMultiplier));
            }

            var showDuration = false;
            // Handle Stat Buffs
            var blankAdded = false;
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                Tuple<string, string> data = null;
                if (mSpell.Combat.StatDiff[i] > 0 && mSpell.Combat.PercentageStatDiff[i] > 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDesc.StatCounts[i], Strings.SpellDesc.RegularAndPercentage.ToString(mSpell.Combat.StatDiff[i], mSpell.Combat.PercentageStatDiff[i]));
                }
                else if (mSpell.Combat.StatDiff[i] > 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDesc.StatCounts[i], mSpell.Combat.StatDiff[i].ToString());
                }
                else if (mSpell.Combat.PercentageStatDiff[i] > 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDesc.StatCounts[i], Strings.ItemDescription.Percentage.ToString(mSpell.Combat.PercentageStatDiff[i]));
                }

                // Make sure we only add a blank row the first time we add a stat row.
                if (data != null)
                {
                    if (!blankAdded)
                    {
                        mDetailRows.AddKeyValueRow(string.Empty, string.Empty);
                        mDetailRows.AddKeyValueRow(Strings.SpellDesc.StatBuff, string.Empty);
                        blankAdded = true;
                    }

                    mDetailRows.AddKeyValueRow(data.Item1, data.Item2);
                    showDuration = true;
                }
            }

            // Handle HoT and DoT displays.
            if (mSpell.Combat.HoTDoT)
            {
                showDuration = true;
                mDetailRows.AddKeyValueRow(string.Empty, string.Empty);
                if (isHeal)
                {
                    mDetailRows.AddKeyValueRow(Strings.SpellDesc.HoT, string.Empty);
                } 
                else if (isDamage)
                {
                    mDetailRows.AddKeyValueRow(Strings.SpellDesc.DoT, string.Empty);
                }
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.Tick, Strings.SpellDesc.Seconds.ToString(mSpell.Combat.HotDotInterval / 1000f));
            }

            // Handle effect display.
            if (mSpell.Combat.Effect != StatusTypes.None)
            {
                showDuration = true;
                mDetailRows.AddKeyValueRow(string.Empty, string.Empty);
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.Effect, Strings.SpellDesc.Effects[(int) mSpell.Combat.Effect]);
            }

            // Show Stat Buff / Effect / HoT / DoT duration.
            if (showDuration)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.Duration, Strings.SpellDesc.Seconds.ToString(mSpell.Combat.Duration / 1000f));
            }

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupDashInfo()
        {
            // Add a divider.
            mDivider3 = AddDivider();

            // Add a row component.
            mDetailRows = AddRowContainer();

            // Dash Distance Information.
            mDetailRows.AddKeyValueRow(Strings.SpellDesc.Distance, Strings.SpellDesc.Tiles.ToString(mSpell.Combat.CastRange));

            // Ignore map blocks?
            if (mSpell.Dash.IgnoreMapBlocks)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.IgnoreMapBlock, String.Empty);
            }

            // Ignore resource blocks?
            if (mSpell.Dash.IgnoreActiveResources)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.IgnoreResourceBlock, String.Empty);
            }

            // Ignore inactive resource blocks?
            if (mSpell.Dash.IgnoreInactiveResources)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.IgnoreConsumedResourceBlock, String.Empty);
            }

            // Ignore Z-Dimension?
            if (Options.Map.ZDimensionVisible && mSpell.Dash.IgnoreZDimensionAttributes)
            {
                mDetailRows.AddKeyValueRow(Strings.SpellDesc.IgnoreZDimension, String.Empty);
            }

            // Resize and position the container.
            mDetailRows.SizeToChildren(true, true);
        }

        protected void SetupExtraInfo()
        {
            // Add a divider.
            mDivider4 = AddDivider();

            // Add a row component.
            mExtraInfo = AddRowContainer();

            // Display shop value.
            mExtraInfo.AddKeyValueRow(Strings.SpellDesc.Bound, string.Empty);

            // Resize and position the container.
            mExtraInfo.SizeToChildren(true, true);
        }

        protected void CorrectDisplay()
        {
            // Set the correct width.
            mContainer.SizeToChildren(true, false);

            // Adjust all components.
            mHeader?.CorrectWidth();
            mDivider1?.CorrectWidth();
            mSpellInfo?.CorrectWidth();
            mDivider2?.CorrectWidth();
            mDescription?.CorrectWidth();
            mDivider3?.CorrectWidth();
            mDetailRows?.CorrectWidth();
            mDivider4?.CorrectWidth();
            mExtraInfo?.CorrectWidth();

            // Position all components.
            PositionComponent(mHeader);
            PositionComponent(mDivider1);
            PositionComponent(mSpellInfo);
            PositionComponent(mDivider2);
            PositionComponent(mDescription);
            PositionComponent(mDivider3);
            PositionComponent(mDetailRows);
            PositionComponent(mDivider4);
            PositionComponent(mExtraInfo);

            // Set the correct height.
            mContainer.SizeToChildren(false, true);
        }

        protected void PositionWindow()
        {
            mContainer.MoveTo(mX - mContainer.Width - mContainer.Padding.Right, mY + mContainer.Padding.Top);
        }

    }
}
