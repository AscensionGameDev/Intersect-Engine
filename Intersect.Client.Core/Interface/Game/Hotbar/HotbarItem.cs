using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Spells;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Hotbar;

public partial class HotbarItem : SlotItem
{
    private const int ItemXPadding = 4;
    private const int ItemYPadding = 4;

    private readonly Label _cooldownLabel;
    private readonly Label _equipLabel;
    private readonly Label _keyLabel;

    private bool _canDrag;
    private long _clickTime;
    private Guid _currentId = Guid.Empty;
    private ItemDescriptor? _currentItem = null;
    private SpellDescriptor? _currentSpell = null;
    private Draggable _dragIcon;
    private bool _isDragging;
    private bool _isEquipped;
    private bool _isFaded;
    private readonly Base _hotbarWindow;
    private ControlBinding? _hotKey;
    private Item? _inventoryItem = null;
    private int _inventoryItemIndex = -1;
    private ItemDescriptionWindow? _itemDescWindow;
    private bool _mouseOver;
    private int _mouseX = -1;
    private int _mouseY = -1;
    private Label _quantityLabel;
    private Spell? _spellBookItem = null;
    private SpellDescriptionWindow? _spellDescWindow;
    private bool _textureLoaded;

    public HotbarItem(int hotbarSlotIndex, Base hotbarWindow)
        : base(hotbarWindow, $"HotbarContainer{hotbarSlotIndex}", hotbarSlotIndex, null)
    {
        _hotbarWindow = hotbarWindow;

        var column = hotbarSlotIndex % 10;
        var row = hotbarSlotIndex / 10;

        X = 4 + column * 40;
        Y = 4 + row * 40;
        Width = 36;
        Height = 36;
        Margin = new Margin(column > 0 ? 4 : 0, row > 0 ? 4 : 0, 0, 0);
        RestrictToParent = true;
        TextureFilename = "hotbaritem.png";

        // _iconImage is layered on top of the container (shows the Item or Spell Icon).
        _iconImage.Name = $"{nameof(HotbarItem)}{SlotIndex}";
        _iconImage.SetPosition(1, 1);
        _iconImage.HoverEnter += _iconImage_HoverEnter;
        _iconImage.HoverLeave += _iconImage_HoverLeave;
        _iconImage.Clicked += _iconImage_Clicked;

        var font = GameContentManager.Current.GetFont("sourcesansproblack");

        _equipLabel = new Label(this, $"EquipLabel{hotbarSlotIndex}")
        {
            Alignment = [Alignments.Top, Alignments.Left],
            X = 26,
            Y = 0,
            Width = 10,
            Height = 11,
            BackgroundTemplateName = "equipped.png",
            Font = font,
            FontSize = 8,
            IsHidden = true,
            Padding = Padding.FourH,
            Text = Strings.Inventory.EquippedSymbol,
            TextColorOverride = Color.White,
        };

        _quantityLabel = new Label(this, $"QuantityLabel{hotbarSlotIndex}")
        {
            Alignment = [Alignments.Top, Alignments.Right],
            X = 32,
            Y = 0,
            Width = 4,
            Height = 11,
            BackgroundTemplateName = "quantity.png",
            Font = font,
            FontSize = 8,
            IsHidden = true,
            Padding = Padding.FourH,
            TextColorOverride = Color.White,
        };

        _cooldownLabel = new Label(this, $"CooldownLabel{hotbarSlotIndex}")
        {
            Alignment = [Alignments.Center],
            TextAlign = Pos.Center,
            X = 16,
            Y = 12,
            Width = 4,
            Height = 11,
            BackgroundTemplateName = "quantity.png",
            Font = font,
            FontSize = 8,
            IsHidden = true,
            Padding = Padding.FourH,
            TextColorOverride = Color.White,
        };

        _keyLabel = new Label(this, $"KeyLabel{hotbarSlotIndex}")
        {
            Alignment = [Alignments.Bottom, Alignments.Right],
            X = 31,
            Y = 25,
            Width = 5,
            Height = 11,
            BackgroundTemplateName = "hotbar_label.png",
            Font = font,
            FontSize = 8,
            Padding = Padding.FourH,
            TextColorOverride = Color.White,
        };
    }

