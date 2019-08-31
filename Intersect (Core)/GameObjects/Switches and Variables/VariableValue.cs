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
            get => CanConvertTo<bool>(Value) ? Convert.ToBoolean(Value) : false;
            set
            {
                Type = VariableDataTypes.Boolean;
                Value = value;
            }
        }

        [JsonIgnore]
        public long Integer
        {
            get => CanConvertTo<int>(Value) ? Convert.ToInt32(Value) : 0;
            set
            {
                Type = VariableDataTypes.Integer;
                Value = value;
            }
        }

        [JsonIgnore]
        public double Number
        {
            get => CanConvertTo<double>(Value) ? Convert.ToDouble(Value) : 0.0;
            set
            {
                Type = VariableDataTypes.Number;
                Value = value;
            }
        }

        [JsonIgnore]
        public string String
        {
            get => (Value as string) ?? "";
            set
            {
                Type = VariableDataTypes.String;
                Value = value ?? "";
            }
        }

        public static bool CanConvertTo<T>(object input)
        {
            Object result = null;
            try
            {
                result = Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        public override string ToString() => Value?.ToString() ?? "No Representation";

        public string ToString(VariableDataTypes forceType)
        {
            if (Value == null)
            {
                switch (forceType)
                {
                    case VariableDataTypes.Boolean:
                        Boolean = false;
                        break;
                    case VariableDataTypes.Integer:
                        Integer = (long) 0;
                        break;
                    case VariableDataTypes.Number:
                        Number = 0.0;
                        break;
                    case VariableDataTypes.String:
                        String = "";
                        break;
                }
            }
            return Value.ToString();
        }

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
                Value = valueToken.Type == JTokenType.Null ? null : valueToken.Value<dynamic>();
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
