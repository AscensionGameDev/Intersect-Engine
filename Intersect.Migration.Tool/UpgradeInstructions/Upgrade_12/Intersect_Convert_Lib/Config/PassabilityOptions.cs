using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config
{
    public class PassabilityOptions
    {
        //Can players move through each other on the following map types/moralities
        public bool Normal = false;
        public bool Safe = true;
        public bool Arena = false;

        [JsonIgnore] public bool[] Passable;

        public PassabilityOptions()
        {
            Passable = new bool[]
            {
                Normal,
                Safe,
                Arena
            };
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Passable = new bool[]
            {
                Normal,
                Safe,
                Arena
            };
        }
    }
}
