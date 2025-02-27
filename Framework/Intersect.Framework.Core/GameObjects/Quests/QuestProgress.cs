using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class QuestProgress
{
    public bool Completed;

    public Guid TaskId;

    public int TaskProgress;

    public QuestProgress(string data)
    {
        JsonConvert.PopulateObject(data, this);
    }
}