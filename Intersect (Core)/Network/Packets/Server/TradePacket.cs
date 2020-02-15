﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class TradePacket: CerasPacket
    {
        public string TradePartner { get; set; }

        public TradePacket(string partnerName)
        {
            TradePartner = partnerName;
        }
    }
}
