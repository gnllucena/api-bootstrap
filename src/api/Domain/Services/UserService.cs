using System.Threading.Tasks;
using API.Domain.Models;
using FluentValidation;

namespace API.Domain.Services
{
    public interface IUserService
    {
        Task<Pagination<User>> ListAsync(int offset, int limit);
        Task<User> GetAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
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

        public Task ActivateDeactivateAsync(int id, bool active)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> CreateAsync(User user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Pagination<User>> ListAsync(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> UpdateAsync(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}
