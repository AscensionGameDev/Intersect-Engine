using System;
using Newtonsoft.Json;


namespace Intersect.Client.Items
{

    public class MapItemInstance : Item
    {
        /// <summary>
        /// Defines the owner of this MapItem.
        /// </summary>
        public Guid Owner;

        /// <summary>
        /// Defines whether this MapItem is visible to everyone, or just its owner.
        /// </summary>
        public bool VisibleToAll;

        /// <summary>
        /// The Unique Id of this particular MapItemInstance so we can refer to it elsewhere.
        /// </summary>
        public Guid UniqueId;

        public MapItemInstance() : base()
        {
        }

        public MapItemInstance(string data) : base()
        {
            JsonConvert.PopulateObject(data, this);
        }

    }

}
