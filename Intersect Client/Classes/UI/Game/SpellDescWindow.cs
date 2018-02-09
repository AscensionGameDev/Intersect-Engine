using System;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellDescWindow
    {
        ImagePanel mDescWindow;

        public SpellDescWindow(int spellnum, int x, int y)
        {
            var spell = SpellBase.Lookup.Get<SpellBase>(spellnum);
            if (spell == null)
            {
                return;
            }
            mDescWindow = new ImagePanel(Gui.GameUi.GameCanvas, "SpellDescWindow");

            ImagePanel icon = new ImagePanel(mDescWindow, "SpellIcon");
            icon.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Pic);

            Label spellName = new Label(mDescWindow, "SpellName");
            spellName.Text = spell.Name;

            Label spellType = new Label(mDescWindow, "SpellType");
            spellType.Text = Strings.Get("spelldesc", "spelltype" + spell.SpellType);

            RichLabel spellDesc = new RichLabel(mDescWindow, "SpellDesc");
            Gui.LoadRootUiData(mDescWindow,
                "InGame.xml"); //Load this up now so we know what color to make the text when filling out the desc
            if (spell.Desc.Length > 0)
            {
                spellDesc.AddText(Strings.Get("spelldesc", "desc", spell.Desc), spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }

            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                spellType.Text = Strings.Get("spelldesc", "targettype" + spell.TargetType, spell.CastRange,
                    spell.HitRadius);
            }
            if (spell.CastDuration > 0)
            {
                spellDesc.AddText(Strings.Get("spelldesc", "casttime", ((float) spell.CastDuration / 10f)),
                    spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }
            if (spell.CooldownDuration > 0)
            {
                spellDesc.AddText(Strings.Get("spelldesc", "cooldowntime", ((float) spell.CooldownDuration / 10f)),
                    spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }

            bool requirements = (spell.VitalCost[(int) Vitals.Health] > 0 || spell.VitalCost[(int) Vitals.Mana] > 0);

            if (requirements == true)
            {
                spellDesc.AddText(Strings.Get("spelldesc", "prereqs"), spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                if (spell.VitalCost[(int) Vitals.Health] > 0)
                {
                    spellDesc.AddText(Strings.Get("spelldesc", "vital0cost", spell.VitalCost[(int) Vitals.Health]),
                        spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
                if (spell.VitalCost[(int) Vitals.Mana] > 0)
                {
                    spellDesc.AddText(Strings.Get("spelldesc", "vital1cost", spell.VitalCost[(int) Vitals.Mana]),
                        spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
                spellDesc.AddLineBreak();
            }

            string stats = "";
            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                stats = Strings.Get("spelldesc", "effects");
                spellDesc.AddText(stats, spellDesc.RenderColor);
                spellDesc.AddLineBreak();

                if (spell.Data3 > 0)
                {
                    spellDesc.AddText(Strings.Get("spelldesc", "effect" + spell.Data3), spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }

                for (var i = 0; i < (int) Vitals.VitalCount; i++)
                {
                    var vitalDiff = spell.VitalDiff?[i] ?? 0;
                    if (vitalDiff == 0) continue;
                    var vitalSymbol = Strings.Get("spelldesc", $"{(vitalDiff < 0 ? "add" : "remove")}symbol");
                    stats = Strings.Get("spelldesc", $"vital{i}", vitalSymbol, Math.Abs(vitalDiff));
                    spellDesc.AddText(stats, spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }

                if (spell.Data2 > 0)
                {
                    for (int i = 0; i < Options.MaxStats; i++)
                    {
                        if (spell.StatDiff[i] != 0)
                        {
                            spellDesc.AddText(
                                Strings.Get("combat", "stat" + i) + ": " + (spell.StatDiff[i] > 0 ? "+ " : "") +
                                spell.StatDiff[i], spellDesc.RenderColor);
                            spellDesc.AddLineBreak();
                        }
                    }
                    spellDesc.AddText(Strings.Get("spelldesc", "duration", (float) spell.Data2 / 10f),
                        spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
            }
            //Load Again for positioning purposes.
            Gui.LoadRootUiData(mDescWindow, "InGame.xml");
            spellDesc.SizeToChildren(false, true);
            mDescWindow.SetPosition(x, y);
        }

        public void Dispose()
        {
            if (mDescWindow == null)
            {
                return;
            }
            Gui.GameUi.GameCanvas.RemoveChild(mDescWindow, false);
            mDescWindow.Dispose();
        }
    }
}