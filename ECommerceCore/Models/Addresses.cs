using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Addresses
    {
        public Addresses()
        {
            Carts = new HashSet<Carts>();
        }

        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string StreetAddress { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<Carts> Carts { get; set; }
    }
}
