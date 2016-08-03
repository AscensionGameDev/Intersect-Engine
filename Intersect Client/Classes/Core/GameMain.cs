/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Library.GameObjects;
using Intersect_Client.Classes.Maps;
using Intersect_Library;

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
            if (Globals.Me == null || MapInstance.GetMap(Globals.Me.CurrentMap) == null) return;
            if (!_createdMapTextures)
            {
                if (Globals.Database.RenderCaching) GameGraphics.CreateMapTextures(9*18);
                _createdMapTextures = true;
            }
            if (!_loadedTilesets && Globals.HasGameData)
            {
                Globals.ContentManager.LoadTilesets(DatabaseObject.GetGameObjectList(GameObject.Tileset));
                _loadedTilesets = true;
            }
            if (Globals.Database.RenderCaching && Globals.Me != null && MapInstance.GetMap(Globals.Me.CurrentMap) != null)
            {
                var gridX = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridX;
                var gridY = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = 0; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight)
                        {
                            var map = MapInstance.GetMap(Globals.MapGrid[x,y]);
                            if (map != null)
                            {
                                if (map.MapLoaded == false)
                                {
                                    return;
                                }
                                else if (map.MapRendered == false && Globals.Database.RenderCaching == true)
                                {
                                    map.PreRenderMap();
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
            
            GameAudio.PlayMusic(MapInstance.GetMap(Globals.Me.CurrentMap).Music, 3, 3, true);
            Globals.GameState = GameStates.InGame;
            GameFade.FadeIn();
        }

        private static void ProcessGame()
        {
            //Update All Entities
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null) continue;
                en.Value.Update();
            }

            for (int i = 0; i < Globals.EntitiesToDispose.Count; i++)
            {
                if (Globals.Entities.ContainsKey(Globals.EntitiesToDispose[i]))
                {
                    if (Globals.EntitiesToDispose[i] == Globals.Me.MyIndex) continue;
                    Globals.Entities.Remove(Globals.EntitiesToDispose[i]);
                }
            }
            Globals.EntitiesToDispose.Clear();

            //Update Maps
            foreach (var map in MapInstance.GetObjects().Values)
            {
                if (map == null) continue;
                map.Update(map.InView());
            }

            //If we are waiting on maps, lets see if we have them
            if (Globals.NeedsMaps)
            {
                bool canShowWorld = true;
                if (MapInstance.GetMap(Globals.Me.CurrentMap) != null)
                {
                    var gridX = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridX;
                    var gridY = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridY;
                    for (int x = gridX - 1; x <= gridX + 1; x++)
                    {
                        for (int y = 0; y <= gridY + 1; y++)
                        {
                            if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight)
                            {
                                var map = MapInstance.GetMap(Globals.MapGrid[x, y]);
                                if (map != null)
                                {
                                    if (map.MapLoaded == false)
                                    {
                                        canShowWorld = false;
                                    }
                                    else if (map.MapRendered == false && Globals.Database.RenderCaching == true)
                                    {
                                        map.PreRenderMap();
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

            //Update Game Animations
            if (_animTimer < Globals.System.GetTimeMS())
            {
                Globals.AnimFrame++;
                if (Globals.AnimFrame == 3) { Globals.AnimFrame = 0; }
                _animTimer = Globals.System.GetTimeMS() + 500;
            }

            //Remove Event Holds If Invalid
            for (int i = 0; i < Globals.EventHolds.Count; i++)
            {
                if (MapInstance.GetMap(Globals.EventHolds[i].MapNum) == null)
                {
                    Globals.EventHolds.RemoveAt(i);
                }
            }

            GameGraphics.UpdatePlayerLight();
        }

        public static void JoinGame()
        {
            Globals.LoggedIn = true;
            GameAudio.StopMusic(3f);
        }


    }


}
