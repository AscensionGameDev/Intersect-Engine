using System.Diagnostics.CodeAnalysis;
using Intersect.Client.Framework.Database;
using Intersect.Client.Framework.GenericClasses;
using Newtonsoft.Json;

namespace Intersect.Client.Framework.Input;

internal sealed class BuiltinControlsProvider : IControlsProvider
{
    private readonly Dictionary<Control, ControlMapping> _defaultMappings = LoadDefaultMappings();

    public Control[] Controls { get; } = Enum.GetValues<Control>().Where(control => control.IsValid()).ToArray();

    public bool TryGetDefaultMapping(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping)
    {
        if (_defaultMappings.TryGetValue(control, out var mapping))
        {
            defaultMapping = new ControlMapping(mapping);
            return true;
        }

        defaultMapping = null;
        return false;
    }

    public void ReloadFromOptions(Options? options)
    {
        // Builtin controls don't change based on options
        // Defaults are loaded once from default_settings.json at initialization
    }

    /// <summary>
    /// Loads default mappings by starting with hardcoded defaults and then merging overrides from default_settings.json if present.
    /// </summary>
    public static Dictionary<Control, ControlMapping> LoadDefaultMappings()
    {
        // Start with all hardcoded defaults
        var defaults = GetHardcodedDefaults();

        // Load default controls from default_settings.json
        var defaultSettingsPath = Path.Combine("resources", "default_settings.json");
        if (File.Exists(defaultSettingsPath))
        {
            var db = new JsonDatabase(defaultSettingsPath);

            foreach (var control in defaults.Keys)
            {
                var controlId = control.GetControlId();
                var bindings = new List<ControlBinding>();

                if (db.HasPreference($"{controlId}_binding0") && db.HasPreference($"{controlId}_binding1"))
                {
                    var binding1 = JsonConvert.DeserializeObject<ControlBinding>(db.LoadPreference($"{controlId}_binding0"));
                    var binding2 = JsonConvert.DeserializeObject<ControlBinding>(db.LoadPreference($"{controlId}_binding1"));
                    if (binding1 != null && binding2 != null)
                    {
                        defaults[control] = new ControlMapping(binding1, binding2);
                    }
                }
            }
        }

        return defaults;
    }

