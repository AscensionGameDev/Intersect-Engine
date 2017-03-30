using System.Collections.Generic;
using System.Linq;
using Intersect;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class QuestBase : DatabaseObject
    {
        //General
        public new const string DATABASE_TABLE = "quests";
        public new const GameObject OBJECT_TYPE = GameObject.Quest;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        //Requirements
        public int ClassReq = 0;
        public string EndDesc = "";
        public int ItemReq = 0;
        public int LevelReq = 0;

        public string Name = "New Quest";
        public int QuestReq = 0;
        public string StartDesc = "";
        public int SwitchReq = 0;

        //Tasks
        public List<QuestTask> Tasks = new List<QuestTask>();
        public int VariableReq = 0;
        public int VariableValue = 0;

        public QuestBase(int id) : base(id)
        {
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
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
                QuestTask Q = new QuestTask()
                {
                    Objective = myBuffer.ReadInteger(),
                    Desc = myBuffer.ReadString(),
                    Data1 = myBuffer.ReadInteger(),
                    Data2 = myBuffer.ReadInteger(),
                    Experience = myBuffer.ReadInteger()
                };
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    Q.Rewards[n].ItemNum = myBuffer.ReadInteger();
                    Q.Rewards[n].Amount = myBuffer.ReadInteger();
                }
                Tasks.Add(Q);
            }

            myBuffer.Dispose();
        }

        public byte[] QuestData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(StartDesc);
            myBuffer.WriteString(EndDesc);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(ItemReq);
            myBuffer.WriteInteger(LevelReq);
            myBuffer.WriteInteger(QuestReq);
            myBuffer.WriteInteger(SwitchReq);
            myBuffer.WriteInteger(VariableReq);
            myBuffer.WriteInteger(VariableValue);

            myBuffer.WriteInteger(Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                myBuffer.WriteInteger(Tasks[i].Objective);
                myBuffer.WriteString(Tasks[i].Desc);
                myBuffer.WriteInteger(Tasks[i].Data1);
                myBuffer.WriteInteger(Tasks[i].Data2);
                myBuffer.WriteInteger(Tasks[i].Experience);
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].ItemNum);
                    myBuffer.WriteInteger(Tasks[i].Rewards[n].Amount);
                }
            }

            return myBuffer.ToArray();
        }

        public static QuestBase GetQuest(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (QuestBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((QuestBase) Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return QuestData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, QuestBase> GetObjects()
        {
            Dictionary<int, QuestBase> objects = Objects.ToDictionary(k => k.Key, v => (QuestBase) v.Value);
            return objects;
        }

        public class QuestTask
        {
            public int Data1 = 0;
            public int Data2 = 0;
            public string Desc = "";
            public int Experience = 0;
            public int Objective = 0;
            public List<QuestReward> Rewards = new List<QuestReward>();

            public QuestTask()
            {
                for (int i = 0; i < Options.MaxNpcDrops; i++)
                {
                    Rewards.Add(new QuestReward());
                }
            }
        }

        public class QuestReward
        {
            public int Amount = 0;
            public int ItemNum = 0;
        }
    }
}