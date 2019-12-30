using System;
using System.Collections.Generic;
using System.Text;
using Tastyfy.DataAccess.Data.Repository.IRepository;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ApplicationDbContext Context;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            Context = dbContext;
            SpCall = new SpCall(Context);
            Category = new CategoryRepository(Context);
            FoodType = new FoodTypeRepository(Context);
            MenuItem = new MenuItemRepository(Context);
            ApplicationUser = new ApplicationUserRepostitory(Context);
            ShoppingCart = new ShoppingCartRepository(Context);
            OrderHeader = new OrderHeaderRepostirory(Context);
            OrderDetails = new OrderDetailsRepository(Context);
        }

        public ICategoryRepository Category { get; private set; }
        public IFoodTypeRepository FoodType { get; private set; }

        public IMenuItemRepository MenuItem { get; private set; }

        public IApplicationUserRepostitory ApplicationUser { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public ISpCall SpCall { get; }


        public void Dispose()
        {
            Context.Dispose();
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
