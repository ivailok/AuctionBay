using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionBay.DataModels
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual User Auctioneer { get; set; }

        public DateTime TimeCreated { get; set; }

        public decimal CurrentPrice { get; set; }

        public string CurrentBidder { get; set; }

        public string BidingType { get; set; }

        public StatusList Status { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; }

        public Product()
        {
            this.ProductImages = new HashSet<ProductImage>();
        }
    }
}
