using System;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.UI.Game
{
    public class SpellDescWindow
    {
        ImagePanel mDescWindow;

        public SpellDescWindow(Guid spellId, int x, int y)
        {
            var spell = SpellBase.Get(spellId);
            if (spell == null)
            {
                return;
            }
            mDescWindow = new ImagePanel(Gui.GameUi.GameCanvas, "SpellDescWindow");

            ImagePanel icon = new ImagePanel(mDescWindow, "SpellIcon");

            Label spellName = new Label(mDescWindow, "SpellName");
            spellName.Text = spell.Name;

            Label spellType = new Label(mDescWindow, "SpellType");
            spellType.Text = Strings.SpellDesc.spelltypes[(int)spell.SpellType];

            RichLabel spellDesc = new RichLabel(mDescWindow, "SpellDesc");
            //Load this up now so we know what color to make the text when filling out the desc
            mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
            if (spell.Description.Length > 0)
            {
                spellDesc.AddText(Strings.SpellDesc.desc.ToString( spell.Description), spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }

            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                spellType.Text = Strings.SpellDesc.targettypes[(int)spell.Combat.TargetType].ToString(spell.Combat.CastRange, spell.Combat.HitRadius);
            }
            if (spell.CastDuration > 0)
            {
				float castDuration = (float)spell.CastDuration / 1000f;
				spellDesc.AddText(Strings.SpellDesc.casttime.ToString(castDuration),
                    spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }
            if (spell.CooldownDuration > 0)
            {
				decimal cdr = 1 - (Globals.Me.GetCooldownReduction() / 100);
				float cd = ((float)(spell.CooldownDuration * cdr) / 1000f);
				spellDesc.AddText(Strings.SpellDesc.cooldowntime.ToString(cd),
                    spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }

            bool requirements = (spell.VitalCost[(int) Vitals.Health] > 0 || spell.VitalCost[(int) Vitals.Mana] > 0);

            if (requirements == true)
            {
                spellDesc.AddText(Strings.SpellDesc.prereqs, spellDesc.RenderColor);
                spellDesc.AddLineBreak();
                if (spell.VitalCost[(int) Vitals.Health] > 0)
                {
                    spellDesc.AddText(Strings.SpellDesc.vitalcosts[(int)Vitals.Health].ToString( spell.VitalCost[(int) Vitals.Health]),
                        spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
                if (spell.VitalCost[(int) Vitals.Mana] > 0)
                {
                    spellDesc.AddText(Strings.SpellDesc.vitalcosts[(int)Vitals.Mana].ToString( spell.VitalCost[(int) Vitals.Mana]),
                        spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
                spellDesc.AddLineBreak();
            }

            string stats = "";
            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                stats = Strings.SpellDesc.effects;
                spellDesc.AddText(stats, spellDesc.RenderColor);
                spellDesc.AddLineBreak();

                if (spell.Combat.Effect > 0)
                {
                    spellDesc.AddText(Strings.SpellDesc.effectlist[(int)spell.Combat.Effect], spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }

                for (var i = 0; i < (int) Vitals.VitalCount; i++)
                {
                    var vitalDiff = spell.Combat.VitalDiff?[i] ?? 0;
                    if (vitalDiff == 0) continue;
                    var vitalSymbol = vitalDiff < 0 ? Strings.SpellDesc.addsymbol : Strings.SpellDesc.removesymbol;
                    stats = Strings.SpellDesc.vitals[i].ToString(vitalSymbol, Math.Abs(vitalDiff));
                    spellDesc.AddText(stats, spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }

                if (spell.Combat.Duration > 0)
                {
                    for (int i = 0; i < Options.MaxStats; i++)
                    {
                        if (spell.Combat.StatDiff[i] != 0)
                        {
                            spellDesc.AddText(Strings.SpellDesc.stats[i].ToString((spell.Combat.StatDiff[i] > 0 ? Strings.SpellDesc.addsymbol.ToString() : Strings.SpellDesc.removesymbol.ToString()) + spell.Combat.StatDiff[i]), spellDesc.RenderColor);
                            spellDesc.AddLineBreak();
                        }
                    }
					float duration = (float)spell.Combat.Duration / 1000f;
					spellDesc.AddText(Strings.SpellDesc.duration.ToString(duration), spellDesc.RenderColor);
                    spellDesc.AddLineBreak();
                }
            }
            //Load Again for positioning purposes.
            mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
            icon.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Icon);
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