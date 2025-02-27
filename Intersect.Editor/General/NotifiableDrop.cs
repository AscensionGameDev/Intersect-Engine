using System.ComponentModel;
using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Editor.General;

public partial class NotifiableDrop: INotifyPropertyChanged
{
    private Guid _itemId;

    private int _minQuantity;

    private int _maxQuantity;

    private double _chance;

    public string DisplayName
    {
        get
        {
            return _itemId == default
            ? TextUtils.None
            : Strings.NpcEditor.dropdisplay.ToString(
                    ItemDescriptor.GetName(_itemId),
                    MinQuantity,
                    MaxQuantity,
                    _chance
                );
        }
    }

    public Guid ItemId
    {
        get => _itemId;
        set
        {
            if (_itemId == value) return;
            _itemId = value;
            OnPropertyChanged(nameof(ItemId));
        }
    }

    public int MinQuantity
    {
        get => _minQuantity;
        set
        {
            if (_minQuantity == value) return;
            _minQuantity = value;
            OnPropertyChanged(nameof(MinQuantity));
        }
    }

    public int MaxQuantity
    {
        get => Math.Max(_maxQuantity, _minQuantity);
        set
        {
            if (_maxQuantity == value) return;
            _maxQuantity = value;
            OnPropertyChanged(nameof(MaxQuantity));
        }
    }

    public double Chance
    {
        get => _chance;
        set
        {
            if (_chance == value) return;
            _chance = value;
            OnPropertyChanged(nameof(Chance));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