    public void Activate()
    {
        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Right))
        {
            return;
        }

        if (_currentId != Guid.Empty && Globals.Me != null)
        {
            if (_currentItem != null)
            {
                if (_inventoryItemIndex > -1)
                {
                    Globals.Me.TryUseItem(_inventoryItemIndex);
                }
            }
            else if (_currentSpell != null)
            {
                Globals.Me.TryUseSpell(_currentSpell.Id);
            }
        }
    }

    private void _iconImage_Clicked(Base sender, MouseButtonState arguments)
    {
        switch (arguments.MouseButton)
        {
            case MouseButton.Left:
                _clickTime = Timing.Global.MillisecondsUtc + 500;
                break;

            case MouseButton.Right:
                Globals.Me?.AddToHotbar(SlotIndex, -1, -1);
                break;
        }
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _mouseOver = false;
        _mouseX = -1;
        _mouseY = -1;
        if (_itemDescWindow != null)
        {
            _itemDescWindow.Dispose();
            _itemDescWindow = null;
        }

        if (_spellDescWindow != null)
        {
            _spellDescWindow.Dispose();
            _spellDescWindow = null;
        }
    }

    private void _iconImage_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null || Globals.Me == null)
        {
            return;
        }

        _mouseOver = true;
        _canDrag = true;
        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            _canDrag = false;

            return;
        }

        if (_currentItem != null && _inventoryItem != null)
        {
            if (_itemDescWindow != null)
            {
                _itemDescWindow.Dispose();
                _itemDescWindow = null;
            }

            var quantityOfItem = 1;

            if (_currentItem.IsStackable)
            {
                quantityOfItem = Globals.Me.GetQuantityOfItemInInventory(_currentItem.Id);
            }

            _itemDescWindow = new ItemDescriptionWindow(
                _currentItem, quantityOfItem, _hotbarWindow.X + (_hotbarWindow.Width / 2), _hotbarWindow.Y + _hotbarWindow.Height + 2,
                _inventoryItem.ItemProperties, _currentItem.Name, ""
            );
        }
        else if (_currentSpell != null)
        {
            if (_spellDescWindow != null)
            {
                _spellDescWindow.Dispose();
                _spellDescWindow = null;
            }

            _spellDescWindow = new SpellDescriptionWindow(
                _currentSpell.Id, _hotbarWindow.X + (_hotbarWindow.Width / 2), _hotbarWindow.Y + _hotbarWindow.Height + 2
            );
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = ToCanvas(new Point(0, 0)).X,
            Y = ToCanvas(new Point(0, 0)).Y,
            Width = Width,
            Height = Height
        };

        return rect;
    }

    public void Update()
    {
        if (Globals.Me == null || Controls.ActiveControls == null)
        {
            return;
        }

        // Check if the label should be changed
        var controlValue = Control.HotkeyOffset + SlotIndex + 1;
        ControlBinding? binding = null;
        if (Controls.ActiveControls.TryGetMappingFor(controlValue, out var mapping))
        {
            binding = mapping.Bindings.FirstOrDefault();
        }

        if (_hotKey == null || _hotKey.Modifier != (binding?.Modifier ?? Keys.None) || _hotKey.Key != (binding?.Key ?? Keys.None))
        {
            if (binding?.Key is null or Keys.None)
            {
                _keyLabel.IsVisibleInTree = false;
            }
            else
            {
                var keyName = binding.Key.GetKeyId(isModifier: false).ToLowerInvariant();
                if (!Strings.Keys.KeyDictionary.TryGetValue(keyName, out var localizedKeyString))
                {
                    localizedKeyString = keyName;
                }

                string assembledKeyText = localizedKeyString;

                var modifier = binding.Modifier;
                if (modifier is not Keys.None)
                {
                    var modifierName = modifier.GetKeyId(isModifier: true).ToLowerInvariant();
                    string modifierText = Strings.Keys.KeyDictionary.TryGetValue(modifierName, out var localizedModifierString)
                        ? localizedModifierString
                        : modifierName;
                    assembledKeyText = Strings.Keys.KeyNameWithModifier.ToString(modifierText, assembledKeyText);
                }

                _keyLabel.Text = assembledKeyText;
                _keyLabel.IsVisibleInTree = true;
            }

            _hotKey = binding == null ? null : new ControlBinding(binding);
        }

        var slot = Globals.Me.Hotbar[SlotIndex];
        var updateDisplay = _currentId != slot.ItemOrSpellId || _textureLoaded == false; // Update display if item changes or we dont have a texture for it.

        if (_currentId != slot.ItemOrSpellId)
        {
            _currentItem = null;
            _currentSpell = null;
            var itm = ItemDescriptor.Get(slot.ItemOrSpellId);
            var spl = SpellDescriptor.Get(slot.ItemOrSpellId);
            if (itm != null)
            {
                _currentItem = itm;
            }

            if (spl != null)
            {
                _currentSpell = spl;
            }

            _currentId = slot.ItemOrSpellId;
        }

        _spellBookItem = null;
        _inventoryItem = null;
        _inventoryItemIndex = -1;

        if (_currentItem != null)
        {
            var itmIndex = Globals.Me.FindHotbarItem(slot);
            if (itmIndex > -1)
            {
                _inventoryItemIndex = itmIndex;
                _inventoryItem = (Item)Globals.Me.Inventory[itmIndex];
            }
        }
        else if (_currentSpell != null)
        {
            var splIndex = Globals.Me.FindHotbarSpell(slot);
            if (splIndex > -1)
            {
                _spellBookItem = Globals.Me.Spells[splIndex] as Spell;
            }
        }

        if (_currentItem != null) //When it's an item
        {
            //We don't have it, and the icon isn't faded
            if (_inventoryItem == null && !_isFaded)
            {
                updateDisplay = true;
            }

            //We have it, and the equip icon doesn't match equipped status
            if (_inventoryItem != null && Globals.Me.IsEquipped(_inventoryItemIndex) != _isEquipped)
            {
                updateDisplay = true;
            }

            //We have it, and it's on cd
            if (_inventoryItem != null && Globals.Me.IsItemOnCooldown(_inventoryItemIndex))
            {
                updateDisplay = true;
            }

            //We have it, and it's on cd, and the fade is incorrect
            if (_inventoryItem != null && Globals.Me.IsItemOnCooldown(_inventoryItemIndex) != _isFaded)
            {
                updateDisplay = true;
            }

            //We have it, and the quantity label is incorrect
            var quantityText = Strings.FormatQuantityAbbreviated(Globals.Me.GetQuantityOfItemInInventory(_currentItem.Id));
            if (_inventoryItem != null && _quantityLabel.Text != quantityText)
            {
                _quantityLabel.Text = quantityText;
                updateDisplay = true;
            }
        }

        if (_currentSpell != null) //When it's a spell
        {
            //We don't know it, remove from hotbar right away!
            if (_spellBookItem == null)
            {
                Globals.Me.AddToHotbar(SlotIndex, -1, -1);
                updateDisplay = true;
            }

            //Spell on cd
            if (_spellBookItem != null &&
                Globals.Me.GetSpellCooldown(_spellBookItem.Id) > Timing.Global.Milliseconds)
            {
                updateDisplay = true;
            }

            //Spell on cd and the fade is incorrect
            if (_spellBookItem != null &&
                Globals.Me.GetSpellCooldown(_spellBookItem.Id) > Timing.Global.Milliseconds != _isFaded)
            {
                updateDisplay = true;
            }
        }

        if (updateDisplay) //Item on cd and fade is incorrect
        {
            if (_currentItem != null)
            {
                _iconImage.Show();
                _iconImage.Texture = Globals.ContentManager.GetTexture(
                    Framework.Content.TextureType.Item, _currentItem.Icon
                );

                _equipLabel.IsHidden = true;
                _quantityLabel.IsHidden = true;
                _cooldownLabel.IsHidden = true;

                if (_inventoryItemIndex > -1)
                {
                    _isFaded = Globals.Me.IsItemOnCooldown(_inventoryItemIndex);
                    _isEquipped = Globals.Me.IsEquipped(_inventoryItemIndex);

                    if (_isFaded)
                    {
                        _cooldownLabel.IsHidden = false;
                        _cooldownLabel.Text = TimeSpan
                            .FromMilliseconds(Globals.Me.GetItemRemainingCooldown(_inventoryItemIndex))
                            .WithSuffix();
                    }
                }
                else
                {
                    _equipLabel.IsHidden = true;
                    _quantityLabel.IsHidden = true;
                    _cooldownLabel.IsHidden = true;
                    _isEquipped = false;
                    _isFaded = true;
                }

                _equipLabel.IsHidden = !_isEquipped || _inventoryItemIndex < 0;
                _quantityLabel.IsHidden = !_currentItem.Stackable || _inventoryItemIndex < 0;
                _cooldownLabel.IsHidden = !_isFaded || _inventoryItemIndex < 0;

                _textureLoaded = true;
            }
            else if (_currentSpell != null)
            {
                _iconImage.Show();
                _iconImage.Texture = Globals.ContentManager.GetTexture(
                    Framework.Content.TextureType.Spell, _currentSpell.Icon
                );

                _equipLabel.IsHidden = true;
                _quantityLabel.IsHidden = true;
                _cooldownLabel.IsHidden = true;
                if (_spellBookItem != null)
                {
                    var spellSlot = Globals.Me.FindHotbarSpell(slot);
                    _isFaded = Globals.Me.IsSpellOnCooldown(spellSlot);
                    if (_isFaded)
                    {
                        _cooldownLabel.IsHidden = false;
                        var remaining = Globals.Me.GetSpellRemainingCooldown(spellSlot);
                        _cooldownLabel.Text = TimeSpan.FromMilliseconds(remaining).WithSuffix("0.0");
                    }
                }
                else
                {
                    _isFaded = true;
                }

                _textureLoaded = true;
                _isEquipped = false;
            }
            else
            {
                _iconImage.Hide();
                _textureLoaded = true;
                _isEquipped = false;
                _equipLabel.IsHidden = true;
                _quantityLabel.IsHidden = true;
                _cooldownLabel.IsHidden = true;
            }

            if (_isFaded)
            {
                if (_currentSpell != null)
                {
                    _iconImage.RenderColor = new Color(60, 255, 255, 255);
                }

                if (_currentItem != null)
                {
                    _iconImage.RenderColor = new Color(60, _currentItem.Color.R, _currentItem.Color.G, _currentItem.Color.B);
                }
            }
            else
            {
                if (_currentSpell != null)
                {
                    _iconImage.RenderColor = Color.White;
                }

                if (_currentItem != null)
                {
                    _iconImage.RenderColor = _currentItem.Color;
                }
            }
        }

        if (_currentItem != null || _currentSpell != null)
        {
            if (!_isDragging)
            {
                _iconImage.IsHidden = false;

                var equipLabelIsHidden = _currentItem == null || !Globals.Me.IsEquipped(_inventoryItemIndex) || _inventoryItemIndex < 0;
                _equipLabel.IsHidden = equipLabelIsHidden;

                var quantityLabelIsHidden = _currentItem is not { Stackable: true } || _inventoryItemIndex < 0;
                _quantityLabel.IsHidden = quantityLabelIsHidden;

                if (_mouseOver)
                {
                    if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
                    {
                        _canDrag = true;
                        _mouseX = -1;
                        _mouseY = -1;
                        if (Timing.Global.MillisecondsUtc < _clickTime)
                        {
                            Activate();
                            _clickTime = 0;
                        }
                    }
                    else
                    {
                        if (_canDrag && Draggable.Active == null)
                        {
                            if (_mouseX == -1 || _mouseY == -1)
                            {
                                _mouseX = InputHandler.MousePosition.X - ToCanvas(new Point(0, 0)).X;
                                _mouseY = InputHandler.MousePosition.Y - ToCanvas(new Point(0, 0)).Y;
                            }
                            else
                            {
                                var xdiff = _mouseX -
                                            (InputHandler.MousePosition.X -
                                             ToCanvas(new Point(0, 0)).X);

                                var ydiff = _mouseY -
                                            (InputHandler.MousePosition.Y -
                                             ToCanvas(new Point(0, 0)).Y);

                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    _isDragging = true;
                                    _dragIcon = new Draggable(
                                        ToCanvas(new Point(0, 0)).X + _mouseX,
                                        ToCanvas(new Point(0, 0)).X + _mouseY, _iconImage.Texture, _iconImage.RenderColor
                                    );

                                    //SOMETHING SHOULD BE RENDERED HERE, RIGHT?
                                }
                            }
                        }
                    }
                }
            }
            else if (_dragIcon.Update())
            {
                //Drug the item and now we stopped
                _isDragging = false;
                FloatRect dragRect = new(
                    _dragIcon.X - ItemXPadding / 2f,
                    _dragIcon.Y - ItemYPadding / 2f,
                    ItemXPadding / 2f + 32,
                    ItemYPadding / 2f + 32
                );

                float bestIntersect = 0;
                var bestIntersectIndex = -1;

                if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                {
                    var hotbarSlotComponents = Interface.GameUi.Hotbar.Items.ToArray();
                    var hotbarSlotLimit = Math.Min(
                        Options.Instance.Player.HotbarSlotCount,
                        hotbarSlotComponents.Length
                    );
                    for (var hotbarSlotIndex = 0; hotbarSlotIndex < hotbarSlotLimit; hotbarSlotIndex++)
                    {
                        var hotbarSlotComponent = hotbarSlotComponents[hotbarSlotIndex];
                        var hotbarSlotRenderBounds = hotbarSlotComponent.RenderBounds();
                        if (!hotbarSlotRenderBounds.IntersectsWith(dragRect))
                        {
                            continue;
                        }

                        var intersection = FloatRect.Intersect(hotbarSlotRenderBounds, dragRect);
                        if (intersection.Width * intersection.Height <= bestIntersect)
                        {
                            continue;
                        }

                        bestIntersect = intersection.Width * intersection.Height;

                        bestIntersectIndex = hotbarSlotIndex;
                    }

                    if (bestIntersectIndex > -1 && bestIntersectIndex != SlotIndex)
                    {
                        Globals.Me.HotbarSwap(SlotIndex, (byte)bestIntersectIndex);
                    }
                }

                _dragIcon.Dispose();
            }
            else
            {
                _iconImage.IsHidden = true;
                _equipLabel.IsHidden = true;
                _quantityLabel.IsHidden = true;
                _cooldownLabel.IsHidden = true;
            }
        }
    }
}
