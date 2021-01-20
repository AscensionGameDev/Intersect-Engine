using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Intersect.Config
{

    public class Passability
    {

        public bool Arena = false;

        private bool[] mPassableCache;

        //Can players move through each other on the following map types/moralities
        public bool Normal = false;

        public bool Safe = true;

        [JsonIgnore]
        public bool[] Passable
        {
            get => mPassableCache ?? (mPassableCache = new[] {Normal, Safe, Arena});
            set => mPassableCache = value;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Passable = new[] {Normal, Safe, Arena};
        }

    }

}
