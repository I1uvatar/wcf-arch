using System;
using System.Collections.Generic;

namespace BookService.DataContracts.Model
{
    public partial class Book
    {
        public Book()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public int BookID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public Nullable<decimal> Price { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
