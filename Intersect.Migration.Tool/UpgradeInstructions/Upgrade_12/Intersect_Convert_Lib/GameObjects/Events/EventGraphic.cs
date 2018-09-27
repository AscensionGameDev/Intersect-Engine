using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventGraphic
    {
        public string Filename;
        public int Height;
        public EventGraphicType Type;
        public int Width;
        public int X;
        public int Y;

        public EventGraphic()
        {
            Type = EventGraphicType.None;
            Filename = "";
            X = -1;
            Y = -1;
            Width = -1;
            Height = -1;
        }

        public void Load(ByteBuffer curBuffer)
        {
            Type = (EventGraphicType)curBuffer.ReadInteger();
            Filename = curBuffer.ReadString();
            X = curBuffer.ReadInteger();
            Y = curBuffer.ReadInteger();
            Width = curBuffer.ReadInteger();
            Height = curBuffer.ReadInteger();
        }

        public void Save(ByteBuffer curBuffer)
        {
            curBuffer.WriteInteger((int)Type);
            curBuffer.WriteString(TextUtils.SanitizeNone(Filename));
            curBuffer.WriteInteger(X);
            curBuffer.WriteInteger(Y);
            curBuffer.WriteInteger(Width);
            curBuffer.WriteInteger(Height);
        }

        public void CopyFrom(EventGraphic toCopy)
        {
            Type = toCopy.Type;
            Filename = toCopy.Filename;
            X = toCopy.X;
            Y = toCopy.Y;
            Width = toCopy.Width;
            Height = toCopy.Height;
        }
    }
}