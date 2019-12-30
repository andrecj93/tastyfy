using System;
using System.Collections.Generic;
using System.Text;

namespace Tastyfy.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        
        IFoodTypeRepository FoodType { get; }
        
        IMenuItemRepository MenuItem { get; }

        IApplicationUserRepostitory ApplicationUser { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }

        ISpCall SpCall { get; }

        void Save();

    } 
}
