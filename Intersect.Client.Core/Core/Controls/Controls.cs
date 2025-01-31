using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect.Client.Core.Controls;

public partial class Controls : IControlSet
{
    public static IControlSet ActiveControls
    {
        get => GameInput.Current.ControlSet;
        set => GameInput.Current.ControlSet = value;
    }

    private readonly Dictionary<Control, ControlMapping> _mappings = [];

    public Controls(IControlSet? other = null)
    {
        if (other != null)
        {
            foreach (var (control, mapping) in other.Mappings)
            {
                this[control] = new ControlMapping(mapping);
            }
        }
        else
        {
            ResetDefaults();
            TryLoad();
        }
    }

    public ControlMapping? this[Control control]
    {
        get => _mappings.GetValueOrDefault(control);
        set
        {
            if (value == null)
            {
                _ = _mappings.Remove(control);
            }
            else
            {
                _mappings[control] = value;
            }
        }
    }

    public IReadOnlyDictionary<Control, ControlMapping> Mappings => _mappings.AsReadOnly();

    private bool TryGetDefaultMappingFor(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping)
    {
        foreach (var controlsProvider in GameInput.Current.ControlsProviders)
        {
            if (controlsProvider.TryGetDefaultMapping(control, out defaultMapping))
            {
                return true;
            }
        }

        defaultMapping = null;
        return false;
    }

    public void ResetDefaults()
    {
        _mappings.Clear();

        foreach (var controlsProvider in GameInput.Current.ControlsProviders)
        {
            foreach (var control in controlsProvider.Controls)
            {
                if (controlsProvider.TryGetDefaultMapping(control, out var defaultMapping))
                {
                    TryAdd(control, defaultMapping);
                }
                else
                {
                    TryAdd(control, ControlBinding.Default, ControlBinding.Default);
                }
            }
        }

        TryAdd(Control.MoveUp, new ControlBinding(Keys.None, Keys.Up), new ControlBinding(Keys.None, Keys.W));
        TryAdd(Control.MoveDown, new ControlBinding(Keys.None, Keys.Down), new ControlBinding(Keys.None, Keys.S));
        TryAdd(Control.MoveLeft, new ControlBinding(Keys.None, Keys.Left), new ControlBinding(Keys.None, Keys.A));
        TryAdd(Control.MoveRight, new ControlBinding(Keys.None, Keys.Right), new ControlBinding(Keys.None, Keys.D));
        TryAdd(Control.AttackInteract, new ControlBinding(Keys.None, Keys.E), new ControlBinding(Keys.None, Keys.LButton));
        TryAdd(Control.Block, new ControlBinding(Keys.None, Keys.Q), new ControlBinding(Keys.None, Keys.RButton));
        TryAdd(Control.AutoTarget, new ControlBinding(Keys.None, Keys.Tab), ControlBinding.Default);
        TryAdd(Control.HoldToSoftRetargetOnSelfCast, new ControlBinding(Keys.None, Keys.LMenu), ControlBinding.Default);
        TryAdd(Control.ToggleAutoSoftRetargetOnSelfCast, ControlBinding.Default, ControlBinding.Default);
        TryAdd(Control.PickUp, new ControlBinding(Keys.None, Keys.Space), ControlBinding.Default);
        TryAdd(Control.Enter, new ControlBinding(Keys.None, Keys.Enter), ControlBinding.Default);
        TryAdd(Control.Screenshot, new ControlBinding(Keys.None, Keys.F12), ControlBinding.Default);
        TryAdd(Control.OpenMenu, new ControlBinding(Keys.None, Keys.Escape), ControlBinding.Default);
        TryAdd(Control.OpenInventory, new ControlBinding(Keys.None, Keys.I), ControlBinding.Default);
        TryAdd(Control.OpenQuests, new ControlBinding(Keys.None, Keys.L), ControlBinding.Default);
        TryAdd(Control.OpenCharacterInfo, new ControlBinding(Keys.None, Keys.C), ControlBinding.Default);
        TryAdd(Control.OpenParties, new ControlBinding(Keys.None, Keys.P), ControlBinding.Default);
        TryAdd(Control.OpenSpells, new ControlBinding(Keys.None, Keys.K), ControlBinding.Default);
        TryAdd(Control.OpenFriends, new ControlBinding(Keys.None, Keys.F), ControlBinding.Default);
        TryAdd(Control.OpenGuild, new ControlBinding(Keys.None, Keys.G), ControlBinding.Default);
        TryAdd(Control.OpenSettings, new ControlBinding(Keys.None, Keys.O), ControlBinding.Default);
        TryAdd(Control.OpenDebugger, new ControlBinding(Keys.None, Keys.F2), ControlBinding.Default);
        TryAdd(Control.OpenAdminPanel, new ControlBinding(Keys.None, Keys.Insert), ControlBinding.Default);
        TryAdd(Control.ToggleGui, new ControlBinding(Keys.None, Keys.F11), ControlBinding.Default);
        TryAdd(Control.TurnAround, new ControlBinding(Keys.None, Keys.Control), ControlBinding.Default);
        TryAdd(Control.ToggleZoomIn, ControlBinding.Default, ControlBinding.Default);
        TryAdd(Control.ToggleZoomOut, ControlBinding.Default, ControlBinding.Default);
        TryAdd(Control.HoldToZoomIn, ControlBinding.Default, ControlBinding.Default);
        TryAdd(Control.HoldToZoomOut, ControlBinding.Default, ControlBinding.Default);
        TryAdd(Control.ToggleFullscreen, new ControlBinding(Keys.Alt, Keys.Enter), ControlBinding.Default);

        // Hotkeys should be at the end of the list
        TryAdd(Control.Hotkey1, new ControlBinding(Keys.None, Keys.D1), ControlBinding.Default);
        TryAdd(Control.Hotkey2, new ControlBinding(Keys.None, Keys.D2), ControlBinding.Default);
        TryAdd(Control.Hotkey3, new ControlBinding(Keys.None, Keys.D3), ControlBinding.Default);
        TryAdd(Control.Hotkey4, new ControlBinding(Keys.None, Keys.D4), ControlBinding.Default);
        TryAdd(Control.Hotkey5, new ControlBinding(Keys.None, Keys.D5), ControlBinding.Default);
        TryAdd(Control.Hotkey6, new ControlBinding(Keys.None, Keys.D6), ControlBinding.Default);
        TryAdd(Control.Hotkey7, new ControlBinding(Keys.None, Keys.D7), ControlBinding.Default);
        TryAdd(Control.Hotkey8, new ControlBinding(Keys.None, Keys.D8), ControlBinding.Default);
        TryAdd(Control.Hotkey9, new ControlBinding(Keys.None, Keys.D9), ControlBinding.Default);
        TryAdd(Control.Hotkey10, new ControlBinding(Keys.None, Keys.D0), ControlBinding.Default);
    }

