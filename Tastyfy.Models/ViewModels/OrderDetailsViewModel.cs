using System;
using System.Collections.Generic;
using System.Text;

namespace Tastyfy.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetailsList { get; set; }
    }
}