    /// <summary>
    /// Returns the hardcoded default control mappings.
    /// </summary>
    private static Dictionary<Control, ControlMapping> GetHardcodedDefaults()
    {
        return new Dictionary<Control, ControlMapping>
        {
            { Control.MoveUp, new ControlMapping(new ControlBinding(Keys.None, Keys.Up), new ControlBinding(Keys.None, Keys.W)) },
            { Control.MoveDown, new ControlMapping(new ControlBinding(Keys.None, Keys.Down), new ControlBinding(Keys.None, Keys.S)) },
            { Control.MoveLeft, new ControlMapping(new ControlBinding(Keys.None, Keys.Left), new ControlBinding(Keys.None, Keys.A)) },
            { Control.MoveRight, new ControlMapping(new ControlBinding(Keys.None, Keys.Right), new ControlBinding(Keys.None, Keys.D)) },
            { Control.AttackInteract, new ControlMapping(new ControlBinding(Keys.None, Keys.E), new ControlBinding(Keys.None, Keys.LButton)) },
            { Control.Block, new ControlMapping(new ControlBinding(Keys.None, Keys.Q), new ControlBinding(Keys.None, Keys.RButton)) },
            { Control.AutoTarget, new ControlMapping(new ControlBinding(Keys.None, Keys.Tab), ControlBinding.Default) },
            { Control.HoldToSoftRetargetOnSelfCast, new ControlMapping(new ControlBinding(Keys.None, Keys.LMenu), ControlBinding.Default) },
            { Control.ToggleAutoSoftRetargetOnSelfCast, new ControlMapping(ControlBinding.Default, ControlBinding.Default) },
            { Control.PickUp, new ControlMapping(new ControlBinding(Keys.None, Keys.Space), ControlBinding.Default) },
            { Control.Enter, new ControlMapping(new ControlBinding(Keys.None, Keys.Enter), ControlBinding.Default) },
            { Control.Screenshot, new ControlMapping(new ControlBinding(Keys.None, Keys.F12), ControlBinding.Default) },
            { Control.OpenMenu, new ControlMapping(new ControlBinding(Keys.None, Keys.Escape), ControlBinding.Default) },
            { Control.OpenInventory, new ControlMapping(new ControlBinding(Keys.None, Keys.I), ControlBinding.Default) },
            { Control.OpenQuests, new ControlMapping(new ControlBinding(Keys.None, Keys.L), ControlBinding.Default) },
            { Control.OpenCharacterInfo, new ControlMapping(new ControlBinding(Keys.None, Keys.C), ControlBinding.Default) },
            { Control.OpenParties, new ControlMapping(new ControlBinding(Keys.None, Keys.P), ControlBinding.Default) },
            { Control.OpenSpells, new ControlMapping(new ControlBinding(Keys.None, Keys.K), ControlBinding.Default) },
            { Control.OpenFriends, new ControlMapping(new ControlBinding(Keys.None, Keys.F), ControlBinding.Default) },
            { Control.OpenGuild, new ControlMapping(new ControlBinding(Keys.None, Keys.G), ControlBinding.Default) },
            { Control.OpenSettings, new ControlMapping(new ControlBinding(Keys.None, Keys.O), ControlBinding.Default) },
            { Control.OpenDebugger, new ControlMapping(new ControlBinding(Keys.None, Keys.F2), ControlBinding.Default) },
            { Control.OpenAdminPanel, new ControlMapping(new ControlBinding(Keys.None, Keys.Insert), ControlBinding.Default) },
            { Control.ToggleGui, new ControlMapping(new ControlBinding(Keys.None, Keys.F11), ControlBinding.Default) },
            { Control.TurnAround, new ControlMapping(new ControlBinding(Keys.None, Keys.Control), ControlBinding.Default) },
            { Control.ToggleZoomIn, new ControlMapping(ControlBinding.Default, ControlBinding.Default) },
            { Control.ToggleZoomOut, new ControlMapping(ControlBinding.Default, ControlBinding.Default) },
            { Control.HoldToZoomIn, new ControlMapping(ControlBinding.Default, ControlBinding.Default) },
            { Control.HoldToZoomOut, new ControlMapping(ControlBinding.Default, ControlBinding.Default) },
            { Control.ToggleFullscreen, new ControlMapping(new ControlBinding(Keys.Alt, Keys.Enter), ControlBinding.Default) },

            // Hotkeys should be at the end of the list
            { Control.Hotkey1, new ControlMapping(new ControlBinding(Keys.None, Keys.D1), ControlBinding.Default) },
            { Control.Hotkey2, new ControlMapping(new ControlBinding(Keys.None, Keys.D2), ControlBinding.Default) },
            { Control.Hotkey3, new ControlMapping(new ControlBinding(Keys.None, Keys.D3), ControlBinding.Default) },
            { Control.Hotkey4, new ControlMapping(new ControlBinding(Keys.None, Keys.D4), ControlBinding.Default) },
            { Control.Hotkey5, new ControlMapping(new ControlBinding(Keys.None, Keys.D5), ControlBinding.Default) },
            { Control.Hotkey6, new ControlMapping(new ControlBinding(Keys.None, Keys.D6), ControlBinding.Default) },
            { Control.Hotkey7, new ControlMapping(new ControlBinding(Keys.None, Keys.D7), ControlBinding.Default) },
            { Control.Hotkey8, new ControlMapping(new ControlBinding(Keys.None, Keys.D8), ControlBinding.Default) },
            { Control.Hotkey9, new ControlMapping(new ControlBinding(Keys.None, Keys.D9), ControlBinding.Default) },
            { Control.Hotkey10, new ControlMapping(new ControlBinding(Keys.None, Keys.D0), ControlBinding.Default) },
        };
    }
}