using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        public readonly ApplicationDbContext Db;

        public OrderDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            Db = dbContext;
        }
        public void Udpate(OrderDetails orderDetails)
        {
            var orderDetailsFromDb = Db.OrderHeader.FirstOrDefault(m => m.Id == orderDetails.Id);

            Db.OrderHeader.Update(orderDetailsFromDb ?? throw new InvalidOperationException());

            Db.SaveChanges();

        }
    }
}
