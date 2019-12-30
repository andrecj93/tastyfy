using System;
using System.Collections.Generic;
using System.Text;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
        void Udpate(OrderDetails orderDetails);

    }
}
