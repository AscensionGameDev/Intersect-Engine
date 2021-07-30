using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Items;
using Intersect.Network.Packets.Server;
using System;
using System.Collections.Immutable;

namespace Intersect.Client.Framework.Maps
{
    public interface IMapInstance
    {
        Guid Id { get; }
        string Name { get; }
        ImmutableList<IActionMessage> ActionMessages { get; }
        ImmutableList<IMapSound> AttributeSounds { get; }
        IMapSound BackgroundSound { get; }
        ImmutableList<IMapItemInstance> Items { get; }
        ImmutableList<IEntity> Entities { get; }
        ImmutableList<IMapAnimation> Animations { get; }
        ImmutableList<IEntity> Critters { get; }
        float X { get; }
        float Y { get; }
        int GridX { get; set; }
        int GridY { get; set; }
        bool IsLoaded { get; }

        void AddEvent(Guid evtId, EventEntityPacket packet);
        void AddTileAnimation(Guid animId, int tileX, int tileY, int dir = -1, IEntity owner = null);
        void CompareEffects(IMapInstance oldMap);
        bool InView();
        void Load(string json);
        void LoadTileData(byte[] packet);
        void Update(bool isLocal);
    }
}