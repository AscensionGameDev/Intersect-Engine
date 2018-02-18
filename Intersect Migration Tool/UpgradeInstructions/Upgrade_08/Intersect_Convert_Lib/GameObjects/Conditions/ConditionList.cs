using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Events;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Conditions
{
    public class ConditionList
    {
        public List<EventCommand> Conditions = new List<EventCommand>(); //Long story.. just go with it.. okay?
        public string Name = "New Condition List";

        public ConditionList()
        {
        }

        public ConditionList(Upgrade_10.Intersect_Convert_Lib.ByteBuffer buff)
        {
            Load(buff);
        }

        public ConditionList(byte[] data)
        {
            Load(data);
        }

        public void Load(Upgrade_10.Intersect_Convert_Lib.ByteBuffer buff)
        {
            Name = buff.ReadString();
            Conditions.Clear();
            var count = buff.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                var cmd = new EventCommand();
                cmd.Load(buff);
                Conditions.Add(cmd);
            }
        }

        public void Load(byte[] data)
        {
            var buff = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();
            buff.WriteBytes(data);
            Load(buff);
            buff.Dispose();
        }

        public void Save(Upgrade_10.Intersect_Convert_Lib.ByteBuffer buff)
        {
            buff.WriteBytes(Data());
        }

        public byte[] Data()
        {
            var buff = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();
            buff.WriteString(Name);
            buff.WriteInteger(Conditions.Count);
            for (int i = 0; i < Conditions.Count; i++)
            {
                Conditions[i].Save(buff);
            }
            return buff.ToArray();
        }
    }
}