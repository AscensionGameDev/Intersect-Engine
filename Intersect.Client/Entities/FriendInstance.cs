using Intersect.Client.Framework.Entities;

namespace Intersect.Client.Entities
{
    public class FriendInstance : IFriendInstance
    {

        public string Map { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; } = false;

    }
}
