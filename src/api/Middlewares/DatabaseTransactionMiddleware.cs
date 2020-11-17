using Common.Factories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class DatabaseTransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public DatabaseTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IDatabaseFactory databaseFactory)
        {
            await databaseFactory.OpenConnectionAsync();

            databaseFactory.BeginTransaction();

            try
            {
                await _next(context);

                databaseFactory.CommitTransaction();
            }
            catch (Exception ex)
            {
                databaseFactory.RollbackTransaction();

                throw ex;
            }
            finally
            {
                databaseFactory.CloseConnection();
            }
        }
    }
}
