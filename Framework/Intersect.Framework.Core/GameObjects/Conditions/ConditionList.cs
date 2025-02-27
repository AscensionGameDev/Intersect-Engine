using Intersect.Framework.Core.Serialization;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Conditions;

public partial class ConditionList
{
    public List<Condition> Conditions { get; set; } = []; //Long story.. just go with it.. okay?

    public string Name { get; set; } = "New Condition List";

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
            data,
            this,
            new JsonSerializerSettings()
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
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
                SerializationBinder = new IntersectTypeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            }
        );
    }
}
