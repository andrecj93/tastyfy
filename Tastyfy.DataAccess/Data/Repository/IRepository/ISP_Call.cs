using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace Tastyfy.DataAccess.Data.Repository.IRepository
{
    public interface ISpCall : IDisposable
    {
        IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters parameters = null);

        void ExecuteWithoutReturn(string procedureName, DynamicParameters parameters = null);
        
        T ExecuteReturnScalar<T>(string procedureName, DynamicParameters parameters = null);
    }
}
