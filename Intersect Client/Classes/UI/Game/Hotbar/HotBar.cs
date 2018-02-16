using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.UI.Game.Hotbar;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class HotBarWindow
    {
        //Controls
        public ImagePanel HotbarWindow;

        //Item List
        public List<HotbarItem> Items = new List<HotbarItem>();

        //Init
        public HotBarWindow(Canvas gameCanvas)
        {
            HotbarWindow = new ImagePanel(gameCanvas, "HotbarWindow");
            InitHotbarItems();
            HotbarWindow.LoadJsonUi(GameContentManager.UI.InGame);
        }

        private void InitHotbarItems()
        {
            int x = 12;
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Items.Add(new HotbarItem(i, HotbarWindow));
                Items[i].Pnl = new ImagePanel(HotbarWindow, "HotbarContainer" + i);
                Items[i].KeyLabel = new Label(Items[i].Pnl, "HotbarLabel" + i);
                Items[i].KeyLabel.SetText(Strings.Keys.keydict[Enum.GetName(typeof(Keys), GameControls.ActiveControls.ControlMapping[Controls.Hotkey1 + i].Key1).ToLower()]);
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
                X = HotbarWindow.LocalPosToCanvas(new Point(0, 0)).X,
                Y = HotbarWindow.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = HotbarWindow.Width,
                Height = HotbarWindow.Height
            };
            return rect;
        }
    }
}