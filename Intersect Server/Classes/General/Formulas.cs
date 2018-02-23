using System;
using System.IO;
using Intersect.Enums;
using Intersect.Server.Classes.Localization;
using Intersect.Server.Classes.Entities;
using Intersect.Utilities;
using NCalc;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Intersect.Server.Classes.General
{
    public class Formulas
    {
        public Formula ExpFormula = new Formula("BaseExp * Power(Gain, Level)");

        private static Formulas _formulas;

        private const string FORMULAS_FILE = "resources/formulas.json";

        public string PhysicalDamage = "Random(((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * .975, ((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * 1.025) * (100 / (100 + V_Defense))";
        public string MagicDamage =  "Random(((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * .975, ((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * 1.025) * (100 / (100 + V_MagicResist))";
        public string TrueDamage = "Random(((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * .975, ((BaseDamage + (ScalingStat * ScaleFactor))) * CritFactor * 1.025)";

        public static void LoadFormulas()
        {
            try
            {
                _formulas = new Formulas();
                if (File.Exists(FORMULAS_FILE))
                {
                    _formulas = JsonConvert.DeserializeObject<Formulas>(File.ReadAllText(FORMULAS_FILE));
                }
                File.WriteAllText(FORMULAS_FILE, JsonConvert.SerializeObject(_formulas, Formatting.Indented));
            }
            catch (Exception ex)
            {
                throw new Exception(Strings.Formulas.missing, ex);
            }
        }

        public static int CalculateDamage(int baseDamage, DamageType damageType, Stats scalingStat, int scaling,
            double critMultiplier, Entity attacker, Entity victim)
        {
            var expression = "";
            switch (damageType)
            {
                case DamageType.Physical:
                    expression = _formulas.PhysicalDamage;
                    break;
                case DamageType.Magic:
                    expression = _formulas.MagicDamage;
                    break;
                case DamageType.True:
                    expression = _formulas.TrueDamage;
                    break;
                default:
                    expression = _formulas.TrueDamage;
                    break;
            }
            Expression e = new Expression(expression);
            var negate = false;
            if (baseDamage < 0)
            {
                baseDamage = Math.Abs(baseDamage);
                negate = true;
            }
            try
            {
                e.Parameters["BaseDamage"] = baseDamage;
                e.Parameters["ScalingStat"] = attacker.Stat[(int) scalingStat].Value();
                e.Parameters["ScaleFactor"] = scaling / 100f;
                e.Parameters["CritFactor"] = critMultiplier;
                e.Parameters["A_Attack"] = attacker.Stat[(int) Stats.Attack].Value();
                e.Parameters["A_Defense"] = attacker.Stat[(int) Stats.Defense].Value();
                e.Parameters["A_Speed"] = attacker.Stat[(int) Stats.Speed].Value();
                e.Parameters["A_AbilityPwr"] = attacker.Stat[(int) Stats.AbilityPower].Value();
                e.Parameters["A_MagicResist"] = attacker.Stat[(int) Stats.MagicResist].Value();
                e.Parameters["V_Attack"] = victim.Stat[(int) Stats.Attack].Value();
                e.Parameters["V_Defense"] = victim.Stat[(int) Stats.Defense].Value();
                e.Parameters["V_Speed"] = victim.Stat[(int) Stats.Speed].Value();
                e.Parameters["V_AbilityPwr"] = victim.Stat[(int) Stats.AbilityPower].Value();
                e.Parameters["V_MagicResist"] = victim.Stat[(int) Stats.MagicResist].Value();
                e.EvaluateFunction += delegate(string name, FunctionArgs args)
                {
                    if (name == "Random")
                    {
                        int left = (int) Math.Round((double) args.Parameters[0].Evaluate());
                        int right = (int) Math.Round((double) args.Parameters[1].Evaluate());
                        if (left >= right)
                        {
                            args.Result = left;
                        }
                        else
                        {
                            args.Result = Globals.Rand.Next(left, right + 1);
                        }
                    }
                };
                double result = Convert.ToDouble(e.Evaluate());
                if (negate) result *= -1;
                return (int)Math.Round(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to evaluate damage formula", ex);
            }
        }
    }
}