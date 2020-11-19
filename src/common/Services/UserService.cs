using Common.Domain.Entities;
using Common.Domain.Models.Events;
using Common.Domain.Models.Responses;
using Common.Factories;
using Common.Models.Options;
using Common.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface IUserService
    {
        Task<User> InsertAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
        Task<User> GetAsync(int id);
        Task<Pagination<User>> PaginateAsync(int offset, int limit, int? id, string name, string email, string password, bool? active, bool? confirmed, DateTime? fromCreated, DateTime? toCreated, string createdBy, DateTime? fromUpdated, DateTime? toUpdated, string updatedBy);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IAuthenticatedService _authenticatedService;
        private readonly IValidator<User> _userValidator;
        private readonly IValidationService _validationService;
        private readonly IUserRepository _userRepository;
        private readonly IMessagingFactory _messagingFactory;
        private readonly ICacheFactory _cacheFactory;
        private readonly IMessagingService _messagingService;
        private readonly ICacheService _cacheService;
        private readonly Messaging _messaging;

        public UserService(
            ILogger<UserService> logger,
            IAuthenticatedService authenticatedService,
            IValidator<User> userValidator,
            IValidationService validationService,
            IUserRepository userRepository,
            IMessagingFactory messagingFactory,
            ICacheFactory cacheFactory,
            IMessagingService messagingService,
            ICacheService cacheService,
            IOptions<Messaging> messaging)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticatedService = authenticatedService ?? throw new ArgumentNullException(nameof(authenticatedService));
            _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _messagingFactory = messagingFactory ?? throw new ArgumentNullException(nameof(messagingFactory));
            _cacheFactory = cacheFactory ?? throw new ArgumentNullException(nameof(cacheFactory));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _messaging = messaging.Value ?? throw new ArgumentNullException(nameof(messaging));
        }

        public async Task<User> InsertAsync(User user)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is starting {nameof(UserService)}.{nameof(InsertAsync)}");

            user.Created = DateTime.Now;
            user.CreatedBy = _authenticatedService.GetUserName();

            _validationService.Validate(_userValidator.Validate(user), new List<(bool, string, object, string)>
            {
                (await _userRepository.ExistsByEmailAsync(user.Email), "Email", user.Email, $"Email {user.Email} already in use")
            });

            var id = await _userRepository.InsertAsync(user);

            var newUser = await GetAsync(id);

            MessageBroker(newUser);
            await CacheAsync("NEWUSER", newUser, false, false);

            _logger.LogDebug($"End of {nameof(UserService)}.{nameof(InsertAsync)}");

            return newUser;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is starting {nameof(UserService)}.{nameof(UpdateAsync)}");

            user.Id = id;
            user.Updated = DateTime.Now;
            user.UpdatedBy = _authenticatedService.GetUserName();

            _validationService.Validate(_userValidator.Validate(user), new List<(bool, string, object, string)>
            {
                (await _userRepository.ExistsByEmailAndDifferentThanIdAsync(user.Email, id), "Email", user.Email, $"Email {user.Email} already in use")
            });

            await _userRepository.UpdateAsync(id, user);

            var updatedUser = await GetAsync(id);

            await CacheAsync("UPDATEDUSER", updatedUser, false, false);

            _logger.LogDebug($"End of {nameof(UserService)}.{nameof(UpdateAsync)}");

            return updatedUser;
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is starting {nameof(UserService)}.{nameof(DeleteAsync)}");

            await _userRepository.DeleteAsync(id);

            await CacheAsync("NEWUSER", id, false, true);
            await CacheAsync("UPDATEDUSER", id, false, true);

            _logger.LogDebug($"End of {nameof(UserService)}.{nameof(DeleteAsync)}");
        }

        public async Task<User> GetAsync(int id)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is starting {nameof(UserService)}.{nameof(GetAsync)}");

            var user = await _userRepository.GetAsync(id);

            _logger.LogDebug($"End of {nameof(UserService)}.{nameof(GetAsync)}");

            return user;
        }

        public async Task<Pagination<User>> PaginateAsync(int offset, int limit, int? id, string name, string email, string password, bool? active, bool? confirmed, DateTime? fromCreated, DateTime? toCreated, string createdBy, DateTime? fromUpdated, DateTime? toUpdated, string updatedBy)
        {
            _logger.LogDebug($"User {_authenticatedService.GetUserKey()} is starting {nameof(UserService)}.{nameof(PaginateAsync)}");

            if (limit > 100)
            {
                limit = 100;
            }

            var pagination = await _userRepository.PaginateAsync(offset, limit, id, name, email, password, active, confirmed, fromCreated, toCreated, createdBy, fromUpdated, toUpdated, updatedBy);

            foreach (var paginated in pagination.Itens)
            {
                await CacheAsync("PAGINATEDUSERS", paginated, true, false);
            }

            _logger.LogDebug($"End of {nameof(UserService)}.{nameof(PaginateAsync)}");

            return pagination;
        }

        private void MessageBroker(User user)
        {
            var newUserEvent = new NewUserEvent()
            {
                Id = user.Id
            };

            try
            {
                var model = _messagingFactory.Configure();

                _messagingService.Queue(_messaging.Publishing.Exchange.Name, _messaging.Publishing.Routingkey, newUserEvent);
            }
            finally
            {
                _messagingFactory.Disconnect();
            }
        }

        private async Task CacheAsync(string key, object value, bool isList, bool isDelete)
        {
            try
            {
                await _cacheFactory.ConnectAsync();

                // don't ever do something like this

                if (isDelete)
                {
                    await _cacheService.RemoveAsync(key);
                }
                else
                {
                    if (isList)
                    {
                        await _cacheService.AddListAsync(key, value, DateTime.Now.AddDays(1));
                    }
                    else
                    {
                        await _cacheService.AddSingleAsync(key, value, DateTime.Now.AddDays(1));
                    }
                }
            }
            finally
            {
                await _cacheFactory.DisconnectAsync();
            }
        }
    }
}
