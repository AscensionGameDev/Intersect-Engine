using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ceras;

namespace Intersect.Network.Packets
{
    public class CerasPacket : IPacket
    {
        private static Ceras sCerasInstance = new Ceras(true);

        public CerasPacket()
        {
        }


        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public byte[] Data()
        {
            return sCerasInstance.Serialize(this);
        }

    }
}
