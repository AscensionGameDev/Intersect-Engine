using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Client.Localization;
using Intersect.Utilities;
using Intersect.Client.Framework.File_Management;

namespace Intersect.Client.Interface.Game.DescriptionWindows;

public partial class SpellDescriptionWindow() : DescriptionWindowBase(Interface.GameUi.GameCanvas, "DescriptionWindow")
{
    private SpellDescriptor? _spellDescriptor;

    public void Show(Guid spellId, ItemDescriptionWindow? itemDecriptionContainer = default)
    {
        _spellDescriptor = SpellDescriptor.Get(spellId);
        SetupDescriptionWindow();

        if (itemDecriptionContainer != default)
        {
            SetPosition(itemDecriptionContainer.X, itemDecriptionContainer.Y + itemDecriptionContainer.Height);
            SetSize(itemDecriptionContainer.Width, itemDecriptionContainer.Height);
        }
        else
        {
            PositionToHoveredControl();
        }

        base.Show();
    }

    protected void SetupDescriptionWindow()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

        // Set up our header information.
        SetupHeader();

        // Set up our basic spell info.
        SetupSpellInfo();

        // if we have a description, set that up.
        if (!string.IsNullOrWhiteSpace(_spellDescriptor.Description))
        {
            SetupDescription();
        }

        // Set up information depending on the item type.
        switch (_spellDescriptor.SpellType)
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
        if (_spellDescriptor == default)
        {
            return;
        }

        // Create our header, but do not load our layout yet since we're adding components manually.
        var header = AddHeader();

        // Set up the icon, if we can load it.
        var tex = GameContentManager.Current.GetTexture(Framework.Content.TextureType.Spell, _spellDescriptor.Icon);
        if (tex != null)
        {
            header.SetIcon(tex, Color.White);
        }

        // Set up the header as the item name.
        header.SetTitle(_spellDescriptor.Name, Color.White);

        // Set up the spell type description.
        Strings.SpellDescription.SpellTypes.TryGetValue((int)_spellDescriptor.SpellType, out var spellType);
        header.SetSubtitle(spellType, Color.White);

        // Set up the spelldescription based on what kind of spell it is.
        if (_spellDescriptor.SpellType == (int)SpellType.CombatSpell)
        {
            if (_spellDescriptor.Combat.TargetType == SpellTargetType.Projectile)
            {
                var proj = ProjectileDescriptor.Get(_spellDescriptor.Combat.ProjectileId);
                header.SetDescription(Strings.SpellDescription.TargetTypes[(int)_spellDescriptor.Combat.TargetType].ToString(proj?.Range ?? 0, _spellDescriptor.Combat.HitRadius), Color.White);
            }
            else
            {
                header.SetDescription(Strings.SpellDescription.TargetTypes[(int)_spellDescriptor.Combat.TargetType].ToString(_spellDescriptor.Combat.CastRange, _spellDescriptor.Combat.HitRadius), Color.White);
            }
        }

