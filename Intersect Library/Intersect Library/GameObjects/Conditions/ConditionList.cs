using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect_Library.GameObjects.Events;

namespace Intersect_Library.GameObjects.Conditions
{
    public class ConditionList
    {
        public string Name = "New Condition List";
        public List<EventCommand> Conditions = new List<EventCommand>(); //Long story.. just go with it.. okay?

        public ConditionList()
        {
            
        }

        public ConditionList(ByteBuffer buff)
        {
            Load(buff);
        }

        public ConditionList(byte[] data)
        {
            Load(data);
        }

        public void Load(ByteBuffer buff)
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
            var buff = new ByteBuffer();
            buff.WriteBytes(data);
            Load(buff);
            buff.Dispose();
        }

        public void Save(ByteBuffer buff)
        {
            buff.WriteBytes(Data());
        }

        public byte[] Data()
        {
            var buff = new ByteBuffer();
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
