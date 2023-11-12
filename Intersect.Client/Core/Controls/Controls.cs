using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Newtonsoft.Json;

namespace Intersect.Client.Core.Controls
{

    public partial class Controls
    {

        public readonly IDictionary<Control, ControlMap> ControlMapping;

        public Controls(Controls gameControls = null)
        {
            ControlMapping = new Dictionary<Control, ControlMap>();

            if (gameControls != null)
            {
                foreach (var mapping in gameControls.ControlMapping)
                {
                    CreateControlMap(mapping.Key, mapping.Value);
                }
            }
            else
            {
                ResetDefaults();
                foreach (Control control in Enum.GetValues(typeof(Control)))
                {
                    MigrateControlBindings(control);

                    var name = Enum.GetName(typeof(Control), control);

                    if (!ControlMapping.TryGetValue(control, out var controlMapping))
                    {
                        continue;
                    }

                    var bindings = controlMapping.Bindings;
                    for (var bindingIndex = 0; bindingIndex < bindings.Count; bindingIndex++)
                    {
                        var preferenceKey = $"{name}_binding{bindingIndex}";
                        var preference = Globals.Database.LoadPreference(preferenceKey);
                        if (string.IsNullOrWhiteSpace(preference))
                        {
                            Globals.Database.SavePreference(preferenceKey, JsonConvert.SerializeObject(bindings[bindingIndex]));
                        }
                        else
                        {
                            bindings[bindingIndex] = JsonConvert.DeserializeObject<ControlValue>(preference);
                        }
                    }
                }
            }
        }

        public static Controls ActiveControls { get; set; }

        public void ResetDefaults()
        {
            CreateControlMap(Control.MoveUp, new ControlValue(Keys.None, Keys.Up), new ControlValue(Keys.None, Keys.W));
            CreateControlMap(Control.MoveDown, new ControlValue(Keys.None, Keys.Down), new ControlValue(Keys.None, Keys.S));
            CreateControlMap(Control.MoveLeft, new ControlValue(Keys.None, Keys.Left), new ControlValue(Keys.None, Keys.A));
            CreateControlMap(Control.MoveRight, new ControlValue(Keys.None, Keys.Right), new ControlValue(Keys.None, Keys.D));
            CreateControlMap(Control.AttackInteract, new ControlValue(Keys.None, Keys.E), new ControlValue(Keys.None, Keys.LButton));
            CreateControlMap(Control.Block, new ControlValue(Keys.None, Keys.Q), new ControlValue(Keys.None, Keys.RButton));
            CreateControlMap(Control.AutoTarget, new ControlValue(Keys.None, Keys.Tab), ControlValue.Default);
            CreateControlMap(Control.PickUp, new ControlValue(Keys.None, Keys.Space), ControlValue.Default);
            CreateControlMap(Control.Enter, new ControlValue(Keys.None, Keys.Enter), ControlValue.Default);
            CreateControlMap(Control.Hotkey1, new ControlValue(Keys.None, Keys.D1), ControlValue.Default);
            CreateControlMap(Control.Hotkey2, new ControlValue(Keys.None, Keys.D2), ControlValue.Default);
            CreateControlMap(Control.Hotkey3, new ControlValue(Keys.None, Keys.D3), ControlValue.Default);
            CreateControlMap(Control.Hotkey4, new ControlValue(Keys.None, Keys.D4), ControlValue.Default);
            CreateControlMap(Control.Hotkey5, new ControlValue(Keys.None, Keys.D5), ControlValue.Default);
            CreateControlMap(Control.Hotkey6, new ControlValue(Keys.None, Keys.D6), ControlValue.Default);
            CreateControlMap(Control.Hotkey7, new ControlValue(Keys.None, Keys.D7), ControlValue.Default);
            CreateControlMap(Control.Hotkey8, new ControlValue(Keys.None, Keys.D8), ControlValue.Default);
            CreateControlMap(Control.Hotkey9, new ControlValue(Keys.None, Keys.D9), ControlValue.Default);
            CreateControlMap(Control.Hotkey0, new ControlValue(Keys.None, Keys.D0), ControlValue.Default);
            CreateControlMap(Control.Screenshot, new ControlValue(Keys.None, Keys.F12), ControlValue.Default);
            CreateControlMap(Control.OpenMenu, new ControlValue(Keys.None, Keys.Escape), ControlValue.Default);
            CreateControlMap(Control.OpenInventory, new ControlValue(Keys.None, Keys.I), ControlValue.Default);
            CreateControlMap(Control.OpenQuests, new ControlValue(Keys.None, Keys.L), ControlValue.Default);
            CreateControlMap(Control.OpenCharacterInfo, new ControlValue(Keys.None, Keys.C), ControlValue.Default);
            CreateControlMap(Control.OpenParties, new ControlValue(Keys.None, Keys.P), ControlValue.Default);
            CreateControlMap(Control.OpenSpells, new ControlValue(Keys.None, Keys.K), ControlValue.Default);
            CreateControlMap(Control.OpenFriends, new ControlValue(Keys.None, Keys.F), ControlValue.Default);
            CreateControlMap(Control.OpenGuild, new ControlValue(Keys.None, Keys.G), ControlValue.Default);
            CreateControlMap(Control.OpenSettings, new ControlValue(Keys.None, Keys.O), ControlValue.Default);
            CreateControlMap(Control.OpenDebugger, new ControlValue(Keys.None, Keys.F2), ControlValue.Default);
            CreateControlMap(Control.OpenAdminPanel, new ControlValue(Keys.None, Keys.Insert), ControlValue.Default);
            CreateControlMap(Control.ToggleGui, new ControlValue(Keys.None, Keys.F11), ControlValue.Default);
            CreateControlMap(Control.TurnAround, new ControlValue(Keys.None, Keys.Control), ControlValue.Default);
            CreateControlMap(Control.ToggleZoomIn, ControlValue.Default, ControlValue.Default);
            CreateControlMap(Control.ToggleZoomOut, ControlValue.Default, ControlValue.Default);
            CreateControlMap(Control.HoldToZoomIn, ControlValue.Default, ControlValue.Default);
            CreateControlMap(Control.HoldToZoomOut, ControlValue.Default, ControlValue.Default);
            CreateControlMap(Control.ToggleFullscreen, new ControlValue(Keys.Alt, Keys.Enter), ControlValue.Default);
            CreateControlMap(Control.OpenMinimap, new ControlValue(Keys.None, Keys.M), ControlValue.Default);
            // CreateControlMap(Control.Submit, new ControlValue(Keys.None, Keys.Enter), ControlValue.Default);
            // CreateControlMap(Control.Cancel, new ControlValue(Keys.None, Keys.Back), ControlValue.Default);
            // CreateControlMap(Control.Next, new ControlValue(Keys.None, Keys.Tab), ControlValue.Default);
            // CreateControlMap(Control.Previous, new ControlValue(Keys.Shift, Keys.Tab), ControlValue.Default);
        }

