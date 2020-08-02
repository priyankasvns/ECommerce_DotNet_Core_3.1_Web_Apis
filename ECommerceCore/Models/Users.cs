using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Users
    {
        public Users()
        {
            Addresses = new HashSet<Addresses>();
            Carts = new HashSet<Carts>();
            Comments = new HashSet<Comments>();
            Orders = new HashSet<Orders>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Activated { get; set; }
        public byte[] Password { get; set; }

        public virtual ICollection<Addresses> Addresses { get; set; }
        public virtual ICollection<Carts> Carts { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