        header.SizeToChildren(true, false);
    }

    protected void SetupSpellInfo()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a new row control to add our details into.
        var rows = AddRowContainer();

        // Friendly / Non Friendly for combat spells.
        if (_spellDescriptor.SpellType == SpellType.CombatSpell || _spellDescriptor.SpellType == SpellType.WarpTo)
        {
            if (_spellDescriptor.Combat.Friendly)
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
        if (_spellDescriptor.CastDuration > 0)
        {
            castTime = TimeSpan.FromMilliseconds(_spellDescriptor.CastDuration).WithSuffix();
        }
        rows.AddKeyValueRow(Strings.SpellDescription.CastTime, castTime);

        // Add Vital Costs
        for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
        {
            if (_spellDescriptor.VitalCost[i] != 0)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.VitalCosts[i], _spellDescriptor.VitalCost[i].ToString());
            }
        }

        // Add Cooldown time
        if (_spellDescriptor.CooldownDuration > 0)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.Cooldown, TimeSpan.FromMilliseconds(_spellDescriptor.CooldownDuration).WithSuffix());
        }

        // Add Cooldown Group
        if (!string.IsNullOrWhiteSpace(_spellDescriptor.CooldownGroup))
        {
            rows.AddKeyValueRow(Strings.SpellDescription.CooldownGroup, _spellDescriptor.CooldownGroup);
        }

        // Ignores global cooldown if enabled?
        if (Options.Instance.Combat.EnableGlobalCooldowns && _spellDescriptor.IgnoreGlobalCooldown)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreGlobalCooldown, string.Empty);
        }

        // Ignore cooldown reduction stat?
        if (_spellDescriptor.IgnoreCooldownReduction)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreCooldownReduction, string.Empty);
        }

        // Resize the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupDescription()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add the actual description.
        var description = AddDescription();
        description.AddText(Strings.ItemDescription.Description.ToString(_spellDescriptor.Description), Color.White);
    }

    protected void SetupCombatInfo()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

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
            if (_spellDescriptor.Combat.VitalDiff[i] < 0)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.VitalRecovery[i], Math.Abs(_spellDescriptor.Combat.VitalDiff[i]).ToString());
                isHeal = true;
            }
            else if (_spellDescriptor.Combat.VitalDiff[i] > 0)
            {
                rows.AddKeyValueRow(Strings.SpellDescription.VitalDamage[i], _spellDescriptor.Combat.VitalDiff[i].ToString());
                isDamage = true;
            }
        }

        // Damage Type:
        Strings.SpellDescription.DamageTypes.TryGetValue(_spellDescriptor.Combat.DamageType, out var damageType);
        rows.AddKeyValueRow(Strings.SpellDescription.DamageType, damageType);

        if (_spellDescriptor.Combat.Scaling > 0)
        {
            Strings.SpellDescription.Stats.TryGetValue(_spellDescriptor.Combat.ScalingStat, out var stat);
            rows.AddKeyValueRow(Strings.SpellDescription.ScalingStat, stat);
            rows.AddKeyValueRow(Strings.SpellDescription.ScalingPercentage, Strings.SpellDescription.Percentage.ToString(_spellDescriptor.Combat.Scaling));
        }

        // Crit Chance
        if (_spellDescriptor.Combat.CritChance > 0)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.CritChance, Strings.SpellDescription.Percentage.ToString(_spellDescriptor.Combat.CritChance));
            rows.AddKeyValueRow(Strings.SpellDescription.CritMultiplier, Strings.SpellDescription.Multiplier.ToString(_spellDescriptor.Combat.CritMultiplier));
        }

        var showDuration = false;
        // Handle Stat Buffs
        var blankAdded = false;
        for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
        {
            Tuple<string, string> data = null;
            if (_spellDescriptor.Combat.StatDiff[i] != 0 && _spellDescriptor.Combat.PercentageStatDiff[i] != 0)
            {
                data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], Strings.SpellDescription.RegularAndPercentage.ToString(_spellDescriptor.Combat.StatDiff[i], _spellDescriptor.Combat.PercentageStatDiff[i]));
            }
            else if (_spellDescriptor.Combat.StatDiff[i] != 0)
            {
                data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], _spellDescriptor.Combat.StatDiff[i].ToString());
            }
            else if (_spellDescriptor.Combat.PercentageStatDiff[i] != 0)
            {
                data = new Tuple<string, string>(Strings.SpellDescription.StatCounts[i], Strings.ItemDescription.Percentage.ToString(_spellDescriptor.Combat.PercentageStatDiff[i]));
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
        if (_spellDescriptor.Combat.HoTDoT)
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
            rows.AddKeyValueRow(Strings.SpellDescription.Tick, TimeSpan.FromMilliseconds(_spellDescriptor.Combat.HotDotInterval).WithSuffix());
        }

        // Handle effect display.
        if (_spellDescriptor.Combat.Effect != SpellEffect.None)
        {
            showDuration = true;
            rows.AddKeyValueRow(string.Empty, string.Empty);
            rows.AddKeyValueRow(Strings.SpellDescription.Effect, Strings.SpellDescription.Effects[(int)_spellDescriptor.Combat.Effect]);
        }

        // Show Stat Buff / Effect / HoT / DoT duration.
        if (showDuration)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.Duration, TimeSpan.FromMilliseconds(_spellDescriptor.Combat.Duration).WithSuffix("0.#"));
        }

        // Resize and position the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupDashInfo()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a row component.
        var rows = AddRowContainer();

        // Dash Distance Information.
        rows.AddKeyValueRow(Strings.SpellDescription.Distance, Strings.SpellDescription.Tiles.ToString(_spellDescriptor.Combat.CastRange));

        // Ignore map blocks?
        if (_spellDescriptor.Dash.IgnoreMapBlocks)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreMapBlock, string.Empty);
        }

        // Ignore resource blocks?
        if (_spellDescriptor.Dash.IgnoreActiveResources)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreResourceBlock, string.Empty);
        }

        // Ignore inactive resource blocks?
        if (_spellDescriptor.Dash.IgnoreInactiveResources)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreConsumedResourceBlock, string.Empty);
        }

        // Ignore Z-Dimension?
        if (Options.Instance.Map.ZDimensionVisible && _spellDescriptor.Dash.IgnoreZDimensionAttributes)
        {
            rows.AddKeyValueRow(Strings.SpellDescription.IgnoreZDimension, string.Empty);
        }

        // Resize and position the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupExtraInfo()
    {
        if (_spellDescriptor == default)
        {
            return;
        }

        // Display only if this spell is bound.
        if (_spellDescriptor.Bound)
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
