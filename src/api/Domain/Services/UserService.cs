using System.Threading.Tasks;
using API.Domain.Models;
using API.Domain.Queries;
using FluentValidation;

namespace API.Domain.Services
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

        public UserService(
             IValidator<User> userValidator,
             ISqlService sqlService) 
        {
            _userValidator = userValidator;
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

            // todo:
            // validações de documento unico na base
            // validações de email unico na base

            var id = await _sqlService.Create(UserQuery.INSERT, user);

            return await this.GetAsync(id);
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
