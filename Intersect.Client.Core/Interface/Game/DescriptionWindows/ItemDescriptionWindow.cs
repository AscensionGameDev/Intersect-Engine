using Intersect.Enums;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Core;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects.Ranges;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Interface.Game.DescriptionWindows;

public partial class ItemDescriptionWindow() : DescriptionWindowBase(Interface.GameUi.GameCanvas, "DescriptionWindow")
{
    private ItemDescriptor? _itemDescriptor;
    private ItemProperties? _itemProperties;
    private int _amount;
    private string? _valueLabel;

    public void Show(
        ItemDescriptor item,
        int amount,
        ItemProperties? itemProperties = default,
        string valueLabel = ""
    )
    {
        _itemDescriptor = item;
        _amount = amount;
        _itemProperties = itemProperties;
        _valueLabel = valueLabel;

        SetupDescriptionWindow();
        PositionToHoveredControl();

        // If a spell, also display the spell description!
        if (_itemDescriptor.ItemType == ItemType.Spell && _itemDescriptor.SpellId != Guid.Empty)
        {
            if (Canvas == default || InputHandler.HoveredControl is not { } hoveredControl)
            {
                return;
            }

            var spellDesc = Interface.GameUi.SpellDescriptionWindow;
            spellDesc.Show(_itemDescriptor.SpellId);

            // we need to control the spell desc window position here
            var hoveredPos = InputHandler.HoveredControl.ToCanvas(new Point(0, 0));
            var windowX = 0;
            var windowY = Y;

            // if spell desc is out of screen
            if (Y + spellDesc.Height > Canvas.Height)
            {
                windowY = Canvas.Height - spellDesc.Height;
            }

            // let consider some situations
            // item desc is on right side of hovered icon
            if (X >= hoveredPos.X + hoveredControl.Width)
            {
                // lets try to put spell desc on left side of hovered icon
                windowX = hoveredPos.X - spellDesc.Width;

                // ops, our spell desc is out of screen
                if (windowX < 0)
                {
                    windowX = X + Width;
                }
            }
            else
            {
                // lets try to put spell desc on right side of hovered icon
                windowX = hoveredPos.X + hoveredControl.Width;

                // ops, our spell desc is out of screen
                if (windowX + spellDesc.Width > Canvas.Width)
                {
                    windowX = X - spellDesc.Width;
                }
            }

            spellDesc.SetPosition(windowX, windowY);
        }

        base.Show();
    }

    public override void Hide()
    {
        if (Interface.GameUi.ItemDescriptionWindow == this)
        {
            Interface.GameUi.GameCanvas.RemoveChild(Interface.GameUi.ItemDescriptionWindow, true);
            Interface.GameUi.ItemDescriptionWindow = default;
        }

        if (Interface.GameUi.SpellDescriptionWindow != default)
        {
            Interface.GameUi.GameCanvas.RemoveChild(Interface.GameUi.SpellDescriptionWindow, true);
            Interface.GameUi.SpellDescriptionWindow = default;
        }
    }

    protected void SetupDescriptionWindow()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        // Set up our header information.
        SetupHeader();

        // Set up our item limit information.
        SetupItemLimits();

        // if we have a description, set that up.
        if (!string.IsNullOrWhiteSpace(_itemDescriptor.Description))
        {
            SetupDescription();
        }

        // Set up information depending on the item type.
        switch (_itemDescriptor.ItemType)
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
        if (_itemDescriptor == default)
        {
            return;
        }

        // Create our header, but do not load our layout yet since we're adding components manually.
        var header = AddHeader();

        // Set up the icon, if we can load it.
        var tex = GameContentManager.Current.GetTexture(Framework.Content.TextureType.Item, _itemDescriptor.Icon);
        if (tex != null)
        {
            header.SetIcon(tex, _itemDescriptor.Color);
        }

        // Set up the header as the item name.
        CustomColors.Items.Rarities.TryGetValue(_itemDescriptor.Rarity, out var rarityColor);
        header.SetTitle(_itemDescriptor.Name, rarityColor ?? Color.White);

