using System;
using System.Threading.Tasks;
using API.Configurations.Factories;
using Microsoft.AspNetCore.Http;
using MySql.Data;

namespace API.Configurations.Middlewares
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IDatabaseFactory databaseFactory)
        {
            await databaseFactory.OpenConnectionAsync();

            await databaseFactory.BeginTransactionAsync();   
            
            try
            {
                await _next(context);

                databaseFactory.CommitTransaction();
            }
            catch
            {
                databaseFactory.RollbackTransaction();

                throw;
            }
            finally
            {
                databaseFactory.CloseConnection();
            }
        }
    }
}