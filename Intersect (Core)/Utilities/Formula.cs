using System;
using System.Collections.Generic;

using Intersect.Logging;

using JetBrains.Annotations;

using NCalc;

namespace Intersect.Utilities
{

    public delegate void FormulaFunction([NotNull] FunctionArgs args);

    public delegate void FormulaParameter([NotNull] ParameterArgs args);

    public class Formula
    {

        [NotNull] public static readonly List<string> SYSTEM_MATH_FUNCTIONS = new List<string>
        {
            "Abs",
            "Acos",
            "Asin",
            "Atan",
            "BigMul",
            "Ceiling",
            "Cos",
            "Cosh",
            "DivRem",
            "Exp",
            "Floor",
            "IEEERemainder",
            "Log",
            "Max",
            "Min",
            "Pow",
            "Round",
            "Sign",
            "Sin",
            "Sinh",
            "Sqrt",
            "Tan",
            "Tanh",
            "Truncate"
        };

        [CanBeNull] private string mLastSource;

        [NotNull] private string mSource;

        public Formula(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;
            Functions = new Dictionary<string, FormulaFunction>();
            Parameters = new Dictionary<string, FormulaParameter>();

            if (Expression == null)
            {
                throw new ArgumentNullException(nameof(Expression));
            }
        }

        [NotNull]
        public string Source
        {
            get => mSource;
            set
            {
                if (string.Equals(mSource, value, StringComparison.Ordinal))
                {
                    return;
                }

                mLastSource = mSource;
                mSource = value;

                if (!Load())
                {
                    Log.Warn($"Error loading formula from {mSource}.");
                }
                else
                {
                    Log.Debug($"Loaded new formula from {mSource}.");
                }
            }
        }

        [NotNull]
        protected Expression Expression { get; set; }

        [NotNull]
        protected IDictionary<string, FormulaFunction> Functions { get; }

        [NotNull]
        protected IDictionary<string, FormulaParameter> Parameters { get; }

        public bool Load()
        {
            if (Source == mLastSource)
            {
                return true;
            }

            Expression = new Expression(Source);

            Expression.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }

                if (!Parameters.TryGetValue(name, out var formulaParameter))
                {
                    Log.Error($"Tried to access non-existent parameter '{name}' in a formula.");

                    return;
                }

                if (args == null)
                {
                    Log.Error($"Formula parameter '{name}' arguments were null.");

                    return;
                }

                formulaParameter(args);
            };

            RegisterParameters();

            Expression.EvaluateFunction += delegate(string name, FunctionArgs args)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }

                if (!Functions.TryGetValue(name, out var formulaFunction))
                {
                    if (!SYSTEM_MATH_FUNCTIONS.Contains(name))
                    {
                        Log.Error($"Tried to access non-existent function '{name}' in a formula.");
                    }

                    return;
                }

                if (args == null)
                {
                    Log.Error($"Formula function '{name}' arguments were null.");

                    return;
                }

                formulaFunction(args);
            };

            RegisterFunctions();

            return true;
        }

        public virtual void RegisterFunctions()
        {
        }

        public bool RegisterFunction([NotNull] string name, FormulaFunction function, bool shouldOverride = false)
        {
            if (Functions.ContainsKey(name))
            {
                if (!shouldOverride)
                {
                    Log.Debug($"Formula function '{name}' already exists, not overriding.");

                    return false;
                }
            }

            Functions[name] = function;

            return true;
        }

        public void RegisterCoreMathParameters()
        {
            RegisterParameter("PI", Math.PI);
            RegisterParameter("E", Math.E);
        }

        public virtual void RegisterParameters()
        {
        }

        public bool RegisterParameter([NotNull] string name, object value, bool shouldOverride = false)
        {
            if (Expression.Parameters == null)
            {
                throw new ArgumentNullException(nameof(Expression.Parameters));
            }

            if (Expression.Parameters.ContainsKey(name))
            {
                if (!shouldOverride)
                {
                    Log.Debug($"Formula parameter '{name}' already exists, not overriding.");

                    return false;
                }
            }

            Expression.Parameters[name] = value;

            return true;
        }

        public bool RegisterEvaluatedParameter([NotNull] string name, object value, bool shouldOverride = false)
        {
            if (Parameters.ContainsKey(name))
            {
                if (!shouldOverride)
                {
                    Log.Debug($"Formula evaluated parameter '{name}' already exists,not overriding.");

                    return false;
                }
            }

            Parameters.Add(
                name, delegate(ParameterArgs args)
                {
                    if (args == null)
                    {
                        Log.Error($"Formula function '{name}' arguments were null.");

                        return;
                    }

                    args.Result = value;
                }
            );

            return Parameters.ContainsKey(name);
        }

        public T Evaluate<T>() where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return (T) (Convert.ChangeType(Expression.Evaluate(), typeof(T)) ?? default(T));
        }

    }

}
