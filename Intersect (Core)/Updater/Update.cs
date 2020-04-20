using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Updater
{
    public class Update
    {
        public List<UpdateFile> Files { get; set; } = new List<UpdateFile>();
        public bool TrustCache { get; set; } = true;
        public string StreamingUrl { get; set; }

    }
}
