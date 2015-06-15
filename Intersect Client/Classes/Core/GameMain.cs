using System;
using System.Windows.Forms;
using Intersect_Client.Classes;
using SFML.Window;

// ReSharper disable All

namespace Intersect_Client
{
    public static class GameMain
    {
        public static bool IsRunning = true;
        private static long _animTimer;
        private static long _attackTimer;
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
            while (IsRunning)
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
                Application.DoEvents();
            }

            //Destroy Game
            //TODO - Destroy Graphics and Networking peacefully
            Gui.DestroyGwen();
            Graphics.RenderWindow.Close();
            Application.Exit();
        }

        private static void ProcessMenu()
        {
            if (!Globals.JoiningGame) return;
            if (Graphics.FadeAmt != 255f) return;
            //Check if maps are loaded and ready
            Globals.GameState = 1;
            Gui.DestroyGwen();
            Gui.InitGwen();
            PacketSender.SendEnterGame();
        }

        private static void ProcessGame()
        {
            if (!Globals.GameLoaded)
            {
                if (Globals.LocalMaps[4] == -1) { return; }
                if (Globals.GameMaps == null) return;
                for (var i = 0; i < 9; i++)
                {
                    if (Globals.LocalMaps[i] <= -1) continue;
                    if (Globals.GameMaps[Globals.LocalMaps[i]] != null)
                    {
                        if (!Globals.GameMaps[Globals.LocalMaps[i]].ShouldLoad(i*2 + 0)) continue;
                        if (Globals.GameMaps[Globals.LocalMaps[i]].MapLoaded == false)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                Globals.GameLoaded = true;
                Graphics.FadeStage = 1;
                return;
            }

            HandleMovement();
            if (Graphics.MouseState.IndexOf(Mouse.Button.Left) >= 0) { TryAttack(); }


            for (var i = 0; i < 9; i++)
            {
                for (var z = 0; z < Globals.Entities.Count; z++)
                {
                    if (Globals.Entities[z] == null) continue;
                    if (Globals.Entities[z].CurrentMap != Globals.LocalMaps[i]) continue;
                    if (z == Globals.MyIndex)
                    {
                        Globals.Entities[z].Update(true);
                    }
                    else
                    {
                        Globals.Entities[z].Update(false);
                    }
                }
                for (var z = 0; z < Globals.Events.Count; z++)
                {
                    if (Globals.Events[z] != null)
                    {
                        if (Globals.Events[z].CurrentMap == Globals.LocalMaps[i])
                        {
                            Globals.Events[z].Update();
                        }
                    }
                }
            }


            if (_animTimer < Environment.TickCount)
            {
                Globals.AnimFrame++;
                if (Globals.AnimFrame == 3) { Globals.AnimFrame = 0; }
                _animTimer = Environment.TickCount + 500;
            }
        }

        private static bool TryAttack()
        {
            if (_attackTimer > Environment.TickCount) { return false; }
            var x = Globals.Entities[Globals.MyIndex].CurrentX;
            var y = Globals.Entities[Globals.MyIndex].CurrentY;
            var map = Globals.Entities[Globals.MyIndex].CurrentMap;
            switch (Globals.Entities[Globals.MyIndex].Dir)
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
            var tmpX = x;
            var tmpY = y;
            var tmpMap = map;
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == map)
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
                        if (Globals.LocalMaps[tmpI - 4] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 2] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI - 1] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 1];
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
                        if (Globals.LocalMaps[tmpI - 2] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 4] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI + 1] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 1];
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
                    if (Globals.LocalMaps[tmpI - 3] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI - 3];
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
                    if (Globals.LocalMaps[tmpI + 3] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI + 3];
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
                    if (Globals.LocalMaps[tmpI] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI];
                    }
                    else
                    {
                        return false;
                    }
                }
                for (var i = 0; i < Globals.Entities.Count; i++)
                {
                    if (i != Globals.MyIndex)
                    {
                        if (Globals.Entities[i] != null)
                        {
                            if (Globals.Entities[i].CurrentMap == tmpMap && Globals.Entities[i].CurrentX == tmpX && Globals.Entities[i].CurrentY == tmpY)
                            {
                                //ATTACKKKKK!!!
                                PacketSender.SendAttack(i);
                                _attackTimer = Environment.TickCount + 1000;
                                return true;
                            }
                        }
                    }
                }
                for (var i = 0; i < Globals.Events.Count; i++)
                {
                    if (Globals.Events[i] != null)
                    {
                        if (Globals.Events[i].CurrentMap == tmpMap && Globals.Events[i].CurrentX == tmpX && Globals.Events[i].CurrentY == tmpY)
                        {
                            //ATTACKKKKK!!!
                            PacketSender.SendActivateEvent(i);
                            _attackTimer = Environment.TickCount + 1000;
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
            var movex = 0f;
            var movey = 0f;
            if (Gui.HasInputFocus()) { return; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.W) >= 0) { movey = 1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.S) >= 0) { movey = -1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.A) >= 0) { movex = -1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.D) >= 0) { movex = 1; }
            Globals.Entities[Globals.MyIndex].MoveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                if (movey < 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 1;
                }
                if (movey > 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 0;
                }
                if (movex < 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 2;
                }
                if (movex > 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 3;
                }

            }
        }
    }


}
