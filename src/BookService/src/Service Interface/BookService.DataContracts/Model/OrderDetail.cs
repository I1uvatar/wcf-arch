using System;

namespace BookService.DataContracts.Model
{
    public partial class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> BookID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public virtual Book Book { get; set; }
        public virtual Order Order { get; set; }
    }
}
