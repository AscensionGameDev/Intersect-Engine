using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Enums;
using Intersect.GameObjects;

using JetBrains.Annotations;
using System.Collections.Generic;

namespace Intersect.Client.Interface.Game
{

    public class ItemDescWindow
    {

        ImagePanel mDescWindow;

        public ItemDescWindow(
            [NotNull] ItemBase item,
            int amount,
            int x,
            int y,
            int[] statBuffs,
			Dictionary<string, int> tags,
			string titleOverride = "",
            string valueLabel = "",
            bool centerHorizontally = false
        )
        {
            var title = titleOverride;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = item.Name;
            }

            mDescWindow = new ImagePanel(Interface.GameUi.GameCanvas, "ItemDescWindow");
            if (item != null && item.ItemType == ItemTypes.Equipment)
            {
                mDescWindow.Name = "ItemDescWindowExpanded";
            }

            if (item != null)
            {
                var icon = new ImagePanel(mDescWindow, "ItemIcon");

                var itemName = new Label(mDescWindow, "ItemNameLabel");
                itemName.Text = title;

                var itemQuantity = new Label(mDescWindow, "ItemQuantityLabel");

                if (amount > 1)
                {
                    itemQuantity.Text += amount.ToString("N0").Replace(",", Strings.Numbers.comma);
                }

                itemName.AddAlignment(Alignments.CenterH);

                var itemType = new Label(mDescWindow, "ItemTypeLabel");
                var itemValue = new Label(mDescWindow, "ItemValueLabel");

                itemType.Text = Strings.ItemDesc.itemtypes[(int) item.ItemType];
                itemValue.SetText(valueLabel);

                if (item.ItemType == ItemTypes.Equipment &&
                    item.EquipmentSlot >= 0 &&
                    item.EquipmentSlot < Options.EquipmentSlots.Count)
                {
                    itemType.Text = Options.EquipmentSlots[item.EquipmentSlot];
                    if (item.EquipmentSlot == Options.WeaponIndex && item.TwoHanded)
                    {
                        itemType.Text += " - " + Strings.ItemDesc.twohand;
                    }
                }

                if (item.Rarity > 0)
                {
                    itemType.Text += " - " + Strings.ItemDesc.rarity[item.Rarity];
                    var rarity = CustomColors.Items.Rarities.ContainsKey(item.Rarity)
                        ? CustomColors.Items.Rarities[item.Rarity]
                        : Color.White;

                    itemType.TextColorOverride.R = rarity.R;
                    itemType.TextColorOverride.G = rarity.G;
                    itemType.TextColorOverride.B = rarity.B;
                    itemType.TextColorOverride.A = rarity.A;
                }

                var itemDesc = new RichLabel(mDescWindow, "ItemDescription");
                var itemDescText = new Label(mDescWindow, "ItemDescText");
                itemDescText.Font = itemDescText.Parent.Skin.DefaultFont;
                var itemStatsText = new Label(mDescWindow, item.ItemType == ItemTypes.Equipment ? "ItemStatsText" : "");
                itemStatsText.Font = itemStatsText.Parent.Skin.DefaultFont;
                var itemStats = new RichLabel(mDescWindow, item.ItemType == ItemTypes.Equipment ? "ItemStats" : "");
                itemDescText.IsHidden = true;
                itemStatsText.IsHidden = true;

                //Load this up now so we know what color to make the text when filling out the desc
                mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                if (item.Description.Length > 0)
                {
                    itemDesc.AddText(
                        Strings.ItemDesc.desc.ToString(item.Description), itemDesc.RenderColor,
                        itemDescText.CurAlignments.Count > 0 ? itemDescText.CurAlignments[0] : Alignments.Left,
                        itemDescText.Font
                    );

                    itemDesc.AddLineBreak();
                    itemDesc.AddLineBreak();
                }

                var stats = "";
                if (item.ItemType == ItemTypes.Equipment)
                {
                    stats = Strings.ItemDesc.bonuses;
                    itemStats.AddText(
                        stats, itemStats.RenderColor,
                        itemStatsText.CurAlignments.Count > 0 ? itemStatsText.CurAlignments[0] : Alignments.Left,
                        itemDescText.Font
                    );

                    itemStats.AddLineBreak();
                    if (item.ItemType == ItemTypes.Equipment && item.EquipmentSlot == Options.WeaponIndex)
                    {
                        stats = Strings.ItemDesc.damage.ToString(item.Damage);
                        itemStats.AddText(
                            stats, itemStats.RenderColor,
                            itemStatsText.CurAlignments.Count > 0 ? itemStatsText.CurAlignments[0] : Alignments.Left,
                            itemDescText.Font
                        );

                        itemStats.AddLineBreak();
                    }

					foreach (KeyValuePair<string, int> tag in tags)
					{
						if (tag.Value == 0 || tag.Key == "tagcount") continue;
						var desc = "";
						string sign = "+";
						if (tag.Value < 0)
							sign = "-";
						switch (tag.Key)
						{
							// Vital
							case "life":
								desc = $"{sign}{tag.Value} {Strings.Combat.vital0}";
								break;
							case "lifeinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.vital0}";
								break;
							case "mana":
								desc = $"{sign}{tag.Value} {Strings.Combat.vital1}";
								break;
							case "manainc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.vital1}";
								break;
							// Stats
							case "attack":
								desc = $"{sign}{tag.Value} {Strings.Combat.stat0}";
								break;
							case "attackinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.stat0}";
								break;
							case "power":
								desc = $"{sign}{tag.Value} {Strings.Combat.stat1}";
								break;
							case "powerinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.stat1}";
								break;
							case "defense":
								desc = $"{sign}{tag.Value} {Strings.Combat.stat2}";
								break;
							case "defenseinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.stat2}";
								break;
							case "resistance":
								desc = $"{sign}{tag.Value} {Strings.Combat.stat3}";
								break;
							case "resistanceinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.stat3}";
								break;
							case "speed":
								desc = $"{sign}{tag.Value} {Strings.Combat.stat4}";
								break;
							case "speedinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% {Strings.Combat.stat4}";
								break;
							// Combat
							case "critchance":
								desc = $"{sign}{tag.Value}% Chance de critique";
								break;
							case "critchanceinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Chance de critique multiplier";
								break;
							case "critmult":
								desc = $"{sign}{tag.Value}% Chance de critique multiplieur";
								break;
							case "critmultint":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Chance de critique multiplieur multiplier";
								break;
							case "weapondamage":
								desc = $"{sign}{tag.Value} Dommage de l'arme";
								break;
							case "weapondamageinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Dommage de l'arme";
								break;
							case "damage":
								desc = $"{sign}{tag.Value} Dommage global";
								break;
							case "damageinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Dommage global";
								break;
							case "physical":
								desc = $"{sign}{tag.Value} Dommage physique";
								break;
							case "physicalinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Dommage physique";
								break;
							case "magical":
								desc = $"{sign}{tag.Value} Dommage magique";
								break;
							case "magicalinc":
								desc = $"{sign}{((float)tag.Value / 100.0f).ToString("0.00")}% Dommage magique";
								break;
							// Special
							case "halfattacktolife":
								desc = $"Ajoute la moitier de la stat '{Strings.Combat.stat0}' en {Strings.Combat.vital0}";
								break;
							case "halfpowertomana":
								desc = $"Ajoute la moitier de la stat '{Strings.Combat.stat1}' en {Strings.Combat.vital1}";
								break;
							default:
								desc = $"{tag.Key} : {tag.Value}";
								break;
						}
						itemStats.AddText(
							desc, itemStats.RenderColor,
							itemStatsText.CurAlignments.Count > 0 ? itemStatsText.CurAlignments[0] : Alignments.Left,
							itemDescText.Font
						);

						itemStats.AddLineBreak();
					}

					//for (var i = 0; i < (int) Vitals.VitalCount; i++)
					//{
					//    var bonus = item.VitalsGiven[i].ToString();
					//    if (item.PercentageVitalsGiven[i] > 0)
					//    {
					//        if (item.VitalsGiven[i] > 0)
					//        {
					//            bonus += " + ";
					//        }
					//        else
					//        {
					//            bonus = "";
					//        }

					//        bonus += item.PercentageVitalsGiven[i] + "%";
					//    }

					//    var vitals = Strings.ItemDesc.vitals[i].ToString(bonus);
					//    itemStats.AddText(
					//        vitals, itemStats.RenderColor,
					//        itemStatsText.CurAlignments.Count > 0 ? itemStatsText.CurAlignments[0] : Alignments.Left,
					//        itemDescText.Font
					//    );

					//    itemStats.AddLineBreak();
					//}

					//if (statBuffs != null)
					//{
					//    for (var i = 0; i < Options.MaxStats; i++)
					//    {
					//        var flatStat = item.StatsGiven[i] + statBuffs[i];
					//        var bonus = flatStat.ToString();

					//        if (item.PercentageStatsGiven[i] > 0)
					//        {
					//            if (flatStat > 0)
					//            {
					//                bonus += " + ";
					//            }
					//            else
					//            {
					//                bonus = "";
					//            }

					//            bonus += item.PercentageStatsGiven[i] + "%";
					//        }

					//        stats = Strings.ItemDesc.stats[i].ToString(bonus);
					//        itemStats.AddText(
					//            stats, itemStats.RenderColor,
					//            itemStatsText.CurAlignments.Count > 0
					//                ? itemStatsText.CurAlignments[0]
					//                : Alignments.Left, itemDescText.Font
					//        );

					//        itemStats.AddLineBreak();
					//    }
					//}
				}

                if (item.ItemType == ItemTypes.Equipment &&
                    item.Effect.Type != EffectType.None &&
                    item.Effect.Percentage > 0)
                {
                    itemStats.AddText(
                        Strings.ItemDesc.effect.ToString(
                            item.Effect.Percentage, Strings.ItemDesc.effects[(int) item.Effect.Type - 1]
                        ), itemStats.RenderColor,
                        itemStatsText.CurAlignments.Count > 0 ? itemStatsText.CurAlignments[0] : Alignments.Left,
                        itemDescText.Font
                    );
                }

                //Load Again for positioning purposes.
                mDescWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    icon.Texture = itemTex;
                }

                itemDesc.SizeToChildren(false, true);
                itemStats.SizeToChildren(false, true);
                itemDescText.IsHidden = true;
                itemStatsText.IsHidden = true;
                if (centerHorizontally)
                {
                    mDescWindow.MoveTo(x - mDescWindow.Width / 2, y + mDescWindow.Padding.Top);
                }
                else
                {
                    mDescWindow.MoveTo(x - mDescWindow.Width - mDescWindow.Padding.Right, y + mDescWindow.Padding.Top);
                }
            }
        }

        public void Dispose()
        {
            Interface.GameUi?.GameCanvas?.RemoveChild(mDescWindow, false);
            mDescWindow.Dispose();
        }

    }

}
