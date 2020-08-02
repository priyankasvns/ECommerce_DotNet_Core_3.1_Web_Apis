using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Products
    {
        public Products()
        {
            Carts = new HashSet<Carts>();
            Comments = new HashSet<Comments>();
            Orders = new HashSet<Orders>();
        }

        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<Carts> Carts { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
