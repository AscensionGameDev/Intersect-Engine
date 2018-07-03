using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Conditions
{
    public class ConditionLists
    {
        public List<ConditionList> Lists = new List<ConditionList>();

        public ConditionLists()
        {
        }

        public ConditionLists(string data)
        {
            Load(data);
        }

        public void Load(string data)
        {
            JsonConvert.PopulateObject(data, this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.Ignore, ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public string Data()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.Ignore });
        }
    }
}