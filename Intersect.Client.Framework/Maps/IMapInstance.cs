using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Items;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Intersect.Client.Framework.Maps
{
    public interface IMapInstance
    {
        Guid Id { get; }
        List<IWeatherParticle> _removeParticles { get; set; }
        List<IWeatherParticle> _weatherParticles { get; set; }
        List<IActionMessage> ActionMsgs { get; set; }
        List<IMapSound> AttributeSounds { get; set; }
        IMapSound BackgroundSound { get; set; }
        bool[] CameraHolds { get; set; }
        Dictionary<Guid, ICritter> Critters { get; set; }
        ConcurrentDictionary<Guid, IMapAnimation> LocalAnimations { get; set; }
        Dictionary<Guid, IEntity> LocalEntities { get; set; }
        List<Guid> LocalEntitiesToDispose { get; set; }
        int MapGridX { get; set; }
        int MapGridY { get; set; }
        Dictionary<int, List<IMapItemInstance>> MapItems { get; set; }
        bool MapLoaded { get; }

        void AddEvent(Guid evtId, EventEntityPacket packet);
        void AddTileAnimation(Guid animId, int tileX, int tileY, int dir = -1, IEntity owner = null);
        void BuildVBOs();
        void CompareEffects(IMapInstance oldMap);
        void CreateMapSounds();
        void Delete();
        void DestroyVBOs();
        void Dispose(bool prep = true, bool killentities = true);
        void Draw(int layer = 0);
        void DrawActionMsgs();
        void DrawFog();
        void DrawItemNames();
        void DrawItemsAndLights();
        void DrawOverlayGraphic();
        void DrawPanorama();
        void DrawWeather();
        MapBase[,] GenerateAutotileGrid();
        float GetX();
        float GetY();
        void GridSwitched();
        bool InView();
        void Load(string json);
        void LoadTileData(byte[] packet);
        void Update(bool isLocal);
    }
}