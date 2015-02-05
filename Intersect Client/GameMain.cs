using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Client
{
    public static class GameMain
    {
        public static bool isRunning = true;
        private static long animTimer = 0;
        private static long attackTimer;
        public static void StartGame()
        {
            //Load Content/Options
            Database.CheckDirectories();
            Database.LoadOptions();

            //Load Graphics
            Graphics.InitGraphics();

            //Init Network
            Network.InitNetwork();

            //Start Game Loop
            while (isRunning)
            {
                Network.CheckNetwork();
                Graphics.DrawGame();
                if (Globals.GameState == 0)
                {
                    ProcessMenu();
                }
                else
                {
                    ProcessGame();
                }
                System.Windows.Forms.Application.DoEvents();
            }

            //Destroy Game
            //TODO - Destroy Graphics and Networking peacefully
            GUI.DestroyGwen();
            Graphics.renderWindow.Close();
            System.Windows.Forms.Application.Exit();
        }

        private static void ProcessMenu()
        {
            if (Globals.JoiningGame)
            {
                if (Graphics.fadeAmt == 255f)
                {
                    //Check if maps are loaded and ready
                    Globals.GameState = 1;
                    GUI.DestroyGwen();
                    GUI.InitGwen();
                    Console.WriteLine("Sent enter game");
                    PacketSender.SendEnterGame();
                }
            }

        }

        private static void ProcessGame()
        {
            if (!Globals.gameLoaded)
            {
                if (Globals.localMaps[4] == -1) { return; }
                if (Globals.GameMaps != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (Globals.localMaps[i] > -1)
                        {
                            if (Globals.GameMaps[Globals.localMaps[i]] != null)
                            {
                                if (Globals.GameMaps[Globals.localMaps[i]].shouldLoad(i * 2 + 0))
                                {
                                    if (Globals.GameMaps[Globals.localMaps[i]].mapLoaded == false)
                                    {
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                    Globals.gameLoaded = true;
                    Graphics.fadeStage = 1;
                }
                return;
            }

            HandleMovement();
            if (Graphics.mouseState.IndexOf(SFML.Window.Mouse.Button.Left) >= 0) { TryAttack(); }


            for (int i = 0; i < 9; i++)
            {
                for (int z = 0; z < Globals.entities.Count; z++)
                {
                    if (Globals.entities[z] != null)
                    {
                        if (Globals.entities[z].currentMap == Globals.localMaps[i])
                        {
                            if (z == Globals.myIndex)
                            {
                                Globals.entities[z].Update(true);
                            }
                            else
                            {
                                Globals.entities[z].Update(false);
                            }
                        }
                    }
                }
                for (int z = 0; z < Globals.events.Count; z++)
                {
                    if (Globals.events[z] != null)
                    {
                        if (Globals.events[z].currentMap == Globals.localMaps[i])
                        {
                            Globals.events[z].Update();
                        }
                    }
                }
            }


            if (animTimer < Environment.TickCount)
            {
                Globals.animFrame++;
                if (Globals.animFrame == 3) { Globals.animFrame = 0; }
                animTimer = Environment.TickCount + 500;
            }
        }

        private static bool TryAttack()
        {
            if (attackTimer > Environment.TickCount) { return false; }
            int x = Globals.entities[Globals.myIndex].currentX;
            int y = Globals.entities[Globals.myIndex].currentY;
            int map = Globals.entities[Globals.myIndex].currentMap;
            switch (Globals.entities[Globals.myIndex].dir)
            {
                case 0:
                    y--;
                    break;
                case 1:
                    y++;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    x++;
                    break;
            }
            int tmpX = x;
            int tmpY = y;
            int tmpMap = map;
            int tmpI = -1;
            for (int i = 0; i < 9; i++)
            {
                if (Globals.localMaps[i] == map)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (tmpI == -1)
            {
                return true;
            }
            try
            {
                if (x < 0)
                {
                    tmpX = 29 - (x * -1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.localMaps[tmpI - 4] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI - 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.localMaps[tmpI + 2] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI + 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.localMaps[tmpI - 1] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI - 1];
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (x > 29)
                {
                    tmpX = x - 29;
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.localMaps[tmpI - 2] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI - 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.localMaps[tmpI + 4] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI + 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.localMaps[tmpI + 1] > -1)
                        {
                            tmpMap = Globals.localMaps[tmpI + 1];
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (y < 0)
                {
                    tmpX = x;
                    tmpY = 29 - (y * -1);
                    if (Globals.localMaps[tmpI - 3] > -1)
                    {
                        tmpMap = Globals.localMaps[tmpI - 3];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (y > 29)
                {
                    tmpX = x;
                    tmpY = y - 29;
                    if (Globals.localMaps[tmpI + 3] > -1)
                    {
                        tmpMap = Globals.localMaps[tmpI + 3];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                    if (Globals.localMaps[tmpI] > -1)
                    {
                        tmpMap = Globals.localMaps[tmpI];
                    }
                    else
                    {
                        return false;
                    }
                }
                for (int i = 0; i < Globals.entities.Count; i++)
                {
                    if (i != Globals.myIndex)
                    {
                        if (Globals.entities[i] != null)
                        {
                            if (Globals.entities[i].currentMap == tmpMap && Globals.entities[i].currentX == tmpX && Globals.entities[i].currentY == tmpY)
                            {
                                //ATTACKKKKK!!!
                                PacketSender.SendAttack(i);
                                attackTimer = Environment.TickCount + 1000;
                                return true;
                            }
                        }
                    }
                }
                for (int i = 0; i < Globals.events.Count; i++)
                {
                    if (Globals.events[i] != null)
                    {
                        if (Globals.events[i].currentMap == tmpMap && Globals.events[i].currentX == tmpX && Globals.events[i].currentY == tmpY)
                        {
                            //ATTACKKKKK!!!
                            PacketSender.SendActivateEvent(i);
                            attackTimer = Environment.TickCount + 1000;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;

            }
        }

        public static void HandleMovement()
        {
            float movex = 0f;
            float movey = 0f;
            if (GUI.HasInputFocus()) { return; }
            if (Graphics.keyStates.IndexOf(SFML.Window.Keyboard.Key.W) >= 0) { movey = 1; }
            if (Graphics.keyStates.IndexOf(SFML.Window.Keyboard.Key.S) >= 0) { movey = -1; }
            if (Graphics.keyStates.IndexOf(SFML.Window.Keyboard.Key.A) >= 0) { movex = -1; }
            if (Graphics.keyStates.IndexOf(SFML.Window.Keyboard.Key.D) >= 0) { movex = 1; }
            Globals.entities[Globals.myIndex].moveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                if (movey < 0)
                {
                    Globals.entities[Globals.myIndex].moveDir = 1;
                }
                if (movey > 0)
                {
                    Globals.entities[Globals.myIndex].moveDir = 0;
                }
                if (movex < 0)
                {
                    Globals.entities[Globals.myIndex].moveDir = 2;
                }
                if (movex > 0)
                {
                    Globals.entities[Globals.myIndex].moveDir = 3;
                }

            }
        }
    }


}
