using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Orders
    {
        public int OrderReferenceId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDateTime { get; set; }
        public int OrderId { get; set; }

        public virtual Carts Cart { get; set; }
        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
    }
}
