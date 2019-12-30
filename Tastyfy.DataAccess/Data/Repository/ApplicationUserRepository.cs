using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository
{
    public class ApplicationUserRepostitory : Repository<ApplicationUser> , IApplicationUserRepostitory
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepostitory(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

       
    }

}
