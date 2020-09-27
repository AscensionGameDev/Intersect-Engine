using Intersect.Enums;
using Intersect.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.GameObjects.Maps
{

    public static class MapLayers
    {

        public static List<MapLayerInfo> Layers { get; }

        static MapLayers()
        {

            Layers = new List<MapLayerInfo>();

            // Below Player
            Layers.Add(new MapLayerInfo(MapLayerRegion.Lower, MapLayerIntersect.Ground));
            Layers.Add(new MapLayerInfo(MapLayerRegion.Lower, MapLayerIntersect.Mask));
            Layers.Add(new MapLayerInfo(MapLayerRegion.Lower, MapLayerIntersect.Mask2));

            // Above Player
            Layers.Add(new MapLayerInfo(MapLayerRegion.Middle, MapLayerIntersect.Fringe));

            // Above Player
            Layers.Add(new MapLayerInfo(MapLayerRegion.Upper, MapLayerIntersect.Fringe2));
        }

        public static bool isValid()
        {
            int tmpID = 0;
            int tmpIntersectID = 0;
            bool blnFound = false;

            try
            {
                // Make sure layer regions and intersect layers are in order 
                for (int x = 0; x < Layers.Count; x++)
                {
                    var tmpLayerRegion = Layers[x].Region;
                    var tmpIntersectLayer = (int)Layers[x].IntersectLayer;

                    if ((int)tmpID > (int)tmpLayerRegion)
                    {
                        return false;
                    }

                    if ((int)tmpIntersectLayer >= (int)MapLayerIntersect.Ground || 
                        (int)tmpIntersectID > (int)tmpIntersectLayer
                    )
                    {
                        return false;
                    }

                    tmpID = (int)Layers[x].Region;

                    if ((int)tmpIntersectLayer >= (int)MapLayerIntersect.Ground)
                    {
                        tmpIntersectID = (int)tmpIntersectLayer;
                    }
                        
                }

                // Make sure all intersect layers are found and only 1 of each
                for (int x = 0; x < 5; x++)
                {

                    blnFound = false;
                    for (int y = 0; y < Layers.Count; y++)
                    {
                        if ((int)Layers[y].IntersectLayer == (int)MapLayerIntersect.None)
                        {
                            continue;
                        }
                            
                        if ((int)Layers[y].IntersectLayer == x)
                        {
                            blnFound = true;

                            for (int z = 0; z < Layers.Count; z++)
                            {
                                if ((int)Layers[z].IntersectLayer == x && z != y)
                                {
                                    return false;
                                }  
                            }

                        }

                    }

                    if (!blnFound)
                    {
                        return false;
                    } 

                }

                // make sure old layer is only mentioned once
                for (int x = 0; x < Layers.Count; x++)
                {
                    if (Layers[x].OldLayerID == -1)
                    {
                        continue;
                    } 

                    for (int y = 0; y < Layers.Count; y++)
                    {
                        if (Layers[x].OldLayerID == Layers[y].OldLayerID && x != y)
                        {
                            return false;
                        }
                            
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error determining map layer definitions are valid. " + ex.Message);
                return false;
            }

            return true;
        }

    }

    public class MapLayerInfo
    {

        public MapLayerRegion Region { get; set; }

        public MapLayerIntersect IntersectLayer { get; set; }

        public int OldLayerID { get; set; }

        public MapLayerInfo(MapLayerRegion _region, MapLayerIntersect _intersectlayer, int _oldlayerID = -1)
        {
            Region = _region;
            IntersectLayer = _intersectlayer;
            OldLayerID = _oldlayerID;
        }

    }
}
