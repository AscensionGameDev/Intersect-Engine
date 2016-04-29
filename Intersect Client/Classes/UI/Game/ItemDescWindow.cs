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


namespace Intersect_Client.Classes.UI.Game
{
    public class ItemDescWindow
    {
        WindowControl _descWindow;
        public ItemDescWindow(int itemnum, int amount, int x, int y, int[] StatBuffs, string titleOverride = "", string valueLabel = "")
        {
            string title = "";
            if (titleOverride == "")
                title = Globals.GameItems[itemnum].Name;
            else
                title = titleOverride;

            _descWindow = new WindowControl(Gui.GameUI.GameCanvas, title, false);
            _descWindow.SetSize(220, 100);
            _descWindow.IsClosable = false;
            _descWindow.DisableResizing();
            _descWindow.Margin = Margin.Zero;
            _descWindow.Padding = Padding.Zero;
            _descWindow.SetPosition(x, y);

            y = 12;



            if (itemnum > -1)
            {
                var innery = 4;
                ImagePanel icon = new ImagePanel(_descWindow);
                icon.SetSize(32, 32);
                icon.SetPosition(220 - 4 - 32, 4);
                GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                    Globals.GameItems[itemnum].Pic);
                if (itemTex != null)
                {
                    icon.Texture = itemTex;
                }

                Label itemName = new Label(_descWindow);
                itemName.SetPosition(4, innery);
                itemName.Text = Globals.GameItems[itemnum].Name;

                innery += 12;
                if (amount > 1)
                {
                    itemName.Text += " x" + amount;
                }

                if ( valueLabel != "")
                {
                    Label itemValue = new Label(_descWindow);
                    itemValue.SetPosition(4, innery);
                    itemValue.SetText(valueLabel);
                    innery += 12;
                }

                if (Globals.GameItems[itemnum].Type != (int)ItemTypes.None)
                {
                    Label itemType = new Label(_descWindow);
                    itemType.SetPosition(4, innery);
                    switch (Globals.GameItems[itemnum].Type)
                    {
                        case (int)ItemTypes.Currency:
                            itemType.Text = "Type: Currency";
                            break;
                        case (int)ItemTypes.Equipment:
                            itemType.Text = "Type: Equipment";
                            break;
                        case (int)ItemTypes.Consumable:
                            itemType.Text = "Type: Consume";
                            break;
                        case (int)ItemTypes.Spell:
                            itemType.Text = "Type: Spell";
                            break;
                    }
                    innery += 12;
                }

                y += innery + 4;
                if (Globals.GameItems[itemnum].Type == (int)ItemTypes.Equipment)
                {
                    Label itemSlot = new Label(_descWindow);
                    itemSlot.Text = "Slot: " + Options.EquipmentSlots[Globals.GameItems[itemnum].Data1];
                    if (Globals.GameItems[itemnum].Data1 == Options.WeaponIndex && Convert.ToBoolean(Globals.GameItems[itemnum].Data4) == true)
                    {
                        itemSlot.Text += " - 2H";
                    }
                    itemSlot.SetPosition(4, y);
                    y += 12;
                }
                RichLabel itemDesc = new RichLabel(_descWindow);
                itemDesc.SetPosition(4, y);
                itemDesc.Width = 210;
                //itemDesc.SetBounds(4, y, 180, 10);
                if (Globals.GameItems[itemnum].Desc.Length > 0)
                {
                    itemDesc.AddText("Desc: " + Globals.GameItems[itemnum].Desc, itemName.TextColor);
                }
                itemDesc.SizeToChildren(false, true);

                y += itemDesc.Height + 8;
                int y1 = y;

                bool requirements = false;
                if (Globals.GameItems[itemnum].Type != (int)ItemTypes.Currency && Globals.GameItems[itemnum].Type != (int)ItemTypes.None)
                {
                    //Check for requirements
                    RichLabel itemReqs = new RichLabel(_descWindow);
                    itemReqs.Width = 110;
                    itemReqs.AddText("Prerequisites", itemName.TextColor);
                    itemReqs.AddLineBreak();
                    itemReqs.SetPosition(4, y);
                    if (Globals.GameItems[itemnum].LevelReq > 0)
                    {
                        requirements = true;
                        itemReqs.AddText("Level: " + Globals.GameItems[itemnum].LevelReq, itemName.TextColor);
                        itemReqs.AddLineBreak();
                    }
                    for (int i = 0; i < Options.MaxStats; i++)
                    {
                        if (Globals.GameItems[itemnum].StatsReq[i] > 0)
                        {
                            requirements = true;
                            itemReqs.AddText(Globals.GetStatName(i) + ": " + Globals.GameItems[itemnum].StatsReq[i], itemName.TextColor);
                            itemReqs.AddLineBreak();
                        }
                    }
                    if (requirements == true)
                    {
                        itemReqs.SizeToChildren(false, true);
                        y1 += itemReqs.Height + 4;
                    }
                    else
                    {
                        itemReqs.IsHidden = true;
                    }
                }

                string stats = "";
                if (Globals.GameItems[itemnum].Type == (int)ItemTypes.Equipment)
                {
                    RichLabel itemStats = new RichLabel(_descWindow);
                    if (requirements != true)
                    {
                        itemStats.SetPosition(4, y);
                        itemStats.Width = 210;
                    }
                    else
                    {
                        itemStats.SetPosition(110, y);
                        itemStats.Width = 110;
                    }
                    stats = "Stats Bonuses:";
                    itemStats.AddText(stats, itemName.TextColor);
                    itemStats.AddLineBreak();
                    if (Globals.GameItems[itemnum].Type == (int)ItemTypes.Equipment && Globals.GameItems[itemnum].Data1 == Options.WeaponIndex)
                    {
                        stats = "Base Damage" + ": " + (Globals.GameItems[itemnum].Damage) + "";
                        itemStats.AddText(stats, itemName.TextColor);
                        itemStats.AddLineBreak();
                    }
                    if (StatBuffs != null)
                    {
                        for (int i = 0; i < Options.MaxStats; i++)
                        {
                            stats = Globals.GetStatName(i) + ": " +
                                    (Globals.GameItems[itemnum].StatsGiven[i] + StatBuffs[i]) + "";
                            itemStats.AddText(stats, itemName.TextColor);
                            itemStats.AddLineBreak();
                        }
                    }

                    itemStats.SizeToChildren(false, true);
                    y += itemStats.Height + 4;

                }

                if (y1 > y) { y = y1; }

                if (Globals.GameItems[itemnum].Type == (int)ItemTypes.Equipment && Globals.GameItems[itemnum].Data2 > 0 && Globals.GameItems[itemnum].Data3 > 0)
                {
                    Label bonusLabel = new Label(_descWindow);
                    bonusLabel.SetPosition(4, y);
                    bonusLabel.Text = "Bonus Effect: " + Globals.GameItems[itemnum].Data3 + "% " + (Globals.GameItems[itemnum].Data2 - 1 == 0 ? "Cooldown Reduction" : "Lifesteal");
                    y += 24;
                }
            }
            _descWindow.SetSize(220, y + 24);

        }

        public void Dispose()
        {
            _descWindow.Close();
            Gui.GameUI.GameCanvas.RemoveChild(_descWindow,false);
            _descWindow.Dispose();
        }
    }
}
