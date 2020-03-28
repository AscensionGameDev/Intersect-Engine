using System;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct ItemInfo
    {

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }

        public bool BankOverflow { get; set; }

    }

}
