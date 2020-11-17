using Common.Domain.Entities;
using Common.Domain.Models.Responses;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("/user")]
    [SwaggerTag("User operations")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost]
        [SwaggerOperation(
           Summary = "Create a new User",
           Description = "Create a new User"
        )]
        [SwaggerResponse(201, "User created", typeof(User))]
        public async Task<ActionResult> Post(
            [SwaggerParameter("The new User")][FromBody] User user)
        {
            var newUser = await _userService.InsertAsync(user);

            return Created(new Uri($"{Request.Path}/{newUser.Id}", UriKind.Relative), newUser);
        }

        [HttpPut("{id}")]
        [SwaggerOperation
        (
          Summary = "Update User information",
          Description = "Update User information"
        )]
        [SwaggerResponse(200, "User updated", typeof(User))]
        public async Task<ActionResult> Put(
            [SwaggerParameter("The User Id")][BindRequired] int id,
            [SwaggerParameter("The User to be updated")][FromBody] User user)
        {
            var updatedUser = await _userService.UpdateAsync(id, user);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation
        (
           Summary = "Delete User",
           Description = "Delete User"
        )]
        [SwaggerResponse(204, "User deleted")]
        public async Task<ActionResult> Delete(
            [SwaggerParameter("The User Id")][BindRequired] int id)
        {
            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{id}")]
        [SwaggerOperation
        (
           Summary = "Get a single User",
           Description = "Get a single User"
        )]
        [SwaggerResponse(200, "Got User", typeof(User))]
        public async Task<ActionResult> Get(
            [SwaggerParameter("The User Id")][BindRequired] int id)
        {
            var user = await _userService.GetAsync(id);

            return Ok(user);
        }

        [HttpGet("/user")]
        [SwaggerOperation(
            Summary = "Get a paginated list of User",
            Description = "Get a paginated list of User"
        )]
        [SwaggerResponse(200, "A paginated list of User", typeof(Pagination<User>))]
        public async Task<ActionResult> List(
            [SwaggerParameter("The offset number of User")][BindRequired] int offset,
            [SwaggerParameter("The limit of User on response")][BindRequired] int limit,
            [SwaggerParameter("The User's Id")] int? id,
            [SwaggerParameter("The User's Name")] string name,
            [SwaggerParameter("The User's Email")] string email,
            [SwaggerParameter("The User's Password")] string password,
            [SwaggerParameter("The User's Active")] bool? active,
            [SwaggerParameter("The User's Confirmed")] bool? confirmed,
            [SwaggerParameter("The User's Created start date")] DateTime? fromCreated,
            [SwaggerParameter("The User's Created end date")] DateTime? toCreated,
            [SwaggerParameter("The User's CreatedBy")] string createdBy,
            [SwaggerParameter("The User's Updated start date")] DateTime? fromUpdated,
            [SwaggerParameter("The User's Updated end date")] DateTime? toUpdated,
            [SwaggerParameter("The User's UpdatedBy")] string updatedBy)
        {
            var pagination = await _userService.PaginateAsync(offset, limit, id, name, email, password, active, confirmed, fromCreated, toCreated, createdBy, fromUpdated, toUpdated, updatedBy);

            return Ok(pagination);
        }
    }
}
