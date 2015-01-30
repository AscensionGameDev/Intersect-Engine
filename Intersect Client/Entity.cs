using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace Intersect_Client
{
    public class Entity
    {
        //Core Values
        public int myIndex;
        public bool isLocal = false;
        public string myName = "";
        public string mySprite = "";
        public bool inView = true;
        public int passable = 0;
        public int hideName = 0;

        //Location Info
        public int currentX = 0;
        public int currentY = 0;
        public int currentMap = -1;
        public int dir = 0;
        public bool isMoving = false;
        public float offsetX;
        public float offsetY;
        public int moveDir = -1;
        public float moveTimer;
        public float spriteWidth;
        public float spriteHeight;

        //Vitals & Stats
        public int[] maxVital = new int[2];
        public int[] vital = new int[2];
        public int[] stat = new int[3];

        private long lastUpdate = 0;
        private long walkTimer;
        private int walkFrame;

        public Entity()
        {

        }

        public void Update(bool local = false)
        {
            bool didMove = false;
            if (lastUpdate == 0) { lastUpdate = Environment.TickCount; }
            long ecTime = Environment.TickCount - lastUpdate;
            int tmpI = -1;
            for (int i = 0; i < 9; i++)
            {
                if (Globals.localMaps[i] == currentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (walkTimer < Environment.TickCount)
            {
                if (isMoving)
                {
                    walkFrame++;
                    if (walkFrame > 3) { walkFrame = 0; }
                }
                else
                {
                    walkFrame = 0;
                }
                walkTimer = Environment.TickCount + 200;
            }
            if (isMoving)
            {
                
                switch (dir)
                {
                    case 0:
                        offsetY -= ecTime * ((float)stat[2] / 10f * 32) / 1000;
                        if (offsetY < 0) { offsetY = 0; }
                        break;

                    case 1:
                        offsetY += ecTime * ((float)stat[2] / 10f * 32) / 1000;
                        if (offsetY > 0) { offsetY = 0; }
                        break;

                    case 2:
                        offsetX -= ecTime * ((float)stat[2] / 10f * 32) / 1000;
                        if (offsetX < 0) { offsetX = 0; }
                        break;

                    case 3:
                        offsetX += ecTime * ((float)stat[2] / 10f * 32) / 1000;
                        if (offsetX > 0) { offsetX = 0; }
                        break;
                }
                if (offsetX == 0 && offsetY == 0)
                {
                    isMoving = false;
                }
            }
            else
            {
                if (local)
                {
                    if (moveDir > -1 && Globals.EventDialogs.Count == 0)
                    {
                        //Try to move
                        if (moveTimer < Environment.TickCount)
                        {
                            switch (moveDir)
                            {
                                case 0:
                                    if (!IsTileBlocked(currentX, currentY - 1, currentMap))
                                    {
                                        currentY--;
                                        dir = 0;
                                        isMoving = true;
                                        offsetY = 32;
                                        offsetX = 0;
                                    }
                                    break;
                                case 1:
                                    if (!IsTileBlocked(currentX, currentY + 1, currentMap))
                                    {
                                        currentY++;
                                        dir = 1;
                                        isMoving = true;
                                        offsetY = -32;
                                        offsetX = 0;
                                    }
                                    break;
                                case 2:
                                    if (!IsTileBlocked(currentX - 1, currentY, currentMap))
                                    {
                                        currentX--;
                                        dir = 2;
                                        isMoving = true;
                                        offsetY = 0;
                                        offsetX = 32;
                                    }
                                    break;
                                case 3:
                                    if (!IsTileBlocked(currentX + 1, currentY, currentMap))
                                    {
                                        currentX++;
                                        dir = 3;
                                        isMoving = true;
                                        offsetY = 0;
                                        offsetX = -32;
                                    }
                                    break;
                            }

                            if (isMoving)
                            {
                                moveTimer = Environment.TickCount + ((float)stat[2] / 10f);
                                didMove = true;
                                if (currentX < 0 || currentY < 0 || currentX > 29 || currentY > 29)
                                {
                                    if (tmpI != -1)
                                    {
                                        try
                                        {
                                            //At each of these cases, we have switched chunks. We need to re-number the chunk renderers.
                                            if (currentX < 0)
                                            {

                                                if (currentY < 0)
                                                {
                                                    if (Globals.localMaps[tmpI - 4] > -1)
                                                    {
                                                        currentMap = Globals.localMaps[tmpI - 4];
                                                        currentX = 29;
                                                        currentY = 29;
                                                        updateMapRenderers(0);
                                                        updateMapRenderers(2);
                                                    }
                                                }
                                                else if (currentY > 29)
                                                {
                                                    if (Globals.localMaps[tmpI + 2] > -1)
                                                    {
                                                        currentMap = Globals.localMaps[tmpI + 2];
                                                        currentX = 29;
                                                        currentY = 0;
                                                        updateMapRenderers(1);
                                                        updateMapRenderers(2);
                                                    }

                                                }
                                                else
                                                {
                                                    if (Globals.localMaps[tmpI - 1] > -1)
                                                    {
                                                        currentMap = Globals.localMaps[tmpI - 1];
                                                        currentX = 29;
                                                        updateMapRenderers(2);
                                                    }
                                                }

                                            }
                                            else if (currentX > 29)
                                            {
                                                if (currentY < 0)
                                                {
                                                    if (Globals.localMaps[tmpI - 2] > -1)
                                                    {
                                                        currentMap = Globals.localMaps[tmpI - 2];
                                                        currentX = 0;
                                                        currentY = 29;
                                                        updateMapRenderers(0);
                                                        updateMapRenderers(3);
                                                    }
                                                }
                                                else if (currentY > 29)
                                                {
                                                    if (Globals.localMaps[tmpI + 4] > -1)
                                                    {
                                                        currentMap = Globals.localMaps[tmpI + 4];
                                                        currentX = 0;
                                                        currentY = 0;
                                                        updateMapRenderers(1);
                                                        updateMapRenderers(3);
                                                    }

                                                }
                                                else
                                                {
                                                    if (Globals.localMaps[tmpI + 1] > -1)
                                                    {
                                                        currentX = 0;
                                                        currentMap = Globals.localMaps[tmpI + 1];
                                                        updateMapRenderers(3);
                                                    }
                                                }
                                            }
                                            else if (currentY < 0)
                                            {
                                                if (Globals.localMaps[tmpI - 3] > -1)
                                                {
                                                    currentY = 29;
                                                    currentMap = Globals.localMaps[tmpI - 3];
                                                    updateMapRenderers(0);
                                                }
                                            }
                                            else if (currentY > 29)
                                            {
                                                if (Globals.localMaps[tmpI + 3] > -1)
                                                {
                                                    currentY = 0;
                                                    currentMap = Globals.localMaps[tmpI + 3];
                                                    updateMapRenderers(1);
                                                }

                                            }
                                        }
                                        catch (Exception)
                                        {
                                            //player out of bounds
                                            //Debug.Log("Detected player out of bounds.");
                                        }


                                    }
                                    else
                                    {
                                        //player out of bounds
                                        //.Log("Detected player out of bounds.");
                                    }
                                }
                            }
                            else
                            {
                                if (moveDir != dir)
                                {
                                    dir = moveDir;
                                    PacketSender.SendDir(dir);
                                }
                            }
                        }
                    }
                }
            }

            if (local)
            {
                Globals.myX = currentX;
                Globals.myY = currentY;
                if (didMove)
                {
                    PacketSender.SendMove();
                }
            }
            lastUpdate = Environment.TickCount;
        }

        bool IsTileBlocked(int x, int y, int map)
        {
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
                            if (Globals.GameMaps[Globals.localMaps[tmpI - 4]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI - 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.localMaps[tmpI + 2] > -1)
                        {
                            if (Globals.GameMaps[Globals.localMaps[tmpI + 2]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI + 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.localMaps[tmpI - 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.localMaps[tmpI - 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI - 1];
                        }
                        else
                        {
                            return true;
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
                            if (Globals.GameMaps[Globals.localMaps[tmpI - 2]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI - 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.localMaps[tmpI + 4] > -1)
                        {
                            if (Globals.GameMaps[Globals.localMaps[tmpI + 4]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI + 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.localMaps[tmpI + 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.localMaps[tmpI + 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Globals.localMaps[tmpI + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (y < 0)
                {
                    tmpX = x;
                    tmpY = 29 - (y * -1);
                    if (Globals.localMaps[tmpI - 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.localMaps[tmpI - 3]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;

                        }
                        tmpMap = Globals.localMaps[tmpI - 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (y > 29)
                {
                    tmpX = x;
                    tmpY = y - 29;
                    if (Globals.localMaps[tmpI + 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.localMaps[tmpI + 3]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                        tmpMap = Globals.localMaps[tmpI + 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                    if (Globals.localMaps[tmpI] > -1)
                    {
                        if (Globals.GameMaps[Globals.localMaps[tmpI]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                        tmpMap = Globals.localMaps[tmpI];
                    }
                    else
                    {
                        return true;
                    }
                }
                for (int i = 0; i < Globals.entities.Count; i++)
                {
                    if (i != myIndex)
                    {
                        if (Globals.entities[i] != null)
                        {
                            if (Globals.entities[i].currentMap == tmpMap && Globals.entities[i].currentX == tmpX && Globals.entities[i].currentY == tmpY && Globals.entities[i].passable == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                for (int i = 0; i < Globals.events.Count; i++)
                {
                    if (Globals.events[i] != null)
                    {
                        if (Globals.events[i].currentMap == tmpMap && Globals.events[i].currentX == tmpX && Globals.events[i].currentY == tmpY && Globals.events[i].passable == 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return true;

            }

        }

        private void updateMapRenderers(int dir)
        {
            if (!isLocal)
            {
                return;
            }
            if (dir == 2)
            {
                if (Globals.localMaps[3] > -1)
                {
                    Globals.currentMap = Globals.localMaps[3];
                    Globals.localMaps[2] = Globals.localMaps[1];
                    Globals.localMaps[1] = Globals.localMaps[0];
                    Globals.localMaps[0] = -1;
                    Globals.localMaps[5] = Globals.localMaps[4];
                    Globals.localMaps[4] = Globals.localMaps[3];
                    Globals.localMaps[3] = -1;
                    Globals.localMaps[8] = Globals.localMaps[7];
                    Globals.localMaps[7] = Globals.localMaps[6];
                    Globals.localMaps[6] = -1;
                    Globals.currentMap = Globals.localMaps[4];
                    currentMap = Globals.localMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
            else if (dir == 3)
            {
                if (Globals.localMaps[5] > -1)
                {
                    Globals.currentMap = Globals.localMaps[5];
                    Globals.localMaps[0] = Globals.localMaps[1];
                    Globals.localMaps[1] = Globals.localMaps[2];
                    Globals.localMaps[2] = -1;
                    Globals.localMaps[3] = Globals.localMaps[4];
                    Globals.localMaps[4] = Globals.localMaps[5];
                    Globals.localMaps[5] = -1;
                    Globals.localMaps[6] = Globals.localMaps[7];
                    Globals.localMaps[7] = Globals.localMaps[8];
                    Globals.localMaps[8] = -1;
                    Globals.currentMap = Globals.localMaps[4];
                    currentMap = Globals.localMaps[4];
                    PacketSender.SendEnterMap();
                }

            }
            else if (dir == 1)
            {
                if (Globals.localMaps[7] > -1)
                {
                    Globals.currentMap = Globals.localMaps[7];
                    Globals.localMaps[0] = Globals.localMaps[3];
                    Globals.localMaps[3] = Globals.localMaps[6];
                    Globals.localMaps[6] = -1;
                    Globals.localMaps[1] = Globals.localMaps[4];
                    Globals.localMaps[4] = Globals.localMaps[7];
                    Globals.localMaps[7] = -1;
                    Globals.localMaps[2] = Globals.localMaps[5];
                    Globals.localMaps[5] = Globals.localMaps[8];
                    Globals.localMaps[8] = -1;
                    Globals.currentMap = Globals.localMaps[4];
                    currentMap = Globals.localMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
            else
            {
                if (Globals.localMaps[1] > -1)
                {
                    Globals.currentMap = Globals.localMaps[1];
                    Globals.localMaps[6] = Globals.localMaps[3];
                    Globals.localMaps[3] = Globals.localMaps[0];
                    Globals.localMaps[0] = -1;
                    Globals.localMaps[7] = Globals.localMaps[4];
                    Globals.localMaps[4] = Globals.localMaps[1];
                    Globals.localMaps[1] = -1;
                    Globals.localMaps[8] = Globals.localMaps[5];
                    Globals.localMaps[5] = Globals.localMaps[2];
                    Globals.localMaps[2] = -1;
                    Globals.currentMap = Globals.localMaps[4];
                    currentMap = Globals.localMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
        }

        public void Draw(int i)
        {
            Sprite tmpSprite;
            int d = 0;
            if (Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png") >= 0)
            {
                tmpSprite = new Sprite(Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")]);
                if (tmpSprite.Texture.Size.Y / 4 > 32)
                {
                    tmpSprite.Position = new SFML.Window.Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + currentX * 32 + offsetX), (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY - ((tmpSprite.Texture.Size.Y / 4) - 32)));
                }
                else
                {
                    tmpSprite.Position = new SFML.Window.Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + currentX * 32 + offsetX), (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY));
                }
                switch (dir)
                {
                    case 0:
                        d = 3;
                        break;
                    case 1:
                        d = 0;
                        break;
                    case 2:
                        d = 1;
                        break;
                    case 3:
                        d = 2;
                        break;
                }
                tmpSprite.TextureRect = new IntRect(walkFrame * (int)tmpSprite.Texture.Size.X / 4, d * (int)tmpSprite.Texture.Size.Y / 4, (int)tmpSprite.Texture.Size.X / 4, (int)tmpSprite.Texture.Size.Y / 4);

                
                Graphics.renderWindow.Draw(tmpSprite);

            }
        }

        //returns the point on the screen that is the center of the player sprite
        public SFML.Window.Vector2f getCenterPos(int mapPos)
        {
            Sprite tmpSprite;
            SFML.Window.Vector2f pos;
            if (Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png") >= 0)
            {
                tmpSprite = new Sprite(Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")]);
                pos = new SFML.Window.Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(mapPos,true) + currentX * 32 + offsetX + tmpSprite.Texture.Size.X / 8f), (int)Math.Ceiling(Graphics.CalcMapOffsetY(mapPos,true) + currentY * 32 + offsetY - ((tmpSprite.Texture.Size.Y / 4) - 32) + tmpSprite.Texture.Size.Y / 8f)) ;
            }
            else
            {
                pos =  new SFML.Window.Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(mapPos,true) + currentX * 32 + offsetX), (int)Math.Ceiling(Graphics.CalcMapOffsetY(mapPos,true) + currentY * 32 + offsetY - 32));
            }
            return pos;
        }

        public void DrawName(int i, bool isEvent)
        {
            if (hideName == 1) { return; }
            SFML.Graphics.Text nameText = new SFML.Graphics.Text(myName, Graphics.GameFont);
            int y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY);
            int x = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + currentX * 32 + offsetX) + 16;
            if (Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY - ((Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 14;
                }
                if (Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
                }
            }
            if (!isEvent) { y -= 8; } //Need room for HP bar if not an event.
            nameText.CharacterSize = 10;
            nameText.Position = new SFML.Window.Vector2f(x - nameText.GetLocalBounds().Width / 2, y);
            Graphics.renderWindow.Draw(nameText);
        }

        public void DrawHPBar(int i)
        {
            int width = 32;
            SFML.Graphics.RectangleShape bgRect = new RectangleShape(new SFML.Window.Vector2f(width, 6));
            SFML.Graphics.RectangleShape fgRect = new RectangleShape(new SFML.Window.Vector2f(width-2, 4));
            int y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY) + 6;
            int x = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + currentX * 32 + offsetX) + 16;
            if (Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + currentY * 32 + offsetY - ((Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 8;
                }
                if (Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.entities[Graphics.entityNames.IndexOf(mySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
                }
            }
            bgRect.FillColor = Color.Black;
            fgRect.FillColor = Color.Red;
            fgRect.Size = new SFML.Window.Vector2f((float)Math.Ceiling((1f * vital[0] / maxVital[0]) * (width-2)), 4f);
            bgRect.Position = new SFML.Window.Vector2f(x - 1 - width/2, y - 1);
            fgRect.Position = new SFML.Window.Vector2f(x - width/2, y);
            Graphics.renderWindow.Draw(bgRect);
            Graphics.renderWindow.Draw(fgRect);
        }
    }
}
