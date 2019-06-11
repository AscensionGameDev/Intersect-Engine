using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

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
                        obj.Add("Value", mIntVal);
                        break;

                    case VariableDataTypes.Boolean:
                        obj.Add("Value", mBoolVal);
                        break;

                    default:
                        obj.Add("Value", "");
                        break;
                }

                return obj.ToString();
            }
            set
            {
                var obj = JObject.Parse(value);
                Type = (VariableDataTypes) (byte.Parse(obj["Type"].ToString()));
                switch (Type)
                {
                    case VariableDataTypes.Integer:
                        mIntVal = int.Parse(obj["Value"].ToString());
                        break;

                    case VariableDataTypes.Boolean:
                        mBoolVal = bool.Parse(obj["Value"].ToString());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        //Values
        private int mIntVal = 0;
        private bool mBoolVal = false;

        public void Set(int value)
        {
            mIntVal = value;
            Type = VariableDataTypes.Integer;
        }

        public void Set(bool value)
        {
            mBoolVal = value;
            Type = VariableDataTypes.Boolean;
        }

        public int GetIntValue()
        {
            return mIntVal;
        }

        public bool GetBooleanValue()
        {
            return mBoolVal;
        }
    }
}
