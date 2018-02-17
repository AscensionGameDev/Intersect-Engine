using System;
using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using JetBrains.Annotations;

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
        Screenshot,
        OpenMenu,
        OpenInventory,
        OpenQuests,
        OpenCharacterInfo,
        OpenParties,
        OpenSpells,
        OpenFriends,
        OpenSettings,
    }

    public class GameControls
    {
        public static GameControls ActiveControls { get; set; }

        [NotNull] public readonly IDictionary<Controls, ControlMap> ControlMapping;

        public GameControls(GameControls gameControls = null)
        {
            ControlMapping = new Dictionary<Controls, ControlMap>();

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
                foreach (Controls control in Enum.GetValues(typeof(Controls)))
                {
                    var name = Enum.GetName(typeof(Controls), control);
                    var key1 = Globals.Database.LoadPreference(name + "_key1");
                    var key2 = Globals.Database.LoadPreference(name + "_key2");
                    if (string.IsNullOrEmpty(key1) || string.IsNullOrEmpty(key2))
                    {
                        Globals.Database.SavePreference(name + "_key1", ((int)ControlMapping[control].Key1).ToString());
                        Globals.Database.SavePreference(name + "_key2", ((int)ControlMapping[control].Key2).ToString());
                    }
                    else
                    {
                        CreateControlMap(control, (Keys)Convert.ToInt32(key1), (Keys)Convert.ToInt32(key2));
                    }
                }
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
            CreateControlMap(Controls.Screenshot, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenMenu, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenInventory, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenQuests, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenCharacterInfo, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenParties, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenSpells, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenFriends, Keys.None, Keys.None);
            CreateControlMap(Controls.OpenSettings, Keys.None, Keys.None);
        }

        public void Save()
        {
            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var name = Enum.GetName(typeof(Controls), control);
                Globals.Database.SavePreference(name + "_key1", ((int)ControlMapping[control].Key1).ToString());
                Globals.Database.SavePreference(name + "_key2", ((int)ControlMapping[control].Key2).ToString());
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
                if (ActiveControls.ControlMapping[control].Key1 == key ||
                    ActiveControls.ControlMapping[control].Key2 == key)
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
                ControlMapping[control].Key1 = key;
            }
            else
            {
                ControlMapping[control].Key2 = key;
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
        public Keys Key1;
        public Keys Key2;

        public ControlMap(Controls control, Keys key1, Keys key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        public bool KeyDown()
        {
            if (Key1 != Keys.None && Globals.InputManager.KeyDown(Key1)) return true;
            if (Key2 != Keys.None && Globals.InputManager.KeyDown(Key2)) return true;
            if (!Gui.MouseHitGui())
            {
                switch (Key1)
                {
                    case Keys.LButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)) return true;
                        break;
                    case Keys.RButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Right)) return true;
                        break;
                    case Keys.MButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Middle)) return true;
                        break;
                }
                switch (Key2)
                {
                    case Keys.LButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)) return true;
                        break;
                    case Keys.RButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Right)) return true;
                        break;
                    case Keys.MButton:
                        if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Middle)) return true;
                        break;
                }
            }
            return false;
        }
    }
}