using Intersect.Framework.Core.Serialization;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Conditions;

[JsonConverter(typeof(ConditionListsSerializer))]
public partial class ConditionLists
{
    public List<ConditionList> Lists { get; set; } = [];

    public ConditionLists()
    {
    }

    public ConditionLists(string data)
    {
        Load(data);
    }

    public int Count => Lists?.Count ?? 0;

    public void Load(string data)
    {
        Lists.Clear();
        JsonConvert.PopulateObject(
            data,
            Lists,
            new JsonSerializerSettings()
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            }
        );
    }

    public string Data()
    {
        return JsonConvert.SerializeObject(
            Lists,
            new JsonSerializerSettings()
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            }
        );
    }
}