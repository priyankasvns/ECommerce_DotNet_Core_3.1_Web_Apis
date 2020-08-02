using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Carts
    {
        public Carts()
        {
            Orders = new HashSet<Orders>();
        }

        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int Quantity { get; set; }
        public DateTime TimeAdded { get; set; }

        public virtual Addresses Address { get; set; }
        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
