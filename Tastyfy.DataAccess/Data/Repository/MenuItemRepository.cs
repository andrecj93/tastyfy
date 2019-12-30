using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        public readonly ApplicationDbContext DbContext;

        public MenuItemRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            DbContext = dbContext;
        }
        public void Udpate(MenuItem menuItem)
        {
            var menuItemFromDb = DbContext.MenuItems.FirstOrDefault(m => m.Id == menuItem.Id);

            //if (menuItemFromDb == null) return;

            menuItemFromDb.Name = menuItem.Name;
            menuItemFromDb.Description = menuItem.Description;
            menuItemFromDb.CategoryId = menuItem.CategoryId;
            menuItemFromDb.FoodTypeId = menuItem.FoodTypeId;
            menuItemFromDb.Price = menuItem.Price;

            if (menuItem.Image != null)
            {
                menuItemFromDb.Image = menuItem.Image;
            }

            DbContext.SaveChanges();
        }
    }
}