        private static void MigrateControlBindings(Control control)
        {
            var name = Enum.GetName(typeof(Control), control);
            if (Globals.Database.HasPreference($"{name}_key1value"))
            {
                Globals.Database.SavePreference($"{name}_binding0", Globals.Database.LoadPreference($"{name}_key1value"));
                Globals.Database.SavePreference($"{name}_binding1", Globals.Database.LoadPreference($"{name}_key2value"));
                Globals.Database.DeletePreference($"{name}_key1value");
                Globals.Database.DeletePreference($"{name}_key2value");
            }
            else if (Globals.Database.HasPreference($"{name}_key1"))
            {
                var key1 = JsonConvert.DeserializeObject<Keys>(Globals.Database.LoadPreference($"{name}_key1"));
                var key2 = JsonConvert.DeserializeObject<Keys>(Globals.Database.LoadPreference($"{name}_key2"));
                Globals.Database.SavePreference($"{name}_binding0", JsonConvert.SerializeObject(new ControlValue(Keys.None, key1)));
                Globals.Database.SavePreference($"{name}_binding1", JsonConvert.SerializeObject(new ControlValue(Keys.None, key2)));
                Globals.Database.DeletePreference($"{name}_key1");
                Globals.Database.DeletePreference($"{name}_key2");
            }
        }

        public void Save()
        {
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var name = Enum.GetName(typeof(Control), control);
                var bindings = ControlMapping[control].Bindings;
                for (var bindingIndex = 0; bindingIndex < bindings.Count; bindingIndex++)
                {
                    var preferenceKey = $"{name}_binding{bindingIndex}";
                    Globals.Database.SavePreference(preferenceKey, JsonConvert.SerializeObject(bindings[bindingIndex]));
                }
            }
        }

        public static void Init()
        {
            ActiveControls = new Controls();
        }

        public static bool KeyDown(Control control)
        {
            if (ActiveControls?.ControlMapping.ContainsKey(control) ?? false)
            {
                return ActiveControls.ControlMapping[control]?.KeyDown() ?? false;
            }

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

            if (!(ActiveControls?.ControlMapping.ContainsKey(control) ?? false))
            {
                return false;
            }

            var mapping = ActiveControls.ControlMapping[control];

            return mapping?.Bindings.Any(binding => binding.Modifier == modifier && binding.Key == key) ?? false;
        }

        public void UpdateControl(Control control, int keyNum, Keys modifier, Keys key)
        {
            var mapping = ControlMapping[control];
            if (mapping == null)
            {
                return;
            }

            mapping.Bindings[keyNum].Modifier = modifier;
            mapping.Bindings[keyNum].Key = key;
        }

        private void CreateControlMap(Control control, ControlValue binding, params ControlValue[] alternateBindings)
        {
            ControlMapping[control] = new ControlMap(binding, alternateBindings);
        }

        private void CreateControlMap(Control control, ControlMap controlMap)
        {
            ControlMapping[control] = new ControlMap(controlMap);
        }

    }

}