    private static void MigrateControlBindings(Control control)
    {
        var controlId = control.GetControlId();
        if (Globals.Database.HasPreference($"{controlId}_key1value"))
        {
            Globals.Database.SavePreference($"{controlId}_binding0", Globals.Database.LoadPreference($"{controlId}_key1value"));
            Globals.Database.SavePreference($"{controlId}_binding1", Globals.Database.LoadPreference($"{controlId}_key2value"));
            Globals.Database.DeletePreference($"{controlId}_key1value");
            Globals.Database.DeletePreference($"{controlId}_key2value");
        }
        else if (Globals.Database.HasPreference($"{controlId}_key1"))
        {
            var key1 = JsonConvert.DeserializeObject<Keys>(Globals.Database.LoadPreference($"{controlId}_key1"));
            var key2 = JsonConvert.DeserializeObject<Keys>(Globals.Database.LoadPreference($"{controlId}_key2"));
            Globals.Database.SavePreference($"{controlId}_binding0", JsonConvert.SerializeObject(new ControlBinding(Keys.None, key1)));
            Globals.Database.SavePreference($"{controlId}_binding1", JsonConvert.SerializeObject(new ControlBinding(Keys.None, key2)));
            Globals.Database.DeletePreference($"{controlId}_key1");
            Globals.Database.DeletePreference($"{controlId}_key2");
        }
    }

    public void UpdateControl(Control control, int keyNum, Keys modifier, Keys key)
    {
        if (!Mappings.TryGetValue(control, out var mapping))
        {
            return;
        }

        mapping.Bindings[keyNum].Modifier = modifier;
        mapping.Bindings[keyNum].Key = key;
    }

    public void ReloadFromOptions(Options options)
    {
        _ = TryLoad();

        if (options.Player is not { } playerOptions)
        {
            return;
        }

        for (var hotbarSlotIndex = 0; hotbarSlotIndex < playerOptions.HotbarSlotCount; ++hotbarSlotIndex)
        {
            Control hotbarSlotControl = Control.HotkeyOffset + hotbarSlotIndex + 1;
            if (Mappings.TryGetValue(hotbarSlotControl, out var mapping))
            {
                continue;
            }

            MigrateControlBindings(hotbarSlotControl);

            mapping = new ControlMapping(ControlBinding.Default, ControlBinding.Default);
            _mappings[hotbarSlotControl] = mapping;
        }
    }

    public bool TryAdd(Control control, params ControlBinding[] bindings) =>
        bindings.Length >= 1 &&
        _mappings.TryAdd(control, new ControlMapping(bindings.First(), bindings.Skip(1).ToArray()));

    public bool TryAdd(Control control, ControlMapping mapping) => _mappings.TryAdd(control, mapping);

    public bool TryGetMappingFor(Control control, [NotNullWhen(true)] out ControlMapping? mapping) =>
        Mappings.TryGetValue(control, out mapping);

