using System;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.DescriptionWindows
{
    public partial class SpellDescriptionWindow : DescriptionWindowBase
    {
        protected SpellBase mSpell;

        public SpellDescriptionWindow(Guid spellId, int x, int y, bool centerOnPosition = false) : base(Interface.GameUi.GameCanvas, "DescriptionWindow", centerOnPosition)
        {
            mSpell = SpellBase.Get(spellId);

            GenerateComponents();
            SetupDescriptionWindow();
            SetPosition(x, y);
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
                case SpellType.CombatSpell:
                case SpellType.WarpTo:
                    SetupCombatInfo();
                    break;
                case SpellType.Dash:
                    SetupDashInfo();
                    break;
            }

            // Set up bind info, if applicable.
            SetupExtraInfo();


            // Resize the container, correct the display and position our window.
            FinalizeWindow();
        }

        protected void SetupHeader()
        {
            // Create our header, but do not load our layout yet since we're adding components manually.
            var header = AddHeader();

            // Set up the icon, if we can load it.
            var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Spell, mSpell.Icon);
            if (tex != null)
            {
                header.SetIcon(tex, Color.White);
            }

            // Set up the header as the item name.
            header.SetTitle(mSpell.Name, Color.White);

            // Set up the spell type description.
            Strings.SpellDescription.SpellTypes.TryGetValue((int)mSpell.SpellType, out var spellType);
            header.SetSubtitle(spellType, Color.White);

            // Set up the spelldescription based on what kind of spell it is.
            if (mSpell.SpellType == (int)SpellType.CombatSpell)
            {
                if (mSpell.Combat.TargetType == SpellTargetType.Projectile)
                {
                    var proj = ProjectileBase.Get(mSpell.Combat.ProjectileId);
                    header.SetDescription(Strings.SpellDescription.TargetTypes[(int)mSpell.Combat.TargetType].ToString(proj?.Range ?? 0, mSpell.Combat.HitRadius), Color.White);
                }
                else
                {
                    header.SetDescription(Strings.SpellDescription.TargetTypes[(int)mSpell.Combat.TargetType].ToString(mSpell.Combat.CastRange, mSpell.Combat.HitRadius), Color.White);
                }
            }

            header.SizeToChildren(true, false);
        }

        protected void SetupSpellInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a new row control to add our details into.
            var rows = AddRowContainer();

            // Friendly / Non Friendly for combat spells.
            if (mSpell.SpellType == SpellType.CombatSpell || mSpell.SpellType == SpellType.WarpTo)
            {
                if (mSpell.Combat.Friendly)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.Friendly, string.Empty);
                }
                else
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.Unfriendly, string.Empty);
                }
            }

            // Add cast time
            var castTime = Strings.SpellDescription.Instant;
            if (mSpell.CastDuration > 0)
            {
                castTime = TimeSpan.FromMilliseconds(mSpell.CastDuration).WithSuffix();
            }
            rows.AddKeyValueRow(Strings.SpellDescription.CastTime, castTime);

            // Add Vital Costs
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                if (mSpell.VitalCost[i] != 0)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.VitalCosts[i], mSpell.VitalCost[i].ToString());
                }
            }

            // Add Cooldown time
            if (mSpell.CooldownDuration > 0)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.Cooldown, TimeSpan.FromMilliseconds(mSpell.CooldownDuration).WithSuffix());
            }

            // Add Cooldown Group
            if (!string.IsNullOrWhiteSpace(mSpell.CooldownGroup))
            {
                rows.AddKeyValueRow(Strings.SpellDescription.CooldownGroup, mSpell.CooldownGroup);
            }

            // Ignores global cooldown if enabled?
            if (Options.Instance.CombatOpts.EnableGlobalCooldowns && mSpell.IgnoreGlobalCooldown)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreGlobalCooldown, string.Empty);
            }

            // Ignore cooldown reduction stat?
            if (mSpell.IgnoreCooldownReduction)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreCooldownReduction, string.Empty);
            }

            // Resize the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupDescription()
        {
            // Add a divider.
            AddDivider();

            // Add the actual description.
            var description = AddDescription();
            description.AddText(Strings.ItemDescription.Description.ToString(mSpell.Description), Color.White);
        }

        protected void SetupCombatInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Vital Damage, if 0 don't display!
            // This bit is a bit iffy.. since in
            var isHeal = false;
            var isDamage = false;
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                if (mSpell.Combat.VitalDiff[i] < 0)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.VitalRecovery[i], Math.Abs(mSpell.Combat.VitalDiff[i]).ToString());
                    isHeal = true;
                }
                else if (mSpell.Combat.VitalDiff[i] > 0)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.VitalDamage[i], mSpell.Combat.VitalDiff[i].ToString());
                    isDamage = true;
                }
            }

            // Damage Type:
            Strings.SpellDescription.DamageTypes.TryGetValue(mSpell.Combat.DamageType, out var damageType);
            rows.AddKeyValueRow(Strings.SpellDescription.DamageType, damageType);

            if (mSpell.Combat.Scaling > 0)
            {
                Strings.SpellDescription.Stats.TryGetValue(mSpell.Combat.ScalingStat, out var stat);
                rows.AddKeyValueRow(Strings.SpellDescription.ScalingStat, stat);
                rows.AddKeyValueRow(Strings.SpellDescription.ScalingPercentage, Strings.SpellDescription.Percentage.ToString(mSpell.Combat.Scaling));
            }

            // Crit Chance
            if (mSpell.Combat.CritChance > 0)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.CritChance, Strings.SpellDescription.Percentage.ToString(mSpell.Combat.CritChance));
                rows.AddKeyValueRow(Strings.SpellDescription.CritMultiplier, Strings.SpellDescription.Multiplier.ToString(mSpell.Combat.CritMultiplier));
            }

            var showDuration = false;
            // Handle Stat Buffs
            var blankAdded = false;
            for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
            {
                Tuple<string, string> data = null;
                if (mSpell.Combat.StatDiff[i] != 0 && mSpell.Combat.PercentageStatDiff[i] != 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], Strings.SpellDescription.RegularAndPercentage.ToString(mSpell.Combat.StatDiff[i], mSpell.Combat.PercentageStatDiff[i]));
                }
                else if (mSpell.Combat.StatDiff[i] != 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], mSpell.Combat.StatDiff[i].ToString());
                }
                else if (mSpell.Combat.PercentageStatDiff[i] != 0)
                {
                    data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], Strings.ItemDescription.Percentage.ToString(mSpell.Combat.PercentageStatDiff[i]));
                }

                // Make sure we only add a blank row the first time we add a stat row.
                if (data != null)
                {
                    if (!blankAdded)
                    {
                        rows.AddKeyValueRow(string.Empty, string.Empty);
                        rows.AddKeyValueRow(Strings.SpellDescription.StatBuff, string.Empty);
                        blankAdded = true;
                    }

                    rows.AddKeyValueRow(data.Item1, data.Item2);
                    showDuration = true;
                }
            }

            // Handle HoT and DoT displays.
            if (mSpell.Combat.HoTDoT)
            {
                showDuration = true;
                rows.AddKeyValueRow(string.Empty, string.Empty);
                if (isHeal)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.HoT, string.Empty);
                }
                else if (isDamage)
                {
                    rows.AddKeyValueRow(Strings.SpellDescription.DoT, string.Empty);
                }
                rows.AddKeyValueRow(Strings.SpellDescription.Tick, TimeSpan.FromMilliseconds(mSpell.Combat.HotDotInterval).WithSuffix());
            }

            // Handle effect display.
            if (mSpell.Combat.Effect != SpellEffect.None)
            {
                showDuration = true;
                rows.AddKeyValueRow(string.Empty, string.Empty);
                rows.AddKeyValueRow(Strings.SpellDescription.Effect, Strings.SpellDescription.Effects[(int)mSpell.Combat.Effect]);
            }

            // Show Stat Buff / Effect / HoT / DoT duration.
            if (showDuration)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.Duration, TimeSpan.FromMilliseconds(mSpell.Combat.Duration).WithSuffix("0.#"));
            }

            // Resize and position the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupDashInfo()
        {
            // Add a divider.
            AddDivider();

            // Add a row component.
            var rows = AddRowContainer();

            // Dash Distance Information.
            rows.AddKeyValueRow(Strings.SpellDescription.Distance, Strings.SpellDescription.Tiles.ToString(mSpell.Combat.CastRange));

            // Ignore map blocks?
            if (mSpell.Dash.IgnoreMapBlocks)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreMapBlock, String.Empty);
            }

            // Ignore resource blocks?
            if (mSpell.Dash.IgnoreActiveResources)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreResourceBlock, String.Empty);
            }

            // Ignore inactive resource blocks?
            if (mSpell.Dash.IgnoreInactiveResources)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreConsumedResourceBlock, String.Empty);
            }

            // Ignore Z-Dimension?
            if (Options.Map.ZDimensionVisible && mSpell.Dash.IgnoreZDimensionAttributes)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.IgnoreZDimension, String.Empty);
            }

            // Resize and position the container.
            rows.SizeToChildren(true, true);
        }

        protected void SetupExtraInfo()
        {
            // Display only if this spell is bound.
            if (mSpell.Bound)
            {
                // Add a divider.
                AddDivider();

                // Add a row component.
                var rows = AddRowContainer();

                // Display shop value.
                rows.AddKeyValueRow(Strings.SpellDescription.Bound, string.Empty);

                // Resize and position the container.
                rows.SizeToChildren(true, true);
            }
        }
    }
}
