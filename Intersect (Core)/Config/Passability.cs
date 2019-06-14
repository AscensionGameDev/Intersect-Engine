using Newtonsoft.Json;
using System.Runtime.Serialization;

using JetBrains.Annotations;

namespace Intersect.Config
{
    public class Passability
    {

        private bool[] mPassableCache;

        [JsonIgnore]
        [NotNull]
        public bool[] Passable
        {
            get => mPassableCache ?? (mPassableCache = new[] { Normal, Safe, Arena });
            set => mPassableCache = value;
        }

        //Can players move through each other on the following map types/moralities
        public bool Normal = false;

        public bool Safe = true;

        public bool Arena = false;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Passable = new[] { Normal, Safe, Arena };
        }

    }
}
