using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class SpellDescWindow
    {
        ImagePanel _descWindow;

        public SpellDescWindow(int spellnum, int x, int y)
        {
            var spell = SpellBase.Lookup.Get(spellnum);
            if (spell == null)
            {
                return;
            }
            _descWindow = new ImagePanel(Gui.GameUI.GameCanvas);
            _descWindow.SetSize(255, 320);
            _descWindow.Margin = Margin.Zero;
            _descWindow.Padding = new Padding(8, 5, 9, 11);
            _descWindow.SetPosition(x, y);
            _descWindow.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                "spelldescpanel.png");

            ImagePanel icon = new ImagePanel(_descWindow);
            icon.SetSize(32, 32);
            icon.SetPosition(240 - 4 - 32, 4);
            icon.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Pic);

            Label spellName = new Label(_descWindow);
            spellName.SetPosition(4, 8);
            spellName.Text = spell.Name;
            spellName.SetTextColor(IntersectClientExtras.GenericClasses.Color.White, Label.ControlState.Normal);
            spellName.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);
            Align.CenterHorizontally(spellName);

            Label spellType = new Label(_descWindow);
            spellType.SetPosition(4, 24);
            spellType.SetTextColor(IntersectClientExtras.GenericClasses.Color.White, Label.ControlState.Normal);
            spellType.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 10);
            spellType.Text = Strings.Get("spelldesc", "spelltype" + spell.SpellType);

            y = 44;
            RichLabel spellDesc = new RichLabel(_descWindow);
            spellDesc.SetPosition(_descWindow.Padding.Left + 4, y);
            spellDesc.Width = 240;
            //itemDesc.SetBounds(4, y, 180, 10);
            if (spell.Desc.Length > 0)
            {
                spellDesc.AddText(Strings.Get("spelldesc", "desc", spell.Desc), spellName.TextColor);
            }
            spellDesc.SizeToChildren(false, true);

            y += spellDesc.Height + 8;

            RichLabel castInfo = new RichLabel(_descWindow);
            castInfo.SetPosition(_descWindow.Padding.Left + 4, y);
            castInfo.Width = 240;

            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                spellType.Text = Strings.Get("spelldesc", "targettype" + spell.TargetType, spell.CastRange,
                    spell.HitRadius);
            }
            if (spell.CastDuration > 0)
            {
                castInfo.AddText(Strings.Get("spelldesc", "casttime", ((float) spell.CastDuration / 10f)),
                    spellName.TextColor);
                castInfo.AddLineBreak();
            }
            if (spell.CooldownDuration > 0)
            {
                castInfo.AddText(Strings.Get("spelldesc", "cooldowntime", ((float) spell.CooldownDuration / 10f)),
                    spellName.TextColor);
                castInfo.AddLineBreak();
            }
            castInfo.SizeToChildren(false, true);
            y += castInfo.Height + 8;

            int y1 = y;

            bool requirements = false;

            //Check for requirements
            RichLabel itemReqs = new RichLabel(_descWindow)
            {
                Width = 120
            };
            itemReqs.AddText(Strings.Get("spelldesc", "prereqs"), spellName.TextColor);
            itemReqs.AddLineBreak();
            itemReqs.SetPosition(_descWindow.Padding.Left + 4, y);
            if (spell.VitalCost[(int) Vitals.Health] > 0)
            {
                requirements = true;
                itemReqs.AddText(Strings.Get("spelldesc", "vital0cost", spell.VitalCost[(int) Vitals.Health]),
                    spellName.TextColor);
                itemReqs.AddLineBreak();
            }
            if (spell.VitalCost[(int) Vitals.Mana] > 0)
            {
                requirements = true;
                itemReqs.AddText(Strings.Get("spelldesc", "vital1cost", spell.VitalCost[(int) Vitals.Mana]),
                    spellName.TextColor);
                itemReqs.AddLineBreak();
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
            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                RichLabel spellStats = new RichLabel(_descWindow);
                if (requirements != true)
                {
                    spellStats.SetPosition(_descWindow.Padding.Left + 4, y);
                    spellStats.Width = 240;
                }
                else
                {
                    spellStats.SetPosition(120, y);
                    spellStats.Width = 120;
                }
                stats = Strings.Get("spelldesc", "effects");
                spellStats.AddText(stats, spellName.TextColor);
                spellStats.AddLineBreak();

                if (spell.Data3 > 0)
                {
                    spellStats.AddText(Strings.Get("spelldesc", "effect" + spell.Data3), spellName.TextColor);
                    spellStats.AddLineBreak();
                }

                if (spell.VitalDiff[(int) Vitals.Health] != 0)
                {
                    stats = Strings.Get("spelldesc", "vital0",
                    (spell.VitalDiff[(int) Vitals.Health] > 0
                        ? Strings.Get("spelldesc", "addsymbol")
                        : Strings.Get("spelldecs", "removesymbol")), spell.VitalDiff[(int) Vitals.Health]);
                    spellStats.AddText(stats, spellName.TextColor);
                    spellStats.AddLineBreak();
                }

                if (spell.VitalDiff[(int) Vitals.Mana] != 0)
                {
                    stats = Strings.Get("spelldesc", "vital1",
                    (spell.VitalDiff[(int) Vitals.Mana] > 0
                        ? Strings.Get("spelldesc", "addsymbol")
                        : Strings.Get("spelldesc", "removesymbol")), spell.VitalDiff[(int) Vitals.Mana]);
                    spellStats.AddText(stats, spellName.TextColor);
                    spellStats.AddLineBreak();
                }

                if (spell.Data2 > 0)
                {
                    for (int i = 0; i < Options.MaxStats; i++)
                    {
                        if (spell.StatDiff[i] != 0)
                        {
                            spellStats.AddText(
                                Strings.Get("combat", "stat" + i) + ": " + (spell.StatDiff[i] > 0 ? "+ " : "") +
                                spell.StatDiff[i], spellName.TextColor);
                            spellStats.AddLineBreak();
                        }
                    }
                    spellStats.AddText(Strings.Get("spelldesc", "duration", (float) spell.Data2 / 10f),
                        spellName.TextColor);
                    spellStats.AddLineBreak();
                }
                spellStats.SizeToChildren(false, true);
            }
            Align.CenterHorizontally(spellType);
        }

        public void Dispose()
        {
            if (_descWindow == null)
            {
                return;
            }
            Gui.GameUI.GameCanvas.RemoveChild(_descWindow, false);
            _descWindow.Dispose();
        }
    }
}