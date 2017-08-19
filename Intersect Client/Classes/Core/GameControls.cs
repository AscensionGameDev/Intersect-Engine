using System;
using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.General;

namespace Intersect.Client.Classes.Core
{
    public enum Controls
    {
        MoveUp,
        MoveLeft,
        MoveDown,
        MoveRight,
        AttackInteract,
        Block,
        PickUp,
        Enter,
        Hotkey1,
        Hotkey2,
        Hotkey3,
        Hotkey4,
        Hotkey5,
        Hotkey6,
        Hotkey7,
        Hotkey8,
        Hotkey9,
        Hotkey0,
    }

    public class GameControls
    {
        public static GameControls ActiveControls;
        public Dictionary<Controls, ControlMap> ControlMapping = new Dictionary<Controls, ControlMap>();

        public GameControls()
        {
            ControlMapping.Clear();
            ResetDefaults();

            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var name = Enum.GetName(typeof(Controls), control);
                var key1 = Globals.Database.LoadPreference(name + "_key1");
                var key2 = Globals.Database.LoadPreference(name + "_key2");
                if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
                {
                    Globals.Database.SavePreference(name + "_key1", ((int) ControlMapping[control].key1).ToString());
                    Globals.Database.SavePreference(name + "_key2", ((int) ControlMapping[control].key2).ToString());
                }
                else
                {
                    CreateControlMap(control, (Keys) Convert.ToInt32(key1), (Keys) Convert.ToInt32(key2));
                }
            }
        }

        public GameControls(GameControls copyFrom)
        {
            ControlMapping.Clear();
            foreach (var mapping in copyFrom.ControlMapping)
            {
                CreateControlMap(mapping.Key, mapping.Value.key1, mapping.Value.key2);
            }
        }

        private void ResetDefaults()
        {
            CreateControlMap(Controls.MoveUp, Keys.W, Keys.Up);
            CreateControlMap(Controls.MoveDown, Keys.S, Keys.Down);
            CreateControlMap(Controls.MoveLeft, Keys.A, Keys.Left);
            CreateControlMap(Controls.MoveRight, Keys.D, Keys.Right);
            CreateControlMap(Controls.AttackInteract, Keys.E, Keys.LButton);
            CreateControlMap(Controls.Block, Keys.Q, Keys.RButton);
            CreateControlMap(Controls.PickUp, Keys.Space, Keys.None);
            CreateControlMap(Controls.Enter, Keys.Enter, Keys.None);
            CreateControlMap(Controls.Hotkey1, Keys.D1, Keys.None);
            CreateControlMap(Controls.Hotkey2, Keys.D2, Keys.None);
            CreateControlMap(Controls.Hotkey3, Keys.D3, Keys.None);
            CreateControlMap(Controls.Hotkey4, Keys.D4, Keys.None);
            CreateControlMap(Controls.Hotkey5, Keys.D5, Keys.None);
            CreateControlMap(Controls.Hotkey6, Keys.D6, Keys.None);
            CreateControlMap(Controls.Hotkey7, Keys.D7, Keys.None);
            CreateControlMap(Controls.Hotkey8, Keys.D8, Keys.None);
            CreateControlMap(Controls.Hotkey9, Keys.D9, Keys.None);
            CreateControlMap(Controls.Hotkey0, Keys.D0, Keys.None);
        }

        public void Save()
        {
            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var name = Enum.GetName(typeof(Controls), control);
                Globals.Database.SavePreference(name + "_key1", ((int) ControlMapping[control].key1).ToString());
                Globals.Database.SavePreference(name + "_key2", ((int) ControlMapping[control].key2).ToString());
            }
        }

        public static void Init()
        {
            ActiveControls = new GameControls();
        }

        public static bool KeyDown(Controls control)
        {
            if (ActiveControls.ControlMapping.ContainsKey(control))
            {
                if (ActiveControls.ControlMapping[control].KeyDown()) return true;
            }
            return false;
        }

        public static bool ControlHasKey(Controls control, Keys key)
        {
            if (ActiveControls.ControlMapping.ContainsKey(control))
            {
                if (ActiveControls.ControlMapping[control].key1 == key ||
                    ActiveControls.ControlMapping[control].key2 == key)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateControl(Controls control, int keyNum, Keys key)
        {
            if (keyNum == 1)
            {
                ControlMapping[control].key1 = key;
            }
            else
            {
                ControlMapping[control].key2 = key;
            }
        }

        private void CreateControlMap(Controls control, Keys key1, Keys key2)
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

    public class ControlMap
    {
        public Keys key1;
        public Keys key2;

        public ControlMap(Controls control, Keys key1, Keys key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }

        public bool KeyDown()
        {
            if (key1 != Keys.None && Globals.InputManager.KeyDown(key1)) return true;
            if (key2 != Keys.None && Globals.InputManager.KeyDown(key2)) return true;
            return false;
        }
    }
}