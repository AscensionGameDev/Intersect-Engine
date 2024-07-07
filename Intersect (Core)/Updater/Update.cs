namespace Intersect.Updater;

public partial class Update
{
    public List<UpdateFile> Files { get; set; } = new List<UpdateFile>();
    
    public string StreamingUrl { get; set; }

    public bool TrustCache { get; set; } = true;

    public Update Filter(bool isClient)
    {
        return new Update
        {
            Files = Files.Where(file => isClient ? !file.ClientIgnore : !file.EditorIgnore).ToList(),
            StreamingUrl = StreamingUrl,
            TrustCache = TrustCache,
        };
    }
}
