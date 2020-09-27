using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Database;
using Intersect.Server.Database.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Handlers;

namespace Intersect.Server.Maps
{
    public static class MapLayer
    {

        public static bool LayersHaveChanged()
        {
            bool changed = false;

            lock (GameContext.Current)
            {
                try
                {
                    var dbLayers = GameContext.Current.MapLayers.OrderBy(l => l.ID).ToList();

                    if (dbLayers.Count != MapLayers.Layers.Count())
                    {
                        changed = true;
                    }
                        
                    if (!changed)
                    {
                        for (int x = 0; x < MapLayers.Layers.Count; x++)
                        {

                            var curLayer = MapLayers.Layers[x];

                            if (dbLayers[x].MapLayerRegionID != (int)curLayer.Region)
                            {
                                changed = true;
                                break;
                            }

                            if (dbLayers[x].IntersectLayerID != (int)curLayer.IntersectLayer)
                            {
                                changed = true;
                                break;
                            }

                        }
                    }
                } catch (Exception ex)
                {
                    Log.Error("Error determining if layers have changed definition. " + ex.Message);
                    changed = false;
                }

            }

            return changed;
        }

        public static bool AdjustLayers(ref byte[] LayerData)
        {
            TileArray[] NewLayers = new TileArray[MapLayers.Layers.Count];

            try
            {
                Ceras mCeras = new Ceras(false);
                TileArray[] OldLayers = mCeras.Decompress<TileArray[]>(LayerData);

                for (int x = 0; x < MapLayers.Layers.Count; x++)
                {
                    if (MapLayers.Layers[x].OldLayerID != -1)
                    {
                        if (OldLayers.Count() >= MapLayers.Layers[x].OldLayerID)
                        {
                            NewLayers[x] = OldLayers[MapLayers.Layers[x].OldLayerID];
                        }   
                        else
                        {
                            return false;
                        }  
                    }
                    else
                    {
                        NewLayers[x] = new TileArray();

                        NewLayers[x].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                        for (var z = 0; z < Options.MapWidth; z++)
                        {
                            for (var y = 0; y < Options.MapHeight; y++)
                            {
                                NewLayers[x].Tiles[z, y] = new Tile();
                            }
                        }
                    }

                }

                LayerData = mCeras.Compress(NewLayers);
            }
            catch (Exception ex)
            {
                Log.Error("Error adjusting layer changes. " + ex.Message);
                return false;
            }

            return true;
        }

        public static bool UpdateDBLayers()
        {
            bool blnSuccess = true;
            MapLayerBase tmpLayer;

            lock (GameContext.Current)
            {
                try
                {
                    for (int x = 0; x < MapLayers.Layers.Count; x++)
                    {

                        var layer = GameContext.Current.MapLayers.Where(l => l.ID == x).FirstOrDefault();

                        if (layer != null)
                            tmpLayer = layer;
                        else
                        {
                            tmpLayer = new MapLayerBase();

                            tmpLayer.ID = x;
                        }

                        tmpLayer.MapLayerRegionID = (int)MapLayers.Layers[x].Region;
                        tmpLayer.IntersectLayerID = (int)MapLayers.Layers[x].IntersectLayer;
                        tmpLayer.OldLayerID = MapLayers.Layers[x].OldLayerID;

                        if (layer != null)
                        {
                            GameContext.Current.MapLayers.Update(tmpLayer);
                        }
                        else
                        {
                            GameContext.Current.MapLayers.Add(tmpLayer);
                        }
                    }

                    // Check for more layers in db than current list
                    var layers = GameContext.Current.MapLayers.Where(l => l.ID >= MapLayers.Layers.Count);

                    if (layers != null)
                    {
                        GameContext.Current.MapLayers.RemoveRange(layers);
                    }

                    blnSuccess = true;
                } 
                catch (Exception ex)
                {
                    Log.Error("Error updating map layer changes in database. " + ex.Message);
                    blnSuccess = false;
                }

            }

            return blnSuccess;
        }

    }
}