    public bool TryLoad()
    {
        bool success = true;

        try
        {
            var allControls = GameInput.Current.AllControls;
            foreach (var control in allControls)
            {
                try
                {
                    MigrateControlBindings(control);
                }
                catch (Exception exception)
                {
                    ApplicationContext.Context.Value?.Logger.LogError(
                        exception,
                        "Error while migrating bindings for control {Control}",
                        control.GetControlId()
                    );
                }

                try
                {
                    if (!TryGetMappingFor(control, out var mapping))
                    {
                        if (!TryGetDefaultMappingFor(control, out var defaultMapping))
                        {
                            ApplicationContext.Context.Value?.Logger.LogDebug(
                                "No existing or default mapping for {ControlId}",
                                control.GetControlId()
                            );
                            defaultMapping = new ControlMapping(ControlBinding.Default, ControlBinding.Default);
                        }

                        if (TryAdd(control, defaultMapping))
                        {
                            mapping = defaultMapping;
                        }
                        else
                        {
                            ApplicationContext.Context.Value?.Logger.LogWarning(
                                "Failed to add default mapping for {ControlId}",
                                control.GetControlId()
                            );
                            continue;
                        }
                    }

                    for (var bindingIndex = 0; bindingIndex < mapping.Bindings.Count; bindingIndex++)
                    {
                        if (TryLoadBindingFor(control, bindingIndex, out var binding))
                        {
                            mapping.Bindings[bindingIndex] = binding;
                        }
                        else
                        {
                            _ = TrySaveBindingFor(control, bindingIndex, mapping.Bindings[bindingIndex]);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ApplicationContext.Context.Value?.Logger.LogError(
                        exception,
                        "Error while loading bindings for control {Control}",
                        control.GetControlId()
                    );

                    success = false;
                }
            }

            return success;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error while getting controls");
            return false;
        }
    }

    private bool TryLoadBindingFor(Control control, int bindingIndex, [NotNullWhen(true)] out ControlBinding? binding)
    {
        try
        {
            var preferenceKey = GetPreferenceKeyFor(control, bindingIndex);
            var preference = Globals.Database.LoadPreference(preferenceKey);
            if (string.IsNullOrWhiteSpace(preference))
            {
                binding = null;
                return false;
            }

            binding = JsonConvert.DeserializeObject<ControlBinding>(preference);
            return binding != null;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error while loading binding {BindingIndex} for control {Control}",
                bindingIndex,
                control.GetControlId()
            );

            binding = null;
            return false;
        }
    }

    public bool TryReload() => TryLoad();

    public bool TrySave()
    {
        bool success = true;

        foreach (var (control, mapping) in Mappings)
        {
            try
            {
                var bindings = mapping.Bindings.ToArray();
                for (var bindingIndex = 0; bindingIndex < bindings.Length; bindingIndex++)
                {
                    success &= TrySaveBindingFor(control, bindingIndex, bindings[bindingIndex]);
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    exception,
                    "Error while saving bindings for control {Control}",
                    control.GetControlId()
                );

                success = false;
            }
        }

        return success;
    }

    private string GetPreferenceKeyFor(Control control, int bindingIndex)
    {
        var controlName = control.GetControlId();
        var preferenceKey = $"{controlName}_binding{bindingIndex}";
        return preferenceKey;
    }

    private bool TrySaveBindingFor(Control control, int bindingIndex, ControlBinding binding)
    {
        try
        {
            var preferenceKey = GetPreferenceKeyFor(control, bindingIndex);
            var serializedBinding = JsonConvert.SerializeObject(binding);
            Globals.Database.SavePreference(preferenceKey, serializedBinding);

            return true;
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error while saving binding {BindingIndex} for control {Control}",
                bindingIndex,
                control.GetControlId()
            );

            return false;
        }
    }

    public static bool IsControlPressed(Control control) => IsControlPressed(control, out _, out _);

    public static bool IsControlPressed(
        Control control,
        [NotNullWhen(true)] out ControlMapping? activeMapping,
        [NotNullWhen(true)] out ControlBinding? activeBinding
    )
    {
        if (ActiveControls.Mappings.TryGetValue(control, out var mapping) && mapping.IsActive(out activeBinding))
        {
            activeMapping = mapping;
            return true;
        }

        activeMapping = null;
        activeBinding = null;
        return false;
    }

    public static List<Control> GetControlsFor(Keys modifier, Keys key)
    {
        return Enum.GetValues(typeof(Control))
            .Cast<Control>()
            .Where(control => ControlHasKey(control, modifier, key))
            .ToList();
    }

    public static bool ControlHasKey(Control control, Keys modifier, Keys key)
    {
        if (key == Keys.None)
        {
            return false;
        }

        if (modifier == Keys.Alt && key is Keys.LMenu or Keys.RMenu)
        {
            return ControlHasKey(control, Keys.None, key);
        }

        return ActiveControls.Mappings.TryGetValue(control, out var mapping) &&
               mapping.Bindings.Any(binding => binding.Modifier == modifier && binding.Key == key);
    }
}
