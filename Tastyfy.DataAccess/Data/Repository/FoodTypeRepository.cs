using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class FoodTypeRepository : Repository<FoodType>, IFoodTypeRepository
    {
        public readonly ApplicationDbContext DbContext;
        public FoodTypeRepository(ApplicationDbContext context) : base(context)
        {
            DbContext = context;
        }

        public IEnumerable<SelectListItem> GetFoodTypeListItems()
        {
            return DbContext.FoodTypes.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(FoodType foodType)
        {
            var foodTypeInDb = DbContext.FoodTypes.FirstOrDefault(c => c.Id == foodType.Id);

            foodTypeInDb.Name = foodType.Name;

            DbContext.SaveChanges();
        }
    }
}
