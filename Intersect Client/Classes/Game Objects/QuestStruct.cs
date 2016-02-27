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
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
{
    public class QuestStruct
    {
        //General
        public const string Version = "0.0.0.1";
        public string Name = "";
        public string StartDesc = "";
        public string EndDesc = "";

        //Requirements
        public int ClassReq = 0;
        public int ItemReq = 0;
        public int LevelReq = 0;
        public int QuestReq = 0;
        public int SwitchReq = 0;
        public int VariableReq = 0;
        public int VariableValue = 0;

        //Tasks
        public List<QuestTask> Tasks = new List<QuestTask>();

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Animation #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
            Name = myBuffer.ReadString();
            StartDesc = myBuffer.ReadString();
            EndDesc = myBuffer.ReadString();
            ClassReq = myBuffer.ReadInteger();
            ItemReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();
            QuestReq = myBuffer.ReadInteger();
            SwitchReq = myBuffer.ReadInteger();
            VariableReq = myBuffer.ReadInteger();
            VariableValue = myBuffer.ReadInteger();

            var MaxTasks = myBuffer.ReadInteger();
            Tasks.Clear();
            for (int i = 0; i < MaxTasks; i++)
            {
                QuestTask Q = new QuestTask();
                Q.Objective = myBuffer.ReadInteger();
                Q.Desc = myBuffer.ReadString();
                Q.Data1 = myBuffer.ReadInteger();
                Q.Data2 = myBuffer.ReadInteger();
                Q.Experience = myBuffer.ReadInteger();
                for (int n = 0; n < Constants.MaxNpcDrops; n++)
                {
                    Q.Rewards[n].ItemNum = myBuffer.ReadInteger();
                    Q.Rewards[n].Amount = myBuffer.ReadInteger();
                }
                Tasks.Add(Q);
            }

            myBuffer.Dispose();
        }

        public class QuestTask
        {
            public int Objective = 0;
            public string Desc = "";
            public int Data1 = 0;
            public int Data2 = 0;
            public int Experience = 0;
            public List<QuestReward> Rewards = new List<QuestReward>();

            public QuestTask()
            {
                for (int i = 0; i < Constants.MaxNpcDrops; i++)
                {
                    Rewards.Add(new QuestReward());
                }
            }
        }

        public class QuestReward
        {
            public int ItemNum = 0;
            public int Amount = 0;
        }
    }
}
