using System.Threading.Tasks;
using API.Domains.Models;
using API.Domains.Models.Faults;
using API.Domains.Queries;
using FluentValidation;

namespace API.Domains.Services
{
    public interface IUserService
    {
        Task<Pagination<User>> ListAsync(int offset, int limit);
        Task<User> GetAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
        Task ActivateDeactivateAsync(int id, bool active);
    }

    public class UserService : IUserService
    {
        private readonly ISqlService _sqlService;
        private readonly IValidator<User> _userValidator;
        private readonly IValidationService _validationService;
        private readonly IAuthenticatedService _authenticatedService;

        public UserService(
             IValidator<User> userValidator,
             ISqlService sqlService,
             IValidationService validationService,
             IAuthenticatedService authenticatedService) 
        {
            _userValidator = userValidator;
            _sqlService = sqlService;
            _validationService = validationService;
            _authenticatedService = authenticatedService;
        }

        public async Task<Pagination<User>> ListAsync(int offset, int limit)
        {
            if (limit - offset > 100) 
            {
                this._validationService.Throw("Limit", "Too much items for pagination", limit, Validation.PaginationExceedsLimits);
            }

            var list = await _sqlService.ListAsync<User>(UserQuery.PAGINATE, new 
            {
                CreatedBy = _authenticatedService.Token().Subject,
                Offset = offset,
                Limit = limit
            });

            var total = await _sqlService.CountAsync(UserQuery.TOTAL, new 
            {
                CreatedBy = _authenticatedService.Token().Subject,
            });

            var pagination = new Pagination<User>() 
            {
                Offset = offset,
                Limit = limit,
                Items = list,
                Total = total
            };

            return pagination;
        }
        
        public async Task<User> GetAsync(int id)
        {
            return await _sqlService.GetAsync<User>(UserQuery.GET, new 
            {
                Id = id,
                CreatedBy = _authenticatedService.Token().Subject
            });
        }

        public async Task<User> CreateAsync(User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);

            var existsDocument = await _sqlService.ExistsAsync(UserQuery.EXISTS_DOCUMENT, new 
            {
                Document = user.Document
            });

            if (existsDocument) 
            {
                this._validationService.Throw("Document", "There is already another user with that document", user.Document, Validation.UserRepeatedDocument);
            }

            var existsEmail = await _sqlService.ExistsAsync(UserQuery.EXISTS_EMAIL, new 
            {
                Email = user.Email
            });

            if (existsEmail) 
            {
                this._validationService.Throw("Email", "There is already another user with that email", user.Email, Validation.UserRepeatedEmail);
            }

            user.Id = await _sqlService.CreateAsync(UserQuery.INSERT, new 
            {
                IdProfile = user.Profile,
                IdCountry = user.Country,
                CreatedBy = _authenticatedService.Token().Subject,
                Name = user.Name,
                Email = user.Email,
                Document = user.Document,
                Birthdate = user.Birthdate,
                Active = user.Active
            });

            return user;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);

            var existsEmail = await _sqlService.ExistsAsync(UserQuery.EXISTS_SAME_EMAIL, new 
            {
                Id = id,
                Email = user.Email
            });

            if (existsEmail) 
            {
                this._validationService.Throw("Email", "There is already another user with that email", user.Email, Validation.UserRepeatedDocument);
            }

            await _sqlService.ExecuteAsync(UserQuery.UPDATE, new 
            {
                Id = id,
                IdProfile = user.Profile,
                IdCountry = user.Country,
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Active = user.Active
            });

            user.Id = id;

            return user;
        }

        public async Task DeleteAsync(int id)
        {
            await _sqlService.ExecuteAsync(UserQuery.DELETE, new 
            {
                Id = id,
                CreatedBy = _authenticatedService.Token().Subject
            });
        }

        public async Task ActivateDeactivateAsync(int id, bool active)
        {
            await _sqlService.ExecuteAsync(UserQuery.ACTIVATE_DEACTIVATE, new 
            {
                Id = id,
                Active = active,
                CreatedBy = _authenticatedService.Token().Subject
            });
        }
    }
}
