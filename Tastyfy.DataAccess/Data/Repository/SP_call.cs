using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tastyfy.DataAccess.Data.Repository.IRepository;

namespace Tastyfy.DataAccess.Data.Repository
{
    class SpCall : ISpCall
    {
        private readonly ApplicationDbContext _db;
        private static string _connectionString = "";

        public SpCall(ApplicationDbContext db)
        {
            _db = db;
            _connectionString = db.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return sqlConnection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public void ExecuteWithoutReturn(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            sqlConnection.Execute(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public T ExecuteReturnScalar<T>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return (T)Convert.ChangeType(sqlConnection.ExecuteScalar<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure), typeof(T));
        }
    }
}
