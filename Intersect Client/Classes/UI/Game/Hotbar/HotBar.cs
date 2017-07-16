using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.UI.Game.Hotbar;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class HotBarWindow
    {
        //Controls
        public ImagePanel _hotbarWindow;

        //Item List
        public List<HotbarItem> Items = new List<HotbarItem>();

        //Init
        public HotBarWindow(Canvas _gameCanvas)
        {
            _hotbarWindow = new ImagePanel(_gameCanvas,"HotbarWindow");
            InitHotbarItems();
        }

        private void InitHotbarItems()
        {
            int x = 12;
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Items.Add(new HotbarItem(i, _hotbarWindow));
                Items[i].pnl = new ImagePanel(_hotbarWindow,"HotbarContainer" + i);
                Items[i].keyLabel = new Label(Items[i].pnl, "HotbarLabel" + i);
                Items[i].keyLabel.SetText(Strings.Get("keys", Enum.GetName(typeof(Keys), GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + i].key1)));
                Items[i].Setup();
            }
        }

        public void Update()
        {
            if (Globals.Me == null)
            {
                return;
            }
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Items[i].Update();
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _hotbarWindow.LocalPosToCanvas(new Point(0, 0)).X,
                Y = _hotbarWindow.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = _hotbarWindow.Width,
                Height = _hotbarWindow.Height
            };
            return rect;
        }
    }
}