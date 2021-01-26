using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game
{

    public class SpellDescWindow
    {

        ImagePanel mDescWindow;

        public SpellDescWindow(Guid spellId, int x, int y, bool centerHorizontally = false)
        {
            var spell = SpellBase.Get(spellId);
            if (spell == null)
            {
                return;
            }

            mDescWindow = new ImagePanel(Interface.GameUi.GameCanvas, "SpellDescWindowExpanded");

            var icon = new ImagePanel(mDescWindow, "SpellIcon");

            var spellName = new Label(mDescWindow, "SpellName");
            spellName.Text = spell.Name;

            var spellType = new Label(mDescWindow, "SpellType");
            spellType.Text = Strings.SpellDesc.spelltypes[(int) spell.SpellType];

            var spellDesc = new RichLabel(mDescWindow, "SpellDesc");
            var spellStats = new RichLabel(mDescWindow, "SpellStats");
            var spellDescText = new Label(mDescWindow, "SpellDescText");
            spellDescText.Font = spellDescText.Parent.Skin.DefaultFont;
            var spellStatsText = new Label(mDescWindow, "SpellStatsText");
            spellStatsText.Font = spellStatsText.Parent.Skin.DefaultFont;
            spellDescText.IsHidden = true;
            spellStatsText.IsHidden = true;

            //Load this up now so we know what color to make the text when filling out the desc
            mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            if (spell.Description.Length > 0)
            {
                spellDesc.AddText(
                    Strings.SpellDesc.desc.ToString(spell.Description), spellDesc.RenderColor,
                    spellDescText.CurAlignments.Count > 0 ? spellDescText.CurAlignments[0] : Alignments.Left,
                    spellDescText.Font
                );

                spellDesc.AddLineBreak();
                spellDesc.AddLineBreak();
            }

            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                if (spell.Combat.TargetType == SpellTargetTypes.Projectile)
                {
                    var proj = ProjectileBase.Get(spell.Combat.ProjectileId);
                    spellType.Text = Strings.SpellDesc.targettypes[(int) spell.Combat.TargetType]
                        .ToString(proj?.Range ?? 0, spell.Combat.HitRadius);
                }
                else
                {
                    spellType.Text = Strings.SpellDesc.targettypes[(int) spell.Combat.TargetType]
                        .ToString(spell.Combat.CastRange, spell.Combat.HitRadius);
                }
            }

            if (spell.SpellType == (int) SpellTypes.CombatSpell &&
                (spell.Combat.TargetType == SpellTargetTypes.AoE ||
                 spell.Combat.TargetType == SpellTargetTypes.Single) &&
                spell.Combat.HitRadius > 0)
            {
                spellStats.AddText(
                    Strings.SpellDesc.radius.ToString(spell.Combat.HitRadius), spellStats.RenderColor,
                    spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                    spellStatsText.Font
                );

                spellStats.AddLineBreak();
                spellStats.AddLineBreak();
            }

            if (spell.CastDuration > 0)
            {
                var castDuration = (float) spell.CastDuration / 1000f;
                spellStats.AddText(
                    Strings.SpellDesc.casttime.ToString(castDuration), spellStats.RenderColor,
                    spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                    spellStatsText.Font
                );

                spellStats.AddLineBreak();
                if (spell.CooldownDuration <= 0)
                {
                    spellStats.AddLineBreak();
                }
            }

            if (spell.CooldownDuration > 0)
            {
                var cdr = 1 - Globals.Me.GetCooldownReduction() / 100;
                var cd = (float) (spell.CooldownDuration * cdr) / 1000f;
                spellStats.AddText(
                    Strings.SpellDesc.cooldowntime.ToString(cd), spellStats.RenderColor,
                    spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                    spellStatsText.Font
                );

                spellStats.AddLineBreak();
                spellStats.AddLineBreak();
            }

            var requirements = spell.VitalCost[(int) Vitals.Health] > 0 || spell.VitalCost[(int) Vitals.Mana] > 0;

            if (requirements == true)
            {
                spellStats.AddText(
                    Strings.SpellDesc.prereqs, spellStats.RenderColor,
                    spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                    spellStatsText.Font
                );

                spellStats.AddLineBreak();
                if (spell.VitalCost[(int) Vitals.Health] > 0)
                {
                    spellStats.AddText(
                        Strings.SpellDesc.vitalcosts[(int) Vitals.Health]
                            .ToString(spell.VitalCost[(int) Vitals.Health]), spellStats.RenderColor,
                        spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                        spellStatsText.Font
                    );

                    spellStats.AddLineBreak();
                }

                if (spell.VitalCost[(int) Vitals.Mana] > 0)
                {
                    spellStats.AddText(
                        Strings.SpellDesc.vitalcosts[(int) Vitals.Mana].ToString(spell.VitalCost[(int) Vitals.Mana]),
                        spellStats.RenderColor,
                        spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                        spellStatsText.Font
                    );

                    spellStats.AddLineBreak();
                }

                spellStats.AddLineBreak();
            }

            var stats = "";
            if (spell.SpellType == (int) SpellTypes.CombatSpell)
            {
                stats = Strings.SpellDesc.effects;
                spellStats.AddText(
                    stats, spellStats.RenderColor,
                    spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                    spellStatsText.Font
                );

                spellStats.AddLineBreak();

                if (spell.Combat.Effect > 0)
                {
                    spellStats.AddText(
                        Strings.SpellDesc.effectlist[(int) spell.Combat.Effect], spellStats.RenderColor,
                        spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                        spellStatsText.Font
                    );

                    spellStats.AddLineBreak();
                }

                for (var i = 0; i < (int) Vitals.VitalCount; i++)
                {
                    var vitalDiff = spell.Combat.VitalDiff?[i] ?? 0;
                    if (vitalDiff == 0)
                    {
                        continue;
                    }

                    var vitalSymbol = vitalDiff < 0 ? Strings.SpellDesc.addsymbol : Strings.SpellDesc.removesymbol;
                    if (spell.Combat.Effect == StatusTypes.Shield)
                    {
                        stats = Strings.SpellDesc.shield.ToString(Math.Abs(vitalDiff));
                    }
                    else
                    {
                        stats = Strings.SpellDesc.vitals[i].ToString(vitalSymbol, Math.Abs(vitalDiff));
                    }

                    spellStats.AddText(
                        stats, spellStats.RenderColor,
                        spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                        spellStatsText.Font
                    );

                    spellStats.AddLineBreak();
                }

                if (spell.Combat.Duration > 0)
                {
                    for (var i = 0; i < (int)Stats.StatCount; i++)
                    {
                        if (spell.Combat.StatDiff[i] != 0)
                        {
                            spellStats.AddText(
                                Strings.SpellDesc.stats[i]
                                    .ToString(
                                        (spell.Combat.StatDiff[i] > 0
                                            ? Strings.SpellDesc.addsymbol.ToString()
                                            : Strings.SpellDesc.removesymbol.ToString()) +
                                        Math.Abs(spell.Combat.StatDiff[i])
                                    ), spellStats.RenderColor,
                                spellStatsText.CurAlignments.Count > 0
                                    ? spellStatsText.CurAlignments[0]
                                    : Alignments.Left, spellStatsText.Font
                            );

                            spellStats.AddLineBreak();
                        }
                    }

                    var duration = (float) spell.Combat.Duration / 1000f;
                    spellStats.AddText(
                        Strings.SpellDesc.duration.ToString(duration), spellStats.RenderColor,
                        spellStatsText.CurAlignments.Count > 0 ? spellStatsText.CurAlignments[0] : Alignments.Left,
                        spellStatsText.Font
                    );

                    spellStats.AddLineBreak();
                }
            }

            spellStats.SizeToChildren(false, true);
            if (spellStats.Children.Count == 0)
            {
                mDescWindow.Name = "SpellDescWindow";
                spellStats.Name = "";
                spellStatsText.Name = "";
            }

            //Load Again for positioning purposes.
            mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            spellDescText.IsHidden = true;
            spellStatsText.IsHidden = true;
            icon.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Spell, spell.Icon);
            spellStats.SizeToChildren(false, true);
            if (centerHorizontally)
            {
                mDescWindow.MoveTo(x - mDescWindow.Width / 2, y + mDescWindow.Padding.Top);
            }
            else
            {
                mDescWindow.MoveTo(x - mDescWindow.Width - mDescWindow.Padding.Right, y + mDescWindow.Padding.Top);
            }
        }

        public void Dispose()
        {
            if (mDescWindow == null)
            {
                return;
            }

            Interface.GameUi.GameCanvas.RemoveChild(mDescWindow, false);
            mDescWindow.Dispose();
        }

    }

}
