using System.Collections.Generic;

namespace Intersect.Updater
{
    public class Update
    {
        public List<UpdateFile> Files { get; set; } = new List<UpdateFile>();
        public bool TrustCache { get; set; } = true;
        public string StreamingUrl { get; set; }

    }
}
