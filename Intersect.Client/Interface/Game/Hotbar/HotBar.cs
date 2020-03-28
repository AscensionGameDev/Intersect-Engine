using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.Hotbar
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
            HotbarWindow.ShouldCacheToTexture = true;
            InitHotbarItems();
            HotbarWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].EquipPanel.Texture == null)
                {
                    Items[i].EquipPanel.Texture = Graphics.Renderer.GetWhiteTexture();
                }
            }
        }

        private void InitHotbarItems()
        {
            var x = 12;
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                Items.Add(new HotbarItem((byte) i, HotbarWindow));
                Items[i].Pnl = new ImagePanel(HotbarWindow, "HotbarContainer" + i);
                Items[i].Setup();
                Items[i].KeyLabel = new Label(Items[i].Pnl, "HotbarLabel" + i);
                Items[i]
                    .KeyLabel.SetText(
                        Strings.Keys.keydict[
                            Enum.GetName(typeof(Keys), Controls.ActiveControls.ControlMapping[Control.Hotkey1 + i].Key1)
                                .ToLower()]
                    );
            }
        }

        public void Update()
        {
            if (Globals.Me == null)
            {
                return;
            }

            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                Items[i].Update();
            }
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
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
