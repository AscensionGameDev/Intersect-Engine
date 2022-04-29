using Intersect.Client.Framework.Entities;

namespace Intersect.Editor.Entities
{
    public class FriendInstance : IFriendInstance
    {

        public string Map { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; } = false;

    }
}
