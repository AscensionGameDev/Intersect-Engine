/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class ItemDescWindow
    {
        ImagePanel _descWindow;
        public ItemDescWindow(int itemnum, int amount, int x, int y, int[] StatBuffs, string titleOverride = "", string valueLabel = "")
        {
            string title = "";
            if (titleOverride == "")
                title = ItemBase.GetName(itemnum);
            else
                title = titleOverride;

            _descWindow = new ImagePanel(Gui.GameUI.GameCanvas);
            _descWindow.SetSize(255, 260);
            _descWindow.Margin = Margin.Zero;
            _descWindow.Padding = new Padding(8, 5, 9, 11);
            _descWindow.SetPosition(x, y);
            _descWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,"itemdescpanel.png");

            y = 12;


            var item = ItemBase.GetItem(itemnum);
            if (item != null)
            {
                var innery = 8;
                ImagePanel icon = new ImagePanel(_descWindow);
                icon.SetSize(32, 32);
                icon.SetPosition(240 - 4 - 32, innery);
                GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                    item.Pic);
                if (itemTex != null)
                {
                    icon.Texture = itemTex;
                }

                Label itemName = new Label(_descWindow);
                itemName.SetPosition(4, innery);
                itemName.Text = title;
                itemName.SetTextColor(IntersectClientExtras.GenericClasses.Color.White, Label.ControlState.Normal);
                itemName.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);

                innery += 18;
                if (amount > 1)
                {
                    itemName.Text += " " + Strings.Get("itemdesc","quantity",amount);
                }

                Align.CenterHorizontally(itemName);

                Label itemType = new Label(_descWindow);
                itemType.SetPosition(4, innery);
                itemType.SetTextColor(IntersectClientExtras.GenericClasses.Color.White, Label.ControlState.Normal);
                itemType.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 10);
                innery += 16;

                if ( valueLabel != "")
                {
                    Label itemValue = new Label(_descWindow);
                    itemValue.SetPosition(4, innery);
                    itemValue.SetText(valueLabel);
                    itemValue.SetTextColor(IntersectClientExtras.GenericClasses.Color.White, Label.ControlState.Normal);
                    Align.CenterHorizontally(itemValue);
                    innery += 12;
                }

                itemType.Text = Strings.Get("itemdesc", "itemtype" + item.ItemType);

                y += innery + 2;
                if (item.ItemType == (int)ItemTypes.Equipment)
                {
                    itemType.Text = Options.EquipmentSlots[item.Data1];
                    if (item.Data1 == Options.WeaponIndex && Convert.ToBoolean(item.Data4) == true)
                    {
                        itemType.Text += " - " + Strings.Get("itemdesc","2hand");
                    }
                }
                RichLabel itemDesc = new RichLabel(_descWindow);
                itemDesc.SetPosition(_descWindow.Padding.Left + 4, y);
                itemDesc.Width = 240-4;
                //itemDesc.SetBounds(4, y, 180, 10);
                if (item.Desc.Length > 0)
                {
                    itemDesc.AddText(Strings.Get("itemdesc","desc",item.Desc), IntersectClientExtras.GenericClasses.Color.White);
                }
                itemDesc.SizeToChildren(false, true);

                y += itemDesc.Height + 8;
                int y1 = y;

                string stats = "";
                if (item.ItemType == (int)ItemTypes.Equipment)
                {
                    RichLabel itemStats = new RichLabel(_descWindow);
                    itemStats.SetPosition(_descWindow.Padding.Left + 4, y);
                    itemStats.Width = 240;
                    stats = Strings.Get("itemdesc","bonuses");
                    itemStats.AddText(stats, IntersectClientExtras.GenericClasses.Color.White);
                    itemStats.AddLineBreak();
                    if (item.ItemType == (int)ItemTypes.Equipment && item.Data1 == Options.WeaponIndex)
                    {
                        stats = Strings.Get("itemdesc","damage",item.Damage);
                        itemStats.AddText(stats, IntersectClientExtras.GenericClasses.Color.White);
                        itemStats.AddLineBreak();
                    }
                    if (StatBuffs != null)
                    {
                        for (int i = 0; i < Options.MaxStats; i++)
                        {
                            stats = Strings.Get("combat", "stat" + i) + ": " +
                                    (item.StatsGiven[i] + StatBuffs[i]) + "";
                            itemStats.AddText(stats, IntersectClientExtras.GenericClasses.Color.White);
                            itemStats.AddLineBreak();
                        }
                    }

                    itemStats.SizeToChildren(false, true);
                    y += itemStats.Height + 4;

                }

                if (y1 > y) { y = y1; }

                y += 6;
                if (item.ItemType == (int)ItemTypes.Equipment && item.Data2 > 0 && item.Data3 > 0)
                {
                    Label bonusLabel = new Label(_descWindow);
                    bonusLabel.SetPosition(_descWindow.Padding.Left + 4, y);
                    bonusLabel.TextColorOverride = IntersectClientExtras.GenericClasses.Color.White;
                    bonusLabel.Text = Strings.Get("itemdesc","effect", item.Data3, Strings.Get("itemdesc","effect" + (item.Data2 - 1)));
                    y += 24;
                }

                Align.CenterHorizontally(itemType);
            }

        }

        public void Dispose()
        {
            Gui.GameUI.GameCanvas.RemoveChild(_descWindow,false);
            _descWindow.Dispose();
        }
    }
}
