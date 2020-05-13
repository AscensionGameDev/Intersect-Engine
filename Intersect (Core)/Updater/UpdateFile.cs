using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Updater
{
    public class UpdateFile
    {
        public string Path { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public bool ClientIgnore { get; set; }
        public bool EditorIgnore { get; set; }

        public UpdateFile()
        {

        }

        public UpdateFile(string path, string hash, long size)
        {
            Path = path;
            Hash = hash;
            Size = size;
        }

    }
}
