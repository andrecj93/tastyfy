using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category> , ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _context.Categories.OrderBy(c=>c.DisplayOrder).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            var categoryInDb = _context.Categories.FirstOrDefault(s => s.Id == category.Id);

            categoryInDb.Name = category.Name;
            categoryInDb.DisplayOrder = category.DisplayOrder;
            
            _context.SaveChanges();
        }
    }
}
