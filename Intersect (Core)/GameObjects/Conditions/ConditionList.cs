using System.Collections.Generic;

using Intersect.GameObjects.Events;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Conditions
{

    public class ConditionList
    {

        public List<Condition> Conditions = new List<Condition>(); //Long story.. just go with it.. okay?

        public string Name = "New Condition List";

        public ConditionList()
        {
        }

        public ConditionList(string data)
        {
            Load(data);
        }

        public void Load(string data)
        {
            JsonConvert.PopulateObject(
                data, this,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        public string Data()
        {
            return JsonConvert.SerializeObject(
                this,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
                }
            );
        }

    }

}
