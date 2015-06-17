using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class ItemDescWindow
    {
        WindowControl _descWindow;
        public ItemDescWindow(int itemnum, int amount, int x, int y, int[] StatBuffs)
        {
            _descWindow = new WindowControl(Gui._GameGui.GameCanvas, Globals.GameItems[itemnum].Name, false);
            _descWindow.SetSize(180, 100);
            _descWindow.IsClosable = false;
            _descWindow.DisableResizing();
            _descWindow.Margin = Margin.Zero;
            _descWindow.Padding = Padding.Zero;
            _descWindow.SetPosition(x, y);

            ImagePanel icon = new ImagePanel(_descWindow);
            icon.SetSize(32, 32);
            icon.SetPosition(180-4-32, 4);
            icon.ImageName = "Resources/Items/" + Globals.GameItems[itemnum].Pic;

            Label itemName = new Label(_descWindow);
            itemName.SetPosition(4, 4);
            itemName.Text = Globals.GameItems[itemnum].Name;

            if (amount > 1)
            {
                itemName.Text += " x" + amount;
            }

            if (Globals.GameItems[itemnum].Type != (int)Enums.ItemTypes.None)
            {
                Label itemType = new Label(_descWindow);
                itemType.SetPosition(4, 16);
                switch (Globals.GameItems[itemnum].Type)
                {
                    case (int)Enums.ItemTypes.Currency:
                        itemType.Text = "Type: Currency";
                        break;
                    case (int)Enums.ItemTypes.Armor:
                        itemType.Text = "Type: Armor";
                        break;
                    case (int)Enums.ItemTypes.Consumable:
                        itemType.Text = "Type: Consume";
                        break;
                    case (int)Enums.ItemTypes.Helmet:
                        itemType.Text = "Type: Helmet";
                        break;
                    case (int)Enums.ItemTypes.Shield:
                        itemType.Text = "Type: Shield";
                        break;
                    case (int)Enums.ItemTypes.Spell:
                        itemType.Text = "Type: Spell";
                        break;
                    case (int)Enums.ItemTypes.Weapon:
                        itemType.Text = "Type: Weapon";
                        break;
                }
            }

            y = 32;
            Gwen.Control.RichLabel itemDesc = new Gwen.Control.RichLabel(_descWindow);
            itemDesc.SetPosition(4, y);
            itemDesc.Width = 170;
            //itemDesc.SetBounds(4, y, 180, 10);
            if (Globals.GameItems[itemnum].Desc.Length > 0)
            {
                itemDesc.AddText("Desc: " + Globals.GameItems[itemnum].Desc,itemName.TextColor);
            }
            itemDesc.SizeToChildren(false, true);

            y += itemDesc.Height + 4;
            int y1 = y;

            bool requirements = false;
            if (Globals.GameItems[itemnum].Type != (int)Enums.ItemTypes.Currency && Globals.GameItems[itemnum].Type != (int)Enums.ItemTypes.None){
                //Check for requirements
                Gwen.Control.RichLabel itemReqs = new Gwen.Control.RichLabel(_descWindow);
                itemReqs.Width = 90;
                itemReqs.AddText("Prerequisites", itemName.TextColor);
                itemReqs.AddLineBreak();
                itemReqs.SetPosition(4, y);
                if (Globals.GameItems[itemnum].LevelReq > 0) {
                    requirements = true;
                    itemReqs.AddText("Level: " + Globals.GameItems[itemnum].LevelReq, itemName.TextColor);
                    itemReqs.AddLineBreak();
                }
                for (int i = 0; i < Constants.MaxStats; i++){
                    if (Globals.GameItems[itemnum].StatsReq[i] > 0) {
                        requirements = true;
                        itemReqs.AddText(Enums.GetStatName(i) + ": " + Globals.GameItems[itemnum].StatsReq[i], itemName.TextColor);
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
            if (Globals.GameItems[itemnum].Type >= (int)Enums.ItemTypes.Weapon && Globals.GameItems[itemnum].Type <= (int)Enums.ItemTypes.Shield)
            {
                Gwen.Control.RichLabel itemStats = new Gwen.Control.RichLabel(_descWindow);
                if (requirements != true)
                {
                    itemStats.SetPosition(4, y);
                    itemStats.Width = 170;
                }
                else
                {
                    itemStats.SetPosition(90, y);
                    itemStats.Width = 90;
                }
                stats = "Stats Bonuses:";
                itemStats.AddText(stats, itemName.TextColor);
                itemStats.AddLineBreak();
                if (Globals.GameItems[itemnum].Type == (int)Enums.ItemTypes.Weapon)
                {
                    stats = "Base Damage" + ": " + (Globals.GameItems[itemnum].Damage) + "";
                    itemStats.AddText(stats, itemName.TextColor);
                    itemStats.AddLineBreak();
                }
                for (int i = 0; i < Constants.MaxStats; i++)
                {
                    stats = Enums.GetStatName(i) + ": " + (Globals.GameItems[itemnum].StatsGiven[i] + StatBuffs[i]) + "";
                    itemStats.AddText(stats, itemName.TextColor);
                    itemStats.AddLineBreak();
                }
                
                itemStats.SizeToChildren(false, true);
                y += itemStats.Height + 4;

            }

            if (y1 > y) { y = y1; }
            _descWindow.SetSize(180, y + 24);

        }

        public void Dispose()
        {
            _descWindow.Close();
            Gui._GameGui.GameCanvas.RemoveChild(_descWindow,false);
            _descWindow.Dispose();
        }
    }
}
