using System;
using System.Collections.Generic;
using System.Text;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementQuantity(ShoppingCart shoppingCart, int count);
        int DecrementQuantity(ShoppingCart shoppingCart, int count);
    }
}