        // Set up the description telling us what type of item this is.
        // if equipment, also list what kind.
        Strings.ItemDescription.ItemTypes.TryGetValue((int)_itemDescriptor.ItemType, out var typeDesc);
        if (_itemDescriptor.ItemType == ItemType.Equipment)
        {
            var equipSlot = Options.Instance.Equipment.Slots[_itemDescriptor.EquipmentSlot];
            var extraInfo = equipSlot;
            if (_itemDescriptor.EquipmentSlot == Options.Instance.Equipment.WeaponSlot && _itemDescriptor.TwoHanded)
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
            if (Options.Instance.Items.TryGetRarityName(_itemDescriptor.Rarity, out var rarityName))
            {
                _ = Strings.ItemDescription.Rarity.TryGetValue(rarityName, out var rarityLabel);
                header.SetDescription(rarityLabel, rarityColor ?? Color.White);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error setting rarity description for rarity {Rarity}",
                _itemDescriptor.Rarity
            );
            throw;
        }

        header.SizeToChildren(true, false);
    }

    protected void SetupItemLimits()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        // Gather up what limitations apply to this item.
        var limits = new List<string>();
        if (!_itemDescriptor.CanBank)
        {
            limits.Add(Strings.ItemDescription.Banked);
        }
        if (!_itemDescriptor.CanGuildBank)
        {
            limits.Add(Strings.ItemDescription.GuildBanked);
        }
        if (!_itemDescriptor.CanBag)
        {
            limits.Add(Strings.ItemDescription.Bagged);
        }
        if (!_itemDescriptor.CanTrade)
        {
            limits.Add(Strings.ItemDescription.Traded);
        }
        if (!_itemDescriptor.CanDrop)
        {
            limits.Add(Strings.ItemDescription.Dropped);
        }
        if (!_itemDescriptor.CanSell)
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
        if (_itemDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add the actual description.
        var description = AddDescription();
        description.AddText(Strings.ItemDescription.Description.ToString(_itemDescriptor.Description), Color.White);
    }

    protected void SetupEquipmentInfo()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        if (Globals.Me is not { } player)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a row component.
        var rows = AddRowContainer();

        // Is this a weapon?
        if (_itemDescriptor.EquipmentSlot == Options.Instance.Equipment.WeaponSlot)
        {
            // Base Damage:
            rows.AddKeyValueRow(Strings.ItemDescription.BaseDamage, _itemDescriptor.Damage.ToString());

            // Damage Type:
            Strings.ItemDescription.DamageTypes.TryGetValue(_itemDescriptor.DamageType, out var damageType);
            rows.AddKeyValueRow(Strings.ItemDescription.BaseDamageType, damageType);

            if (_itemDescriptor.Scaling > 0)
            {
                Strings.ItemDescription.Stats.TryGetValue(_itemDescriptor.ScalingStat, out var stat);
                rows.AddKeyValueRow(Strings.ItemDescription.ScalingStat, stat);
                rows.AddKeyValueRow(Strings.ItemDescription.ScalingPercentage, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.Scaling));
            }

            // Crit Chance
            if (_itemDescriptor.CritChance > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.CritChance, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.CritChance));
                rows.AddKeyValueRow(Strings.ItemDescription.CritMultiplier, Strings.ItemDescription.Multiplier.ToString(_itemDescriptor.CritMultiplier));
            }

            // Attack Speed
            // Are we supposed to change our attack time based on a modifier?
            if (_itemDescriptor.AttackSpeedModifier == 0)
            {
                // No modifier, assuming base attack rate? We have to calculate the speed stat manually here though..!
                var speed = player.Stat[(int)Stat.Speed];

                // Remove currently equipped weapon stats.. We want to create a fair display!
                var weaponSlot = player.MyEquipment[Options.Instance.Equipment.WeaponSlot];
                if (weaponSlot != -1)
                {
                    var randomStats = player.Inventory[weaponSlot].ItemProperties.StatModifiers;
                    var weapon = ItemDescriptor.Get(player.Inventory[weaponSlot].ItemId);
                    if (weapon != null && randomStats != null)
                    {
                        speed = (int)Math.Round(speed / ((100 + weapon.PercentageStatsGiven[(int)Stat.Speed]) / 100f));
                        speed -= weapon.StatsGiven[(int)Stat.Speed];
                        speed -= randomStats[(int)Stat.Speed];
                    }
                }

                // Add current item's speed stats!
                if (_itemProperties?.StatModifiers != default)
                {
                    speed += _itemDescriptor.StatsGiven[(int)Stat.Speed];
                    speed += _itemProperties.StatModifiers[(int)Stat.Speed];
                    speed += (int)Math.Floor(speed * (_itemDescriptor.PercentageStatsGiven[(int)Stat.Speed] / 100f));
                }

                // Display the actual speed this weapon would have based off of our calculated speed stat.
                rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, TimeSpan.FromMilliseconds(player.CalculateAttackTime(speed)).WithSuffix());
            }
            else if (_itemDescriptor.AttackSpeedModifier == 1)
            {
                // Static, so this weapon's attack speed.
                rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, TimeSpan.FromMilliseconds(_itemDescriptor.AttackSpeedValue).WithSuffix());
            }
            else if (_itemDescriptor.AttackSpeedModifier == 2)
            {
                // Percentage based.
                rows.AddKeyValueRow(Strings.ItemDescription.AttackSpeed, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.AttackSpeedValue));
            }
        }

        //Blocking options
        if (_itemDescriptor.EquipmentSlot == Options.Instance.Equipment.ShieldSlot)
        {
            if (_itemDescriptor.BlockChance > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.BlockChance, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.BlockChance));
            }

            if (_itemDescriptor.BlockAmount > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.BlockAmount, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.BlockAmount));
            }

            if (_itemDescriptor.BlockAbsorption > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.BlockAbsorption, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.BlockAbsorption));
            }
        }

        // Vitals
        for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
        {
            if (_itemDescriptor.VitalsGiven[i] != 0 && _itemDescriptor.PercentageVitalsGiven[i] != 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.RegularAndPercentage.ToString(_itemDescriptor.VitalsGiven[i], _itemDescriptor.PercentageVitalsGiven[i]));
            }
            else if (_itemDescriptor.VitalsGiven[i] != 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], _itemDescriptor.VitalsGiven[i].ToString());
            }
            else if (_itemDescriptor.PercentageVitalsGiven[i] != 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.Vitals[i], Strings.ItemDescription.Percentage.ToString(_itemDescriptor.PercentageVitalsGiven[i]));
            }
        }

        // Vitals Regen
        for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
        {
            if (_itemDescriptor.VitalsRegen[i] != 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.VitalsRegen[i], Strings.ItemDescription.Percentage.ToString(_itemDescriptor.VitalsRegen[i]));
            }
        }

        // Stats
        var statModifiers = _itemProperties?.StatModifiers;
        for (var statIndex = 0; statIndex < Enum.GetValues<Stat>().Length; statIndex++)
        {
            var stat = (Stat)statIndex;
            // Do we have item properties, if so this is a finished item. Otherwise does this item not have growing stats?
            var statLabel = Strings.ItemDescription.StatCounts[statIndex];
            ItemRange? rangeForStat = default;
            var percentageGivenForStat = _itemDescriptor.PercentageStatsGiven[statIndex];
            if (statModifiers != default || !_itemDescriptor.TryGetRangeFor(stat, out rangeForStat) || rangeForStat.LowRange == rangeForStat.HighRange)
            {
                var flatValueGivenForStat = _itemDescriptor.StatsGiven[statIndex];
                if (statModifiers != default)
                {
                    flatValueGivenForStat += statModifiers[statIndex];
                }

                // If the range is something like 1 to 1 then it should just be added into the flat stat
                flatValueGivenForStat += rangeForStat?.LowRange ?? 0;

                if (flatValueGivenForStat != 0 && percentageGivenForStat != 0)
                {
                    rows.AddKeyValueRow(
                        statLabel,
                        Strings.ItemDescription.RegularAndPercentage.ToString(flatValueGivenForStat, percentageGivenForStat)
                    );
                }
                else if (flatValueGivenForStat != 0)
                {
                    rows.AddKeyValueRow(statLabel, flatValueGivenForStat.ToString());
                }
                else if (percentageGivenForStat != 0)
                {
                    rows.AddKeyValueRow(
                        statLabel,
                        Strings.ItemDescription.Percentage.ToString(percentageGivenForStat)
                    );
                }
            }
            // We do not have item properties and have growing stats! So don't display a finished stat but a range instead.
            else if (_itemDescriptor.TryGetRangeFor(stat, out var range))
            {
                var statGiven = _itemDescriptor.StatsGiven[statIndex];
                var percentageStatGiven = percentageGivenForStat;
                var statLow = statGiven + range.LowRange;
                var statHigh = statGiven + range.HighRange;

                var statMessage = Strings.ItemDescription.StatGrowthRange.ToString(statLow, statHigh);

                if (percentageStatGiven != 0)
                {
                    statMessage = Strings.ItemDescription.RegularAndPercentage.ToString(
                        statMessage,
                        percentageStatGiven
                    );
                }

                rows.AddKeyValueRow(statLabel, statMessage);
            }
        }

        // Bonus Effect
        foreach (var effect in _itemDescriptor.Effects)
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
        if (_itemDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a row component.
        var rows = AddRowContainer();

        // Consumable data.
        if (_itemDescriptor.Consumable != null)
        {
            if (_itemDescriptor.Consumable.Value > 0 && _itemDescriptor.Consumable.Percentage > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)_itemDescriptor.Consumable.Type], Strings.ItemDescription.RegularAndPercentage.ToString(_itemDescriptor.Consumable.Value, _itemDescriptor.Consumable.Percentage));
            }
            else if (_itemDescriptor.Consumable.Value > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)_itemDescriptor.Consumable.Type], _itemDescriptor.Consumable.Value.ToString());
            }
            else if (_itemDescriptor.Consumable.Percentage > 0)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.ConsumableTypes[(int)_itemDescriptor.Consumable.Type], Strings.ItemDescription.Percentage.ToString(_itemDescriptor.Consumable.Percentage));
            }
        }

        // Resize and position the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupSpellInfo()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a row component.
        var rows = AddRowContainer();

        // Spell data.
        if (_itemDescriptor.Spell != null)
        {
            if (_itemDescriptor.QuickCast)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.CastSpell.ToString(_itemDescriptor.Spell.Name), string.Empty);
            }
            else
            {
                rows.AddKeyValueRow(Strings.ItemDescription.TeachSpell.ToString(_itemDescriptor.Spell.Name), string.Empty);
            }

            if (_itemDescriptor.SingleUse)
            {
                rows.AddKeyValueRow(Strings.ItemDescription.SingleUse, string.Empty);
            }
        }

        // Resize and position the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupBagInfo()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        // Add a divider.
        AddDivider();

        // Add a row component.
        var rows = AddRowContainer();

        // Bag data.
        rows.AddKeyValueRow(Strings.ItemDescription.BagSlots, _itemDescriptor.SlotCount.ToString());

        // Resize and position the container.
        rows.SizeToChildren(true, true);
    }

    protected void SetupExtraInfo()
    {
        if (_itemDescriptor == default)
        {
            return;
        }

        // Our list of data to add, should we need to.
        var data = new List<Tuple<string, string>>();

        // Display our amount, but only if we are stackable and have more than one.
        if (_itemDescriptor.IsStackable && _amount > 1)
        {
            data.Add(new Tuple<string, string>(Strings.ItemDescription.Amount, _amount.ToString("N0").Replace(",", Strings.Numbers.Comma)));
        }

        // Display item drop chance if configured.
        if (_itemDescriptor.DropChanceOnDeath > 0)
        {
            data.Add(new Tuple<string, string>(Strings.ItemDescription.DropOnDeath, Strings.ItemDescription.Percentage.ToString(_itemDescriptor.DropChanceOnDeath)));
        }

        // Display shop value if we have one.
        if (!string.IsNullOrWhiteSpace(_valueLabel))
        {
            data.Add(new Tuple<string, string>(_valueLabel, string.Empty));
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
}
