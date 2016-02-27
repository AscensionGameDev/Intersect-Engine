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

using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

// ReSharper disable All

namespace Intersect_Client.Classes.Core
{
    public static class GameMain
    {
        private static long _animTimer;
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
                if (Globals.GameState == Enums.GameStates.Intro)
                {
                    ProcessIntro();
                }
                else if (Globals.GameState == Enums.GameStates.Menu)
                {
                    ProcessMenu();
                }
                else if (Globals.GameState == Enums.GameStates.Loading)
                {
                    ProcessLoading();
                }
                if (Globals.GameState == Enums.GameStates.InGame)
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
                if (GameGraphics.ImageFileNames.IndexOf(Globals.Database.IntroBG[Globals.IntroIndex]) > -1)
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
                    Globals.GameState = Enums.GameStates.Menu;
                }
            }
            else
            {
                Globals.GameState = Enums.GameStates.Menu;
            }
        }

        private static void ProcessMenu()
        {
            if (!Globals.JoiningGame) return;
            //if (GameGraphics.FadeAmt != 255f) return;
            //Check if maps are loaded and ready
            Globals.GameState = Enums.GameStates.Loading;
            Gui.DestroyGwen();
            PacketSender.SendEnterGame();
        }

        private static void ProcessLoading()
        {
            if (Globals.Tilesets != null && Globals.Tilesets.Length > GameGraphics.Tilesets.Count)
            {
                Globals.ContentManager.LoadTilesets(Globals.Tilesets);
            }
            if (Globals.LocalMaps[4] == -1) { return; }
            if (Globals.GameMaps == null) return;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] <= -1) continue;
                if (Globals.GameMaps.ContainsKey(Globals.LocalMaps[i]) && Globals.GameMaps[Globals.LocalMaps[i]] != null)
                {
                    if (!Globals.GameMaps[Globals.LocalMaps[i]].ShouldLoad(i * 2 + 0)) continue;
                    if (Globals.GameMaps[Globals.LocalMaps[i]].MapLoaded == false)
                    {
                        return;
                    }
                    else if (Globals.GameMaps[Globals.LocalMaps[i]].MapRendered == false && Globals.Database.RenderCaching == true)
                    {
                        Globals.GameMaps[Globals.LocalMaps[i]].PreRenderMap();
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            

            GameAudio.PlayMusic(Globals.GameMaps[Globals.LocalMaps[4]].Music, 3, 3, true);
            Globals.GameState = Enums.GameStates.InGame;
            GameFade.FadeIn();
        }

        private static void ProcessGame()
        {
            //Reset Single-Frame Variables
            GameGraphics.FogOffsetX = 0;
            GameGraphics.FogOffsetY = 0;

            if (Globals.GameMaps.ContainsKey(Globals.CurrentMap))
            {
                GameGraphics.SunIntensity = (float) (Globals.GameMaps[Globals.CurrentMap].Brightness/100f)*255f;
            }


            //Update All Entities

            foreach (var en in Globals.Entities)
            {
                if (en.Value == null) continue;
                en.Value.Update();
            }
            foreach (var en in Globals.LocalEntities)
            {
                if (en.Value == null) continue;
                en.Value.Update();
            }

            for (int i = 0; i < Globals.EntitiesToDispose.Count; i++)
            {
                if (Globals.Entities[Globals.EntitiesToDispose[i]] == Globals.Me) continue;
                Globals.Entities.Remove(Globals.EntitiesToDispose[i]);
            }
            Globals.EntitiesToDispose.Clear();
            for (int i = 0; i < Globals.LocalEntitiesToDispose.Count; i++)
            {
                Globals.LocalEntities.Remove(Globals.LocalEntitiesToDispose[i]);
            }
            Globals.LocalEntitiesToDispose.Clear();

            //Update Maps
            bool handled = false;
            foreach (var map in Globals.GameMaps)
            {
                handled = false;
                if (map.Value == null) continue;
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    if (map.Value.MyMapNum == Globals.LocalMaps[i])
                    {
                        map.Value.Update(true);
                        handled = true;
                    }
                }
                if (!handled)
                {
                    map.Value.Update(false);
                }
            }

            for (int i = 0; i < Globals.MapsToRemove.Count; i++)
            {
                Globals.GameMaps[Globals.MapsToRemove[i]].Dispose(false);
            }
            Globals.MapsToRemove.Clear();


            //Update Game Animations
            if (_animTimer < Globals.System.GetTimeMS())
            {
                Globals.AnimFrame++;
                if (Globals.AnimFrame == 3) { Globals.AnimFrame = 0; }
                _animTimer = Globals.System.GetTimeMS() + 500;
            }
        }

        public static void JoinGame()
        {
            Globals.LoggedIn = true;
            GameAudio.StopMusic(3f);
        }


    }


}
