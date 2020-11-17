using Common.Domain.Entities;
using Common.Domain.Models.Responses;
using Common.Queries;
using Common.Services;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IUserRepository
    {
        Task<int> InsertAsync(User user);
        Task UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
        Task<User> GetAsync(int id);
        Task<Pagination<User>> PaginateAsync(int offset, int limit, int? id, string name, string email, string password, bool? active, bool? confirmed, DateTime? fromCreated, DateTime? toCreated, string createdBy, DateTime? fromUpdated, DateTime? toUpdated, string updatedBy);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPasswordAsync(string password);
        Task<bool> ExistsByActiveAsync(bool active);
        Task<bool> ExistsByConfirmedAsync(bool confirmed);
        Task<bool> ExistsByCreatedAsync(DateTime created);
        Task<bool> ExistsByCreatedByAsync(string createdBy);
        Task<bool> ExistsByUpdatedAsync(DateTime updated);
        Task<bool> ExistsByUpdatedByAsync(string updatedBy);
        Task<bool> ExistsByNameAndDifferentThanIdAsync(string name, int id);
        Task<bool> ExistsByEmailAndDifferentThanIdAsync(string email, int id);
        Task<bool> ExistsByPasswordAndDifferentThanIdAsync(string password, int id);
        Task<bool> ExistsByActiveAndDifferentThanIdAsync(bool active, int id);
        Task<bool> ExistsByConfirmedAndDifferentThanIdAsync(bool confirmed, int id);
        Task<bool> ExistsByCreatedAndDifferentThanIdAsync(DateTime created, int id);
        Task<bool> ExistsByCreatedByAndDifferentThanIdAsync(string createdBy, int id);
        Task<bool> ExistsByUpdatedAndDifferentThanIdAsync(DateTime updated, int id);
        Task<bool> ExistsByUpdatedByAndDifferentThanIdAsync(string updatedBy, int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly ISqlService _sqlService;
        private readonly IAuthenticatedService _authenticatedService;

        public UserRepository(
            ILogger<UserRepository> logger,
            ISqlService sqlService,
            IAuthenticatedService authenticatedService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sqlService = sqlService ?? throw new ArgumentNullException(nameof(sqlService));
            _authenticatedService = authenticatedService ?? throw new ArgumentNullException(nameof(authenticatedService));
        }

        public async Task<int> InsertAsync(User user)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is inserting a new User - {user}");

            var parameters = new DynamicParameters();
            parameters.Add("name", user.Name, direction: ParameterDirection.Input);
            parameters.Add("email", user.Email, direction: ParameterDirection.Input);
            parameters.Add("password", user.Password, direction: ParameterDirection.Input);
            parameters.Add("active", user.Active, direction: ParameterDirection.Input);
            parameters.Add("confirmed", user.Confirmed, direction: ParameterDirection.Input);
            parameters.Add("created", user.Created, direction: ParameterDirection.Input);
            parameters.Add("createdby", user.CreatedBy, direction: ParameterDirection.Input);
            parameters.Add("updated", user.Updated, direction: ParameterDirection.Input);
            parameters.Add("updatedby", user.UpdatedBy, direction: ParameterDirection.Input);

            var id = await _sqlService.ExecuteScalarAsync<int>(UserQuery.INSERT, CommandType.Text, parameters);

            _logger.LogDebug($"User {id} inserted");

            return id;
        }

        public async Task UpdateAsync(int id, User user)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is updating User {id} - {user}");

            await _sqlService.ExecuteAsync(UserQuery.UPDATE, CommandType.Text, new
            {
                id = id,
                name = user.Name,
                email = user.Email,
                password = user.Password,
                active = user.Active,
                confirmed = user.Confirmed,
                created = user.Created,
                createdby = user.CreatedBy,
                updated = user.Updated,
                updatedby = user.UpdatedBy
            });

            _logger.LogDebug($"User {id} updated");
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is deleting User {id}");

            await _sqlService.ExecuteAsync(UserQuery.DELETE, CommandType.Text, new
            {
                id = id
            });

            _logger.LogDebug($"User {id} deleted");
        }

        public async Task<User> GetAsync(int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is getting User {id}");

            var user = await _sqlService.QueryFirstOrDefaultAsync<User>(UserQuery.GET, CommandType.Text, new
            {
                id = id
            });

            _logger.LogDebug($"Got User {id} - {user}");

            return user;
        }

        public async Task<Pagination<User>> PaginateAsync(int offset, int limit, int? id, string name, string email, string password, bool? active, bool? confirmed, DateTime? fromCreated, DateTime? toCreated, string createdBy, DateTime? fromUpdated, DateTime? toUpdated, string updatedBy)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is paginating User - offset: {offset} - limit: {limit} - id: {id} - name: {name} - email: {email} - password: {password} - active: {active} - confirmed: {confirmed} - fromCreated: {fromCreated} - toCreated: {toCreated} - createdBy: {createdBy} - fromUpdated: {fromUpdated} - toUpdated: {toUpdated} - updatedBy: {updatedBy}");

            DateTime? parsedFromCreated = null;

            if (fromCreated != null)
            {
                parsedFromCreated = fromCreated.Value.Date;
            }

            DateTime? parsedToCreated = null;

            if (toCreated != null)
            {
                parsedToCreated = toCreated.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            DateTime? parsedFromUpdated = null;

            if (fromUpdated != null)
            {
                parsedFromUpdated = fromUpdated.Value.Date;
            }

            DateTime? parsedToUpdated = null;

            if (toUpdated != null)
            {
                parsedToUpdated = toUpdated.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            var paginated = await _sqlService.QueryAsync<User>(UserQuery.PAGINATE, CommandType.Text, new
            {
                offset = offset,
                limit = limit,
                id = id,
                name = name,
                email = email,
                password = password,
                active = active,
                confirmed = confirmed,
                fromCreated = parsedFromCreated,
                toCreated = parsedToCreated,
                createdby = createdBy,
                fromUpdated = parsedFromUpdated,
                toUpdated = parsedToUpdated,
                updatedby = updatedBy
            });

            var total = await _sqlService.ExecuteScalarAsync<int>(UserQuery.PAGINATE_COUNT, CommandType.Text);

            var pagination = new Pagination<User>(paginated, offset, limit, total);

            _logger.LogDebug($"Got pagination, the informed filter has {pagination.Itens.Count()} results in database");

            return pagination;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {id} in column Id on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_ID, CommandType.Text, new
            {
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {name} in column Name on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_NAME, CommandType.Text, new
            {
                name = name
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {email} in column Email on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_EMAIL, CommandType.Text, new
            {
                email = email
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByPasswordAsync(string password)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {password} in column Password on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_PASSWORD, CommandType.Text, new
            {
                password = password
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByActiveAsync(bool active)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {active} in column Active on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_ACTIVE, CommandType.Text, new
            {
                active = active
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByConfirmedAsync(bool confirmed)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {confirmed} in column Confirmed on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CONFIRMED, CommandType.Text, new
            {
                confirmed = confirmed
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByCreatedAsync(DateTime created)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {created} in column Created on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CREATED, CommandType.Text, new
            {
                created = created
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByCreatedByAsync(string createdBy)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {createdBy} in column CreatedBy on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CREATEDBY, CommandType.Text, new
            {
                createdby = createdBy
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByUpdatedAsync(DateTime updated)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {updated} in column Updated on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_UPDATED, CommandType.Text, new
            {
                updated = updated
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByUpdatedByAsync(string updatedBy)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {updatedBy} in column UpdatedBy on User table");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_UPDATEDBY, CommandType.Text, new
            {
                updatedby = updatedBy
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByNameAndDifferentThanIdAsync(string name, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {name} in column Name on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_NAME_AND_DIFFERENT_ID, CommandType.Text, new
            {
                name = name,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByEmailAndDifferentThanIdAsync(string email, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {email} in column Email on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_EMAIL_AND_DIFFERENT_ID, CommandType.Text, new
            {
                email = email,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByPasswordAndDifferentThanIdAsync(string password, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {password} in column Password on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_PASSWORD_AND_DIFFERENT_ID, CommandType.Text, new
            {
                password = password,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByActiveAndDifferentThanIdAsync(bool active, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {active} in column Active on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_ACTIVE_AND_DIFFERENT_ID, CommandType.Text, new
            {
                active = active,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByConfirmedAndDifferentThanIdAsync(bool confirmed, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {confirmed} in column Confirmed on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CONFIRMED_AND_DIFFERENT_ID, CommandType.Text, new
            {
                confirmed = confirmed,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByCreatedAndDifferentThanIdAsync(DateTime created, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {created} in column Created on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CREATED_AND_DIFFERENT_ID, CommandType.Text, new
            {
                created = created,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByCreatedByAndDifferentThanIdAsync(string createdBy, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {createdBy} in column CreatedBy on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_CREATEDBY_AND_DIFFERENT_ID, CommandType.Text, new
            {
                createdby = createdBy,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByUpdatedAndDifferentThanIdAsync(DateTime updated, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {updated} in column Updated on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_UPDATED_AND_DIFFERENT_ID, CommandType.Text, new
            {
                updated = updated,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }

        public async Task<bool> ExistsByUpdatedByAndDifferentThanIdAsync(string updatedBy, int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is searching for a match with {updatedBy} in column UpdatedBy on User table with a different Id than {id}");

            var exists = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_UPDATEDBY_AND_DIFFERENT_ID, CommandType.Text, new
            {
                updatedby = updatedBy,
                id = id
            });

            _logger.LogDebug(exists ? "Found a match" : "No match found");

            return exists;
        }
    }
}
