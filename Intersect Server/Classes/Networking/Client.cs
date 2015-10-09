/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Intersect_Server.Classes
{
    public class Client
    {

        //Game Incorperation Variables
        public int ClientIndex;
        public int EntityIndex;
        public Player Entity;

        //Ping Values
        public long ConnectionTimeout;
        public long TimeoutLength = 10;
        public long PingTime = 0;

        //Client Properties
        public bool IsEditor;
        public int Power = 0;

        //Database ID
        public int Id = -1;

        //Network Variables
        private GameSocket mySocket;

        public Client(int myIndex, int entIndex, GameSocket socket)
        {
            mySocket = socket;
            ClientIndex = myIndex;
            EntityIndex = entIndex;
            if (EntityIndex > -1) { Entity = (Player)Globals.Entities[EntityIndex]; }
            if (mySocket.IsConnected()) { PacketSender.SendPing(this); }
        }

        public void SendPacket(byte[] packet)
        {
            mySocket.SendData(packet);
        }

        public bool IsConnected()
        {
            return mySocket.IsConnected();
        }
    }
}

