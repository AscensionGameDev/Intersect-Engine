using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.GameObjects.Switches_and_Variables
{
    public class VariableValue
    {
        public VariableDataTypes Type { get; set; }
        public string JsonValue
        {
            get
            {
                var obj = new JObject();
                obj.Add("Type", (byte)Type);
                switch (Type)
                {
                    case VariableDataTypes.Integer:
                        obj.Add("Value", Integer);
                        break;

                    case VariableDataTypes.Boolean:
                        obj.Add("Value", Boolean);
                        break;

                    case VariableDataTypes.Number:
                        obj.Add("Value", Number);
                        break;

                    case VariableDataTypes.String:
                        obj.Add("Value", String);
                        break;

                    default:
                        obj.Add("Value", "");
                        break;
                }

                return obj.ToString(Formatting.None);
            }
            set
            {
                var obj = JObject.Parse(value);
                Type = (VariableDataTypes) (byte.Parse(obj["Type"].ToString()));
                switch (Type)
                {
                    case VariableDataTypes.Integer:
                        Integer = long.Parse(obj["Value"].ToString());
                        break;

                    case VariableDataTypes.Boolean:
                        Boolean = bool.Parse(obj["Value"].ToString());
                        break;

                    case VariableDataTypes.Number:
                        Number = double.Parse(obj["Value"].ToString());
                        break;

                    case VariableDataTypes.String:
                        String = obj["Value"].ToString();
                        break;

                    default:
                        break;
                }

            }
        }

        private long mInteger;
        [JsonIgnore]
        public long Integer
        {
            get => mInteger;
            set
            {
                Type = VariableDataTypes.Integer;
                mInteger = value;
            }
        }

        private bool mBoolean;
        [JsonIgnore]
        public bool Boolean
        {
            get => mBoolean;
            set
            {
                Type = VariableDataTypes.Boolean;
                mBoolean = value;
            }
        }

        private double mNumber;
        [JsonIgnore]
        public double Number
        {
            get => mNumber;
            set
            {
                Type = VariableDataTypes.Number;
                mNumber = value;
            }
        }

        private string mString;
        [JsonIgnore]
        public string String
        {
            get => mString ?? "";
            set
            {
                Type = VariableDataTypes.String;
                mString = value ?? "";
            }
        }

        public string StringRepresentation(VariableDataTypes ofType)
        {
            switch (ofType)
            {
                case VariableDataTypes.Boolean:
                    return Boolean.ToString();

                case VariableDataTypes.Integer:
                    return Integer.ToString();

                case VariableDataTypes.Number:
                    break;

                case VariableDataTypes.String:
                    break;
            }

            return "No Representation";
        }

    }
}
