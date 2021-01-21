using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.GameObjects.Switches_and_Variables
{

    public class VariableValue
    {

        private dynamic mValue;

        public VariableDataTypes Type { get; set; }

        [NotMapped]
        public dynamic Value { get => mValue; set => SetValue(value); }

        [JsonIgnore]
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
                    throw new InvalidCastException(
                        $@"Expected 'Integer' for 'Type', received '{typeToken.Type.ToString()}'."
                    );
                }

                if (!value.TryGetValue(nameof(Value), out var valueToken))
                {
                    return;
                }

                Type = (VariableDataTypes) typeToken.Value<byte>();
                Value = valueToken.Type == JTokenType.Null ? null : valueToken.Value<dynamic>();
            }
        }

        public void SetValue(object value)
        {
            //Doing these also updates the variable data types appropriately.
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                {
                    Boolean = (bool) value;
                }

                if (value.GetType() == typeof(long))
                {
                    Integer = (long) value;
                }

                if (value.GetType() == typeof(double))
                {
                    Number = (double) value;
                }

                if (value.GetType() == typeof(string))
                {
                    String = (string) value;
                }
            }

            mValue = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "No Representation";
        }

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

        public static bool TryParse(string value, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(value);

                if (jObject != null)
                {
                    return true;
                }

                Log.Warn(new ArgumentNullException(nameof(jObject), @"Invalid variable value stored in the database."));
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

        #region Typed Properties

        [JsonIgnore]
        public bool Boolean
        {
            get => CanConvertTo<bool>(Value) ? Convert.ToBoolean(Value) : false;
            set
            {
                Type = VariableDataTypes.Boolean;
                mValue = value;
            }
        }

        [JsonIgnore]
        public long Integer
        {
            get => CanConvertTo<long>(Value) ? Convert.ToInt64(Value) : 0;
            set
            {
                Type = VariableDataTypes.Integer;
                mValue = value;
            }
        }

        [JsonIgnore]
        public double Number
        {
            get => CanConvertTo<double>(Value) ? Convert.ToDouble(Value) : 0.0;
            set
            {
                Type = VariableDataTypes.Number;
                mValue = value;
            }
        }

        [JsonIgnore]
        public string String
        {
            get => CanConvertTo<string>(Value) ? Convert.ToString(Value) : "";
            set
            {
                Type = VariableDataTypes.String;
                mValue = value ?? "";
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

        #region Operators

        public static implicit operator VariableValue(bool value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.Boolean,
                Boolean = value
            };
        }

        public static implicit operator bool(VariableValue variableValue)
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

        public static implicit operator long(VariableValue variableValue)
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

        public static implicit operator double(VariableValue variableValue)
        {
            return variableValue.Number;
        }

        public static implicit operator VariableValue(string value)
        {
            return new VariableValue
            {
                Type = VariableDataTypes.String,
                String = value
            };
        }

        public static implicit operator string(VariableValue variableValue)
        {
            return variableValue.String;
        }

        #endregion

    }

}
