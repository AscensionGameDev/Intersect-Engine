using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;

namespace Intersect.Client.Core.Controls
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
                    var key1 = Globals.Database.LoadPreference(name + "_key1");
                    var key2 = Globals.Database.LoadPreference(name + "_key2");
                    if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
                    {
                        Globals.Database.SavePreference(
                            name + "_key1", ((int) ControlMapping[control].Key1).ToString()
                        );

                        Globals.Database.SavePreference(
                            name + "_key2", ((int) ControlMapping[control].Key2).ToString()
                        );
                    }
                    else
                    {
                        CreateControlMap(control, (Keys) Convert.ToInt32(key1), (Keys) Convert.ToInt32(key2));
                    }
                }
            }
        }

        public static Controls ActiveControls { get; set; }

        public void ResetDefaults()
        {
            CreateControlMap(Control.MoveUp, Keys.Up, Keys.W);
            CreateControlMap(Control.MoveDown, Keys.Down, Keys.S);
            CreateControlMap(Control.MoveLeft, Keys.Left, Keys.A);
            CreateControlMap(Control.MoveRight, Keys.Right, Keys.D);
            CreateControlMap(Control.AttackInteract, Keys.E, Keys.LButton);
            CreateControlMap(Control.Block, Keys.Q, Keys.RButton);
            CreateControlMap(Control.AutoTarget, Keys.Tab, Keys.None);
            CreateControlMap(Control.PickUp, Keys.Space, Keys.None);
            CreateControlMap(Control.Enter, Keys.Enter, Keys.None);
            CreateControlMap(Control.Hotkey1, Keys.D1, Keys.None);
            CreateControlMap(Control.Hotkey2, Keys.D2, Keys.None);
            CreateControlMap(Control.Hotkey3, Keys.D3, Keys.None);
            CreateControlMap(Control.Hotkey4, Keys.D4, Keys.None);
            CreateControlMap(Control.Hotkey5, Keys.D5, Keys.None);
            CreateControlMap(Control.Hotkey6, Keys.D6, Keys.None);
            CreateControlMap(Control.Hotkey7, Keys.D7, Keys.None);
            CreateControlMap(Control.Hotkey8, Keys.D8, Keys.None);
            CreateControlMap(Control.Hotkey9, Keys.D9, Keys.None);
            CreateControlMap(Control.Hotkey0, Keys.D0, Keys.None);
            CreateControlMap(Control.Screenshot, Keys.F12, Keys.None);
            CreateControlMap(Control.OpenMenu, Keys.Escape, Keys.None);
            CreateControlMap(Control.OpenInventory, Keys.I, Keys.None);
            CreateControlMap(Control.OpenQuests, Keys.L, Keys.None);
            CreateControlMap(Control.OpenCharacterInfo, Keys.C, Keys.None);
            CreateControlMap(Control.OpenParties, Keys.P, Keys.None);
            CreateControlMap(Control.OpenSpells, Keys.X, Keys.None);
            CreateControlMap(Control.OpenFriends, Keys.F, Keys.None);
            CreateControlMap(Control.OpenSettings, Keys.None, Keys.None);
            CreateControlMap(Control.OpenDebugger, Keys.F2, Keys.None);
            CreateControlMap(Control.OpenAdminPanel, Keys.Insert, Keys.None);
            CreateControlMap(Control.ToggleGui, Keys.F11, Keys.None);
        }

        public void Save()
        {
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var name = Enum.GetName(typeof(Control), control);
                Globals.Database.SavePreference(name + "_key1", ((int) ControlMapping[control].Key1).ToString());
                Globals.Database.SavePreference(name + "_key2", ((int) ControlMapping[control].Key2).ToString());
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

        public static List<Control> GetControlsFor(Keys key)
        {
            return Enum.GetValues(typeof(Control))
                .Cast<Control>()
                .Where(control => ControlHasKey(control, key))
                .ToList();
        }

        public static bool ControlHasKey(Control control, Keys key)
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

            return mapping?.Key1 == key || mapping?.Key2 == key;
        }

        public void UpdateControl(Control control, int keyNum, Keys key)
        {
            var mapping = ControlMapping[control];
            if (mapping == null)
            {
                return;
            }

            if (keyNum == 1)
            {
                mapping.Key1 = key;
            }
            else
            {
                mapping.Key2 = key;
            }
        }

        private void CreateControlMap(Control control, Keys key1, Keys key2)
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
