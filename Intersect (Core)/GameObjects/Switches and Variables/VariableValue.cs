using Intersect.Enums;
using Intersect.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.GameObjects.Switches_and_Variables
{
    public class VariableValue
    {
        public VariableDataTypes Type { get; set; }

        [NotMapped]
        public dynamic Value { get; set; }

        #region Typed Properties

        [JsonIgnore]
        public bool Boolean
        {
            get => Value;
            set
            {
                Type = VariableDataTypes.Boolean;
                Value = value;
            }
        }

        [JsonIgnore]
        public long Integer
        {
            get => Value;
            set
            {
                Type = VariableDataTypes.Integer;
                Value = value;
            }
        }

        [JsonIgnore]
        public double Number
        {
            get => Value;
            set
            {
                Type = VariableDataTypes.Number;
                Value = value;
            }
        }

        [JsonIgnore]
        public string String
        {
            get => Value ?? "";
            set
            {
                Type = VariableDataTypes.String;
                Value = value ?? "";
            }
        }

        #endregion

        public override string ToString() => Value?.ToString() ?? "No representation";

        [JsonIgnore]
        [NotNull]
        public JObject Json
        {
            get => new JObject
            {
                {nameof(Type), (byte) Type},
                {nameof(Value), Value}
            };

            set
            {
                if (!value.TryGetValue(nameof(Type), out var typeToken))
                {
                    return;
                }

                if (typeToken.Type != JTokenType.Integer)
                {
                    throw new InvalidCastException($@"Expected 'Integer' for 'Type', received '{typeToken.Type.ToString()}'.");
                }

                if (!value.TryGetValue(nameof(Value), out var valueToken))
                {
                    return;
                }

                Type = (VariableDataTypes)typeToken.Value<byte>();
                Value = valueToken.Value<dynamic>();
            }
        }

        #region Operators

        public static implicit operator VariableValue(bool value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.Boolean,
                Boolean = value
            };
        }

        public static implicit operator bool([NotNull] VariableValue variableValue)
        {
            return variableValue.Boolean;
        }

        public static implicit operator VariableValue(long value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.String,
                Integer = value
            };
        }

        public static implicit operator long([NotNull] VariableValue variableValue)
        {
            return variableValue.Integer;
        }

        public static implicit operator VariableValue(double value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.Number,
                Number = value
            };
        }

        public static implicit operator double([NotNull] VariableValue variableValue)
        {
            return variableValue.Number;
        }

        public static implicit operator VariableValue([CanBeNull] string value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.String,
                String = value
            };
        }

        public static implicit operator string([NotNull] VariableValue variableValue)
        {
            return variableValue.String;
        }

        #endregion

        public static bool TryParse(string value, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(value);

                if (jObject != null)
                {
                    return true;
                }

                Log.Warn(
                    new ArgumentNullException(
                        nameof(jObject),
                        @"Invalid variable value stored in the database."
                    )
                );
            }
            catch (Exception exception)
            {
                jObject = null;
#if DEBUG
                /* Only log in DEBUG in case the variable contains
                 * sensitive information. */
                Log.Warn(exception);
#endif
            }

            return false;
        }
    }
}
