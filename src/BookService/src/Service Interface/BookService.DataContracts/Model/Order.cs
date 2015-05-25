using System;
using System.Collections.Generic;

namespace BookService.DataContracts.Model
{
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public int OrderID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
