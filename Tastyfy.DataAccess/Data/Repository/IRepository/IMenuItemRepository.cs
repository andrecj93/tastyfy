using System;
using System.Collections.Generic;
using System.Text;
using Tastyfy.Models;

namespace Tastyfy.DataAccess.Data.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        void Udpate(MenuItem menuItem);

    }
}
