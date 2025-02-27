using Intersect.Framework.Core.Serialization;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public abstract partial class EventCommand
{
    public abstract EventCommandType Type { get; }

    public virtual string GetCopyData(
        Dictionary<Guid, List<EventCommand>> commandLists,
        Dictionary<Guid, List<EventCommand>> copyLists
    )
    {
        return JsonConvert.SerializeObject(
            this,
            typeof(EventCommand),
            new JsonSerializerSettings()
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );
    }

    public virtual void FixBranchIds(Dictionary<Guid, Guid> idDict)
    {
    }

}