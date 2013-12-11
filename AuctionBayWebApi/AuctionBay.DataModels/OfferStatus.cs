using System;
using System.Linq;

namespace AuctionBay.DataModels
{
    public enum OfferStatus
    {
        Initialized,
        Closed,
        Cancelled,
        Finalized,
        Successful
    }
}
