using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib
{
    public struct BytePoint
    {
        public byte X;
        public byte Y;

        public BytePoint(byte x, byte y)
        {
            X = x;
            Y = y;
        }
    }
}
