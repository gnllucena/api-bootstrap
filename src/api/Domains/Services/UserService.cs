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
            throw new System.NotImplementedException();
        }
        
        public async Task<User> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);

            var existsEmail = await _sqlService.ExistsAsync(UserQuery.EXISTS_DOCUMENT, new 
            {
                Document = user.Document
            });

            if (existsEmail) 
            {
                this._validationService.Throw("Document", "There is already other user with that document", user.Document, Validation.UserRepeatedDocument);
            }

            var existsDocument = await _sqlService.ExistsAsync(UserQuery.EXISTS_EMAIL, new 
            {
                Email = user.Email
            });

            if (existsDocument) 
            {
                this._validationService.Throw("Email", "There is already other user with that email", user.Email, Validation.UserRepeatedDocument);
            }

            user.Id = await _sqlService.CreateAsync(UserQuery.INSERT, new {
                IdProfile = user.Profile,
                IdCountry = user.Country,
                CreatedBy = _authenticatedService.Token().Subject,
                Name = user.Name,
                Document = user.Document,
                Birthdate = user.Birthdate,
                Active = user.Active
            });

            return user;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task ActivateDeactivateAsync(int id, bool active)
        {
            throw new System.NotImplementedException();
        }
    }
}
