using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class OrderHeaderRepostirory : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public readonly ApplicationDbContext _db;

        public OrderHeaderRepostirory(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }
        public void Udpate(OrderHeader orderHeader)
        {
            var orderHeaderFomDb = _db.OrderHeader.FirstOrDefault(m => m.Id == orderHeader.Id);

            _db.OrderHeader.Update(orderHeaderFomDb ?? throw new InvalidOperationException());

            _db.SaveChanges();
        }
    }
}
