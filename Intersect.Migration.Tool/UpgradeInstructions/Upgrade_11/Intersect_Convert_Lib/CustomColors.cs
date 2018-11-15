using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib
{
    public class CustomColors
    {
        public static Color HpForeground = Color.Red;
        public static Color HpBackground = Color.Black;
        public static Color CastingForeground = Color.Cyan;
        public static Color CastingBackground = Color.Black;

        public static Color EventName = Color.White;
        public static Color EventNameBorder = Color.Black;
        public static Color EventNameBackground = new Color(180, 0, 0, 0);

        public static Color AgressiveNpcName = new Color(255, 255, 40, 0);
        public static Color AgressiveNpcNameBorder = Color.Black;
        public static Color AgressiveNpcNameBackground = new Color(180, 0, 0, 0);

        public static Color AttackWhenAttackedName = new Color(255, 255, 255, 255);
        public static Color AttackWhenAttackedNameBorder = Color.Black;
        public static Color AttackWhenAttackedNameBackground = new Color(180, 0, 0, 0);

        public static Color AttackOnSightName = new Color(255, 255, 200, 0);
        public static Color AttackOnSightNameBorder = Color.Black;
        public static Color AttackOnSightNameBackground = new Color(180, 0, 0, 0);

        public static Color NeutralName = new Color(255, 100, 230, 100);
        public static Color NeutralNameBorder = Color.Black;
        public static Color NeutralNameBackground = new Color(180, 0, 0, 0);

        public static Color GuardName = new Color(255, 240, 142, 105);
        public static Color GuardNameBorder = Color.Black;
        public static Color GuardNameBackground = new Color(180, 0, 0, 0);

        public static Color PlayerNameNormal = Color.White;
        public static Color PlayerNameNormalBorder = Color.Black;
        public static Color PlayerNameNormalBackground = new Color(180, 0, 0, 0);

        public static Color PlayerNameMod = new Color(255, 0, 255, 255);
        public static Color PlayerNameModBorder = Color.Black;
        public static Color PlayerNameModBackground = new Color(180, 0, 0, 0);

        public static Color PlayerNameAdmin = new Color(255, 255, 255, 0);
        public static Color PlayerNameAdminBorder = Color.Black;
        public static Color PlayerNameAdminBackground = new Color(180, 0, 0, 0);

        public static Color GlobalMsg = new Color(255, 220, 220, 220);
        public static Color PlayerMsg = new Color(255, 220, 220, 220);
        public static Color ProximityMsg = new Color(255, 220, 220, 220);
        public static Color Blocked = new Color(255, 0, 0, 255);
        public static Color Status = new Color(255, 255, 255, 0);
        public static Color Missed = new Color(255, 255, 255, 255);
        public static Color Critical = new Color(255, 255, 255, 0);
        public static Color PhysicalDamage = new Color(255, 255, 0, 0);
        public static Color MagicDamage = new Color(255, 255, 0, 255);
        public static Color TrueDamage = new Color(255, 255, 255, 255);
        public static Color Heal = new Color(255, 0, 255, 0);
        public static Color AddMana = new Color(255, 0, 0, 255);
        public static Color RemoveMana = new Color(255, 255, 127, 80);
        public static Color Dash = new Color(255, 0, 0, 255);
        public static Color NoAmmo = Color.Red;
        public static Color AdminJoined = new Color(255, 255, 255, 0);
        public static Color ModJoined = new Color(255, 0, 255, 255);
        public static Color NoTarget = Color.Red;
        public static Color Declined = Color.Red;
        public static Color Accepted = Color.Green;
        public static Color Error = Color.Red;
        public static Color Info = Color.White;
        public static Color AdminGlobalChat = new Color(255, 255, 255, 0);
        public static Color ModGlobalChat = new Color(255, 0, 255, 255);
        public static Color GlobalChat = new Color(255, 220, 220, 220);
        public static Color AnnouncementChat = Color.Yellow;
        public static Color AdminChat = Color.Cyan;
        public static Color PartyChat = Color.Green;
        public static Color PrivateChat = Color.Magenta;
        public static Color AdminLocalChat = new Color(255, 255, 255, 0);
        public static Color ModLocalChat = new Color(255, 0, 255, 255);
        public static Color LocalChat = new Color(255, 240, 240, 240);
        public static Color QuestStarted = Color.Cyan;
        public static Color QuestDeclined = Color.Red;
        public static Color QuestAbandoned = Color.Red;
        public static Color QuestCompleted = Color.Green;
        public static Color TaskUpdated = Color.Cyan;
        public static Color LevelUp = Color.Cyan;
        public static Color StatPoints = Color.Cyan;
        public static Color ItemBound = Color.Red;
        public static Color Experience = Color.White;
        public static Color Crafted = Color.Green;
        public static Color RequestSent = Color.Yellow;
        public static Color ChatBubbleTextColor = Color.Black;

        public static void Load()
        {
            if (File.Exists(Path.Combine("resources", "colors.json")))
            {
                Dictionary<string, string> colors = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine("resources", "colors.json")));

                Type type = typeof(CustomColors);
                foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static |
                                                 System.Reflection.BindingFlags.Public))
                {
                    if (colors.ContainsKey(p.Name))
                    {
                        var value = colors[p.Name];
                        Match match = Regex.Match(value,
                            "argb" + Regex.Escape("(") + "([0-9]*),([0-9]*),([0-9]*),([0-9]*)" + Regex.Escape(")"));
                        if (match.Success)
                        {
                            p.SetValue(p, Color.FromArgb(Convert.ToInt32(match.Groups[1].Value),
                                Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value),
                                Convert.ToInt32(match.Groups[4].Value)));
                        }
                    }
                }
            }
            Save();
        }

        private static void Save()
        {
            Dictionary<string, string> colors = new Dictionary<string, string>();

            Type type = typeof(CustomColors);
            foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static |
                                             System.Reflection.BindingFlags.Public))
            {
                Color val = (Color)p.GetValue(null);
                colors.Add(p.Name, "argb(" + String.Join(",",
                                       new string[4] { val.A.ToString(), val.R.ToString(), val.G.ToString(), val.B.ToString() }) + ")");
            }
            File.WriteAllText(Path.Combine("resources", "colors.json"), JsonConvert.SerializeObject(colors,Formatting.Indented));
        }

        public static byte[] GetData()
        {
            var bf = new ByteBuffer();
            Type type = typeof(CustomColors);
            foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static |
                                             System.Reflection.BindingFlags.Public))
            {
                bf.WriteInteger(((Color)p.GetValue(null)).ToArgb());
            }
            return bf.ToArray();
        }

        public static void Load(ByteBuffer bf)
        {
            Type type = typeof(CustomColors);
            foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static |
                                             System.Reflection.BindingFlags.Public))
            {
                p.SetValue(p, Color.FromArgb(bf.ReadInteger()));
            }
        }
    }
}