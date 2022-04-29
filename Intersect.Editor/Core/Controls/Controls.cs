using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Editor.General;
using Newtonsoft.Json;

namespace Intersect.Editor.Core.Controls
{

    public class Controls
    {

        public readonly IDictionary<Control, ControlMap> ControlMapping;

        public Controls(Controls gameControls = null)
        {
            ControlMapping = new Dictionary<Control, ControlMap>();

            if (gameControls != null)
            {
                foreach (var mapping in gameControls.ControlMapping)
                {
                    CreateControlMap(mapping.Key, mapping.Value.Key1, mapping.Value.Key2);
                }
            }
            else
            {
                ResetDefaults();
                foreach (Control control in Enum.GetValues(typeof(Control)))
                {
                    var name = Enum.GetName(typeof(Control), control);
                    var key1 = Globals.Database.LoadPreference(name + "_key1value");
                    var key2 = Globals.Database.LoadPreference(name + "_key2value");
                    if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
                    {
                        Globals.Database.SavePreference(
                            name + "_key1value", JsonConvert.SerializeObject(ControlMapping[control].Key1)
                        );

                        Globals.Database.SavePreference(
                            name + "_key2value", JsonConvert.SerializeObject(ControlMapping[control].Key2)
                        );
                    }
                    else
                    {
                        CreateControlMap(control, JsonConvert.DeserializeObject<ControlValue>(key1), JsonConvert.DeserializeObject<ControlValue>(key2));
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
            CreateControlMap(Control.AutoTarget, new ControlValue(Keys.None, Keys.Tab), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.PickUp, new ControlValue(Keys.None, Keys.Space), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Enter, new ControlValue(Keys.None, Keys.Enter), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey1, new ControlValue(Keys.None, Keys.D1), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey2, new ControlValue(Keys.None, Keys.D2), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey3, new ControlValue(Keys.None, Keys.D3), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey4, new ControlValue(Keys.None, Keys.D4), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey5, new ControlValue(Keys.None, Keys.D5), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey6, new ControlValue(Keys.None, Keys.D6), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey7, new ControlValue(Keys.None, Keys.D7), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey8, new ControlValue(Keys.None, Keys.D8), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey9, new ControlValue(Keys.None, Keys.D9), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Hotkey0, new ControlValue(Keys.None, Keys.D0), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.Screenshot, new ControlValue(Keys.None, Keys.F12), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenMenu, new ControlValue(Keys.None, Keys.Escape), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenInventory, new ControlValue(Keys.None, Keys.I), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenQuests, new ControlValue(Keys.None, Keys.L), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenCharacterInfo, new ControlValue(Keys.None, Keys.C), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenParties, new ControlValue(Keys.None, Keys.P), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenSpells, new ControlValue(Keys.None, Keys.K), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenFriends, new ControlValue(Keys.None, Keys.F), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenGuild, new ControlValue(Keys.None, Keys.G), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenSettings, new ControlValue(Keys.None, Keys.O), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenDebugger, new ControlValue(Keys.None, Keys.F2), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.OpenAdminPanel, new ControlValue(Keys.None, Keys.Insert), new ControlValue(Keys.None, Keys.None));
            CreateControlMap(Control.ToggleGui, new ControlValue(Keys.None, Keys.F11), new ControlValue(Keys.None, Keys.None));
        }

        public void Save()
        {
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var name = Enum.GetName(typeof(Control), control);
                Globals.Database.SavePreference(name + "_key1value", JsonConvert.SerializeObject(ControlMapping[control].Key1));
                Globals.Database.SavePreference(name + "_key2value", JsonConvert.SerializeObject(ControlMapping[control].Key2));
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

            return (mapping?.Key1.Modifier == modifier && mapping?.Key1.Key == key) || (mapping?.Key2.Modifier == modifier && mapping?.Key2.Key == key);
        }

        public void UpdateControl(Control control, int keyNum, Keys modifier, Keys key)
        {
            var mapping = ControlMapping[control];
            if (mapping == null)
            {
                return;
            }

            if (keyNum == 1)
            {
                mapping.Key1.Modifier = modifier;
                mapping.Key1.Key = key;
            }
            else
            {
                mapping.Key2.Modifier = modifier;
                mapping.Key2.Key = key;
            }
        }

        private void CreateControlMap(Control control, ControlValue key1, ControlValue key2)
        {
            if (ControlMapping.ContainsKey(control))
            {
                ControlMapping[control] = new ControlMap(control, key1, key2);
            }
            else
            {
                ControlMapping.Add(control, new ControlMap(control, key1, key2));
            }
        }

    }

}
