using System;

using Intersect.Logging;

using NCalc;

namespace Intersect.Server.Utilities
{

    public class ExperienceCurve
    {

        public const string DEFAULT_EXPERIENCE_FORMULA = "Floor(BaseExp * Pow(Gain, Level - 1))";

        public const string PARAM_BASE_EXP = "BaseExp";

        public const string PARAM_GAIN = "Gain";

        public const string PARAM_LEVEL = "Level";

        public ExperienceCurve(string source = DEFAULT_EXPERIENCE_FORMULA)
        {
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            //Formula = new Formula(source);
            //Formula.RegisterFunction("Exp", Exp);

            BaseExperience = 100;
            Gain = 1.5;
        }

        //private Formula mFormula;

        //public Formula Formula
        //{
        //    get => mFormula ?? (mFormula = new Formula(DEFAULT_EXPERIENCE_FORMULA));
        //    set => mFormula = value;
        //}

        public long BaseExperience { get; set; }

        public double Gain { get; set; }

        protected virtual void Exp(FunctionArgs args)
        {
            if (args.Parameters == null || args.Parameters.Length < 3)
            {
                Log.Error("Tried to execute Exp with fewer than three arguments.");
                args.Result = 0L;
            }

            var level = (int) (args.Parameters?[0]?.Evaluate() ?? -1);
            var baseExperience = (long) (args.Parameters?[1]?.Evaluate() ?? -1);
            var gain = (double) (args.Parameters?[2]?.Evaluate() ?? -1);
            args.Result = Calculate(level, baseExperience, gain);
        }

        public long Calculate(int level)
        {
            return Calculate(level, BaseExperience, Gain);
        }

        public long Calculate(int level, long baseExperience)
        {
            return Calculate(level, baseExperience, Gain);
        }

        public long Calculate(int level, long baseExperience, double gain)
        {
            //Formula.RegisterParameter(PARAM_BASE_EXP, baseExperience, true);
            //Formula.RegisterParameter(PARAM_GAIN, gain, true);
            //Formula.RegisterParameter(PARAM_LEVEL, level, true);
            //return Formula.Evaluate<long>();
            return (long) Math.Floor(baseExperience * Math.Pow(gain, level - 1));
        }

    }

}
