using System.Linq;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

// ReSharper disable All

namespace Intersect_Client.Classes.Core
{
    public static class GameMain
    {
        private static long _animTimer;
        private static bool _createdMapTextures;
        private static bool _loadedTilesets;

        public static void Start()
        {
            //Load Graphics
            GameGraphics.InitGraphics();

            //Load Sounds
            GameAudio.Init();
            GameAudio.PlayMusic(Globals.Database.MenuBGM, 3, 3, true);

            //Init Network
            GameNetwork.InitNetwork();
            GameFade.FadeIn();

            Globals.IsRunning = true;
        }

        public static void DestroyGame()
        {
            //Destroy Game
            //TODO - Destroy Graphics and Networking peacefully
            //Network.Close();
            Gui.DestroyGwen();
            GameGraphics.Renderer.Close();
        }

        public static void Update()
        {
            lock (Globals.GameLock)
            {
                GameNetwork.Update();
                GameFade.Update();
                if (Globals.GameState == GameStates.Intro)
                {
                    ProcessIntro();
                }
                else if (Globals.GameState == GameStates.Menu)
                {
                    ProcessMenu();
                }
                else if (Globals.GameState == GameStates.Loading)
                {
                    ProcessLoading();
                }
                if (Globals.GameState == GameStates.InGame)
                {
                    ProcessGame();
                }
                Globals.InputManager.Update();
                GameAudio.Update();
            }
        }

        private static void ProcessIntro()
        {
            if (Globals.Database.IntroBG.Count > 0)
            {
                GameTexture imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image,
                    Globals.Database.IntroBG[Globals.IntroIndex]);
                if (imageTex != null)
                {
                    if (Globals.IntroStartTime == -1)
                    {
                        if (GameFade.DoneFading())
                        {
                            if (Globals.IntroComing)
                            {
                                Globals.IntroStartTime = Globals.System.GetTimeMS();
                            }
                            else
                            {
                                Globals.IntroIndex++;
                                GameFade.FadeIn();
                                Globals.IntroComing = true;
                            }
                        }
                    }
                    else
                    {
                        if (Globals.System.GetTimeMS() > Globals.IntroStartTime + Globals.IntroDelay)
                        {
                            //If we have shown an image long enough, fade to black -- keep track that the image is going
                            GameFade.FadeOut();
                            Globals.IntroStartTime = -1;
                            Globals.IntroComing = false;
                        }
                    }
                }
                else
                {
                    Globals.IntroIndex++;
                }
                if (Globals.IntroIndex >= Globals.Database.IntroBG.Count)
                {
                    Globals.GameState = GameStates.Menu;
                }
            }
            else
            {
                Globals.GameState = GameStates.Menu;
            }
        }

        private static void ProcessMenu()
        {
            if (!Globals.JoiningGame) return;
            //if (GameGraphics.FadeAmt != 255f) return;
            //Check if maps are loaded and ready
            Globals.GameState = GameStates.Loading;
            Gui.DestroyGwen();
            PacketSender.SendEnterGame();
        }

        private static void ProcessLoading()
        {
            if (Globals.Me == null || Globals.Me.MapInstance == null) return;
            if (!_createdMapTextures)
            {
                if (Globals.Database.RenderCaching) GameGraphics.CreateMapTextures(9 * 18);
                _createdMapTextures = true;
            }
            if (!_loadedTilesets && Globals.HasGameData)
            {
                Globals.ContentManager.LoadTilesets(TilesetBase.GetNameList());
                _loadedTilesets = true;
            }
            if (Globals.Database.RenderCaching && Globals.Me != null && Globals.Me.MapInstance != null)
            {
                var gridX = Globals.Me.MapInstance.MapGridX;
                var gridY = Globals.Me.MapInstance.MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != -1)
                        {
                            var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                            if (map != null)
                            {
                                if (map.MapLoaded == false)
                                {
                                    return;
                                }
                                else if (map.MapRendered == false && Globals.Database.RenderCaching == true)
                                {
                                    lock (map.GetMapLock())
                                    {
                                        map.PreRenderMap();
                                    }
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }

            GameAudio.PlayMusic(MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).Music, 3, 3, true);
            Globals.GameState = GameStates.InGame;
            GameFade.FadeIn();
        }

        private static void ProcessGame()
        {
            //Update All Entities
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null && en.Value != Globals.Me) continue;
                en.Value.Update();
            }
            if (Globals.Me != null) Globals.Me.Update();

            for (int i = 0; i < Globals.EntitiesToDispose.Count; i++)
            {
                if (Globals.Entities.ContainsKey(Globals.EntitiesToDispose[i]))
                {
                    if (Globals.EntitiesToDispose[i] == Globals.Me.MyIndex) continue;
                    Globals.Entities[Globals.EntitiesToDispose[i]].Dispose();
                    Globals.Entities.Remove(Globals.EntitiesToDispose[i]);
                }
            }
            Globals.EntitiesToDispose.Clear();

            //Update Maps
            var maps = MapInstance.Lookup.Values.ToArray();
            foreach (MapInstance map in maps)
            {
                if (map == null) continue;
                map.Update(map.InView());
            }

            //If we are waiting on maps, lets see if we have them
            if (Globals.NeedsMaps)
            {
                bool canShowWorld = true;
                if (MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
                {
                    var gridX = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridX;
                    var gridY = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridY;
                    for (int x = gridX - 1; x <= gridX + 1; x++)
                    {
                        for (int y = gridY - 1; y <= gridY + 1; y++)
                        {
                            if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                                Globals.MapGrid[x, y] != -1)
                            {
                                var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                                if (map != null)
                                {
                                    if (map.MapLoaded == false)
                                    {
                                        canShowWorld = false;
                                    }
                                }
                                else
                                {
                                    canShowWorld = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    canShowWorld = false;
                }
                if (canShowWorld) Globals.NeedsMaps = false;
            }
            else
            {
                if (MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap) != null)
                {
                    var gridX = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridX;
                    var gridY = MapInstance.Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridY;
                    for (int x = gridX - 1; x <= gridX + 1; x++)
                    {
                        for (int y = gridY - 1; y <= gridY + 1; y++)
                        {
                            if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                                Globals.MapGrid[x, y] != -1)
                            {
                                var map = MapInstance.Lookup.Get<MapInstance>(Globals.MapGrid[x, y]);
                                if (map == null &&
                                    (!MapInstance.MapRequests.ContainsKey(Globals.MapGrid[x, y]) ||
                                     MapInstance.MapRequests[Globals.MapGrid[x, y]] < Globals.System.GetTimeMS()))
                                {
                                    //Send for the map
                                    PacketSender.SendNeedMap(Globals.MapGrid[x, y]);
                                }
                            }
                        }
                    }
                }
            }

            //Update Game Animations
            if (_animTimer < Globals.System.GetTimeMS())
            {
                Globals.AnimFrame++;
                if (Globals.AnimFrame == 3)
                {
                    Globals.AnimFrame = 0;
                }
                _animTimer = Globals.System.GetTimeMS() + 500;
            }

            //Remove Event Holds If Invalid
            for (int i = 0; i < Globals.EventHolds.Count; i++)
            {
                if (Globals.EventHolds[i].MapNum != -1 && MapInstance.Lookup.Get<MapInstance>(Globals.EventHolds[i].MapNum) == null)
                {
                    Globals.EventHolds.RemoveAt(i);
                }
            }

            GameGraphics.UpdatePlayerLight();
            ClientTime.Update();
        }

        public static void JoinGame()
        {
            Globals.LoggedIn = true;
            GameAudio.StopMusic(3f);
        }
    }
}