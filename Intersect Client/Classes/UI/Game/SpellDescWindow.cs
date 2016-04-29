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

using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Intersect_Library;
using Options = Intersect_Client.Classes.General.Options;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellDescWindow
    {
        WindowControl _descWindow;
        public SpellDescWindow(int spellnum, int x, int y)
        {
            if (spellnum == -1) { return; }
            _descWindow = new WindowControl(Gui.GameUI.GameCanvas, Globals.GameSpells[spellnum].Name, false);
            _descWindow.SetSize(220, 100);
            _descWindow.IsClosable = false;
            _descWindow.DisableResizing();
            _descWindow.Margin = Margin.Zero;
            _descWindow.Padding = Padding.Zero;
            _descWindow.SetPosition(x, y);

            ImagePanel icon = new ImagePanel(_descWindow);
            icon.SetSize(32, 32);
            icon.SetPosition(220 - 4 - 32, 4);
            icon.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, Globals.GameSpells[spellnum].Pic);

            Label spellName = new Label(_descWindow);
            spellName.SetPosition(4, 4);
            spellName.Text = Globals.GameSpells[spellnum].Name;

            Label spellType = new Label(_descWindow);
            spellType.SetPosition(4, 16);
            if (Globals.GameSpells[spellnum].Type == (int)SpellTypes.CombatSpell)
            {
                spellType.Text = "Combat Spell";
            }
            else if (Globals.GameSpells[spellnum].Type == (int)SpellTypes.Warp)
            {
                spellType.Text = "Warp - Self Cast";
            }

            y = 32;

            RichLabel castInfo = new RichLabel(_descWindow);
            castInfo.SetPosition(4, y);
            castInfo.Width = 140;

            if (Globals.GameSpells[spellnum].Type == (int)SpellTypes.CombatSpell)
            {
                switch (Globals.GameSpells[spellnum].TargetType)
                {
                    case (int)SpellTargetTypes.Self:
                        castInfo.AddText("Target: Self", spellName.TextColor);
                        break;
                    case (int)SpellTargetTypes.Single:
                        castInfo.AddText("Target: Targetted", spellName.TextColor);
                        castInfo.AddLineBreak();
                        castInfo.AddText("Range: " + Globals.GameSpells[spellnum].CastRange + " Tiles", spellName.TextColor);
                        break;
                    case (int)SpellTargetTypes.AoE:
                        castInfo.AddText("Target: AOE", spellName.TextColor);
                        castInfo.AddLineBreak();
                        castInfo.AddText("Range: " + Globals.GameSpells[spellnum].CastRange + " Tiles", spellName.TextColor);
                        castInfo.AddLineBreak();
                        castInfo.AddText("Radius: " + Globals.GameSpells[spellnum].HitRadius + " Tiles", spellName.TextColor);
                        break;
                    case (int)SpellTargetTypes.Projectile:
                        castInfo.AddText("Target: Projectile", spellName.TextColor);
                        castInfo.AddLineBreak();
                        castInfo.AddText("Range: " + Globals.GameSpells[spellnum].CastRange + " Tiles", spellName.TextColor);
                        break;
                }
                castInfo.AddLineBreak();
            }
            if (Globals.GameSpells[spellnum].CastDuration > 0)
            {
                castInfo.AddText("Cast Time: " + ((float)Globals.GameSpells[spellnum].CastDuration / 10f) + " Seconds", spellName.TextColor);
                castInfo.AddLineBreak();
            }
            if (Globals.GameSpells[spellnum].CooldownDuration > 0)
            {
                castInfo.AddText("Cooldown: " + ((float)Globals.GameSpells[spellnum].CooldownDuration / 10f) + " Seconds", spellName.TextColor);
                castInfo.AddLineBreak();
            }
            castInfo.SizeToChildren(false, true);
            y += castInfo.Height + 8;


            RichLabel spellDesc = new RichLabel(_descWindow);
            spellDesc.SetPosition(4, y);
            spellDesc.Width = 210;
            //itemDesc.SetBounds(4, y, 180, 10);
            if (Globals.GameSpells[spellnum].Desc.Length > 0)
            {
                spellDesc.AddText("Desc: " + Globals.GameSpells[spellnum].Desc, spellName.TextColor);
            }
            spellDesc.SizeToChildren(false, true);

            y += spellDesc.Height + 8;
            int y1 = y;

            bool requirements = false;

            //Check for requirements
            RichLabel itemReqs = new RichLabel(_descWindow);
            itemReqs.Width = 110;
            itemReqs.AddText("Prerequisites", spellName.TextColor);
            itemReqs.AddLineBreak();
            itemReqs.SetPosition(4, y);
            if (Globals.GameSpells[spellnum].VitalCost[(int)Vitals.Health] > 0)
            {
                requirements = true;
                itemReqs.AddText("HP Cost: " + Globals.GameSpells[spellnum].VitalCost[(int)Vitals.Health], spellName.TextColor);
                itemReqs.AddLineBreak();
            }
            if (Globals.GameSpells[spellnum].VitalCost[(int)Vitals.Mana] > 0)
            {
                requirements = true;
                itemReqs.AddText("MP Cost: " + Globals.GameSpells[spellnum].VitalCost[(int)Vitals.Mana], spellName.TextColor);
                itemReqs.AddLineBreak();
            }
            if (Globals.GameSpells[spellnum].LevelReq > 0)
            {
                requirements = true;
                itemReqs.AddText("Level: " + Globals.GameSpells[spellnum].LevelReq, spellName.TextColor);
                itemReqs.AddLineBreak();
            }
            for (int i = 0; i < Options.MaxStats; i++)
            {
                if (Globals.GameSpells[spellnum].StatReq[i] > 0)
                {
                    requirements = true;
                    itemReqs.AddText(Globals.GetStatName(i) + ": " + Globals.GameSpells[spellnum].StatReq[i], spellName.TextColor);
                    itemReqs.AddLineBreak();
                }
            }
            if (requirements == true)
            {
                itemReqs.SizeToChildren(false, true);
                y1 += itemReqs.Height + 8;
            }
            else
            {
                itemReqs.IsHidden = true;
            }

            string stats = "";
            if (Globals.GameSpells[spellnum].Type == (int)SpellTypes.CombatSpell)
            {
                RichLabel spellStats = new RichLabel(_descWindow);
                if (requirements != true)
                {
                    spellStats.SetPosition(4, y);
                    spellStats.Width = 220;
                }
                else
                {
                    spellStats.SetPosition(110, y);
                    spellStats.Width = 110;
                }
                stats = "Info:";
                spellStats.AddText(stats, spellName.TextColor);
                spellStats.AddLineBreak();

                if (Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Health] != 0)
                {
                    if (Globals.GameSpells[spellnum].Data1 == 1 && Globals.GameSpells[spellnum].Data2 > 0)
                    {
                        stats = "HP:" + ((float)Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Health] / (float)Globals.GameSpells[spellnum].Data2) + " / sec";

                    }
                    else
                    {
                        stats = "HP: " + Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Health];
                    }
                    spellStats.AddText(stats, spellName.TextColor);
                    spellStats.AddLineBreak();
                }

                if (Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Mana] != 0)
                {
                    if (Globals.GameSpells[spellnum].Data1 == 1 && Globals.GameSpells[spellnum].Data2 > 0)
                    {
                        stats = "MP:" + ((float)Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Mana] / (float)Globals.GameSpells[spellnum].Data2) + " / sec";
                    }
                    else
                    {
                        stats = "MP: " + Globals.GameSpells[spellnum].VitalDiff[(int)Vitals.Mana];
                    }
                    spellStats.AddText(stats, spellName.TextColor);
                    spellStats.AddLineBreak();
                }

                if (Globals.GameSpells[spellnum].Data2 > 0)
                {
                    for (int i = 0; i < Options.MaxStats; i++)
                    {
                        if (Globals.GameSpells[spellnum].StatDiff[i] != 0)
                        {
                            spellStats.AddText(Globals.GetStatName(i) + ": " + Globals.GameSpells[spellnum].StatDiff[i], spellName.TextColor);
                            spellStats.AddLineBreak();
                        }
                    }
                    switch (Globals.GameSpells[spellnum].Data3)
                    {
                        case 1:
                            spellStats.AddText("Silences Target", spellName.TextColor);
                            spellStats.AddLineBreak();
                            break;
                        case 2:
                            spellStats.AddText("Stuns Target", spellName.TextColor);
                            spellStats.AddLineBreak();
                            break;
                    }
                    spellStats.AddText("Effect Duration: " + ((float)Globals.GameSpells[spellnum].Data2 / 10f) + " seconds", spellName.TextColor);
                    spellStats.AddLineBreak();
                }



                spellStats.SizeToChildren(false, true);
                y += spellStats.Height + 4;

            }

            if (y1 > y) { y = y1; }
            _descWindow.SetSize(220, y + 24);

        }

        public void Dispose()
        {
            if (_descWindow == null) { return; }
            _descWindow.Close();
            Gui.GameUI.GameCanvas.RemoveChild(_descWindow, false);
            _descWindow.Dispose();
        }
    }
}
