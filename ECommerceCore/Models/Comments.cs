using System;
using System.Collections.Generic;

namespace ECommerceCore.Models
{
    public partial class Comments
    {
        public int CommentId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public byte[] CommentImage { get; set; }

        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
    }
}
